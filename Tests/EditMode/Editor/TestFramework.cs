// // Copyright (c) 2020-2021 dotBunny Inc.
// // dotBunny licenses this file to you under the BSL-1.0 license.
// // See the LICENSE file in the project root for more information.
//

using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace Editor
{
    public static class TestFramework
    {
        private static TestRunnerApi s_testRunner;
        private static TestMonitor s_testMonitor;

        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            if (s_testRunner == null)
            {
                s_testRunner = ScriptableObject.CreateInstance<TestRunnerApi>();
            }
            s_testMonitor = new TestMonitor();
            s_testRunner.RegisterCallbacks(s_testMonitor);
        }

        class TestMonitor : ICallbacks
        {
            /// <inheritdoc />
            public void RunStarted(ITestAdaptor testsToRun)
            {
                GDX.Editor.Automation.ClearTempFolder();
            }

            /// <inheritdoc />
            public void RunFinished(ITestResultAdaptor result)
            {

                //Debug.Log($"PASS: {result.PassCount} SKIP:{result.SkipCount} FAIL:{result.FailCount}");
#if !GDX_SAVE_TEST_OUTPUT
                if (result.FailCount == 0)
                {
                    GDX.Editor.Automation.ClearTempFolder();
                }
#endif
            }

            /// <inheritdoc />
            public void TestStarted(ITestAdaptor test)
            {

            }

            /// <inheritdoc />
            public void TestFinished(ITestResultAdaptor result)
            {
            }
        }
    }
}
