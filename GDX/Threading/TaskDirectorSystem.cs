// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

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
        static float s_TimeSinceLastTick = 0;

        public static void AddToPlayerLoop()
        {
            if (s_AddedToPlayerLoop) return;

            PlayerLoopSystem systemRoot = PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopSystem taskDirectorSystem = new PlayerLoopSystem()
            {
                updateDelegate = PlayerLoopTick, type = typeof(TaskDirectorSystem)
            };
            systemRoot.AddSubSystemToFirstSubSystemOfType(typeof(Update.ScriptRunDelayedTasks), ref taskDirectorSystem);
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
        /// <param name="tickRate">The new tick rate.</param>
        public static void SetTickRate(float tickRate)
        {
            s_TickRate = tickRate;
        }

        /// <summary>
        ///     Sets up some default state for the <see cref="TaskDirectorSystem"/>.
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            if (Config.EnvironmentEditorTaskDirector)
            {
                if (s_TickRate < 0)
                {
                    s_TickRate = 0.1f; // TODO: COnfig
                }
            }
        }

        static void PlayerLoopTick()
        {
            if (!Application.isPlaying) { return; }

            s_TimeSinceLastTick += Time.deltaTime;
            if (s_TimeSinceLastTick < s_TickRate)
            {
                return;
            }

            TaskDirector.Tick();
            s_TimeSinceLastTick = 0;
        }
    }
}