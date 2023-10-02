// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using GDX.Developer;
using UnityEditor;
using UnityEditor.Networking.PlayerConnection;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.UIElements;
using Console = GDX.Developer.Console;

namespace GDX.Editor.Windows
{
#if UNITY_2022_2_OR_NEWER
    public class CommandPalette : EditorWindow
    {
        enum ConsoleTarget
        {
            Local,
            PlayerConnection
        }


        const int k_TargetWidth = 500;
        const int k_TargetHeight = 40;

        static readonly StyleLength k_OneHundredPercent = new Length(100, LengthUnit.Percent);
        static readonly StyleFloat k_NoPixel = new StyleFloat(1f);
        static readonly StyleLength k_FontSize = new Length(22, LengthUnit.Pixel);
        static readonly StyleLength k_ZeroLength = 0;
        static readonly StyleColor k_PlayerConnection = new StyleColor(new Color(1f, 0.6470588235294118f, 0.2352941176470588f));


        static CommandPalette s_Instance;
        static ConsoleTarget s_Target = ConsoleTarget.Local;

        bool m_IsBound;
        TextField m_TextField;

        [Shortcut("GDX/Command Palette", null, KeyCode.BackQuote, ShortcutModifiers.Control)]
        public static void Open()
        {
            if (Application.isPlaying || !Config.EnvironmentDeveloperConsole)
            {
                return;
            }


            s_Target = EditorConnection.instance.ConnectedPlayers.Count > 0 ? ConsoleTarget.PlayerConnection : ConsoleTarget.Local;

            if (s_Instance == null)
            {
                s_Instance = CreateInstance<CommandPalette>();
                s_Instance.hideFlags = HideFlags.DontSave;
                s_Instance.Bind();
            }

            s_Instance.position = EditorGUIUtility.GetMainWindowPosition().GetCenteredRect(
                new Vector2(k_TargetWidth, k_TargetHeight));
            s_Instance.minSize = s_Instance.maxSize = s_Instance.position.size;
            s_Instance.ShowPopup();

            // Focus our text field
            s_Instance.m_TextField.Focus();
        }

        void Bind()
        {
            Label prefix = new Label
            {
                name = "gdx-command-palette-caret",
                style =
                {
                    height = k_OneHundredPercent,
                    width = new Length(20, LengthUnit.Pixel),
                    paddingLeft = new Length(10, LengthUnit.Pixel),
                    paddingRight = new Length(10, LengthUnit.Pixel),
                    flexShrink = 0,
                    flexGrow = 0,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    fontSize = k_FontSize,
                    unityFontStyleAndWeight = FontStyle.Bold
                },
                text = ">"
            };

            TextField input = new TextField
            {
                name = "gdx-command-palette-input",
                style =
                {
                    height = k_OneHundredPercent,
                    flexGrow = 1,
                    fontSize = k_FontSize,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    borderBottomWidth = k_NoPixel,
                    borderLeftWidth = k_NoPixel,
                    borderRightWidth = k_NoPixel,
                    borderTopWidth = k_NoPixel,
                    marginLeft = k_ZeroLength,
                    marginRight = k_ZeroLength,
                    marginTop = k_ZeroLength,
                    marginBottom = k_ZeroLength,
                    backgroundColor = new StyleColor(Color.clear)

                },
                isDelayed = true
            };
            if (s_Target == ConsoleTarget.PlayerConnection)
            {
                input[0].style.borderBottomColor = k_PlayerConnection;
                input[0].style.borderLeftColor = k_PlayerConnection;
                input[0].style.borderRightColor = k_PlayerConnection;
                input[0].style.borderTopColor = k_PlayerConnection;
            }

            input[0].Insert(0, prefix);
            rootVisualElement.Add(input);

            m_TextField = rootVisualElement.Q<TextField>("gdx-command-palette-input");
            TextInputBaseField<string> textInput = m_TextField.Q<TextInputBaseField<string>>();
            textInput.autoCorrection = false;


            m_TextField.RegisterCallback<KeyDownEvent>(OnSubmit, TrickleDown.TrickleDown);
            m_TextField.RegisterCallback<FocusOutEvent>(OnFocusOutEvent);
        }

        void OnSubmit(KeyDownEvent evt)
        {
            switch (evt.keyCode)
            {
                case KeyCode.Escape:
                    DelayedClose();
                    break;
                case KeyCode.Return:
                case KeyCode.KeypadEnter:
                    if (s_Target == ConsoleTarget.PlayerConnection)
                    {
                        EditorConnection.instance.Send(ConsoleCommandBase.PlayerConnectionGuid, Encoding.UTF8.GetBytes(m_TextField.text));
                    }
                    else
                    {
                        Console.QueueCommand(m_TextField.text);
                    }

                    DelayedClose();
                    break;
            }
        }

        void OnFocusOutEvent(FocusOutEvent evt)
        {
            DelayedClose();
        }

        void DelayedClose()
        {
            // We need to do this after the event to avoid a null ref, this seems like a really bad pattern.
            rootVisualElement.schedule.Execute(Close).ExecuteLater(5);
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}