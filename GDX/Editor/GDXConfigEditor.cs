// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using UnityEditor;

namespace GDX.Editor
{
    /// <summary>
    ///     A custom editor for the <see cref="GDXConfig" /> scriptable object.
    /// </summary>
    /// <remarks>
    ///     This just enforces editing through the project settings window only.
    /// </remarks>
    [CustomEditor(typeof(GDXConfig))]
    // ReSharper disable once InconsistentNaming
    public class GDXConfigEditor : UnityEditor.Editor
    {
        /// <summary>
        ///     Message to display in the inspector if a <see cref="GDXConfig" /> is selected in the project.
        /// </summary>
        private const string HelpContent = "GDX project settings must be changed via the Project Settings window.";

        /// <summary>
        ///     Prevent the inspector actually showing for a <see cref="GDXConfig" /> to prevent unintentional editing.
        /// </summary>
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(HelpContent, MessageType.Info);
        }
    }
}