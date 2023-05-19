// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using GDX.Collections;
using UnityEngine;

namespace GDX.Tables
{

    [CreateAssetMenu(menuName = "GDX/Stable Table", fileName = "GDXStableTable")]
    [Serializable]
    public class StableTable : ScriptableObject, ITable
    {
        [Serializable]
        internal struct ColumnEntry
        {
            public Serializable.SerializableTypes ColumnType;
            public int columnDenseIndex;
        }

        [SerializeField] internal ArrayHolder<string>[] allStringColumns;
        [SerializeField] internal ArrayHolder<bool>[] allBoolColumns;
        [SerializeField] internal ArrayHolder<char>[] allCharColumns;
        [SerializeField] internal ArrayHolder<sbyte>[] allSbyteColumns;
        [SerializeField] internal ArrayHolder<byte>[] allByteColumns;
        [SerializeField] internal ArrayHolder<short>[] allShortColumns;
        [SerializeField] internal ArrayHolder<ushort>[] allUshortColumns;
        [SerializeField] internal ArrayHolder<int>[] allIntColumns;
        [SerializeField] internal ArrayHolder<uint>[] allUintColumns;
        [SerializeField] internal ArrayHolder<long>[] allLongColumns;
        [SerializeField] internal ArrayHolder<ulong>[] allUlongColumns;
        [SerializeField] internal ArrayHolder<float>[] allFloatColumns;
        [SerializeField] internal ArrayHolder<double>[] allDoubleColumns;
        [SerializeField] internal ArrayHolder<Vector2>[] allVector2Columns;
        [SerializeField] internal ArrayHolder<Vector3>[] allVector3Columns;
        [SerializeField] internal ArrayHolder<Vector4>[] allVector4Columns;
        [SerializeField] internal ArrayHolder<Vector2Int>[] allVector2IntColumns;
        [SerializeField] internal ArrayHolder<Vector3Int>[] allVector3IntColumns;
        [SerializeField] internal ArrayHolder<Quaternion>[] allQuaternionColumns;
        [SerializeField] internal ArrayHolder<Rect>[] allRectColumns;
        [SerializeField] internal ArrayHolder<RectInt>[] allRectIntColumns;
        [SerializeField] internal ArrayHolder<Color>[] allColorColumns;
        [SerializeField] internal ArrayHolder<LayerMask>[] allLayerMaskColumns;
        [SerializeField] internal ArrayHolder<Bounds>[] allBoundsColumns;
        [SerializeField] internal ArrayHolder<BoundsInt>[] allBoundsIntColumns;
        [SerializeField] internal ArrayHolder<Hash128>[] allHash128Columns;
        [SerializeField] internal ArrayHolder<Gradient>[] allGradientColumns;
        [SerializeField] internal ArrayHolder<AnimationCurve>[] allAnimationCurveColumns;
        [SerializeField] internal ArrayHolder<UnityEngine.Object>[] allObjectRefColumns;
        [SerializeField] internal ArrayHolder<string>[] allColumnNames = new ArrayHolder<string>[Serializable.SerializableTypesCount]; // Contains the name of each column of each type. Ordered by Serializable.SerializableTypes
        [SerializeField] internal ArrayHolder<int>[] allColumnOrders = new ArrayHolder<int>[Serializable.SerializableTypesCount]; // Contains the left-to-right order of each column of each type. Ordered by Serializable.SerializableTypes


        [SerializeField] internal string[] allRowNames;
        [SerializeField] internal int[] rowIDToDenseIndexMap;
        [SerializeField] internal int[] rowDenseIndexToIDMap;
        [SerializeField] internal int rowEntriesFreeListHead;


        [SerializeField]
        internal int rowCount;

        [SerializeField]
        internal ColumnEntry[] columnIDToDenseIndexMap;

        // TODO move with other block
        [SerializeField] ArrayHolder<int>[] columnDenseIndexToIDMap = new ArrayHolder<int>[Serializable.SerializableTypesCount];

        [SerializeField]
        internal int columnEntriesFreeListHead;

        [SerializeField]
        internal int combinedColumnCount;

        [SerializeField]
        internal ulong dataVersion = 1;


        public ulong GetDataVersion()
        {
            return dataVersion;
        }

        /// <inheritdoc />
        public int GetColumnCount()
        {
            return combinedColumnCount;
        }

        /// <inheritdoc />
        public int GetRowCount()
        {
            return rowCount;
        }

        public ITable.RowDescription[] GetAllRowDescriptions()
        {
            if (combinedColumnCount == 0) return null;
            ITable.RowDescription[] returnArray = new ITable.RowDescription[rowCount];

            // TODO: populate with stable etc

            return returnArray;
        }
        public ITable.RowDescription GetRowDescription(string name)
        {
            throw new NotImplementedException();
        }

        public ITable.RowDescription GetRowDescription(int order)
        {
            throw new NotImplementedException();
        }

        public ITable.ColumnDescription GetColumnDescription(string name)
        {
            throw new NotImplementedException();
        }

        public ITable.ColumnDescription GetColumnDescription(int order)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ITable.ColumnDescription[] GetAllColumnDescriptions()
        {
            if (combinedColumnCount == 0) return null;
            ITable.ColumnDescription[] returnArray = new ITable.ColumnDescription[combinedColumnCount];

            for (int columnIndex = 0; columnIndex < Serializable.SerializableTypesCount; columnIndex++)
            {

                int[] columnOrders = allColumnOrders[columnIndex].TArray;
                int columnOrdersLength = columnOrders?.Length ?? 0;

                int[] columnIndices = columnDenseIndexToIDMap[columnIndex].TArray;
                string[] columnNames = allColumnNames[columnIndex].TArray;

                for (int i = 0; i < columnOrdersLength; i++)
                {
                    returnArray[columnOrders[i]] = new ITable.ColumnDescription
                    {
                        Name =  columnNames[i],
                        Index = columnIndices[i],
                        Type = (Serializable.SerializableTypes)columnIndex
                    };
                }
            }
            return returnArray;
        }

        public void SetColumnName(string name, int column)
        {
            throw new NotImplementedException();
            // TODO: Way to set column name
        }





        public void AddRow(string rowName = null, int insertAt = -1)
        {
            // TODO: For adam to do
            // int rowIndex = rowEntriesFreeListHead;
            // int rowIDToDenseIndexMapLength = rowIDToDenseIndexMap?.Length ?? 0;
            // if (rowIndex >= rowIDToDenseIndexMapLength)
            // {
            //     int newSize = rowIndex * 2;
            //     newSize = newSize == 0 ? 1 : newSize;
            //     Array.Resize(ref rowIDToDenseIndexMap, newSize);
            //     for (int i = 0; i < rowIndex; i++)
            //     {
            //         rowIDToDenseIndexMap[rowIndex + i] = rowIndex + i + 1;
            //     }
            // }
            // int denseIndexToIDMapLength = rowDenseIndexToIDMap?.Length ?? 0;
            // Array.Resize(ref rowDenseIndexToIDMap, denseIndexToIDMapLength + 1);
            //
            //
            // rowDenseIndexToIDMap[denseIndexToIDMapLength] = rowIndex;


            insertAt = insertAt < 0 ? rowCount : insertAt;

            Array.Resize(ref allRowNames, rowCount + 1);
            for (int i = rowCount; i > insertAt; i--)
            {
                allRowNames[i] = allRowNames[i - 1];
            }

            rowName ??= string.Empty;
            allRowNames[insertAt] = rowName;

            InsertRowsOfTypeInternal(ref allStringColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref allBoolColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref allCharColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref allSbyteColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref allByteColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref allShortColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref allUshortColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref allIntColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref allUintColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref allLongColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref allUlongColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref allFloatColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref allDoubleColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref allVector2Columns, insertAt, 1);
            InsertRowsOfTypeInternal(ref allVector3Columns, insertAt, 1);
            InsertRowsOfTypeInternal(ref allVector4Columns, insertAt, 1);
            InsertRowsOfTypeInternal(ref allVector2IntColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref allVector3IntColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref allQuaternionColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref allRectColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref allRectIntColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref allColorColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref allLayerMaskColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref allBoundsColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref allBoundsIntColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref allHash128Columns, insertAt, 1);
            InsertRowsOfTypeInternal(ref allGradientColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref allAnimationCurveColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref allObjectRefColumns, insertAt, 1);

            ++rowCount;
            dataVersion++;
        }

        public void AddRows(int numberOfNewRows, string[] rowNames = null, int insertAt = -1)
        {
            insertAt = insertAt < 0 ? rowCount : insertAt;

            Array.Resize(ref allRowNames, rowCount + 1);
            for (int i = rowCount; i > insertAt; i--)
            {
                allRowNames[i] = allRowNames[i - 1];
            }

            string empty = string.Empty;
            int rowNamesLength = rowNames?.Length ?? 0;
            for (int i = 0; i < rowNames.Length; i++)
            {
                string nameAt = rowNames[i];
                allRowNames[insertAt + i] = nameAt ?? empty;
            }

            for (int i = rowNamesLength; i < numberOfNewRows; i++)
            {
                allRowNames[insertAt + i] = empty;
            }

            InsertRowsOfTypeInternal(ref allStringColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref allBoolColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref allCharColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref allSbyteColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref allByteColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref allShortColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref allUshortColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref allIntColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref allUintColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref allLongColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref allUlongColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref allFloatColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref allDoubleColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref allVector2Columns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref allVector3Columns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref allVector4Columns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref allVector2IntColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref allVector3IntColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref allQuaternionColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref allRectColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref allRectIntColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref allColorColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref allLayerMaskColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref allBoundsColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref allBoundsIntColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref allHash128Columns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref allGradientColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref allAnimationCurveColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref allObjectRefColumns, insertAt, numberOfNewRows);

            rowCount += numberOfNewRows;
            dataVersion++;
        }

        public void RemoveRow(int removeAt)
        {
            int newRowCount = rowCount - 1;
            for (int j = removeAt; j < newRowCount; j++)
            {
                allRowNames[j] = allRowNames[j + 1];
            }

            Array.Resize(ref allRowNames, newRowCount);

            DeleteRowsOfTypeInternal(ref allStringColumns, removeAt, 1);
            DeleteRowsOfTypeInternal(ref allBoolColumns, removeAt, 1);
            DeleteRowsOfTypeInternal(ref allCharColumns, removeAt, 1);
            DeleteRowsOfTypeInternal(ref allSbyteColumns, removeAt, 1);
            DeleteRowsOfTypeInternal(ref allByteColumns, removeAt, 1);
            DeleteRowsOfTypeInternal(ref allShortColumns, removeAt, 1);
            DeleteRowsOfTypeInternal(ref allUshortColumns, removeAt, 1);
            DeleteRowsOfTypeInternal(ref allIntColumns, removeAt, 1);
            DeleteRowsOfTypeInternal(ref allUintColumns, removeAt, 1);
            DeleteRowsOfTypeInternal(ref allLongColumns, removeAt, 1);
            DeleteRowsOfTypeInternal(ref allUlongColumns, removeAt, 1);
            DeleteRowsOfTypeInternal(ref allFloatColumns, removeAt, 1);
            DeleteRowsOfTypeInternal(ref allDoubleColumns, removeAt, 1);
            DeleteRowsOfTypeInternal(ref allVector2Columns, removeAt, 1);
            DeleteRowsOfTypeInternal(ref allVector3Columns, removeAt, 1);
            DeleteRowsOfTypeInternal(ref allVector4Columns, removeAt, 1);
            DeleteRowsOfTypeInternal(ref allVector2IntColumns, removeAt, 1);
            DeleteRowsOfTypeInternal(ref allVector3IntColumns, removeAt, 1);
            DeleteRowsOfTypeInternal(ref allQuaternionColumns, removeAt, 1);
            DeleteRowsOfTypeInternal(ref allRectColumns, removeAt, 1);
            DeleteRowsOfTypeInternal(ref allRectIntColumns, removeAt, 1);
            DeleteRowsOfTypeInternal(ref allColorColumns, removeAt, 1);
            DeleteRowsOfTypeInternal(ref allLayerMaskColumns, removeAt, 1);
            DeleteRowsOfTypeInternal(ref allBoundsColumns, removeAt, 1);
            DeleteRowsOfTypeInternal(ref allBoundsIntColumns, removeAt, 1);
            DeleteRowsOfTypeInternal(ref allHash128Columns, removeAt, 1);
            DeleteRowsOfTypeInternal(ref allGradientColumns, removeAt, 1);
            DeleteRowsOfTypeInternal(ref allAnimationCurveColumns, removeAt, 1);
            DeleteRowsOfTypeInternal(ref allObjectRefColumns, removeAt, 1);

            --rowCount;
            dataVersion++;
        }

        public void RemoveRows(int removeAt, int numberOfRowsToDelete)
        {
            int newRowCount = rowCount - numberOfRowsToDelete;
            for (int j = removeAt; j < rowCount - numberOfRowsToDelete; j++)
            {
                allRowNames[j] = allRowNames[j + numberOfRowsToDelete];
            }

            Array.Resize(ref allRowNames, newRowCount);

            DeleteRowsOfTypeInternal(ref allStringColumns, removeAt, numberOfRowsToDelete);
            DeleteRowsOfTypeInternal(ref allBoolColumns, removeAt, numberOfRowsToDelete);
            DeleteRowsOfTypeInternal(ref allCharColumns, removeAt, numberOfRowsToDelete);
            DeleteRowsOfTypeInternal(ref allSbyteColumns, removeAt, numberOfRowsToDelete);
            DeleteRowsOfTypeInternal(ref allByteColumns, removeAt, numberOfRowsToDelete);
            DeleteRowsOfTypeInternal(ref allShortColumns, removeAt, numberOfRowsToDelete);
            DeleteRowsOfTypeInternal(ref allUshortColumns, removeAt, numberOfRowsToDelete);
            DeleteRowsOfTypeInternal(ref allIntColumns, removeAt, numberOfRowsToDelete);
            DeleteRowsOfTypeInternal(ref allUintColumns, removeAt, numberOfRowsToDelete);
            DeleteRowsOfTypeInternal(ref allLongColumns, removeAt, numberOfRowsToDelete);
            DeleteRowsOfTypeInternal(ref allUlongColumns, removeAt, numberOfRowsToDelete);
            DeleteRowsOfTypeInternal(ref allFloatColumns, removeAt, numberOfRowsToDelete);
            DeleteRowsOfTypeInternal(ref allDoubleColumns, removeAt, numberOfRowsToDelete);
            DeleteRowsOfTypeInternal(ref allVector2Columns, removeAt, numberOfRowsToDelete);
            DeleteRowsOfTypeInternal(ref allVector3Columns, removeAt, numberOfRowsToDelete);
            DeleteRowsOfTypeInternal(ref allVector4Columns, removeAt, numberOfRowsToDelete);
            DeleteRowsOfTypeInternal(ref allVector2IntColumns, removeAt, numberOfRowsToDelete);
            DeleteRowsOfTypeInternal(ref allVector3IntColumns, removeAt, numberOfRowsToDelete);
            DeleteRowsOfTypeInternal(ref allQuaternionColumns, removeAt, numberOfRowsToDelete);
            DeleteRowsOfTypeInternal(ref allRectColumns, removeAt, numberOfRowsToDelete);
            DeleteRowsOfTypeInternal(ref allRectIntColumns, removeAt, numberOfRowsToDelete);
            DeleteRowsOfTypeInternal(ref allColorColumns, removeAt, numberOfRowsToDelete);
            DeleteRowsOfTypeInternal(ref allLayerMaskColumns, removeAt, numberOfRowsToDelete);
            DeleteRowsOfTypeInternal(ref allBoundsColumns, removeAt, numberOfRowsToDelete);
            DeleteRowsOfTypeInternal(ref allBoundsIntColumns, removeAt, numberOfRowsToDelete);
            DeleteRowsOfTypeInternal(ref allHash128Columns, removeAt, numberOfRowsToDelete);
            DeleteRowsOfTypeInternal(ref allGradientColumns, removeAt, numberOfRowsToDelete);
            DeleteRowsOfTypeInternal(ref allAnimationCurveColumns, removeAt, numberOfRowsToDelete);
            DeleteRowsOfTypeInternal(ref allObjectRefColumns, removeAt, numberOfRowsToDelete);

            rowCount -= numberOfRowsToDelete;
            dataVersion++;
        }


        // TODO: Need to make sure that insertAt is the stableID, also this should return the stableID of the newly created row.
        public int AddColumn(Serializable.SerializableTypes columnType, string columnName, int insertAt = -1)
        {
            switch (columnType)
            {
                case Serializable.SerializableTypes.String:
                    return AddColumnInternal(columnName, ref allStringColumns, Serializable.SerializableTypes.String, insertAt);
                case Serializable.SerializableTypes.Char:
                    return AddColumnInternal(columnName, ref allCharColumns, Serializable.SerializableTypes.Char, insertAt);
                case Serializable.SerializableTypes.Bool:
                    return AddColumnInternal(columnName, ref allBoolColumns, Serializable.SerializableTypes.Bool, insertAt);
                case Serializable.SerializableTypes.SByte:
                    return AddColumnInternal(columnName, ref allSbyteColumns, Serializable.SerializableTypes.SByte, insertAt);
                case Serializable.SerializableTypes.Byte:
                    return AddColumnInternal(columnName, ref allByteColumns, Serializable.SerializableTypes.Byte, insertAt);
                case Serializable.SerializableTypes.Short:
                    return AddColumnInternal(columnName, ref allShortColumns, Serializable.SerializableTypes.Short, insertAt);
                case Serializable.SerializableTypes.UShort:
                    return AddColumnInternal(columnName, ref allUshortColumns, Serializable.SerializableTypes.UShort, insertAt);
                case Serializable.SerializableTypes.Int:
                    return AddColumnInternal(columnName, ref allIntColumns, Serializable.SerializableTypes.Int, insertAt);
                case Serializable.SerializableTypes.UInt:
                    return AddColumnInternal(columnName, ref allUintColumns, Serializable.SerializableTypes.UInt, insertAt);
                case Serializable.SerializableTypes.Long:
                    return AddColumnInternal(columnName, ref allLongColumns, Serializable.SerializableTypes.Long, insertAt);
                case Serializable.SerializableTypes.ULong:
                    return AddColumnInternal(columnName, ref allUlongColumns, Serializable.SerializableTypes.ULong, insertAt);
                case Serializable.SerializableTypes.Float:
                    return AddColumnInternal(columnName, ref allFloatColumns, Serializable.SerializableTypes.Float, insertAt);
                case Serializable.SerializableTypes.Double:
                    return AddColumnInternal(columnName, ref allDoubleColumns, Serializable.SerializableTypes.Double, insertAt);
                case Serializable.SerializableTypes.Vector2:
                    return AddColumnInternal(columnName, ref allVector2Columns, Serializable.SerializableTypes.Vector2, insertAt);
                case Serializable.SerializableTypes.Vector3:
                    return AddColumnInternal(columnName, ref allVector3Columns, Serializable.SerializableTypes.Vector3, insertAt);
                case Serializable.SerializableTypes.Vector4:
                    return AddColumnInternal(columnName, ref allVector4Columns, Serializable.SerializableTypes.Vector4, insertAt);
                case Serializable.SerializableTypes.Vector2Int:
                    return AddColumnInternal(columnName, ref allVector2IntColumns, Serializable.SerializableTypes.Vector2Int, insertAt);
                case Serializable.SerializableTypes.Vector3Int:
                    return AddColumnInternal(columnName, ref allVector3IntColumns, Serializable.SerializableTypes.Vector3Int, insertAt);
                case Serializable.SerializableTypes.Quaternion:
                    return AddColumnInternal(columnName, ref allQuaternionColumns, Serializable.SerializableTypes.Quaternion, insertAt);
                case Serializable.SerializableTypes.Rect:
                    return AddColumnInternal(columnName, ref allRectColumns, Serializable.SerializableTypes.Rect, insertAt);
                case Serializable.SerializableTypes.RectInt:
                    return AddColumnInternal(columnName, ref allRectIntColumns, Serializable.SerializableTypes.RectInt, insertAt);
                case Serializable.SerializableTypes.Color:
                    return AddColumnInternal(columnName, ref allColorColumns, Serializable.SerializableTypes.Color, insertAt);
                case Serializable.SerializableTypes.LayerMask:
                    return AddColumnInternal(columnName, ref allLayerMaskColumns, Serializable.SerializableTypes.LayerMask, insertAt);
                case Serializable.SerializableTypes.Bounds:
                    return AddColumnInternal(columnName, ref allBoundsColumns, Serializable.SerializableTypes.Bounds, insertAt);
                case Serializable.SerializableTypes.BoundsInt:
                    return AddColumnInternal(columnName, ref allBoundsIntColumns, Serializable.SerializableTypes.BoundsInt, insertAt);
                case Serializable.SerializableTypes.Hash128:
                    return AddColumnInternal(columnName, ref allHash128Columns, Serializable.SerializableTypes.Hash128, insertAt);
                case Serializable.SerializableTypes.Gradient:
                    return AddColumnInternal(columnName, ref allGradientColumns, Serializable.SerializableTypes.Gradient, insertAt);
                case Serializable.SerializableTypes.AnimationCurve:
                    return AddColumnInternal(columnName, ref allAnimationCurveColumns, Serializable.SerializableTypes.AnimationCurve, insertAt);
                case Serializable.SerializableTypes.Object:
                    return AddColumnInternal(columnName, ref allObjectRefColumns, Serializable.SerializableTypes.Object, insertAt);
            }
            return -1;
        }

        // TODO: need to make sure this is the stable ID?
        public void RemoveColumn(Serializable.SerializableTypes columnType, int removeAt = -1)
        {
            switch (columnType)
            {
                case Serializable.SerializableTypes.String:
                    RemoveColumnInternal(ref allStringColumns, Serializable.SerializableTypes.String, removeAt);
                    break;
                case Serializable.SerializableTypes.Char:
                    RemoveColumnInternal(ref allCharColumns, Serializable.SerializableTypes.Char, removeAt);
                    break;
                case Serializable.SerializableTypes.Bool:
                    RemoveColumnInternal(ref allBoolColumns, Serializable.SerializableTypes.Bool, removeAt);
                    break;
                case Serializable.SerializableTypes.SByte:
                    RemoveColumnInternal(ref allSbyteColumns, Serializable.SerializableTypes.SByte, removeAt);
                    break;
                case Serializable.SerializableTypes.Byte:
                    RemoveColumnInternal(ref allByteColumns, Serializable.SerializableTypes.Byte, removeAt);
                    break;
                case Serializable.SerializableTypes.Short:
                    RemoveColumnInternal(ref allShortColumns, Serializable.SerializableTypes.Short, removeAt);
                    break;
                case Serializable.SerializableTypes.UShort:
                    RemoveColumnInternal(ref allUshortColumns, Serializable.SerializableTypes.UShort, removeAt);
                    break;
                case Serializable.SerializableTypes.Int:
                    RemoveColumnInternal(ref allIntColumns, Serializable.SerializableTypes.Int, removeAt);
                    break;
                case Serializable.SerializableTypes.UInt:
                    RemoveColumnInternal(ref allUintColumns, Serializable.SerializableTypes.UInt, removeAt);
                    break;
                case Serializable.SerializableTypes.Long:
                    RemoveColumnInternal(ref allLongColumns, Serializable.SerializableTypes.Long, removeAt);
                    break;
                case Serializable.SerializableTypes.ULong:
                    RemoveColumnInternal(ref allUlongColumns, Serializable.SerializableTypes.ULong, removeAt);
                    break;
                case Serializable.SerializableTypes.Float:
                    RemoveColumnInternal(ref allFloatColumns, Serializable.SerializableTypes.Float, removeAt);
                    break;
                case Serializable.SerializableTypes.Double:
                    RemoveColumnInternal(ref allDoubleColumns, Serializable.SerializableTypes.Double, removeAt);
                    break;
                case Serializable.SerializableTypes.Vector2:
                    RemoveColumnInternal(ref allVector2Columns, Serializable.SerializableTypes.Vector2, removeAt);
                    break;
                case Serializable.SerializableTypes.Vector3:
                    RemoveColumnInternal(ref allVector3Columns, Serializable.SerializableTypes.Vector3, removeAt);
                    break;
                case Serializable.SerializableTypes.Vector4:
                    RemoveColumnInternal(ref allVector4Columns, Serializable.SerializableTypes.Vector4, removeAt);
                    break;
                case Serializable.SerializableTypes.Vector2Int:
                    RemoveColumnInternal(ref allVector2IntColumns, Serializable.SerializableTypes.Vector2Int, removeAt);
                    break;
                case Serializable.SerializableTypes.Vector3Int:
                    RemoveColumnInternal(ref allVector3IntColumns, Serializable.SerializableTypes.Vector3Int, removeAt);
                    break;
                case Serializable.SerializableTypes.Quaternion:
                    RemoveColumnInternal(ref allQuaternionColumns, Serializable.SerializableTypes.Quaternion, removeAt);
                    break;
                case Serializable.SerializableTypes.Rect:
                    RemoveColumnInternal(ref allRectColumns, Serializable.SerializableTypes.Rect, removeAt);
                    break;
                case Serializable.SerializableTypes.RectInt:
                    RemoveColumnInternal(ref allRectIntColumns, Serializable.SerializableTypes.RectInt, removeAt);
                    break;
                case Serializable.SerializableTypes.Color:
                    RemoveColumnInternal(ref allColorColumns, Serializable.SerializableTypes.Color, removeAt);
                    break;
                case Serializable.SerializableTypes.LayerMask:
                    RemoveColumnInternal(ref allLayerMaskColumns, Serializable.SerializableTypes.LayerMask, removeAt);
                    break;
                case Serializable.SerializableTypes.Bounds:
                    RemoveColumnInternal(ref allBoundsColumns, Serializable.SerializableTypes.Bounds, removeAt);
                    break;
                case Serializable.SerializableTypes.BoundsInt:
                    RemoveColumnInternal(ref allBoundsIntColumns, Serializable.SerializableTypes.BoundsInt, removeAt);
                    break;
                case Serializable.SerializableTypes.Hash128:
                    RemoveColumnInternal(ref allHash128Columns, Serializable.SerializableTypes.Hash128, removeAt);
                    break;
                case Serializable.SerializableTypes.Gradient:
                    RemoveColumnInternal(ref allGradientColumns, Serializable.SerializableTypes.Gradient, removeAt);
                    break;
                case Serializable.SerializableTypes.AnimationCurve:
                    RemoveColumnInternal(ref allAnimationCurveColumns, Serializable.SerializableTypes.AnimationCurve, removeAt);
                    break;
                case Serializable.SerializableTypes.Object:
                    RemoveColumnInternal(ref allObjectRefColumns, Serializable.SerializableTypes.Object, removeAt);
                    break;
            }
        }




        // Set

        public ulong SetString(int row, int column, string value)
        {
            return SetCell(row, column, ref allStringColumns, value);
        }

        public ulong SetBool(int row, int column, bool value)
        {
            return SetCell(row, column, ref allBoolColumns, value);
        }

        public ulong SetChar(int row, int column, char value)
        {
            return SetCell(row, column, ref allCharColumns, value);
        }

        public ulong SetSByte(int row, int column, sbyte value)
        {
            return SetCell(row, column, ref allSbyteColumns, value);
        }

        public ulong SetByte(int row, int column, byte value)
        {
            return SetCell(row, column, ref allByteColumns, value);
        }

        public ulong SetShort(int row, int column, short value)
        {
            return SetCell(row, column, ref allShortColumns, value);
        }

        public ulong SetUShort(int row, int column, ushort value)
        {
            return SetCell(row, column, ref allUshortColumns, value);
        }

        public ulong SetInt(int row, int column, int value)
        {
            return SetCell(row, column, ref allIntColumns, value);
        }

        public ulong SetUInt(int row, int column, uint value)
        {
            return SetCell(row, column, ref allUintColumns, value);
        }

        public ulong SetLong(int row, int column, long value)
        {
            return SetCell(row, column, ref allLongColumns, value);
        }

        public ulong SetULong(int row, int column, ulong value)
        {
            return SetCell(row, column, ref allUlongColumns, value);
        }

        public ulong SetFloat(int row, int column, float value)
        {
            return SetCell(row, column, ref allFloatColumns, value);
        }

        public ulong SetDouble(int row, int column, double value)
        {
            return SetCell(row, column, ref allDoubleColumns, value);
        }

        public ulong SetVector2(int row, int column, Vector2 value)
        {
            return SetCell(row, column, ref allVector2Columns, value);
        }

        public ulong SetVector3(int row, int column, Vector3 value)
        {
            return SetCell(row, column, ref allVector3Columns, value);
        }

        public ulong SetVector4(int row, int column, Vector4 value)
        {
            return SetCell(row, column, ref allVector4Columns, value);
        }

        public ulong SetVector2Int(int row, int column, Vector2Int value)
        {
            return SetCell(row, column, ref allVector2IntColumns, value);
        }

        public ulong SetVector3Int(int row, int column, Vector3Int value)
        {
            return SetCell(row, column, ref allVector3IntColumns, value);
        }

        public ulong SetQuaternion(int row, int column, Quaternion value)
        {
            return SetCell(row, column, ref allQuaternionColumns, value);
        }

        public ulong SetRect(int row, int column, Rect value)
        {
            return SetCell(row, column, ref allRectColumns, value);
        }

        public ulong SetRectInt(int row, int column, RectInt value)
        {
            return SetCell(row, column, ref allRectIntColumns, value);
        }

        public ulong SetColor(int row, int column, Color value)
        {
            return SetCell(row, column, ref allColorColumns, value);
        }

        public ulong SetLayerMask(int row, int column, LayerMask value)
        {
            return SetCell(row, column, ref allLayerMaskColumns, value);
        }

        public ulong SetBounds(int row, int column, Bounds value)
        {
            return SetCell(row, column, ref allBoundsColumns, value);
        }

        public ulong SetBoundsInt(int row, int column, BoundsInt value)
        {
            return SetCell(row, column, ref allBoundsIntColumns, value);
        }

        public ulong SetHash128(int row, int column, Hash128 value)
        {
            return SetCell(row, column, ref allHash128Columns, value);
        }

        public ulong SetGradient(int row, int column, Gradient value)
        {
            return SetCell(row, column, ref allGradientColumns, value);
        }

        public ulong SetAnimationCurve(int row, int column, AnimationCurve value)
        {
            return SetCell(row, column, ref allAnimationCurveColumns, value);
        }

        public ulong SetObject(int row, int column, UnityEngine.Object value)
        {
            return SetCell(row, column, ref allObjectRefColumns, value);
        }

        // Get
        public string GetString(int row, int column)
        {
            return GetCell(row, column, ref allStringColumns);
        }

        public bool GetBool(int row, int column)
        {
            return GetCell(row, column, ref allBoolColumns);
        }

        public char GetChar(int row, int column)
        {
            return GetCell(row, column, ref allCharColumns);
        }

        public sbyte GetSByte(int row, int column)
        {
            return GetCell(row, column, ref allSbyteColumns);
        }

        public byte GetByte(int row, int column)
        {
            return GetCell(row, column, ref allByteColumns);
        }

        public short GetShort(int row, int column)
        {
            return GetCell(row, column, ref allShortColumns);
        }

        public ushort GetUShort(int row, int column)
        {
            return GetCell(row, column, ref allUshortColumns);
        }

        public int GetInt(int row, int column)
        {
            return GetCell(row, column, ref allIntColumns);
        }

        public uint GetUInt(int row, int column)
        {
            return GetCell(row, column, ref allUintColumns);
        }

        public long GetLong(int row, int column)
        {
            return GetCell(row, column, ref allLongColumns);
        }

        public ulong GetULong(int row, int column)
        {
            return GetCell(row, column, ref allUlongColumns);
        }

        public float GetFloat(int row, int column)
        {
            return GetCell(row, column, ref allFloatColumns);
        }

        public double GetDouble(int row, int column)
        {
            return GetCell(row, column, ref allDoubleColumns);
        }

        public Vector2 GetVector2(int row, int column)
        {
            return GetCell(row, column, ref allVector2Columns);
        }

        public Vector3 GetVector3(int row, int column)
        {
            return GetCell(row, column, ref allVector3Columns);
        }

        public Vector4 GetVector4(int row, int column)
        {
            return GetCell(row, column, ref allVector4Columns);
        }

        public Vector2Int GetVector2Int(int row, int column)
        {
            return GetCell(row, column, ref allVector2IntColumns);
        }

        public Vector3Int GetVector3Int(int row, int column)
        {
            return GetCell(row, column, ref allVector3IntColumns);
        }

        public Quaternion GetQuaternion(int row, int column)
        {
            return GetCell(row, column, ref allQuaternionColumns);
        }

        public Rect GetRect(int row, int column)
        {
            return GetCell(row, column, ref allRectColumns);
        }

        public RectInt GetRectInt(int row, int column)
        {
            return GetCell(row, column, ref allRectIntColumns);
        }

        public Color GetColor(int row, int column)
        {
            return GetCell(row, column, ref allColorColumns);
        }

        public LayerMask GetLayerMask(int row, int column)
        {
            return GetCell(row, column, ref allLayerMaskColumns);
        }

        public Bounds GetBounds(int row, int column)
        {
            return GetCell(row, column, ref allBoundsColumns);
        }

        public BoundsInt GetBoundsInt(int row, int column)
        {
            return GetCell(row, column, ref allBoundsIntColumns);
        }

        public Hash128 GetHash128(int row, int column)
        {
            return GetCell(row, column, ref allHash128Columns);
        }

        public Gradient GetGradient(int row, int column)
        {
            return GetCell(row, column, ref allGradientColumns);
        }

        public AnimationCurve GetAnimationCurve(int row, int column)
        {
            return GetCell(row, column, ref allAnimationCurveColumns);
        }

        public UnityEngine.Object GetObject(int row, int column)
        {
            return GetCell(row, column, ref allObjectRefColumns);
        }

        // Get ref

        public ref string GetStringRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref allStringColumns);
        }

        public ref bool GetBoolRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref allBoolColumns);
        }

        public ref char GetCharRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref allCharColumns);
        }

        public ref sbyte GetSbyteRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref allSbyteColumns);
        }

        public ref byte GetByteRef(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allByteColumns);
        }

        public ref short GetShortRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref allShortColumns);
        }

        public ref ushort GetUshortRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref allUshortColumns);
        }

        public ref int GetIntRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref allIntColumns);
        }

        public ref uint GetUintRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref allUintColumns);
        }

        public ref long GetLongRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref allLongColumns);
        }

        public ref ulong GetUlongRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref allUlongColumns);
        }

        public ref float GetFloatRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref allFloatColumns);
        }

        public ref double GetDoubleRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref allDoubleColumns);
        }

        public ref Vector2 GetVector2Ref(int row, int column)
        {
            return ref GetCellRef(row, column, ref allVector2Columns);
        }

        public ref Vector3 GetVector3Ref(int row, int column)
        {
            return ref GetCellRef(row, column, ref allVector3Columns);
        }

        public ref Vector4 GetVector4Ref(int row, int column)
        {
            return ref GetCellRef(row, column, ref allVector4Columns);
        }

        public ref Vector2Int GetVector2IntRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref allVector2IntColumns);
        }

        public ref Vector3Int GetVector3IntRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref allVector3IntColumns);
        }

        public ref Quaternion GetQuaternionRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref allQuaternionColumns);
        }

        public ref Rect GetRectRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref allRectColumns);
        }

        public ref RectInt GetRectIntRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref allRectIntColumns);
        }

        public ref Color GetColorRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref allColorColumns);
        }

        public ref LayerMask GetLayerMaskRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref allLayerMaskColumns);
        }

        public ref Bounds GetBoundsRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref allBoundsColumns);
        }

        public ref BoundsInt GetBoundsIntRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref allBoundsIntColumns);
        }

        public ref Hash128 GetHash128Ref(int row, int column)
        {
            return ref GetCellRef(row, column, ref allHash128Columns);
        }

        public ref Gradient GetGradientRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref allGradientColumns);
        }

        public ref AnimationCurve GetAnimationCurveRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref allAnimationCurveColumns);
        }

        public ref UnityEngine.Object GetObjectRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref allObjectRefColumns);
        }

        // Get Column

        public string[] GetStringColumn(int column)
        {
            return GetColumn(column, ref allStringColumns);
        }

        public bool[] GetBoolColumn(int column)
        {
            return GetColumn(column, ref allBoolColumns);
        }

        public char[] GetCharColumn(int column)
        {
            return GetColumn(column, ref allCharColumns);
        }

        public sbyte[] GetSbyteColumn(int column)
        {
            return GetColumn(column, ref allSbyteColumns);
        }

        public byte[] GetByteColumn(int column)
        {
            return GetColumn(column, ref allByteColumns);
        }

        public short[] GetShortColumn(int column)
        {
            return GetColumn(column, ref allShortColumns);
        }

        public ushort[] GetUshortColumn(int column)
        {
            return GetColumn(column, ref allUshortColumns);
        }

        public int[] GetIntColumn(int column)
        {
            return GetColumn(column, ref allIntColumns);
        }

        public uint[] GetUintColumn(int column)
        {
            return GetColumn(column, ref allUintColumns);
        }

        public long[] GetLongColumn(int column)
        {
            return GetColumn(column, ref allLongColumns);
        }

        public ulong[] GetUlongColumn(int column)
        {
            return GetColumn(column, ref allUlongColumns);
        }

        public float[] GetFloatColumn(int column)
        {
            return GetColumn(column, ref allFloatColumns);
        }

        public double[] GetDoubleColumn(int column)
        {
            return GetColumn(column, ref allDoubleColumns);
        }

        public Vector2[] GetVector2Column(int column)
        {
            return GetColumn(column, ref allVector2Columns);
        }

        public Vector3[] GetVector3Column(int column)
        {
            return GetColumn(column, ref allVector3Columns);
        }

        public Vector4[] GetVector4Column(int column)
        {
            return GetColumn(column, ref allVector4Columns);
        }

        public Vector2Int[] GetVector2IntColumn(int column)
        {
            return GetColumn(column, ref allVector2IntColumns);
        }

        public Vector3Int[] GetVector3IntColumn(int column)
        {
            return GetColumn(column, ref allVector3IntColumns);
        }

        public Quaternion[] GetQuaternionColumn(int column)
        {
            return GetColumn(column, ref allQuaternionColumns);
        }

        public Rect[] GetRectColumn(int column)
        {
            return GetColumn(column, ref allRectColumns);
        }

        public RectInt[] GetRectIntColumn(int column)
        {
            return GetColumn(column, ref allRectIntColumns);
        }

        public Color[] GetColorColumn(int column)
        {
            return GetColumn(column, ref allColorColumns);
        }

        public LayerMask[] GetLayerMaskColumn(int column)
        {
            return GetColumn(column, ref allLayerMaskColumns);
        }

        public Bounds[] GetBoundsColumn(int column)
        {
            return GetColumn(column, ref allBoundsColumns);
        }

        public BoundsInt[] GetBoundsIntColumn(int column)
        {
            return GetColumn(column, ref allBoundsIntColumns);
        }

        public Hash128[] GetHash128Column(int column)
        {
            return GetColumn(column, ref allHash128Columns);
        }

        public Gradient[] GetGradientColumn(int column)
        {
            return GetColumn(column, ref allGradientColumns);
        }

        public AnimationCurve[] GetAnimationCurveColumn(int column)
        {
            return GetColumn(column, ref allAnimationCurveColumns);
        }

        public UnityEngine.Object[] GetObjectColumn(int column)
        {
            return GetColumn(column, ref allObjectRefColumns);
        }

        // Internal

        internal int AddColumnInternal<T>(string columnName, ref ArrayHolder<T>[] allColumnsOfType, Serializable.SerializableTypes typeIndex, int insertAt)
        {
            int columnCount = allColumnsOfType?.Length ?? 0;
            Array.Resize(ref allColumnsOfType, columnCount + 1);
            allColumnsOfType[columnCount].TArray = new T[rowCount];

            string[] columnNamesForType = allColumnNames[(int)typeIndex].TArray;
            int columnNamesCount = columnNamesForType?.Length ?? 0;
            Array.Resize(ref columnNamesForType, columnNamesCount + 1);
            columnNamesForType[columnNamesCount] = columnName;
            allColumnNames[(int)typeIndex].TArray = columnNamesForType;

            int columnIndex = columnEntriesFreeListHead;
            int columnIDToDenseIndexMapLength = columnIDToDenseIndexMap?.Length ?? 0;
            if (columnIndex >= columnIDToDenseIndexMapLength)
            {
                int newSize = columnIndex * 2;
                newSize = newSize == 0 ? 1 : newSize;
                Array.Resize(ref columnIDToDenseIndexMap, newSize);
                for (int i = 0; i < columnIndex; i++)
                {
                    ref ColumnEntry entry = ref columnIDToDenseIndexMap[columnIndex + i];
                    entry.columnDenseIndex = columnIndex + i + 1;
                    entry.ColumnType = Serializable.SerializableTypes.Invalid;
                }
            }

            ref int[] denseIndexToIDMap = ref columnDenseIndexToIDMap[(int)typeIndex].TArray;
            int denseIndexToIDMapLength = denseIndexToIDMap?.Length ?? 0;
            Array.Resize(ref denseIndexToIDMap, denseIndexToIDMapLength + 1);
            denseIndexToIDMap[denseIndexToIDMapLength] = columnIndex;

            ref ColumnEntry newEntry = ref columnIDToDenseIndexMap[columnIndex];
            newEntry.columnDenseIndex = denseIndexToIDMapLength;
            newEntry.ColumnType = typeIndex;

            insertAt = insertAt < 0 ? combinedColumnCount : insertAt;
            ref int[] columnOrdersOfType = ref allColumnOrders[(int)typeIndex].TArray;
            int columnOrdersOfTypeLength = columnOrdersOfType?.Length ?? 0;
            Array.Resize(ref columnOrdersOfType, columnOrdersOfTypeLength + 1);
            columnOrdersOfType[columnOrdersOfTypeLength] = insertAt;

            for (int i = 0; i < Serializable.SerializableTypesCount; i++)
            {
                int[] columnOrdersOfTypeCurrent = allColumnOrders[i].TArray;
                int columnOrdersLength = columnOrdersOfTypeCurrent?.Length ?? 0;
                for (int j = 0; j < columnOrdersLength; j++)
                {
                    int columnOrderCurrent = columnOrdersOfTypeCurrent[j];

                    if (columnOrderCurrent > insertAt)
                    {
                        columnOrdersOfType[j] = columnOrderCurrent + 1;
                    }
                }
            }

            ++combinedColumnCount;
            dataVersion++;

            return columnIndex;
        }

        internal void RemoveColumnInternal<T>(ref ArrayHolder<T>[] allColumnsOfType, Serializable.SerializableTypes typeIndex, int column)
        {
            int columnLocation = columnIDToDenseIndexMap[column].columnDenseIndex;

            int lastIndex = allColumnsOfType.Length - 1;
            allColumnsOfType[columnLocation] = allColumnsOfType[lastIndex];
           // T[][] newColumnArray = new T[lastIndex][];
            ArrayHolder<T>[] newColumnArray = new ArrayHolder<T>[lastIndex];
            Array.Copy(allColumnsOfType, 0, newColumnArray, 0, lastIndex);
            allColumnsOfType = newColumnArray;

            string[] columnNamesOfType = allColumnNames[(int)typeIndex].TArray;
            columnNamesOfType[columnLocation] = columnNamesOfType[lastIndex];
            string[] newColumnNamesOfType = new string[lastIndex];
            Array.Copy(columnNamesOfType, 0, newColumnNamesOfType, 0, lastIndex);
            allColumnNames[(int)typeIndex].TArray = newColumnNamesOfType;

            int[] columnOrdersOfType = allColumnOrders[(int)typeIndex].TArray;
            int columnOrder = columnOrdersOfType[columnLocation];
            columnOrdersOfType[columnLocation] = columnOrdersOfType[lastIndex];
            int[] newColumnOrdersOfType = new int[lastIndex];
            Array.Copy(columnOrdersOfType, 0, newColumnOrdersOfType, 0, lastIndex);
            allColumnOrders[(int)typeIndex].TArray = newColumnOrdersOfType;

            int[] denseIndicesOfType = columnDenseIndexToIDMap[(int)typeIndex].TArray;
            int sparseIndexAt = denseIndicesOfType[columnLocation];
            int sparseIndexToSwap = columnOrdersOfType[lastIndex];
            ref ColumnEntry sparseIndexToFree = ref columnIDToDenseIndexMap[sparseIndexAt];
            sparseIndexToFree.ColumnType = Serializable.SerializableTypes.Invalid;
            sparseIndexToFree.columnDenseIndex = columnEntriesFreeListHead;
            columnEntriesFreeListHead = sparseIndexAt;
            columnIDToDenseIndexMap[sparseIndexToSwap].columnDenseIndex = columnLocation;
            denseIndicesOfType[columnLocation] = sparseIndexToSwap;
            int[] newDenseIndicesOfType = new int[lastIndex];
            Array.Copy(denseIndicesOfType, 0, newDenseIndicesOfType, 0, lastIndex);
            columnDenseIndexToIDMap[(int)typeIndex].TArray = newDenseIndicesOfType;

            for (int i = 0; i < Serializable.SerializableTypesCount; i++)
            {
                int[] columnOrdersOfTypeCurrent = allColumnOrders[i].TArray;

                int columnOrdersLength = columnOrdersOfTypeCurrent.Length;
                for (int j = 0; j < columnOrdersLength; j++)
                {
                    int columnOrderCurrent = columnOrdersOfTypeCurrent[j];

                    if (columnOrderCurrent > columnOrder)
                    {
                        columnOrdersOfType[j] = columnOrderCurrent - 1;
                    }
                }
            }

            --combinedColumnCount;
            dataVersion++;
        }

        internal void InsertRowsOfTypeInternal<T>(ref ArrayHolder<T>[] allColumnsOfType, int insertAt, int numberOfNewRows)
        {
            int columnCount = allColumnsOfType?.Length ?? 0;
            for (int i = 0; i < columnCount; i++)
            {
                ref T[] column = ref allColumnsOfType[i].TArray;
                int newRowCount = rowCount + numberOfNewRows;
                Array.Resize(ref column, newRowCount);
                for (int j = newRowCount - 1; j > insertAt + numberOfNewRows - 1; j--)
                {
                    column[j] = column[j - numberOfNewRows];
                }

                for (int j = 0; j < numberOfNewRows; j++)
                {
                    column[insertAt + j] = default;
                }
            }
        }

        internal void DeleteRowsOfTypeInternal<T>(ref ArrayHolder<T>[] allColumnsOfType, int removeAt, int numberOfRowsToDelete)
        {
            int columnCount = allColumnsOfType?.Length ?? 0;

            for (int i = 0; i < columnCount; i++)
            {
                ref T[] column = ref allColumnsOfType[i].TArray;
                int newRowCount = rowCount - numberOfRowsToDelete;

                for (int j = removeAt; j < rowCount - numberOfRowsToDelete; j++)
                {
                    column[j] = column[j + numberOfRowsToDelete];
                }

                Array.Resize(ref column, newRowCount);
            }
        }

        internal ref T GetCellRef<T>(int rowID, int columnID, ref ArrayHolder<T>[] allColumnsOfType)
        {
            int column = columnIDToDenseIndexMap[columnID].columnDenseIndex;
            return ref allColumnsOfType[column][rowID];
        }

        internal T GetCell<T>(int rowID, int columnID, ref ArrayHolder<T>[] allColumnsOfType)
        {
            int column = columnIDToDenseIndexMap[columnID].columnDenseIndex;
            return allColumnsOfType[column][rowID];
        }

        internal ulong SetCell<T>(int rowID, int columnID, ref ArrayHolder<T>[] allColumnsOfType, T value)
        {
            int column = columnIDToDenseIndexMap[columnID].columnDenseIndex;
            allColumnsOfType[column][rowID] = value;
            dataVersion++;
            return dataVersion;
        }

        internal T[] GetColumn<T>(int columnID, ref ArrayHolder<T>[] allColumnsOfType)
        {
            int column = columnIDToDenseIndexMap[columnID].columnDenseIndex;
            return allColumnsOfType[column].TArray;
        }
    }
}