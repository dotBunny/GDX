﻿// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Tables.CellValues;
using UnityEditor;

namespace GDX.Editor.PropertyDrawers.CellValues
{
    [CustomPropertyDrawer(typeof(RectIntCellValue))]
    public class RectIntCellValueDrawer : CellValueDrawerBase
    {
        RectIntCellValue m_CellValue;
        /// <inheritdoc />
        protected override void Init(SerializedProperty serializedProperty)
        {
            if (Reflection.TryGetFieldValue(serializedProperty.serializedObject.targetObject, typeof(RectIntCellValue),
                    serializedProperty.name, out RectIntCellValue cell))
            {
                m_CellValue = cell;
                if (m_CellValue.Table != null)
                {
                    m_Table = m_CellValue.Table;
                }
                m_RowInternalIndex = m_CellValue.Row;
                m_ColumnInternalIndex = m_CellValue.Column;
            }
        }
    }
}