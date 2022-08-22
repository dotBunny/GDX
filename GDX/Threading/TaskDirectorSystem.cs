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

        public static void AddToPlayerLoop()
        {
            if (s_AddedToPlayerLoop) return;

            PlayerLoopSystem systemRoot = PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopSystem taskDirectorSystem = new PlayerLoopSystem()
            {
                updateDelegate = PlayerLoopTick, type = typeof(TaskDirectorSystem)
            };
            systemRoot.AddChildSystem(typeof(Update.ScriptRunDelayedTasks), ref taskDirectorSystem);
            PlayerLoop.SetPlayerLoop(systemRoot);

            s_AddedToPlayerLoop = true;
        }

        public static void RemoveFromPlayerLoop()
        {
            if (!s_AddedToPlayerLoop) return;
            PlayerLoopSystem systemRoot = PlayerLoop.GetCurrentPlayerLoop();
            systemRoot.RemoveChildSystem(typeof(Update.ScriptRunDelayedTasks), typeof(TaskDirectorSystem));
            PlayerLoop.SetPlayerLoop(systemRoot);
            s_AddedToPlayerLoop = false;
        }

        static void PlayerLoopTick()
        {
            if (!Application.isPlaying) { return; }

            Debug.Log("TICKING FROM PLAYERLOOP");
            // Todo TickRate? Runtime?
            //TaskDirector.Tick();
        }
    }
}