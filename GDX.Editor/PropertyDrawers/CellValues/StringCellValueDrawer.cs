﻿// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Tables;
using GDX.Tables.CellValues;
using UnityEditor;
using UnityEngine.UIElements;

namespace GDX.Editor.PropertyDrawers.CellValues
{
    [CustomPropertyDrawer(typeof(StringCellValue))]
    public class StringCellValueDrawer : CellValueDrawerBase
    {
        StringCellValue m_CellValue;
        /// <inheritdoc />
        protected override void Init(SerializedProperty serializedProperty)
        {
            if (Reflection.TryGetFieldValue(serializedProperty.serializedObject.targetObject, typeof(StringCellValue),
                    serializedProperty.name, out StringCellValue cell))
            {
                m_CellValue = cell;
                CopyToInspector();
            }
        }

        /// <inheritdoc />
        protected override void CopyFromInspector(ITable table, int rowInternalIndex, int columnInternalIndex)
        {
            if (table != null && rowInternalIndex != -1 && columnInternalIndex != -1)
            {
                m_CellValue = new StringCellValue(table, rowInternalIndex, columnInternalIndex);
            }
        }

        /// <inheritdoc />
        protected override void CopyToInspector()
        {
            if (m_CellValue.Table != null)
            {
                m_Table = m_CellValue.Table;
                m_RowInternalIndex = m_CellValue.Row;
                m_ColumnInternalIndex = m_CellValue.Column;
            }
            else
            {
                m_RowInternalIndex = -1;
                m_ColumnInternalIndex = -1;
            }
        }

        /// <inheritdoc />
        protected override VisualElement GetCellElement()
        {
            TextField newField = new TextField(null) { name = k_CellFieldName };
            newField.SetValueWithoutNotify(m_CellValue.Get());
            newField.RegisterValueChangedCallback(e =>
            {
                m_CellValue.Set(e.newValue);
                NotifyOfChange();
            });
            return newField;
        }
    }
}