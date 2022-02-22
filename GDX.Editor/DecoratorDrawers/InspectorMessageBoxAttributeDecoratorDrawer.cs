// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;
using UnityEditor;

namespace GDX.Editor.DecoratorDrawers
{
    /// <summary>
    ///     The drawing component of the <see cref="InspectorMessageBoxAttribute" />.
    /// </summary>
    [HideFromDocFX]
    [CustomPropertyDrawer(typeof(InspectorMessageBoxAttribute))]
    // ReSharper disable once UnusedType.Global
    public class InspectorMessageBoxAttributeDecoratorDrawer : DecoratorDrawer
    {
        /// <summary>
        ///     Cached reference to the target <see cref="InspectorMessageBoxAttribute"/>.
        /// </summary>
        private InspectorMessageBoxAttribute m_Target;

        /// <summary>
        ///     Cached GUIContent of text, strictly used by height calculations.
        /// </summary>
        private GUIContent m_TargetMessage;

        /// <summary>
        ///     A cached reference to the "help box" style.
        /// </summary>
        // ReSharper disable once StringLiteralTypo
        private static readonly GUIStyle s_CachedHelpBoxStyle = new GUIStyle("helpbox");

        /// <summary>
        ///     Returns the inspector height needed for this decorator.
        /// </summary>
        /// <returns>The height in pixels.</returns>
        public override float GetHeight()
        {
            // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
            if (m_Target == null)
            {
                m_Target = (InspectorMessageBoxAttribute)attribute;
            }
            // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
            if (m_TargetMessage == null)
            {
                m_TargetMessage = new GUIContent(m_Target.Message);
            }
            return s_CachedHelpBoxStyle.CalcHeight(m_TargetMessage, EditorGUIUtility.currentViewWidth) + 4;
        }

        /// <summary>
        ///     Unity IMGUI Draw Event
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the decorator.</param>
        public override void OnGUI(Rect position)
        {
            // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
            if (m_Target == null)
            {
                m_Target = (InspectorMessageBoxAttribute)attribute;
            }

            switch (m_Target.MessageType)
            {
                case InspectorMessageBoxAttribute.MessageBoxType.Info:
                    EditorGUI.HelpBox(position, m_Target.Message, MessageType.Info);
                    break;
                case InspectorMessageBoxAttribute.MessageBoxType.Warning:
                    EditorGUI.HelpBox(position, m_Target.Message, MessageType.Warning);
                    break;
                case InspectorMessageBoxAttribute.MessageBoxType.Error:
                    EditorGUI.HelpBox(position, m_Target.Message, MessageType.Error);
                    break;
                default:
                    EditorGUI.HelpBox(position, m_Target.Message, MessageType.None);
                    break;
            }
        }
    }
}