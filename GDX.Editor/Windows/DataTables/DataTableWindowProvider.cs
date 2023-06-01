// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using GDX.DataTables;
using UnityEditor;
using UnityEditor.Callbacks;
using Object = UnityEngine.Object;

namespace GDX.Editor.Windows.DataTables
{
#if UNITY_2022_2_OR_NEWER
    public static class DataTableWindowProvider
    {
        public const string UndoPrefix = "Table:";

        static readonly Dictionary<DataTableBase, DataTableWindow> k_TableToTableWindow = new Dictionary<DataTableBase, DataTableWindow>();
        static readonly Dictionary<int, DataTableWindow> k_TicketToTableWindow = new Dictionary<int, DataTableWindow>(5);


        static bool s_SubscribedForUndo;

        internal static DataTableWindow GetTableWindow(int tableTicket)
        {
            return k_TicketToTableWindow.TryGetValue(tableTicket, out DataTableWindow tableWindow) ? tableWindow : null;
        }

        static void UndoRedoEvent(in UndoRedoInfo undo)
        {
            if (!undo.undoName.StartsWith("Table:", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            // Update windows / inspectors
            foreach (KeyValuePair<int, DataTableWindow> kvp in k_TicketToTableWindow)
            {
                // TODO: Maybe we should make an undo subscribe event?
                DataTableTracker.NotifyOfColumnChange(kvp.Key, -1);
            }
        }

        internal static void RegisterTableWindow(DataTableWindow dataTableWindow, DataTableBase dataTable)
        {

            int ticket = dataTableWindow.GetDataTableTicket();
            k_TableToTableWindow[dataTable] = dataTableWindow;
            k_TicketToTableWindow[ticket] = dataTableWindow;

            // We're only going to subscribe for undo events when the table window is open and we have support
            if (!s_SubscribedForUndo && dataTable.GetFlag(DataTableBase.Settings.EnableUndo))
            {
                Undo.undoRedoEvent += UndoRedoEvent;
                s_SubscribedForUndo = true;
            }
        }

        internal static void UnregisterTableWindow(DataTableWindow dataTableWindow)
        {
            int tableWindowTicket = dataTableWindow.GetDataTableTicket();
            k_TicketToTableWindow.Remove(tableWindowTicket);

            DataTableBase dataTable = dataTableWindow.GetDataTable();
            if (dataTable != null)
            {
                k_TableToTableWindow.Remove(dataTable);
            }

            if (k_TicketToTableWindow.Count == 0 && s_SubscribedForUndo)
            {
                Undo.undoRedoEvent -= UndoRedoEvent;
                s_SubscribedForUndo = false;
            }
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAssetTable(int instanceID, int line)
        {
            Object unityObject = EditorUtility.InstanceIDToObject(instanceID);
            if (unityObject is DataTableBase table)
            {
                OpenAsset(table);
                return true;
            }

            return false;
        }

        public static DataTableWindow OpenAsset(DataTableBase dataTable)
        {
            DataTableWindow dataTableWindow = GetTableWindow(DataTableTracker.RegisterTable(dataTable));
            if (dataTableWindow == null)
            {
                dataTableWindow = EditorWindow.CreateWindow<DataTableWindow>();
            }

            if (!dataTableWindow.IsBound())
            {
                dataTableWindow.BindTable(dataTable);
            }

            dataTableWindow.Show();
            dataTableWindow.Focus();

            return dataTableWindow;
        }

#if GDX_TOOLS
        [MenuItem("Tools/GDX/Create StableTable Example", false)]
#endif // GDX_TOOLS
        public static void CreateStableTableExample()
        {
            StableDataTable asset = UnityEngine.ScriptableObject.CreateInstance<StableDataTable>();
            AssetDatabase.CreateAsset(asset, "Assets/StableTableExample.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            asset.SetFlag(DataTableBase.Settings.EnableUndo, true);

            asset.AddColumn(Serializable.SerializableTypes.String, "String");
            asset.AddRow("0");
            asset.AddColumn(Serializable.SerializableTypes.Char, "Char");
            asset.AddRow("1");
            asset.AddColumn(Serializable.SerializableTypes.Bool, "Bool");
            asset.AddRow("2");
            asset.AddColumn(Serializable.SerializableTypes.SByte, "SByte");
            asset.AddRow("3");
            asset.AddColumn(Serializable.SerializableTypes.Byte, "Byte");
            asset.AddRow("4");
            asset.AddColumn(Serializable.SerializableTypes.Short, "Short");
            asset.AddRow("5");
            asset.AddColumn(Serializable.SerializableTypes.UShort, "UShort");
            asset.AddRow("6");
            asset.AddColumn(Serializable.SerializableTypes.Int, "Int");
            asset.AddRow("7");
            asset.AddColumn(Serializable.SerializableTypes.UInt, "UInt");
            asset.AddRow("8");
            asset.AddColumn(Serializable.SerializableTypes.Long, "Long");
            asset.AddRow("9");
            asset.AddColumn(Serializable.SerializableTypes.ULong, "ULong");
            asset.AddRow("10");
            asset.AddColumn(Serializable.SerializableTypes.Float, "Float");
            asset.AddRow("11");
            asset.AddColumn(Serializable.SerializableTypes.Double, "Double");
            asset.AddRow("12");
            asset.AddColumn(Serializable.SerializableTypes.Vector2, "Vector2");
            asset.AddRow("13");
            asset.AddColumn(Serializable.SerializableTypes.Vector3, "Vector3");
            asset.AddRow("14");
            asset.AddColumn(Serializable.SerializableTypes.Vector4, "Vector4");
            asset.AddRow("15");
            asset.AddColumn(Serializable.SerializableTypes.Vector2Int, "Vector2Int");
            asset.AddRow("16");
            asset.AddColumn(Serializable.SerializableTypes.Vector3Int, "Vector3Int");
            asset.AddRow("17");
            asset.AddColumn(Serializable.SerializableTypes.Quaternion, "Quaternion");
            asset.AddRow("18");
            asset.AddColumn(Serializable.SerializableTypes.Rect, "Rect");
            asset.AddRow("19");
            asset.AddColumn(Serializable.SerializableTypes.RectInt, "RectInt");
            asset.AddRow("20");
            asset.AddColumn(Serializable.SerializableTypes.Color, "Color");
            asset.AddRow("21");
            asset.AddColumn(Serializable.SerializableTypes.LayerMask, "LayerMask");
            asset.AddRow("22");
            asset.AddColumn(Serializable.SerializableTypes.Bounds, "Bounds");
            asset.AddRow("23");
            asset.AddColumn(Serializable.SerializableTypes.BoundsInt, "BoundsInt");
            asset.AddRow("24");
            asset.AddColumn(Serializable.SerializableTypes.Hash128, "Hash128");
            asset.AddRow("25");
            asset.AddColumn(Serializable.SerializableTypes.Gradient, "Gradient");
            asset.AddRow("26");
            asset.AddColumn(Serializable.SerializableTypes.AnimationCurve, "AnimationCurve");
            asset.AddRow("27");
            asset.AddColumn(Serializable.SerializableTypes.Object, "Object");
            asset.AddRow("28");

            OpenAsset(asset);
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}