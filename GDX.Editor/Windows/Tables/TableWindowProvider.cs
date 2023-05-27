// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using GDX.Editor.Inspectors;
using GDX.Tables;
using UnityEditor;
using UnityEditor.Callbacks;
using Object = UnityEngine.Object;

namespace GDX.Editor.Windows.Tables
{
#if UNITY_2022_2_OR_NEWER
    public static class TableWindowProvider
    {
        public const string UndoPrefix = "Table:";
        static readonly Dictionary<TableBase, TableWindow> k_TableWindowMap =
            new Dictionary<TableBase, TableWindow>();

        static int s_TableWindowTicketHead;
        static readonly Dictionary<TableWindow, int> k_TableWindowToTicket = new Dictionary<TableWindow, int>(5);
        static readonly Dictionary<int, TableWindow> k_TicketToTableWindow = new Dictionary<int, TableWindow>(5);
        static bool s_SubscribedForUndo = false;

        internal static TableWindow GetTableWindow(int ticket)
        {
            return k_TicketToTableWindow.TryGetValue(ticket, out TableWindow tableWindow) ? tableWindow : null;
        }



        internal static int GetTableWindowTicket(TableWindow tableWindow)
        {
            return k_TableWindowToTicket.TryGetValue(tableWindow, out int ticket) ? ticket : -1;
        }

        internal static TableWindow GetTableWindow(TableBase table)
        {
            return k_TableWindowMap.TryGetValue(table, out TableWindow window) ? window : null;
        }




        static void UndoRedoEvent(in UndoRedoInfo undo)
        {
            if (!undo.undoName.StartsWith("Table:", StringComparison.InvariantCultureIgnoreCase))
                return;

            // Update windows / inspectors
            foreach(KeyValuePair<TableBase,TableWindow> kvp in k_TableWindowMap)
            {
                kvp.Value.BindTable(kvp.Key);
                TableInspectorBase.RedrawInspector(kvp.Key);
            }
        }

        internal static int RegisterTableWindow(TableWindow tableWindow, TableBase table)
        {
            int ticket = GetTableWindowTicket(tableWindow);
            k_TableWindowMap[table] = tableWindow;
            if (ticket != -1)
            {
                return ticket;
            }

            // Register table
            int head = s_TableWindowTicketHead;
            k_TicketToTableWindow.Add(head, tableWindow);
            k_TableWindowToTicket.Add(tableWindow, head);

            // We're only going to subscribe for undo events when the table window is open and we have support
            if (!s_SubscribedForUndo && table.GetFlag(TableBase.Flags.EnableUndo))
            {
                Undo.undoRedoEvent += UndoRedoEvent;
                s_SubscribedForUndo = true;
            }

            // Increment our next head
            s_TableWindowTicketHead++;
            return head;
        }




        internal static void UnregisterTableWindow(TableWindow tableWindow)
        {
            int ticket = GetTableWindowTicket(tableWindow);
            k_TableWindowToTicket.Remove(tableWindow);
            k_TicketToTableWindow.Remove(ticket);

            if (k_TableWindowToTicket.Count == 0 && s_SubscribedForUndo)
            {
                Undo.undoRedoEvent -= UndoRedoEvent;
                s_SubscribedForUndo = false;
            }

            // We need to remove all cells associated with a map
            TableWindowCells.CleanTableReferences(ticket);
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAssetTable(int instanceID, int line)
        {
            Object unityObject = EditorUtility.InstanceIDToObject(instanceID);
            if (unityObject is TableBase table)
            {
                OpenAsset(table);
                return true;
            }

            return false;
        }

        public static TableWindow OpenAsset(TableBase table)
        {
            TableWindow tableWindow = GetTableWindow(table);
            if (tableWindow == null)
            {
                tableWindow = EditorWindow.CreateWindow<TableWindow>();
                RegisterTableWindow(tableWindow, table);
            }

            tableWindow.BindTable(table);
            tableWindow.Show();
            tableWindow.Focus();

            return tableWindow;
        }
    }
#endif
}