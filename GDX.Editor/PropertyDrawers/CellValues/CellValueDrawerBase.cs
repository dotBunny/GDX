// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Tables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#if !UNITY_2022_2_OR_NEWER
using UnityEngine;
#endif

namespace GDX.Editor.PropertyDrawers.CellValues
{
    public abstract class CellValueDrawerBase : PropertyDrawer
    {
        protected ITable m_Table;
        protected int m_RowInternalIndex = -1;
        protected int m_ColumnInternalIndex = -1;

        protected abstract void Init(SerializedProperty serializedProperty);

        // ITable m_Table;
        // int m_columnInternalIndex;
        // int m_rowInternalIndex;
        // public override

#if UNITY_2022_2_OR_NEWER

        /// <summary>
        /// Overrides the method to make a UIElements based GUI for the property.
        /// </summary>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <returns>A disabled visual element.</returns>
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            Init(property);

            VisualElement container = new VisualElement();

            // If we dont have a table we need to select one
            if (m_Table == null)
            {
                // drop down of table display names?
                string[] paths = AssetDatabase.FindAssets($"t:{typeof(ITable)}");
                foreach (string s in paths)
                {
                    UnityEngine.Debug.Log(s);
                }
            }
            container.Add(new ObjectField("Table") { allowSceneObjects = false, objectType = typeof(ITable) });


            //IntCellValue cell = (IntCellValue))property.objectReferenceValue;


       //     container.Add(new Label("test"));
         //   PropertyField propertyField = new PropertyField(property);
           // container.Add(propertyField);
            return container;
        }
#else
        /// <summary>
        ///     Unity IMGUI Draw Event
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true;
        }
#endif
    }
}