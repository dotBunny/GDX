// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using GDX.DataTables;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace GDX.Editor.Windows.DataTables
{
#if UNITY_2022_2_OR_NEWER
    public static class DataTableWindowProvider
    {
        static readonly Dictionary<int, DataTableWindow>
            k_TicketToTableWindow = new Dictionary<int, DataTableWindow>(5);


        static bool s_SubscribedForUndo;

        internal static DataTableWindow GetTableWindow(int tableTicket)
        {
            return k_TicketToTableWindow.TryGetValue(tableTicket, out DataTableWindow tableWindow) ? tableWindow : null;
        }

        internal static void RegisterTableWindow(DataTableWindow dataTableWindow, DataTableBase dataTable)
        {
            int ticket = dataTableWindow.GetDataTableTicket();
            k_TicketToTableWindow[ticket] = dataTableWindow;
        }

        internal static void UnregisterTableWindow(DataTableWindow dataTableWindow)
        {
            int tableWindowTicket = dataTableWindow.GetDataTableTicket();
            k_TicketToTableWindow.Remove(tableWindowTicket);
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
    }
#endif // UNITY_2022_2_OR_NEWER
}