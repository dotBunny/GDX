﻿// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine.LowLevel;

namespace GDX.Developer.ConsoleCommands
{
#if UNITY_2022_2_OR_NEWER
    public class PlayerLoopConsoleCommand : ConsoleCommandBase
    {
        /// <inheritdoc />
        public override bool Evaluate(float deltaTime)
        {
            PlayerLoopSystem loop = PlayerLoop.GetCurrentPlayerLoop();
            TextGenerator text = new TextGenerator();
            loop.GenerateSystemTree(text);

            UnityEngine.Debug.Log(text.ToString());
            return true;
        }

        /// <inheritdoc />
        public override Console.ConsoleAccessLevel GetAccessLevel()
        {
            return Console.ConsoleAccessLevel.Developer;
        }

        /// <inheritdoc />
        public override string GetKeyword()
        {
            return "playerloop";
        }

        /// <inheritdoc />
        public override string GetHelpMessage()
        {
            return "Output a structured list of the the current systems in Unity's player loop.";
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}