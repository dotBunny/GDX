// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using GDX.Editor.Windows;
using GDX.Tables;
using UnityEditor;
using UnityEngine;
#if UNITY_2022_2_OR_NEWER
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#endif

namespace GDX.Editor.Inspectors
{
    public class ITableInspector : UnityEditor.Editor
    {
#if UNITY_2022_2_OR_NEWER

        const string k_ButtonText = "Open Table";

        void OpenTargetAsset()
        {
            ITable table = (ITable)target;
            TableWindow.OpenAsset(table);
        }

        /// <inheritdoc />
        public override VisualElement CreateInspectorGUI()
        {
            ITable table = (ITable)target;
            VisualElement container = new VisualElement();
            int columnCount = table.GetColumnCount();
            int rowCount = table.GetRowCount();

            Label dataLabel = new Label( $"{rowCount} Rows with {columnCount} Columns.");
            container.Add(dataLabel);

            Button button = new Button(OpenTargetAsset);
            button.text = k_ButtonText;
            container.Add(button);

            return container;
        }
#else
        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            GUILayout.Label("Editing an ITable is unsupported on this version of Unity.");
        }
#endif
    }
}