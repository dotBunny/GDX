// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using GDX.DataTables;
using UnityEditor;
using UnityEngine;

namespace GDX.Editor
{
    /// <summary>
    ///     A observer-like pattern for <see cref="DataTableObject" /> usage in the editor.
    /// </summary>
    /// <remarks>
    ///     This is useful for driving updates across multiple windows, inspectors and property drawers.
    /// </remarks>
    public static class DataTableTracker
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

        public static DataTableTrackerStats GetStats(int tableTicket)
        {
            DataTableTrackerStats stats = new DataTableTrackerStats();
            if (k_CellValueChangeCallbackReceivers.TryGetValue(tableTicket,
                    out List<ICellValueChangedCallbackReceiver> cellChange))
            {
                stats.CellValueChanged = cellChange.Count;
            }

            if (k_ColumnChangeCallbackReceivers.TryGetValue(tableTicket,
                    out List<IColumnDefinitionChangeCallbackReceiver> columnChange))
            {
                stats.ColumnDefinitionChange = columnChange.Count;
            }

            if (k_RowChangeCallbackReceivers.TryGetValue(tableTicket,
                    out List<IRowDefinitionChangeCallbackReceiver> rowChange))
            {
                stats.RowDefinitionChange = rowChange.Count;
            }

            if (k_TableUsageCounters.TryGetValue(tableTicket, out int usageCount))
            {
                stats.Usages = usageCount;
            }

            return stats;
        }

        public static void RegisterCellValueChanged(ICellValueChangedCallbackReceiver callback,
            DataTableObject dataTable)
        {
            int ticket = GetTicket(dataTable);
            if (ticket == -1)
            {
                Debug.LogError(
                    $"The DataTable '{dataTable.GetDisplayName()} has not yet been registered. You must register a DataTable before registering for callbacks.");
                return;
            }

            RegisterCellValueChanged(callback, ticket);
        }

        public static void RegisterCellValueChanged(ICellValueChangedCallbackReceiver callback, int tableTicket)
        {
            if (tableTicket == -1)
            {
                Debug.LogError("Unable to register for CellValueChanged callback as the ticket provided is invalid.");
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

        public static void UnregisterCellValueChanged(ICellValueChangedCallbackReceiver callback,
            DataTableObject dataTable)
        {
            UnregisterCellValueChanged(callback, GetTicket(dataTable));
        }

        public static void UnregisterCellValueChanged(ICellValueChangedCallbackReceiver callback, int tableTicket)
        {
            if (k_CellValueChangeCallbackReceivers.TryGetValue(tableTicket,
                    out List<ICellValueChangedCallbackReceiver> receiver))
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
            if (k_RowChangeCallbackReceivers.TryGetValue(tableTicket,
                    out List<IRowDefinitionChangeCallbackReceiver> receiver))
            {
                receiver.Remove(callback);
            }
        }

        /// <summary>
        ///     Notify all registered <see cref="IRowDefinitionChangeCallbackReceiver" />s that a change has occured.
        /// </summary>
        /// <remarks>Marks the changed <see cref="DataTableObject"/> as dirty.</remarks>
        /// <param name="tableTicket">The unique ticket generated by <see cref="RegisterTable" />.</param>
        /// <param name="rowIdentifier">The affected row's unique identifier.</param>
        /// <param name="ignoreReceiver">
        ///     A <see cref="IRowDefinitionChangeCallbackReceiver" /> that should be ignored in the notification
        ///     process. This is used to stop self-notification.
        /// </param>
        public static void NotifyOfRowChange(int tableTicket, int rowIdentifier,
            IRowDefinitionChangeCallbackReceiver ignoreReceiver = null)
        {
            int count = k_RowChangeCallbackReceivers[tableTicket].Count;
            for (int i = 0; i < count; i++)
            {
                IRowDefinitionChangeCallbackReceiver callback = k_RowChangeCallbackReceivers[tableTicket][i];
                if (callback != ignoreReceiver)
                {
                    callback.OnRowDefinitionChange(rowIdentifier);
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
            if (k_ColumnChangeCallbackReceivers.TryGetValue(tableTicket,
                    out List<IColumnDefinitionChangeCallbackReceiver> receiver))
            {
                receiver.Remove(callback);
            }
        }

        /// <summary>
        ///     Notify all registered <see cref="IColumnDefinitionChangeCallbackReceiver" />s that a change has occured.
        /// </summary>
        /// <remarks>Marks the changed <see cref="DataTableObject"/> as dirty.</remarks>
        /// <param name="tableTicket">The unique ticket generated by <see cref="RegisterTable" />.</param>
        /// <param name="columnIdentifier">The affected column's unique identifier.</param>
        /// <param name="ignoreReceiver">
        ///     A <see cref="IColumnDefinitionChangeCallbackReceiver" /> that should be ignored in the notification
        ///     process. This is used to stop self-notification.
        /// </param>
        public static void NotifyOfColumnChange(int tableTicket, int columnIdentifier,
            IColumnDefinitionChangeCallbackReceiver ignoreReceiver = null)
        {
            int count = k_ColumnChangeCallbackReceivers[tableTicket].Count;
            for (int i = 0; i < count; i++)
            {
                IColumnDefinitionChangeCallbackReceiver callback = k_ColumnChangeCallbackReceivers[tableTicket][i];
                if (callback != ignoreReceiver)
                {
                    callback.OnColumnDefinitionChange(columnIdentifier);
                }
            }

            EditorUtility.SetDirty(k_TableTicketToTable[tableTicket]);
        }

        /// <summary>
        ///     Notify all registered <see cref="ICellValueChangedCallbackReceiver" />s that a change has occured.
        /// </summary>
        /// <remarks>Marks the changed <see cref="DataTableObject"/> as dirty.</remarks>
        /// <param name="tableTicket">The unique ticket generated by <see cref="RegisterTable" />.</param>
        /// <param name="rowIdentifier">The affected row's unique identifier.</param>
        /// <param name="columnIdentifier">The affected column's unique identifier.</param>
        /// <param name="ignoreReceiver">
        ///     A <see cref="ICellValueChangedCallbackReceiver" /> that should be ignored in the notification
        ///     process. This is used to stop self-notification.
        /// </param>
        public static void NotifyOfCellValueChange(int tableTicket, int rowIdentifier, int columnIdentifier,
            ICellValueChangedCallbackReceiver ignoreReceiver = null)
        {
            int count = k_CellValueChangeCallbackReceivers[tableTicket].Count;
            for (int i = 0; i < count; i++)
            {
                ICellValueChangedCallbackReceiver callback = k_CellValueChangeCallbackReceivers[tableTicket][i];
                if (callback != ignoreReceiver)
                {
                    callback.OnCellValueChanged(rowIdentifier, columnIdentifier);
                }
            }

            EditorUtility.SetDirty(k_TableTicketToTable[tableTicket]);
        }

        internal static AssetDatabaseReference[] FindTables()
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

        /// <summary>
        ///     Attempt to get the registered <see cref="DataTableObject" /> by ticket number.
        /// </summary>
        /// <param name="ticket">The ticket number.</param>
        /// <returns>A <see cref="DataTableObject" /> if found, otherwise null.</returns>
        public static DataTableObject GetTable(int ticket)
        {
            return k_TableTicketToTable.TryGetValue(ticket, out DataTableObject table) ? table : null;
        }

        /// <summary>
        ///     Attempt to get the ticket number of the provided <see cref="DataTableObject" />.
        /// </summary>
        /// <param name="dataTable">The target <see cref="DataTableObject" />.</param>
        /// <returns>A ticket number, otherwise -1.</returns>
        static int GetTicket(DataTableObject dataTable)
        {
            if (dataTable == null)
            {
                return -1;
            }

            return k_TableToTableTicket.TryGetValue(dataTable, out int registerTable) ? registerTable : -1;
        }

        /// <summary>
        ///     Registers a <see cref="DataTableObject" /> for use by all of the different supporting systems inside of the Unity
        ///     editor.
        /// </summary>
        /// <param name="dataTable">The <see cref="DataTableObject" /> to be registered for use.</param>
        /// <param name="forcedTicket">
        ///     Force a specific ticket to be used for a table; this is explicitly used during domain reload
        ///     to ensure references are kept.
        /// </param>
        /// <returns>A domain incremented ticket number.</returns>
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
#endif // UNITY_2022_2_OR_NEWER

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

        /// <summary>
        ///     Indicate that a <see cref="DataTableObject" /> is being used by something.
        /// </summary>
        /// <param name="tableTicket">The unique ticket generated by <see cref="RegisterTable" />.</param>
        public static void AddUsage(int tableTicket)
        {
            k_TableUsageCounters[tableTicket]++;
        }

        /// <summary>
        ///     Indicate that a <see cref="DataTableObject" /> is no longer being used by something.
        /// </summary>
        /// <remarks>
        ///     When there are no more usages, a cleanup pass is ran to remove other mappings.
        /// </remarks>
        /// <param name="tableTicket">The unique ticket generated by <see cref="RegisterTable" />.</param>
        public static void RemoveUsage(int tableTicket)
        {
            if (!k_TableUsageCounters.ContainsKey(tableTicket))
            {
                return;
            }

            k_TableUsageCounters[tableTicket]--;
            if (k_TableUsageCounters[tableTicket] > 0)
            {
                return;
            }

            k_TableToTableTicket.Remove(k_TableTicketToTable[tableTicket]);
            k_TableTicketToTable.Remove(tableTicket);
            k_TableUsageCounters.Remove(tableTicket);
        }

        public struct DataTableTrackerStats
        {
            public int CellValueChanged;
            public int ColumnDefinitionChange;
            public int RowDefinitionChange;
            public int Usages;
        }

        /// <summary>
        ///     An interface describing the functionality needed for an object to get a callback when a
        ///     a cell values changes in a <see cref="DataTableObject" />.
        /// </summary>
        public interface ICellValueChangedCallbackReceiver
        {
            /// <summary>
            ///     A cell value change has occured.
            /// </summary>
            /// <param name="rowIdentifier">The affected row's unique identifier.</param>
            /// <param name="columnIdentifier">The affected column's unique identifier.</param>
            void OnCellValueChanged(int rowIdentifier, int columnIdentifier);
        }

        /// <summary>
        ///     An interface describing the functionality needed for an object to get a callback when a
        ///     <see cref="DataTableObject.ColumnDescription" /> changes in a <see cref="DataTableObject" />.
        /// </summary>
        public interface IColumnDefinitionChangeCallbackReceiver
        {
            /// <summary>
            ///     A <see cref="DataTableObject.ColumnDescription" /> change has occured.
            /// </summary>
            /// <param name="columnIdentifier">The affected column's unique identifier.</param>
            void OnColumnDefinitionChange(int columnIdentifier);
        }

        /// <summary>
        ///     An interface describing the functionality needed for an object to get a callback when a
        ///     <see cref="DataTableObject.RowDescription" /> changes in a <see cref="DataTableObject" />.
        /// </summary>
        public interface IRowDefinitionChangeCallbackReceiver
        {
            /// <summary>
            ///     A <see cref="DataTableObject.RowDescription" /> change has occured.
            /// </summary>
            /// <param name="rowIdentifier">The affected row's unique identifier.</param>
            void OnRowDefinitionChange(int rowIdentifier);
        }
    }
}