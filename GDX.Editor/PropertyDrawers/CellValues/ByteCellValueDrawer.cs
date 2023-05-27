// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Tables;
using GDX.Tables.CellValues;
using UnityEditor;
using UnityEngine.UIElements;

namespace GDX.Editor.PropertyDrawers.CellValues
{
    [CustomPropertyDrawer(typeof(ByteCellValue))]
    public class ByteCellValueDrawer : CellValueDrawerBase
    {
        ByteCellValue m_CellValue;
        /// <inheritdoc />
        protected override void Init(SerializedProperty serializedProperty)
        {
            if (Reflection.TryGetFieldValue(serializedProperty.serializedObject.targetObject, typeof(ByteCellValue),
                    serializedProperty.name, out ByteCellValue cell))
            {
                m_CellValue = cell;
                if (m_CellValue.Table != null)
                {
                    m_Table = m_CellValue.Table;
                    m_RowInternalIndex = m_CellValue.Row;
                    m_ColumnInternalIndex = m_CellValue.Column;
                }
                else
                {
                    m_RowInternalIndex = -1;
                    m_ColumnInternalIndex = -1;
                }
            }
        }

        /// <inheritdoc />
        protected override void CreateCellValue(TableBase table, int rowInternalIndex, int columnInternalIndex)
        {
            if (table != null && rowInternalIndex != -1 && columnInternalIndex != -1)
            {
                m_CellValue = new ByteCellValue(table, rowInternalIndex, columnInternalIndex);
            }
        }

        /// <inheritdoc />
        protected override VisualElement GetCellElement()
        {
            SliderInt newField = new SliderInt(byte.MinValue, byte.MaxValue)
            {
                name = k_CellFieldName, showInputField = true, label = null
            };
            newField.SetValueWithoutNotify(m_CellValue.Get());
            newField.RegisterValueChangedCallback(e =>
            {
                m_CellValue.Set((byte)e.newValue);
                NotifyOfChange();
            });
            return newField;
        }

        /// <inheritdoc />
        protected override Serializable.SerializableTypes GetSupportedType()
        {
            return ByteCellValue.GetSupportedType();
        }
    }
}