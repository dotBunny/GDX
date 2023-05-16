using System;
using System.Collections;
using System.Collections.Generic;
using GDX.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.Windows
{
    public class SimpleTableWindow : EditorWindow
    {
        static readonly Dictionary<SimpleTable, SimpleTableWindow> k_Windows = new Dictionary<SimpleTable, SimpleTableWindow>();

        SimpleTable targetTable;
        bool initialized;
        VisualTreeAsset rowAsset;

        [OnOpenAsset(1)]
        public static bool OpenSimpleTable(int instanceID, int line)
        {
            UnityEngine.Object unityObject = EditorUtility.InstanceIDToObject(instanceID);
            if (unityObject is SimpleTable table)
            {
                OpenAsset(table);
                return true;
            }
            return false;
        }

        public static SimpleTableWindow OpenAsset(SimpleTable table)
        {
            SimpleTableWindow simpleTableWindow;
            if (k_Windows.TryGetValue(table, out SimpleTableWindow window))
            {
                simpleTableWindow = window;
            }
            else
            {
                simpleTableWindow = CreateWindow<SimpleTableWindow>();
                k_Windows.Add(table, simpleTableWindow);
            }

            if (!simpleTableWindow.initialized)
            {
                VisualElement rootElement = simpleTableWindow.rootVisualElement;
                ResourcesProvider.SetupStylesheets(rootElement);
                ResourcesProvider.GetVisualTreeAsset("GDXSimpleTable").CloneTree(rootElement);
                ResourcesProvider.CheckTheme(rootElement);

                simpleTableWindow.rowAsset = ResourcesProvider.GetVisualTreeAsset("GDXSimpleTableRow");
                simpleTableWindow.initialized = true;
            }

            simpleTableWindow.targetTable = table;
            simpleTableWindow.Rebuild();
            simpleTableWindow.Show();
            simpleTableWindow.Focus();

            return simpleTableWindow;
        }

        public void OnDestroy()
        {
            Debug.Log("Remove Key:" + targetTable.name);
            k_Windows.Remove(targetTable);
        }


        public void Rebuild()
        {
            this.titleContent = new GUIContent(targetTable.name);
        }
    }
}
