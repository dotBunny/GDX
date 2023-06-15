// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using GDX.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_2022_2_OR_NEWER

namespace GDX.Editor.Windows.DataTables
{
    public class TypePicker : VisualElement
    {

        TextField m_Field;
        VisualElement m_TextInput;
        ListView m_ListView;

        // Working data sets for the list view
        SimpleList<Type> m_AllTypes;
        SimpleList<string> m_SearchStrings;
        List<int> m_FilteredTypes;



        string m_LastQuery;

        int m_AllTypesCount;

        public void SetType(Type baseType, bool includeBaseType = true)
        {
            //TODO: add base type
            TypeCache.TypeCollection collection = TypeCache.GetTypesDerivedFrom(baseType);
            m_AllTypes = new SimpleList<Type>(collection.Count);
            m_SearchStrings = new SimpleList<string>(collection.Count);
            foreach (Type type in collection)
            {
                // Only show public things
                if (!type.IsPublic || type.FullName == null) continue;
                m_AllTypes.AddUnchecked(type);
                m_SearchStrings.AddUnchecked(type.FullName);
            }
            m_AllTypes.Compact();
            m_AllTypesCount = m_AllTypes.Count;
            m_SearchStrings.Compact();

            m_FilteredTypes = new List<int>(m_AllTypesCount);
            for (int i = 0; i < m_AllTypesCount; i++)
            {
                m_FilteredTypes.Add(i);
            }
        }

        public TypePicker(TextField textField)
        {
            m_Field = textField;

            m_ListView = new ListView(m_FilteredTypes)
            {
                name = "gdx-type-list",
                showAddRemoveFooter = false,
                reorderable = false,
                showBorder = false,
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                showBoundCollectionSize = false,
                showFoldoutHeader = false
            };
            m_ListView.selectionChanged += ListViewOnselectionChanged;
            m_ListView.makeItem += MakeItem;
            m_ListView.bindItem += BindItem;
            m_ListView.destroyItem += DestroyItem;
            m_ListView.AddToClassList("gdx-picker-list");


            Add(m_ListView);
            AddToClassList("gdx-picker");

            m_TextInput = m_Field.Q("unity-text-input");
            m_Field.Add(this);
            textField.RegisterValueChangedCallback(UpdatePickerData);
            SetLimits();
        }

        void SetLimits()
        {
            
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
            label.text = m_AllTypes.Array[m_FilteredTypes[arg2]].Name;
        }



        void ListViewOnselectionChanged(IEnumerable<object> obj)
        {
            // Update textfied with more details
        }

        void UpdatePickerData(ChangeEvent<string> evt)
        {
            Debug.Log($"QUERY FOR: {evt.newValue}");
            if (evt.newValue == m_LastQuery)
            {
                Debug.Log("OLD DONT");
                return;
            }

            string token = evt.newValue;
            m_FilteredTypes.Clear();

            for (int i = 0; i < m_AllTypesCount; i++)
            {
                Debug.Log($"{m_SearchStrings.Array[i]} contains {token}?");
                if (m_SearchStrings.Array[i].Contains(token))
                {
                    Debug.Log("yes");
                    m_FilteredTypes.Add(i);
                }
            }

            m_ListView.itemsSource = m_FilteredTypes;
            m_ListView.RefreshItems();
            m_LastQuery = evt.newValue;
        }
    }
}

#endif // UNITY_2022_2_OR_NEWER