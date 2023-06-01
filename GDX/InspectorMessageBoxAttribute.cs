// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace GDX
{
    /// <summary>
    ///     Create a message box of information above the property in the inspector.
    /// </summary>
    [HideFromDocFX]
    [ExcludeFromCodeCoverage]
    public class InspectorMessageBoxAttribute : PropertyAttribute
    {
        /// <summary>
        ///     Valid types of messages.
        /// </summary>
        public enum MessageBoxType
        {
            None,
            Info,
            Warning,
            Error
        }

#if UNITY_EDITOR
        /// <summary>
        ///     The message to show in the message box.
        /// </summary>
        public readonly string Message;

        /// <summary>
        ///     The icon/type of message being displayed.
        /// </summary>
        public readonly MessageBoxType MessageType;
#endif // UNITY_EDITOR
        // ReSharper disable UnusedParameter.Local
        public InspectorMessageBoxAttribute(string message, MessageBoxType messageType = MessageBoxType.Info)
        {
#if UNITY_EDITOR
            Message = message;
            MessageType = messageType;
#endif // UNITY_EDITOR
        }
        // ReSharper enable UnusedParameter.Local
    }
}