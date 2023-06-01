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
    public static class TableWindowProvider
    {
        public const string UndoPrefix = "Table:";

        static readonly Dictionary<DataTableObject, DataTableWindow> k_TableToTableWindow = new Dictionary<DataTableObject, DataTableWindow>();
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
            foreach (KeyValuePair<DataTableObject, DataTableWindow> kvp in k_TableToTableWindow)
            {
                DataTableTracker.NotifyOfColumnChange(kvp.Key);
            }
        }

        internal static void RegisterTableWindow(DataTableWindow dataTableWindow, DataTableObject dataTable)
        {

            int ticket = dataTableWindow.GetDataTableTicket();
            k_TableToTableWindow[dataTable] = dataTableWindow;
            k_TicketToTableWindow[ticket] = dataTableWindow;

            // We're only going to subscribe for undo events when the table window is open and we have support
            if (!s_SubscribedForUndo && dataTable.GetFlag(DataTableObject.Flags.EnableUndo))
            {
                Undo.undoRedoEvent += UndoRedoEvent;
                s_SubscribedForUndo = true;
            }
        }

        internal static void UnregisterTableWindow(DataTableWindow dataTableWindow)
        {
            int tableWindowTicket = dataTableWindow.GetDataTableTicket();
            k_TicketToTableWindow.Remove(tableWindowTicket);

            DataTableObject dataTable = dataTableWindow.GetDataTable();
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
            if (unityObject is DataTableObject table)
            {
                OpenAsset(table);
                return true;
            }

            return false;
        }

        public static DataTableWindow OpenAsset(DataTableObject dataTable)
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
    }
#endif
}