// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using UnityEditor;

namespace GDX.Editor
{
    /// <summary>
    ///     <see cref="SerializedProperty" /> Based Extension Methods
    /// </summary>
    public static class SerializedPropertyExtensions
    {
        /// <summary>
        /// A single level copy of value from the <paramref name="sourceProperty"/> to the <paramref name="targetProperty"/>.
        /// </summary>
        /// <param name="targetProperty">The receiver of the value.</param>
        /// <param name="sourceProperty">The originator of the value to be used.</param>
        public static void ShallowCopy(this SerializedProperty targetProperty, SerializedProperty sourceProperty)
        {
            switch (targetProperty.type)
            {
                case "int":
                    targetProperty.intValue = sourceProperty.intValue;
                    break;
                case "bool":
                    targetProperty.boolValue = sourceProperty.boolValue;
                    break;
                case "bounds":
                    targetProperty.boundsValue = sourceProperty.boundsValue;
                    break;
                case "color":
                    targetProperty.colorValue = sourceProperty.colorValue;
                    break;
                case "double":
                    targetProperty.doubleValue = sourceProperty.doubleValue;
                    break;
                case "float":
                    targetProperty.floatValue = sourceProperty.floatValue;
                    break;
                case "long":
                    targetProperty.longValue = sourceProperty.longValue;
                    break;
                case "quaternion":
                    targetProperty.quaternionValue = sourceProperty.quaternionValue;
                    break;
                case "rect":
                    targetProperty.rectValue = sourceProperty.rectValue;
                    break;
                case "string":
                    targetProperty.stringValue = sourceProperty.stringValue;
                    break;
                case "vector2":
                    targetProperty.vector2Value = sourceProperty.vector2Value;
                    break;
                case "vector3":
                    targetProperty.vector3Value = sourceProperty.vector3Value;
                    break;
                case "vector4":
                    targetProperty.vector4Value = sourceProperty.vector4Value;
                    break;
                case "animationCurve":
                    targetProperty.animationCurveValue = sourceProperty.animationCurveValue;
                    break;
                case "boundsInt":
                    targetProperty.boundsIntValue = sourceProperty.boundsIntValue;
                    break;
                case "enum":
                    targetProperty.enumValueIndex = sourceProperty.enumValueIndex;
                    break;
                case "exposedReference":
                    targetProperty.exposedReferenceValue = sourceProperty.exposedReferenceValue;
                    break;
                case "object":
                    targetProperty.objectReferenceValue = sourceProperty.objectReferenceValue;
                    targetProperty.objectReferenceInstanceIDValue = sourceProperty.objectReferenceInstanceIDValue;
                    break;

            }
        }
    }
}