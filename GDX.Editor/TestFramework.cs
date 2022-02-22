// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEngine;
using UnityEditor.TestTools.TestRunner.Api;

// ReSharper disable UnusedMember.Global

namespace GDX.Editor
{
    // ReSharper disable once UnusedType.Global
    public static class TestFramework
    {
        static TestRunnerApi s_TestRunner;
        static TestMonitor s_TestMonitor;

        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            if (s_TestRunner == null)
            {
                s_TestRunner = ScriptableObject.CreateInstance<TestRunnerApi>();
            }
            s_TestMonitor ??= new TestMonitor();
            s_TestRunner.RegisterCallbacks(s_TestMonitor);
        }
    }
}
