// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using GDX.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.VisualElements
{
#if UNITY_2022_2_OR_NEWER
    public class TypePicker : VisualElement
    {
        readonly VisualElement m_ContainerElement;
        readonly TextField m_Field;
        readonly ListView m_ListView;
        readonly Action m_OnSelected;
        readonly VisualElement m_TextInput;
        readonly VisualElement m_TextLabel;

        SimpleList<string> m_DisplayName;
        List<int> m_FilteredTypes;
        bool m_HasEntered;
        string m_LastQuery;
        bool m_Listening;
        SimpleList<string> m_Namespace;
        int m_TypeCount;
        SimpleList<string> m_TypeFullNames;

        // Working data sets for the list view
        SimpleList<string> m_TypeQualifiedNames;

        public TypePicker(TextField textField, VisualElement lastChildOfElement, VisualElement containerElement,
            Action onSelected = null)
        {
            m_Field = textField;
            m_ContainerElement = containerElement;
            m_OnSelected = onSelected;

            AddToClassList("gdx-picker");
            ResourcesProvider.SetupSharedStylesheets(this);
            ResourcesProvider.SetupStylesheet("GDXTypePicker", this);
            ResourcesProvider.CheckTheme(this);

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
            m_ListView.selectedIndicesChanged += OnSelectedIndicesChanged;
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
            textField.RegisterValueChangedCallback(OnValueChange);

            m_Field.RegisterCallback<DetachFromPanelEvent>(OnFieldDetachFromPanelEvent);
            m_Field.RegisterCallback<KeyDownEvent>(OnFieldKeyboardEvent);

            style.display = DisplayStyle.None;
        }

        public void Hide()
        {
            if (style.display != DisplayStyle.Flex)
            {
                return;
            }

            m_HasEntered = false;

            m_ContainerElement.UnregisterCallback<MouseLeaveWindowEvent>(OnContainerMouseLeaveWindowEvent);
            m_ContainerElement.UnregisterCallback<GeometryChangedEvent>(OnContainerGeometryChanged);
            m_ContainerElement.UnregisterCallback<MouseDownEvent>(OnContainerMouseDownEvent);
            UnregisterCallback<MouseEnterEvent>(OnMouseEnterEvent);
            UnregisterCallback<MouseLeaveEvent>(OnMouseLeaveEvent);

            style.display = DisplayStyle.None;
        }

        public void ScheduleUpdateSizeAndPosition()
        {
            schedule.Execute(UpdateSizeAndPosition);
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
            m_TypeFullNames = new SimpleList<string>(count);
            m_DisplayName = new SimpleList<string>(count);
            m_Namespace = new SimpleList<string>(count);
            foreach (Type type in collection)
            {
                // Only show public things
                if (type.IsNotPublic)
                {
                    continue;
                }

                if (type.FullName == null)
                {
                    continue;
                }

                m_TypeQualifiedNames.AddUnchecked(Reflection.GetTypeQualifiedName(type));
                m_TypeFullNames.AddUnchecked(type.FullName.ToUpper());
                m_DisplayName.AddUnchecked(type.Name);
                m_Namespace.AddUnchecked(type.Namespace);
            }

            if (includeBaseType && baseType.FullName != null)
            {
                m_TypeQualifiedNames.AddUnchecked(Reflection.GetTypeQualifiedName(baseType));
                m_TypeFullNames.AddUnchecked(baseType.FullName.ToUpper());
                m_DisplayName.AddUnchecked(baseType.Name);
                m_Namespace.AddUnchecked(baseType.Namespace);
            }

            m_TypeQualifiedNames.Compact();
            m_TypeFullNames.Compact();
            m_TypeCount = m_TypeQualifiedNames.Count;
            m_DisplayName.Compact();
            m_Namespace.Compact();

            m_FilteredTypes = new List<int>(m_TypeCount);
            for (int i = 0; i < m_TypeCount; i++)
            {
                m_FilteredTypes.Add(i);
            }
        }

        public void Show()
        {
            if (style.display == DisplayStyle.Flex)
            {
                return;
            }

            m_HasEntered = false;
            m_ContainerElement.RegisterCallback<MouseLeaveWindowEvent>(OnContainerMouseLeaveWindowEvent);
            m_ContainerElement.RegisterCallback<GeometryChangedEvent>(OnContainerGeometryChanged);
            m_ContainerElement.RegisterCallback<MouseDownEvent>(OnContainerMouseDownEvent);

            RegisterCallback<MouseEnterEvent>(OnMouseEnterEvent);
            RegisterCallback<MouseLeaveEvent>(OnMouseLeaveEvent);

            style.display = DisplayStyle.Flex;
        }

        void OnMouseEnterEvent(MouseEnterEvent evt)
        {
            m_HasEntered = true;
        }

        void OnMouseLeaveEvent(MouseLeaveEvent evt)
        {
            if (m_HasEntered)
            {
                Hide();
            }
        }

        void BindItem(VisualElement container, int index)
        {
            int filteredIndex = m_FilteredTypes[index];
            Label title = (Label)container[0];
            title.text = m_DisplayName.Array[filteredIndex];
            Label description = (Label)container[1];
            description.text = m_Namespace.Array[filteredIndex];
        }

        void OnContainerGeometryChanged(GeometryChangedEvent evt)
        {
            UpdateSizeAndPosition();
        }

        void OnContainerMouseDownEvent(MouseDownEvent evt)
        {
            if (style.display == DisplayStyle.Flex)
            {
                Hide();
            }
        }

        void OnContainerMouseLeaveWindowEvent(MouseLeaveWindowEvent evt)
        {
            if (style.display == DisplayStyle.Flex)
            {
                Hide();
            }
        }

        void OnFieldDetachFromPanelEvent(DetachFromPanelEvent evt)
        {
            Hide();

            m_Field.UnregisterCallback<KeyDownEvent>(OnFieldKeyboardEvent);
            m_Field.UnregisterCallback<DetachFromPanelEvent>(OnFieldDetachFromPanelEvent);
        }

        void OnFieldKeyboardEvent(KeyDownEvent evt)
        {
            // Escape to cancel overlay
            if (style.display == DisplayStyle.Flex && evt.keyCode == KeyCode.Escape)
            {
                Hide();
            }
        }

        void OnSelectedIndicesChanged(IEnumerable<int> selectedIndices)
        {
            if (m_ListView.selectedIndex == -1)
            {
                return;
            }

            m_Field.SetValueWithoutNotify(m_TypeQualifiedNames.Array[m_FilteredTypes[m_ListView.selectedIndex]]);
            m_OnSelected?.Invoke();
            Hide();
        }

        void OnValueChange(ChangeEvent<string> evt)
        {
            if (evt.newValue == m_LastQuery)
            {
                return;
            }

            if (evt.newValue == null)
            {
                m_FilteredTypes.Clear();
                Hide();
                return;
            }

            string token = evt.newValue.ToUpper();

            // Optimized search
            if (evt.previousValue != null && evt.newValue.StartsWith(evt.previousValue))
            {
                for (int i = m_FilteredTypes.Count - 1; i >= 0; i--)
                {
                    // We use ordinal because we already have adjusted the case
                    if (m_TypeFullNames.Array[m_FilteredTypes[i]].IndexOf(token, StringComparison.Ordinal) == -1)
                    {
                        m_FilteredTypes.RemoveAt(i);
                    }
                }
            }
            else
            {
                // Full search
                m_FilteredTypes.Clear();
                for (int i = 0; i < m_TypeCount; i++)
                {
                    if (m_TypeFullNames.Array[i].IndexOf(token, StringComparison.Ordinal) != -1)
                    {
                        m_FilteredTypes.Add(i);
                    }
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
                ScheduleUpdateSizeAndPosition();
            }
        }

        void UpdateSizeAndPosition()
        {
            // Put to the corner of the input
            style.left = m_TextLabel.resolvedStyle.width + 5;
            style.top = m_Field.layout.yMin + m_TextInput.resolvedStyle.height + 5;

            // Make sure it doesnt exceed its length
            float inputWidth = m_TextInput.resolvedStyle.width;
            style.width = inputWidth;
            style.maxWidth = inputWidth;
            style.minWidth = inputWidth;

            // Make sure we dont exceed our container
            style.maxHeight = m_ContainerElement.resolvedStyle.height - m_Field.worldBound.yMin - 25f;
        }

        static VisualElement MakeItem()
        {
            VisualElement container = new VisualElement();
            container.AddToClassList("gdx-picker-item");
            container.Add(new Label { name = "gdx-picker-item-title" });
            container.Add(new Label { name = "gdx-picker-item-description" });
            return container;
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}