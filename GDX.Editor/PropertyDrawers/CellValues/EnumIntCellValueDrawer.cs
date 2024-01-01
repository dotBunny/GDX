// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.DataTables.CellValues;
using UnityEditor;
using UnityEngine.UIElements;

namespace GDX.Editor.PropertyDrawers.CellValues
{
#if UNITY_2022_2_OR_NEWER
    [CustomPropertyDrawer(typeof(EnumIntCellValue))]
    public class EnumIntCellValueDrawer : CellValueDrawerBase
    {
        EnumIntCellValue m_CellValue;

        /// <inheritdoc />
        protected override VisualElement GetCellElement()
        {
            m_CellValue = new EnumIntCellValue(m_DataTable, m_RowIdentifier, m_ColumnIdentifier);

            EnumField newField = new EnumField(m_CellValue.GetEnum()) { name = k_CellFieldName };
            newField.RegisterValueChangedCallback(e =>
            {
                DataTableTracker.RecordCellValueUndo(m_TableTicket, m_RowIdentifier, m_ColumnIdentifier);
                m_CellValue.Set((int)(object)e.newValue);
                NotifyOfChange();
            });
            return newField;
        }

        /// <inheritdoc />
        protected override void UpdateValue()
        {
            EnumField enumField = (EnumField)m_CellElement;
            enumField.SetValueWithoutNotify(m_CellValue.GetEnum());
        }

        /// <inheritdoc />
        protected override ulong GetDataVersion()
        {
            return m_CellValue.GetDataVersion();
        }

        /// <inheritdoc />
        protected override Serializable.SerializableTypes GetSupportedType()
        {
            return EnumIntCellValue.GetSupportedType();
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}