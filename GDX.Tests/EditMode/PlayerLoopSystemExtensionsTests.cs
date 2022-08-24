// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Threading;
using NUnit.Framework;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace GDX
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="PlayerLoopSystemExtensions" />
    ///     class.
    /// </summary>
    public class PlayerLoopSystemExtensionsTests
    {
        [TearDown]
        public void TearDown()
        {
            RemoveTestSystem();
        }
        /*
         * AddSubSystem
         * RemoveSubSystemsOfType
         * RemoveSubSystemsOfTypeFromFirstSubSystemOfType
         * ReplaceFirstSubSystemOfType
         * ReplaceSubSystemsOfType
         * TryGetFirstSubSystemOfType
         * TryGetFirstSystemWithSubSystemOfType
         */


        [Test]
        [Category(Core.TestCategory)]
        public void AddSubSystemToFirstSubSystemOfType_PlayerSystem_Added()
        {
            AddTestSystem();

            PlayerLoopSystem baseLoop = PlayerLoop.GetCurrentPlayerLoop();
            Assert.IsTrue(baseLoop.GenerateSystemTree().ToString().Contains(nameof(PlayerLoopSystemExtensionsTests)));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddSubSystemToFirstSubSystemOfType_MethodType_Added()
        {
            PlayerLoopSystem playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            playerLoop.AddSubSystemToFirstSubSystemOfType(
                typeof(Update.ScriptRunBehaviourUpdate),
                typeof(PlayerLoopSystemExtensionsTests),
                TestTick);
            PlayerLoop.SetPlayerLoop(playerLoop);

            PlayerLoopSystem baseLoop = PlayerLoop.GetCurrentPlayerLoop();
            Assert.IsTrue(baseLoop.GenerateSystemTree().ToString().Contains(nameof(PlayerLoopSystemExtensionsTests)));
        }

        void AddTestSystem()
        {
            PlayerLoopSystem playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopSystem testSystem = new PlayerLoopSystem()
            {
                updateDelegate = TestTick, type = typeof(PlayerLoopSystemExtensionsTests)
            };
            playerLoop.AddSubSystemToFirstSubSystemOfType(typeof(Update.ScriptRunBehaviourUpdate), ref testSystem);
            PlayerLoop.SetPlayerLoop(playerLoop);
        }

        void RemoveTestSystem()
        {
            PlayerLoopSystem systemRoot = PlayerLoop.GetCurrentPlayerLoop();
            systemRoot.RemoveSubSystemsOfTypeFromFirstSubSystemOfType(
                typeof(Update.ScriptRunBehaviourUpdate),
                typeof(PlayerLoopSystemExtensionsTests));
            PlayerLoop.SetPlayerLoop(systemRoot);
        }

        void TestTick()
        {

        }
    }
}