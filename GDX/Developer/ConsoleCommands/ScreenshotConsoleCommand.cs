// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.Developer.ConsoleCommands
{
    public class ScreenshotConsoleCommand : ConsoleCommandBase
    {
        int m_SuperSize;

        public override bool Evaluate(float deltaTime)
        {
            if (Application.isPlaying)
            {
                ScreenCapture.CaptureScreenshot("test.png", 1);
            }
            else
            {
              
            }

            return true;
        }

        public override string GetKeyword()
        {
            return "screenshot";
        }

        public override string GetHelpMessage()
        {
            return "Captures a screenshot.";
        }
    }
}