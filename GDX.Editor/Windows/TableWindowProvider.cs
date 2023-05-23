// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using GDX.Tables;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace GDX.Editor.Windows
{
#if UNITY_2022_2_OR_NEWER
    static class TableWindowProvider
    {
        static readonly Dictionary<ITable, TableWindow> k_TableWindowMap =
            new Dictionary<ITable, TableWindow>();

        static int s_TableTicketHead = 0;
        static readonly Dictionary<ITable, int> k_TableToTicket = new Dictionary<ITable, int>(5);
        static readonly Dictionary<int, ITable> k_TicketToTable = new Dictionary<int, ITable>(5);

        static int s_TableWindowTicketHead = 0;
        static readonly Dictionary<TableWindow, int> k_TableWindowToTicket = new Dictionary<TableWindow, int>(5);
        static readonly Dictionary<int, TableWindow> k_TicketToTableWindow = new Dictionary<int, TableWindow>(5);

        [GDXShortcut("Add Row",  KeyCode.R, ShortcutModifiers.Control | ShortcutModifiers.Shift, typeof(TableWindow))]
        internal static void ShortcutAddRow()
        {
            TableWindow table = (TableWindow)EditorWindow.focusedWindow;
            if(table != null) table.AddRow();
        }

        [GDXShortcut("Add Column", KeyCode.Plus, ShortcutModifiers.Control | ShortcutModifiers.Shift, typeof(TableWindow))]
        internal static void ShortcutAddColumn()
        {
            TableWindow table = (TableWindow)EditorWindow.focusedWindow;
            if(table != null) table.AddColumn();
        }

        internal static ITable GetTable(int ticket)
        {
            return k_TicketToTable.TryGetValue(ticket, out ITable table) ? table : null;
        }

        internal static TableWindow GetTableWindow(int ticket)
        {
            return k_TicketToTableWindow.TryGetValue(ticket, out TableWindow tableWindow) ? tableWindow : null;
        }

        internal static int GetTableTicket(ITable table)
        {
            return k_TableToTicket.TryGetValue(table, out int ticket) ? ticket : -1;
        }

        internal static int GetTableWindowTicket(TableWindow tableWindow)
        {
            return k_TableWindowToTicket.TryGetValue(tableWindow, out int ticket) ? ticket : -1;
        }
        internal static TableWindow GetTableWindow(ITable table)
        {
            return k_TableWindowMap.TryGetValue(table, out TableWindow window) ? window : null;
        }

        internal static int RegisterTable(ITable table)
        {
            int ticket = GetTableTicket(table);
            if (ticket != -1) return ticket;

            // Register table
            int head = s_TableTicketHead;
            k_TicketToTable.Add(head, table);
            k_TableToTicket.Add(table, head);

            // Increment our next head
            s_TableTicketHead++;
            return head;
        }

        internal static void UnregisterTable(ITable table)
        {
            int ticket = GetTableTicket(table);
            k_TableToTicket.Remove(table);
            k_TicketToTable.Remove(ticket);
        }

        internal static int RegisterTableWindow(TableWindow tableWindow, ITable table)
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

            // Increment our next head
            s_TableWindowTicketHead++;
            return head;
        }

        internal static void UnregisterTableWindow(TableWindow tableWindow)
        {
            int ticket = GetTableWindowTicket(tableWindow);
            k_TableWindowToTicket.Remove(tableWindow);
            k_TicketToTableWindow.Remove(ticket);
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAssetTable(int instanceID, int line)
        {
            Object unityObject = EditorUtility.InstanceIDToObject(instanceID);
            if (unityObject is ITable table)
            {
                OpenAsset(table);
                return true;
            }

            return false;
        }

        public static TableWindow OpenAsset(ITable table)
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