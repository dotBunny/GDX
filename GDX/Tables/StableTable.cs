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

        [SerializeField] internal string m_DisplayName = "GDXStableTable";

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

        [SerializeField] internal int[] rowIDToDenseIndexMap;
        [SerializeField] internal int[] rowDenseIndexToIDMap;
        [SerializeField] internal string[] rowNames;
        [SerializeField] internal int rowEntriesFreeListHead;

        [SerializeField]
        internal int rowCount;

        [SerializeField] internal ColumnEntry[] columnIDToDenseIndexMap;
        [SerializeField] internal int[] columnIDToSortOrderMap;
        [SerializeField] internal int[] sortedOrderToColumnIDMap;

        // TODO move with other block
        [SerializeField] ArrayHolder<int>[] columnDenseIndexToIDMap = new ArrayHolder<int>[Serializable.SerializableTypesCount];

        [SerializeField]
        internal int columnEntriesFreeListHead;

        [SerializeField]
        internal int combinedColumnCount;

        [SerializeField]
        internal ulong dataVersion = 1;

        [SerializeField] BitArray8 m_Flags;

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

        public string GetDisplayName()
        {
            return m_DisplayName;
        }

        public void SetDisplayName(string displayName)
        {
            m_DisplayName = displayName;
        }

        public bool GetFlag(byte index)
        {
            return m_Flags[index];
        }

        public void SetFlag(byte index, bool toggle)
        {
            m_Flags[index] = toggle;
        }

        public ITable.RowDescription[] GetAllRowDescriptions()
        {
            if (combinedColumnCount == 0 || rowCount == 0) return null;
            ITable.RowDescription[] returnArray = new ITable.RowDescription[rowCount];
            string empty = string.Empty;
            for (int i = 0; i < rowCount; i++)
            {
                returnArray[i].Index = rowDenseIndexToIDMap[i];
                returnArray[i].Name = rowNames[i];
            }

            return returnArray;
        }
        public ITable.RowDescription GetRowDescription(string name)
        {
            for (int i = 0; i < rowCount; i++)
            {
                string nameAt = rowNames[i];

                if (nameAt == name)
                {
                    return new ITable.RowDescription
                    {
                        Index = rowDenseIndexToIDMap[i],
                        Name = nameAt
                    };
                }
            }

            throw new ArgumentException("Row with name " + name + " does not exist in the table");
        }

        public ITable.RowDescription GetRowDescription(int order)
        {
            return new ITable.RowDescription
            {
                Index = rowDenseIndexToIDMap[order],
                Name = rowNames[order]
            };
        }

        public ITable.ColumnDescription GetColumnDescription(string name)
        {
            for (int i = 0; i < Serializable.SerializableTypesCount; i++)
            {
                string[] columnNames = allColumnNames[i].TArray;

                if (columnNames != null)
                {
                    for (int j = 0; j < columnNames.Length; j++)
                    {
                        string nameAt = columnNames[j];

                        if (name == nameAt)
                        {
                            int columnID = columnDenseIndexToIDMap[i].TArray[j];

                            ref ColumnEntry columnEntry = ref columnIDToDenseIndexMap[columnID];
                            return new ITable.ColumnDescription
                            {
                                Index = columnID,
                                Name = nameAt,
                                Type = columnEntry.ColumnType
                            };
                        }
                    }
                }
            }

            throw new ArgumentException("Column with name " + name + " does not exist in the table");
        }

        public ITable.ColumnDescription GetColumnDescription(int order)
        {
            int idAtOrderedIndex = sortedOrderToColumnIDMap[order];
            ref ColumnEntry columnEntry = ref columnIDToDenseIndexMap[idAtOrderedIndex];

            string columnName = allColumnNames[(int)columnEntry.ColumnType][columnEntry.columnDenseIndex];

            return new ITable.ColumnDescription
            {
                Index = idAtOrderedIndex,
                Name = columnName,
                Type = columnEntry.ColumnType
            };
        }

        /// <inheritdoc />
        public ITable.ColumnDescription[] GetAllColumnDescriptions()
        {
            if (combinedColumnCount == 0) return null;
            ITable.ColumnDescription[] returnArray = new ITable.ColumnDescription[combinedColumnCount];

            for (int i = 0; i < combinedColumnCount; i++)
            {
                int columnID = sortedOrderToColumnIDMap[i];
                Serializable.SerializableTypes columnType = columnIDToDenseIndexMap[columnID].ColumnType;
                string name = allColumnNames[(int)columnType][columnIDToDenseIndexMap[columnID].columnDenseIndex];

                returnArray[i] = new ITable.ColumnDescription
                {
                    Name = name,
                    Index = columnID,
                    Type = columnType
                };
            }

            return returnArray;
        }

        public void SetColumnName(string columnName, int column)
        {
            ref ColumnEntry columnEntry = ref columnIDToDenseIndexMap[column];
            allColumnNames[(int)columnEntry.ColumnType][columnEntry.columnDenseIndex] = columnName;
        }

        public string GetColumnName(int column)
        {
            ref ColumnEntry columnEntry = ref columnIDToDenseIndexMap[column];
            return allColumnNames[(int)columnEntry.ColumnType][columnEntry.columnDenseIndex];
        }

        public void SetRowName(string rowName, int row)
        {
            int rowDenseIndex = rowIDToDenseIndexMap[row];
            rowNames[rowDenseIndex] = rowName;
        }

        public string GetRowName(int row)
        {
            int rowDenseIndex = rowIDToDenseIndexMap[row];
            return rowNames[rowDenseIndex];
        }

        public ref string GetRowNameRef(int row)
        {
            int rowDenseIndex = rowIDToDenseIndexMap[row];
            return ref rowNames[rowDenseIndex];
        }

        public ref string GetColumnNameRef(int columnID)
        {
            ref ColumnEntry columnEntry = ref columnIDToDenseIndexMap[columnID];
            return ref allColumnNames[(int)columnEntry.ColumnType][columnEntry.columnDenseIndex];
        }


        public int AddRow(string rowName = null, int insertAtRowID = -1)
        {
            int rowID = rowEntriesFreeListHead;
            int rowIDToDenseIndexMapLength = rowIDToDenseIndexMap?.Length ?? 0;
            if (rowID >= rowIDToDenseIndexMapLength)
            {
                int newSize = rowID * 2;
                newSize = newSize == 0 ? 1 : newSize;
                Array.Resize(ref rowIDToDenseIndexMap, newSize);
                for (int i = rowID; i < newSize; i++)
                {
                    rowIDToDenseIndexMap[i] = i + 1;
                }
            }

            int denseIndexToIDMapLength = rowDenseIndexToIDMap?.Length ?? 0;
            Array.Resize(ref rowDenseIndexToIDMap, denseIndexToIDMapLength + 1);
            Array.Resize(ref rowNames, denseIndexToIDMapLength + 1);

            int insertAt = insertAtRowID < 0 ? rowCount : rowIDToDenseIndexMap[insertAtRowID];

            for (int i = denseIndexToIDMapLength; i > insertAt; i--)
            {
                int currentRowID = rowDenseIndexToIDMap[i - 1];
                rowDenseIndexToIDMap[i] = currentRowID;

                rowIDToDenseIndexMap[currentRowID] = i;

                rowNames[i] = rowNames[i - 1];
            }

            rowEntriesFreeListHead = rowIDToDenseIndexMap[rowID];
            rowIDToDenseIndexMap[rowID] = insertAt;
            rowDenseIndexToIDMap[insertAt] = rowID;
            rowNames[insertAt] = rowName == null ? rowID.ToString() : rowName;

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

            return rowID;
        }

        public void AddRows(int numberOfNewRows, string[] rowNames = null, int insertAtRowID = -1)
        {
            int rowIDToDenseIndexMapLength = rowIDToDenseIndexMap?.Length ?? 0;
            int newCount = rowCount + numberOfNewRows;
            if (newCount > rowIDToDenseIndexMapLength)
            {
                int newSize = newCount;
                --newSize;
                newSize |= newSize >> 1;
                newSize |= newSize >> 2;
                newSize |= newSize >> 4;
                newSize |= newSize >> 8;
                newSize |= newSize >> 16;
                ++newSize;

                newSize = newSize == 0 ? 1 : newSize;
                Array.Resize(ref rowIDToDenseIndexMap, newSize);
                for (int i = rowIDToDenseIndexMapLength; i < newSize; i++)
                {
                    rowIDToDenseIndexMap[i] = i + 1;
                }
            }

            int denseIndexToIDMapLength = rowDenseIndexToIDMap?.Length ?? 0;
            Array.Resize(ref rowDenseIndexToIDMap, denseIndexToIDMapLength + numberOfNewRows);
            Array.Resize(ref rowNames, denseIndexToIDMapLength + numberOfNewRows);

            int insertAt = insertAtRowID < 0 ? rowCount : rowIDToDenseIndexMap[insertAtRowID];

            for (int i = denseIndexToIDMapLength; i > insertAt + numberOfNewRows - 1; i--)
            {
                int currentRowID = rowDenseIndexToIDMap[i - numberOfNewRows];
                rowDenseIndexToIDMap[i] = currentRowID;

                rowIDToDenseIndexMap[currentRowID] = i;

                rowNames[i] = rowNames[i - numberOfNewRows];
            }

            int freeListHead = rowEntriesFreeListHead;

            for (int i = 0; i < numberOfNewRows; i++)
            {
                int rowID = freeListHead;
                freeListHead = rowIDToDenseIndexMap[rowID];
                rowIDToDenseIndexMap[rowID] = insertAt + i;
                rowDenseIndexToIDMap[insertAt + i] = rowID;
            }

            int numberOfNewRowNames = rowNames?.Length ?? 0;
            string emptyString = string.Empty;
            for (int i = 0; i < numberOfNewRowNames; i++)
            {
                string currentRowName = rowNames[i];
                int rowIDAt = rowDenseIndexToIDMap[insertAt + i];
                rowNames[insertAt + i] = currentRowName == null ? rowIDAt.ToString() : currentRowName;
            }

            for (int i = numberOfNewRowNames; i < numberOfNewRows; i++)
            {
                int rowIDAt = rowDenseIndexToIDMap[insertAt + i];
                rowNames[insertAt + i] = rowIDAt.ToString();
            }

            rowEntriesFreeListHead = freeListHead;

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

        public void AddRows(int numberOfNewRows, ref int[] rowIDs, string[] rowNames = null, int insertAtRowID = -1)
        {
            int rowIDToDenseIndexMapLength = rowIDToDenseIndexMap?.Length ?? 0;
            int newCount = rowCount + numberOfNewRows;
            if (newCount > rowIDToDenseIndexMapLength)
            {
                int newSize = newCount;
                --newSize;
                newSize |= newSize >> 1;
                newSize |= newSize >> 2;
                newSize |= newSize >> 4;
                newSize |= newSize >> 8;
                newSize |= newSize >> 16;
                ++newSize;

                newSize = newSize == 0 ? 1 : newSize;
                Array.Resize(ref rowIDToDenseIndexMap, newSize);
                for (int i = rowIDToDenseIndexMapLength; i < newSize; i++)
                {
                    rowIDToDenseIndexMap[i] = i + 1;
                }
            }

            int denseIndexToIDMapLength = rowDenseIndexToIDMap?.Length ?? 0;
            Array.Resize(ref rowDenseIndexToIDMap, denseIndexToIDMapLength + numberOfNewRows);

            int insertAt = insertAtRowID < 0 ? rowCount : rowIDToDenseIndexMap[insertAtRowID];

            for (int i = denseIndexToIDMapLength; i > insertAt + numberOfNewRows - 1; i--)
            {
                int currentRowID = rowDenseIndexToIDMap[i - numberOfNewRows];
                rowDenseIndexToIDMap[i] = currentRowID;

                rowIDToDenseIndexMap[currentRowID] = i;

                rowNames[i] = rowNames[i - numberOfNewRows];
            }

            int freeListHead = rowEntriesFreeListHead;

            for (int i = 0; i < numberOfNewRows; i++)
            {
                int rowID = freeListHead;
                freeListHead = rowIDToDenseIndexMap[rowID];
                rowIDToDenseIndexMap[rowID] = insertAt + i;
                rowDenseIndexToIDMap[insertAt + i] = rowID;
                rowIDs[i] = rowID;
            }

            int numberOfNewRowNames = rowNames?.Length ?? 0;
            for (int i = 0; i < numberOfNewRowNames; i++)
            {
                string currentRowName = rowNames[i];
                int rowIDAt = rowDenseIndexToIDMap[insertAt + i];
                rowNames[insertAt + i] = currentRowName == null ? rowIDAt.ToString() : currentRowName;
            }

            for (int i = numberOfNewRowNames; i < numberOfNewRows; i++)
            {
                int rowIDAt = rowDenseIndexToIDMap[insertAt + i];
                rowNames[insertAt + i] = rowIDAt.ToString();
            }

            rowEntriesFreeListHead = freeListHead;

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

        public void RemoveRow(int rowID)
        {
            int rowDenseIndex = rowIDToDenseIndexMap[rowID];
            for (int i = rowDenseIndex + 1; i < rowCount; i++)
            {
                int currentRowID = rowDenseIndexToIDMap[i];
                rowIDToDenseIndexMap[currentRowID] = i - 1;
                rowDenseIndexToIDMap[i - 1] = currentRowID;
                rowNames[i - 1] = rowNames[i];
            }

            rowIDToDenseIndexMap[rowID] = rowEntriesFreeListHead;
            rowEntriesFreeListHead = rowID;
            Array.Resize(ref rowDenseIndexToIDMap, rowCount - 1);
            Array.Resize(ref rowNames, rowCount - 1);

            DeleteRowsOfTypeInternal(ref allStringColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref allBoolColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref allCharColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref allSbyteColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref allByteColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref allShortColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref allUshortColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref allIntColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref allUintColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref allLongColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref allUlongColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref allFloatColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref allDoubleColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref allVector2Columns, rowID, 1);
            DeleteRowsOfTypeInternal(ref allVector3Columns, rowID, 1);
            DeleteRowsOfTypeInternal(ref allVector4Columns, rowID, 1);
            DeleteRowsOfTypeInternal(ref allVector2IntColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref allVector3IntColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref allQuaternionColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref allRectColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref allRectIntColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref allColorColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref allLayerMaskColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref allBoundsColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref allBoundsIntColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref allHash128Columns, rowID, 1);
            DeleteRowsOfTypeInternal(ref allGradientColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref allAnimationCurveColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref allObjectRefColumns, rowID, 1);

            --rowCount;
            dataVersion++;
        }

        public int AddColumn(Serializable.SerializableTypes columnType, string columnName, int insertAtColumnID = -1)
        {
            switch (columnType)
            {
                case Serializable.SerializableTypes.String:
                    return AddColumnInternal(columnName, ref allStringColumns, Serializable.SerializableTypes.String, insertAtColumnID);
                case Serializable.SerializableTypes.Char:
                    return AddColumnInternal(columnName, ref allCharColumns, Serializable.SerializableTypes.Char, insertAtColumnID);
                case Serializable.SerializableTypes.Bool:
                    return AddColumnInternal(columnName, ref allBoolColumns, Serializable.SerializableTypes.Bool, insertAtColumnID);
                case Serializable.SerializableTypes.SByte:
                    return AddColumnInternal(columnName, ref allSbyteColumns, Serializable.SerializableTypes.SByte, insertAtColumnID);
                case Serializable.SerializableTypes.Byte:
                    return AddColumnInternal(columnName, ref allByteColumns, Serializable.SerializableTypes.Byte, insertAtColumnID);
                case Serializable.SerializableTypes.Short:
                    return AddColumnInternal(columnName, ref allShortColumns, Serializable.SerializableTypes.Short, insertAtColumnID);
                case Serializable.SerializableTypes.UShort:
                    return AddColumnInternal(columnName, ref allUshortColumns, Serializable.SerializableTypes.UShort, insertAtColumnID);
                case Serializable.SerializableTypes.Int:
                    return AddColumnInternal(columnName, ref allIntColumns, Serializable.SerializableTypes.Int, insertAtColumnID);
                case Serializable.SerializableTypes.UInt:
                    return AddColumnInternal(columnName, ref allUintColumns, Serializable.SerializableTypes.UInt, insertAtColumnID);
                case Serializable.SerializableTypes.Long:
                    return AddColumnInternal(columnName, ref allLongColumns, Serializable.SerializableTypes.Long, insertAtColumnID);
                case Serializable.SerializableTypes.ULong:
                    return AddColumnInternal(columnName, ref allUlongColumns, Serializable.SerializableTypes.ULong, insertAtColumnID);
                case Serializable.SerializableTypes.Float:
                    return AddColumnInternal(columnName, ref allFloatColumns, Serializable.SerializableTypes.Float, insertAtColumnID);
                case Serializable.SerializableTypes.Double:
                    return AddColumnInternal(columnName, ref allDoubleColumns, Serializable.SerializableTypes.Double, insertAtColumnID);
                case Serializable.SerializableTypes.Vector2:
                    return AddColumnInternal(columnName, ref allVector2Columns, Serializable.SerializableTypes.Vector2, insertAtColumnID);
                case Serializable.SerializableTypes.Vector3:
                    return AddColumnInternal(columnName, ref allVector3Columns, Serializable.SerializableTypes.Vector3, insertAtColumnID);
                case Serializable.SerializableTypes.Vector4:
                    return AddColumnInternal(columnName, ref allVector4Columns, Serializable.SerializableTypes.Vector4, insertAtColumnID);
                case Serializable.SerializableTypes.Vector2Int:
                    return AddColumnInternal(columnName, ref allVector2IntColumns, Serializable.SerializableTypes.Vector2Int, insertAtColumnID);
                case Serializable.SerializableTypes.Vector3Int:
                    return AddColumnInternal(columnName, ref allVector3IntColumns, Serializable.SerializableTypes.Vector3Int, insertAtColumnID);
                case Serializable.SerializableTypes.Quaternion:
                    return AddColumnInternal(columnName, ref allQuaternionColumns, Serializable.SerializableTypes.Quaternion, insertAtColumnID);
                case Serializable.SerializableTypes.Rect:
                    return AddColumnInternal(columnName, ref allRectColumns, Serializable.SerializableTypes.Rect, insertAtColumnID);
                case Serializable.SerializableTypes.RectInt:
                    return AddColumnInternal(columnName, ref allRectIntColumns, Serializable.SerializableTypes.RectInt, insertAtColumnID);
                case Serializable.SerializableTypes.Color:
                    return AddColumnInternal(columnName, ref allColorColumns, Serializable.SerializableTypes.Color, insertAtColumnID);
                case Serializable.SerializableTypes.LayerMask:
                    return AddColumnInternal(columnName, ref allLayerMaskColumns, Serializable.SerializableTypes.LayerMask, insertAtColumnID);
                case Serializable.SerializableTypes.Bounds:
                    return AddColumnInternal(columnName, ref allBoundsColumns, Serializable.SerializableTypes.Bounds, insertAtColumnID);
                case Serializable.SerializableTypes.BoundsInt:
                    return AddColumnInternal(columnName, ref allBoundsIntColumns, Serializable.SerializableTypes.BoundsInt, insertAtColumnID);
                case Serializable.SerializableTypes.Hash128:
                    return AddColumnInternal(columnName, ref allHash128Columns, Serializable.SerializableTypes.Hash128, insertAtColumnID);
                case Serializable.SerializableTypes.Gradient:
                    return AddColumnInternal(columnName, ref allGradientColumns, Serializable.SerializableTypes.Gradient, insertAtColumnID);
                case Serializable.SerializableTypes.AnimationCurve:
                    return AddColumnInternal(columnName, ref allAnimationCurveColumns, Serializable.SerializableTypes.AnimationCurve, insertAtColumnID);
                case Serializable.SerializableTypes.Object:
                    return AddColumnInternal(columnName, ref allObjectRefColumns, Serializable.SerializableTypes.Object, insertAtColumnID);
            }
            return -1;
        }

        public void RemoveColumn(Serializable.SerializableTypes columnType, int columnID)
        {
            switch (columnType)
            {
                case Serializable.SerializableTypes.String:
                    RemoveColumnInternal(ref allStringColumns, Serializable.SerializableTypes.String, columnID);
                    break;
                case Serializable.SerializableTypes.Char:
                    RemoveColumnInternal(ref allCharColumns, Serializable.SerializableTypes.Char, columnID);
                    break;
                case Serializable.SerializableTypes.Bool:
                    RemoveColumnInternal(ref allBoolColumns, Serializable.SerializableTypes.Bool, columnID);
                    break;
                case Serializable.SerializableTypes.SByte:
                    RemoveColumnInternal(ref allSbyteColumns, Serializable.SerializableTypes.SByte, columnID);
                    break;
                case Serializable.SerializableTypes.Byte:
                    RemoveColumnInternal(ref allByteColumns, Serializable.SerializableTypes.Byte, columnID);
                    break;
                case Serializable.SerializableTypes.Short:
                    RemoveColumnInternal(ref allShortColumns, Serializable.SerializableTypes.Short, columnID);
                    break;
                case Serializable.SerializableTypes.UShort:
                    RemoveColumnInternal(ref allUshortColumns, Serializable.SerializableTypes.UShort, columnID);
                    break;
                case Serializable.SerializableTypes.Int:
                    RemoveColumnInternal(ref allIntColumns, Serializable.SerializableTypes.Int, columnID);
                    break;
                case Serializable.SerializableTypes.UInt:
                    RemoveColumnInternal(ref allUintColumns, Serializable.SerializableTypes.UInt, columnID);
                    break;
                case Serializable.SerializableTypes.Long:
                    RemoveColumnInternal(ref allLongColumns, Serializable.SerializableTypes.Long, columnID);
                    break;
                case Serializable.SerializableTypes.ULong:
                    RemoveColumnInternal(ref allUlongColumns, Serializable.SerializableTypes.ULong, columnID);
                    break;
                case Serializable.SerializableTypes.Float:
                    RemoveColumnInternal(ref allFloatColumns, Serializable.SerializableTypes.Float, columnID);
                    break;
                case Serializable.SerializableTypes.Double:
                    RemoveColumnInternal(ref allDoubleColumns, Serializable.SerializableTypes.Double, columnID);
                    break;
                case Serializable.SerializableTypes.Vector2:
                    RemoveColumnInternal(ref allVector2Columns, Serializable.SerializableTypes.Vector2, columnID);
                    break;
                case Serializable.SerializableTypes.Vector3:
                    RemoveColumnInternal(ref allVector3Columns, Serializable.SerializableTypes.Vector3, columnID);
                    break;
                case Serializable.SerializableTypes.Vector4:
                    RemoveColumnInternal(ref allVector4Columns, Serializable.SerializableTypes.Vector4, columnID);
                    break;
                case Serializable.SerializableTypes.Vector2Int:
                    RemoveColumnInternal(ref allVector2IntColumns, Serializable.SerializableTypes.Vector2Int, columnID);
                    break;
                case Serializable.SerializableTypes.Vector3Int:
                    RemoveColumnInternal(ref allVector3IntColumns, Serializable.SerializableTypes.Vector3Int, columnID);
                    break;
                case Serializable.SerializableTypes.Quaternion:
                    RemoveColumnInternal(ref allQuaternionColumns, Serializable.SerializableTypes.Quaternion, columnID);
                    break;
                case Serializable.SerializableTypes.Rect:
                    RemoveColumnInternal(ref allRectColumns, Serializable.SerializableTypes.Rect, columnID);
                    break;
                case Serializable.SerializableTypes.RectInt:
                    RemoveColumnInternal(ref allRectIntColumns, Serializable.SerializableTypes.RectInt, columnID);
                    break;
                case Serializable.SerializableTypes.Color:
                    RemoveColumnInternal(ref allColorColumns, Serializable.SerializableTypes.Color, columnID);
                    break;
                case Serializable.SerializableTypes.LayerMask:
                    RemoveColumnInternal(ref allLayerMaskColumns, Serializable.SerializableTypes.LayerMask, columnID);
                    break;
                case Serializable.SerializableTypes.Bounds:
                    RemoveColumnInternal(ref allBoundsColumns, Serializable.SerializableTypes.Bounds, columnID);
                    break;
                case Serializable.SerializableTypes.BoundsInt:
                    RemoveColumnInternal(ref allBoundsIntColumns, Serializable.SerializableTypes.BoundsInt, columnID);
                    break;
                case Serializable.SerializableTypes.Hash128:
                    RemoveColumnInternal(ref allHash128Columns, Serializable.SerializableTypes.Hash128, columnID);
                    break;
                case Serializable.SerializableTypes.Gradient:
                    RemoveColumnInternal(ref allGradientColumns, Serializable.SerializableTypes.Gradient, columnID);
                    break;
                case Serializable.SerializableTypes.AnimationCurve:
                    RemoveColumnInternal(ref allAnimationCurveColumns, Serializable.SerializableTypes.AnimationCurve, columnID);
                    break;
                case Serializable.SerializableTypes.Object:
                    RemoveColumnInternal(ref allObjectRefColumns, Serializable.SerializableTypes.Object, columnID);
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

        internal int AddColumnInternal<T>(string columnName, ref ArrayHolder<T>[] allColumnsOfType, Serializable.SerializableTypes typeIndex, int insertAtColumnID)
        {
            int columnCount = allColumnsOfType?.Length ?? 0;
            Array.Resize(ref allColumnsOfType, columnCount + 1);
            allColumnsOfType[columnCount].TArray = new T[rowCount];

            int columnID = columnEntriesFreeListHead;
            string[] columnNamesForType = allColumnNames[(int)typeIndex].TArray;
            int columnNamesCount = columnNamesForType?.Length ?? 0;
            Array.Resize(ref columnNamesForType, columnNamesCount + 1);
            columnNamesForType[columnNamesCount] = columnName == null ? columnID.ToString() : columnName;
            allColumnNames[(int)typeIndex].TArray = columnNamesForType;


            int columnIDToDenseIndexMapLength = columnIDToDenseIndexMap?.Length ?? 0;
            if (columnID >= columnIDToDenseIndexMapLength)
            {
                int newSize = columnIDToDenseIndexMapLength * 2;
                newSize = newSize == 0 ? 1 : newSize;
                Array.Resize(ref columnIDToDenseIndexMap, newSize);
                for (int i = columnIDToDenseIndexMapLength; i < newSize; i++)
                {
                    ref ColumnEntry entry = ref columnIDToDenseIndexMap[i];
                    entry.columnDenseIndex = i + 1;
                    entry.ColumnType = Serializable.SerializableTypes.Invalid;
                }

                Array.Resize(ref columnIDToSortOrderMap, newSize);
                for (int i = columnIDToDenseIndexMapLength; i < newSize; i++)
                {
                    columnIDToSortOrderMap[i] = -1;
                }
            }

            columnEntriesFreeListHead = columnIDToDenseIndexMap[columnID].columnDenseIndex;

            ref int[] denseIndexToIDMap = ref columnDenseIndexToIDMap[(int)typeIndex].TArray;
            int denseIndexToIDMapLength = denseIndexToIDMap?.Length ?? 0;
            Array.Resize(ref denseIndexToIDMap, denseIndexToIDMapLength + 1);
            denseIndexToIDMap[denseIndexToIDMapLength] = columnID;

            ref ColumnEntry newEntry = ref columnIDToDenseIndexMap[columnID];
            newEntry.columnDenseIndex = denseIndexToIDMapLength;
            newEntry.ColumnType = typeIndex;

            int insertAtSortedIndex = insertAtColumnID < 0 ? combinedColumnCount : columnIDToSortOrderMap[insertAtColumnID];
            Array.Resize(ref sortedOrderToColumnIDMap, combinedColumnCount + 1);
            for (int i = combinedColumnCount; i > insertAtSortedIndex; i--)
            {
                int currentColumnID = sortedOrderToColumnIDMap[i - 1];
                sortedOrderToColumnIDMap[i] = currentColumnID;
                columnIDToSortOrderMap[currentColumnID] = i;
            }


            columnIDToSortOrderMap[columnID] = insertAtSortedIndex;
            sortedOrderToColumnIDMap[insertAtSortedIndex] = columnID;

            ++combinedColumnCount;
            dataVersion++;

            return columnID;
        }

        internal void RemoveColumnInternal<T>(ref ArrayHolder<T>[] allColumnsOfType, Serializable.SerializableTypes typeIndex, int columnID)
        {
            int columnLocation = columnIDToDenseIndexMap[columnID].columnDenseIndex;

            int lastIndex = allColumnsOfType.Length - 1;
            allColumnsOfType[columnLocation] = allColumnsOfType[lastIndex];
            Array.Resize(ref allColumnsOfType, lastIndex);

            ref string[] columnNamesOfType = ref allColumnNames[(int)typeIndex].TArray;
            columnNamesOfType[columnLocation] = columnNamesOfType[lastIndex];
            Array.Resize(ref columnNamesOfType, lastIndex);

            int columnOrder = columnIDToSortOrderMap[columnID];

            ref int[] denseIndicesOfType = ref columnDenseIndexToIDMap[(int)typeIndex].TArray;
            int sparseIndexToSwap = denseIndicesOfType[lastIndex];

            ref ColumnEntry sparseIndexToFree = ref columnIDToDenseIndexMap[columnID];
            sparseIndexToFree.ColumnType = Serializable.SerializableTypes.Invalid;
            sparseIndexToFree.columnDenseIndex = columnEntriesFreeListHead;
            columnEntriesFreeListHead = columnID;

            columnIDToDenseIndexMap[sparseIndexToSwap].columnDenseIndex = columnLocation;
            denseIndicesOfType[columnLocation] = sparseIndexToSwap;
            Array.Resize(ref denseIndicesOfType, lastIndex);

            for (int i = columnOrder + 1; i < combinedColumnCount; i++)
            {
                int currentColumnID = sortedOrderToColumnIDMap[i];
                sortedOrderToColumnIDMap[i - 1] = currentColumnID;
                columnIDToSortOrderMap[currentColumnID] = i - 1;
            }

            Array.Resize(ref sortedOrderToColumnIDMap, combinedColumnCount - 1);

            --combinedColumnCount;
            dataVersion++;
        }

        internal void InsertRowsOfTypeInternal<T>(ref ArrayHolder<T>[] allColumnsOfType, int insertAt, int numberOfNewRows)
        {
            int columnCount = allColumnsOfType?.Length ?? 0;
            for (int i = 0; i < columnCount; i++)
            {
                ref T[] rows = ref allColumnsOfType[i].TArray;
                int newRowCount = rowCount + numberOfNewRows;
                Array.Resize(ref rows, newRowCount);
                for (int j = newRowCount - 1; j > insertAt + numberOfNewRows - 1; j--)
                {
                    rows[j] = rows[j - numberOfNewRows];
                }

                for (int j = 0; j < numberOfNewRows; j++)
                {
                    rows[insertAt + j] = default;
                }
            }
        }

        internal void DeleteRowsOfTypeInternal<T>(ref ArrayHolder<T>[] allColumnsOfType, int removeAt, int numberOfRowsToDelete)
        {
            int columnCount = allColumnsOfType?.Length ?? 0;

            for (int i = 0; i < columnCount; i++)
            {
                ref T[] rows = ref allColumnsOfType[i].TArray;
                int newRowCount = rowCount - numberOfRowsToDelete;

                for (int j = removeAt + numberOfRowsToDelete; j < rowCount; j++)
                {
                    rows[j - numberOfRowsToDelete] = rows[j];
                }

                Array.Resize(ref rows, newRowCount);
            }
        }

        internal ref T GetCellRef<T>(int rowID, int columnID, ref ArrayHolder<T>[] allColumnsOfType)
        {
            int column = columnIDToDenseIndexMap[columnID].columnDenseIndex;
            int row = rowIDToDenseIndexMap[rowID];
            return ref allColumnsOfType[column][row];
        }

        internal T GetCell<T>(int rowID, int columnID, ref ArrayHolder<T>[] allColumnsOfType)
        {
            int column = columnIDToDenseIndexMap[columnID].columnDenseIndex;
            int row = rowIDToDenseIndexMap[rowID];
            return allColumnsOfType[column][row];
        }

        internal ulong SetCell<T>(int rowID, int columnID, ref ArrayHolder<T>[] allColumnsOfType, T value)
        {
            int column = columnIDToDenseIndexMap[columnID].columnDenseIndex;
            int row = rowIDToDenseIndexMap[rowID];
            allColumnsOfType[column][row] = value;
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