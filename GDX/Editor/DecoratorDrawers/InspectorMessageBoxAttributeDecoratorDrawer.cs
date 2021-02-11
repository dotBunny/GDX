// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using UnityEngine;
using UnityEditor;

namespace GDX.Editor.DecoratorDrawers
{
    /// <summary>
    ///     The drawing component of the <see cref="InspectorMessageBoxAttribute" />.
    /// </summary>
    [CustomPropertyDrawer(typeof(InspectorMessageBoxAttribute))]
    public class InspectorMessageBoxAttributeDecoratorDrawer : DecoratorDrawer
    {
        /// <summary>
        ///     Cached reference to the target <see cref="InspectorMessageBoxAttribute"/>.
        /// </summary>
        private InspectorMessageBoxAttribute _target;

        /// <summary>
        ///     Cached GUIContent of text, strictly used by height calculations.
        /// </summary>
        private GUIContent _targetMessage;

        /// <summary>
        ///     A cached reference to the "helpbox" style.
        /// </summary>
        private static readonly GUIStyle s_cachedHelpBoxStyle = new GUIStyle("helpbox");

        /// <summary>
        ///     Returns the inspector height needed for this decorator.
        /// </summary>
        /// <returns>The height in pixels.</returns>
        public override float GetHeight()
        {
            _target ??= (InspectorMessageBoxAttribute)attribute;
            _targetMessage ??= new GUIContent(_target.Message);
            return s_cachedHelpBoxStyle.CalcHeight(_targetMessage, EditorGUIUtility.currentViewWidth) + 4;
        }

        /// <summary>
        ///     Unity IMGUI Draw Event
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the decorator.</param>
        public override void OnGUI(Rect position)
        {
            _target ??= (InspectorMessageBoxAttribute)attribute;
            switch (_target.MessageType)
            {
                case InspectorMessageBoxAttribute.MessageBoxType.Info:
                    EditorGUI.HelpBox(position, _target.Message, MessageType.Info);
                    break;
                case InspectorMessageBoxAttribute.MessageBoxType.Warning:
                    EditorGUI.HelpBox(position, _target.Message, MessageType.Warning);
                    break;
                case InspectorMessageBoxAttribute.MessageBoxType.Error:
                    EditorGUI.HelpBox(position, _target.Message, MessageType.Error);
                    break;
                default:
                    EditorGUI.HelpBox(position, _target.Message, MessageType.None);
                    break;
            }
        }
    }
}