// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.DataTables.CellValues;
using UnityEditor;
using UnityEngine.UIElements;

namespace GDX.Editor.PropertyDrawers.CellValues
{
#if UNITY_2022_2_OR_NEWER
    [CustomPropertyDrawer(typeof(UIntCellValue))]
    public class UIntCellValueDrawer : CellValueDrawerBase
    {
        UIntCellValue m_CellValue;

        /// <inheritdoc />
        protected override VisualElement GetCellElement()
        {
            m_CellValue = new UIntCellValue(m_DataTable, m_RowIdentifier, m_ColumnIdentifier);
#if UNITY_2022_3_OR_NEWER
            UnsignedIntegerField newField = new UnsignedIntegerField(null) { name = k_CellFieldName };
            newField.SetValueWithoutNotify(m_CellValue.Get());
#else
            IntegerField newField = new IntegerField(null) { name = k_CellFieldName };
            newField.SetValueWithoutNotify((int)m_CellValue.Get());
#endif // UNITY_2022_3_OR_NEWER
            newField.RegisterValueChangedCallback(e =>
            {
#if UNITY_2022_3_OR_NEWER
                m_CellValue.Set(e.newValue);
#else
                m_CellValue.Set((uint)e.newValue);
#endif // UNITY_2022_3_OR_NEWER
                NotifyOfChange();
            });
            return newField;
        }

        /// <inheritdoc />
        protected override void UpdateValue()
        {
#if UNITY_2022_3_OR_NEWER
            UnsignedIntegerField cellField = (UnsignedIntegerField)m_CellElement;
            cellField.SetValueWithoutNotify(m_CellValue.Get());
#else
            IntegerField cellField = (IntegerField)m_CellElement;
            cellField.SetValueWithoutNotify((int)m_CellValue.Get());
#endif
        }

        /// <inheritdoc />
        protected override ulong GetDataVersion()
        {
            return m_CellValue.GetDataVersion();
        }

        /// <inheritdoc />
        protected override Serializable.SerializableTypes GetSupportedType()
        {
            return UIntCellValue.GetSupportedType();
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}