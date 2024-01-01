// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.DataTables.CellValues;
using UnityEditor;
using UnityEngine.UIElements;

namespace GDX.Editor.PropertyDrawers.CellValues
{
#if UNITY_2022_2_OR_NEWER
    [CustomPropertyDrawer(typeof(DoubleCellValue))]
    public class DoubleCellValueDrawer : CellValueDrawerBase
    {
        DoubleCellValue m_CellValue;

        /// <inheritdoc />
        protected override VisualElement GetCellElement()
        {
            m_CellValue = new DoubleCellValue(m_DataTable, m_RowIdentifier, m_ColumnIdentifier);
            DoubleField newField = new DoubleField(null) { name = k_CellFieldName };
            newField.SetValueWithoutNotify(m_CellValue.Get());
            newField.RegisterValueChangedCallback(e =>
            {
                DataTableTracker.RecordCellValueUndo(m_TableTicket, m_RowIdentifier, m_ColumnIdentifier);
                m_CellValue.Set(e.newValue);
                NotifyOfChange();
            });
            return newField;
        }

        /// <inheritdoc />
        protected override void UpdateValue()
        {
            DoubleField cellField = (DoubleField)m_CellElement;
            cellField.SetValueWithoutNotify(m_CellValue.Get());
        }

        protected override ulong GetDataVersion()
        {
            return m_CellValue.GetDataVersion();
        }

        /// <inheritdoc />
        protected override Serializable.SerializableTypes GetSupportedType()
        {
            return DoubleCellValue.GetSupportedType();
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}