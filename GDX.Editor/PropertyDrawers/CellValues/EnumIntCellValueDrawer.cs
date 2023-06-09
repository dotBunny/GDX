// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using GDX.DataTables.CellValues;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GDX.Editor.PropertyDrawers.CellValues
{
#if UNITY_2022_2_OR_NEWER
    [CustomPropertyDrawer(typeof(ObjectCellValue))]
    public class EnumIntCellValueDrawer : CellValueDrawerBase
    {
        EnumIntCellValue m_CellValue;

        /// <inheritdoc />
        protected override VisualElement GetCellElement()
        {
            m_CellValue = new EnumIntCellValue(m_DataTable, m_RowIdentifier, m_ColumnIdentifier);
            EnumField newField = new EnumField(null, null) { name = k_CellFieldName };

            string qualifiedType = m_DataTable.GetTypeNameForColumn(m_ColumnIdentifier);
            // newField.objectType = (!string.IsNullOrEmpty(qualifiedType) ? System.Type.GetType(qualifiedType) : typeof(UnityEngine.Object)) ??
            //                       typeof(UnityEngine.Object);
            //
            // newField.SetValueWithoutNotify(m_CellValue.Get());
            newField.RegisterValueChangedCallback(e =>
            {
                DataTableTracker.RecordCellValueUndo(m_TableTicket, m_RowIdentifier, m_ColumnIdentifier);
                //m_CellValue.Set(e.newValue);
                NotifyOfChange();
            });
            return newField;
        }

        /// <inheritdoc />
        protected override void UpdateValue()
        {
            // TODO: maybe this is jsut a dropdown we manage?
            EnumField cellField = (EnumField)m_CellElement;
          //  cellField.SetValueWithoutNotify(m_CellValue.Get());
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