﻿// Copyright (c) 2020-2024 dotBunny Inc.
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
            m_CellValue = new ULongCellValue(m_DataTable, m_RowIdentifier, m_ColumnIdentifier);
#if UNITY_2022_3_OR_NEWER
            UnsignedLongField newField = new UnsignedLongField(null) { name = k_CellFieldName };
            newField.SetValueWithoutNotify(m_CellValue.Get());
#else
            LongField newField = new LongField(null) { name = k_CellFieldName };
            newField.SetValueWithoutNotify((long)m_CellValue.Get());
#endif // UNITY_2022_3_OR_NEWER

            newField.RegisterValueChangedCallback(e =>
            {
                DataTableTracker.RecordCellValueUndo(m_TableTicket, m_RowIdentifier, m_ColumnIdentifier);
#if UNITY_2022_3_OR_NEWER
                m_CellValue.Set(e.newValue);
#else
                m_CellValue.Set((ulong)e.newValue);
#endif // UNITY_2022_3_OR_NEWER
                NotifyOfChange();
            });
            return newField;
        }

        /// <inheritdoc />
        protected override void UpdateValue()
        {
#if UNITY_2022_3_OR_NEWER
            UnsignedLongField cellField = (UnsignedLongField)m_CellElement;
            cellField.SetValueWithoutNotify(m_CellValue.Get());
#else
            LongField cellField = (LongField)m_CellElement;
            cellField.SetValueWithoutNotify((long)m_CellValue.Get());
#endif // UNITY_2022_3_OR_NEWER
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
#endif // UNITY_2022_2_OR_NEWER
}