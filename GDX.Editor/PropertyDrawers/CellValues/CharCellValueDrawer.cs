// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Tables;
using GDX.Tables.CellValues;
using UnityEditor;
using UnityEngine.UIElements;

namespace GDX.Editor.PropertyDrawers.CellValues
{
    [CustomPropertyDrawer(typeof(CharCellValue))]
    public class CharCellValueDrawer : CellValueDrawerBase
    {
        CharCellValue m_CellValue;

        /// <inheritdoc />
        protected override VisualElement GetCellElement()
        {
            m_CellValue = new CharCellValue(m_Table, m_RowInternalIndex, m_ColumnInternalIndex);
            TextField newField = new TextField(null, 1, false, false, ' ') { name = k_CellFieldName };
            newField.SetValueWithoutNotify(m_CellValue.Get().ToString());
            newField.RegisterValueChangedCallback(e =>
            {
                m_CellValue.Set(e.newValue[0]);
                NotifyOfChange();
            });
            return newField;
        }

        /// <inheritdoc />
        protected override Serializable.SerializableTypes GetSupportedType()
        {
            return CharCellValue.GetSupportedType();
        }
    }
}