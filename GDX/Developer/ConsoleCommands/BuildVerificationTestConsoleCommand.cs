// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GDX.Developer.Reports;
using GDX.Developer.Reports.BuildVerification;
using GDX.Logging;
using UnityEngine.SceneManagement;

namespace GDX.Developer.ConsoleCommands
{
#if UNITY_2021_3_OR_NEWER

    public class BuildVerificationTestConsoleCommand : ConsoleCommandBase
    {
        TestScene[] m_Scenes;
        int m_ScenesCount;

        State m_State;
        Task m_TestRunner;

        /// <inheritdoc />
        public override bool Evaluate(float deltaTime)
        {
            switch (m_State)
            {
                case State.Init:
                    BuildVerificationReport.Reset();
                    m_State = State.Run;
                    return false;
                case State.Run:
                    m_TestRunner = TestRunner.Execute(m_Scenes);
                    m_State = State.Wait;
                    return false;

                case State.Wait:

                    if (m_TestRunner == null)
                    {
                        ManagedLog.Error(LogCategory.Test, "The test runner was unable to run.");
                        return true;
                    }

                    // Test runner thinks everything is done
                    if (m_TestRunner.IsCompleted)
                    {
                        m_State = State.Output;
                        return false;
                    }


                    return false;

                case State.Output:

                    string outputPath = Path.GetFullPath(Path.Combine(Platform.GetOutputFolder(),
                        $"BVT-{DateTime.Now.ToString(Localization.FilenameTimestampFormat)}.xml"));

                    string result = BuildVerificationReport.OutputReport(outputPath);

                    ManagedLog.Info(LogCategory.Test,
                        File.Exists(outputPath)
                            ? $"Build checks ({result}) written to {outputPath}."
                            : $"Unable to write file to {outputPath}.");
                    return true;
            }

            return true;
        }

        /// <inheritdoc />
        public override string GetKeyword()
        {
            return "bvt";
        }

        /// <inheritdoc />
        public override string GetHelpUsage()
        {
            return "bvt <SceneName, ...>";
        }

        /// <inheritdoc />
        public override string GetHelpMessage()
        {
            return "Loads a specific scene and attempts to run any automated tests associated with that process.";
        }

        /// <inheritdoc />
        public override ConsoleCommandBase GetInstance(string context)
        {
            BuildVerificationTestConsoleCommand command =
                new BuildVerificationTestConsoleCommand { m_State = State.Init };

            string[] scenes = context.Split(',');
            int sceneCount = scenes.Length;
            List<TestScene> validScenes = new List<TestScene>(sceneCount);
            for (int i = 0; i < sceneCount; i++)
            {
                Scene s = SceneManager.GetSceneByName(scenes[i]);
                if (s.IsValid())
                {
                    validScenes.Add(new TestScene(s.buildIndex));
                }
            }

            command.m_Scenes = validScenes.ToArray();
            command.m_ScenesCount = command.m_Scenes.Length;

            if (command.m_ScenesCount > 0)
            {
                return command;
            }

            ManagedLog.Warning(LogCategory.Default, $"No valid scenes were able to be extracted from '{context}'.");
            return null;
        }

        enum State
        {
            Init,
            Run,
            Wait,
            Output
        }
    }
#endif // UNITY_2021_3_OR_NEWER
}