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
        VisualElement m_TextLabel;
        VisualElement m_TextInput;
        ListView m_ListView;
        VisualElement m_RootElement;

        // Working data sets for the list view
        SimpleList<Type> m_AllTypes;
        SimpleList<string> m_SearchArtifacts;
        SimpleList<string> m_DisplayName;
        List<int> m_FilteredTypes;

        Action m_OnSelected;



        string m_LastQuery;

        int m_AllTypesCount;

        public void SetType(Type baseType, bool includeBaseType = true)
        {
            TypeCache.TypeCollection collection = TypeCache.GetTypesDerivedFrom(baseType);
            int count = collection.Count;
            if (includeBaseType)
            {
                count++;
            }

            m_AllTypes = new SimpleList<Type>(count);
            m_SearchArtifacts = new SimpleList<string>(count);
            m_DisplayName = new SimpleList<string>(count);
            foreach (Type type in collection)
            {
                // Only show public things
                if (!type.IsPublic) continue;

                m_AllTypes.AddUnchecked(type);
                m_SearchArtifacts.AddUnchecked(type.AssemblyQualifiedName.ToLower());
                m_DisplayName.AddUnchecked(type.FullName);
            }


            if (includeBaseType)
            {
                m_AllTypes.AddUnchecked(baseType);
                m_SearchArtifacts.AddUnchecked(baseType.AssemblyQualifiedName.ToLower());
                m_DisplayName.AddUnchecked(baseType.FullName);
            }

            m_AllTypes.Compact();
            m_AllTypesCount = m_AllTypes.Count;
            m_SearchArtifacts.Compact();
            m_DisplayName.Compact();

            m_FilteredTypes = new List<int>(m_AllTypesCount);
            for (int i = 0; i < m_AllTypesCount; i++)
            {
                m_FilteredTypes.Add(i);
            }
        }

        public TypePicker(TextField textField, VisualElement lastChildOfElement, VisualElement rootElement, Action onSelected = null)
        {
            m_Field = textField;
            m_RootElement = rootElement;
            m_OnSelected = onSelected;
            AddToClassList("gdx-picker");

            m_ListView = new ListView(m_FilteredTypes)
            {
                name = "gdx-type-list",
                showAddRemoveFooter = false,
                reorderable = false,
                showBorder = true,
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                showBoundCollectionSize = false,
                showFoldoutHeader = false,
                selectionType = SelectionType.Single,
                fixedItemHeight = 16f,
            };
            m_ListView.selectedIndicesChanged += SelectedItem;
            m_ListView.makeItem += MakeItem;
            m_ListView.bindItem += BindItem;
            m_ListView.destroyItem += DestroyItem;

            m_ListView.AddToClassList("gdx-picker-list");
            Add(m_ListView);


            m_TextLabel = m_Field[0];
            m_TextInput = m_Field[1];

            int index = 0;
            if (lastChildOfElement.childCount > 0)
            {
                index = lastChildOfElement.childCount;
            }
            lastChildOfElement.Insert(index, this);
            textField.RegisterValueChangedCallback(UpdatePickerData);
        }

        void Hide()
        {
            style.display = DisplayStyle.None;
        }

        void Show()
        {
            style.display = DisplayStyle.Flex;
        }

        void SelectedItem(IEnumerable<int> selectedIndices)
        {

            if (m_ListView.selectedIndex == -1) return;
            m_Field.SetValueWithoutNotify(CreateNonVersionedQualifiedName(m_AllTypes.Array[m_FilteredTypes[m_ListView.selectedIndex]]));
            m_OnSelected?.Invoke();
            Hide();
        }

        string CreateNonVersionedQualifiedName(Type type)
        {
            return type.AssemblyQualifiedName;
        }

        public void ScheduleSizeUpdate()
        {
            schedule.Execute(UpdateSizeAndPosition);
        }

        public void UpdateSizeAndPosition()
        {
            // Put to the corner of the input
            style.left = m_TextLabel.resolvedStyle.width + 5;
            style.top = m_Field.layout.yMin + m_TextInput.resolvedStyle.height + 5;


            //m_RootElement.

            // Make sure it doesnt exceed its length
            float inputWidth = m_TextInput.resolvedStyle.width;
            style.width = inputWidth;
            style.maxWidth = inputWidth;
            style.minWidth = inputWidth;

            float available = contentContainer.resolvedStyle.height - resolvedStyle.top - 20f;
            style.maxHeight = 300;


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
            label.text = m_DisplayName.Array[m_FilteredTypes[arg2]];
        }

        void UpdatePickerData(ChangeEvent<string> evt)
        {
            if (evt.newValue == m_LastQuery)
            {
                return;
            }

            string token = evt.newValue.ToLower();
            m_FilteredTypes.Clear();

            // TODO We should cache the artifacts and only search already sorted, need to bind to clear them somehow
            for (int i = 0; i < m_AllTypesCount; i++)
            {
                if (m_SearchArtifacts.Array[i].Contains(token))
                {
                    m_FilteredTypes.Add(i);
                }
            }

            m_ListView.selectedIndex = -1;
            m_ListView.itemsSource = m_FilteredTypes;
            m_ListView.RefreshItems();
            m_LastQuery = evt.newValue;

            if (m_FilteredTypes.Count == 0)
            {
                Hide();
            }
            else
            {
                Show();
                ScheduleSizeUpdate();
            }
        }
    }
}

#endif // UNITY_2022_2_OR_NEWER