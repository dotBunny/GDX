// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Runtime.InteropServices;

namespace GDX.Developer
{
    public static class InputProxy
    {
        /// <summary>
        ///     A set of flags to describe various aspects of <see cref="KeyboardInput" />, mainly used to define
        ///     additional information related to <see cref="KeyboardInput.Key" />.
        /// </summary>
        [Flags]
        public enum KeyboardFlag : uint
        {
            KeyDown = 0x0000,

            /// <summary>
            ///     Is the key part of the extended set.
            /// </summary>
            ExtendedKey = 0x0001,

            /// <summary>
            ///     A key has been released.
            /// </summary>
            KeyUp = 0x0002,
            Unicode = 0x0004,
            ScanCode = 0x0008
        }

        /// <summary>
        ///     Virtual key codes.
        /// </summary>
        /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes" />
        public enum KeyCode : ushort
        {
            Backspace = 0x08,
            Tab = 0x09,
            Clear = 0x0c,
            Return = 0x0d,
            Shift = 0x10,
            Control = 0x11,
            Alt = 0x12,
            Pause = 0x13,
            CapsLock = 0x14,
            Escape = 0x1b,
            Space = 0x20,
            PageUp = 0x21,
            PageDown = 0x22,
            End = 0x23,
            Home = 0x24,
            Left = 0x25,
            Up = 0x26,
            Right = 0x27,
            Down = 0x28,
            PrintScreen = 0x2c,
            Insert = 0x2d,
            Delete = 0x2e,
            Number0 = 0x30,
            Number1 = 0x31,
            Number2 = 0x32,
            Number3 = 0x33,
            Number4 = 0x34,
            Number5 = 0x35,
            Number6 = 0x36,
            Number7 = 0x37,
            Number8 = 0x38,
            Number9 = 0x39,
            A = 0x41,
            B = 0x42,
            C = 0x43,
            D = 0x44,
            E = 0x45,
            F = 0x46,
            G = 0x47,
            H = 0x48,
            I = 0x49,
            J = 0x4a,
            K = 0x4b,
            L = 0x4c,
            M = 0x4d,
            N = 0x4e,
            O = 0x4f,
            P = 0x50,
            Q = 0x51,
            R = 0x52,
            S = 0x53,
            T = 0x54,
            U = 0x55,
            V = 0x56,
            W = 0x57,
            X = 0x58,
            Y = 0x59,
            Z = 0x5a,
            LeftWindows = 0x5b,
            RightWindows = 0x5c,
            Applications = 0x5d,
            Sleep = 0x5f,
            NumPad0 = 0x60,
            NumPad1 = 0x61,
            NumPad2 = 0x62,
            NumPad3 = 0x63,
            NumPad4 = 0x64,
            NumPad5 = 0x65,
            NumPad6 = 0x66,
            NumPad7 = 0x67,
            NumPad8 = 0x68,
            NumPad9 = 0x69,
            NumPadMultiply = 0x6a,
            NumPadAdd = 0x6b,
            NumPadSubtract = 0x6d,
            NumPadDecimal = 0x6e,
            NumPadDivide = 0x6f,
            F1 = 0x70,
            F2 = 0x71,
            F3 = 0x72,
            F4 = 0x73,
            F5 = 0x74,
            F6 = 0x75,
            F7 = 0x76,
            F8 = 0x77,
            F9 = 0x78,
            F10 = 0x79,
            F11 = 0x7a,
            F12 = 0x7b,
            NumLock = 0x90,
            ScrollLock = 0x91,
            LeftShift = 0xa0,
            RightShift = 0xa1,
            LeftControl = 0xa2,
            RightControl = 0xa3
        }

        /// <summary>
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/inputdev/about-keyboard-input" />
        public static bool IsExtendedKey(KeyCode keyCode)
        {
            if (keyCode == KeyCode.Control ||
                keyCode == KeyCode.RightControl ||
                keyCode == KeyCode.Insert ||
                keyCode == KeyCode.Delete ||
                keyCode == KeyCode.Home ||
                keyCode == KeyCode.End ||
                keyCode == KeyCode.Right ||
                keyCode == KeyCode.Up ||
                keyCode == KeyCode.Left ||
                keyCode == KeyCode.Down ||
                keyCode == KeyCode.NumLock ||
                keyCode == KeyCode.NumPadDivide)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     A set of flags to describe various aspects of <see cref="MouseInput" />, mainly used to define
        ///     additional information related to <see cref="MouseInput.Data" />.
        /// </summary>
        [Flags]
        public enum MouseFlag : uint
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            VerticalWheel = 0x0800,
            HorizontalWheel = 0x1000,

            /// <summary>
            ///     Used in multi-desktop scenarios where you want to treat X/Y against the combined screen space.
            ///     You must also use <see cref="MouseFlag.Absolute" />.
            /// </summary>
            VirtualDesk = 0x4000,

            /// <summary>
            ///     Indicates provided X/Y are in absolute screenspace instead of deltas of previous position.
            /// </summary>
            Absolute = 0x8000
        }

        /// <summary>
        ///     A precalculated size of the <see cref="InputItem" />. This is calculated by finding the largest InputData
        ///     struct size and adding either 8 (64bit) or 4 (32bit) bytes to it to account for the difference in pointer size.
        /// </summary>
#if UNITY_64
        const uint k_InputStructureSize = 40;
#else
        const uint k_InputStructureSize = 36;
#endif

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint numberOfInputs, InputItem[] inputs, uint sizeOfInputStructure);

        [DllImport("user32.dll")]
        static extern uint MapVirtualKey(uint uCode, uint uMapType);
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Synthesize(KeyboardInput keyboardInput)
        {
            return Synthesize(new[] { keyboardInput }) == 1;
        }


        public static uint Synthesize(KeyboardInput[] keyboardInputs)
        {
            uint count = (uint)keyboardInputs.Length;
            if (count == 0)
            {
                return 0;
            }

            InputItem[] items = new InputItem[count];
            for (int i = 0; i < count; i++)
            {
                items[i] = new InputItem(InputType.Keyboard, new InputData(keyboardInputs[i]));
            }

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            return SendInput(count, items, k_InputStructureSize);
#else
            return 0;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Synthesize(MouseInput mouseInput)
        {
            return Synthesize(new[] { mouseInput }) == 1;
        }

        public static uint Synthesize(MouseInput[] mouseInputs)
        {
            uint count = (uint)mouseInputs.Length;
            if (count == 0)
            {
                return 0;
            }

            InputItem[] items = new InputItem[count];
            for (int i = 0; i < count; i++)
            {
                items[i] = new InputItem(InputType.Mouse, new InputData(mouseInputs[i]));
            }
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            return SendInput(count, items, k_InputStructureSize);
#else
            return 0;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Synthesize(HardwareInput hardwareInput)
        {
            return Synthesize(new[] { hardwareInput }) == 1;
        }

        public static uint Synthesize(HardwareInput[] hardwareInputs)
        {
            uint count = (uint)hardwareInputs.Length;
            if (count == 0)
            {
                return 0;
            }

            InputItem[] items = new InputItem[count];
            for (int i = 0; i < count; i++)
            {
                items[i] = new InputItem(InputType.Hardware, new InputData(hardwareInputs[i]));
            }
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            return SendInput(count, items, k_InputStructureSize);
#else
            return 0;
#endif
        }

        public static bool KeyPress(KeyCode keyCode)
        {
            return Synthesize(new[]
            {
                new KeyboardInput(keyCode, KeyboardFlag.KeyDown, 0, IntPtr.Zero),
                new KeyboardInput(keyCode, KeyboardFlag.KeyUp, 0, IntPtr.Zero)
            }) == 2;
        }

        public static bool MouseClick(int x, int y, bool virtualMode = false)
        {
            return Synthesize(new[]
            {
                new MouseInput(x, y, 0, MouseFlag.Move & MouseFlag.Absolute, 0, IntPtr.Zero),
                new MouseInput(x, y, 0, MouseFlag.LeftDown, 0, IntPtr.Zero),
                new MouseInput(x, y, 0, MouseFlag.LeftUp, 0, IntPtr.Zero)
            }) == 3;
        }

        /// <summary>
        ///     The type of event being synthesized.
        /// </summary>
        enum InputType : uint
        {
            Mouse = 0,
            Keyboard = 1,
            Hardware = 2
        }

        /// <summary>
        ///     A keyboard input event.
        /// </summary>
        /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-keybdinput" />
        /// <remarks>Order and types matters as it is mapped into native, using 24 bytes.</remarks>
        public struct KeyboardInput
        {
            public readonly ushort Key;
            public readonly ushort ScanCode;
            public readonly uint Flags;

            /// <summary>
            ///     The timestamp of the event, if 0, OS will just make its own. This is useful if you want to simulate
            ///     a duration of time between input events.
            /// </summary>
            public uint Timestamp;

            public IntPtr ExtraInfo;

            public KeyboardInput(KeyCode key, KeyboardFlag flags, uint timestamp, IntPtr extraInfo)
            {
                Key = (ushort)key;


                // Safety check for extended key
                if (IsExtendedKey(key) && !flags.HasFlags(KeyboardFlag.ExtendedKey))
                {
                    Flags = (uint)(flags | KeyboardFlag.ExtendedKey);
                }
                else
                {
                    Flags = (uint)flags;
                }

                if (k_KnownScanCodes.TryGetValue(Key, out ushort value))
                {
                    ScanCode = value;
                }
                else
                {
                    ScanCode = (ushort)(MapVirtualKey((uint)key, 0) & 0xFFU);
                    k_KnownScanCodes.Add(Key, ScanCode);
                }

                Timestamp = timestamp;
                ExtraInfo = extraInfo;
            }
        }

        static readonly Dictionary<ushort, ushort> k_KnownScanCodes = new Dictionary<ushort, ushort>();

        /// <summary>
        ///     A generic hardware input event.
        /// </summary>
        /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-hardwareinput" />
        /// <remarks>
        ///     Order and types matters as it is mapped into native, using 8 bytes.
        /// </remarks>
        public struct HardwareInput
        {
            /// <summary>
            ///     The message generated by the input hardware.
            /// </summary>
            public uint Message;

            /// <summary>
            ///     The low-order word of the lParam parameter for.
            /// </summary>
            public ushort ParamL;

            /// <summary>
            ///     The high-order word of the lParam parameter for uMsg.
            /// </summary>
            public ushort ParamH;

            public HardwareInput(uint message, ushort paramL, ushort paramH)
            {
                Message = message;
                ParamL = paramL;
                ParamH = paramH;
            }
        }

        /// <summary>
        ///     A mouse input event.
        /// </summary>
        /// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-mouseinput" />
        /// <remarks>
        ///     Order and types matters as it is mapped into native, using 32 bytes.
        /// </remarks>
        public struct MouseInput
        {
            /// <summary>
            ///     The absolute X position or delta depending on the <see cref="MouseFlag" /> used.
            /// </summary>
            public readonly int X;

            /// <summary>
            ///     The absolute Y position or delta depending on the <see cref="MouseFlag" /> used.
            /// </summary>
            public readonly int Y;

            public uint Data;
            public readonly uint Flags;

            /// <summary>
            ///     The timestamp of the event, if 0, OS will just make its own. This is useful if you want to simulate
            ///     a duration of time between input events.
            /// </summary>
            public uint Timestamp;

            public IntPtr ExtraInfo;

            public MouseInput(int x, int y, uint data, MouseFlag flags, uint timestamp, IntPtr extraInfo)
            {

                Data = data;

                // Absolute on main monitor?
                if (flags.HasFlags(MouseFlag.Absolute) && !flags.HasFlags(MouseFlag.VirtualDesk))
                {

                    float widthPercent = (float)x / Screen.currentResolution.width;
                    float heightPercent = (float)y / Screen.currentResolution.height;
                    X = (int)(widthPercent * 65535);
                    Y = (int)(heightPercent * 65535);
                }
                else
                {
                    X = x;
                    Y = y;
                }
                Flags = (uint)flags;
                Timestamp = timestamp;
                ExtraInfo = extraInfo;
            }
        }

        /// <summary>
        ///     An explicit data structure used to represent the input event being synthesized.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        struct InputData
        {
            [FieldOffset(0)] readonly MouseInput Mouse;

            [FieldOffset(0)] readonly KeyboardInput Keyboard;

            [FieldOffset(0)] readonly HardwareInput Hardware;

            public InputData(MouseInput mouseInput)
            {
                Keyboard = default;
                Hardware = default;
                Mouse = mouseInput;
            }

            public InputData(KeyboardInput keyboardInput)
            {
                Hardware = default;
                Mouse = default;
                Keyboard = keyboardInput;
            }

            public InputData(HardwareInput hardwareInput)
            {
                Mouse = default;
                Keyboard = default;
                Hardware = hardwareInput;
            }
        }

        /// <summary>
        ///     An explicit data structure used to represent the input event being synthesized.
        /// </summary>
        struct InputItem
        {
            readonly InputType Type;
            readonly InputData Data;
            public InputItem(InputType type, InputData data)
            {
                Type = type;
                Data = data;
            }
        }
    }
}