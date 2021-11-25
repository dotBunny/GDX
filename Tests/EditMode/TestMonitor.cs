// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#define GDX_SAVE_TEST_OUTPUT

#if UNITY_2019_1_OR_NEWER

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

namespace GDX.Tests.EditMode
{
    class TestMonitor : ICallbacks
    {
        private string _cachedTempFolder;

        /// <inheritdoc />
        public void RunStarted(ITestAdaptor testsToRun)
        {

            _cachedTempFolder = Editor.Automation.GetTempFolder();
            if (Application.isBatchMode)
            {
                Editor.Automation.StashWindowLayout();
            }

            EditorWindow gameView = Editor.Automation.GetGameView();
            if (gameView != null)
            {
                gameView.Show(true);
            }

            // Make sure our temp folder is absolutely clear at the start
            Editor.Automation.ClearTempFolder();
        }

        /// <inheritdoc />
        public void RunFinished(ITestResultAdaptor result)
        {
            if (Application.isBatchMode)
            {
                Editor.Automation.RestoreWindowLayout();
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
            }
            else
            {
                string[] foundFiles = Directory.GetFiles(_cachedTempFolder, "*", SearchOption.TopDirectoryOnly);
                foreach (string file in foundFiles)
                {
                    File.Delete(file);
                }
            }
#endif // GDX_SAVE_TEST_OUTPUT
        }
    }
}

#endif // UNITY_2019_1_OR_NEWER