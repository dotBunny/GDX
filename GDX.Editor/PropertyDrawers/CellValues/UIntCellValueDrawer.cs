// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.DataTables.CellValues;
using UnityEditor;
using UnityEngine.UIElements;

namespace GDX.Editor.PropertyDrawers.CellValues
{
#if UNITY_2022_2_OR_NEWER
    [CustomPropertyDrawer(typeof(UIntCellValue))]
    public class UIntCellValueDrawer : CellValueDrawerBase
    {
        UIntCellValue m_CellValue;

        /// <inheritdoc />
        protected override VisualElement GetCellElement()
        {
            m_CellValue = new UIntCellValue(m_DataTable, m_RowInternalIndex, m_ColumnInternalIndex);
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
        protected override void UpdateValue()
        {
            IntegerField cellField = (IntegerField)m_CellElement;
            cellField.SetValueWithoutNotify((int)m_CellValue.Get());
        }

        /// <inheritdoc />
        protected override ulong GetDataVersion()
        {
            return m_CellValue.GetDataVersion();
        }

        /// <inheritdoc />
        protected override Serializable.SerializableTypes GetSupportedType()
        {
            return UIntCellValue.GetSupportedType();
        }
    }
#endif
}