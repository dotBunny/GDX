// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using GDX.Editor.Windows.Tables;
using GDX.Tables;
#if UNITY_2022_2_OR_NEWER
using System.Collections.Generic;
using UnityEngine.UIElements;
#endif

namespace GDX.Editor.Inspectors
{
#if UNITY_2022_2_OR_NEWER
    public abstract class TableInspectorBase : UnityEditor.Editor, TableCache.IColumnDefinitionChangeCallbackReceiver,
        TableCache.IRowDefinitionChangeCallbackReceiver
    {
        const string k_ButtonText = "Open Table";

        VisualElement m_RootElement;
        int m_TableTicket;
        bool m_Bound;

        void OnDisable()
        {
            if (!m_Bound)
            {
                return;
            }

            TableCache.UnregisterColumnChanged(this, m_TableTicket);
            TableCache.UnregisterRowChanged(this, m_TableTicket);
            TableCache.UnregisterUsage(m_TableTicket);
        }


        /// <inheritdoc />
        public override VisualElement CreateInspectorGUI()
        {
            TableBase table = (TableBase)target;

            m_TableTicket = TableCache.RegisterTable(table);


            m_RootElement = new VisualElement();

            Label dataLabel = new Label { name = "gdx-table-inspector-data" };
            m_RootElement.Add(dataLabel);

            Button button = new Button(OpenTargetAsset);
            button.text = k_ButtonText;
            m_RootElement.Add(button);

            UpdateInspector();

            TableCache.RegisterColumnChanged(this, m_TableTicket);
            TableCache.RegisterRowChanged(this, m_TableTicket);
            TableCache.RegisterUsage(m_TableTicket);
            m_Bound = true;

            return m_RootElement;
        }

        void UpdateInspector()
        {
            TableBase table = (TableBase)target;
            Label dataLabel = m_RootElement.Q<Label>("gdx-table-inspector-data");

            int columnCount = table.GetColumnCount();
            int rowCount = table.GetRowCount();

            dataLabel.text = $"{rowCount.ToString()} Rows with {columnCount.ToString()} Columns.";
        }

        void OpenTargetAsset()
        {
            TableBase table = (TableBase)target;
            TableWindowProvider.OpenAsset(table);
        }

        /// <inheritdoc />
        public void OnColumnDefinitionChange()
        {
            UpdateInspector();
        }

        /// <inheritdoc />
        public void OnRowDefinitionChange()
        {
            UpdateInspector();
        }
    }
#else
    public class TableInspectorBase : UnityEditor.Editor
    {
        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            UnityEngine.GUILayout.Label("Editing an TableBase is unsupported on this version of Unity.");
        }
    }
#endif
}