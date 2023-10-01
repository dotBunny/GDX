// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

// TODO: Make GDX_SAVE_TEST_OUTPUT and option to define
namespace GDX.Editor
{
    class TestMonitor : ICallbacks
    {
        public static bool IsTesting;

        string m_CachedTempFolder;
        bool m_IsMonitoredTestRun;

        /// <inheritdoc />
        public void RunStarted(ITestAdaptor testsToRun)
        {
            IsTesting = true;

            // We are only going to monitor GDX tests
            m_IsMonitoredTestRun = testsToRun.FullName == "GDX";
            if (!m_IsMonitoredTestRun)
            {
                return;
            }

            m_CachedTempFolder = Platform.GetOutputFolder(Config.PlatformAutomationFolder);
            if (Application.isBatchMode)
            {
                Automation.StashWindowLayout();
            }

            // This will skip if theres no device to capture
            EditorWindow gameView = Automation.GetGameView();
            if (gameView != null)
            {
                gameView.Show(true);
            }

            // Make sure our temp folder is absolutely clear at the start
            Automation.ClearTempFolder();
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public void RunFinished(ITestResultAdaptor result)
        {
            IsTesting = false;

            // We only really want to do this for GDX tests
            if (m_IsMonitoredTestRun && Application.isBatchMode)
            {
                Automation.RestoreWindowLayout();
            }
        }

        /// <inheritdoc />
        public void TestStarted(ITestAdaptor test)
        {
        }

        /// <inheritdoc />
        public void TestFinished(ITestResultAdaptor result)
        {
            if (!m_IsMonitoredTestRun)
            {
                return;
            }

            // We dont want to do any processing during the parent suites currently, only after an actual test method
            // has been invoked/ran
            if (result.Test.IsSuite)
            {
                return;
            }

            bool validTest = result.Test.Categories.ContainsValue(Core.TestCategory) ||
                             result.Test.Categories.ContainsValue(Core.PerformanceCategory);

            // If it is not a valid test we have nothing to do here
            if (!validTest)
            {
                return;
            }

#if GDX_SAVE_TEST_OUTPUT
            string testFolder = Path.Combine(_cachedTempFolder, $"TEST_{result.Test.FullName}");
            string[] foundFiles = Directory.GetFiles(_cachedTempFolder, "*", SearchOption.TopDirectoryOnly);
            if (foundFiles.Length > 0)
            {
                Platform.EnsureFolderHierarchyExists(testFolder);
            }
            foreach (string file in foundFiles)
            {
                string newFilePath = Path.Combine(testFolder, Path.GetFileName(file));
                File.Move(file, newFilePath);
            }
#else
            if (result.FailCount > 0)
            {
                Console.WriteLine($"Test {result.Test.FullName} Failed.");
                string testFolder = Path.Combine(m_CachedTempFolder, $"TEST_{result.Test.FullName}");
                string[] foundFiles = Directory.GetFiles(m_CachedTempFolder, "*", SearchOption.TopDirectoryOnly);
                if (foundFiles.Length > 0)
                {
                    Platform.EnsureFolderHierarchyExists(testFolder);
                }

                foreach (string file in foundFiles)
                {
                    string newFilePath = Path.Combine(testFolder, Path.GetFileName(file));
                    File.Move(file, newFilePath);
                }
            }
            else
            {
                string[] foundFiles = Directory.GetFiles(m_CachedTempFolder, "*", SearchOption.TopDirectoryOnly);
                foreach (string file in foundFiles)
                {
                    File.Delete(file);
                }
            }
#endif // GDX_SAVE_TEST_OUTPUT
        }
    }
}