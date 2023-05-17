// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using GDX.Collections;
using UnityEngine;

namespace GDX.Data
{
    [Serializable]
    public class SimpleTable : ScriptableObject
    {
        public enum ColumnType
        {
            Invalid = -1,
            String,
            Char,
            Bool,
            Sbyte,
            Byte,
            Short,
            Ushort,
            Int,
            Uint,
            Long,
            Ulong,
            Float,
            Double,
            Vector2,
            Vector3,
            Vector4,
            Vector2Int,
            Vector3Int,
            Quaternion,
            Rect,
            RectInt,
            Color,
            LayerMask,
            Bounds,
            BoundsInt,
            Hash128,
            Gradient,
            AnimationCurve,
            Object,
            Count
        }

        [Serializable]
        public struct ColumnEntry
        {
            public string Name;
            public int Id;
            public ColumnType Type;
        }

        [Serializable]
        internal struct ColumnEntryInternal
        {
            public ColumnType columnType;
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
        [SerializeField] internal ArrayHolder<string>[] allColumnNames = new ArrayHolder<string>[(int)ColumnType.Count]; // Contains the name of each column of each type. Ordered by ColumnType
        [SerializeField] internal ArrayHolder<int>[] allColumnOrders = new ArrayHolder<int>[(int)ColumnType.Count]; // Contains the left-to-right order of each column of each type. Ordered by ColumnType


        [SerializeField]
        internal string[] allRowNames;

        [SerializeField]
        internal int rowCount;

        [SerializeField]
        internal ColumnEntryInternal[] columnIDToDenseIndexMap;

        // TODO move with other block
        [SerializeField] internal int[][] columnDenseIndexToIDMap = new int[(int)ColumnType.Count][];

        [SerializeField]
        internal int columnEntriesFreeListHead;

        [SerializeField]
        internal int combinedColumnCount;

        [SerializeField]
        internal ulong dataVersion = 1;

        // BEGIN - View Hacks
        public int ColumnCount
        {
            get
            {
                return combinedColumnCount;
            }
        }

        public int RowCount
        {
            get
            {
                return rowCount;
            }
        }

        public ColumnEntry[] GetOrderedColumns()
        {
            if (combinedColumnCount == 0) return null;

            int columnCount = (int)ColumnType.Count;
            ColumnEntry[] returnArray = new ColumnEntry[combinedColumnCount];

            for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
            {

                int[] columnOrders = allColumnOrders[columnIndex].TArray;
                int columnOrdersLength = columnOrders?.Length ?? 0;

                int[] columnIndices = columnDenseIndexToIDMap[columnIndex];
                string[] columnNames = allColumnNames[columnIndex].TArray;

                for (int i = 0; i < columnOrdersLength; i++)
                {
                    returnArray[columnOrders[i]] = new ColumnEntry
                    {
                        Name =  columnNames[i],
                        Id = columnIndices[i],
                        Type = (ColumnType)columnIndex
                    };
                }
            }
            return returnArray;
        }


        // END - View Hacks

        // TODO: Way to set column name



        public void AddRow(string rowName = null, int insertAt = -1)
        {
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

        // Add Column

        public int AddStringColumn(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allStringColumns, ColumnType.String, insertAt);
        }

        public int AddBoolColumn(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allBoolColumns, ColumnType.Bool, insertAt);
        }

        public int AddCharColumn(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allCharColumns, ColumnType.Char, insertAt);
        }

        public int AddSbyteColumn(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allSbyteColumns, ColumnType.Sbyte, insertAt);
        }

        public int AddByteColumn(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allByteColumns, ColumnType.Byte, insertAt);
        }

        public int AddShortColumn(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allShortColumns, ColumnType.Short, insertAt);
        }

        public int AddUshortColumn(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allUshortColumns, ColumnType.Ushort, insertAt);
        }

        public int AddIntColumn(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allIntColumns, ColumnType.Int, insertAt);
        }

        public int AddUintColumn(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allUintColumns, ColumnType.Uint, insertAt);
        }

        public int AddLongColumn(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allLongColumns, ColumnType.Long, insertAt);
        }

        public int AddUlongColumn(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allUlongColumns, ColumnType.Ulong, insertAt);
        }

        public int AddFloatColumn(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allFloatColumns, ColumnType.Float, insertAt);
        }

        public int AddDoubleColumn(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allDoubleColumns, ColumnType.Double, insertAt);
        }

        public int AddVector2Column(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allVector2Columns, ColumnType.Vector2, insertAt);
        }

        public int AddVector3Column(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allVector3Columns, ColumnType.Vector3, insertAt);
        }

        public int AddVector4Column(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allVector4Columns, ColumnType.Vector4, insertAt);
        }

        public int AddVector2IntColumn(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allVector2IntColumns, ColumnType.Vector2Int, insertAt);
        }

        public int AddVector3IntColumn(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allVector3IntColumns, ColumnType.Vector3Int, insertAt);
        }

        public int AddQuaternionColumn(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allQuaternionColumns, ColumnType.Quaternion, insertAt);
        }

        public int AddRectColumn(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allRectColumns, ColumnType.Rect, insertAt);
        }

        public int AddRectIntColumn(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allRectIntColumns, ColumnType.RectInt, insertAt);
        }

        public int AddColorColumn(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allColorColumns, ColumnType.Color, insertAt);
        }

        public int AddLayerMaskColumn(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allLayerMaskColumns, ColumnType.LayerMask, insertAt);
        }

        public int AddBoundsColumn(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allBoundsColumns, ColumnType.Bounds, insertAt);
        }

        public int AddBoundsIntColumn(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allBoundsIntColumns, ColumnType.BoundsInt, insertAt);
        }

        public int AddHash128Column(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allHash128Columns, ColumnType.Hash128, insertAt);
        }

        public int AddGradientColumn(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allGradientColumns, ColumnType.Gradient, insertAt);
        }

        public int AddAnimationCurveColumn(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allAnimationCurveColumns, ColumnType.AnimationCurve, insertAt);
        }

        public int AddObjectColumn(string columnName, int insertAt = -1)
        {
            return AddColumnInternal(columnName, ref allObjectRefColumns, ColumnType.Object, insertAt);
        }

        // Remove Column

        public void RemoveStringColumn(int removeAt)
        {
            RemoveColumnInternal(ref allStringColumns, ColumnType.String, removeAt);
        }

        public void RemoveBoolColumn(int removeAt)
        {
            RemoveColumnInternal(ref allBoolColumns, ColumnType.Bool, removeAt);
        }

        public void RemoveCharColumn(int removeAt)
        {
            RemoveColumnInternal(ref allCharColumns, ColumnType.Char, removeAt);
        }

        public void RemoveSbyteColumn(int removeAt)
        {
            RemoveColumnInternal(ref allSbyteColumns, ColumnType.Sbyte, removeAt);
        }

        public void RemoveByteColumn(int removeAt)
        {
            RemoveColumnInternal(ref allByteColumns, ColumnType.Byte, removeAt);
        }

        public void RemoveShortColumn(int removeAt)
        {
            RemoveColumnInternal(ref allShortColumns, ColumnType.Short, removeAt);
        }

        public void RemoveUshortColumn(int removeAt)
        {
            RemoveColumnInternal(ref allUshortColumns, ColumnType.Ushort, removeAt);
        }

        public void RemoveIntColumn(int removeAt)
        {
            RemoveColumnInternal(ref allIntColumns, ColumnType.Int, removeAt);
        }

        public void RemoveUintColumn(int removeAt)
        {
            RemoveColumnInternal(ref allUintColumns, ColumnType.Uint, removeAt);
        }

        public void RemoveLongColumn(int removeAt)
        {
            RemoveColumnInternal(ref allLongColumns, ColumnType.Long, removeAt);
        }

        public void RemoveUlongColumn(int removeAt)
        {
            RemoveColumnInternal(ref allUlongColumns, ColumnType.Ulong, removeAt);
        }

        public void RemoveFloatColumn(int removeAt)
        {
            RemoveColumnInternal(ref allFloatColumns, ColumnType.Float, removeAt);
        }

        public void RemoveDoubleColumn(int removeAt)
        {
            RemoveColumnInternal(ref allDoubleColumns, ColumnType.Double, removeAt);
        }

        public void RemoveVector2Column(int removeAt)
        {
            RemoveColumnInternal(ref allVector2Columns, ColumnType.Vector2, removeAt);
        }

        public void RemoveVector3Column(int removeAt)
        {
            RemoveColumnInternal(ref allVector3Columns, ColumnType.Vector3, removeAt);
        }

        public void RemoveVector4Column(int removeAt)
        {
            RemoveColumnInternal(ref allVector4Columns, ColumnType.Vector4, removeAt);
        }

        public void RemoveVector2IntColumn(int removeAt)
        {
            RemoveColumnInternal(ref allVector2IntColumns, ColumnType.Vector2Int, removeAt);
        }

        public void RemoveVector3IntColumn(int removeAt)
        {
            RemoveColumnInternal(ref allVector3IntColumns, ColumnType.Vector3Int, removeAt);
        }

        public void RemoveQuaternionColumn(int removeAt)
        {
            RemoveColumnInternal(ref allQuaternionColumns, ColumnType.Quaternion, removeAt);
        }

        public void RemoveRectColumn(int removeAt)
        {
            RemoveColumnInternal(ref allRectColumns, ColumnType.Rect, removeAt);
        }

        public void RemoveRectIntColumn(int removeAt)
        {
            RemoveColumnInternal(ref allRectIntColumns, ColumnType.RectInt, removeAt);
        }

        public void RemoveColorColumn(int removeAt)
        {
            RemoveColumnInternal(ref allColorColumns, ColumnType.Color, removeAt);
        }

        public void RemoveLayerMaskColumn(int removeAt)
        {
            RemoveColumnInternal(ref allLayerMaskColumns, ColumnType.LayerMask, removeAt);
        }

        public void RemoveBoundsColumn(int removeAt)
        {
            RemoveColumnInternal(ref allBoundsColumns, ColumnType.Bounds, removeAt);
        }

        public void RemoveBoundsIntColumn(int removeAt)
        {
            RemoveColumnInternal(ref allBoundsIntColumns, ColumnType.BoundsInt, removeAt);
        }

        public void RemoveHash128Column(int removeAt)
        {
            RemoveColumnInternal(ref allHash128Columns, ColumnType.Hash128, removeAt);
        }

        public void RemoveGradientColumn(int removeAt)
        {
            RemoveColumnInternal(ref allGradientColumns, ColumnType.Gradient, removeAt);
        }

        public void RemoveAnimationCurveColumn(int removeAt)
        {
            RemoveColumnInternal(ref allAnimationCurveColumns, ColumnType.AnimationCurve, removeAt);
        }

        public void RemoveObjectColumn(int removeAt)
        {
            RemoveColumnInternal(ref allObjectRefColumns, ColumnType.Object, removeAt);
        }

        // Set

        public void SetString(int row, int columnID, string value)
        {
            SetCell(row, columnID, ref allStringColumns, value);
        }

        public void SetBool(int row, int columnID, bool value)
        {
            SetCell(row, columnID, ref allBoolColumns, value);
        }

        public void SetChar(int row, int columnID, char value)
        {
            SetCell(row, columnID, ref allCharColumns, value);
        }

        public void SetSbyte(int row, int columnID, sbyte value)
        {
            SetCell(row, columnID, ref allSbyteColumns, value);
        }

        public void SetByte(int row, int columnID, byte value)
        {
            SetCell(row, columnID, ref allByteColumns, value);
        }

        public void SetShort(int row, int columnID, short value)
        {
            SetCell(row, columnID, ref allShortColumns, value);
        }

        public void SetUshort(int row, int columnID, ushort value)
        {
            SetCell(row, columnID, ref allUshortColumns, value);
        }

        public void SetInt(int row, int columnID, int value)
        {
            SetCell(row, columnID, ref allIntColumns, value);
        }

        public void SetUint(int row, int columnID, uint value)
        {
            SetCell(row, columnID, ref allUintColumns, value);
        }

        public void SetLong(int row, int columnID, long value)
        {
            SetCell(row, columnID, ref allLongColumns, value);
        }

        public void SetUlong(int row, int columnID, ulong value)
        {
            SetCell(row, columnID, ref allUlongColumns, value);
        }

        public void SetFloat(int row, int columnID, float value)
        {
            SetCell(row, columnID, ref allFloatColumns, value);
        }

        public void SetDouble(int row, int columnID, double value)
        {
            SetCell(row, columnID, ref allDoubleColumns, value);
        }

        public void SetVector2(int row, int columnID, Vector2 value)
        {
            SetCell(row, columnID, ref allVector2Columns, value);
        }

        public void SetVector3(int row, int columnID, Vector3 value)
        {
            SetCell(row, columnID, ref allVector3Columns, value);
        }

        public void SetVector4(int row, int columnID, Vector4 value)
        {
            SetCell(row, columnID, ref allVector4Columns, value);
        }

        public void SetVector2Int(int row, int columnID, Vector2Int value)
        {
            SetCell(row, columnID, ref allVector2IntColumns, value);
        }

        public void SetVector3Int(int row, int columnID, Vector3Int value)
        {
            SetCell(row, columnID, ref allVector3IntColumns, value);
        }

        public void SetQuaternion(int row, int columnID, Quaternion value)
        {
            SetCell(row, columnID, ref allQuaternionColumns, value);
        }

        public void SetRect(int row, int columnID, Rect value)
        {
            SetCell(row, columnID, ref allRectColumns, value);
        }

        public void SetRectInt(int row, int columnID, RectInt value)
        {
            SetCell(row, columnID, ref allRectIntColumns, value);
        }

        public void SetColor(int row, int columnID, Color value)
        {
            SetCell(row, columnID, ref allColorColumns, value);
        }

        public void SetLayerMask(int row, int columnID, LayerMask value)
        {
            SetCell(row, columnID, ref allLayerMaskColumns, value);
        }

        public void SetBounds(int row, int columnID, Bounds value)
        {
            SetCell(row, columnID, ref allBoundsColumns, value);
        }

        public void SetBoundsInt(int row, int columnID, BoundsInt value)
        {
            SetCell(row, columnID, ref allBoundsIntColumns, value);
        }

        public void SetHash128(int row, int columnID, Hash128 value)
        {
            SetCell(row, columnID, ref allHash128Columns, value);
        }

        public void SetGradient(int row, int columnID, Gradient value)
        {
            SetCell(row, columnID, ref allGradientColumns, value);
        }

        public void SetAnimationCurve(int row, int columnID, AnimationCurve value)
        {
            SetCell(row, columnID, ref allAnimationCurveColumns, value);
        }

        public void SetObject(int row, int columnID, UnityEngine.Object value)
        {
            SetCell(row, columnID, ref allObjectRefColumns, value);
        }

        // Get

        public string GetString(int row, int columnID)
        {
            return GetCell(row, columnID, ref allStringColumns);
        }

        public bool GetBool(int row, int columnID)
        {
            return GetCell(row, columnID, ref allBoolColumns);
        }

        public char GetChar(int row, int columnID)
        {
            return GetCell(row, columnID, ref allCharColumns);
        }

        public sbyte GetSbyte(int row, int columnID)
        {
            return GetCell(row, columnID, ref allSbyteColumns);
        }

        public byte GetByte(int row, int columnID)
        {
            return GetCell(row, columnID, ref allByteColumns);
        }

        public short GetShort(int row, int columnID)
        {
            return GetCell(row, columnID, ref allShortColumns);
        }

        public ushort GetUshort(int row, int columnID)
        {
            return GetCell(row, columnID, ref allUshortColumns);
        }

        public int GetInt(int row, int columnID)
        {
            return GetCell(row, columnID, ref allIntColumns);
        }

        public uint GetUint(int row, int columnID)
        {
            return GetCell(row, columnID, ref allUintColumns);
        }

        public long GetLong(int row, int columnID)
        {
            return GetCell(row, columnID, ref allLongColumns);
        }

        public ulong GetUlong(int row, int columnID)
        {
            return GetCell(row, columnID, ref allUlongColumns);
        }

        public float GetFloat(int row, int columnID)
        {
            return GetCell(row, columnID, ref allFloatColumns);
        }

        public double GetDouble(int row, int columnID)
        {
            return GetCell(row, columnID, ref allDoubleColumns);
        }

        public Vector2 GetVector2(int row, int columnID)
        {
            return GetCell(row, columnID, ref allVector2Columns);
        }

        public Vector3 GetVector3(int row, int columnID)
        {
            return GetCell(row, columnID, ref allVector3Columns);
        }

        public Vector4 GetVector4(int row, int columnID)
        {
            return GetCell(row, columnID, ref allVector4Columns);
        }

        public Vector2Int GetVector2Int(int row, int columnID)
        {
            return GetCell(row, columnID, ref allVector2IntColumns);
        }

        public Vector3Int GetVector3Int(int row, int columnID)
        {
            return GetCell(row, columnID, ref allVector3IntColumns);
        }

        public Quaternion GetQuaternion(int row, int columnID)
        {
            return GetCell(row, columnID, ref allQuaternionColumns);
        }

        public Rect GetRect(int row, int columnID)
        {
            return GetCell(row, columnID, ref allRectColumns);
        }

        public RectInt GetRectInt(int row, int columnID)
        {
            return GetCell(row, columnID, ref allRectIntColumns);
        }

        public Color GetColor(int row, int columnID)
        {
            return GetCell(row, columnID, ref allColorColumns);
        }

        public LayerMask GetLayerMask(int row, int columnID)
        {
            return GetCell(row, columnID, ref allLayerMaskColumns);
        }

        public Bounds GetBounds(int row, int columnID)
        {
            return GetCell(row, columnID, ref allBoundsColumns);
        }

        public BoundsInt GetBoundsInt(int row, int columnID)
        {
            return GetCell(row, columnID, ref allBoundsIntColumns);
        }

        public Hash128 GetHash128(int row, int columnID)
        {
            return GetCell(row, columnID, ref allHash128Columns);
        }

        public Gradient GetGradient(int row, int columnID)
        {
            return GetCell(row, columnID, ref allGradientColumns);
        }

        public AnimationCurve GetAnimationCurve(int row, int columnID)
        {
            return GetCell(row, columnID, ref allAnimationCurveColumns);
        }

        public UnityEngine.Object GetObject(int row, int columnID)
        {
            return GetCell(row, columnID, ref allObjectRefColumns);
        }

        // Get ref

        public ref string GetStringRef(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allStringColumns);
        }

        public ref bool GetBoolRef(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allBoolColumns);
        }

        public ref char GetCharRef(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allCharColumns);
        }

        public ref sbyte GetSbyteRef(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allSbyteColumns);
        }

        public ref byte GetByteRef(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allByteColumns);
        }

        public ref short GetShortRef(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allShortColumns);
        }

        public ref ushort GetUshortRef(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allUshortColumns);
        }

        public ref int GetIntRef(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allIntColumns);
        }

        public ref uint GetUintRef(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allUintColumns);
        }

        public ref long GetLongRef(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allLongColumns);
        }

        public ref ulong GetUlongRef(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allUlongColumns);
        }

        public ref float GetFloatRef(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allFloatColumns);
        }

        public ref double GetDoubleRef(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allDoubleColumns);
        }

        public ref Vector2 GetVector2Ref(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allVector2Columns);
        }

        public ref Vector3 GetVector3Ref(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allVector3Columns);
        }

        public ref Vector4 GetVector4Ref(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allVector4Columns);
        }

        public ref Vector2Int GetVector2IntRef(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allVector2IntColumns);
        }

        public ref Vector3Int GetVector3IntRef(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allVector3IntColumns);
        }

        public ref Quaternion GetQuaternionRef(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allQuaternionColumns);
        }

        public ref Rect GetRectRef(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allRectColumns);
        }

        public ref RectInt GetRectIntRef(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allRectIntColumns);
        }

        public ref Color GetColorRef(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allColorColumns);
        }

        public ref LayerMask GetLayerMaskRef(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allLayerMaskColumns);
        }

        public ref Bounds GetBoundsRef(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allBoundsColumns);
        }

        public ref BoundsInt GetBoundsIntRef(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allBoundsIntColumns);
        }

        public ref Hash128 GetHash128Ref(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allHash128Columns);
        }

        public ref Gradient GetGradientRef(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allGradientColumns);
        }

        public ref AnimationCurve GetAnimationCurveRef(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allAnimationCurveColumns);
        }

        public ref UnityEngine.Object GetObjectRef(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref allObjectRefColumns);
        }

        // Get Column

        public string[] GetStringColumn(int columnID)
        {
            return GetColumn(columnID, ref allStringColumns);
        }

        public bool[] GetBoolColumn(int columnID)
        {
            return GetColumn(columnID, ref allBoolColumns);
        }

        public char[] GetCharColumn(int columnID)
        {
            return GetColumn(columnID, ref allCharColumns);
        }

        public sbyte[] GetSbyteColumn(int columnID)
        {
            return GetColumn(columnID, ref allSbyteColumns);
        }

        public byte[] GetByteColumn(int columnID)
        {
            return GetColumn(columnID, ref allByteColumns);
        }

        public short[] GetShortColumn(int columnID)
        {
            return GetColumn(columnID, ref allShortColumns);
        }

        public ushort[] GetUshortColumn(int columnID)
        {
            return GetColumn(columnID, ref allUshortColumns);
        }

        public int[] GetIntColumn(int columnID)
        {
            return GetColumn(columnID, ref allIntColumns);
        }

        public uint[] GetUintColumn(int columnID)
        {
            return GetColumn(columnID, ref allUintColumns);
        }

        public long[] GetLongColumn(int columnID)
        {
            return GetColumn(columnID, ref allLongColumns);
        }

        public ulong[] GetUlongColumn(int columnID)
        {
            return GetColumn(columnID, ref allUlongColumns);
        }

        public float[] GetFloatColumn(int columnID)
        {
            return GetColumn(columnID, ref allFloatColumns);
        }

        public double[] GetDoubleColumn(int columnID)
        {
            return GetColumn(columnID, ref allDoubleColumns);
        }

        public Vector2[] GetVector2Column(int columnID)
        {
            return GetColumn(columnID, ref allVector2Columns);
        }

        public Vector3[] GetVector3Column(int columnID)
        {
            return GetColumn(columnID, ref allVector3Columns);
        }

        public Vector4[] GetVector4Column(int columnID)
        {
            return GetColumn(columnID, ref allVector4Columns);
        }

        public Vector2Int[] GetVector2IntColumn(int columnID)
        {
            return GetColumn(columnID, ref allVector2IntColumns);
        }

        public Vector3Int[] GetVector3IntColumn(int columnID)
        {
            return GetColumn(columnID, ref allVector3IntColumns);
        }

        public Quaternion[] GetQuaternionColumn(int columnID)
        {
            return GetColumn(columnID, ref allQuaternionColumns);
        }

        public Rect[] GetRectColumn(int columnID)
        {
            return GetColumn(columnID, ref allRectColumns);
        }

        public RectInt[] GetRectIntColumn(int columnID)
        {
            return GetColumn(columnID, ref allRectIntColumns);
        }

        public Color[] GetColorColumn(int columnID)
        {
            return GetColumn(columnID, ref allColorColumns);
        }

        public LayerMask[] GetLayerMaskColumn(int columnID)
        {
            return GetColumn(columnID, ref allLayerMaskColumns);
        }

        public Bounds[] GetBoundsColumn(int columnID)
        {
            return GetColumn(columnID, ref allBoundsColumns);
        }

        public BoundsInt[] GetBoundsIntColumn(int columnID)
        {
            return GetColumn(columnID, ref allBoundsIntColumns);
        }

        public Hash128[] GetHash128Column(int columnID)
        {
            return GetColumn(columnID, ref allHash128Columns);
        }

        public Gradient[] GetGradientColumn(int columnID)
        {
            return GetColumn(columnID, ref allGradientColumns);
        }

        public AnimationCurve[] GetAnimationCurveColumn(int columnID)
        {
            return GetColumn(columnID, ref allAnimationCurveColumns);
        }

        public UnityEngine.Object[] GetObjectColumn(int columnID)
        {
            return GetColumn(columnID, ref allObjectRefColumns);
        }

        // Internal

        internal int AddColumnInternal<T>(string columnName, ref ArrayHolder<T>[] allColumnsOfType, ColumnType typeIndex, int insertAt)
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
                    ref ColumnEntryInternal entry = ref columnIDToDenseIndexMap[columnIndex + i];
                    entry.columnDenseIndex = columnIndex + i + 1;
                    entry.columnType = ColumnType.Invalid;
                }
            }

            ref int[] denseIndexToIDMap = ref columnDenseIndexToIDMap[(int)typeIndex];
            int denseIndexToIDMapLength = denseIndexToIDMap?.Length ?? 0;
            Array.Resize(ref denseIndexToIDMap, denseIndexToIDMapLength + 1);
            denseIndexToIDMap[denseIndexToIDMapLength] = columnIndex;

            ref ColumnEntryInternal newEntryInternal = ref columnIDToDenseIndexMap[columnIndex];
            newEntryInternal.columnDenseIndex = denseIndexToIDMapLength;
            newEntryInternal.columnType = typeIndex;

            insertAt = insertAt < 0 ? combinedColumnCount : insertAt;
            ref int[] columnOrdersOfType = ref allColumnOrders[(int)typeIndex].TArray;
            int columnOrdersOfTypeLength = columnOrdersOfType?.Length ?? 0;
            Array.Resize(ref columnOrdersOfType, columnOrdersOfTypeLength + 1);
            columnOrdersOfType[columnOrdersOfTypeLength] = insertAt;

            for (int i = 0; i < (int)ColumnType.Count; i++)
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

        internal void RemoveColumnInternal<T>(ref ArrayHolder<T>[] allColumnsOfType, ColumnType typeIndex, int columnID)
        {
            int columnLocation = columnIDToDenseIndexMap[columnID].columnDenseIndex;

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

            int[] denseIndicesOfType = columnDenseIndexToIDMap[(int)typeIndex];
            int sparseIndexAt = denseIndicesOfType[columnLocation];
            int sparseIndexToSwap = columnOrdersOfType[lastIndex];
            ref ColumnEntryInternal sparseIndexToFree = ref columnIDToDenseIndexMap[sparseIndexAt];
            sparseIndexToFree.columnType = ColumnType.Invalid;
            sparseIndexToFree.columnDenseIndex = columnEntriesFreeListHead;
            columnEntriesFreeListHead = sparseIndexAt;
            columnIDToDenseIndexMap[sparseIndexToSwap].columnDenseIndex = columnLocation;
            denseIndicesOfType[columnLocation] = sparseIndexToSwap;
            int[] newDenseIndicesOfType = new int[lastIndex];
            Array.Copy(denseIndicesOfType, 0, newDenseIndicesOfType, 0, lastIndex);
            columnDenseIndexToIDMap[(int)typeIndex] = newDenseIndicesOfType;

            for (int i = 0; i < (int)ColumnType.Count; i++)
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
                    column[insertAt + i] = default;
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

        internal ref T GetCellRef<T>(int row, int columnID, ref ArrayHolder<T>[] allColumnsOfType)
        {
            int column = columnIDToDenseIndexMap[columnID].columnDenseIndex;
            return ref allColumnsOfType[column][row];
        }

        internal T GetCell<T>(int row, int columnID, ref ArrayHolder<T>[] allColumnsOfType)
        {
            int column = columnIDToDenseIndexMap[columnID].columnDenseIndex;
            return allColumnsOfType[column][row];
        }

        internal void SetCell<T>(int row, int columnID, ref ArrayHolder<T>[] allColumnsOfType, T value)
        {
            int column = columnIDToDenseIndexMap[columnID].columnDenseIndex;
            allColumnsOfType[column][row] = value;
            dataVersion++;
        }

        internal T[] GetColumn<T>(int columnID, ref ArrayHolder<T>[] allColumnsOfType)
        {
            int column = columnIDToDenseIndexMap[columnID].columnDenseIndex;
            return allColumnsOfType[column].TArray;
        }
    }
}