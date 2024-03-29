﻿// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Developer;
using Unity.Mathematics;
using UnityEditor;

namespace GDX.Editor
{
#if UNITY_2022_2_OR_NEWER
    public static class EditorConsoleSystem
    {
        /// <summary>
        ///     How often should the <see cref="Console" /> be ticked?
        /// </summary>
        /// <remarks>
        ///     This works by accumulation, when time between the last tick and current time exceeds
        ///     <see cref="k_TickRate" />, a tick will be triggered.
        /// </remarks>
        const double k_TickRate = 0.5f;

        /// <summary>
        ///     The last time a tick occured.
        /// </summary>
        static double s_LastTick;

        /// <summary>
        ///     Is the <see cref="EditorConsoleSystem" /> subscribed to <see cref="EditorApplication.update" />?
        /// </summary>
        static bool s_SubscribedToEditorApplicationUpdate;

        /// <summary>
        ///     Sets up some default state for the <see cref="EditorConsoleSystem" />.
        /// </summary>
        [InitializeOnLoadMethod]
        static void Initialize()
        {
            if (!Config.EnvironmentDeveloperConsole) return;

            SubscribeToEditorApplicationUpdate(true);

            // Always subscribe, maybe in the future we will make a monolithic UnityHooks type system.
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        /// <summary>
        ///     Method invoked by <see cref="EditorApplication.playModeStateChanged" /> when monitoring.
        /// </summary>
        /// <param name="state"></param>
        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredEditMode:
                    if (k_TickRate >= 0)
                    {
                        SubscribeToEditorApplicationUpdate(true);
                    }

                    break;
                case PlayModeStateChange.ExitingEditMode:
                    SubscribeToEditorApplicationUpdate(false);
                    break;
            }
        }

        /// <summary>
        ///     Method invoked by the <see cref="EditorApplication.update" /> callback.
        /// </summary>
        /// <remarks>
        ///     Functions much like a proxy tick system where it manages the delta and eventually triggers the
        ///     <see cref="Console.Tick" /> when the <see cref="k_TickRate" /> has been exceeded.
        /// </remarks>
        static void OnUpdate()
        {
            // We're going to avoid ticking when the editor really is actually busy
            // It's important to check greater then due to ambiguity of the precision
            if (k_TickRate < 0 ||
                EditorApplication.isCompiling ||
                EditorApplication.isUpdating ||
                EditorApplication.isPlaying)
            {
                return;
            }

            // Figure delta out
            float deltaTime = (float)math.abs(s_LastTick - EditorApplication.timeSinceStartup);

            // Handle our tick rate, ensuring the obscure rollover case is handled
            if (deltaTime < k_TickRate)
            {
                return;
            }

            // Record last tick time
            s_LastTick = EditorApplication.timeSinceStartup;

            Console.Tick(deltaTime);
        }

        /// <summary>
        ///     Sets whether the <see cref="EditorApplication.update" /> callback is subscribed too or not.
        /// </summary>
        /// <param name="subscribe">A true/false indication.</param>
        static void SubscribeToEditorApplicationUpdate(bool subscribe)
        {
            if (subscribe && !s_SubscribedToEditorApplicationUpdate && Config.EnvironmentDeveloperConsole)
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
#endif // UNITY_2022_2_OR_NEWER
}