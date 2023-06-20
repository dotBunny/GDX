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

namespace GDX.Editor.VisualElements
{
    public class TypePicker : VisualElement
    {
        readonly TextField m_Field;
        readonly ListView m_ListView;
        readonly Action m_OnSelected;
        readonly VisualElement m_RootElement;
        readonly VisualElement m_TextInput;
        readonly VisualElement m_TextLabel;

        SimpleList<string> m_DisplayName;
        List<int> m_FilteredTypes;
        string m_LastQuery;
        SimpleList<string> m_Namespace;
        int m_TypeCount;

        // Working data sets for the list view
        SimpleList<string> m_TypeQualifiedNames;

        public TypePicker(TextField textField, VisualElement lastChildOfElement, VisualElement rootElement,
            Action onSelected = null)
        {
            m_Field = textField;
            m_RootElement = rootElement;
            m_OnSelected = onSelected;

            ResourcesProvider.SetupSharedStylesheets(m_RootElement);
            ResourcesProvider.SetupStylesheet("GDXTypePicker", m_RootElement);
            ResourcesProvider.CheckTheme(m_RootElement);

            m_Field.RegisterCallback<KeyDownEvent>(OnKeyboardEvent);
            m_Field.RegisterCallback<DetachFromPanelEvent>(Unbind);


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
                fixedItemHeight = 38f
            };
            m_ListView.selectedIndicesChanged += SelectedItem;
            m_ListView.makeItem += MakeItem;
            m_ListView.bindItem += BindItem;

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

        void OnKeyboardEvent(KeyDownEvent evt)
        {
            // Escape to cancel overlay
            if (style.display == DisplayStyle.Flex && evt.keyCode == KeyCode.Escape)
            {
                Hide();
            }
        }


        void Unbind(DetachFromPanelEvent evt)
        {
            m_Field.UnregisterCallback<DetachFromPanelEvent>(Unbind);
            m_Field.UnregisterCallback<KeyDownEvent>(OnKeyboardEvent);
        }

        public void SetType(Type baseType, bool includeBaseType = true)
        {
            TypeCache.TypeCollection collection = TypeCache.GetTypesDerivedFrom(baseType);
            int count = collection.Count;
            if (includeBaseType)
            {
                count++;
            }

            m_TypeQualifiedNames = new SimpleList<string>(count);
            m_DisplayName = new SimpleList<string>(count);
            m_Namespace = new SimpleList<string>(count);
            foreach (Type type in collection)
            {
                // Only show public things
                if (!type.IsPublic)
                {
                    continue;
                }

                m_TypeQualifiedNames.AddUnchecked(Reflection.GetTypeQualifiedName(type));
                m_DisplayName.AddUnchecked(type.Name);
                m_Namespace.AddUnchecked(type.Namespace);
            }

            if (includeBaseType)
            {
                m_TypeQualifiedNames.AddUnchecked(Reflection.GetTypeQualifiedName(baseType));
                m_DisplayName.AddUnchecked(baseType.Name);
                m_Namespace.AddUnchecked(baseType.Namespace);
            }

            m_TypeQualifiedNames.Compact();
            m_TypeCount = m_TypeQualifiedNames.Count;
            m_DisplayName.Compact();
            m_Namespace.Compact();

            m_FilteredTypes = new List<int>(m_TypeCount);
            for (int i = 0; i < m_TypeCount; i++)
            {
                m_FilteredTypes.Add(i);
            }
        }

        void Hide()
        {
            style.display = DisplayStyle.None;

            // Unbind escape
        }

        void Show()
        {
            style.display = DisplayStyle.Flex;

            // Bind escape // need to block table
        }

        void SelectedItem(IEnumerable<int> selectedIndices)
        {
            if (m_ListView.selectedIndex == -1)
            {
                return;
            }

            m_Field.SetValueWithoutNotify(m_TypeQualifiedNames.Array[m_FilteredTypes[m_ListView.selectedIndex]]);
            m_OnSelected?.Invoke();
            Hide();
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

            // TODO: #GDX-113
            float available = contentContainer.resolvedStyle.height - resolvedStyle.top - 20f;
            style.maxHeight = 300;
        }

        VisualElement MakeItem()
        {
            VisualElement container = new VisualElement();
            container.AddToClassList("gdx-picker-item");
            container.Add(new Label { name = "gdx-picker-item-title" });
            container.Add(new Label { name = "gdx-picker-item-description" });
            return container;
        }

        void BindItem(VisualElement container, int index)
        {
            Label title = (Label)container[0];
            title.text = m_DisplayName.Array[m_FilteredTypes[index]];
            Label description = (Label)container[1];
            description.text = m_Namespace.Array[m_FilteredTypes[index]];
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
            for (int i = 0; i < m_TypeCount; i++)
            {
                if (m_TypeQualifiedNames.Array[i].Contains(token, StringComparison.InvariantCultureIgnoreCase))
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