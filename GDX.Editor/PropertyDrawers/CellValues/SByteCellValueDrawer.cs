﻿// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Tables;
using GDX.Tables.CellValues;
using UnityEditor;
using UnityEngine.UIElements;

namespace GDX.Editor.PropertyDrawers.CellValues
{
    [CustomPropertyDrawer(typeof(SByteCellValue))]
    public class SByteCellValueDrawer : CellValueDrawerBase
    {
        SByteCellValue m_CellValue;

        /// <inheritdoc />
        protected override VisualElement GetCellElement()
        {
            m_CellValue = new SByteCellValue(m_Table, m_RowInternalIndex, m_ColumnInternalIndex);
            SliderInt newField = new SliderInt(sbyte.MinValue, sbyte.MaxValue)
            {
                name = k_CellFieldName, showInputField = true, label = null
            };
            newField.SetValueWithoutNotify(m_CellValue.Get());
            newField.RegisterValueChangedCallback(e =>
            {
                m_CellValue.Set((sbyte)e.newValue);
            });
            return newField;
        }

        /// <inheritdoc />
        protected override Serializable.SerializableTypes GetSupportedType()
        {
            return SByteCellValue.GetSupportedType();
        }
    }
}