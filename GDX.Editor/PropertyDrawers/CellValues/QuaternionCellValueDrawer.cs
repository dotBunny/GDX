// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Tables;
using GDX.Tables.CellValues;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.PropertyDrawers.CellValues
{
    [CustomPropertyDrawer(typeof(QuaternionCellValue))]
    public class QuaternionCellValueDrawer : CellValueDrawerBase
    {
        QuaternionCellValue m_CellValue;

        /// <inheritdoc />
        protected override VisualElement GetCellElement()
        {
            m_CellValue = new QuaternionCellValue(m_Table, m_RowInternalIndex, m_ColumnInternalIndex);
            Vector4Field newField = new Vector4Field(null) { name = k_CellFieldName };

            Quaternion localQuaternion = m_CellValue.Get();
            Vector4 fieldValue =
                new Vector4(localQuaternion.x, localQuaternion.y, localQuaternion.z, localQuaternion.w);

            newField.SetValueWithoutNotify(fieldValue);
            newField.RegisterValueChangedCallback(e =>
            {
                m_CellValue.Set(e.newValue);
                NotifyOfChange();
            });
            return newField;
        }

        /// <inheritdoc />
        protected override Serializable.SerializableTypes GetSupportedType()
        {
            return QuaternionCellValue.GetSupportedType();
        }
    }
}