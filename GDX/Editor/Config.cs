// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEngine;

namespace GDX.Editor
{
    public static class Config
    {
        public static GDXConfig Get()
        {
            GDXConfig settings = AssetDatabase.LoadAssetAtPath<GDXConfig>("Assets/" + GDXConfig.ResourcesPath);

            if (settings != null)
            {
                return settings;
            }

            settings = ScriptableObject.CreateInstance<GDXConfig>();

            Platform.EnsureFolderHierarchyExists(
                System.IO.Path.Combine(Application.dataPath,
                    GDXConfig.ResourcesPath));

            AssetDatabase.CreateAsset(settings, "Assets/" + GDXConfig.ResourcesPath);
            AssetDatabase.SaveAssets();

            return settings;
        }

        public static SerializedObject GetSerializedConfig()
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            return new SerializedObject(Get());
        }
    }
}