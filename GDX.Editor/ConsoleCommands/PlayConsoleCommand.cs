// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Developer;
using UnityEditor;

namespace GDX.Editor.ConsoleCommands
{
#if UNITY_2022_2_OR_NEWER
    public class PlayConsoleCommand : ConsoleCommandBase
    {
        public override bool Evaluate(float deltaTime)
        {
            if (!EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = true;
            }

            return true;
        }

        public override string GetKeyword()
        {
            return "play";
        }

        public override string GetHelpMessage()
        {
            return "Enter playmode in the editor.";
        }

        public override bool IsEditorOnly()
        {
            return true;
        }

        [InitializeOnLoadMethod]
        static void RegisterCommand()
        {
            Console.RegisterCommand(new PlayConsoleCommand());
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}