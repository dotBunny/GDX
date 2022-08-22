// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Threading;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace GDX.Editor
{
    // TODO: Block going into playmode when there are background tasks still

    /// <summary>
    ///     An editor-only method of ticking <see cref="TaskDirector"/>.
    /// </summary>
    public static class EditorTaskDirector
    {
        /// <summary>
        ///     The last time a tick occured.
        /// </summary>
        static double s_LastTick;

        /// <summary>
        ///     A cached index provided when creating a new editor progress indicator.
        /// </summary>
        static int s_ProgressID = -1;

        /// <summary>
        ///     Is the <see cref="EditorTaskDirector"/> subscribed to <see cref="EditorApplication.update"/>?
        /// </summary>
        static bool s_SubscribedToEditorUpdate;

        /// <summary>
        ///     Should the <see cref="EditorTaskDirector"/> tick when the editor is in playmode? Default configured in
        ///     <see cref="Config"/>.
        /// </summary>
        static bool s_TickInPlayMode;

        /// <summary>
        ///     How often should the <see cref="TaskDirector"/> be ticked?
        /// </summary>
        /// <remarks>
        ///     This works by accumulation, when time between the last tick and current time exceeds
        ///     <see cref="s_TickRate"/>, a tick will be triggered. Default configured in <see cref="Config"/>.
        /// </remarks>
        static double s_TickRate = -1;

        /// <summary>
        ///     Get whether the <see cref="EditorTaskDirector"/> triggers the <see cref="TaskDirector "/> when in playmode?
        /// </summary>
        /// <returns>A true or false answer.</returns>
        public static bool GetTickInPlayMode()
        {
            return s_TickInPlayMode;
        }

        /// <summary>
        ///     Get the current tick rate used by the <see cref="EditorTaskDirector"/>.
        /// </summary>
        /// <returns>
        ///     A double value representing the elapsed time necessary to trigger an update to the
        ///     <see cref="TaskDirector"/>.
        /// </returns>
        public static double GetTickRate()
        {
            return s_TickRate;
        }

        /// <summary>
        ///     Set whether the <see cref="EditorTaskDirector"/> triggers the <see cref="TaskDirector"/> when in playmode.
        /// </summary>
        /// <remarks>
        ///     There are specific scenarios where you may want to have <see cref="TaskBase"/> that execute in playmode,
        ///     but not in a player.
        /// </remarks>
        /// <param name="shouldTick">
        ///     A true/false value indicating if it should trigger the <see cref="TaskDirector"/> in playmode.
        /// </param>
        public static void SetTickInPlayMode(bool shouldTick)
        {
            if (EditorApplication.isPlaying && !shouldTick)
            {
                EditorApplicationOnplayModeStateChanged(PlayModeStateChange.ExitingEditMode);
                EditorApplicationOnplayModeStateChanged(PlayModeStateChange.EnteredPlayMode);
            }
            s_TickInPlayMode = shouldTick;
        }

        /// <summary>
        ///     Update the rate at which the <see cref="EditorTaskDirector"/> updates the <see cref="TaskDirector"/>.
        /// </summary>
        /// <param name="tickRate">The new tick rate.</param>
        public static void SetTickRate(double tickRate)
        {
            s_TickRate = tickRate;
        }

        /// <summary>
        ///     Sets up some default state for the <see cref="EditorTaskDirector"/>.
        /// </summary>
        [InitializeOnLoadMethod]
        static void Initialize()
        {
            if (Config.EnvironmentEditorTaskDirector)
            {
                if (s_TickRate < 0)
                {
                    s_TickRate = Config.EnvironmentEditorTaskDirectorTickRate;
                }

                EditorUpdateCallback(true);
                SetTickInPlayMode(Config.EnvironmentEditorTaskDirectorTickInPlayMode);
                EditorApplication.playModeStateChanged += EditorApplicationOnplayModeStateChanged;
            }
        }

        /// <summary>
        ///     Method invoked by <see cref="EditorApplication.playModeStateChanged"/> when monitoring.
        /// </summary>
        /// <param name="state"></param>
        static void EditorApplicationOnplayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredEditMode:
                    EditorUpdateCallback(true);
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    EditorUpdateCallback(false);
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    if (s_TickInPlayMode)
                    {
                        TaskDirectorPlayerLoopSystem.Subscribe();
                    }
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    TaskDirectorPlayerLoopSystem.Unsubscribe();
                    break;
            }
        }

        /// <summary>
        ///     Method invoked by the <see cref="EditorApplication.update"/> callback.
        /// </summary>
        /// <remarks>
        ///     Functions much like a proxy tick system where it manages the delta and eventually triggers the
        ///     <see cref="TaskDirector.Tick"/> when the <see cref="s_TickRate"/> has been exceeded.
        /// </remarks>
        static void EditorUpdate()
        {
            // We're going to avoid ticking when the editor really is actually busy
            // It's important to check greater then due to ambiguity of the precision
            if (s_TickRate < Platform.DoubleTolerance ||
                EditorApplication.isCompiling ||
                EditorApplication.isUpdating ||
                (!s_TickInPlayMode && EditorApplication.isPlaying))

            {
                return;
            }

            // Handle our tick rate, ensuring the obscure rollover case is handled
            if (math.abs(EditorApplication.timeSinceStartup - s_LastTick) < s_TickRate)
            {
                return;
            }

            // Record last tick time
            s_LastTick = EditorApplication.timeSinceStartup;

            // Update the task director
            TaskDirector.Tick();

            // Update background worker
            int busyCount = TaskDirector.GetBusyCount();
            int waitingCount = TaskDirector.GetBusyCount();

            if (busyCount > 0 || waitingCount > 0)
            {
                // Create our progress indicator if we don't have one
                if (s_ProgressID == -1)
                {
                    s_ProgressID = Progress.Start("Task Director", "Executing Tasks ...", Progress.Options.Indefinite);
                }

                // Update the description on the progress indicator
                Progress.SetDescription(s_ProgressID, $"{busyCount} Working / {waitingCount} Waiting");
            }
            else if (s_ProgressID != -1)
            {
                // Remove the progress indicator
                Progress.Finish(s_ProgressID);
                s_ProgressID = -1;
            }
        }

        /// <summary>
        ///     Sets whether the <see cref="EditorApplication.update"/> callback is subscribed too or not.
        /// </summary>
        /// <param name="subscribe">A true/false indication.</param>
        static void EditorUpdateCallback(bool subscribe)
        {
            if (subscribe && !s_SubscribedToEditorUpdate)
            {
                EditorApplication.update += EditorUpdate;
                s_SubscribedToEditorUpdate = true;
            }
            else if (!subscribe && s_SubscribedToEditorUpdate)
            {
                EditorApplication.update -= EditorUpdate;
                s_SubscribedToEditorUpdate = false;
            }
        }
    }
}