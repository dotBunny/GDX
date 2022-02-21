// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.IO;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

// TODO: Make GDX_SAVE_TEST_OUTPUT and option to define
namespace GDX.Editor
{
    internal class TestMonitor : ICallbacks
    {
        private string m_CachedTempFolder;

        /// <inheritdoc />
        public void RunStarted(ITestAdaptor testsToRun)
        {
            
            m_CachedTempFolder = Platform.GetOutputFolder("GDX_Automation");
            if (Application.isBatchMode)
            {
                Automation.StashWindowLayout();
            }

            EditorWindow gameView = Automation.GetGameView();
            if (gameView != null)
            {
                gameView.Show(true);
            }

            // Make sure our temp folder is absolutely clear at the start
            Automation.ClearTempFolder();
        }

        /// <inheritdoc />
        public void RunFinished(ITestResultAdaptor result)
        {
            if (Application.isBatchMode)
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
            // We dont want to do any processing during the parent suites currently, only after an actual test method
            // has been invoked/ran
            if (result.Test.IsSuite)
            {
                return;
            }

            int categoryCount = result.Test.Categories.Length;
            bool validTest = false;
            for (int i = 0; i < categoryCount; i++)
            {
                // TODO: We're only going to watch GDX tests atm, maybe in the future add an option
                if (result.Test.Categories[i] == Core.TestCategory ||
                    result.Test.Categories[i] == Core.PerformanceCategory)
                {
                    validTest = true;
                    break;
                }
            }
            if (!validTest) return;

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
                System.Console.WriteLine($"Test {result.Test.FullName} Failed.");
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