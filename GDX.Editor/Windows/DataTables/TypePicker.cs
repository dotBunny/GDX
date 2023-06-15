// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using GDX.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace GDX.Editor.Windows.DataTables
{
    public class TypePicker : VisualElement
    {

        BaseField<string> m_Field;
        ListView m_ListView;

        SimpleList<Type> m_AllTypes;
        SimpleList<string> m_SearchStrings;
        List<int> m_TypeIndices;

       // readonly List<Type> m_FilteredTypes = new List<Type>();



        string m_LastQuery;

        int m_AllTypesCount;

        public void SetBaseType(Type baseType)
        {
            TypeCache.TypeCollection collection = TypeCache.GetTypesDerivedFrom(baseType);
            m_AllTypes = new SimpleList<Type>(collection.Count);
            m_SearchStrings = new SimpleList<string>(collection.Count);
            foreach (Type type in collection)
            {
                // Only show public things
                if (!type.IsPublic) continue;
                m_AllTypes.AddUnchecked(type);
                m_SearchStrings.AddUnchecked(type.FullName);
            }
            m_AllTypes.Compact();
            m_AllTypesCount = m_AllTypes.Count;
            m_SearchStrings.Compact();

            m_TypeIndices = new List<int>(m_AllTypesCount);
            for (int i = 0; i < m_AllTypesCount; i++)
            {
                m_TypeIndices[i] = i;
            }
        }

        public TypePicker(TextField textField)
        {
            textField.Add(this);
            textField.RegisterValueChangedCallback(UpdatePickerData);

            m_ListView = new ListView(m_TypeIndices) { name = "gdx-type-list"};
            m_ListView.selectionChanged += ListViewOnselectionChanged;
            m_ListView.makeItem += MakeItem;
            m_ListView.bindItem += BindItem;
            m_ListView.destroyItem += DestroyItem;
            Add(m_ListView);

            AddToClassList("gdx-picker");
        }

        VisualElement MakeItem()
        {
            return new Label();

        }
        void DestroyItem(VisualElement obj)
        {
            //throw new NotImplementedException();
        }

        void BindItem(VisualElement arg1, int arg2)
        {
            Label label = (Label)arg1;
           // label.text = m_TypeIndices[arg2].Name;
        }



        void ListViewOnselectionChanged(IEnumerable<object> obj)
        {
            // Update textfied with more details
        }

        void UpdatePickerData(ChangeEvent<string> evt)
        {
            if (evt.newValue == m_LastQuery) return;

            // string token = evt.newValue;
            // m_FilteredTypes.Clear();
            // for (int i = 0; i < m_AllTypesCount; i++)
            // {
            //     Type type = m_AllTypes.Array[i];
            //     if (type.FullName.Contains(token))
            //     {
            //         m_FilteredTypes.Add(type);
            //     }
            // }
            // m_ListView.RefreshItems();
        }
    }
}