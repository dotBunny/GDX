﻿// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using GDX.Developer.ConsoleCommands;
using GDX.RuntimeContent;
using UnityEngine;
#if GDX_INPUT
using UnityEngine.InputSystem;
#endif
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GDX.Developer
{
#if UNITY_2022_2_OR_NEWER
    public class RuntimeConsoleController
    {
        const string k_GameObjectName = "GDX_RuntimeConsole";

        // Maybe expose these for people messing with fonts
        const int k_SourceFontSize = 14;
        const int k_FrameCountWidth = 100;
        const int k_SourceLevelWidth = 22;

        readonly ListView m_ConsoleListView;
        readonly Label m_InputCaret;
        readonly ScrollView m_ConsoleScrollView;
        readonly StringBuilder m_InputBuilder = new StringBuilder(1000);
        readonly Label m_InputLabel;
        readonly Label m_SuggestionLabel;
        readonly VisualElement m_ConsoleBarElement;

        int m_CommandBufferOffset = -1;

        uint m_CurrentVersion;
        bool m_IsSubscribedToEvents;
        readonly VisualElement m_RootElement;

        int m_FontSize = 14;
        float m_FontSizeMultiplier = 1f;

        StyleLength m_FrameWidth = new StyleLength(new Length(k_FrameCountWidth, LengthUnit.Pixel));
        StyleLength m_LevelWidth = new StyleLength(new Length(k_SourceLevelWidth, LengthUnit.Pixel));

        public UIDocument Document { get; private set; }
        public GameObject ConsoleGameObject { get; private set; }

        public RuntimeConsoleController(GameObject parentGameObject, int initialFontSize)
        {
#if UNITY_EDITOR

            // Helps with domain reload, removes the old object, and recreates
            if (parentGameObject.transform.childCount != 0)
            {
                int childCount = parentGameObject.transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    if (parentGameObject.transform.GetChild(i).gameObject.name == k_GameObjectName)
                    {
                        Object.Destroy(parentGameObject.transform.GetChild(i).gameObject);
                        break;
                    }
                }
            }
#endif

            // UIDocuments do not allow multiple components per Game Object so we have to make a child object.
            ConsoleGameObject = new GameObject(k_GameObjectName);
            ConsoleGameObject.transform.SetParent(parentGameObject.transform, false);

            // Create isolated UI document  (thanks Damian, boy do I feel stupid.)
            Document = ConsoleGameObject.AddComponent<UIDocument>();
            Document.sortingOrder = float.MaxValue; // Above all
            Document.visualTreeAsset = ResourceProvider.GetUIElements().RuntimeConsole;

            // Build out the required references
            m_RootElement = Document.rootVisualElement.Q<VisualElement>("gdx-console");
            m_ConsoleBarElement = m_RootElement.Q<VisualElement>("gdx-console-bar");
            m_InputLabel = m_ConsoleBarElement.Q<Label>("gdx-console-input");
            m_SuggestionLabel = m_ConsoleBarElement.Q<Label>("gdx-console-suggestion");
            m_InputCaret = m_ConsoleBarElement.Q<Label>("gdx-console-caret");

            // We use a very slimmed down view of the consoles logs here as it takes time to process.
            m_ConsoleListView = m_RootElement.Q<ListView>("gdx-console-list");
            m_ConsoleScrollView = m_RootElement.Q<ScrollView>("");
            m_ConsoleListView.bindItem += BindItem;
            m_ConsoleListView.makeItem += MakeItem;
            m_ConsoleListView.itemsSource = Console.Log;

            UpdateFontSize(initialFontSize);
        }

        public void UpdateFontSize(int fontSize)
        {
            m_FontSize = fontSize;
            m_FontSizeMultiplier = (float)m_FontSize / k_SourceFontSize;

            // Calculate our Category with based on the managed log longest
            m_FrameWidth = new StyleLength(new Length(Mathf.RoundToInt(k_FrameCountWidth * m_FontSizeMultiplier)));
            m_LevelWidth = new StyleLength(new Length(Mathf.RoundToInt(k_SourceLevelWidth * m_FontSizeMultiplier)));
            m_ConsoleBarElement.style.height = fontSize + 10;
            m_InputCaret.style.fontSize = fontSize;
            m_InputLabel.style.fontSize = fontSize;
            m_SuggestionLabel.style.fontSize = fontSize;

            m_ConsoleListView.Rebuild();
        }

        public void OnInputVerticalScroll(float delta)
        {
            m_ConsoleScrollView.Focus();
            m_ConsoleScrollView.scrollOffset =
                new Vector2(
                    m_ConsoleScrollView.scrollOffset.x,
                    m_ConsoleScrollView.scrollOffset.y - delta * 10f);
        }

        public void OnInputUp()
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

            ClearSuggestion();
        }

        public void OnInputDown()
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

            m_SuggestionLabel.text = string.Empty;
            m_InputLabel.text = m_InputBuilder.ToString();
        }


        public void OnInputLeft()
        {
        }

        public void OnInputRight()
        {

        }

        public void OnInputSubmit()
        {
            if (m_SuggestionLabel.text != string.Empty)
            {
                m_InputBuilder.Append(m_SuggestionLabel.text);
                m_InputLabel.text = m_InputBuilder.ToString();
            }
            else
            {
                Console.QueueCommand(m_InputLabel.text);
                m_CommandBufferOffset = -1;
                m_InputLabel.text = "";
                m_InputBuilder.Clear();
                m_InputLabel.text = m_InputBuilder.ToString();
            }
            ClearSuggestion();
        }

        public void OnInputBackspace()
        {
            if (m_InputBuilder.Length >= 1)
            {
                m_InputBuilder.Remove(m_InputBuilder.Length - 1, 1);

                m_InputLabel.text = m_InputBuilder.ToString();

                ResetSuggestion();
            }
        }

        public void OnInputAutocomplete()
        {
            m_SuggestionLabel.text = ConsoleAutoCompleteProvider.UpdateSuggestion(m_InputLabel.text)
                ? ConsoleAutoCompleteProvider.GetCurrentSuggestion()
                : string.Empty;
        }

        void ClearSuggestion()
        {
            m_SuggestionLabel.text = string.Empty;
            ConsoleAutoCompleteProvider.Reset();
        }
        void ResetSuggestion()
        {
            if (m_SuggestionLabel.text != string.Empty)
            {
                ConsoleAutoCompleteProvider.Reset();
                OnInputAutocomplete();
            }
        }

        public void OnInputPageUp()
        {
            m_ConsoleScrollView.Focus();
            m_ConsoleScrollView.scrollOffset =
                new Vector2(
                    m_ConsoleScrollView.scrollOffset.x,
                    m_ConsoleScrollView.scrollOffset.y + (m_ConsoleScrollView.horizontalPageSize * 100));
        }


        public void OnInputPageDown()
        {
            m_ConsoleScrollView.Focus();
            m_ConsoleScrollView.scrollOffset =
                new Vector2(
                    m_ConsoleScrollView.scrollOffset.x,
                    m_ConsoleScrollView.scrollOffset.y - (m_ConsoleScrollView.horizontalPageSize * 100));
        }

        VisualElement MakeItem()
        {
            VisualElement itemBaseElement = new VisualElement { name = "gdx-console-item" };

            Label timestampLabel = new Label { name = "gdx-console-item-frame", style = { fontSize = m_FontSize, width = m_FrameWidth }};
            Label levelLabel = new Label { name = "gdx-console-item-level" , style = { fontSize = m_FontSize, width = m_LevelWidth}};
            Label messageLabel = new Label { name = "gdx-console-item-message", style = { fontSize = m_FontSize}, enableRichText = false};

            itemBaseElement.Add(timestampLabel);
            itemBaseElement.Add(levelLabel);
            itemBaseElement.Add(messageLabel);

            return itemBaseElement;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static char LogLevelToIcon(LogType level)
        {
            switch (level)
            {
                case LogType.Error:
                    return '\uf06a';
                case LogType.Assert:
                    return '\uf2d3';
                case LogType.Warning:
                    return '\uf071';
                case LogType.Log:
                    return '\uf27a';
                case LogType.Exception:
                    return '\uf188';
            }
            return '\uf27a';
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static string LogLevelToClass(LogType level)
        {
            switch (level)
            {
                case LogType.Error:
                    return "error";
                case LogType.Assert:
                    return "assert";
                case LogType.Warning:
                    return "warning";
                case LogType.Log:
                    return "log";
                case LogType.Exception:
                    return "exception";
            }
            return "default";
        }

        static void BindItem(VisualElement element, int index)
        {
            ConsoleLogEntry entry = Console.Log.GetEntryAt(index);
            element.ClearClassList();
            element.AddToClassList(LogLevelToClass(entry.Level));

            ((Label)element[0]).text = entry.FrameCount;

            ((Label)element[1]).text = LogLevelToIcon(entry.Level).ToString();
            ((Label)element[1]).tooltip = entry.StackTrace;

            ((Label)element[2]).text = entry.Message;
        }

        public void ClearInput()
        {
            m_InputBuilder.Clear();
            m_InputLabel.text = "";
            ClearSuggestion();
        }

        public void Tick()
        {
            if (Console.Log.Version != m_CurrentVersion)
            {
                m_CurrentVersion = Console.Log.Version;
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

            ResetSuggestion();
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}