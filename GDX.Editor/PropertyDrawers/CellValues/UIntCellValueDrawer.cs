// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Tables;
using GDX.Tables.CellValues;
using UnityEditor;
using UnityEngine.UIElements;

namespace GDX.Editor.PropertyDrawers.CellValues
{
    [CustomPropertyDrawer(typeof(UIntCellValue))]
    public class UIntCellValueDrawer : CellValueDrawerBase
    {
        UIntCellValue m_CellValue;

        /// <inheritdoc />
        protected override VisualElement GetCellElement()
        {
            m_CellValue = new UIntCellValue(m_Table, m_RowInternalIndex, m_ColumnInternalIndex);
            // TODO: crushes
            IntegerField newField = new IntegerField(null) { name = k_CellFieldName };
            newField.SetValueWithoutNotify((int)m_CellValue.Get());
            newField.RegisterValueChangedCallback(e =>
            {
                m_CellValue.Set((uint)e.newValue);
                NotifyOfChange();
            });
            return newField;
        }

        /// <inheritdoc />
        protected override Serializable.SerializableTypes GetSupportedType()
        {
            return UIntCellValue.GetSupportedType();
        }
    }
}