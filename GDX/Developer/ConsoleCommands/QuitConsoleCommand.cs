// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Logging;
using UnityEditor;
#if !UNITY_EDITOR
using UnityEngine;
#endif

namespace GDX.Developer.ConsoleCommands
{
#if UNITY_2022_2_OR_NEWER
    public class QuitConsoleCommand : ConsoleCommandBase
    {
        int errorCode;

        /// <inheritdoc />
        public override bool Evaluate(float deltaTime)
        {
            ManagedLog.Info(LogCategory.Default, $"Quitting! [{errorCode}]");
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit(errorCode);
#endif
            return true;
        }

        /// <inheritdoc />
        public override ConsoleCommandLevel GetAccessLevel()
        {
            return ConsoleCommandLevel.Anonymous;
        }

        /// <inheritdoc />
        public override string GetKeyword()
        {
            return "quit";
        }

        /// <inheritdoc />
        public override string GetHelpMessage()
        {
            return
                "Exits the application with the provided error code, if none is present will use the currently set one OR exits playmode in the editor.";
        }

        /// <inheritdoc />
        public override ConsoleCommandBase GetInstance(string context)
        {
            QuitConsoleCommand command = new QuitConsoleCommand();
            if (string.IsNullOrEmpty(context))
            {
                return command;
            }

            int.TryParse(context, out int result);
            command.errorCode = result;
            return command;
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}