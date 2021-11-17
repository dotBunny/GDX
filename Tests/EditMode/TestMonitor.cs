// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if UNITY_2019_1_OR_NEWER

using System.Collections.Generic;
using System.IO;
using UnityEditor.TestTools.TestRunner.Api;

namespace GDX.Tests.EditMode
{
    class TestMonitor : ICallbacks
    {
        private string _cachedTempFolder;
        private List<string> _knownArtifacts = new List<string>(20);

        /// <inheritdoc />
        public void RunStarted(ITestAdaptor testsToRun)
        {
            _cachedTempFolder = Editor.Automation.GetTempFolder();
            _knownArtifacts.Clear();

            // Make sure our temp folder is absolutely clear at the start
            Editor.Automation.ClearTempFolder();
        }

        /// <inheritdoc />
        public void RunFinished(ITestResultAdaptor result)
        {
        }

        private string[] _knownFiles;
        /// <inheritdoc />
        public void TestStarted(ITestAdaptor test)
        {
        }

        /// <inheritdoc />
        public void TestFinished(ITestResultAdaptor result)
        {
#if !GDX_SAVE_TEST_OUTPUT
            string[] foundFiles = Directory.GetFiles(_cachedTempFolder, "*", SearchOption.AllDirectories);
            foreach (string file in foundFiles)
            {
                // We already know about it and have decided to keep it
                if (_knownArtifacts.Contains(file))
                {

                }
                else if (result.FailCount > 0)
                {
                    _knownArtifacts.Add(file);
                }
                else
                {
                    Platform.ForceDeleteFile(file);
                }
            }
#endif
        }
    }
}

#endif