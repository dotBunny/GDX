// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using GDX.Tables;
using UnityEditor;

namespace GDX.Editor
{
    public static class TableCache
    {
        static readonly Dictionary<int, List<ICellValueChangedCallbackReceiver>> k_CellValueChangeCallbackReceivers =
            new Dictionary<int, List<ICellValueChangedCallbackReceiver>>(5);

        static readonly Dictionary<int, List<IColumnDefinitionChangeCallbackReceiver>> k_ColumnChangeCallbackReceivers =
            new Dictionary<int, List<IColumnDefinitionChangeCallbackReceiver>>(5);

        static readonly Dictionary<int, List<IRowDefinitionChangeCallbackReceiver>> k_RowChangeCallbackReceivers =
            new Dictionary<int, List<IRowDefinitionChangeCallbackReceiver>>(5);

        static int s_TableTicketHead;
        static readonly Dictionary<int, TableBase> k_TableTicketToTable = new Dictionary<int, TableBase>(5);
        static readonly Dictionary<TableBase, int> k_TableToTableTicket = new Dictionary<TableBase, int>(5);
        static readonly Dictionary<TableBase, int> k_TableUsageCounters = new Dictionary<TableBase, int>(5);

        public static void RegisterCellValueChanged(ICellValueChangedCallbackReceiver callback, TableBase table)
        {
            RegisterCellValueChanged(callback, GetTicket(table));
        }

        public static void RegisterCellValueChanged(ICellValueChangedCallbackReceiver callback, int tableTicket)
        {
            if (tableTicket == -1)
            {
                return;
            }

            if (!k_CellValueChangeCallbackReceivers.ContainsKey(tableTicket))
            {
                k_CellValueChangeCallbackReceivers.Add(tableTicket,
                    new List<ICellValueChangedCallbackReceiver>());
            }

            k_CellValueChangeCallbackReceivers[tableTicket].Add(callback);
        }

        public static void UnregisterCellValueChanged(ICellValueChangedCallbackReceiver callback, TableBase table)
        {
            UnregisterCellValueChanged(callback, GetTicket(table));
        }

        public static void UnregisterCellValueChanged(ICellValueChangedCallbackReceiver callback, int tableTicket)
        {
            if (k_CellValueChangeCallbackReceivers.TryGetValue(tableTicket, out List<ICellValueChangedCallbackReceiver> receiver))
            {
                receiver.Remove(callback);
            }
        }

        public static void RegisterRowChanged(IRowDefinitionChangeCallbackReceiver callback, int tableTicket)
        {
            if (tableTicket == -1)
            {
                return;
            }

            if (!k_RowChangeCallbackReceivers.ContainsKey(tableTicket))
            {
                k_RowChangeCallbackReceivers.Add(tableTicket,
                    new List<IRowDefinitionChangeCallbackReceiver>());
            }

            k_RowChangeCallbackReceivers[tableTicket].Add(callback);
        }

        public static void UnregisterRowChanged(IRowDefinitionChangeCallbackReceiver callback, int tableTicket)
        {
            if (k_RowChangeCallbackReceivers.TryGetValue(tableTicket, out List<IRowDefinitionChangeCallbackReceiver> receiver))
            {
                receiver.Remove(callback);
            }
        }

        public static void NotifyOfRowChange(TableBase table, IRowDefinitionChangeCallbackReceiver ignore = null)
        {
            NotifyOfRowChange(GetTicket(table), ignore);
        }

        public static void NotifyOfRowChange(int tableTicket, IRowDefinitionChangeCallbackReceiver ignore = null)
        {
            int count = k_RowChangeCallbackReceivers[tableTicket].Count;
            for (int i = 0; i < count; i++)
            {
                IRowDefinitionChangeCallbackReceiver callback = k_RowChangeCallbackReceivers[tableTicket][i];
                if (callback != ignore)
                {
                    callback.OnRowDefinitionChange();
                }
            }

            EditorUtility.SetDirty(k_TableTicketToTable[tableTicket]);
        }

        public static void RegisterColumnChanged(IColumnDefinitionChangeCallbackReceiver callback, int tableTicket)
        {
            if (tableTicket == -1)
            {
                return;
            }

            if (!k_ColumnChangeCallbackReceivers.ContainsKey(tableTicket))
            {
                k_ColumnChangeCallbackReceivers.Add(tableTicket,
                    new List<IColumnDefinitionChangeCallbackReceiver>());
            }

            k_ColumnChangeCallbackReceivers[tableTicket].Add(callback);
        }

        public static void UnregisterColumnChanged(IColumnDefinitionChangeCallbackReceiver callback, int tableTicket)
        {
            if (k_ColumnChangeCallbackReceivers.TryGetValue(tableTicket, out List<IColumnDefinitionChangeCallbackReceiver> receiver))
            {
                receiver.Remove(callback);
            }
        }

        public static void NotifyOfColumnChange(TableBase table, IColumnDefinitionChangeCallbackReceiver ignore = null)
        {
            NotifyOfColumnChange(GetTicket(table), ignore);
        }

        public static void NotifyOfColumnChange(int tableTicket, IColumnDefinitionChangeCallbackReceiver ignore = null)
        {
            int count = k_ColumnChangeCallbackReceivers[tableTicket].Count;
            for (int i = 0; i < count; i++)
            {
                IColumnDefinitionChangeCallbackReceiver callback = k_ColumnChangeCallbackReceivers[tableTicket][i];
                if (callback != ignore)
                {
                    callback.OnColumnDefinitionChange();
                }
            }

            EditorUtility.SetDirty(k_TableTicketToTable[tableTicket]);
        }

        public static void NotifyOfCellValueChange(TableBase table, int rowInternalIndex, int columnInternalIndex, ICellValueChangedCallbackReceiver ignore = null)
        {
            NotifyOfCellValueChange(GetTicket(table), rowInternalIndex, columnInternalIndex, ignore);
        }

        public static void NotifyOfCellValueChange(int tableTicket, int rowInternalIndex, int columnInternalIndex, ICellValueChangedCallbackReceiver ignore = null)
        {
            int count = k_CellValueChangeCallbackReceivers[tableTicket].Count;
            for (int i = 0; i < count; i++)
            {
                ICellValueChangedCallbackReceiver callback = k_CellValueChangeCallbackReceivers[tableTicket][i];
                if (callback != ignore)
                {
                    callback.OnCellValueChanged(rowInternalIndex, columnInternalIndex);
                }
            }

            EditorUtility.SetDirty(k_TableTicketToTable[tableTicket]);
        }

        public static AssetDatabaseReference[] FindTables()
        {
            List<AssetDatabaseReference> returnList = new List<AssetDatabaseReference>(5);

            // Get derived types
            TypeCache.TypeCollection tableTypeCollection = TypeCache.GetTypesDerivedFrom<TableBase>();
            int count = tableTypeCollection.Count;
            for (int i = 0; i < count; i++)
            {
                string typeDef = tableTypeCollection[i].ToString();
                string[] paths = AssetDatabase.FindAssets($"t:{typeDef}");
                int pathCount = paths.Length;
                for (int j = 0; j < pathCount; j++)
                {
                    returnList.Add(new AssetDatabaseReference(paths[j], tableTypeCollection[i].UnderlyingSystemType));
                }
            }

            return returnList.ToArray();
        }

        public static TableBase GetTable(int ticket)
        {
            return k_TableTicketToTable.TryGetValue(ticket, out TableBase table) ? table : null;
        }

        public static int GetTicket(TableBase table)
        {
            return k_TableToTableTicket.TryGetValue(table, out int ticket) ? ticket : -1;
        }

        public static void UnregisterTable(TableBase table)
        {
            if (table == null)
            {
                return;
            }

            int ticket = GetTicket(table);

            if (ticket != -1)
            {
                k_TableUsageCounters[table]--;
                if (k_TableUsageCounters[table] == 0)
                {
                    k_TableToTableTicket.Remove(table);
                    k_TableTicketToTable.Remove(ticket);
                    k_TableUsageCounters.Remove(table);
                }
            }
        }

        public static int RegisterTable(TableBase table)
        {
            int ticket = GetTicket(table);
            if (ticket != -1)
            {
                k_TableUsageCounters[table]++;
                return ticket;
            }

            // Register table
            int head = s_TableTicketHead;
            k_TableTicketToTable.Add(head, table);
            k_TableToTableTicket.Add(table, head);
            k_TableUsageCounters.Add(table, 1);

            // Increment our next head
            s_TableTicketHead++;
            return head;
        }

        public interface ICellValueChangedCallbackReceiver
        {
            void OnCellValueChanged(int rowInternalIndex, int columnInternalIndex);
        }

        public interface IColumnDefinitionChangeCallbackReceiver
        {
            void OnColumnDefinitionChange();
        }

        public interface IRowDefinitionChangeCallbackReceiver
        {
            void OnRowDefinitionChange();
        }
    }
}