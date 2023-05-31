﻿// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.DataTables.CellValues;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GDX.Editor.PropertyDrawers.CellValues
{
#if UNITY_2022_2_OR_NEWER
    [CustomPropertyDrawer(typeof(ObjectCellValue))]
    public class ObjectCellValueDrawer : CellValueDrawerBase
    {
        ObjectCellValue m_CellValue;

        /// <inheritdoc />
        protected override VisualElement GetCellElement()
        {
            m_CellValue = new ObjectCellValue(m_DataTable, m_RowInternalIndex, m_ColumnInternalIndex);
            ObjectField newField = new ObjectField(null) { name = k_CellFieldName };

            string qualifiedType = m_DataTable.GetTypeNameForObjectColumn(m_ColumnInternalIndex);
            newField.objectType = (!string.IsNullOrEmpty(qualifiedType) ? System.Type.GetType(qualifiedType) : typeof(UnityEngine.Object)) ??
                                  typeof(UnityEngine.Object);

            newField.SetValueWithoutNotify(m_CellValue.Get());
            newField.RegisterValueChangedCallback(e =>
            {
                m_CellValue.Set(e.newValue);
                NotifyOfChange();
            });
            return newField;
        }

        /// <inheritdoc />
        protected override void UpdateValue()
        {
            ObjectField cellField = (ObjectField)m_CellElement;
            cellField.SetValueWithoutNotify(m_CellValue.Get());
        }

        /// <inheritdoc />
        protected override ulong GetDataVersion()
        {
            return m_CellValue.GetDataVersion();
        }

        /// <inheritdoc />
        protected override Serializable.SerializableTypes GetSupportedType()
        {
            return ObjectCellValue.GetSupportedType();
        }
    }
#endif
}