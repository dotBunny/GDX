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
        public string Label { get ; private set; }
        public InspectorLabelAttribute(string label)
        {
            Label = label ;
        }
    }
}