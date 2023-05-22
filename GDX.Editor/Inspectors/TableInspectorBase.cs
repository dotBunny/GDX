// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Editor.Windows;
using GDX.Tables;
#if UNITY_2022_2_OR_NEWER
using System.Collections.Generic;
using UnityEngine.UIElements;
#endif

namespace GDX.Editor.Inspectors
{
    public abstract class TableInspectorBase : UnityEditor.Editor
    {
#if UNITY_2022_2_OR_NEWER

        static readonly Dictionary<ITable, VisualElement> k_KnownInspectors = new Dictionary<ITable, VisualElement>();

        public static void RedrawInspector(ITable table)
        {
            if (k_KnownInspectors.TryGetValue(table, out VisualElement inspectorElement))
            {
                UpdateInspector(inspectorElement, table);
            }
        }

        static void UpdateInspector(VisualElement rootElement, ITable table)
        {
            Label dataLabel = rootElement.Q<Label>("gdx-table-inspector-data");

            int columnCount = table.GetColumnCount();
            int rowCount = table.GetRowCount();

            dataLabel.text = $"{rowCount.ToString()} Rows with {columnCount.ToString()} Columns.";
        }

        const string k_ButtonText = "Open Table";

        void OpenTargetAsset()
        {
            ITable table = (ITable)target;
            TableWindowProvider.OpenAsset(table);
        }

        void OnDestroy()
        {
            ITable table = (ITable)target;
            k_KnownInspectors.Remove(table);
        }

        /// <inheritdoc />
        public override VisualElement CreateInspectorGUI()
        {
            ITable table = (ITable)target;
            VisualElement container = new VisualElement();

            Label dataLabel = new Label { name = "gdx-table-inspector-data" };
            container.Add(dataLabel);

            Button button = new Button(OpenTargetAsset);
            button.text = k_ButtonText;
            container.Add(button);

            k_KnownInspectors[table] = container;
            UpdateInspector(container, table);

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