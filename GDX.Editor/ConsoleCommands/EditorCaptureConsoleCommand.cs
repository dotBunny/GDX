// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using GDX.Developer;
using GDX.Developer.ConsoleCommands;
using UnityEditor;
using UnityEngine;

namespace GDX.Editor.ConsoleCommands
{
#if UNITY_2022_2_OR_NEWER
    public class EditorCaptureConsoleCommand : ConsoleCommandBase
    {
        string m_FilePath;

        public override bool Evaluate(float deltaTime)
        {
            if (Automation.CaptureAllWindowsToPNG(m_FilePath))
            {
                Debug.Log($"Wrote editor capture to '{m_FilePath}'.");
            }
            return true;
        }

        public override string GetKeyword()
        {
            return "editorcapture";
        }

        public override string GetHelpMessage()
        {
            return "Captures a screenshot of the editor";
        }

        public override bool IsEditorOnly()
        {
            return true;
        }

        /// <inheritdoc />
        public override ConsoleCommandBase GetInstance(string context)
        {
            if (string.IsNullOrEmpty(context))
            {
                return new EditorCaptureConsoleCommand()
                {
                    m_FilePath = ScreenCaptureConsoleCommand.GetPath(GetDefaultName())
                };
            }
            return new EditorCaptureConsoleCommand() { m_FilePath = ScreenCaptureConsoleCommand.GetPath(context) };
        }

        [InitializeOnLoadMethod]
        static void RegisterCommand()
        {
            Developer.Console.RegisterCommand(new EditorCaptureConsoleCommand());
        }

        static string GetDefaultName()
        {
            return $"EditorScreenshot_{DateTime.Now.ToString(Platform.FilenameTimestampFormat)}.png";
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}