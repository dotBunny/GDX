// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace GDX.Experimental.Logging
{
    /// <summary>
    ///     Supports pumping the managed logs tick mechanism
    /// </summary>
    public class ManagedLogSystem
    {
        // TODO: Add config var?

        /// <summary>
        ///     Has the managed log system been added to the player loop for updating at runtime.
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
                typeof(ManagedLogSystem), PlayerLoopTick);
            PlayerLoop.SetPlayerLoop(systemRoot);

            s_AddedToPlayerLoop = true;
        }

        public static void PlayerLoopTick()
        {
            ManagedLog.Tick();

        }
    }
}