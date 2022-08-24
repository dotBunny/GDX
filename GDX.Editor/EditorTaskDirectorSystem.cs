// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Threading;
using Unity.Mathematics;
using UnityEditor;

namespace GDX.Editor
{
    // TODO: Block going into playmode when there are background tasks still

    /// <summary>
    ///     An editor-only method of ticking <see cref="TaskDirector"/>.
    /// </summary>
    public static class EditorTaskDirectorSystem
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
        ///     Is the <see cref="EditorTaskDirectorSystem"/> subscribed to <see cref="EditorApplication.update"/>?
        /// </summary>
        static bool s_SubscribedToEditorApplicationUpdate;

        /// <summary>
        ///     How often should the <see cref="TaskDirector"/> be ticked?
        /// </summary>
        /// <remarks>
        ///     This works by accumulation, when time between the last tick and current time exceeds
        ///     <see cref="s_TickRate"/>, a tick will be triggered. Default configured in <see cref="Config"/>.
        /// </remarks>
        static double s_TickRate = -1;

        /// <summary>
        ///     Triggered after the <see cref="EditorTaskDirectorSystem"/> has ticked.
        /// </summary>
        public static System.Action ticked;

        /// <summary>
        ///     Get the current tick rate used by the <see cref="EditorTaskDirectorSystem"/>.
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
        ///     Update the rate at which the <see cref="EditorTaskDirectorSystem"/> updates the <see cref="TaskDirector"/>.
        /// </summary>
        /// <remarks>
        ///     Setting the tick rate is temporary in comparison to setting the actual value in the GDX config.
        /// </remarks>
        /// <param name="tickRate">The new tick rate.</param>
        public static void SetTickRate(double tickRate)
        {
            s_TickRate = tickRate;
#if UNITY_EDITOR
            if (s_TickRate >= 0 && !Config.EnvironmentEditorTaskDirector)
            {
                Trace.Output(Trace.TraceLevel.Warning,
                    "Tick rate set whilst EditorTaskDirectorSystem has been configured off.");
            }
#endif
            SubscribeToEditorApplicationUpdate(tickRate >= 0);
        }

        /// <summary>
        ///     Sets up some default state for the <see cref="EditorTaskDirectorSystem"/>.
        /// </summary>
        [InitializeOnLoadMethod]
        static void Initialize()
        {
            // Default tick rate if enabled
            if (Config.EnvironmentEditorTaskDirector && s_TickRate < 0)
            {
                SetTickRate(Config.EnvironmentEditorTaskDirectorTickRate);
            }

            // Always subscribe, maybe in the future we will make a monolithic UnityHooks type system.
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        /// <summary>
        ///     Method invoked by <see cref="EditorApplication.playModeStateChanged"/> when monitoring.
        /// </summary>
        /// <param name="state"></param>
        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredEditMode:
                    if (s_TickRate >= 0)
                    {
                        SubscribeToEditorApplicationUpdate(true);
                    }
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    SubscribeToEditorApplicationUpdate(false);
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    TaskDirectorSystem.AddToPlayerLoop();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    TaskDirectorSystem.RemoveFromPlayerLoop();
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
        static void OnUpdate()
        {
            // We're going to avoid ticking when the editor really is actually busy
            // It's important to check greater then due to ambiguity of the precision
            if (s_TickRate < 0 ||
                EditorApplication.isCompiling ||
                EditorApplication.isUpdating ||
                EditorApplication.isPlaying)
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

            // Trigger any subscribers
            ticked?.Invoke();
        }

        /// <summary>
        ///     Sets whether the <see cref="EditorApplication.update"/> callback is subscribed too or not.
        /// </summary>
        /// <param name="subscribe">A true/false indication.</param>
        static void SubscribeToEditorApplicationUpdate(bool subscribe)
        {
            if (subscribe && !s_SubscribedToEditorApplicationUpdate && Config.EnvironmentEditorTaskDirector)
            {
                EditorApplication.update += OnUpdate;
                s_SubscribedToEditorApplicationUpdate = true;
            }
            else if (!subscribe && s_SubscribedToEditorApplicationUpdate)
            {
                EditorApplication.update -= OnUpdate;
                s_SubscribedToEditorApplicationUpdate = false;
            }
        }
    }
}