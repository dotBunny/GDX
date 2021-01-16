// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GDX
{
    using UnityEngine;

    /// <summary>
    ///     Override the label content of the scripting variable.
    /// </summary>
    public class InspectorLabelAttribute : PropertyAttribute
    {
        /// <summary>
        /// Label Text
        /// </summary>
        public readonly string Label;

        /// <summary>
        /// Create a new instance of <see cref="InspectorLabelAttribute"/>.
        /// </summary>
        /// <param name="label">The labels <see cref="GUIContent.text"/>.</param>
        public InspectorLabelAttribute(string label)
        {
            Label = label ;
        }
    }
}