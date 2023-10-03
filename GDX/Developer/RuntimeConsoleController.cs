// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using GDX.Logging;
using UnityEngine;
#if GDX_INPUT
using UnityEngine.InputSystem;
#endif
using UnityEngine.UIElements;

namespace GDX.Developer
{
#if UNITY_2022_2_OR_NEWER
    public class RuntimeConsoleController
    {
        // TODO CHANGE TO A MULTICOLUMN LIST VIEW!!!!

        readonly MultiColumnListView m_ConsoleView;
        readonly Label m_InputCaret;
        ScrollView m_ConsoleScrollView;
        readonly StringBuilder m_InputBuilder = new StringBuilder(1000);
        readonly Label m_InputLabel;
        readonly VisualElement m_ConsoleBarElement;
        readonly VisualElement m_TableViewHeader;
        int m_FontSize = 14;

        int m_AutoCompleteOffset = -1;

        int m_CommandBufferOffset = -1;

        uint m_CurrentVersion;
        bool m_IsSubscribedToEvents;
        readonly VisualElement m_RootElement;

        public RuntimeConsoleController(VisualElement rootElement)
        {
            m_RootElement = rootElement.Q<VisualElement>("gdx-console");
            m_ConsoleBarElement = rootElement.Q<VisualElement>("gdx-console-bar");
            m_InputLabel = m_ConsoleBarElement.Q<Label>("gdx-console-input");
            m_InputCaret = m_ConsoleBarElement.Q<Label>("gdx-console-caret");

            // Right now were using a multi column, maybe we want to go to tree for nested hierarchy?
            m_ConsoleView = m_RootElement.Q<MultiColumnListView>("gdx-console-view");
            m_TableViewHeader = m_ConsoleView.Q<VisualElement>(null, "unity-multi-column-header");

            m_ConsoleView.reorderable = false;
            m_ConsoleView.sortingEnabled = false;
            m_ConsoleView.allowAdd = false;
            m_ConsoleView.allowRemove = false;
            m_ConsoleView.showBorder = false;

            //    reorderable="false" resizable="false" sortable="false" stretchable="true"

            m_ConsoleView.itemsSource = new ManagedLogWrapper();
            m_ConsoleView.columns[0].makeCell = () =>
                new Label { name = "gdx-console-item-timestamp", style = { fontSize = m_FontSize } };
            m_ConsoleView.columns[1].makeCell = () =>
                new Label { name = "gdx-console-item-level", style = { fontSize = m_FontSize } };
            m_ConsoleView.columns[2].makeCell = () =>
                new Label { name = "gdx-console-item-category", style = { fontSize = m_FontSize } };
            m_ConsoleView.columns[3].makeCell = () =>
                new Label { name = "gdx-console-item-message", style = { fontSize = m_FontSize } };

            m_ConsoleView.columns[0].bindCell = (element, index) =>
            {
                LogEntry entry = ManagedLog.GetEntryAt(index);
                Label label = (Label)element;
                label.parent.ClearClassList();
                label.parent.AddToClassList(entry.GetLevelLabel());
                label.text = entry.Timestamp.ToString(Localization.LocalTimestampFormat);
            };

            m_ConsoleView.columns[1].bindCell = (element, index) =>
            {
                LogEntry entry = ManagedLog.GetEntryAt(index);
                ((Label)element).text = LogLevelToIcon(entry.Level).ToString();;
            };

            m_ConsoleView.columns[2].bindCell = (element, index) =>
            {
                LogEntry entry = ManagedLog.GetEntryAt(index);
                ((Label)element).text = ManagedLog.GetCategoryLabel(entry.CategoryIdentifier);
            };

            m_ConsoleView.columns[3].bindCell = (element, index) =>
            {
                LogEntry entry = ManagedLog.GetEntryAt(index);
                ((Label)element).text = entry.Message;
            };

            // TODO this puts at risk of stripping
            m_RootElement.schedule.Execute(() => { Reflection.InvokeMethod(m_TableViewHeader, "ResizeToFit");});
        }

        ScrollView GetScrollView()
        {
            if (m_ConsoleScrollView == null)
            {
                m_ConsoleScrollView = m_ConsoleView.Q<ScrollView>("");
            }

            return m_ConsoleScrollView;
        }

        public void UpdateFontSize(int fontSize)
        {
            m_FontSize = fontSize;

            m_ConsoleBarElement.style.height = m_FontSize + 10;
            m_InputCaret.style.fontSize = m_FontSize;
            m_InputLabel.style.fontSize = m_FontSize;

            m_ConsoleView.Rebuild();

            // TODO: UGH!
            Reflection.InvokeMethod(m_TableViewHeader, "ResizeToFit");
        }

#if GDX_INPUT
        public void OnInputVerticalScroll(InputAction.CallbackContext obj)
        {
            ScrollView scrollView = GetScrollView();
            if (scrollView != null)
            {
                scrollView.scrollOffset =
                    new Vector2(
                        scrollView.scrollOffset.x,
                        scrollView.scrollOffset.y - obj.ReadValue<float>() * 10f);
            }
        }
#else
        public void OnInputVerticalScroll(float value)
        {
            ScrollView scrollView = GetScrollView();
            if (scrollView != null)
            {
                scrollView.scrollOffset =
                    new Vector2(
                        scrollView.scrollOffset.x,
                        scrollView.scrollOffset.y - value * 10f);
            }
        }
#endif

#if GDX_INPUT
        public void OnInputUp(InputAction.CallbackContext obj)
#else
        public void OnInputUp()
#endif
        {
            int bufferCount = Console.PreviousCommandCount;
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
            m_InputBuilder.Append(Console.GetPreviousCommand(m_CommandBufferOffset));
            m_InputLabel.text = m_InputBuilder.ToString();
        }

#if GDX_INPUT
        public void OnInputDown(InputAction.CallbackContext obj)
#else
        public void OnInputDown()
#endif
        {
            int bufferCount = Console.PreviousCommandCount;
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
                m_InputBuilder.Append(Console.GetPreviousCommand(m_CommandBufferOffset));
            }

            m_InputLabel.text = m_InputBuilder.ToString();
        }

#if GDX_INPUT
        public void OnInputLeft(InputAction.CallbackContext obj)
#else
        public void OnInputLeft()
#endif
        {
        }

#if GDX_INPUT
        public void OnInputRight(InputAction.CallbackContext obj)
#else
        public void OnInputRight()
#endif
        {
        }

#if GDX_INPUT
        public void OnInputSubmit(InputAction.CallbackContext obj)
#else
        public void OnInputSubmit()
#endif
        {
            Console.QueueCommand(m_InputLabel.text);
            m_CommandBufferOffset = -1;
            m_AutoCompleteOffset = -1;
            m_InputLabel.text = "";
            m_InputBuilder.Clear();
            m_InputLabel.text = m_InputBuilder.ToString();
        }

#if GDX_INPUT
        public void OnInputBackspace(InputAction.CallbackContext obj)
#else
        public void OnInputBackspace()
#endif
        {
            if (m_InputBuilder.Length >= 1)
            {
                m_InputBuilder.Remove(m_InputBuilder.Length - 1, 1);

                m_InputLabel.text = m_InputBuilder.ToString();
            }
        }

#if GDX_INPUT
        public void OnInputAutocomplete(InputAction.CallbackContext obj)
#else
        public void OnInputAutocomplete()
#endif
        {
            m_AutoCompleteOffset++;
        }


        VisualElement MakeItem()
        {
            VisualElement itemBaseElement = new VisualElement { name = "gdx-console-item" };

            Label timestampLabel = new Label { name = "gdx-console-item-timestamp", style = { fontSize = m_FontSize}};
            Label levelLabel = new Label { name = "gdx-console-item-level" , style = { fontSize = m_FontSize}};
            Label categoryLabel = new Label { name = "gdx-console-item-category" , style = { fontSize = m_FontSize}};
            Label messageLabel = new Label { name = "gdx-console-item-message", style = { fontSize = m_FontSize}};

            itemBaseElement.Add(timestampLabel);
            itemBaseElement.Add(levelLabel);
            itemBaseElement.Add(categoryLabel);
            itemBaseElement.Add(messageLabel);

            return itemBaseElement;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static char LogLevelToIcon(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return '\uf3c5';
                case LogLevel.Debug:
                    return '\uf304';
                case LogLevel.Info:
                    return '\uf27a';
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

            return '\uf27a';
        }

        public void ClearInput()
        {
            m_InputBuilder.Clear();
            m_InputLabel.text = "";
        }

        public void Tick()
        {
            if (ManagedLog.Version != m_CurrentVersion)
            {
                m_CurrentVersion = ManagedLog.Version;
                m_ConsoleView.RefreshItems();
                m_ConsoleView.ScrollToItem(-1);
            }
        }

        public void Show(Action delayedSubscribe)
        {
            m_InputLabel.schedule.Execute(delayedSubscribe);
            Tick();

            // This is bad, we have to scroll to fix things showing
            m_InputLabel.schedule.Execute(() =>
            {
                m_ConsoleView.ScrollToItem(-1);
            });
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
#endif // UNITY_2022_2_OR_NEWER
}