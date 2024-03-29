﻿// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.DataTables.CellValues;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.PropertyDrawers.CellValues
{
#if UNITY_2022_2_OR_NEWER
    [CustomPropertyDrawer(typeof(QuaternionCellValue))]
    public class QuaternionCellValueDrawer : CellValueDrawerBase
    {
        QuaternionCellValue m_CellValue;

        /// <inheritdoc />
        protected override VisualElement GetCellElement()
        {
            m_CellValue = new QuaternionCellValue(m_DataTable, m_RowIdentifier, m_ColumnIdentifier);
            Vector4Field newField = new Vector4Field(null) { name = k_CellFieldName };

            Quaternion localQuaternion = m_CellValue.Get();
            Vector4 fieldValue =
                new Vector4(localQuaternion.x, localQuaternion.y, localQuaternion.z, localQuaternion.w);

            newField.SetValueWithoutNotify(fieldValue);
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
            Vector4Field cellField = (Vector4Field)m_CellElement;

            Quaternion localQuaternion = m_CellValue.Get();
            Vector4 fieldValue =
                new Vector4(localQuaternion.x, localQuaternion.y, localQuaternion.z, localQuaternion.w);

            cellField.SetValueWithoutNotify(fieldValue);
        }

        /// <inheritdoc />
        protected override ulong GetDataVersion()
        {
            return m_CellValue.GetDataVersion();
        }

        /// <inheritdoc />
        protected override Serializable.SerializableTypes GetSupportedType()
        {
            return QuaternionCellValue.GetSupportedType();
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}