// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using GDX.Logging;
using UnityEngine;

namespace GDX.Developer.ConsoleCommands
{
#if UNITY_2022_2_OR_NEWER
    public class ScreenCaptureConsoleCommand : ConsoleCommandBase
    {
        string m_FilePath;
        int m_SuperSize = 1;

        public override bool Evaluate(float deltaTime)
        {
            ScreenCapture.CaptureScreenshot(m_FilePath, m_SuperSize);
            ManagedLog.Info(LogCategory.GDX, $"Wrote screen capture to '{m_FilePath}'.");
            return true;
        }

        public override string GetKeyword()
        {
            return "screencapture";
        }

        public override string GetHelpMessage()
        {
            return "Captures a screenshot.";
        }

        /// <inheritdoc />
        public override ConsoleCommandBase GetInstance(string context)
        {
            // Early out
            if (string.IsNullOrEmpty(context))
            {
                return new ScreenCaptureConsoleCommand() { m_FilePath = GetPath(GetDefaultName())};
            }

            // Handle arguments
            string[] split = context.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (split.Length > 1)
            {
                return new ScreenCaptureConsoleCommand()
                {
                    m_FilePath = GetPath(split[0]), m_SuperSize = int.Parse(split[1])
                };
            }

            if (split[0].IsNumeric())
            {
                return new ScreenCaptureConsoleCommand()
                {
                    m_FilePath = GetPath(GetDefaultName()), m_SuperSize = int.Parse(split[0])
                };
            }

            return new ScreenCaptureConsoleCommand()
            {
                m_FilePath = GetPath(split[0])
            };
        }

        public static string GetPath(string fileName)
        {
            return Path.Combine(Platform.GetOutputFolder("Screenshots"), fileName);
        }
        static string GetDefaultName()
        {
            return $"Screenshot_{DateTime.Now.ToString(Platform.FilenameTimestampFormat)}.png";
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}