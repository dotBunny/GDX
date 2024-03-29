﻿// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using GDX.DataTables;
using UnityEditor;
using UnityEngine;

namespace GDX.Editor
{
    /// <summary>
    ///     A observer-like pattern for <see cref="DataTableBase" /> usage in the editor.
    /// </summary>
    /// <remarks>
    ///     This is useful for driving updates across multiple windows, inspectors and property drawers.
    /// </remarks>
    public static class DataTableTracker
    {
#if UNITY_2022_2_OR_NEWER
        /// <summary>
        ///     Is the tracker subscribed to Undo events.
        /// </summary>
        static bool s_SubscribedToUndo;
#endif

        /// <summary>
        ///     The registered <see cref="ICellValueChangedCallbackReceiver" />s by <see cref="DataTableBase" /> ticket.
        /// </summary>
        static readonly Dictionary<int, List<ICellValueChangedCallbackReceiver>> k_CellValueChangeCallbackReceivers =
            new Dictionary<int, List<ICellValueChangedCallbackReceiver>>(5);

        /// <summary>
        ///     The registered <see cref="IStructuralChangeCallbackReceiver" />s by <see cref="DataTableBase" /> ticket.
        /// </summary>
        static readonly Dictionary<int, List<IStructuralChangeCallbackReceiver>> k_StructuralChangeCallbackReceivers =
            new Dictionary<int, List<IStructuralChangeCallbackReceiver>>(5);

        /// <summary>
        ///     The registered <see cref="IUndoRedoEventCallbackReceiver" />s by <see cref="DataTableBase" /> ticket.
        /// </summary>
        static readonly Dictionary<int, List<IUndoRedoEventCallbackReceiver>> k_UndoRedoCallbackReceivers =
            new Dictionary<int, List<IUndoRedoEventCallbackReceiver>>(5);

        /// <summary>
        ///     Next ticket to distribute when registering a <see cref="DataTableBase" />.
        /// </summary>
        static int s_TableTicketHead;

        /// <summary>
        ///     A mapping between tickets generated by <see cref="RegisterTable" /> and the provided <see cref="DataTableBase" />
        ///     s.
        /// </summary>
        static readonly Dictionary<int, DataTableBase> k_TableTicketToTable = new Dictionary<int, DataTableBase>(5);

        /// <summary>
        ///     A mapping between <see cref="RegisterTable" /> provided <see cref="DataTableBase" />s and tickets.
        /// </summary>
        static readonly Dictionary<DataTableBase, int> k_TableToTableTicket = new Dictionary<DataTableBase, int>(5);

        /// <summary>
        ///     A counter of usages, keyed by ticket.
        /// </summary>
        static readonly Dictionary<int, int> k_TableUsageCounters = new Dictionary<int, int>(5);

        /// <summary>
        ///     Indicate that a <see cref="DataTableBase" /> is being used by something.
        /// </summary>
        /// <param name="tableTicket">The unique ticket generated by <see cref="RegisterTable" />.</param>
        public static void AddUsage(int tableTicket)
        {
            k_TableUsageCounters[tableTicket]++;
        }

        /// <summary>
        ///     Get the summary of currently what is being tracked by the <see cref="DataTableTracker" /> for a
        ///     <see cref="DataTableBase" />.
        /// </summary>
        /// <param name="tableTicket">The unique ticket generated by <see cref="RegisterTable" />.</param>
        /// <returns></returns>
        public static DataTableTrackerStats GetStats(int tableTicket)
        {
            DataTableTrackerStats stats = new DataTableTrackerStats();
            if (k_CellValueChangeCallbackReceivers.TryGetValue(tableTicket,
                    out List<ICellValueChangedCallbackReceiver> cellChange))
            {
                stats.CellValueChanged = cellChange.Count;
            }

            if (k_StructuralChangeCallbackReceivers.TryGetValue(tableTicket,
                    out List<IStructuralChangeCallbackReceiver> structuralChange))
            {
                stats.StructuralChange = structuralChange.Count;
            }

            if (k_TableUsageCounters.TryGetValue(tableTicket, out int usageCount))
            {
                stats.Usages = usageCount;
            }

            return stats;
        }

        /// <summary>
        ///     Attempt to get the registered <see cref="DataTableBase" /> by ticket number.
        /// </summary>
        /// <param name="tableTicket">The unique ticket generated by <see cref="RegisterTable" />.</param>
        /// <returns>A <see cref="DataTableBase" /> if found, otherwise null.</returns>
        public static DataTableBase GetTable(int tableTicket)
        {
            return k_TableTicketToTable.TryGetValue(tableTicket, out DataTableBase table) ? table : null;
        }

        /// <summary>
        ///     Attempt to get the ticket number of the provided <see cref="DataTableBase" />.
        /// </summary>
        /// <param name="dataTable">The target <see cref="DataTableBase" />.</param>
        /// <returns>A ticket number, otherwise -1.</returns>
        public static int GetTicket(DataTableBase dataTable)
        {
            if (dataTable == null)
            {
                return -1;
            }

            return k_TableToTableTicket.TryGetValue(dataTable, out int registerTable) ? registerTable : -1;
        }

        /// <summary>
        ///     Notify all registered <see cref="ICellValueChangedCallbackReceiver" />s that a change has occured.
        /// </summary>
        /// <remarks>Marks the changed <see cref="DataTableBase" /> as dirty.</remarks>
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

        /// <summary>
        ///     Notify all registered <see cref="IStructuralChangeCallbackReceiver" />s that a change has occured.
        /// </summary>
        /// <remarks>Marks the changed <see cref="DataTableBase" /> as dirty.</remarks>
        /// <param name="tableTicket">The unique ticket generated by <see cref="RegisterTable" />.</param>
        /// <param name="columnIdentifier">The affected column's unique identifier.</param>
        /// <param name="ignoreReceiver">
        ///     A <see cref="IStructuralChangeCallbackReceiver" /> that should be ignored in the notification
        ///     process. This is used to stop self-notification.
        /// </param>
        public static void NotifyOfColumnChange(int tableTicket, int columnIdentifier,
            IStructuralChangeCallbackReceiver ignoreReceiver = null)
        {
            int count = k_StructuralChangeCallbackReceivers[tableTicket].Count;
            for (int i = 0; i < count; i++)
            {
                IStructuralChangeCallbackReceiver callback = k_StructuralChangeCallbackReceivers[tableTicket][i];
                if (callback != ignoreReceiver)
                {
                    callback.OnColumnDefinitionChange(columnIdentifier);
                }
            }

            EditorUtility.SetDirty(k_TableTicketToTable[tableTicket]);
        }

        /// <summary>
        ///     Notify all registered <see cref="IStructuralChangeCallbackReceiver" />s that a row change has occured.
        /// </summary>
        /// <remarks>Marks the changed <see cref="DataTableBase" /> as dirty.</remarks>
        /// <param name="tableTicket">The unique ticket generated by <see cref="RegisterTable" />.</param>
        /// <param name="rowIdentifier">The affected row's unique identifier.</param>
        /// <param name="ignoreReceiver">
        ///     A <see cref="IStructuralChangeCallbackReceiver" /> that should be ignored in the notification
        ///     process. This is used to stop self-notification.
        /// </param>
        public static void NotifyOfRowChange(int tableTicket, int rowIdentifier,
            IStructuralChangeCallbackReceiver ignoreReceiver = null)
        {
            int count = k_StructuralChangeCallbackReceivers[tableTicket].Count;
            for (int i = 0; i < count; i++)
            {
                IStructuralChangeCallbackReceiver callback = k_StructuralChangeCallbackReceivers[tableTicket][i];
                if (callback != ignoreReceiver)
                {
                    callback.OnRowDefinitionChange(rowIdentifier);
                }
            }

            EditorUtility.SetDirty(k_TableTicketToTable[tableTicket]);
        }

        /// <summary>
        ///     Notify all registered <see cref="IStructuralChangeCallbackReceiver" />s that a setting change has occured.
        /// </summary>
        /// <remarks>Marks the changed <see cref="DataTableBase" /> as dirty.</remarks>
        /// <param name="tableTicket">The unique ticket generated by <see cref="RegisterTable" />.</param>
        /// <param name="ignoreReceiver">
        ///     A <see cref="IStructuralChangeCallbackReceiver" /> that should be ignored in the notification
        ///     process. This is used to stop self-notification.
        /// </param>
        public static void NotifyOfSettingsChange(int tableTicket,
            IStructuralChangeCallbackReceiver ignoreReceiver = null)
        {
            int count = k_StructuralChangeCallbackReceivers[tableTicket].Count;
            for (int i = 0; i < count; i++)
            {
                IStructuralChangeCallbackReceiver callback = k_StructuralChangeCallbackReceivers[tableTicket][i];
                if (callback != ignoreReceiver)
                {
                    callback.OnSettingsChange();
                }
            }

            EditorUtility.SetDirty(k_TableTicketToTable[tableTicket]);
        }

        /// <summary>
        ///     Record the state of a <see cref="DataTableBase " /> for undoing a cell value change.
        /// </summary>
        /// <param name="tableTicket">The unique ticket generated by <see cref="RegisterTable" />.</param>
        /// <param name="rowIdentifier">The affected row's unique identifier.</param>
        /// <param name="columnIdentifier">The affected column's unique identifier.</param>
        public static void RecordCellValueUndo(int tableTicket, int rowIdentifier, int columnIdentifier)
        {
            if (!k_TableTicketToTable.ContainsKey(tableTicket))
            {
                Debug.LogError($"A cell value undo event was recorded for {tableTicket}, but that ticket is invalid.");
                return;
            }

            DataTableBase table = k_TableTicketToTable[tableTicket];
            if (!table.GetMetaData().SupportsUndo)
            {
                return;
            }

            Undo.RegisterCompleteObjectUndo(table,
                $"DataTable {tableTicket} Cell {rowIdentifier} {columnIdentifier} - Value Changed");
        }

        /// <summary>
        ///     Record the state of a <see cref="DataTableBase " /> for undoing a column definition change.
        /// </summary>
        /// <param name="tableTicket">The unique ticket generated by <see cref="RegisterTable" />.</param>
        /// <param name="columnIdentifier">The affected column's unique identifier.</param>
        /// <param name="actionDescription">A description of the action being done.</param>
        public static void RecordColumnDefinitionUndo(int tableTicket, int columnIdentifier,
            string actionDescription = null)
        {
            if (!k_TableTicketToTable.ContainsKey(tableTicket))
            {
                Debug.LogError(
                    $"A column definition undo event was recorded for {tableTicket}, but that ticket is invalid.");
                return;
            }

            DataTableBase table = k_TableTicketToTable[tableTicket];
            if (!table.GetMetaData().SupportsUndo)
            {
                return;
            }

            if (string.IsNullOrEmpty(actionDescription))
            {
                Undo.RegisterCompleteObjectUndo(table,
                    columnIdentifier != -1
                        ? $"DataTable {tableTicket} Column {columnIdentifier} - Definition Changed"
                        : $"DataTable {tableTicket} Column - Definition Changed");
            }
            else
            {
                Undo.RegisterCompleteObjectUndo(table,
                    columnIdentifier != -1
                        ? $"DataTable {tableTicket} Column {columnIdentifier} - {actionDescription}"
                        : $"DataTable {tableTicket} Column - {actionDescription}");
            }
        }

        /// <summary>
        ///     Record the state of a <see cref="DataTableBase " /> for undoing a row definition change.
        /// </summary>
        /// <param name="tableTicket">The unique ticket generated by <see cref="RegisterTable" />.</param>
        /// <param name="rowIdentifier">The affected row's unique identifier.</param>
        /// <param name="actionDescription">A description of the action being done.</param>
        public static void RecordRowDefinitionUndo(int tableTicket, int rowIdentifier, string actionDescription = null)
        {
            if (!k_TableTicketToTable.ContainsKey(tableTicket))
            {
                Debug.LogError(
                    $"A row definition undo event was recorded for {tableTicket}, but that ticket is invalid.");
                return;
            }

            DataTableBase table = k_TableTicketToTable[tableTicket];
            if (!table.GetMetaData().SupportsUndo)
            {
                return;
            }

            if (string.IsNullOrEmpty(actionDescription))
            {
                Undo.RegisterCompleteObjectUndo(table,
                    rowIdentifier != -1
                        ? $"DataTable {tableTicket} Row {rowIdentifier} - Definition Changed"
                        : $"DataTable {tableTicket} Row - Definition Changed");
            }
            else
            {
                Undo.RegisterCompleteObjectUndo(table,
                    rowIdentifier != -1
                        ? $"DataTable {tableTicket} Row {rowIdentifier} - {actionDescription}"
                        : $"DataTable {tableTicket} Row - {actionDescription}");
            }
        }

        /// <summary>
        ///     Record the state of a <see cref="DataTableBase " /> for undoing a settings change.
        /// </summary>
        /// <param name="tableTicket">The unique ticket generated by <see cref="RegisterTable" />.</param>
        public static void RecordSettingsUndo(int tableTicket)
        {
            if (!k_TableTicketToTable.ContainsKey(tableTicket))
            {
                Debug.LogError($"A settings undo event was recorded for {tableTicket}, but that ticket is invalid.");
                return;
            }

            DataTableBase table = k_TableTicketToTable[tableTicket];
            if (!table.GetMetaData().SupportsUndo)
            {
                return;
            }

            Undo.RegisterCompleteObjectUndo(table, $"DataTable {tableTicket} - Settings Changed");
        }

        /// <summary>
        ///     Register an <see cref="ICellValueChangedCallbackReceiver" /> for callbacks based on the provided ticket.
        /// </summary>
        /// <param name="callback">The target receiver to add to the callback list.</param>
        /// <param name="tableTicket">The unique ticket generated by <see cref="RegisterTable" />.</param>
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

        /// <summary>
        ///     Register a <see cref="IStructuralChangeCallbackReceiver" /> for callbacks based on the provided ticket.
        /// </summary>
        /// <param name="callback">The target receiver to add to the callback list.</param>
        /// <param name="tableTicket">The unique ticket generated by <see cref="RegisterTable" />.</param>
        public static void RegisterStructuralChanged(IStructuralChangeCallbackReceiver callback, int tableTicket)
        {
            if (tableTicket == -1)
            {
                return;
            }

            if (!k_StructuralChangeCallbackReceivers.ContainsKey(tableTicket))
            {
                k_StructuralChangeCallbackReceivers.Add(tableTicket,
                    new List<IStructuralChangeCallbackReceiver>());
            }

            if (!k_StructuralChangeCallbackReceivers[tableTicket].Contains(callback))
            {
                k_StructuralChangeCallbackReceivers[tableTicket].Add(callback);
            }
        }

        /// <summary>
        ///     Registers a <see cref="DataTableBase" /> for use by all of the different supporting systems inside of
        ///     the Unity editor.
        /// </summary>
        /// <param name="dataTable">The <see cref="DataTableBase" /> to be registered for use.</param>
        /// <returns>A domain incremented ticket number.</returns>
        public static int RegisterTable(DataTableBase dataTable)
        {
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

#if UNITY_2022_2_OR_NEWER
            if (!s_SubscribedToUndo && dataTable.GetMetaData().SupportsUndo)
            {
                Undo.undoRedoEvent += OnUndoRedoEvent;
                s_SubscribedToUndo = true;
            }
#endif

            return head;
        }


        /// <summary>
        ///     Registers a <see cref="DataTableBase" /> for use after a domain reload or undo/redo.
        /// </summary>
        /// <param name="dataTable">The <see cref="DataTableBase" /> to be registered for use.</param>
        /// <param name="tableTicket">
        ///     Force a specific ticket to be used for a table.
        /// </param>
        /// <returns>A domain incremented ticket number.</returns>
        public static int RegisterTableAfterReload(DataTableBase dataTable, int tableTicket)
        {
            // This is the scenario where we need to rebuild after domain reload
            if (tableTicket == -1)
            {
                return RegisterTable(dataTable);
            }

            k_TableTicketToTable[tableTicket] = dataTable;
            k_TableToTableTicket[dataTable] = tableTicket;

#if UNITY_2022_2_OR_NEWER
            k_TableUsageCounters.TryAdd(tableTicket, 0);
#else
            if (!k_TableUsageCounters.ContainsKey(tableTicket))
            {
                k_TableUsageCounters[tableTicket] = 0;
            }
#endif // UNITY_2022_2_OR_NEWER

            if (tableTicket >= s_TableTicketHead)
            {
                s_TableTicketHead = tableTicket + 1;
            }

#if UNITY_2022_2_OR_NEWER
            if (!s_SubscribedToUndo && dataTable.GetMetaData().SupportsUndo)
            {
                Undo.undoRedoEvent += OnUndoRedoEvent;
                s_SubscribedToUndo = true;
            }
#endif
            return tableTicket;
        }

        /// <summary>
        ///     Register an <see cref="IUndoRedoEventCallbackReceiver" /> for callbacks based on the provided ticket.
        /// </summary>
        /// <param name="callback">The target receiver to add to the callback list.</param>
        /// <param name="tableTicket">The unique ticket generated by <see cref="RegisterTable" />.</param>
        public static void RegisterUndoRedoEvent(IUndoRedoEventCallbackReceiver callback, int tableTicket)
        {
            if (tableTicket == -1)
            {
                return;
            }

            if (!k_UndoRedoCallbackReceivers.ContainsKey(tableTicket))
            {
                k_UndoRedoCallbackReceivers.Add(tableTicket,
                    new List<IUndoRedoEventCallbackReceiver>());
            }

            if (!k_UndoRedoCallbackReceivers[tableTicket].Contains(callback))
            {
                k_UndoRedoCallbackReceivers[tableTicket].Add(callback);
            }
        }

        /// <summary>
        ///     Indicate that a <see cref="DataTableBase" /> is no longer being used by something.
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

            DataTableBase table = k_TableTicketToTable[tableTicket];
#if UNITY_2022_2_OR_NEWER
            Undo.ClearUndo(table);
#endif
            k_TableToTableTicket.Remove(table);
            k_TableTicketToTable.Remove(tableTicket);
            k_TableUsageCounters.Remove(tableTicket);
        }

        /// <summary>
        ///     Unregister an <see cref="ICellValueChangedCallbackReceiver" /> from callbacks based on the provided ticket.
        /// </summary>
        /// <param name="callback">The target receiver to remove from the callback list.</param>
        /// <param name="tableTicket">The unique ticket generated by <see cref="RegisterTable" />.</param>
        public static void UnregisterCellValueChanged(ICellValueChangedCallbackReceiver callback, int tableTicket)
        {
            if (k_CellValueChangeCallbackReceivers.TryGetValue(tableTicket,
                    out List<ICellValueChangedCallbackReceiver> receiver))
            {
                receiver.Remove(callback);
            }
        }

        /// <summary>
        ///     Unregister an <see cref="IStructuralChangeCallbackReceiver" /> from callbacks based on the provided ticket.
        /// </summary>
        /// <param name="callback">The target receiver to remove from the callback list.</param>
        /// <param name="tableTicket">The unique ticket generated by <see cref="RegisterTable" />.</param>
        public static void UnregisterStructuralChanged(IStructuralChangeCallbackReceiver callback, int tableTicket)
        {
            if (k_StructuralChangeCallbackReceivers.TryGetValue(tableTicket,
                    out List<IStructuralChangeCallbackReceiver> receiver))
            {
                receiver.Remove(callback);
            }
        }

        /// <summary>
        ///     Unregister an <see cref="IUndoRedoEventCallbackReceiver" /> from callbacks based on the provided ticket.
        /// </summary>
        /// <param name="callback">The target receiver to remove from the callback list.</param>
        /// <param name="tableTicket">The unique ticket generated by <see cref="RegisterTable" />.</param>
        public static void UnregisterUndoRedoEvent(IUndoRedoEventCallbackReceiver callback, int tableTicket)
        {
            if (k_UndoRedoCallbackReceivers.TryGetValue(tableTicket,
                    out List<IUndoRedoEventCallbackReceiver> receiver))
            {
                receiver.Remove(callback);
            }
        }

        /// <summary>
        ///     Generates a weak-reference list of all <see cref="DataTableBase" /> based objects in the Asset Database.
        /// </summary>
        /// <returns>An array of <see cref="AssetDatabaseReference" />s.</returns>
        internal static AssetDatabaseReference[] FindTables()
        {
            List<AssetDatabaseReference> returnList = new List<AssetDatabaseReference>(5);

            // Get derived types
            TypeCache.TypeCollection tableTypeCollection = TypeCache.GetTypesDerivedFrom<DataTableBase>();
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

#if UNITY_2022_2_OR_NEWER
        /// <summary>
        ///     The event fired when Unity performs an undo/redo.
        /// </summary>
        /// <param name="undo">More information about the event that occured.</param>
        static void OnUndoRedoEvent(in UndoRedoInfo undo)
        {
            if (string.CompareOrdinal(undo.undoName.Substring(0, 10), "DataTable ") != 0)
            {
                return;
            }

            string[] chunks = undo.undoName.Split(' ', 6, StringSplitOptions.RemoveEmptyEntries);

            // Only if we have a good ticket
            if (int.TryParse(chunks[1], out int tableTicket))
            {
                // We have no actual watchers for that table
                if (!k_UndoRedoCallbackReceivers.ContainsKey(tableTicket))
                {
                    return;
                }

                int count = k_UndoRedoCallbackReceivers[tableTicket].Count;

                if (string.CompareOrdinal(chunks[2], "Cell") == 0)
                {
                    if (int.TryParse(chunks[3], out int rowIdentifier) &&
                        int.TryParse(chunks[4], out int columnIdentifier))
                    {
                        for (int i = 0; i < count; i++)
                        {
                            k_UndoRedoCallbackReceivers[tableTicket][i]
                                .OnUndoRedoCellValueChanged(rowIdentifier, columnIdentifier);
                        }
                    }
                }
                else if (string.CompareOrdinal(chunks[2], "Row") == 0)
                {
                    if (int.TryParse(chunks[3], out int rowIdentifier))
                    {
                        for (int i = 0; i < count; i++)
                        {
                            k_UndoRedoCallbackReceivers[tableTicket][i].OnUndoRedoRowDefinitionChange(rowIdentifier);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < count; i++)
                        {
                            k_UndoRedoCallbackReceivers[tableTicket][i].OnUndoRedoRowDefinitionChange(-1);
                        }
                    }
                }
                else if (string.CompareOrdinal(chunks[2], "Column") == 0)
                {
                    if (int.TryParse(chunks[3], out int columnIdentifier))
                    {
                        for (int i = 0; i < count; i++)
                        {
                            k_UndoRedoCallbackReceivers[tableTicket][i]
                                .OnUndoRedoColumnDefinitionChange(columnIdentifier);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < count; i++)
                        {
                            k_UndoRedoCallbackReceivers[tableTicket][i].OnUndoRedoColumnDefinitionChange(-1);
                        }
                    }
                }
                else if (string.CompareOrdinal(chunks[2], "Settings") == 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        k_UndoRedoCallbackReceivers[tableTicket][i].OnUndoRedoSettingsChanged();
                    }
                }
            }
        }
#endif

        /// <summary>
        ///     A <see cref="DataTableTracker" /> based set of stats.
        /// </summary>
        public struct DataTableTrackerStats
        {
            public int CellValueChanged;
            public int StructuralChange;
            public int Usages;
        }

        /// <summary>
        ///     An interface describing the functionality needed for an object to get a callback when a
        ///     a cell values changes in a <see cref="DataTableBase" />.
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
        ///     structural changes in a <see cref="DataTableBase" />.
        /// </summary>
        public interface IStructuralChangeCallbackReceiver
        {
            /// <summary>
            ///     A <see cref="ColumnDescription" /> change has occured.
            /// </summary>
            /// <param name="columnIdentifier">The affected column's unique identifier.</param>
            void OnColumnDefinitionChange(int columnIdentifier);

            /// <summary>
            ///     A <see cref="RowDescription" /> change has occured.
            /// </summary>
            /// <param name="rowIdentifier">The affected row's unique identifier.</param>
            void OnRowDefinitionChange(int rowIdentifier);

            /// <summary>
            ///     A <see cref="DataTableBase" /> setting change has occured.
            /// </summary>
            void OnSettingsChange();
        }

        /// <summary>
        ///     An interface describing the functionality needed for an object to get a callback when Unity performs
        ///     an undo/redo operation on a <see cref="DataTableBase" />.
        /// </summary>
        public interface IUndoRedoEventCallbackReceiver
        {
            /// <summary>
            ///     An undo/redo of a <see cref="RowDescription" /> change has occured.
            /// </summary>
            /// <param name="rowIdentifier">The affected row's unique identifier.</param>
            void OnUndoRedoRowDefinitionChange(int rowIdentifier);

            /// <summary>
            ///     An undo/redo of a <see cref="ColumnDescription" /> change has occured.
            /// </summary>
            /// <param name="columnIdentifier">The affected column's unique identifier.</param>
            void OnUndoRedoColumnDefinitionChange(int columnIdentifier);

            /// <summary>
            ///     An undo/redo of a cell value change has occured.
            /// </summary>
            /// <param name="rowIdentifier">The affected row's unique identifier.</param>
            /// <param name="columnIdentifier">The affected column's unique identifier.</param>
            void OnUndoRedoCellValueChanged(int rowIdentifier, int columnIdentifier);

            /// <summary>
            ///     An undo/redo of a settings change has occured.
            /// </summary>
            void OnUndoRedoSettingsChanged();
        }
    }
}