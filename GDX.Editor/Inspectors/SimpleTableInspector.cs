// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEngine;

namespace GDX.Editor.Inspectors
{
    public class SimpleTableInspector : UnityEditor.EditorWindow
    {
        [MenuItem("Assets/Create/GDX/Simple Table")]
        public static void CreateAsset()
        {
            SimpleTable asset = ScriptableObject.CreateInstance<SimpleTable>();

            AssetDatabase.CreateAsset(asset, "Assets/SimpleTable.asset");
            AssetDatabase.SaveAssets();

            ProjectWindowUtil.ShowCreatedAsset(asset);
        }
    }
}