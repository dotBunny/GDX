// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using GDX.Editor.Windows;
using UnityEditor;
using UnityEngine;
#if UNITY_2022_2_OR_NEWER
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#endif

namespace GDX.Editor.Inspectors
{
    [CustomEditor(typeof(Data.SimpleTable))]
    public class SimpleTableInspector : UnityEditor.Editor
    {
        [MenuItem("Assets/Create/GDX/Simple Table")]
        public static void CreateAsset()
        {
            Data.SimpleTable asset = CreateInstance<Data.SimpleTable>();
            object[] args = { null };
            bool found = (bool)Reflection.InvokeStaticMethod("UnityEditor.ProjectWindowUtil", "TryGetActiveFolderPath", args,
                BindingFlags.Static | BindingFlags.NonPublic);
            if (found)
            {
                string basePath = (string)args[0];
                AssetDatabase.CreateAsset(asset, $"{basePath}/SimpleTable.asset");
            }
            else
            {
                AssetDatabase.CreateAsset(asset, "Assets/SimpleTable.asset");
            }
            AssetDatabase.SaveAssets();
            ProjectWindowUtil.ShowCreatedAsset(asset);
        }

#if UNITY_2022_2_OR_NEWER

        const string k_ButtonText = "Open Table";

        void OpenTargetAsset()
        {
            Data.SimpleTable table = (Data.SimpleTable)target;
            SimpleTableWindow.OpenAsset(table);
        }

        /// <inheritdoc />
        public override VisualElement CreateInspectorGUI()
        {
            Data.SimpleTable table = (Data.SimpleTable)target;
            VisualElement container = new VisualElement();

            var columns = table.GetOrderedColumns();
            Label columnsLabel;
            if (columns == null)
            {
                columnsLabel = new Label($"No columns");
            }
            else
            {
                columnsLabel = new Label($"{columns.Length.ToString()} Columns");

            }
            container.Add(columnsLabel);

            Button button = new Button(OpenTargetAsset);
            button.text = k_ButtonText;
            container.Add(button);

            return container;
        }
#else
        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            GUILayout.Label("Editing a SimpleTable is unsupported on this version of Unity.");
        }
#endif
    }
}