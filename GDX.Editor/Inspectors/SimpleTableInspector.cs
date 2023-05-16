// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEngine;
#if UNITY_2022_2_OR_NEWER
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#endif

namespace GDX.Editor.Inspectors
{
    [CustomEditor(typeof(SimpleTable))]
    public class SimpleTableInspector : UnityEditor.Editor
    {
        const string k_ButtonText = "Open Table";

        [MenuItem("Assets/Create/GDX/Simple Table")]
        public static void CreateAsset()
        {
            SimpleTable asset = CreateInstance<SimpleTable>();

            AssetDatabase.CreateAsset(asset, "Assets/SimpleTable.asset");
            AssetDatabase.SaveAssets();

            ProjectWindowUtil.ShowCreatedAsset(asset);
        }

        public static void OpenAsset(SimpleTable table)
        {

        }

        void OpenTargetAsset()
        {
            SimpleTable table = (SimpleTable)target;
            OpenAsset(table);
        }

#if UNITY_2022_2_OR_NEWER
        /// <inheritdoc />
        public override VisualElement CreateInspectorGUI()
        {
            SimpleTable table = (SimpleTable)target;
            VisualElement container = new VisualElement();

            Button button = new Button(OpenTargetAsset);
            button.text = k_ButtonText;

            container.Add(button);
            return container;
        }
#else
        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            SimpleTable table = (SimpleTable)target;

            // TODO: info on table?

            if (GUILayout.Button(k_ButtonText))
            {
                OpenTargetAsset();
            }

        }
#endif


    }
}