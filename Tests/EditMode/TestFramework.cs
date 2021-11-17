﻿// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEngine;
#if UNITY_2019_1_OR_NEWER
using UnityEditor.TestTools.TestRunner.Api;
#endif

namespace GDX.Tests.EditMode
{
#if UNITY_2019_1_OR_NEWER
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
            if (s_testMonitor == null)
            {
                s_testMonitor = new TestMonitor();
            }
            s_testRunner.RegisterCallbacks(s_testMonitor);
        }
    }
#else
    public static class TestFramework
    {
        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            GDX.Editor.Automation.ClearTempFolder();
        }
    }
#endif
}
