// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace GDX.Experimental
{
    public class DeveloperConsoleSystem
    {
        /// <summary>
        ///     Has the developer console system been added to the player loop for updating at runtime.
        /// </summary>
        static bool s_AddedToPlayerLoop;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void InitializeAtRuntime()
        {
            AddToPlayerLoop();
        }

        static void AddToPlayerLoop()
        {
            if (s_AddedToPlayerLoop) return;

            PlayerLoopSystem systemRoot = PlayerLoop.GetCurrentPlayerLoop();
            systemRoot.AddSubSystemToFirstSubSystemOfType(
                typeof(Initialization),
                typeof(DeveloperConsoleSystem), PlayerLoopTick);
            PlayerLoop.SetPlayerLoop(systemRoot);

            s_AddedToPlayerLoop = true;
        }

        static void PlayerLoopTick()
        {
            DeveloperConsole.Tick(Time.deltaTime);
        }
    }
}