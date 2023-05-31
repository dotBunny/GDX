// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using GDX.DataTables;
using UnityEditor;
using UnityEditor.Callbacks;
using Object = UnityEngine.Object;

namespace GDX.Editor.Windows.Tables
{
#if UNITY_2022_2_OR_NEWER
    public static class TableWindowProvider
    {
        public const string UndoPrefix = "Table:";

        static readonly Dictionary<DataTableObject, TableWindow> k_TableToTableWindow = new Dictionary<DataTableObject, TableWindow>();
        static readonly Dictionary<int, TableWindow> k_TicketToTableWindow = new Dictionary<int, TableWindow>(5);


        static bool s_SubscribedForUndo;

        internal static TableWindow GetTableWindow(int tableTicket)
        {
            return k_TicketToTableWindow.TryGetValue(tableTicket, out TableWindow tableWindow) ? tableWindow : null;
        }

        static void UndoRedoEvent(in UndoRedoInfo undo)
        {
            if (!undo.undoName.StartsWith("Table:", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            // Update windows / inspectors
            foreach (KeyValuePair<DataTableObject, TableWindow> kvp in k_TableToTableWindow)
            {
                TableCache.NotifyOfColumnChange(kvp.Key);
            }
        }

        internal static void RegisterTableWindow(TableWindow tableWindow, DataTableObject dataTable)
        {

            int ticket = tableWindow.GetTableTicket();
            k_TableToTableWindow[dataTable] = tableWindow;
            k_TicketToTableWindow[ticket] = tableWindow;

            // We're only going to subscribe for undo events when the table window is open and we have support
            if (!s_SubscribedForUndo && dataTable.GetFlag(DataTableObject.Flags.EnableUndo))
            {
                Undo.undoRedoEvent += UndoRedoEvent;
                s_SubscribedForUndo = true;
            }
        }

        internal static void UnregisterTableWindow(TableWindow tableWindow)
        {
            int tableWindowTicket = tableWindow.GetTableTicket();
            k_TicketToTableWindow.Remove(tableWindowTicket);

            DataTableObject dataTable = tableWindow.GetDataTable();
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

        public static TableWindow OpenAsset(DataTableObject dataTable)
        {
            TableWindow tableWindow = GetTableWindow(TableCache.RegisterTable(dataTable));
            if (tableWindow == null)
            {
                tableWindow = EditorWindow.CreateWindow<TableWindow>();
            }

            if (!tableWindow.IsBound())
            {
                tableWindow.BindTable(dataTable);
            }

            tableWindow.Show();
            tableWindow.Focus();

            return tableWindow;
        }
    }
#endif
}