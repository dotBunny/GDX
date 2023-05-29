// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Tables.CellValues;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GDX.Editor.PropertyDrawers.CellValues
{
#if UNITY_2022_2_OR_NEWER
    [CustomPropertyDrawer(typeof(LayerMaskCellValue))]
    public class LayerMaskCellValueDrawer : CellValueDrawerBase
    {
        LayerMaskCellValue m_CellValue;

        /// <inheritdoc />
        protected override VisualElement GetCellElement()
        {
            m_CellValue = new LayerMaskCellValue(m_Table, m_RowInternalIndex, m_ColumnInternalIndex);
            LayerMaskField newField = new LayerMaskField(null) { name = k_CellFieldName };
            newField.SetValueWithoutNotify(m_CellValue.Get());
            newField.RegisterValueChangedCallback(e =>
            {
                m_CellValue.Set(e.newValue);
                NotifyOfChange();
            });
            return newField;
        }

        /// <inheritdoc />
        protected override ulong GetDataVersion()
        {
            return m_CellValue.GetDataVersion();
        }

        /// <inheritdoc />
        protected override Serializable.SerializableTypes GetSupportedType()
        {
            return LayerMaskCellValue.GetSupportedType();
        }
    }
#endif
}