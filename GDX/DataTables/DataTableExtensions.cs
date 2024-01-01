// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using GDX.DataTables.ColumnSorters;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.DataTables
{
    /// <summary>
    ///     <see cref="DataTableBase" /> Based Extension Methods
    /// </summary>
    public static class DataTableExtensions
    {
        /// <summary>
        ///     Get the value of a cell in a <see cref="DataTableBase" />, as a <see cref="string" />.
        /// </summary>
        /// <param name="dataTable">The target Data Table to query for the value.</param>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="columnType">A hint as to the type of the column. If this is not provided a cost will be paid to get it.</param>
        /// <returns>The <see cref="string" /> value of the target row's column.</returns>
        public static string GetCellValueAsString(this DataTableBase dataTable, int rowIdentifier, int columnIdentifier,
            Serializable.SerializableTypes columnType = Serializable.SerializableTypes.Invalid)
        {
            // If type wasn't provided.
            if (columnType == Serializable.SerializableTypes.Invalid)
            {
                columnType = dataTable.GetColumnDescription(columnIdentifier).Type;
            }

            switch (columnType)
            {
                case Serializable.SerializableTypes.String:
                    return dataTable.GetString(rowIdentifier, columnIdentifier);
                case Serializable.SerializableTypes.Char:
                    return dataTable.GetChar(rowIdentifier, columnIdentifier).ToString();
                case Serializable.SerializableTypes.Bool:
                    return dataTable.GetBool(rowIdentifier, columnIdentifier).ToString();
                case Serializable.SerializableTypes.SByte:
                    return dataTable.GetSByte(rowIdentifier, columnIdentifier).ToString();
                case Serializable.SerializableTypes.Byte:
                    return dataTable.GetByte(rowIdentifier, columnIdentifier).ToString();
                case Serializable.SerializableTypes.Short:
                    return dataTable.GetShort(rowIdentifier, columnIdentifier).ToString();
                case Serializable.SerializableTypes.UShort:
                    return dataTable.GetUShort(rowIdentifier, columnIdentifier).ToString();
                case Serializable.SerializableTypes.Int:
                    return dataTable.GetInt(rowIdentifier, columnIdentifier).ToString();
                case Serializable.SerializableTypes.UInt:
                    return dataTable.GetUInt(rowIdentifier, columnIdentifier).ToString();
                case Serializable.SerializableTypes.Long:
                    return dataTable.GetLong(rowIdentifier, columnIdentifier).ToString();
                case Serializable.SerializableTypes.ULong:
                    return dataTable.GetULong(rowIdentifier, columnIdentifier).ToString();
                case Serializable.SerializableTypes.Float:
                    return dataTable.GetFloat(rowIdentifier, columnIdentifier).ToString(CultureInfo.InvariantCulture);
                case Serializable.SerializableTypes.Double:
                    return dataTable.GetDouble(rowIdentifier, columnIdentifier).ToString(CultureInfo.InvariantCulture);
                case Serializable.SerializableTypes.Vector2:
                    return dataTable.GetUShort(rowIdentifier, columnIdentifier).ToString();
                case Serializable.SerializableTypes.Vector3:
                    return dataTable.GetVector3(rowIdentifier, columnIdentifier).ToString();
                case Serializable.SerializableTypes.Vector4:
                    return dataTable.GetVector4(rowIdentifier, columnIdentifier).ToString();
                case Serializable.SerializableTypes.Vector2Int:
                    return dataTable.GetVector2Int(rowIdentifier, columnIdentifier).ToString();
                case Serializable.SerializableTypes.Vector3Int:
                    return dataTable.GetVector3Int(rowIdentifier, columnIdentifier).ToString();
                case Serializable.SerializableTypes.Quaternion:
                    return dataTable.GetQuaternion(rowIdentifier, columnIdentifier).ToString();
                case Serializable.SerializableTypes.Rect:
                    return dataTable.GetRect(rowIdentifier, columnIdentifier).ToString();
                case Serializable.SerializableTypes.RectInt:
                    return dataTable.GetRectInt(rowIdentifier, columnIdentifier).ToString();
                case Serializable.SerializableTypes.Color:
                    return dataTable.GetColor(rowIdentifier, columnIdentifier).ToString();
                case Serializable.SerializableTypes.LayerMask:
                    return dataTable.GetLayerMask(rowIdentifier, columnIdentifier).ToString();
                case Serializable.SerializableTypes.Bounds:
                    return dataTable.GetBounds(rowIdentifier, columnIdentifier).ToString();
                case Serializable.SerializableTypes.BoundsInt:
                    return dataTable.GetBoundsInt(rowIdentifier, columnIdentifier).ToString();
                case Serializable.SerializableTypes.Hash128:
                    return dataTable.GetHash128(rowIdentifier, columnIdentifier).ToString();
                case Serializable.SerializableTypes.Gradient:
                    return dataTable.GetGradient(rowIdentifier, columnIdentifier)?.ToString();
                case Serializable.SerializableTypes.AnimationCurve:
                    return dataTable.GetAnimationCurve(rowIdentifier, columnIdentifier)?.ToString();
                case Serializable.SerializableTypes.Object:
                    return dataTable.GetObject(rowIdentifier, columnIdentifier)?.ToString();
                case Serializable.SerializableTypes.EnumInt:
                    return dataTable.GetEnumInt(rowIdentifier, columnIdentifier).ToString();
            }

            return null;
        }

        /// <summary>
        ///     Set the value of a cell in a <see cref="DataTableBase" /> from a <see cref="string" /> value.
        /// </summary>
        /// <param name="dataTable">The target Data Table to set a value for.</param>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newValue">The value to set.</param>
        /// <param name="columnType">A hint as to the type of the column. If this is not provided a cost will be paid to get it.</param>
        public static void SetCellValueFromString(this DataTableBase dataTable, int rowIdentifier, int columnIdentifier,
            string newValue,
            Serializable.SerializableTypes columnType = Serializable.SerializableTypes.Invalid)
        {
            // If type wasn't provided.
            if (columnType == Serializable.SerializableTypes.Invalid)
            {
                columnType = dataTable.GetColumnDescription(columnIdentifier).Type;
            }

            switch (columnType)
            {
                case Serializable.SerializableTypes.String:
                    dataTable.SetString(rowIdentifier, columnIdentifier, newValue);
                    break;
                case Serializable.SerializableTypes.Char:
                    dataTable.SetChar(rowIdentifier, columnIdentifier, newValue[0]);
                    break;
                case Serializable.SerializableTypes.Bool:
                    dataTable.SetBool(rowIdentifier, columnIdentifier,
                        bool.Parse(newValue));
                    break;
                case Serializable.SerializableTypes.SByte:
                    dataTable.SetSByte(rowIdentifier, columnIdentifier,
                        sbyte.Parse(newValue));
                    break;
                case Serializable.SerializableTypes.Byte:
                    dataTable.SetByte(rowIdentifier, columnIdentifier,
                        byte.Parse(newValue));
                    break;
                case Serializable.SerializableTypes.Short:
                    dataTable.SetShort(rowIdentifier, columnIdentifier,
                        short.Parse(newValue));
                    break;
                case Serializable.SerializableTypes.UShort:
                    dataTable.SetUShort(rowIdentifier, columnIdentifier,
                        ushort.Parse(newValue));
                    break;
                case Serializable.SerializableTypes.Int:
                    dataTable.SetInt(rowIdentifier, columnIdentifier, int.Parse(newValue));
                    break;
                case Serializable.SerializableTypes.UInt:
                    dataTable.SetUInt(rowIdentifier, columnIdentifier,
                        uint.Parse(newValue));
                    break;
                case Serializable.SerializableTypes.Long:
                    dataTable.SetLong(rowIdentifier, columnIdentifier,
                        long.Parse(newValue));
                    break;
                case Serializable.SerializableTypes.ULong:
                    dataTable.SetULong(rowIdentifier, columnIdentifier,
                        ulong.Parse(newValue));
                    break;
                case Serializable.SerializableTypes.Float:
                    dataTable.SetFloat(rowIdentifier, columnIdentifier,
                        float.Parse(newValue));
                    break;
                case Serializable.SerializableTypes.Double:
                    dataTable.SetDouble(rowIdentifier, columnIdentifier,
                        double.Parse(newValue));
                    break;
            }
        }

#if UNITY_2022_2_OR_NEWER

        /// <summary>
        ///     Sort <see cref="DataTableBase" /> rows based on the provided sorting parameters.
        /// </summary>
        /// <param name="dataTable">The target <see cref="DataTableBase" />.</param>
        /// <param name="columnIdentifiers">The column identifiers to use in order of priority.</param>
        /// <param name="columnTypes">The types of the given columns.</param>
        /// <param name="directions">The direction which the column should be used to calculate order.</param>
        /// <returns>A sorted array of <see cref="RowDescription" />, or null.</returns>
        public static RowDescription[] GetAllRowDescriptionsSortedByColumns(this DataTableBase dataTable,
            int[] columnIdentifiers,
            Serializable.SerializableTypes[] columnTypes, SortDirection[] directions)
        {
            RowDescription[] rows = dataTable.GetAllRowDescriptions();

            if (columnIdentifiers.Length != columnTypes.Length || columnTypes.Length != directions.Length)
            {
                Debug.LogError("Unable to sort, all arrays much have same length.");
                return rows;
            }


            int rowCount = dataTable.GetRowCount();
            if (rowCount <= 1 || columnIdentifiers.Length == 0)
            {
                return rows;
            }

            bool multiSort = columnIdentifiers.Length > 1;

            // Primary Sort Pass
            List<int> needAdditionalSortingIdentifiers = new List<int>(rowCount);
            IComparer<RowDescription> primaryComparer = GetComparer(columnTypes[0], dataTable, rowCount,
                columnIdentifiers[0], (int)directions[0], multiSort);
            if (primaryComparer == null)
            {
                return rows;
            }

            Array.Sort(rows, 0, rowCount, primaryComparer);

            // Multi column support
            if (multiSort)
            {
                ColumnSorterBase primarySorter = (ColumnSorterBase)primaryComparer;
                needAdditionalSortingIdentifiers.AddRange(primarySorter.EqualIdentifiers);
                if (needAdditionalSortingIdentifiers.Count > 0)
                {
                    // We have additional sorting to be done and have additional sorter options
                    int sorterIndex = 1;

                    // Iterate till we have nothing to do
                    while (needAdditionalSortingIdentifiers.Count > 0 && sorterIndex < columnIdentifiers.Length)
                    {
                        int currentSortingCount = needAdditionalSortingIdentifiers.Count;

                        // Build list
                        List<int> startIndex = new List<int>(currentSortingCount);
                        List<int> stopIndex = new List<int>(currentSortingCount);
                        bool insideOfRange = false;
                        for (int i = 0; i < rowCount; i++)
                        {
                            ref RowDescription currentRow = ref rows[i];

                            if (needAdditionalSortingIdentifiers.Contains(currentRow.Identifier) && !insideOfRange)
                            {
                                startIndex.Add(i);
                                insideOfRange = true;
                                continue;
                            }

                            if (insideOfRange && !needAdditionalSortingIdentifiers.Contains(currentRow.Identifier))
                            {
                                stopIndex.Add(i - 1);
                                insideOfRange = false;
                            }
                        }

                        // If the last item
                        if (insideOfRange)
                        {
                            stopIndex.Add(rowCount - 1);
                        }

                        // Clear the scratch pad now that we've created our actual indices
                        needAdditionalSortingIdentifiers.Clear();

                        int indexCount = startIndex.Count;
                        for (int i = 0; i < indexCount; i++)
                        {
                            int length = stopIndex[i] - startIndex[i] + 1;
                            IComparer<RowDescription> secondaryComparer = GetComparer(columnTypes[sorterIndex],
                                dataTable, length,
                                columnIdentifiers[sorterIndex], (int)directions[sorterIndex], true);

                            if (secondaryComparer == null)
                            {
                                break;
                            }

                            Array.Sort(rows, startIndex[i], length, secondaryComparer);
                            ColumnSorterBase secondarySorter = (ColumnSorterBase)secondaryComparer;
                            needAdditionalSortingIdentifiers.AddRange(secondarySorter.EqualIdentifiers);
                        }

                        // Next sorter it appears
                        sorterIndex++;
                    }
                }
            }

            return rows;
        }

        /// <summary>
        ///     Get the appropriate <see cref="IComparer{T}" /> for a <see cref="DataTableBase" /> column.
        /// </summary>
        /// <param name="type">The <see cref="Serializable.SerializableTypes" /> of the column.</param>
        /// <param name="dataTable">The target <see cref="DataTableBase" /> the column belongs to.</param>
        /// <param name="rowCount">
        ///     The number of values to be sorted. This is only used to help pre-size an internal counter used
        ///     for multi-sort.
        /// </param>
        /// <param name="columnIdentifier">The identifier of the column being sorted.</param>
        /// <param name="direction">The direction to sort.</param>
        /// <param name="supportMultiSort">Should the extra multi-sort work be done?</param>
        /// <returns>A qualified <see cref="IComparer{T}" /> for the column.</returns>
        static IComparer<RowDescription> GetComparer(Serializable.SerializableTypes type, DataTableBase dataTable,
            int rowCount, int columnIdentifier, int direction, bool supportMultiSort = false)
        {
            if (columnIdentifier == -1 && type == Serializable.SerializableTypes.String)
            {
                return new RowNameColumnSorter(dataTable, rowCount, -1, direction, supportMultiSort);
            }

            // Default sorters
            switch (type)
            {
                case Serializable.SerializableTypes.Float:
                    return new FloatColumnSorter(dataTable, rowCount, columnIdentifier, direction, supportMultiSort);
                case Serializable.SerializableTypes.Int:
                    return new IntColumnSorter(dataTable, rowCount, columnIdentifier, direction, supportMultiSort);
                case Serializable.SerializableTypes.String:
                    return new StringColumnSorter(dataTable, rowCount, columnIdentifier, direction, supportMultiSort);
                case Serializable.SerializableTypes.Bool:
                    return new BoolColumnSorter(dataTable, rowCount, columnIdentifier, direction, supportMultiSort);
                case Serializable.SerializableTypes.Double:
                    return new DoubleColumnSorter(dataTable, rowCount, columnIdentifier, direction, supportMultiSort);
                case Serializable.SerializableTypes.Long:
                    return new LongColumnSorter(dataTable, rowCount, columnIdentifier, direction, supportMultiSort);
                case Serializable.SerializableTypes.ULong:
                    return new ULongColumnSorter(dataTable, rowCount, columnIdentifier, direction, supportMultiSort);
                case Serializable.SerializableTypes.UInt:
                    return new UIntColumnSorter(dataTable, rowCount, columnIdentifier, direction, supportMultiSort);
                case Serializable.SerializableTypes.EnumInt:
                    return new EnumIntColumnSorter(dataTable, rowCount, columnIdentifier, direction, supportMultiSort);
            }

            Debug.LogError($"Unable to find sorter for requested column type [{type.GetLabel()}].");
            return null;
        }
#endif
    }
}