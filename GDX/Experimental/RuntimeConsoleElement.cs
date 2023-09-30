// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Text;
using GDX.Experimental.Logging;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace GDX.Experimental
{
    public class RuntimeConsoleElement
    {
        public bool UseFontAwesomeIcons = true;

        readonly StringBuilder m_InputBuilder = new(1000);
        uint m_CurrentVersion;
        int m_CommandBufferOffset = -1;
        int m_AutoCompleteOffset = -1;

        readonly VisualElement m_ConsoleElement;
        readonly VisualElement m_ConsoleContainerElement;
        readonly ListView m_ConsoleListView;
        readonly ScrollView m_ConsoleScrollView;
        readonly VisualElement m_ConsoleBarElement;
        readonly Label m_ConsoleCaretLabel;
        readonly Label m_InputLabel;

        public RuntimeConsoleElement(StyleSheet stylesheet, string caret = "$")
        {
            m_ConsoleElement = new VisualElement() { name = "gdx-console" };
            m_ConsoleContainerElement = new VisualElement() { name = "gdx-console-container" };
            m_ConsoleListView = new ListView()
            {
                name = "gdx-console-list",
                allowAdd = false,
                allowRemove = false,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                selectionType = SelectionType.Single,
                showBoundCollectionSize = false,
                showAlternatingRowBackgrounds = AlternatingRowBackground.None,
                focusable = false
            };

            m_ConsoleBarElement = new VisualElement() { name = "gdx-console-bar" };
            m_ConsoleCaretLabel = new Label() { name = "gdx-console-caret", enableRichText = false, text = caret };
            m_InputLabel = new Label() { name = "gdx-console-input", enableRichText = false };

            m_ConsoleContainerElement.Add(m_ConsoleListView);

            m_ConsoleBarElement.Add(m_ConsoleCaretLabel);
            m_ConsoleBarElement.Add(m_InputLabel);
            m_ConsoleElement.Add(m_ConsoleContainerElement);
            m_ConsoleElement.Add(m_ConsoleBarElement);

            m_ConsoleScrollView = m_ConsoleListView.Q<ScrollView>("");

            if (stylesheet != null)
            {
                m_ConsoleElement.styleSheets.Add(stylesheet);
            }

            m_ConsoleListView.bindItem += BindItem;
            m_ConsoleListView.makeItem += MakeItem;
            m_ConsoleListView.itemsSource = new ManagedLogWrapper();
            m_ConsoleListView.Rebuild();
        }

        char LogLevelToFontAwesomeIcon(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return '\uf3c5';
                case LogLevel.Debug:
                    return '\uf304';
                case LogLevel.Info:
                    return '\uf4a5';
                case LogLevel.Warning:
                    return '\uf071';
                case LogLevel.Error:
                    return '\uf06a';
                case LogLevel.Exception:
                    return '\uf188';
                case LogLevel.Assertion:
                    return '\uf2d3';
                case LogLevel.Fatal:
                    return '\uf1e2';
            }
            return '\uf4a5';
        }
        char LogLevelToIcon(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return '+';
                case LogLevel.Debug:
                    return '+';
                case LogLevel.Info:
                    return '+';
                case LogLevel.Warning:
                    return '-';
                case LogLevel.Error:
                    return '!';
                case LogLevel.Exception:
                    return '!';
                case LogLevel.Assertion:
                    return '!';
                case LogLevel.Fatal:
                    return '!';
            }
            return '+';
        }

        public VisualElement GetVisualElement()
        {
            return m_ConsoleElement;
        }

        public void ClearInput()
        {
            m_InputBuilder.Clear();
            m_InputLabel.text = "";
        }

        public bool HasLabel()
        {
            return m_InputLabel != null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        ///     This needs to be called in a MonoBehaviour's Update method.s
        /// </remarks>
        public void Tick()
        {
            if (ManagedLog.Version == m_CurrentVersion)
            {
                return;
            }

            m_CurrentVersion = ManagedLog.Version;
            m_ConsoleListView.RefreshItems();
            m_ConsoleListView.ScrollToItem(-1);
        }

        public void Show(Action delayedSubscribe)
        {
            m_InputLabel.schedule.Execute(delayedSubscribe);
            Tick();

            // This is bad, we have to scroll to fix things showing
            m_InputLabel.schedule.Execute(() =>
            {
                m_ConsoleListView.ScrollToItem(-1);
            });
        }

        public VisualElement MakeItem()
        {
            VisualElement itemBaseElement = new() { name = "gdx-console-item" };

            Label timestampLabel = new() { name = "gdx-console-item-timestamp" };
            Label levelLabel = new() { name = "gdx-console-item-level" };
            Label categoryLabel = new() { name = "gdx-console-item-category" };
            Label messageLabel = new() { name = "gdx-console-item-message" };

            itemBaseElement.Add(timestampLabel);
            itemBaseElement.Add(levelLabel);
            itemBaseElement.Add(categoryLabel);
            itemBaseElement.Add(messageLabel);

            return itemBaseElement;
        }

        void BindItem(VisualElement element, int index)
        {
            LogEntry entry = ManagedLog.GetEntryAt(index);
            element.ClearClassList();
            element.AddToClassList(entry.GetLevelLabel());

            ((Label)element[0]).text = entry.Timestamp.ToString(CultureInfo.InvariantCulture);

            ((Label)element[1]).text = UseFontAwesomeIcons
                ? LogLevelToFontAwesomeIcon(entry.Level).ToString()
                : LogLevelToIcon(entry.Level).ToString();

            ((Label)element[2]).text = ManagedLog.GetCategoryLabel(entry.CategoryIdentifier);
            ((Label)element[3]).text = entry.Message;
        }


        public void OnInputVerticalScroll(InputAction.CallbackContext obj)
        {
            m_ConsoleScrollView.scrollOffset =
                new Vector2(
                    m_ConsoleScrollView.scrollOffset.x,
                    m_ConsoleScrollView.scrollOffset.y - (obj.ReadValue<float>() * 10f));
        }

        public void OnInputUp(InputAction.CallbackContext obj)
        {
            int bufferCount = DeveloperConsole.PreviousCommandCount;
            if (bufferCount <= 0)
            {
                return;
            }

            m_CommandBufferOffset++;
            if (m_CommandBufferOffset >= bufferCount)
            {
                m_CommandBufferOffset = bufferCount - 1;
            }

            m_InputBuilder.Clear();
            m_InputBuilder.Append(DeveloperConsole.GetPreviousCommand(m_CommandBufferOffset));
            m_InputLabel.text = m_InputBuilder.ToString();
        }

        public void OnInputDown(InputAction.CallbackContext obj)
        {
            int bufferCount = DeveloperConsole.PreviousCommandCount;
            if (bufferCount <= 0)
            {
                return;
            }

            m_CommandBufferOffset--;
            if (m_CommandBufferOffset < 0)
            {
                m_CommandBufferOffset = -1;
                m_InputBuilder.Clear();
            }
            else
            {
                m_InputBuilder.Clear();
                m_InputBuilder.Append(DeveloperConsole.GetPreviousCommand(m_CommandBufferOffset));
            }
            m_InputLabel.text = m_InputBuilder.ToString();
        }

        public void OnInputLeft(InputAction.CallbackContext obj)
        {
        }

        public void OnInputRight(InputAction.CallbackContext obj)
        {
        }

        public void OnInputSubmit(InputAction.CallbackContext obj)
        {
            DeveloperConsole.QueueCommand(m_InputLabel.text);
            m_CommandBufferOffset = -1;
            m_AutoCompleteOffset = -1;
            m_InputLabel.text = "";
            m_InputBuilder.Clear();
            m_InputLabel.text = m_InputBuilder.ToString();
        }

        public void OnInputBackspace(InputAction.CallbackContext obj)
        {
            if (m_InputBuilder.Length >= 1)
            {
                m_InputBuilder.Remove(m_InputBuilder.Length - 1, 1);
                m_InputLabel.text = m_InputBuilder.ToString();
            }
        }

        public void OnInputAutocomplete(InputAction.CallbackContext obj)
        {
            m_AutoCompleteOffset++;

        }

        public void OnTextInput(char inputCharacter)
        {
            // There are a few commands we can process for the console as they have defined text characters (in Ascii)
            int asciiCode = inputCharacter;

            // Outside the range of ASCII we want to support
            if (asciiCode < 32 || asciiCode > 125)
            {
                return;
            }

            m_InputBuilder.Append(inputCharacter);
            m_InputLabel.text = m_InputBuilder.ToString();
        }
    }
}