// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using GDX.Tables;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace GDX.Editor.Windows
{
    static class TableWindowProvider
    {
        static readonly Dictionary<ITable, TableWindow> k_TableWindowMap =
            new Dictionary<ITable, TableWindow>();


        static readonly List<ITable> k_KnownTables = new List<ITable>(2);

        static readonly List<TableWindow> k_KnownTablesWindows = new List<TableWindow>(2);

        internal static ITable GetTable(int index)
        {
            return k_KnownTables[index];
        }

        internal static int GetTableIndex(ITable table)
        {
            return k_KnownTables.IndexOf(table);
        }

        internal static int RegisterTable(ITable table)
        {
            if (k_KnownTables.Contains(table))
            {
                return k_KnownTables.IndexOf(table);
            }

            k_KnownTables.Add(table);
            return k_KnownTables.Count - 1;
        }

        internal static void UnregisterTable(ITable table)
        {
            if (k_KnownTables.Contains(table))
            {
                k_KnownTables.Remove(table);
            }
        }

        internal static TableWindow GetTableWindow(int index)
        {
            return k_KnownTablesWindows[index];
        }

        internal static TableWindow GetTableWindow(ITable table)
        {
            return k_TableWindowMap.TryGetValue(table, out TableWindow window) ? window : null;
        }

        internal static int GetTableWindowIndex(TableWindow tableWindow)
        {
            return k_KnownTablesWindows.IndexOf(tableWindow);
        }

        internal static int RegisterTableWindow(TableWindow tableWindow, ITable table)
        {
            k_TableWindowMap[table] = tableWindow;

            if (k_KnownTablesWindows.Contains(tableWindow))
            {
                return k_KnownTablesWindows.IndexOf(tableWindow);
            }

            k_KnownTablesWindows.Add(tableWindow);
            return k_KnownTablesWindows.Count - 1;
        }

        internal static void UnregisterTableWindow(TableWindow tableWindow, ITable table)
        {
            if (k_KnownTablesWindows.Contains(tableWindow))
            {
                k_KnownTablesWindows.Remove(tableWindow);
            }
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
}