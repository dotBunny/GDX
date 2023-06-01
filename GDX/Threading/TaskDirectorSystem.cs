// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace GDX.Threading
{
    public static class TaskDirectorSystem
    {
        /// <summary>
        ///     Has the task director been added to the player loop for updating at runtime.
        /// </summary>
        static bool s_AddedToPlayerLoop;

        /// <summary>
        ///     How often should the <see cref="TaskDirector"/> be ticked?
        /// </summary>
        /// <remarks>
        ///     This works by accumulation, when time between the last tick and current time exceeds
        ///     <see cref="s_TickRate"/>, a tick will be triggered. Default configured in <see cref="Config"/>.
        /// </remarks>
        static float s_TickRate = -1;

        /// <summary>
        ///     An accumulation of time since the last tick.
        /// </summary>
        static float s_TimeSinceLastTick;

        /// <summary>
        ///     Triggered after the <see cref="TaskDirectorSystem"/> has ticked, with the delta time.
        /// </summary>
        public static Action<float> ticked;

        public static void AddToPlayerLoop()
        {
            if (s_AddedToPlayerLoop || !Config.TaskDirectorSystem) return;

#if !UNITY_EDITOR
            Trace.Output(Trace.TraceLevel.Info, "GDX runtime task scheduler activated.");
#endif // !UNITY_EDITOR

            PlayerLoopSystem systemRoot = PlayerLoop.GetCurrentPlayerLoop();
            systemRoot.AddSubSystemToFirstSubSystemOfType(
                typeof(Update.ScriptRunDelayedTasks),
                typeof(TaskDirectorSystem), PlayerLoopTick);
            PlayerLoop.SetPlayerLoop(systemRoot);

            s_AddedToPlayerLoop = true;
        }

        /// <summary>
        ///     Get the current tick rate used by the <see cref="TaskDirectorSystem"/>.
        /// </summary>
        /// <returns>
        ///     A double value representing the elapsed time necessary to trigger an update to the
        ///     <see cref="TaskDirectorSystem"/>.
        /// </returns>
        public static float GetTickRate()
        {
            return s_TickRate;
        }

        public static void RemoveFromPlayerLoop()
        {
            if (!s_AddedToPlayerLoop) return;
            PlayerLoopSystem systemRoot = PlayerLoop.GetCurrentPlayerLoop();
            systemRoot.RemoveSubSystemsOfTypeFromFirstSubSystemOfType(typeof(Update.ScriptRunDelayedTasks),
                typeof(TaskDirectorSystem));
            PlayerLoop.SetPlayerLoop(systemRoot);
            s_AddedToPlayerLoop = false;
        }

        /// <summary>
        ///     Update the rate at which the <see cref="TaskDirectorSystem"/> updates the <see cref="TaskDirector"/>.
        /// </summary>
        /// <remarks>
        ///     This will not survive domain reload, please see <see cref="Config.TaskDirectorSystemTickRate"/>.
        /// </remarks>
        /// <param name="tickRate">The new tick rate.</param>
        public static void SetTickRate(float tickRate)
        {
            s_TickRate = tickRate;
#if UNITY_EDITOR
            if (s_TickRate >= 0 && !Config.TaskDirectorSystem)
            {
                Trace.Output(Trace.TraceLevel.Warning,
                    "Tick rate set whilst TaskDirectorSystem has been configured off.");
            }
#endif // UNITY_EDITOR

            // If for some reason the tick rate is set at runtime
#if !UNITY_EDITOR
            if (tickRate >= 0)
            {
                AddToPlayerLoop();
            }
            else
            {
                RemoveFromPlayerLoop();
            }
#endif // !UNITY_EDITOR
        }

        /// <summary>
        ///     Sets up some default state for the <see cref="TaskDirectorSystem"/>.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Initialize()
        {
            // Because invoke order is not deterministic, we cheat a bit and make sure that the core gets called
            // regardless from this system. This pattern probably needs to happen in other cases like this.
            Core.InitializeOnMainThread();

            if (Config.TaskDirectorSystem)
            {
                if (s_TickRate < 0)
                {
                    s_TickRate = Config.TaskDirectorSystemTickRate;
                }

                AddToPlayerLoop();
            }
        }

        /// <summary>
        ///     The internal update call to the <see cref="TaskDirector"/>.
        /// </summary>
        static void PlayerLoopTick()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Trace.Output(Trace.TraceLevel.Warning,
                    "Unable to tick Task Director from PlayerLoop outside of PlayMode.");
                return;
            }
#endif // UNITY_EDITOR

            s_TimeSinceLastTick += Time.deltaTime;
            if (s_TimeSinceLastTick < s_TickRate)
            {
                return;
            }

            TaskDirector.Tick();
            ticked?.Invoke(s_TimeSinceLastTick);
            s_TimeSinceLastTick = 0;
        }
    }
}