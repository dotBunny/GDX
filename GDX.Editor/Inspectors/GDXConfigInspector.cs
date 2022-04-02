// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEditor;

namespace GDX.Editor.Inspectors
{
    /// <summary>
    ///     A custom inspector for the <see cref="Config" /> scriptable object.
    /// </summary>
    /// <remarks>
    ///     This just enforces editing through the project settings window only.
    /// </remarks>
    [HideFromDocFX]
    [CustomEditor(typeof(Config))]
    public class GDXConfigInspector : UnityEditor.Editor
    {
        /// <summary>
        ///     Message to display in the inspector if a <see cref="Config" /> is selected in the project.
        /// </summary>
        const string k_HelpContent = "GDX project settings must be changed via the Project Settings window.";

        /// <summary>
        ///     Prevent the inspector actually showing for a <see cref="Config" /> to prevent unintentional editing.
        /// </summary>
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(k_HelpContent, MessageType.Info);
        }
    }
}