// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using GDX.Logging;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace GDX.Developer
{
    public class RuntimeConsoleController
    {
        readonly ListView m_ConsoleListView;
        readonly ScrollView m_ConsoleScrollView;
        readonly StringBuilder m_InputBuilder = new StringBuilder(1000);
        readonly Label m_InputLabel;

        readonly VisualElement m_RootElement;
        int m_AutoCompleteOffset = -1;

        int m_CommandBufferOffset = -1;

        uint m_CurrentVersion;
        bool m_IsSubscribedToEvents;

        public RuntimeConsoleController(VisualElement rootElement)
        {
            m_RootElement = rootElement;
            m_InputLabel = m_RootElement.Q<Label>("gdx-console-input");

            // We use a very slimmed down view of the consoles logs here as it takes time to process.
            m_ConsoleListView = m_RootElement.Q<ListView>("gdx-console-list");
            m_ConsoleScrollView = m_RootElement.Q<ScrollView>("");
            m_ConsoleListView.bindItem += BindItem;
            m_ConsoleListView.makeItem += MakeItem;
            m_ConsoleListView.itemsSource = new ManagedLogWrapper();
            m_ConsoleListView.Rebuild();
        }

        public void OnInputVerticalScroll(InputAction.CallbackContext obj)
        {
            m_ConsoleScrollView.scrollOffset =
                new Vector2(
                    m_ConsoleScrollView.scrollOffset.x,
                    m_ConsoleScrollView.scrollOffset.y - obj.ReadValue<float>() * 10f);
        }

        public void OnInputUp(InputAction.CallbackContext obj)
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

        public void OnInputDown(InputAction.CallbackContext obj)
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

        public void OnInputLeft(InputAction.CallbackContext obj)
        {
        }

        public void OnInputRight(InputAction.CallbackContext obj)
        {
        }

        public void OnInputSubmit(InputAction.CallbackContext obj)
        {
            Console.QueueCommand(m_InputLabel.text);
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

        VisualElement MakeItem()
        {
            VisualElement itemBaseElement = new VisualElement { name = "gdx-console-item" };

            Label timestampLabel = new Label { name = "gdx-console-item-timestamp" };
            Label levelLabel = new Label { name = "gdx-console-item-level" };
            Label categoryLabel = new Label { name = "gdx-console-item-category" };
            Label messageLabel = new Label { name = "gdx-console-item-message" };

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

        static void BindItem(VisualElement element, int index)
        {
            LogEntry entry = ManagedLog.GetEntryAt(index);
            element.ClearClassList();
            element.AddToClassList(entry.GetLevelLabel());

            ((Label)element[0]).text = entry.Timestamp.ToString(CultureInfo.InvariantCulture);
            ((Label)element[1]).text = LogLevelToIcon(entry.Level).ToString();
            ((Label)element[2]).text = ManagedLog.GetCategoryLabel(entry.CategoryIdentifier);
            ((Label)element[3]).text = entry.Message;
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
                m_ConsoleListView.RefreshItems();
                m_ConsoleListView.ScrollToItem(-1);
            }
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