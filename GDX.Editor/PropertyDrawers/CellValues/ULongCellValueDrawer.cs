// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.DataTables.CellValues;
using UnityEditor;
using UnityEngine.UIElements;

namespace GDX.Editor.PropertyDrawers.CellValues
{
#if UNITY_2022_2_OR_NEWER
    [CustomPropertyDrawer(typeof(ULongCellValue))]
    public class ULongCellValueDrawer : CellValueDrawerBase
    {
        ULongCellValue m_CellValue;

        /// <inheritdoc />
        protected override VisualElement GetCellElement()
        {
            m_CellValue = new ULongCellValue(m_DataTable, m_RowInternalIndex, m_ColumnInternalIndex);
            // TODO: crushes
            LongField newField = new LongField(null) { name = k_CellFieldName };
            newField.SetValueWithoutNotify((int)m_CellValue.Get());
            newField.RegisterValueChangedCallback(e =>
            {
                m_CellValue.Set((ulong)e.newValue);
                NotifyOfChange();
            });
            return newField;
        }

        /// <inheritdoc />
        protected override void UpdateValue()
        {
            LongField cellField = (LongField)m_CellElement;
            cellField.SetValueWithoutNotify((long)m_CellValue.Get());
        }

        /// <inheritdoc />
        protected override ulong GetDataVersion()
        {
            return m_CellValue.GetDataVersion();
        }

        /// <inheritdoc />
        protected override Serializable.SerializableTypes GetSupportedType()
        {
            return ULongCellValue.GetSupportedType();
        }
    }
#endif
}