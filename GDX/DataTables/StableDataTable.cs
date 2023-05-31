// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using GDX.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GDX.DataTables
{
    [CreateAssetMenu(menuName = "GDX/Stable Table", fileName = "GDXStableTable")]
    [Serializable]
    public class StableDataTable : DataTableObject
    {
        internal static string UnityObjectString = typeof(Object).AssemblyQualifiedName;

        [SerializeField] internal string DisplayName = "GDXStableTable";

        [SerializeField] internal ArrayHolder<string>[] AllStringColumns;
        [SerializeField] internal ArrayHolder<bool>[] AllBoolColumns;
        [SerializeField] internal ArrayHolder<char>[] AllCharColumns;
        [SerializeField] internal ArrayHolder<sbyte>[] AllSbyteColumns;
        [SerializeField] internal ArrayHolder<byte>[] AllByteColumns;
        [SerializeField] internal ArrayHolder<short>[] AllShortColumns;
        [SerializeField] internal ArrayHolder<ushort>[] AllUshortColumns;
        [SerializeField] internal ArrayHolder<int>[] AllIntColumns;
        [SerializeField] internal ArrayHolder<uint>[] AllUintColumns;
        [SerializeField] internal ArrayHolder<long>[] AllLongColumns;
        [SerializeField] internal ArrayHolder<ulong>[] AllUlongColumns;
        [SerializeField] internal ArrayHolder<float>[] AllFloatColumns;
        [SerializeField] internal ArrayHolder<double>[] AllDoubleColumns;
        [SerializeField] internal ArrayHolder<Vector2>[] AllVector2Columns;
        [SerializeField] internal ArrayHolder<Vector3>[] AllVector3Columns;
        [SerializeField] internal ArrayHolder<Vector4>[] AllVector4Columns;
        [SerializeField] internal ArrayHolder<Vector2Int>[] AllVector2IntColumns;
        [SerializeField] internal ArrayHolder<Vector3Int>[] AllVector3IntColumns;
        [SerializeField] internal ArrayHolder<Quaternion>[] AllQuaternionColumns;
        [SerializeField] internal ArrayHolder<Rect>[] AllRectColumns;
        [SerializeField] internal ArrayHolder<RectInt>[] AllRectIntColumns;
        [SerializeField] internal ArrayHolder<Color>[] AllColorColumns;
        [SerializeField] internal ArrayHolder<LayerMask>[] AllLayerMaskColumns;
        [SerializeField] internal ArrayHolder<Bounds>[] AllBoundsColumns;
        [SerializeField] internal ArrayHolder<BoundsInt>[] AllBoundsIntColumns;
        [SerializeField] internal ArrayHolder<Hash128>[] AllHash128Columns;
        [SerializeField] internal ArrayHolder<Gradient>[] AllGradientColumns;
        [SerializeField] internal ArrayHolder<AnimationCurve>[] AllAnimationCurveColumns;
        [SerializeField] internal ArrayHolder<Object>[] AllObjectRefColumns;
        [SerializeField] internal string[] AllObjectRefTypeNames;

        [SerializeField] internal ArrayHolder<string>[]
            AllColumnNames =
                new ArrayHolder<string>[Serializable
                    .SerializableTypesCount]; // Contains the name of each column of each type. Ordered by Serializable.SerializableTypes

        [SerializeField] internal int[] RowIDToDenseIndexMap;
        [SerializeField] internal int[] RowDenseIndexToIDMap;
        [SerializeField] internal string[] RowNames;
        [SerializeField] internal int RowEntriesFreeListHead;

        [SerializeField] internal int RowCount;

        [SerializeField] internal ColumnEntry[] ColumnIDToDenseIndexMap;
        [SerializeField] internal int[] ColumnIDToSortOrderMap;
        [SerializeField] internal int[] SortedOrderToColumnIDMap;

        // TODO move with other block
        [SerializeField]
        ArrayHolder<int>[] ColumnDenseIndexToIDMap = new ArrayHolder<int>[Serializable.SerializableTypesCount];

        [SerializeField] internal int ColumnEntriesFreeListHead;

        [SerializeField] internal int CombinedColumnCount;

        [SerializeField] internal ulong DataVersion = 1;

        [SerializeField] BitArray8 SettingsFlags;

        /// <inheritdoc />
        public override ulong GetDataVersion()
        {
            return DataVersion;
        }

        /// <inheritdoc />
        public override int GetColumnCount()
        {
            return CombinedColumnCount;
        }

        /// <inheritdoc />
        public override int GetRowCount()
        {
            return RowCount;
        }

        /// <inheritdoc />
        public override string GetDisplayName()
        {
            return DisplayName;
        }

        /// <inheritdoc />
        public override void SetDisplayName(string displayName)
        {
            DisplayName = displayName;
        }

        /// <inheritdoc />
        public override bool GetFlag(Flags flag)
        {
            return SettingsFlags[(byte)flag];
        }

        /// <inheritdoc />
        public override void SetFlag(Flags flag, bool toggle)
        {
            SettingsFlags[(byte)flag] = toggle;
        }

        /// <inheritdoc />
        public override RowDescription[] GetAllRowDescriptions()
        {
            if (CombinedColumnCount == 0 || RowCount == 0)
            {
                return null;
            }

            RowDescription[] returnArray = new RowDescription[RowCount];
            for (int i = 0; i < RowCount; i++)
            {
                returnArray[i].Identifier = RowDenseIndexToIDMap[i];
                returnArray[i].Name = RowNames[i];
            }

            return returnArray;
        }

        /// <inheritdoc />
        public override RowDescription GetRowDescription(string name)
        {
            for (int i = 0; i < RowCount; i++)
            {
                string nameAt = RowNames[i];

                if (nameAt == name)
                {
                    return new RowDescription { Identifier = RowDenseIndexToIDMap[i], Name = nameAt };
                }
            }

            throw new ArgumentException("Row with name " + name + " does not exist in the table");
        }

        /// <inheritdoc />
        public override RowDescription GetRowDescription(int order)
        {
            return new RowDescription { Identifier = RowDenseIndexToIDMap[order], Name = RowNames[order] };
        }

        /// <inheritdoc />
        public override void SetAllRowDescriptionsOrder(RowDescription[] orderedRows)
        {
            // TODO: @adam array coming in be in the new order, just use the internalIndex (stable to reorder inside here
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override ColumnDescription GetColumnDescription(string name)
        {
            for (int i = 0; i < Serializable.SerializableTypesCount; i++)
            {
                string[] columnNames = AllColumnNames[i].TArray;

                if (columnNames != null)
                {
                    for (int j = 0; j < columnNames.Length; j++)
                    {
                        string nameAt = columnNames[j];

                        if (name == nameAt)
                        {
                            int columnID = ColumnDenseIndexToIDMap[i].TArray[j];

                            ref ColumnEntry columnEntry = ref ColumnIDToDenseIndexMap[columnID];
                            return new ColumnDescription
                            {
                                Identifier = columnID, Name = nameAt, Type = columnEntry.ColumnType
                            };
                        }
                    }
                }
            }

            throw new ArgumentException("Column with name " + name + " does not exist in the table");
        }

        /// <inheritdoc />
        public override ColumnDescription GetColumnDescription(int order)
        {
            int idAtOrderedIndex = SortedOrderToColumnIDMap[order];
            ref ColumnEntry columnEntry = ref ColumnIDToDenseIndexMap[idAtOrderedIndex];

            string columnName = AllColumnNames[(int)columnEntry.ColumnType][columnEntry.ColumnDenseIndex];

            return new ColumnDescription
            {
                Identifier = idAtOrderedIndex, Name = columnName, Type = columnEntry.ColumnType
            };
        }

        /// <inheritdoc />
        public override void SetAllColumnDescriptionsOrder(ColumnDescription[] orderedColumns)
        {
            // TODO: @adam array coming in be in the new order, just use the internalIndex (stable to reorder inside here
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override ColumnDescription[] GetAllColumnDescriptions()
        {
            if (CombinedColumnCount == 0)
            {
                return null;
            }

            ColumnDescription[] returnArray = new ColumnDescription[CombinedColumnCount];

            for (int i = 0; i < CombinedColumnCount; i++)
            {
                int columnID = SortedOrderToColumnIDMap[i];
                AssertColumnIDValid(columnID);
                ref ColumnEntry entryForID = ref ColumnIDToDenseIndexMap[columnID];
                ref ArrayHolder<string> nameColumnsForType = ref AllColumnNames[(int)entryForID.ColumnType];

                string name = nameColumnsForType[entryForID.ColumnDenseIndex];

                returnArray[i] = new ColumnDescription
                {
                    Name = name, Identifier = columnID, Type = entryForID.ColumnType
                };
            }

            return returnArray;
        }

        internal void AssertColumnIDValid(int columnID)
        {
            if (columnID < 0 || columnID >= ColumnIDToDenseIndexMap.Length)
            {
                throw new ArgumentException("Invalid column outside valid ID range: " + columnID);
            }

            ref ColumnEntry columnEntry = ref ColumnIDToDenseIndexMap[columnID];

            if (columnEntry.ColumnType == Serializable.SerializableTypes.Invalid)
            {
                throw new ArgumentException("Invalid column pointing to deallocated entry: " + columnID);
            }
        }

        internal void AssertRowIDValid(int rowID)
        {
            if (rowID < 0 || rowID >= RowIDToDenseIndexMap.Length)
            {
                throw new ArgumentException("Invalid row outside valid ID range: " + rowID);
            }

            int rowIndex = RowIDToDenseIndexMap[rowID];

            if (rowIndex >= RowCount || rowIndex < 0)
            {
                throw new ArgumentException("Invalid row outside valid ID range: " + rowID);
            }
        }

        /// <inheritdoc />
        public override void SetColumnName(string columnName, int columnIdentifier)
        {
            AssertColumnIDValid(columnIdentifier);
            ref ColumnEntry columnEntry = ref ColumnIDToDenseIndexMap[columnIdentifier];
            AllColumnNames[(int)columnEntry.ColumnType][columnEntry.ColumnDenseIndex] = columnName;
        }

        /// <inheritdoc />
        public override string GetColumnName(int columnIdentifier)
        {
            AssertColumnIDValid(columnIdentifier);
            ref ColumnEntry columnEntry = ref ColumnIDToDenseIndexMap[columnIdentifier];
            return AllColumnNames[(int)columnEntry.ColumnType][columnEntry.ColumnDenseIndex];
        }


        /// <inheritdoc />
        public override void SetRowName(string rowName, int rowIdentifier)
        {
            AssertRowIDValid(rowIdentifier);
            int rowDenseIndex = RowIDToDenseIndexMap[rowIdentifier];
            RowNames[rowDenseIndex] = rowName;
        }

        /// <inheritdoc />
        public override string GetRowName(int rowIdentifier)
        {
            AssertRowIDValid(rowIdentifier);
            int rowDenseIndex = RowIDToDenseIndexMap[rowIdentifier];
            return RowNames[rowDenseIndex];
        }

        public ref string GetRowNameRef(int row)
        {
            AssertRowIDValid(row);
            int rowDenseIndex = RowIDToDenseIndexMap[row];
            return ref RowNames[rowDenseIndex];
        }

        public ref string GetColumnNameRef(int columnID)
        {
            AssertColumnIDValid(columnID);
            ref ColumnEntry columnEntry = ref ColumnIDToDenseIndexMap[columnID];
            return ref AllColumnNames[(int)columnEntry.ColumnType][columnEntry.ColumnDenseIndex];
        }

        /// <inheritdoc />
        public override int AddRow(string rowName = null, int insertAtRowID = -1)
        {
            if (insertAtRowID >= 0)
            {
                AssertRowIDValid(insertAtRowID);
            }

            int rowID = RowEntriesFreeListHead;
            int rowIDToDenseIndexMapLength = RowIDToDenseIndexMap?.Length ?? 0;
            if (rowID >= rowIDToDenseIndexMapLength)
            {
                int newSize = rowID * 2;
                newSize = newSize == 0 ? 1 : newSize;
                Array.Resize(ref RowIDToDenseIndexMap, newSize);
                for (int i = rowID; i < newSize; i++)
                {
                    RowIDToDenseIndexMap[i] = i + 1;
                }
            }

            int denseIndexToIDMapLength = RowDenseIndexToIDMap?.Length ?? 0;
            Array.Resize(ref RowDenseIndexToIDMap, denseIndexToIDMapLength + 1);
            Array.Resize(ref RowNames, denseIndexToIDMapLength + 1);

            int insertAt = insertAtRowID < 0 ? RowCount : RowIDToDenseIndexMap[insertAtRowID];

            for (int i = denseIndexToIDMapLength; i > insertAt; i--)
            {
                int currentRowID = RowDenseIndexToIDMap[i - 1];
                RowDenseIndexToIDMap[i] = currentRowID;

                RowIDToDenseIndexMap[currentRowID] = i;

                RowNames[i] = RowNames[i - 1];
            }

            RowEntriesFreeListHead = RowIDToDenseIndexMap[rowID];
            RowIDToDenseIndexMap[rowID] = insertAt;
            RowDenseIndexToIDMap[insertAt] = rowID;
            RowNames[insertAt] = rowName == null ? rowID.ToString() : rowName;

            InsertRowsOfTypeInternal(ref AllStringColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref AllBoolColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref AllCharColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref AllSbyteColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref AllByteColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref AllShortColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref AllUshortColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref AllIntColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref AllUintColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref AllLongColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref AllUlongColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref AllFloatColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref AllDoubleColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref AllVector2Columns, insertAt, 1);
            InsertRowsOfTypeInternal(ref AllVector3Columns, insertAt, 1);
            InsertRowsOfTypeInternal(ref AllVector4Columns, insertAt, 1);
            InsertRowsOfTypeInternal(ref AllVector2IntColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref AllVector3IntColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref AllQuaternionColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref AllRectColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref AllRectIntColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref AllColorColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref AllLayerMaskColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref AllBoundsColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref AllBoundsIntColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref AllHash128Columns, insertAt, 1);
            InsertRowsOfTypeInternal(ref AllGradientColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref AllAnimationCurveColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref AllObjectRefColumns, insertAt, 1);

            ++RowCount;
            DataVersion++;

            return rowID;
        }

        public void AddRows(int numberOfNewRows, string[] rowNames = null, int insertAtRowID = -1)
        {
            if (insertAtRowID >= 0)
            {
                AssertRowIDValid(insertAtRowID);
            }

            int rowIDToDenseIndexMapLength = RowIDToDenseIndexMap?.Length ?? 0;
            int newCount = RowCount + numberOfNewRows;
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
                Array.Resize(ref RowIDToDenseIndexMap, newSize);
                for (int i = rowIDToDenseIndexMapLength; i < newSize; i++)
                {
                    RowIDToDenseIndexMap[i] = i + 1;
                }
            }

            int denseIndexToIDMapLength = RowDenseIndexToIDMap?.Length ?? 0;
            Array.Resize(ref RowDenseIndexToIDMap, denseIndexToIDMapLength + numberOfNewRows);
            Array.Resize(ref rowNames, denseIndexToIDMapLength + numberOfNewRows);

            int insertAt = insertAtRowID < 0 ? RowCount : RowIDToDenseIndexMap[insertAtRowID];

            for (int i = denseIndexToIDMapLength; i > insertAt + numberOfNewRows - 1; i--)
            {
                int currentRowID = RowDenseIndexToIDMap[i - numberOfNewRows];
                RowDenseIndexToIDMap[i] = currentRowID;

                RowIDToDenseIndexMap[currentRowID] = i;

                rowNames[i] = rowNames[i - numberOfNewRows];
            }

            int freeListHead = RowEntriesFreeListHead;

            for (int i = 0; i < numberOfNewRows; i++)
            {
                int rowID = freeListHead;
                freeListHead = RowIDToDenseIndexMap[rowID];
                RowIDToDenseIndexMap[rowID] = insertAt + i;
                RowDenseIndexToIDMap[insertAt + i] = rowID;
            }

            int numberOfNewRowNames = rowNames?.Length ?? 0;
            string emptyString = string.Empty;
            for (int i = 0; i < numberOfNewRowNames; i++)
            {
                string currentRowName = rowNames[i];
                int rowIDAt = RowDenseIndexToIDMap[insertAt + i];
                rowNames[insertAt + i] = currentRowName == null ? rowIDAt.ToString() : currentRowName;
            }

            for (int i = numberOfNewRowNames; i < numberOfNewRows; i++)
            {
                int rowIDAt = RowDenseIndexToIDMap[insertAt + i];
                rowNames[insertAt + i] = rowIDAt.ToString();
            }

            RowEntriesFreeListHead = freeListHead;

            InsertRowsOfTypeInternal(ref AllStringColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllBoolColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllCharColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllSbyteColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllByteColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllShortColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllUshortColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllIntColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllUintColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllLongColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllUlongColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllFloatColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllDoubleColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllVector2Columns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllVector3Columns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllVector4Columns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllVector2IntColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllVector3IntColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllQuaternionColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllRectColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllRectIntColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllColorColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllLayerMaskColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllBoundsColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllBoundsIntColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllHash128Columns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllGradientColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllAnimationCurveColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllObjectRefColumns, insertAt, numberOfNewRows);

            RowCount += numberOfNewRows;
            DataVersion++;
        }

        public void AddRows(int numberOfNewRows, ref int[] rowIDs, string[] rowNames = null, int insertAtRowID = -1)
        {
            if (insertAtRowID >= 0)
            {
                AssertRowIDValid(insertAtRowID);
            }

            int rowIDToDenseIndexMapLength = RowIDToDenseIndexMap?.Length ?? 0;
            int newCount = RowCount + numberOfNewRows;
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
                Array.Resize(ref RowIDToDenseIndexMap, newSize);
                for (int i = rowIDToDenseIndexMapLength; i < newSize; i++)
                {
                    RowIDToDenseIndexMap[i] = i + 1;
                }
            }

            int denseIndexToIDMapLength = RowDenseIndexToIDMap?.Length ?? 0;
            Array.Resize(ref RowDenseIndexToIDMap, denseIndexToIDMapLength + numberOfNewRows);

            int insertAt = insertAtRowID < 0 ? RowCount : RowIDToDenseIndexMap[insertAtRowID];

            for (int i = denseIndexToIDMapLength; i > insertAt + numberOfNewRows - 1; i--)
            {
                int currentRowID = RowDenseIndexToIDMap[i - numberOfNewRows];
                RowDenseIndexToIDMap[i] = currentRowID;

                RowIDToDenseIndexMap[currentRowID] = i;

                rowNames[i] = rowNames[i - numberOfNewRows];
            }

            int freeListHead = RowEntriesFreeListHead;

            for (int i = 0; i < numberOfNewRows; i++)
            {
                int rowID = freeListHead;
                freeListHead = RowIDToDenseIndexMap[rowID];
                RowIDToDenseIndexMap[rowID] = insertAt + i;
                RowDenseIndexToIDMap[insertAt + i] = rowID;
                rowIDs[i] = rowID;
            }

            int numberOfNewRowNames = rowNames?.Length ?? 0;
            for (int i = 0; i < numberOfNewRowNames; i++)
            {
                string currentRowName = rowNames[i];
                int rowIDAt = RowDenseIndexToIDMap[insertAt + i];
                rowNames[insertAt + i] = currentRowName == null ? rowIDAt.ToString() : currentRowName;
            }

            for (int i = numberOfNewRowNames; i < numberOfNewRows; i++)
            {
                int rowIDAt = RowDenseIndexToIDMap[insertAt + i];
                rowNames[insertAt + i] = rowIDAt.ToString();
            }

            RowEntriesFreeListHead = freeListHead;

            InsertRowsOfTypeInternal(ref AllStringColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllBoolColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllCharColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllSbyteColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllByteColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllShortColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllUshortColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllIntColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllUintColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllLongColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllUlongColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllFloatColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllDoubleColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllVector2Columns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllVector3Columns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllVector4Columns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllVector2IntColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllVector3IntColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllQuaternionColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllRectColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllRectIntColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllColorColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllLayerMaskColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllBoundsColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllBoundsIntColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllHash128Columns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllGradientColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllAnimationCurveColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref AllObjectRefColumns, insertAt, numberOfNewRows);

            RowCount += numberOfNewRows;
            DataVersion++;
        }

        /// <inheritdoc />
        public override void RemoveRow(int rowID)
        {
            AssertRowIDValid(rowID);
            int rowDenseIndex = RowIDToDenseIndexMap[rowID];
            for (int i = rowDenseIndex + 1; i < RowCount; i++)
            {
                int currentRowID = RowDenseIndexToIDMap[i];
                RowIDToDenseIndexMap[currentRowID] = i - 1;
                RowDenseIndexToIDMap[i - 1] = currentRowID;
                RowNames[i - 1] = RowNames[i];
            }

            RowIDToDenseIndexMap[rowID] = RowEntriesFreeListHead;
            RowEntriesFreeListHead = rowID;
            Array.Resize(ref RowDenseIndexToIDMap, RowCount - 1);
            Array.Resize(ref RowNames, RowCount - 1);

            DeleteRowsOfTypeInternal(ref AllStringColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref AllBoolColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref AllCharColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref AllSbyteColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref AllByteColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref AllShortColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref AllUshortColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref AllIntColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref AllUintColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref AllLongColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref AllUlongColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref AllFloatColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref AllDoubleColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref AllVector2Columns, rowID, 1);
            DeleteRowsOfTypeInternal(ref AllVector3Columns, rowID, 1);
            DeleteRowsOfTypeInternal(ref AllVector4Columns, rowID, 1);
            DeleteRowsOfTypeInternal(ref AllVector2IntColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref AllVector3IntColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref AllQuaternionColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref AllRectColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref AllRectIntColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref AllColorColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref AllLayerMaskColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref AllBoundsColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref AllBoundsIntColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref AllHash128Columns, rowID, 1);
            DeleteRowsOfTypeInternal(ref AllGradientColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref AllAnimationCurveColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref AllObjectRefColumns, rowID, 1);

            --RowCount;
            DataVersion++;
        }

        /// <inheritdoc />
        public override int AddColumn(Serializable.SerializableTypes columnType, string columnName,
            int insertAtColumnID = -1)
        {
            switch (columnType)
            {
                case Serializable.SerializableTypes.String:
                    return AddColumnInternal(columnName, ref AllStringColumns, Serializable.SerializableTypes.String,
                        insertAtColumnID);
                case Serializable.SerializableTypes.Char:
                    return AddColumnInternal(columnName, ref AllCharColumns, Serializable.SerializableTypes.Char,
                        insertAtColumnID);
                case Serializable.SerializableTypes.Bool:
                    return AddColumnInternal(columnName, ref AllBoolColumns, Serializable.SerializableTypes.Bool,
                        insertAtColumnID);
                case Serializable.SerializableTypes.SByte:
                    return AddColumnInternal(columnName, ref AllSbyteColumns, Serializable.SerializableTypes.SByte,
                        insertAtColumnID);
                case Serializable.SerializableTypes.Byte:
                    return AddColumnInternal(columnName, ref AllByteColumns, Serializable.SerializableTypes.Byte,
                        insertAtColumnID);
                case Serializable.SerializableTypes.Short:
                    return AddColumnInternal(columnName, ref AllShortColumns, Serializable.SerializableTypes.Short,
                        insertAtColumnID);
                case Serializable.SerializableTypes.UShort:
                    return AddColumnInternal(columnName, ref AllUshortColumns, Serializable.SerializableTypes.UShort,
                        insertAtColumnID);
                case Serializable.SerializableTypes.Int:
                    return AddColumnInternal(columnName, ref AllIntColumns, Serializable.SerializableTypes.Int,
                        insertAtColumnID);
                case Serializable.SerializableTypes.UInt:
                    return AddColumnInternal(columnName, ref AllUintColumns, Serializable.SerializableTypes.UInt,
                        insertAtColumnID);
                case Serializable.SerializableTypes.Long:
                    return AddColumnInternal(columnName, ref AllLongColumns, Serializable.SerializableTypes.Long,
                        insertAtColumnID);
                case Serializable.SerializableTypes.ULong:
                    return AddColumnInternal(columnName, ref AllUlongColumns, Serializable.SerializableTypes.ULong,
                        insertAtColumnID);
                case Serializable.SerializableTypes.Float:
                    return AddColumnInternal(columnName, ref AllFloatColumns, Serializable.SerializableTypes.Float,
                        insertAtColumnID);
                case Serializable.SerializableTypes.Double:
                    return AddColumnInternal(columnName, ref AllDoubleColumns, Serializable.SerializableTypes.Double,
                        insertAtColumnID);
                case Serializable.SerializableTypes.Vector2:
                    return AddColumnInternal(columnName, ref AllVector2Columns, Serializable.SerializableTypes.Vector2,
                        insertAtColumnID);
                case Serializable.SerializableTypes.Vector3:
                    return AddColumnInternal(columnName, ref AllVector3Columns, Serializable.SerializableTypes.Vector3,
                        insertAtColumnID);
                case Serializable.SerializableTypes.Vector4:
                    return AddColumnInternal(columnName, ref AllVector4Columns, Serializable.SerializableTypes.Vector4,
                        insertAtColumnID);
                case Serializable.SerializableTypes.Vector2Int:
                    return AddColumnInternal(columnName, ref AllVector2IntColumns,
                        Serializable.SerializableTypes.Vector2Int, insertAtColumnID);
                case Serializable.SerializableTypes.Vector3Int:
                    return AddColumnInternal(columnName, ref AllVector3IntColumns,
                        Serializable.SerializableTypes.Vector3Int, insertAtColumnID);
                case Serializable.SerializableTypes.Quaternion:
                    return AddColumnInternal(columnName, ref AllQuaternionColumns,
                        Serializable.SerializableTypes.Quaternion, insertAtColumnID);
                case Serializable.SerializableTypes.Rect:
                    return AddColumnInternal(columnName, ref AllRectColumns, Serializable.SerializableTypes.Rect,
                        insertAtColumnID);
                case Serializable.SerializableTypes.RectInt:
                    return AddColumnInternal(columnName, ref AllRectIntColumns, Serializable.SerializableTypes.RectInt,
                        insertAtColumnID);
                case Serializable.SerializableTypes.Color:
                    return AddColumnInternal(columnName, ref AllColorColumns, Serializable.SerializableTypes.Color,
                        insertAtColumnID);
                case Serializable.SerializableTypes.LayerMask:
                    return AddColumnInternal(columnName, ref AllLayerMaskColumns,
                        Serializable.SerializableTypes.LayerMask, insertAtColumnID);
                case Serializable.SerializableTypes.Bounds:
                    return AddColumnInternal(columnName, ref AllBoundsColumns, Serializable.SerializableTypes.Bounds,
                        insertAtColumnID);
                case Serializable.SerializableTypes.BoundsInt:
                    return AddColumnInternal(columnName, ref AllBoundsIntColumns,
                        Serializable.SerializableTypes.BoundsInt, insertAtColumnID);
                case Serializable.SerializableTypes.Hash128:
                    return AddColumnInternal(columnName, ref AllHash128Columns, Serializable.SerializableTypes.Hash128,
                        insertAtColumnID);
                case Serializable.SerializableTypes.Gradient:
                    return AddColumnInternal(columnName, ref AllGradientColumns,
                        Serializable.SerializableTypes.Gradient, insertAtColumnID);
                case Serializable.SerializableTypes.AnimationCurve:
                    return AddColumnInternal(columnName, ref AllAnimationCurveColumns,
                        Serializable.SerializableTypes.AnimationCurve, insertAtColumnID);
                case Serializable.SerializableTypes.Object:
                    return AddColumnInternal(columnName, ref AllObjectRefColumns, Serializable.SerializableTypes.Object,
                        insertAtColumnID);
            }

            return -1;
        }

        /// <inheritdoc />
        public override void RemoveColumn(Serializable.SerializableTypes columnType, int columnID)
        {
            switch (columnType)
            {
                case Serializable.SerializableTypes.String:
                    RemoveColumnInternal(ref AllStringColumns, Serializable.SerializableTypes.String, columnID);
                    break;
                case Serializable.SerializableTypes.Char:
                    RemoveColumnInternal(ref AllCharColumns, Serializable.SerializableTypes.Char, columnID);
                    break;
                case Serializable.SerializableTypes.Bool:
                    RemoveColumnInternal(ref AllBoolColumns, Serializable.SerializableTypes.Bool, columnID);
                    break;
                case Serializable.SerializableTypes.SByte:
                    RemoveColumnInternal(ref AllSbyteColumns, Serializable.SerializableTypes.SByte, columnID);
                    break;
                case Serializable.SerializableTypes.Byte:
                    RemoveColumnInternal(ref AllByteColumns, Serializable.SerializableTypes.Byte, columnID);
                    break;
                case Serializable.SerializableTypes.Short:
                    RemoveColumnInternal(ref AllShortColumns, Serializable.SerializableTypes.Short, columnID);
                    break;
                case Serializable.SerializableTypes.UShort:
                    RemoveColumnInternal(ref AllUshortColumns, Serializable.SerializableTypes.UShort, columnID);
                    break;
                case Serializable.SerializableTypes.Int:
                    RemoveColumnInternal(ref AllIntColumns, Serializable.SerializableTypes.Int, columnID);
                    break;
                case Serializable.SerializableTypes.UInt:
                    RemoveColumnInternal(ref AllUintColumns, Serializable.SerializableTypes.UInt, columnID);
                    break;
                case Serializable.SerializableTypes.Long:
                    RemoveColumnInternal(ref AllLongColumns, Serializable.SerializableTypes.Long, columnID);
                    break;
                case Serializable.SerializableTypes.ULong:
                    RemoveColumnInternal(ref AllUlongColumns, Serializable.SerializableTypes.ULong, columnID);
                    break;
                case Serializable.SerializableTypes.Float:
                    RemoveColumnInternal(ref AllFloatColumns, Serializable.SerializableTypes.Float, columnID);
                    break;
                case Serializable.SerializableTypes.Double:
                    RemoveColumnInternal(ref AllDoubleColumns, Serializable.SerializableTypes.Double, columnID);
                    break;
                case Serializable.SerializableTypes.Vector2:
                    RemoveColumnInternal(ref AllVector2Columns, Serializable.SerializableTypes.Vector2, columnID);
                    break;
                case Serializable.SerializableTypes.Vector3:
                    RemoveColumnInternal(ref AllVector3Columns, Serializable.SerializableTypes.Vector3, columnID);
                    break;
                case Serializable.SerializableTypes.Vector4:
                    RemoveColumnInternal(ref AllVector4Columns, Serializable.SerializableTypes.Vector4, columnID);
                    break;
                case Serializable.SerializableTypes.Vector2Int:
                    RemoveColumnInternal(ref AllVector2IntColumns, Serializable.SerializableTypes.Vector2Int, columnID);
                    break;
                case Serializable.SerializableTypes.Vector3Int:
                    RemoveColumnInternal(ref AllVector3IntColumns, Serializable.SerializableTypes.Vector3Int, columnID);
                    break;
                case Serializable.SerializableTypes.Quaternion:
                    RemoveColumnInternal(ref AllQuaternionColumns, Serializable.SerializableTypes.Quaternion, columnID);
                    break;
                case Serializable.SerializableTypes.Rect:
                    RemoveColumnInternal(ref AllRectColumns, Serializable.SerializableTypes.Rect, columnID);
                    break;
                case Serializable.SerializableTypes.RectInt:
                    RemoveColumnInternal(ref AllRectIntColumns, Serializable.SerializableTypes.RectInt, columnID);
                    break;
                case Serializable.SerializableTypes.Color:
                    RemoveColumnInternal(ref AllColorColumns, Serializable.SerializableTypes.Color, columnID);
                    break;
                case Serializable.SerializableTypes.LayerMask:
                    RemoveColumnInternal(ref AllLayerMaskColumns, Serializable.SerializableTypes.LayerMask, columnID);
                    break;
                case Serializable.SerializableTypes.Bounds:
                    RemoveColumnInternal(ref AllBoundsColumns, Serializable.SerializableTypes.Bounds, columnID);
                    break;
                case Serializable.SerializableTypes.BoundsInt:
                    RemoveColumnInternal(ref AllBoundsIntColumns, Serializable.SerializableTypes.BoundsInt, columnID);
                    break;
                case Serializable.SerializableTypes.Hash128:
                    RemoveColumnInternal(ref AllHash128Columns, Serializable.SerializableTypes.Hash128, columnID);
                    break;
                case Serializable.SerializableTypes.Gradient:
                    RemoveColumnInternal(ref AllGradientColumns, Serializable.SerializableTypes.Gradient, columnID);
                    break;
                case Serializable.SerializableTypes.AnimationCurve:
                    RemoveColumnInternal(ref AllAnimationCurveColumns, Serializable.SerializableTypes.AnimationCurve,
                        columnID);
                    break;
                case Serializable.SerializableTypes.Object:
                    RemoveColumnInternal(ref AllObjectRefColumns, Serializable.SerializableTypes.Object, columnID);
                    break;
            }
        }

        // Set
        /// <inheritdoc />
        public override ulong SetString(int rowIdentifier, int columnIdentifier, string newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllStringColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetBool(int rowIdentifier, int columnIdentifier, bool newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllBoolColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetChar(int rowIdentifier, int columnIdentifier, char newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllCharColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetSByte(int rowIdentifier, int columnIdentifier, sbyte newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllSbyteColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetByte(int rowIdentifier, int columnIdentifier, byte newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllByteColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetShort(int rowIdentifier, int columnIdentifier, short newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllShortColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetUShort(int rowIdentifier, int columnIdentifier, ushort newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllUshortColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetInt(int rowIdentifier, int columnIdentifier, int newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllIntColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetUInt(int rowIdentifier, int columnIdentifier, uint newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllUintColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetLong(int rowIdentifier, int columnIdentifier, long newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllLongColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetULong(int rowIdentifier, int columnIdentifier, ulong newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllUlongColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetFloat(int rowIdentifier, int columnIdentifier, float newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllFloatColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetDouble(int rowIdentifier, int columnIdentifier, double newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllDoubleColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetVector2(int rowIdentifier, int columnIdentifier, Vector2 newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllVector2Columns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetVector3(int rowIdentifier, int columnIdentifier, Vector3 newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllVector3Columns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetVector4(int rowIdentifier, int columnIdentifier, Vector4 value)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllVector4Columns, value);
        }

        /// <inheritdoc />
        public override ulong SetVector2Int(int rowIdentifier, int columnIdentifier, Vector2Int newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllVector2IntColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetVector3Int(int rowIdentifier, int columnIdentifier, Vector3Int newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllVector3IntColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetQuaternion(int rowIdentifier, int columnIdentifier, Quaternion newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllQuaternionColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetRect(int rowIdentifier, int columnIdentifier, Rect newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllRectColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetRectInt(int rowIdentifier, int columnIdentifier, RectInt newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllRectIntColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetColor(int rowIdentifier, int columnIdentifier, Color newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllColorColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetLayerMask(int rowIdentifier, int columnIdentifier, LayerMask newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllLayerMaskColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetBounds(int rowIdentifier, int columnIdentifier, Bounds newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllBoundsColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetBoundsInt(int rowIdentifier, int columnIdentifier, BoundsInt newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllBoundsIntColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetHash128(int rowIdentifier, int columnIdentifier, Hash128 newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllHash128Columns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetGradient(int rowIdentifier, int columnIdentifier, Gradient newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllGradientColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetAnimationCurve(int rowIdentifier, int columnIdentifier, AnimationCurve newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllAnimationCurveColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetObject(int rowIdentifier, int columnIdentifier, Object newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref AllObjectRefColumns, newValue);
        }

        /// <inheritdoc />
        public override void SetTypeNameForObjectColumn(int columnIdentifier, string assemblyQualifiedName)
        {
            AssertObjectColumnIDValid(columnIdentifier);
            int denseIndex = ColumnIDToDenseIndexMap[columnIdentifier].ColumnDenseIndex;
            AllObjectRefTypeNames[denseIndex] = assemblyQualifiedName;
        }

        /// <inheritdoc />
        public override string GetTypeNameForObjectColumn(int columnIdentifier)
        {
            AssertObjectColumnIDValid(columnIdentifier);
            int denseIndex = ColumnIDToDenseIndexMap[columnIdentifier].ColumnDenseIndex;
            return AllObjectRefTypeNames[denseIndex];
        }

        // Get
        /// <inheritdoc />
        public override string GetString(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllStringColumns);
        }

        /// <inheritdoc />
        public override bool GetBool(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllBoolColumns);
        }

        /// <inheritdoc />
        public override char GetChar(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllCharColumns);
        }

        /// <inheritdoc />
        public override sbyte GetSByte(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllSbyteColumns);
        }

        /// <inheritdoc />
        public override byte GetByte(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllByteColumns);
        }

        /// <inheritdoc />
        public override short GetShort(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllShortColumns);
        }

        /// <inheritdoc />
        public override ushort GetUShort(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllUshortColumns);
        }

        /// <inheritdoc />
        public override int GetInt(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllIntColumns);
        }

        /// <inheritdoc />
        public override uint GetUInt(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllUintColumns);
        }

        /// <inheritdoc />
        public override long GetLong(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllLongColumns);
        }

        /// <inheritdoc />
        public override ulong GetULong(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllUlongColumns);
        }

        /// <inheritdoc />
        public override float GetFloat(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllFloatColumns);
        }

        /// <inheritdoc />
        public override double GetDouble(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllDoubleColumns);
        }

        /// <inheritdoc />
        public override Vector2 GetVector2(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllVector2Columns);
        }

        /// <inheritdoc />
        public override Vector3 GetVector3(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllVector3Columns);
        }

        /// <inheritdoc />
        public override Vector4 GetVector4(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllVector4Columns);
        }

        /// <inheritdoc />
        public override Vector2Int GetVector2Int(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllVector2IntColumns);
        }

        /// <inheritdoc />
        public override Vector3Int GetVector3Int(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllVector3IntColumns);
        }

        /// <inheritdoc />
        public override Quaternion GetQuaternion(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllQuaternionColumns);
        }

        /// <inheritdoc />
        public override Rect GetRect(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllRectColumns);
        }

        /// <inheritdoc />
        public override RectInt GetRectInt(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllRectIntColumns);
        }

        /// <inheritdoc />
        public override Color GetColor(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllColorColumns);
        }

        /// <inheritdoc />
        public override LayerMask GetLayerMask(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllLayerMaskColumns);
        }

        /// <inheritdoc />
        public override Bounds GetBounds(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllBoundsColumns);
        }

        /// <inheritdoc />
        public override BoundsInt GetBoundsInt(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllBoundsIntColumns);
        }

        /// <inheritdoc />
        public override Hash128 GetHash128(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllHash128Columns);
        }

        /// <inheritdoc />
        public override Gradient GetGradient(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllGradientColumns);
        }

        /// <inheritdoc />
        public override AnimationCurve GetAnimationCurve(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllAnimationCurveColumns);
        }

        /// <inheritdoc />
        public override Object GetObject(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref AllObjectRefColumns);
        }

        // Get ref

        public ref string GetStringRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref AllStringColumns);
        }

        public ref bool GetBoolRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref AllBoolColumns);
        }

        public ref char GetCharRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref AllCharColumns);
        }

        public ref sbyte GetSbyteRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref AllSbyteColumns);
        }

        public ref byte GetByteRef(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref AllByteColumns);
        }

        public ref short GetShortRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref AllShortColumns);
        }

        public ref ushort GetUshortRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref AllUshortColumns);
        }

        public ref int GetIntRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref AllIntColumns);
        }

        public ref uint GetUintRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref AllUintColumns);
        }

        public ref long GetLongRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref AllLongColumns);
        }

        public ref ulong GetUlongRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref AllUlongColumns);
        }

        public ref float GetFloatRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref AllFloatColumns);
        }

        public ref double GetDoubleRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref AllDoubleColumns);
        }

        public ref Vector2 GetVector2Ref(int row, int column)
        {
            return ref GetCellRef(row, column, ref AllVector2Columns);
        }

        public ref Vector3 GetVector3Ref(int row, int column)
        {
            return ref GetCellRef(row, column, ref AllVector3Columns);
        }

        public ref Vector4 GetVector4Ref(int row, int column)
        {
            return ref GetCellRef(row, column, ref AllVector4Columns);
        }

        public ref Vector2Int GetVector2IntRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref AllVector2IntColumns);
        }

        public ref Vector3Int GetVector3IntRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref AllVector3IntColumns);
        }

        public ref Quaternion GetQuaternionRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref AllQuaternionColumns);
        }

        public ref Rect GetRectRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref AllRectColumns);
        }

        public ref RectInt GetRectIntRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref AllRectIntColumns);
        }

        public ref Color GetColorRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref AllColorColumns);
        }

        public ref LayerMask GetLayerMaskRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref AllLayerMaskColumns);
        }

        public ref Bounds GetBoundsRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref AllBoundsColumns);
        }

        public ref BoundsInt GetBoundsIntRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref AllBoundsIntColumns);
        }

        public ref Hash128 GetHash128Ref(int row, int column)
        {
            return ref GetCellRef(row, column, ref AllHash128Columns);
        }

        public ref Gradient GetGradientRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref AllGradientColumns);
        }

        public ref AnimationCurve GetAnimationCurveRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref AllAnimationCurveColumns);
        }

        public ref Object GetObjectRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref AllObjectRefColumns);
        }

        // Get Column

        public string[] GetStringColumn(int column)
        {
            return GetColumn(column, ref AllStringColumns);
        }

        public bool[] GetBoolColumn(int column)
        {
            return GetColumn(column, ref AllBoolColumns);
        }

        public char[] GetCharColumn(int column)
        {
            return GetColumn(column, ref AllCharColumns);
        }

        public sbyte[] GetSbyteColumn(int column)
        {
            return GetColumn(column, ref AllSbyteColumns);
        }

        public byte[] GetByteColumn(int column)
        {
            return GetColumn(column, ref AllByteColumns);
        }

        public short[] GetShortColumn(int column)
        {
            return GetColumn(column, ref AllShortColumns);
        }

        public ushort[] GetUshortColumn(int column)
        {
            return GetColumn(column, ref AllUshortColumns);
        }

        public int[] GetIntColumn(int column)
        {
            return GetColumn(column, ref AllIntColumns);
        }

        public uint[] GetUintColumn(int column)
        {
            return GetColumn(column, ref AllUintColumns);
        }

        public long[] GetLongColumn(int column)
        {
            return GetColumn(column, ref AllLongColumns);
        }

        public ulong[] GetUlongColumn(int column)
        {
            return GetColumn(column, ref AllUlongColumns);
        }

        public float[] GetFloatColumn(int column)
        {
            return GetColumn(column, ref AllFloatColumns);
        }

        public double[] GetDoubleColumn(int column)
        {
            return GetColumn(column, ref AllDoubleColumns);
        }

        public Vector2[] GetVector2Column(int column)
        {
            return GetColumn(column, ref AllVector2Columns);
        }

        public Vector3[] GetVector3Column(int column)
        {
            return GetColumn(column, ref AllVector3Columns);
        }

        public Vector4[] GetVector4Column(int column)
        {
            return GetColumn(column, ref AllVector4Columns);
        }

        public Vector2Int[] GetVector2IntColumn(int column)
        {
            return GetColumn(column, ref AllVector2IntColumns);
        }

        public Vector3Int[] GetVector3IntColumn(int column)
        {
            return GetColumn(column, ref AllVector3IntColumns);
        }

        public Quaternion[] GetQuaternionColumn(int column)
        {
            return GetColumn(column, ref AllQuaternionColumns);
        }

        public Rect[] GetRectColumn(int column)
        {
            return GetColumn(column, ref AllRectColumns);
        }

        public RectInt[] GetRectIntColumn(int column)
        {
            return GetColumn(column, ref AllRectIntColumns);
        }

        public Color[] GetColorColumn(int column)
        {
            return GetColumn(column, ref AllColorColumns);
        }

        public LayerMask[] GetLayerMaskColumn(int column)
        {
            return GetColumn(column, ref AllLayerMaskColumns);
        }

        public Bounds[] GetBoundsColumn(int column)
        {
            return GetColumn(column, ref AllBoundsColumns);
        }

        public BoundsInt[] GetBoundsIntColumn(int column)
        {
            return GetColumn(column, ref AllBoundsIntColumns);
        }

        public Hash128[] GetHash128Column(int column)
        {
            return GetColumn(column, ref AllHash128Columns);
        }

        public Gradient[] GetGradientColumn(int column)
        {
            return GetColumn(column, ref AllGradientColumns);
        }

        public AnimationCurve[] GetAnimationCurveColumn(int column)
        {
            return GetColumn(column, ref AllAnimationCurveColumns);
        }

        public Object[] GetObjectColumn(int column)
        {
            return GetColumn(column, ref AllObjectRefColumns);
        }

        // SetOrder

        public void SetColumnOrder(int columnID, int newSortOrder)
        {
            AssertColumnIDValid(columnID);
            AssertColumnSortOrderValid(newSortOrder);
            int oldSortOrder = ColumnIDToSortOrderMap[columnID];
            int iterDirection = newSortOrder > oldSortOrder ? 1 : -1;
            for (int i = oldSortOrder; i != newSortOrder; i += iterDirection)
            {
                int columnIDAt = SortedOrderToColumnIDMap[i + iterDirection];
                ColumnIDToSortOrderMap[columnIDAt] = i;
                SortedOrderToColumnIDMap[i] = SortedOrderToColumnIDMap[i + iterDirection];
            }

            SortedOrderToColumnIDMap[newSortOrder] = columnID;
            ColumnIDToSortOrderMap[columnID] = newSortOrder;
        }

        public void SetAllColumnOrders(int[] sortedColumnIDs)
        {
            AssertSortedColumnsArgValid(sortedColumnIDs);
            for (int i = 0; i < SortedOrderToColumnIDMap.Length; i++)
            {
                int columnID = sortedColumnIDs[i];
                SortedOrderToColumnIDMap[i] = columnID;
                ColumnIDToSortOrderMap[columnID] = i;
            }
        }

        public void SetRowOrder(int rowID, int newSortOrder)
        {
            AssertRowIDValid(rowID);
            AssertRowSortOrderValid(newSortOrder);

            int oldSortOrder = RowIDToDenseIndexMap[rowID];
            int iterDirection = newSortOrder > oldSortOrder ? 1 : -1;

            for (int i = oldSortOrder; i != newSortOrder; i += iterDirection)
            {
                int rowIDAt = RowDenseIndexToIDMap[i + iterDirection];
                RowIDToDenseIndexMap[rowIDAt] = i;
                RowDenseIndexToIDMap[i] = RowDenseIndexToIDMap[i + iterDirection];
            }

            SetRowOrderForColumns(AllStringColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(AllBoolColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(AllCharColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(AllSbyteColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(AllByteColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(AllShortColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(AllUshortColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(AllIntColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(AllUintColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(AllLongColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(AllUlongColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(AllFloatColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(AllDoubleColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(AllVector2Columns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(AllVector3Columns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(AllVector4Columns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(AllVector2IntColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(AllVector3IntColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(AllQuaternionColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(AllRectColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(AllRectIntColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(AllColorColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(AllLayerMaskColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(AllBoundsColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(AllBoundsIntColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(AllHash128Columns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(AllGradientColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(AllAnimationCurveColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(AllObjectRefColumns, oldSortOrder, newSortOrder);
        }

        public void SetAllRowOrders(int[] sortedRowIDs)
        {
            AssertSortRowsArgValid(sortedRowIDs);

            ReSortRows(AllStringColumns, sortedRowIDs);
            ReSortRows(AllBoolColumns, sortedRowIDs);
            ReSortRows(AllCharColumns, sortedRowIDs);
            ReSortRows(AllSbyteColumns, sortedRowIDs);
            ReSortRows(AllByteColumns, sortedRowIDs);
            ReSortRows(AllShortColumns, sortedRowIDs);
            ReSortRows(AllUshortColumns, sortedRowIDs);
            ReSortRows(AllIntColumns, sortedRowIDs);
            ReSortRows(AllUintColumns, sortedRowIDs);
            ReSortRows(AllLongColumns, sortedRowIDs);
            ReSortRows(AllUlongColumns, sortedRowIDs);
            ReSortRows(AllFloatColumns, sortedRowIDs);
            ReSortRows(AllDoubleColumns, sortedRowIDs);
            ReSortRows(AllVector2Columns, sortedRowIDs);
            ReSortRows(AllVector3Columns, sortedRowIDs);
            ReSortRows(AllVector4Columns, sortedRowIDs);
            ReSortRows(AllVector2IntColumns, sortedRowIDs);
            ReSortRows(AllVector3IntColumns, sortedRowIDs);
            ReSortRows(AllQuaternionColumns, sortedRowIDs);
            ReSortRows(AllRectColumns, sortedRowIDs);
            ReSortRows(AllRectIntColumns, sortedRowIDs);
            ReSortRows(AllColorColumns, sortedRowIDs);
            ReSortRows(AllLayerMaskColumns, sortedRowIDs);
            ReSortRows(AllBoundsColumns, sortedRowIDs);
            ReSortRows(AllBoundsIntColumns, sortedRowIDs);
            ReSortRows(AllHash128Columns, sortedRowIDs);
            ReSortRows(AllGradientColumns, sortedRowIDs);
            ReSortRows(AllAnimationCurveColumns, sortedRowIDs);
            ReSortRows(AllObjectRefColumns, sortedRowIDs);

            for (int i = 0; i < sortedRowIDs.Length; i++)
            {
                int rowID = sortedRowIDs[i];
                RowDenseIndexToIDMap[i] = rowID;
                RowIDToDenseIndexMap[rowID] = i;
            }
        }

        internal void ReSortRows<T>(ArrayHolder<T>[] columns, int[] sortedRowIDs)
        {
            int columnCount = columns?.Length ?? 0;
            for (int i = 0; i < columnCount; i++)
            {
                T[] column = columns[i].TArray;
                T[] newColumn = new T[column.Length];
                for (int j = 0; j < sortedRowIDs.Length; j++)
                {
                    int rowID = sortedRowIDs[j];
                    int oldRowIndex = RowIDToDenseIndexMap[rowID];

                    newColumn[j] = column[oldRowIndex];
                }

                columns[i].TArray = newColumn;
            }
        }

        // Internal

        internal void AddTypeNameEntryForUnityObjectColumn()
        {
            int nameArrayLength = AllObjectRefTypeNames?.Length ?? 0;
            Array.Resize(ref AllObjectRefTypeNames, nameArrayLength + 1);
            AllObjectRefTypeNames[nameArrayLength] = UnityObjectString;
        }

        internal void RemoveTypeNameEntryForUnityObjectColumn(int columnDenseIndex)
        {
            int nameArrayLength = AllObjectRefTypeNames?.Length ?? 0;
            AllObjectRefTypeNames[columnDenseIndex] = AllObjectRefTypeNames[nameArrayLength];
            Array.Resize(ref AllObjectRefTypeNames, nameArrayLength - 1);
        }

        internal void AssertObjectColumnIDValid(int columnID)
        {
            AssertColumnIDValid(columnID);
            if (ColumnIDToDenseIndexMap[columnID].ColumnType != Serializable.SerializableTypes.Object)
            {
                throw new ArgumentException("Column ID must correspond to a UnityEngine.Object column.");
            }
        }

        internal int AddColumnInternal<T>(string columnName, ref ArrayHolder<T>[] allColumnsOfType,
            Serializable.SerializableTypes typeIndex, int insertAtColumnID)
        {
            if (insertAtColumnID >= 0)
            {
                AssertColumnIDValid(insertAtColumnID);
            }

            int columnCount = allColumnsOfType?.Length ?? 0;
            Array.Resize(ref allColumnsOfType, columnCount + 1);
            allColumnsOfType[columnCount].TArray = new T[RowCount];

            int columnID = ColumnEntriesFreeListHead;
            string[] columnNamesForType = AllColumnNames[(int)typeIndex].TArray;
            int columnNamesCount = columnNamesForType?.Length ?? 0;
            Array.Resize(ref columnNamesForType, columnNamesCount + 1);
            columnNamesForType[columnNamesCount] = columnName == null ? columnID.ToString() : columnName;
            AllColumnNames[(int)typeIndex].TArray = columnNamesForType;


            int columnIDToDenseIndexMapLength = ColumnIDToDenseIndexMap?.Length ?? 0;
            if (columnID >= columnIDToDenseIndexMapLength)
            {
                int newSize = columnIDToDenseIndexMapLength * 2;
                newSize = newSize == 0 ? 1 : newSize;
                Array.Resize(ref ColumnIDToDenseIndexMap, newSize);
                for (int i = columnIDToDenseIndexMapLength; i < newSize; i++)
                {
                    ref ColumnEntry entry = ref ColumnIDToDenseIndexMap[i];
                    entry.ColumnDenseIndex = i + 1;
                    entry.ColumnType = Serializable.SerializableTypes.Invalid;
                }

                Array.Resize(ref ColumnIDToSortOrderMap, newSize);
                for (int i = columnIDToDenseIndexMapLength; i < newSize; i++)
                {
                    ColumnIDToSortOrderMap[i] = -1;
                }
            }

            ColumnEntriesFreeListHead = ColumnIDToDenseIndexMap[columnID].ColumnDenseIndex;

            ref int[] denseIndexToIDMap = ref ColumnDenseIndexToIDMap[(int)typeIndex].TArray;
            int denseIndexToIDMapLength = denseIndexToIDMap?.Length ?? 0;
            Array.Resize(ref denseIndexToIDMap, denseIndexToIDMapLength + 1);
            denseIndexToIDMap[denseIndexToIDMapLength] = columnID;

            ref ColumnEntry newEntry = ref ColumnIDToDenseIndexMap[columnID];
            newEntry.ColumnDenseIndex = denseIndexToIDMapLength;
            newEntry.ColumnType = typeIndex;

            int insertAtSortedIndex =
                insertAtColumnID < 0 ? CombinedColumnCount : ColumnIDToSortOrderMap[insertAtColumnID];
            Array.Resize(ref SortedOrderToColumnIDMap, CombinedColumnCount + 1);
            for (int i = CombinedColumnCount; i > insertAtSortedIndex; i--)
            {
                int currentColumnID = SortedOrderToColumnIDMap[i - 1];
                SortedOrderToColumnIDMap[i] = currentColumnID;
                ColumnIDToSortOrderMap[currentColumnID] = i;
            }

            if (typeIndex == Serializable.SerializableTypes.Object)
            {
                AddTypeNameEntryForUnityObjectColumn();
            }

            ColumnIDToSortOrderMap[columnID] = insertAtSortedIndex;
            SortedOrderToColumnIDMap[insertAtSortedIndex] = columnID;

            ++CombinedColumnCount;
            DataVersion++;

            return columnID;
        }

        internal void RemoveColumnInternal<T>(ref ArrayHolder<T>[] allColumnsOfType,
            Serializable.SerializableTypes typeIndex, int columnID)
        {
            AssertColumnIDValid(columnID);
            int columnLocation = ColumnIDToDenseIndexMap[columnID].ColumnDenseIndex;

            int lastIndex = allColumnsOfType.Length - 1;
            allColumnsOfType[columnLocation] = allColumnsOfType[lastIndex];
            Array.Resize(ref allColumnsOfType, lastIndex);

            ref string[] columnNamesOfType = ref AllColumnNames[(int)typeIndex].TArray;
            columnNamesOfType[columnLocation] = columnNamesOfType[lastIndex];
            Array.Resize(ref columnNamesOfType, lastIndex);

            int columnOrder = ColumnIDToSortOrderMap[columnID];

            ref int[] denseIndicesOfType = ref ColumnDenseIndexToIDMap[(int)typeIndex].TArray;
            int sparseIndexToSwap = denseIndicesOfType[lastIndex];

            ColumnIDToDenseIndexMap[sparseIndexToSwap].ColumnDenseIndex = columnLocation;
            ref ColumnEntry sparseIndexToFree = ref ColumnIDToDenseIndexMap[columnID];
            sparseIndexToFree.ColumnType = Serializable.SerializableTypes.Invalid;
            sparseIndexToFree.ColumnDenseIndex = ColumnEntriesFreeListHead;

            ColumnEntriesFreeListHead = columnID;

            denseIndicesOfType[columnLocation] = sparseIndexToSwap;
            Array.Resize(ref denseIndicesOfType, lastIndex);

            if (typeIndex == Serializable.SerializableTypes.Object)
            {
                RemoveTypeNameEntryForUnityObjectColumn(columnLocation);
            }

            for (int i = columnOrder + 1; i < CombinedColumnCount; i++)
            {
                int currentColumnID = SortedOrderToColumnIDMap[i];
                SortedOrderToColumnIDMap[i - 1] = currentColumnID;
                ColumnIDToSortOrderMap[currentColumnID] = i - 1;
            }

            ColumnIDToSortOrderMap[columnID] = -1;

            Array.Resize(ref SortedOrderToColumnIDMap, CombinedColumnCount - 1);

            --CombinedColumnCount;
            DataVersion++;
        }

        internal void InsertRowsOfTypeInternal<T>(ref ArrayHolder<T>[] allColumnsOfType, int insertAt,
            int numberOfNewRows)
        {
            int columnCount = allColumnsOfType?.Length ?? 0;
            for (int i = 0; i < columnCount; i++)
            {
                ref T[] rows = ref allColumnsOfType[i].TArray;
                int newRowCount = RowCount + numberOfNewRows;
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

        internal void DeleteRowsOfTypeInternal<T>(ref ArrayHolder<T>[] allColumnsOfType, int removeAt,
            int numberOfRowsToDelete)
        {
            int columnCount = allColumnsOfType?.Length ?? 0;

            for (int i = 0; i < columnCount; i++)
            {
                ref T[] rows = ref allColumnsOfType[i].TArray;
                int newRowCount = RowCount - numberOfRowsToDelete;

                for (int j = removeAt + numberOfRowsToDelete; j < RowCount; j++)
                {
                    rows[j - numberOfRowsToDelete] = rows[j];
                }

                Array.Resize(ref rows, newRowCount);
            }
        }

        internal ref T GetCellRef<T>(int rowID, int columnID, ref ArrayHolder<T>[] allColumnsOfType)
        {
            AssertColumnIDValid(columnID);
            AssertRowIDValid(rowID);
            int column = ColumnIDToDenseIndexMap[columnID].ColumnDenseIndex;
            int row = RowIDToDenseIndexMap[rowID];
            return ref allColumnsOfType[column][row];
        }

        internal T GetCell<T>(int rowID, int columnID, ref ArrayHolder<T>[] allColumnsOfType)
        {
            AssertColumnIDValid(columnID);
            AssertRowIDValid(rowID);
            int column = ColumnIDToDenseIndexMap[columnID].ColumnDenseIndex;
            int row = RowIDToDenseIndexMap[rowID];
            return allColumnsOfType[column][row];
        }

        internal ulong SetCell<T>(int rowID, int columnID, ref ArrayHolder<T>[] allColumnsOfType, T value)
        {
            AssertColumnIDValid(columnID);
            AssertRowIDValid(rowID);
            int column = ColumnIDToDenseIndexMap[columnID].ColumnDenseIndex;
            int row = RowIDToDenseIndexMap[rowID];
            allColumnsOfType[column][row] = value;
            DataVersion++;
            return DataVersion;
        }

        internal T[] GetColumn<T>(int columnID, ref ArrayHolder<T>[] allColumnsOfType)
        {
            AssertColumnIDValid(columnID);
            int column = ColumnIDToDenseIndexMap[columnID].ColumnDenseIndex;
            return allColumnsOfType[column].TArray;
        }

        internal void SetRowOrderForColumns<T>(ArrayHolder<T>[] columns, int oldSortOrder, int newSortOrder)
        {
            int columnCount = columns?.Length ?? 0;
            int iterDirection = newSortOrder > oldSortOrder ? 1 : -1;
            for (int i = 0; i < columnCount; i++)
            {
                T[] column = columns[i].TArray;

                for (int j = oldSortOrder; j != newSortOrder; j += iterDirection)
                {
                    column[j] = column[j + iterDirection];
                }
            }
        }

        internal void AssertSortedColumnsArgValid(int[] sortedColumnIDs)
        {
            if (sortedColumnIDs == null)
            {
                throw new ArgumentException("sortedColumnIDs array cannot be null.");
            }

            if (sortedColumnIDs.Length != SortedOrderToColumnIDMap.Length)
            {
                throw new ArgumentException("sortedColumnIDs array must be the same length as GetColumnCount.");
            }

            for (int i = 0; i < sortedColumnIDs.Length; i++)
            {
                AssertColumnIDValid(sortedColumnIDs[i]);
            }
        }

        internal void AssertColumnSortOrderValid(int sortedOrder)
        {
            if (sortedOrder >= CombinedColumnCount || sortedOrder < 0)
            {
                throw new ArgumentException("Invalid column sort order argument: " + sortedOrder);
            }
        }

        internal void AssertRowSortOrderValid(int sortedOrder)
        {
            if (sortedOrder >= RowCount || sortedOrder < 0)
            {
                throw new ArgumentException("Invalid row sort order argument: " + sortedOrder);
            }
        }

        internal void AssertSortRowsArgValid(int[] sortedRowIDs)
        {
            if (sortedRowIDs == null)
            {
                throw new ArgumentException("sortedRowIDs array cannot be null.");
            }

            if (sortedRowIDs.Length != RowDenseIndexToIDMap.Length)
            {
                throw new ArgumentException("sortedRowIDs array must be the same length as GetRowCount.");
            }

            for (int i = 0; i < sortedRowIDs.Length; i++)
            {
                AssertRowIDValid(sortedRowIDs[i]);
            }
        }

        [Serializable]
        internal struct ColumnEntry
        {
            public Serializable.SerializableTypes ColumnType;
            public int ColumnDenseIndex;
        }
    }
}