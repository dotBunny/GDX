// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX
{
    /// <summary>
    ///     Create a message box of information above the property in the inspector.
    /// </summary>
    public class InspectorMessageBoxAttribute : PropertyAttribute
    {
#if UNITY_EDITOR
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

        /// <summary>
        ///     The message to show in the message box.
        /// </summary>
        public readonly string Message;

        /// <summary>
        ///     The icon/type of message being displayed.
        /// </summary>
        public readonly MessageBoxType MessageType;

        public InspectorMessageBoxAttribute(string message, MessageBoxType messageType = MessageBoxType.Info)
        {
            Message = message;
            MessageType = messageType;
        }
    }
#endif
}