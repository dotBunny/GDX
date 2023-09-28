// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine.SceneManagement;

namespace GDX
{
    public static class SceneExtensions
    {
        public static bool IsSceneManagerBusy()
        {
            int sceneCount = SceneManager.sceneCount;
            for (int i = 0; i < sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (!scene.isLoaded)
                {
                    return true;
                }
            }
            return false;
        }
    }
}