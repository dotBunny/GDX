// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine.SceneManagement;

namespace GDX.Developer.Reports.BuildVerification
{
    public readonly struct TestScene
    {
        public readonly int BuildIndex;
        public readonly string ScenePath;
        public readonly int LoadTimeout;
        public readonly int TestTimeout;
        public readonly int UnloadTimeout;

        public TestScene(int buildIndex, int loadTimeout = 10000, int testTimeout = 30000, int unloadTimeout = 10000)
        {
            BuildIndex = buildIndex;
            ScenePath = SceneUtility.GetScenePathByBuildIndex(buildIndex);

            LoadTimeout = loadTimeout;
            TestTimeout = testTimeout;
            UnloadTimeout = unloadTimeout;
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(ScenePath);
        }
    }
}