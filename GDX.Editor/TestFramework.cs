// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEngine;
using UnityEditor.TestTools.TestRunner.Api;

// ReSharper disable UnusedMember.Global

namespace GDX.Editor
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
            s_testMonitor ??= new TestMonitor();
            s_testRunner.RegisterCallbacks(s_testMonitor);
        }
    }
}
