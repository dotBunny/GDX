// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using GDX.DataTables;
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

        static readonly Dictionary<int, DataTableObject> k_TableTicketToTable = new Dictionary<int, DataTableObject>(5);
        static readonly Dictionary<DataTableObject, int> k_TableToTableTicket = new Dictionary<DataTableObject, int>(5);

        static readonly Dictionary<int, int> k_TableUsageCounters = new Dictionary<int, int>(5);

        public static void RegisterCellValueChanged(ICellValueChangedCallbackReceiver callback, DataTableObject dataTable)
        {
            RegisterCellValueChanged(callback, GetTicket(dataTable));
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

            if (!k_CellValueChangeCallbackReceivers[tableTicket].Contains(callback))
            {
                k_CellValueChangeCallbackReceivers[tableTicket].Add(callback);
            }
        }

        public static void UnregisterCellValueChanged(ICellValueChangedCallbackReceiver callback, DataTableObject dataTable)
        {
            UnregisterCellValueChanged(callback, GetTicket(dataTable));
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

            if (!k_RowChangeCallbackReceivers[tableTicket].Contains(callback))
            {
                k_RowChangeCallbackReceivers[tableTicket].Add(callback);
            }
        }

        public static void UnregisterRowChanged(IRowDefinitionChangeCallbackReceiver callback, int tableTicket)
        {
            if (k_RowChangeCallbackReceivers.TryGetValue(tableTicket, out List<IRowDefinitionChangeCallbackReceiver> receiver))
            {
                receiver.Remove(callback);
            }
        }

        public static void NotifyOfRowChange(DataTableObject dataTable, IRowDefinitionChangeCallbackReceiver ignore = null)
        {
            NotifyOfRowChange(GetTicket(dataTable), ignore);
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

            if (!k_ColumnChangeCallbackReceivers[tableTicket].Contains(callback))
            {
                k_ColumnChangeCallbackReceivers[tableTicket].Add(callback);
            }
        }

        public static void UnregisterColumnChanged(IColumnDefinitionChangeCallbackReceiver callback, int tableTicket)
        {
            if (k_ColumnChangeCallbackReceivers.TryGetValue(tableTicket, out List<IColumnDefinitionChangeCallbackReceiver> receiver))
            {
                receiver.Remove(callback);
            }
        }

        public static void NotifyOfColumnChange(DataTableObject dataTable, IColumnDefinitionChangeCallbackReceiver ignore = null)
        {
            NotifyOfColumnChange(GetTicket(dataTable), ignore);
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

        public static void NotifyOfCellValueChange(DataTableObject dataTable, int rowInternalIndex, int columnInternalIndex, ICellValueChangedCallbackReceiver ignore = null)
        {
            NotifyOfCellValueChange(GetTicket(dataTable), rowInternalIndex, columnInternalIndex, ignore);
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
            TypeCache.TypeCollection tableTypeCollection = TypeCache.GetTypesDerivedFrom<DataTableObject>();
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

        public static DataTableObject GetTable(int ticket)
        {
            return k_TableTicketToTable.TryGetValue(ticket, out DataTableObject table) ? table : null;
        }

        static int GetTicket(DataTableObject dataTable)
        {
            if (dataTable == null) return -1;
            return k_TableToTableTicket.TryGetValue(dataTable, out int registerTable) ? registerTable: -1;
        }

        public static int RegisterTable(DataTableObject dataTable, int forcedTicket = -1)
        {
            // This is the scenario where we need to rebuild after domain reload
            if (forcedTicket != -1)
            {
                k_TableTicketToTable[forcedTicket] = dataTable;
                k_TableToTableTicket[dataTable] = forcedTicket;
#if UNITY_2022_2_OR_NEWER
                k_TableUsageCounters.TryAdd(forcedTicket, 0);
#else
                if (!k_TableUsageCounters.ContainsKey(forcedTicket))
                {
                    k_TableUsageCounters[forcedTicket] = 0;
                }
#endif


                if (forcedTicket >= s_TableTicketHead)
                {
                    s_TableTicketHead = forcedTicket + 1;
                }

                return forcedTicket;
            }

            if (k_TableToTableTicket.TryGetValue(dataTable, out int registerTable))
            {
                return registerTable;
            }

            // Register table
            int head = s_TableTicketHead;
            k_TableTicketToTable.Add(head, dataTable);
            k_TableToTableTicket.Add(dataTable, head);
            k_TableUsageCounters.Add(head, 0);

            // Increment our next head
            s_TableTicketHead++;
            return head;
        }

        public static void RegisterUsage(int tableTicket)
        {
            k_TableUsageCounters[tableTicket]++;
        }
        public static void UnregisterUsage(int tableTicket)
        {
            if (k_TableUsageCounters.ContainsKey(tableTicket))
            {
                k_TableUsageCounters[tableTicket]--;
                if (k_TableUsageCounters[tableTicket] <= 0)
                {
                    k_TableToTableTicket.Remove(k_TableTicketToTable[tableTicket]);
                    k_TableTicketToTable.Remove(tableTicket);
                    k_TableUsageCounters.Remove(tableTicket);
                }
            }
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