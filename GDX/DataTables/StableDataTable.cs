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

#pragma warning disable IDE1006
        // ReSharper disable InconsistentNaming
        [SerializeField] internal string m_DisplayName = "GDXStableTable";
        [SerializeField] internal ArrayHolder<string>[] m_AllStringColumns;
        [SerializeField] internal ArrayHolder<bool>[] m_AllBoolColumns;
        [SerializeField] internal ArrayHolder<char>[] m_AllCharColumns;
        [SerializeField] internal ArrayHolder<sbyte>[] m_AllSByteColumns;
        [SerializeField] internal ArrayHolder<byte>[] m_AllByteColumns;
        [SerializeField] internal ArrayHolder<short>[] m_AllShortColumns;
        [SerializeField] internal ArrayHolder<ushort>[] m_AllUShortColumns;
        [SerializeField] internal ArrayHolder<int>[] m_AllIntColumns;
        [SerializeField] internal ArrayHolder<uint>[] m_AllUIntColumns;
        [SerializeField] internal ArrayHolder<long>[] m_AllLongColumns;
        [SerializeField] internal ArrayHolder<ulong>[] m_AllULongColumns;
        [SerializeField] internal ArrayHolder<float>[] m_AllFloatColumns;
        [SerializeField] internal ArrayHolder<double>[] m_AllDoubleColumns;
        [SerializeField] internal ArrayHolder<Vector2>[] m_AllVector2Columns;
        [SerializeField] internal ArrayHolder<Vector3>[] m_AllVector3Columns;
        [SerializeField] internal ArrayHolder<Vector4>[] m_AllVector4Columns;
        [SerializeField] internal ArrayHolder<Vector2Int>[] m_AllVector2IntColumns;
        [SerializeField] internal ArrayHolder<Vector3Int>[] m_AllVector3IntColumns;
        [SerializeField] internal ArrayHolder<Quaternion>[] m_AllQuaternionColumns;
        [SerializeField] internal ArrayHolder<Rect>[] m_AllRectColumns;
        [SerializeField] internal ArrayHolder<RectInt>[] m_AllRectIntColumns;
        [SerializeField] internal ArrayHolder<Color>[] m_AllColorColumns;
        [SerializeField] internal ArrayHolder<LayerMask>[] m_AllLayerMaskColumns;
        [SerializeField] internal ArrayHolder<Bounds>[] m_AllBoundsColumns;
        [SerializeField] internal ArrayHolder<BoundsInt>[] m_AllBoundsIntColumns;
        [SerializeField] internal ArrayHolder<Hash128>[] m_AllHash128Columns;
        [SerializeField] internal ArrayHolder<Gradient>[] m_AllGradientColumns;
        [SerializeField] internal ArrayHolder<AnimationCurve>[] m_AllAnimationCurveColumns;
        [SerializeField] internal ArrayHolder<Object>[] m_AllObjectRefColumns;
        [SerializeField] internal string[] m_AllObjectRefTypeNames;

        /// <summary>
        ///     Contains the name of each column of each type. Ordered by Serializable.SerializableTypes
        /// </summary>
        [SerializeField] internal ArrayHolder<string>[] m_AllColumnNames = new ArrayHolder<string>[Serializable.SerializableTypesCount];
        [SerializeField] internal int[] m_RowIdentifierToDenseIndexMap;
        [SerializeField] internal int[] m_RowDenseIndexToIDMap;
        [SerializeField] internal string[] m_RowNames;
        [SerializeField] internal int m_RowEntriesFreeListHead;
        [SerializeField] internal int m_RowCount;
        [SerializeField] internal ColumnEntry[] m_ColumnIdentifierToDenseIndexMap;
        [SerializeField] internal int[] m_ColumnIdentifierToSortOrderMap;
        [SerializeField] internal int[] m_SortedOrderToColumnIdentifierMap;

        // TODO @adam move with other block
        [SerializeField] ArrayHolder<int>[] m_ColumnDenseIndexToIDMap = new ArrayHolder<int>[Serializable.SerializableTypesCount];

        [SerializeField] internal int m_ColumnEntriesFreeListHead;
        [SerializeField] internal int m_CombinedColumnCount;
        [SerializeField] internal ulong m_DataVersion = 1;
        [SerializeField] internal BitArray8 m_SettingsFlags;
        // ReSharper enable InconsistentNaming
#pragma warning restore IDE1006


        /// <inheritdoc />
        public override ulong GetDataVersion()
        {
            return m_DataVersion;
        }

        /// <inheritdoc />
        public override int GetColumnCount()
        {
            return m_CombinedColumnCount;
        }

        /// <inheritdoc />
        public override int GetRowCount()
        {
            return m_RowCount;
        }

        /// <inheritdoc />
        public override string GetDisplayName()
        {
            return m_DisplayName;
        }

        /// <inheritdoc />
        public override void SetDisplayName(string displayName)
        {
            m_DisplayName = displayName;
        }

        /// <inheritdoc />
        public override bool GetFlag(Settings setting)
        {
            return m_SettingsFlags[(byte)setting];
        }

        /// <inheritdoc />
        public override void SetFlag(Settings setting, bool toggle)
        {
            m_SettingsFlags[(byte)setting] = toggle;
        }

        /// <inheritdoc />
        public override RowDescription[] GetAllRowDescriptions()
        {
            if (m_CombinedColumnCount == 0 || m_RowCount == 0)
            {
                return null;
            }

            RowDescription[] returnArray = new RowDescription[m_RowCount];
            for (int i = 0; i < m_RowCount; i++)
            {
                returnArray[i].Identifier = m_RowDenseIndexToIDMap[i];
                returnArray[i].Name = m_RowNames[i];
            }

            return returnArray;
        }

        /// <inheritdoc />
        public override RowDescription GetRowDescription(string name)
        {
            for (int i = 0; i < m_RowCount; i++)
            {
                string nameAt = m_RowNames[i];

                if (nameAt == name)
                {
                    return new RowDescription { Identifier = m_RowDenseIndexToIDMap[i], Name = nameAt };
                }
            }

            throw new ArgumentException("Row with name " + name + " does not exist in the table");
        }

        /// <inheritdoc />
        public override RowDescription GetRowDescription(int order)
        {
            return new RowDescription { Identifier = m_RowDenseIndexToIDMap[order], Name = m_RowNames[order] };
        }

        /// <inheritdoc />
        public override void SetAllRowDescriptionsOrder(RowDescription[] orderedRows)
        {
            // TODO: @adam array coming in be in the new order, just use the rowIdentifier (stable to reorder inside here
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override ColumnDescription GetColumnDescription(string name)
        {
            for (int i = 0; i < Serializable.SerializableTypesCount; i++)
            {
                string[] columnNames = m_AllColumnNames[i].TArray;

                if (columnNames != null)
                {
                    for (int j = 0; j < columnNames.Length; j++)
                    {
                        string nameAt = columnNames[j];

                        if (name == nameAt)
                        {
                            int columnID = m_ColumnDenseIndexToIDMap[i].TArray[j];

                            ref ColumnEntry columnEntry = ref m_ColumnIdentifierToDenseIndexMap[columnID];
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
            int idAtOrderedIndex = m_SortedOrderToColumnIdentifierMap[order];
            ref ColumnEntry columnEntry = ref m_ColumnIdentifierToDenseIndexMap[idAtOrderedIndex];

            string columnName = m_AllColumnNames[(int)columnEntry.ColumnType][columnEntry.ColumnDenseIndex];

            return new ColumnDescription
            {
                Identifier = idAtOrderedIndex, Name = columnName, Type = columnEntry.ColumnType
            };
        }

        /// <inheritdoc />
        public override void SetAllColumnDescriptionsOrder(ColumnDescription[] orderedColumns)
        {
            // TODO: @adam array coming in be in the new order, just use the columnIdentifier (stable to reorder inside here
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override ColumnDescription[] GetAllColumnDescriptions()
        {
            if (m_CombinedColumnCount == 0)
            {
                return null;
            }

            ColumnDescription[] returnArray = new ColumnDescription[m_CombinedColumnCount];

            for (int i = 0; i < m_CombinedColumnCount; i++)
            {
                int columnID = m_SortedOrderToColumnIdentifierMap[i];
                AssertColumnIDValid(columnID);
                ref ColumnEntry entryForID = ref m_ColumnIdentifierToDenseIndexMap[columnID];
                ref ArrayHolder<string> nameColumnsForType = ref m_AllColumnNames[(int)entryForID.ColumnType];

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
            if (columnID < 0 || columnID >= m_ColumnIdentifierToDenseIndexMap.Length)
            {
                throw new ArgumentException("Invalid column outside valid ID range: " + columnID);
            }

            ref ColumnEntry columnEntry = ref m_ColumnIdentifierToDenseIndexMap[columnID];

            if (columnEntry.ColumnType == Serializable.SerializableTypes.Invalid)
            {
                throw new ArgumentException("Invalid column pointing to deallocated entry: " + columnID);
            }
        }

        internal void AssertRowIDValid(int rowID)
        {
            if (rowID < 0 || rowID >= m_RowIdentifierToDenseIndexMap.Length)
            {
                throw new ArgumentException("Invalid row outside valid ID range: " + rowID);
            }

            int rowIndex = m_RowIdentifierToDenseIndexMap[rowID];

            if (rowIndex >= m_RowCount || rowIndex < 0)
            {
                throw new ArgumentException("Invalid row outside valid ID range: " + rowID);
            }
        }

        /// <inheritdoc />
        public override void SetColumnName(string columnName, int columnIdentifier)
        {
            AssertColumnIDValid(columnIdentifier);
            ref ColumnEntry columnEntry = ref m_ColumnIdentifierToDenseIndexMap[columnIdentifier];
            m_AllColumnNames[(int)columnEntry.ColumnType][columnEntry.ColumnDenseIndex] = columnName;
        }

        /// <inheritdoc />
        public override string GetColumnName(int columnIdentifier)
        {
            AssertColumnIDValid(columnIdentifier);
            ref ColumnEntry columnEntry = ref m_ColumnIdentifierToDenseIndexMap[columnIdentifier];
            return m_AllColumnNames[(int)columnEntry.ColumnType][columnEntry.ColumnDenseIndex];
        }


        /// <inheritdoc />
        public override void SetRowName(string rowName, int rowIdentifier)
        {
            AssertRowIDValid(rowIdentifier);
            int rowDenseIndex = m_RowIdentifierToDenseIndexMap[rowIdentifier];
            m_RowNames[rowDenseIndex] = rowName;
        }

        /// <inheritdoc />
        public override string GetRowName(int rowIdentifier)
        {
            AssertRowIDValid(rowIdentifier);
            int rowDenseIndex = m_RowIdentifierToDenseIndexMap[rowIdentifier];
            return m_RowNames[rowDenseIndex];
        }

        public ref string GetRowNameRef(int row)
        {
            AssertRowIDValid(row);
            int rowDenseIndex = m_RowIdentifierToDenseIndexMap[row];
            return ref m_RowNames[rowDenseIndex];
        }

        public ref string GetColumnNameRef(int columnID)
        {
            AssertColumnIDValid(columnID);
            ref ColumnEntry columnEntry = ref m_ColumnIdentifierToDenseIndexMap[columnID];
            return ref m_AllColumnNames[(int)columnEntry.ColumnType][columnEntry.ColumnDenseIndex];
        }

        /// <inheritdoc />
        public override int AddRow(string rowName = null, int insertAtRowID = -1)
        {
            if (insertAtRowID >= 0)
            {
                AssertRowIDValid(insertAtRowID);
            }

            int rowID = m_RowEntriesFreeListHead;
            int rowIDToDenseIndexMapLength = m_RowIdentifierToDenseIndexMap?.Length ?? 0;
            if (rowID >= rowIDToDenseIndexMapLength)
            {
                int newSize = rowID * 2;
                newSize = newSize == 0 ? 1 : newSize;
                Array.Resize(ref m_RowIdentifierToDenseIndexMap, newSize);
                for (int i = rowID; i < newSize; i++)
                {
                    m_RowIdentifierToDenseIndexMap[i] = i + 1;
                }
            }

            int denseIndexToIDMapLength = m_RowDenseIndexToIDMap?.Length ?? 0;
            Array.Resize(ref m_RowDenseIndexToIDMap, denseIndexToIDMapLength + 1);
            Array.Resize(ref m_RowNames, denseIndexToIDMapLength + 1);

            int insertAt = insertAtRowID < 0 ? m_RowCount : m_RowIdentifierToDenseIndexMap[insertAtRowID];

            for (int i = denseIndexToIDMapLength; i > insertAt; i--)
            {
                int currentRowID = m_RowDenseIndexToIDMap[i - 1];
                m_RowDenseIndexToIDMap[i] = currentRowID;

                m_RowIdentifierToDenseIndexMap[currentRowID] = i;

                m_RowNames[i] = m_RowNames[i - 1];
            }

            m_RowEntriesFreeListHead = m_RowIdentifierToDenseIndexMap[rowID];
            m_RowIdentifierToDenseIndexMap[rowID] = insertAt;
            m_RowDenseIndexToIDMap[insertAt] = rowID;
            m_RowNames[insertAt] = rowName == null ? rowID.ToString() : rowName;

            InsertRowsOfTypeInternal(ref m_AllStringColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref m_AllBoolColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref m_AllCharColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref m_AllSByteColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref m_AllByteColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref m_AllShortColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref m_AllUShortColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref m_AllIntColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref m_AllUIntColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref m_AllLongColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref m_AllULongColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref m_AllFloatColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref m_AllDoubleColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref m_AllVector2Columns, insertAt, 1);
            InsertRowsOfTypeInternal(ref m_AllVector3Columns, insertAt, 1);
            InsertRowsOfTypeInternal(ref m_AllVector4Columns, insertAt, 1);
            InsertRowsOfTypeInternal(ref m_AllVector2IntColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref m_AllVector3IntColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref m_AllQuaternionColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref m_AllRectColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref m_AllRectIntColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref m_AllColorColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref m_AllLayerMaskColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref m_AllBoundsColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref m_AllBoundsIntColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref m_AllHash128Columns, insertAt, 1);
            InsertRowsOfTypeInternal(ref m_AllGradientColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref m_AllAnimationCurveColumns, insertAt, 1);
            InsertRowsOfTypeInternal(ref m_AllObjectRefColumns, insertAt, 1);

            ++m_RowCount;
            m_DataVersion++;

            return rowID;
        }

        public void AddRows(int numberOfNewRows, string[] rowNames = null, int insertAtRowID = -1)
        {
            if (insertAtRowID >= 0)
            {
                AssertRowIDValid(insertAtRowID);
            }

            int rowIDToDenseIndexMapLength = m_RowIdentifierToDenseIndexMap?.Length ?? 0;
            int newCount = m_RowCount + numberOfNewRows;
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
                Array.Resize(ref m_RowIdentifierToDenseIndexMap, newSize);
                for (int i = rowIDToDenseIndexMapLength; i < newSize; i++)
                {
                    m_RowIdentifierToDenseIndexMap[i] = i + 1;
                }
            }

            int denseIndexToIDMapLength = m_RowDenseIndexToIDMap?.Length ?? 0;
            Array.Resize(ref m_RowDenseIndexToIDMap, denseIndexToIDMapLength + numberOfNewRows);
            Array.Resize(ref rowNames, denseIndexToIDMapLength + numberOfNewRows);

            int insertAt = insertAtRowID < 0 ? m_RowCount : m_RowIdentifierToDenseIndexMap[insertAtRowID];

            for (int i = denseIndexToIDMapLength; i > insertAt + numberOfNewRows - 1; i--)
            {
                int currentRowID = m_RowDenseIndexToIDMap[i - numberOfNewRows];
                m_RowDenseIndexToIDMap[i] = currentRowID;

                m_RowIdentifierToDenseIndexMap[currentRowID] = i;

                rowNames[i] = rowNames[i - numberOfNewRows];
            }

            int freeListHead = m_RowEntriesFreeListHead;

            for (int i = 0; i < numberOfNewRows; i++)
            {
                int rowID = freeListHead;
                freeListHead = m_RowIdentifierToDenseIndexMap[rowID];
                m_RowIdentifierToDenseIndexMap[rowID] = insertAt + i;
                m_RowDenseIndexToIDMap[insertAt + i] = rowID;
            }

            int numberOfNewRowNames = rowNames?.Length ?? 0;
            string emptyString = string.Empty;
            for (int i = 0; i < numberOfNewRowNames; i++)
            {
                string currentRowName = rowNames[i];
                int rowIDAt = m_RowDenseIndexToIDMap[insertAt + i];
                rowNames[insertAt + i] = currentRowName == null ? rowIDAt.ToString() : currentRowName;
            }

            for (int i = numberOfNewRowNames; i < numberOfNewRows; i++)
            {
                int rowIDAt = m_RowDenseIndexToIDMap[insertAt + i];
                rowNames[insertAt + i] = rowIDAt.ToString();
            }

            m_RowEntriesFreeListHead = freeListHead;

            InsertRowsOfTypeInternal(ref m_AllStringColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllBoolColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllCharColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllSByteColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllByteColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllShortColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllUShortColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllIntColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllUIntColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllLongColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllULongColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllFloatColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllDoubleColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllVector2Columns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllVector3Columns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllVector4Columns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllVector2IntColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllVector3IntColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllQuaternionColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllRectColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllRectIntColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllColorColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllLayerMaskColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllBoundsColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllBoundsIntColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllHash128Columns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllGradientColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllAnimationCurveColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllObjectRefColumns, insertAt, numberOfNewRows);

            m_RowCount += numberOfNewRows;
            m_DataVersion++;
        }

        public void AddRows(int numberOfNewRows, ref int[] rowIDs, string[] rowNames = null, int insertAtRowID = -1)
        {
            if (insertAtRowID >= 0)
            {
                AssertRowIDValid(insertAtRowID);
            }

            int rowIDToDenseIndexMapLength = m_RowIdentifierToDenseIndexMap?.Length ?? 0;
            int newCount = m_RowCount + numberOfNewRows;
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
                Array.Resize(ref m_RowIdentifierToDenseIndexMap, newSize);
                for (int i = rowIDToDenseIndexMapLength; i < newSize; i++)
                {
                    m_RowIdentifierToDenseIndexMap[i] = i + 1;
                }
            }

            int denseIndexToIDMapLength = m_RowDenseIndexToIDMap?.Length ?? 0;
            Array.Resize(ref m_RowDenseIndexToIDMap, denseIndexToIDMapLength + numberOfNewRows);

            int insertAt = insertAtRowID < 0 ? m_RowCount : m_RowIdentifierToDenseIndexMap[insertAtRowID];

            for (int i = denseIndexToIDMapLength; i > insertAt + numberOfNewRows - 1; i--)
            {
                int currentRowID = m_RowDenseIndexToIDMap[i - numberOfNewRows];
                m_RowDenseIndexToIDMap[i] = currentRowID;

                m_RowIdentifierToDenseIndexMap[currentRowID] = i;

                rowNames[i] = rowNames[i - numberOfNewRows];
            }

            int freeListHead = m_RowEntriesFreeListHead;

            for (int i = 0; i < numberOfNewRows; i++)
            {
                int rowID = freeListHead;
                freeListHead = m_RowIdentifierToDenseIndexMap[rowID];
                m_RowIdentifierToDenseIndexMap[rowID] = insertAt + i;
                m_RowDenseIndexToIDMap[insertAt + i] = rowID;
                rowIDs[i] = rowID;
            }

            int numberOfNewRowNames = rowNames?.Length ?? 0;
            for (int i = 0; i < numberOfNewRowNames; i++)
            {
                string currentRowName = rowNames[i];
                int rowIDAt = m_RowDenseIndexToIDMap[insertAt + i];
                rowNames[insertAt + i] = currentRowName == null ? rowIDAt.ToString() : currentRowName;
            }

            for (int i = numberOfNewRowNames; i < numberOfNewRows; i++)
            {
                int rowIDAt = m_RowDenseIndexToIDMap[insertAt + i];
                rowNames[insertAt + i] = rowIDAt.ToString();
            }

            m_RowEntriesFreeListHead = freeListHead;

            InsertRowsOfTypeInternal(ref m_AllStringColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllBoolColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllCharColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllSByteColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllByteColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllShortColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllUShortColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllIntColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllUIntColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllLongColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllULongColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllFloatColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllDoubleColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllVector2Columns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllVector3Columns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllVector4Columns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllVector2IntColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllVector3IntColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllQuaternionColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllRectColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllRectIntColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllColorColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllLayerMaskColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllBoundsColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllBoundsIntColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllHash128Columns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllGradientColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllAnimationCurveColumns, insertAt, numberOfNewRows);
            InsertRowsOfTypeInternal(ref m_AllObjectRefColumns, insertAt, numberOfNewRows);

            m_RowCount += numberOfNewRows;
            m_DataVersion++;
        }

        /// <inheritdoc />
        public override void RemoveRow(int rowID)
        {
            AssertRowIDValid(rowID);
            int rowDenseIndex = m_RowIdentifierToDenseIndexMap[rowID];
            for (int i = rowDenseIndex + 1; i < m_RowCount; i++)
            {
                int currentRowID = m_RowDenseIndexToIDMap[i];
                m_RowIdentifierToDenseIndexMap[currentRowID] = i - 1;
                m_RowDenseIndexToIDMap[i - 1] = currentRowID;
                m_RowNames[i - 1] = m_RowNames[i];
            }

            m_RowIdentifierToDenseIndexMap[rowID] = m_RowEntriesFreeListHead;
            m_RowEntriesFreeListHead = rowID;
            Array.Resize(ref m_RowDenseIndexToIDMap, m_RowCount - 1);
            Array.Resize(ref m_RowNames, m_RowCount - 1);

            DeleteRowsOfTypeInternal(ref m_AllStringColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref m_AllBoolColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref m_AllCharColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref m_AllSByteColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref m_AllByteColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref m_AllShortColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref m_AllUShortColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref m_AllIntColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref m_AllUIntColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref m_AllLongColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref m_AllULongColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref m_AllFloatColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref m_AllDoubleColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref m_AllVector2Columns, rowID, 1);
            DeleteRowsOfTypeInternal(ref m_AllVector3Columns, rowID, 1);
            DeleteRowsOfTypeInternal(ref m_AllVector4Columns, rowID, 1);
            DeleteRowsOfTypeInternal(ref m_AllVector2IntColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref m_AllVector3IntColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref m_AllQuaternionColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref m_AllRectColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref m_AllRectIntColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref m_AllColorColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref m_AllLayerMaskColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref m_AllBoundsColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref m_AllBoundsIntColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref m_AllHash128Columns, rowID, 1);
            DeleteRowsOfTypeInternal(ref m_AllGradientColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref m_AllAnimationCurveColumns, rowID, 1);
            DeleteRowsOfTypeInternal(ref m_AllObjectRefColumns, rowID, 1);

            --m_RowCount;
            m_DataVersion++;
        }

        /// <inheritdoc />
        public override int AddColumn(Serializable.SerializableTypes columnType, string columnName,
            int insertAtColumnID = -1)
        {
            switch (columnType)
            {
                case Serializable.SerializableTypes.String:
                    return AddColumnInternal(columnName, ref m_AllStringColumns, Serializable.SerializableTypes.String,
                        insertAtColumnID);
                case Serializable.SerializableTypes.Char:
                    return AddColumnInternal(columnName, ref m_AllCharColumns, Serializable.SerializableTypes.Char,
                        insertAtColumnID);
                case Serializable.SerializableTypes.Bool:
                    return AddColumnInternal(columnName, ref m_AllBoolColumns, Serializable.SerializableTypes.Bool,
                        insertAtColumnID);
                case Serializable.SerializableTypes.SByte:
                    return AddColumnInternal(columnName, ref m_AllSByteColumns, Serializable.SerializableTypes.SByte,
                        insertAtColumnID);
                case Serializable.SerializableTypes.Byte:
                    return AddColumnInternal(columnName, ref m_AllByteColumns, Serializable.SerializableTypes.Byte,
                        insertAtColumnID);
                case Serializable.SerializableTypes.Short:
                    return AddColumnInternal(columnName, ref m_AllShortColumns, Serializable.SerializableTypes.Short,
                        insertAtColumnID);
                case Serializable.SerializableTypes.UShort:
                    return AddColumnInternal(columnName, ref m_AllUShortColumns, Serializable.SerializableTypes.UShort,
                        insertAtColumnID);
                case Serializable.SerializableTypes.Int:
                    return AddColumnInternal(columnName, ref m_AllIntColumns, Serializable.SerializableTypes.Int,
                        insertAtColumnID);
                case Serializable.SerializableTypes.UInt:
                    return AddColumnInternal(columnName, ref m_AllUIntColumns, Serializable.SerializableTypes.UInt,
                        insertAtColumnID);
                case Serializable.SerializableTypes.Long:
                    return AddColumnInternal(columnName, ref m_AllLongColumns, Serializable.SerializableTypes.Long,
                        insertAtColumnID);
                case Serializable.SerializableTypes.ULong:
                    return AddColumnInternal(columnName, ref m_AllULongColumns, Serializable.SerializableTypes.ULong,
                        insertAtColumnID);
                case Serializable.SerializableTypes.Float:
                    return AddColumnInternal(columnName, ref m_AllFloatColumns, Serializable.SerializableTypes.Float,
                        insertAtColumnID);
                case Serializable.SerializableTypes.Double:
                    return AddColumnInternal(columnName, ref m_AllDoubleColumns, Serializable.SerializableTypes.Double,
                        insertAtColumnID);
                case Serializable.SerializableTypes.Vector2:
                    return AddColumnInternal(columnName, ref m_AllVector2Columns, Serializable.SerializableTypes.Vector2,
                        insertAtColumnID);
                case Serializable.SerializableTypes.Vector3:
                    return AddColumnInternal(columnName, ref m_AllVector3Columns, Serializable.SerializableTypes.Vector3,
                        insertAtColumnID);
                case Serializable.SerializableTypes.Vector4:
                    return AddColumnInternal(columnName, ref m_AllVector4Columns, Serializable.SerializableTypes.Vector4,
                        insertAtColumnID);
                case Serializable.SerializableTypes.Vector2Int:
                    return AddColumnInternal(columnName, ref m_AllVector2IntColumns,
                        Serializable.SerializableTypes.Vector2Int, insertAtColumnID);
                case Serializable.SerializableTypes.Vector3Int:
                    return AddColumnInternal(columnName, ref m_AllVector3IntColumns,
                        Serializable.SerializableTypes.Vector3Int, insertAtColumnID);
                case Serializable.SerializableTypes.Quaternion:
                    return AddColumnInternal(columnName, ref m_AllQuaternionColumns,
                        Serializable.SerializableTypes.Quaternion, insertAtColumnID);
                case Serializable.SerializableTypes.Rect:
                    return AddColumnInternal(columnName, ref m_AllRectColumns, Serializable.SerializableTypes.Rect,
                        insertAtColumnID);
                case Serializable.SerializableTypes.RectInt:
                    return AddColumnInternal(columnName, ref m_AllRectIntColumns, Serializable.SerializableTypes.RectInt,
                        insertAtColumnID);
                case Serializable.SerializableTypes.Color:
                    return AddColumnInternal(columnName, ref m_AllColorColumns, Serializable.SerializableTypes.Color,
                        insertAtColumnID);
                case Serializable.SerializableTypes.LayerMask:
                    return AddColumnInternal(columnName, ref m_AllLayerMaskColumns,
                        Serializable.SerializableTypes.LayerMask, insertAtColumnID);
                case Serializable.SerializableTypes.Bounds:
                    return AddColumnInternal(columnName, ref m_AllBoundsColumns, Serializable.SerializableTypes.Bounds,
                        insertAtColumnID);
                case Serializable.SerializableTypes.BoundsInt:
                    return AddColumnInternal(columnName, ref m_AllBoundsIntColumns,
                        Serializable.SerializableTypes.BoundsInt, insertAtColumnID);
                case Serializable.SerializableTypes.Hash128:
                    return AddColumnInternal(columnName, ref m_AllHash128Columns, Serializable.SerializableTypes.Hash128,
                        insertAtColumnID);
                case Serializable.SerializableTypes.Gradient:
                    return AddColumnInternal(columnName, ref m_AllGradientColumns,
                        Serializable.SerializableTypes.Gradient, insertAtColumnID);
                case Serializable.SerializableTypes.AnimationCurve:
                    return AddColumnInternal(columnName, ref m_AllAnimationCurveColumns,
                        Serializable.SerializableTypes.AnimationCurve, insertAtColumnID);
                case Serializable.SerializableTypes.Object:
                    return AddColumnInternal(columnName, ref m_AllObjectRefColumns, Serializable.SerializableTypes.Object,
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
                    RemoveColumnInternal(ref m_AllStringColumns, Serializable.SerializableTypes.String, columnID);
                    break;
                case Serializable.SerializableTypes.Char:
                    RemoveColumnInternal(ref m_AllCharColumns, Serializable.SerializableTypes.Char, columnID);
                    break;
                case Serializable.SerializableTypes.Bool:
                    RemoveColumnInternal(ref m_AllBoolColumns, Serializable.SerializableTypes.Bool, columnID);
                    break;
                case Serializable.SerializableTypes.SByte:
                    RemoveColumnInternal(ref m_AllSByteColumns, Serializable.SerializableTypes.SByte, columnID);
                    break;
                case Serializable.SerializableTypes.Byte:
                    RemoveColumnInternal(ref m_AllByteColumns, Serializable.SerializableTypes.Byte, columnID);
                    break;
                case Serializable.SerializableTypes.Short:
                    RemoveColumnInternal(ref m_AllShortColumns, Serializable.SerializableTypes.Short, columnID);
                    break;
                case Serializable.SerializableTypes.UShort:
                    RemoveColumnInternal(ref m_AllUShortColumns, Serializable.SerializableTypes.UShort, columnID);
                    break;
                case Serializable.SerializableTypes.Int:
                    RemoveColumnInternal(ref m_AllIntColumns, Serializable.SerializableTypes.Int, columnID);
                    break;
                case Serializable.SerializableTypes.UInt:
                    RemoveColumnInternal(ref m_AllUIntColumns, Serializable.SerializableTypes.UInt, columnID);
                    break;
                case Serializable.SerializableTypes.Long:
                    RemoveColumnInternal(ref m_AllLongColumns, Serializable.SerializableTypes.Long, columnID);
                    break;
                case Serializable.SerializableTypes.ULong:
                    RemoveColumnInternal(ref m_AllULongColumns, Serializable.SerializableTypes.ULong, columnID);
                    break;
                case Serializable.SerializableTypes.Float:
                    RemoveColumnInternal(ref m_AllFloatColumns, Serializable.SerializableTypes.Float, columnID);
                    break;
                case Serializable.SerializableTypes.Double:
                    RemoveColumnInternal(ref m_AllDoubleColumns, Serializable.SerializableTypes.Double, columnID);
                    break;
                case Serializable.SerializableTypes.Vector2:
                    RemoveColumnInternal(ref m_AllVector2Columns, Serializable.SerializableTypes.Vector2, columnID);
                    break;
                case Serializable.SerializableTypes.Vector3:
                    RemoveColumnInternal(ref m_AllVector3Columns, Serializable.SerializableTypes.Vector3, columnID);
                    break;
                case Serializable.SerializableTypes.Vector4:
                    RemoveColumnInternal(ref m_AllVector4Columns, Serializable.SerializableTypes.Vector4, columnID);
                    break;
                case Serializable.SerializableTypes.Vector2Int:
                    RemoveColumnInternal(ref m_AllVector2IntColumns, Serializable.SerializableTypes.Vector2Int, columnID);
                    break;
                case Serializable.SerializableTypes.Vector3Int:
                    RemoveColumnInternal(ref m_AllVector3IntColumns, Serializable.SerializableTypes.Vector3Int, columnID);
                    break;
                case Serializable.SerializableTypes.Quaternion:
                    RemoveColumnInternal(ref m_AllQuaternionColumns, Serializable.SerializableTypes.Quaternion, columnID);
                    break;
                case Serializable.SerializableTypes.Rect:
                    RemoveColumnInternal(ref m_AllRectColumns, Serializable.SerializableTypes.Rect, columnID);
                    break;
                case Serializable.SerializableTypes.RectInt:
                    RemoveColumnInternal(ref m_AllRectIntColumns, Serializable.SerializableTypes.RectInt, columnID);
                    break;
                case Serializable.SerializableTypes.Color:
                    RemoveColumnInternal(ref m_AllColorColumns, Serializable.SerializableTypes.Color, columnID);
                    break;
                case Serializable.SerializableTypes.LayerMask:
                    RemoveColumnInternal(ref m_AllLayerMaskColumns, Serializable.SerializableTypes.LayerMask, columnID);
                    break;
                case Serializable.SerializableTypes.Bounds:
                    RemoveColumnInternal(ref m_AllBoundsColumns, Serializable.SerializableTypes.Bounds, columnID);
                    break;
                case Serializable.SerializableTypes.BoundsInt:
                    RemoveColumnInternal(ref m_AllBoundsIntColumns, Serializable.SerializableTypes.BoundsInt, columnID);
                    break;
                case Serializable.SerializableTypes.Hash128:
                    RemoveColumnInternal(ref m_AllHash128Columns, Serializable.SerializableTypes.Hash128, columnID);
                    break;
                case Serializable.SerializableTypes.Gradient:
                    RemoveColumnInternal(ref m_AllGradientColumns, Serializable.SerializableTypes.Gradient, columnID);
                    break;
                case Serializable.SerializableTypes.AnimationCurve:
                    RemoveColumnInternal(ref m_AllAnimationCurveColumns, Serializable.SerializableTypes.AnimationCurve,
                        columnID);
                    break;
                case Serializable.SerializableTypes.Object:
                    RemoveColumnInternal(ref m_AllObjectRefColumns, Serializable.SerializableTypes.Object, columnID);
                    break;
            }
        }

        // Set
        /// <inheritdoc />
        public override ulong SetString(int rowIdentifier, int columnIdentifier, string newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllStringColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetBool(int rowIdentifier, int columnIdentifier, bool newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllBoolColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetChar(int rowIdentifier, int columnIdentifier, char newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllCharColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetSByte(int rowIdentifier, int columnIdentifier, sbyte newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllSByteColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetByte(int rowIdentifier, int columnIdentifier, byte newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllByteColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetShort(int rowIdentifier, int columnIdentifier, short newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllShortColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetUShort(int rowIdentifier, int columnIdentifier, ushort newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllUShortColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetInt(int rowIdentifier, int columnIdentifier, int newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllIntColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetUInt(int rowIdentifier, int columnIdentifier, uint newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllUIntColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetLong(int rowIdentifier, int columnIdentifier, long newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllLongColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetULong(int rowIdentifier, int columnIdentifier, ulong newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllULongColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetFloat(int rowIdentifier, int columnIdentifier, float newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllFloatColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetDouble(int rowIdentifier, int columnIdentifier, double newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllDoubleColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetVector2(int rowIdentifier, int columnIdentifier, Vector2 newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllVector2Columns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetVector3(int rowIdentifier, int columnIdentifier, Vector3 newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllVector3Columns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetVector4(int rowIdentifier, int columnIdentifier, Vector4 value)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllVector4Columns, value);
        }

        /// <inheritdoc />
        public override ulong SetVector2Int(int rowIdentifier, int columnIdentifier, Vector2Int newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllVector2IntColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetVector3Int(int rowIdentifier, int columnIdentifier, Vector3Int newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllVector3IntColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetQuaternion(int rowIdentifier, int columnIdentifier, Quaternion newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllQuaternionColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetRect(int rowIdentifier, int columnIdentifier, Rect newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllRectColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetRectInt(int rowIdentifier, int columnIdentifier, RectInt newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllRectIntColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetColor(int rowIdentifier, int columnIdentifier, Color newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllColorColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetLayerMask(int rowIdentifier, int columnIdentifier, LayerMask newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllLayerMaskColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetBounds(int rowIdentifier, int columnIdentifier, Bounds newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllBoundsColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetBoundsInt(int rowIdentifier, int columnIdentifier, BoundsInt newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllBoundsIntColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetHash128(int rowIdentifier, int columnIdentifier, Hash128 newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllHash128Columns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetGradient(int rowIdentifier, int columnIdentifier, Gradient newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllGradientColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetAnimationCurve(int rowIdentifier, int columnIdentifier, AnimationCurve newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllAnimationCurveColumns, newValue);
        }

        /// <inheritdoc />
        public override ulong SetObject(int rowIdentifier, int columnIdentifier, Object newValue)
        {
            return SetCell(rowIdentifier, columnIdentifier, ref m_AllObjectRefColumns, newValue);
        }

        /// <inheritdoc />
        public override void SetTypeNameForObjectColumn(int columnIdentifier, string assemblyQualifiedName)
        {
            AssertObjectColumnIDValid(columnIdentifier);
            int denseIndex = m_ColumnIdentifierToDenseIndexMap[columnIdentifier].ColumnDenseIndex;
            m_AllObjectRefTypeNames[denseIndex] = assemblyQualifiedName;
        }

        /// <inheritdoc />
        public override string GetTypeNameForObjectColumn(int columnIdentifier)
        {
            AssertObjectColumnIDValid(columnIdentifier);
            int denseIndex = m_ColumnIdentifierToDenseIndexMap[columnIdentifier].ColumnDenseIndex;
            return m_AllObjectRefTypeNames[denseIndex];
        }

        // Get
        /// <inheritdoc />
        public override string GetString(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllStringColumns);
        }

        /// <inheritdoc />
        public override bool GetBool(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllBoolColumns);
        }

        /// <inheritdoc />
        public override char GetChar(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllCharColumns);
        }

        /// <inheritdoc />
        public override sbyte GetSByte(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllSByteColumns);
        }

        /// <inheritdoc />
        public override byte GetByte(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllByteColumns);
        }

        /// <inheritdoc />
        public override short GetShort(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllShortColumns);
        }

        /// <inheritdoc />
        public override ushort GetUShort(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllUShortColumns);
        }

        /// <inheritdoc />
        public override int GetInt(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllIntColumns);
        }

        /// <inheritdoc />
        public override uint GetUInt(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllUIntColumns);
        }

        /// <inheritdoc />
        public override long GetLong(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllLongColumns);
        }

        /// <inheritdoc />
        public override ulong GetULong(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllULongColumns);
        }

        /// <inheritdoc />
        public override float GetFloat(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllFloatColumns);
        }

        /// <inheritdoc />
        public override double GetDouble(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllDoubleColumns);
        }

        /// <inheritdoc />
        public override Vector2 GetVector2(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllVector2Columns);
        }

        /// <inheritdoc />
        public override Vector3 GetVector3(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllVector3Columns);
        }

        /// <inheritdoc />
        public override Vector4 GetVector4(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllVector4Columns);
        }

        /// <inheritdoc />
        public override Vector2Int GetVector2Int(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllVector2IntColumns);
        }

        /// <inheritdoc />
        public override Vector3Int GetVector3Int(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllVector3IntColumns);
        }

        /// <inheritdoc />
        public override Quaternion GetQuaternion(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllQuaternionColumns);
        }

        /// <inheritdoc />
        public override Rect GetRect(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllRectColumns);
        }

        /// <inheritdoc />
        public override RectInt GetRectInt(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllRectIntColumns);
        }

        /// <inheritdoc />
        public override Color GetColor(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllColorColumns);
        }

        /// <inheritdoc />
        public override LayerMask GetLayerMask(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllLayerMaskColumns);
        }

        /// <inheritdoc />
        public override Bounds GetBounds(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllBoundsColumns);
        }

        /// <inheritdoc />
        public override BoundsInt GetBoundsInt(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllBoundsIntColumns);
        }

        /// <inheritdoc />
        public override Hash128 GetHash128(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllHash128Columns);
        }

        /// <inheritdoc />
        public override Gradient GetGradient(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllGradientColumns);
        }

        /// <inheritdoc />
        public override AnimationCurve GetAnimationCurve(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllAnimationCurveColumns);
        }

        /// <inheritdoc />
        public override Object GetObject(int rowIdentifier, int columnIdentifier)
        {
            return GetCell(rowIdentifier, columnIdentifier, ref m_AllObjectRefColumns);
        }

        // Get ref

        public ref string GetStringRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref m_AllStringColumns);
        }

        public ref bool GetBoolRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref m_AllBoolColumns);
        }

        public ref char GetCharRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref m_AllCharColumns);
        }

        public ref sbyte GetSbyteRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref m_AllSByteColumns);
        }

        public ref byte GetByteRef(int row, int columnID)
        {
            return ref GetCellRef(row, columnID, ref m_AllByteColumns);
        }

        public ref short GetShortRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref m_AllShortColumns);
        }

        public ref ushort GetUshortRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref m_AllUShortColumns);
        }

        public ref int GetIntRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref m_AllIntColumns);
        }

        public ref uint GetUintRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref m_AllUIntColumns);
        }

        public ref long GetLongRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref m_AllLongColumns);
        }

        public ref ulong GetUlongRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref m_AllULongColumns);
        }

        public ref float GetFloatRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref m_AllFloatColumns);
        }

        public ref double GetDoubleRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref m_AllDoubleColumns);
        }

        public ref Vector2 GetVector2Ref(int row, int column)
        {
            return ref GetCellRef(row, column, ref m_AllVector2Columns);
        }

        public ref Vector3 GetVector3Ref(int row, int column)
        {
            return ref GetCellRef(row, column, ref m_AllVector3Columns);
        }

        public ref Vector4 GetVector4Ref(int row, int column)
        {
            return ref GetCellRef(row, column, ref m_AllVector4Columns);
        }

        public ref Vector2Int GetVector2IntRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref m_AllVector2IntColumns);
        }

        public ref Vector3Int GetVector3IntRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref m_AllVector3IntColumns);
        }

        public ref Quaternion GetQuaternionRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref m_AllQuaternionColumns);
        }

        public ref Rect GetRectRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref m_AllRectColumns);
        }

        public ref RectInt GetRectIntRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref m_AllRectIntColumns);
        }

        public ref Color GetColorRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref m_AllColorColumns);
        }

        public ref LayerMask GetLayerMaskRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref m_AllLayerMaskColumns);
        }

        public ref Bounds GetBoundsRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref m_AllBoundsColumns);
        }

        public ref BoundsInt GetBoundsIntRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref m_AllBoundsIntColumns);
        }

        public ref Hash128 GetHash128Ref(int row, int column)
        {
            return ref GetCellRef(row, column, ref m_AllHash128Columns);
        }

        public ref Gradient GetGradientRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref m_AllGradientColumns);
        }

        public ref AnimationCurve GetAnimationCurveRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref m_AllAnimationCurveColumns);
        }

        public ref Object GetObjectRef(int row, int column)
        {
            return ref GetCellRef(row, column, ref m_AllObjectRefColumns);
        }

        // Get Column

        public string[] GetStringColumn(int column)
        {
            return GetColumn(column, ref m_AllStringColumns);
        }

        public bool[] GetBoolColumn(int column)
        {
            return GetColumn(column, ref m_AllBoolColumns);
        }

        public char[] GetCharColumn(int column)
        {
            return GetColumn(column, ref m_AllCharColumns);
        }

        public sbyte[] GetSbyteColumn(int column)
        {
            return GetColumn(column, ref m_AllSByteColumns);
        }

        public byte[] GetByteColumn(int column)
        {
            return GetColumn(column, ref m_AllByteColumns);
        }

        public short[] GetShortColumn(int column)
        {
            return GetColumn(column, ref m_AllShortColumns);
        }

        public ushort[] GetUshortColumn(int column)
        {
            return GetColumn(column, ref m_AllUShortColumns);
        }

        public int[] GetIntColumn(int column)
        {
            return GetColumn(column, ref m_AllIntColumns);
        }

        public uint[] GetUintColumn(int column)
        {
            return GetColumn(column, ref m_AllUIntColumns);
        }

        public long[] GetLongColumn(int column)
        {
            return GetColumn(column, ref m_AllLongColumns);
        }

        public ulong[] GetUlongColumn(int column)
        {
            return GetColumn(column, ref m_AllULongColumns);
        }

        public float[] GetFloatColumn(int column)
        {
            return GetColumn(column, ref m_AllFloatColumns);
        }

        public double[] GetDoubleColumn(int column)
        {
            return GetColumn(column, ref m_AllDoubleColumns);
        }

        public Vector2[] GetVector2Column(int column)
        {
            return GetColumn(column, ref m_AllVector2Columns);
        }

        public Vector3[] GetVector3Column(int column)
        {
            return GetColumn(column, ref m_AllVector3Columns);
        }

        public Vector4[] GetVector4Column(int column)
        {
            return GetColumn(column, ref m_AllVector4Columns);
        }

        public Vector2Int[] GetVector2IntColumn(int column)
        {
            return GetColumn(column, ref m_AllVector2IntColumns);
        }

        public Vector3Int[] GetVector3IntColumn(int column)
        {
            return GetColumn(column, ref m_AllVector3IntColumns);
        }

        public Quaternion[] GetQuaternionColumn(int column)
        {
            return GetColumn(column, ref m_AllQuaternionColumns);
        }

        public Rect[] GetRectColumn(int column)
        {
            return GetColumn(column, ref m_AllRectColumns);
        }

        public RectInt[] GetRectIntColumn(int column)
        {
            return GetColumn(column, ref m_AllRectIntColumns);
        }

        public Color[] GetColorColumn(int column)
        {
            return GetColumn(column, ref m_AllColorColumns);
        }

        public LayerMask[] GetLayerMaskColumn(int column)
        {
            return GetColumn(column, ref m_AllLayerMaskColumns);
        }

        public Bounds[] GetBoundsColumn(int column)
        {
            return GetColumn(column, ref m_AllBoundsColumns);
        }

        public BoundsInt[] GetBoundsIntColumn(int column)
        {
            return GetColumn(column, ref m_AllBoundsIntColumns);
        }

        public Hash128[] GetHash128Column(int column)
        {
            return GetColumn(column, ref m_AllHash128Columns);
        }

        public Gradient[] GetGradientColumn(int column)
        {
            return GetColumn(column, ref m_AllGradientColumns);
        }

        public AnimationCurve[] GetAnimationCurveColumn(int column)
        {
            return GetColumn(column, ref m_AllAnimationCurveColumns);
        }

        public Object[] GetObjectColumn(int column)
        {
            return GetColumn(column, ref m_AllObjectRefColumns);
        }

        // SetOrder

        public void SetColumnOrder(int columnID, int newSortOrder)
        {
            AssertColumnIDValid(columnID);
            AssertColumnSortOrderValid(newSortOrder);
            int oldSortOrder = m_ColumnIdentifierToSortOrderMap[columnID];
            int iterDirection = newSortOrder > oldSortOrder ? 1 : -1;
            for (int i = oldSortOrder; i != newSortOrder; i += iterDirection)
            {
                int columnIDAt = m_SortedOrderToColumnIdentifierMap[i + iterDirection];
                m_ColumnIdentifierToSortOrderMap[columnIDAt] = i;
                m_SortedOrderToColumnIdentifierMap[i] = m_SortedOrderToColumnIdentifierMap[i + iterDirection];
            }

            m_SortedOrderToColumnIdentifierMap[newSortOrder] = columnID;
            m_ColumnIdentifierToSortOrderMap[columnID] = newSortOrder;
        }

        public void SetAllColumnOrders(int[] sortedColumnIDs)
        {
            AssertSortedColumnsArgValid(sortedColumnIDs);
            for (int i = 0; i < m_SortedOrderToColumnIdentifierMap.Length; i++)
            {
                int columnID = sortedColumnIDs[i];
                m_SortedOrderToColumnIdentifierMap[i] = columnID;
                m_ColumnIdentifierToSortOrderMap[columnID] = i;
            }
        }

        public void SetRowOrder(int rowID, int newSortOrder)
        {
            AssertRowIDValid(rowID);
            AssertRowSortOrderValid(newSortOrder);

            int oldSortOrder = m_RowIdentifierToDenseIndexMap[rowID];
            int iterDirection = newSortOrder > oldSortOrder ? 1 : -1;

            for (int i = oldSortOrder; i != newSortOrder; i += iterDirection)
            {
                int rowIDAt = m_RowDenseIndexToIDMap[i + iterDirection];
                m_RowIdentifierToDenseIndexMap[rowIDAt] = i;
                m_RowDenseIndexToIDMap[i] = m_RowDenseIndexToIDMap[i + iterDirection];
            }

            SetRowOrderForColumns(m_AllStringColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(m_AllBoolColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(m_AllCharColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(m_AllSByteColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(m_AllByteColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(m_AllShortColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(m_AllUShortColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(m_AllIntColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(m_AllUIntColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(m_AllLongColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(m_AllULongColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(m_AllFloatColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(m_AllDoubleColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(m_AllVector2Columns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(m_AllVector3Columns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(m_AllVector4Columns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(m_AllVector2IntColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(m_AllVector3IntColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(m_AllQuaternionColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(m_AllRectColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(m_AllRectIntColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(m_AllColorColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(m_AllLayerMaskColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(m_AllBoundsColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(m_AllBoundsIntColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(m_AllHash128Columns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(m_AllGradientColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(m_AllAnimationCurveColumns, oldSortOrder, newSortOrder);
            SetRowOrderForColumns(m_AllObjectRefColumns, oldSortOrder, newSortOrder);
        }

        public void SetAllRowOrders(int[] sortedRowIDs)
        {
            AssertSortRowsArgValid(sortedRowIDs);

            ReSortRows(m_AllStringColumns, sortedRowIDs);
            ReSortRows(m_AllBoolColumns, sortedRowIDs);
            ReSortRows(m_AllCharColumns, sortedRowIDs);
            ReSortRows(m_AllSByteColumns, sortedRowIDs);
            ReSortRows(m_AllByteColumns, sortedRowIDs);
            ReSortRows(m_AllShortColumns, sortedRowIDs);
            ReSortRows(m_AllUShortColumns, sortedRowIDs);
            ReSortRows(m_AllIntColumns, sortedRowIDs);
            ReSortRows(m_AllUIntColumns, sortedRowIDs);
            ReSortRows(m_AllLongColumns, sortedRowIDs);
            ReSortRows(m_AllULongColumns, sortedRowIDs);
            ReSortRows(m_AllFloatColumns, sortedRowIDs);
            ReSortRows(m_AllDoubleColumns, sortedRowIDs);
            ReSortRows(m_AllVector2Columns, sortedRowIDs);
            ReSortRows(m_AllVector3Columns, sortedRowIDs);
            ReSortRows(m_AllVector4Columns, sortedRowIDs);
            ReSortRows(m_AllVector2IntColumns, sortedRowIDs);
            ReSortRows(m_AllVector3IntColumns, sortedRowIDs);
            ReSortRows(m_AllQuaternionColumns, sortedRowIDs);
            ReSortRows(m_AllRectColumns, sortedRowIDs);
            ReSortRows(m_AllRectIntColumns, sortedRowIDs);
            ReSortRows(m_AllColorColumns, sortedRowIDs);
            ReSortRows(m_AllLayerMaskColumns, sortedRowIDs);
            ReSortRows(m_AllBoundsColumns, sortedRowIDs);
            ReSortRows(m_AllBoundsIntColumns, sortedRowIDs);
            ReSortRows(m_AllHash128Columns, sortedRowIDs);
            ReSortRows(m_AllGradientColumns, sortedRowIDs);
            ReSortRows(m_AllAnimationCurveColumns, sortedRowIDs);
            ReSortRows(m_AllObjectRefColumns, sortedRowIDs);

            for (int i = 0; i < sortedRowIDs.Length; i++)
            {
                int rowID = sortedRowIDs[i];
                m_RowDenseIndexToIDMap[i] = rowID;
                m_RowIdentifierToDenseIndexMap[rowID] = i;
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
                    int oldRowIndex = m_RowIdentifierToDenseIndexMap[rowID];

                    newColumn[j] = column[oldRowIndex];
                }

                columns[i].TArray = newColumn;
            }
        }

        // Internal

        internal void AddTypeNameEntryForUnityObjectColumn()
        {
            int nameArrayLength = m_AllObjectRefTypeNames?.Length ?? 0;
            Array.Resize(ref m_AllObjectRefTypeNames, nameArrayLength + 1);
            m_AllObjectRefTypeNames[nameArrayLength] = UnityObjectString;
        }

        internal void RemoveTypeNameEntryForUnityObjectColumn(int columnDenseIndex)
        {
            int nameArrayLength = m_AllObjectRefTypeNames?.Length ?? 0;
            m_AllObjectRefTypeNames[columnDenseIndex] = m_AllObjectRefTypeNames[nameArrayLength];
            Array.Resize(ref m_AllObjectRefTypeNames, nameArrayLength - 1);
        }

        internal void AssertObjectColumnIDValid(int columnID)
        {
            AssertColumnIDValid(columnID);
            if (m_ColumnIdentifierToDenseIndexMap[columnID].ColumnType != Serializable.SerializableTypes.Object)
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
            allColumnsOfType[columnCount].TArray = new T[m_RowCount];

            int columnID = m_ColumnEntriesFreeListHead;
            string[] columnNamesForType = m_AllColumnNames[(int)typeIndex].TArray;
            int columnNamesCount = columnNamesForType?.Length ?? 0;
            Array.Resize(ref columnNamesForType, columnNamesCount + 1);
            columnNamesForType[columnNamesCount] = columnName == null ? columnID.ToString() : columnName;
            m_AllColumnNames[(int)typeIndex].TArray = columnNamesForType;


            int columnIDToDenseIndexMapLength = m_ColumnIdentifierToDenseIndexMap?.Length ?? 0;
            if (columnID >= columnIDToDenseIndexMapLength)
            {
                int newSize = columnIDToDenseIndexMapLength * 2;
                newSize = newSize == 0 ? 1 : newSize;
                Array.Resize(ref m_ColumnIdentifierToDenseIndexMap, newSize);
                for (int i = columnIDToDenseIndexMapLength; i < newSize; i++)
                {
                    ref ColumnEntry entry = ref m_ColumnIdentifierToDenseIndexMap[i];
                    entry.ColumnDenseIndex = i + 1;
                    entry.ColumnType = Serializable.SerializableTypes.Invalid;
                }

                Array.Resize(ref m_ColumnIdentifierToSortOrderMap, newSize);
                for (int i = columnIDToDenseIndexMapLength; i < newSize; i++)
                {
                    m_ColumnIdentifierToSortOrderMap[i] = -1;
                }
            }

            m_ColumnEntriesFreeListHead = m_ColumnIdentifierToDenseIndexMap[columnID].ColumnDenseIndex;

            ref int[] denseIndexToIDMap = ref m_ColumnDenseIndexToIDMap[(int)typeIndex].TArray;
            int denseIndexToIDMapLength = denseIndexToIDMap?.Length ?? 0;
            Array.Resize(ref denseIndexToIDMap, denseIndexToIDMapLength + 1);
            denseIndexToIDMap[denseIndexToIDMapLength] = columnID;

            ref ColumnEntry newEntry = ref m_ColumnIdentifierToDenseIndexMap[columnID];
            newEntry.ColumnDenseIndex = denseIndexToIDMapLength;
            newEntry.ColumnType = typeIndex;

            int insertAtSortedIndex =
                insertAtColumnID < 0 ? m_CombinedColumnCount : m_ColumnIdentifierToSortOrderMap[insertAtColumnID];
            Array.Resize(ref m_SortedOrderToColumnIdentifierMap, m_CombinedColumnCount + 1);
            for (int i = m_CombinedColumnCount; i > insertAtSortedIndex; i--)
            {
                int currentColumnID = m_SortedOrderToColumnIdentifierMap[i - 1];
                m_SortedOrderToColumnIdentifierMap[i] = currentColumnID;
                m_ColumnIdentifierToSortOrderMap[currentColumnID] = i;
            }

            if (typeIndex == Serializable.SerializableTypes.Object)
            {
                AddTypeNameEntryForUnityObjectColumn();
            }

            m_ColumnIdentifierToSortOrderMap[columnID] = insertAtSortedIndex;
            m_SortedOrderToColumnIdentifierMap[insertAtSortedIndex] = columnID;

            ++m_CombinedColumnCount;
            m_DataVersion++;

            return columnID;
        }

        internal void RemoveColumnInternal<T>(ref ArrayHolder<T>[] allColumnsOfType,
            Serializable.SerializableTypes typeIndex, int columnID)
        {
            AssertColumnIDValid(columnID);
            int columnLocation = m_ColumnIdentifierToDenseIndexMap[columnID].ColumnDenseIndex;

            int lastIndex = allColumnsOfType.Length - 1;
            allColumnsOfType[columnLocation] = allColumnsOfType[lastIndex];
            Array.Resize(ref allColumnsOfType, lastIndex);

            ref string[] columnNamesOfType = ref m_AllColumnNames[(int)typeIndex].TArray;
            columnNamesOfType[columnLocation] = columnNamesOfType[lastIndex];
            Array.Resize(ref columnNamesOfType, lastIndex);

            int columnOrder = m_ColumnIdentifierToSortOrderMap[columnID];

            ref int[] denseIndicesOfType = ref m_ColumnDenseIndexToIDMap[(int)typeIndex].TArray;
            int sparseIndexToSwap = denseIndicesOfType[lastIndex];

            m_ColumnIdentifierToDenseIndexMap[sparseIndexToSwap].ColumnDenseIndex = columnLocation;
            ref ColumnEntry sparseIndexToFree = ref m_ColumnIdentifierToDenseIndexMap[columnID];
            sparseIndexToFree.ColumnType = Serializable.SerializableTypes.Invalid;
            sparseIndexToFree.ColumnDenseIndex = m_ColumnEntriesFreeListHead;

            m_ColumnEntriesFreeListHead = columnID;

            denseIndicesOfType[columnLocation] = sparseIndexToSwap;
            Array.Resize(ref denseIndicesOfType, lastIndex);

            if (typeIndex == Serializable.SerializableTypes.Object)
            {
                RemoveTypeNameEntryForUnityObjectColumn(columnLocation);
            }

            for (int i = columnOrder + 1; i < m_CombinedColumnCount; i++)
            {
                int currentColumnID = m_SortedOrderToColumnIdentifierMap[i];
                m_SortedOrderToColumnIdentifierMap[i - 1] = currentColumnID;
                m_ColumnIdentifierToSortOrderMap[currentColumnID] = i - 1;
            }

            m_ColumnIdentifierToSortOrderMap[columnID] = -1;

            Array.Resize(ref m_SortedOrderToColumnIdentifierMap, m_CombinedColumnCount - 1);

            --m_CombinedColumnCount;
            m_DataVersion++;
        }

        internal void InsertRowsOfTypeInternal<T>(ref ArrayHolder<T>[] allColumnsOfType, int insertAt,
            int numberOfNewRows)
        {
            int columnCount = allColumnsOfType?.Length ?? 0;
            for (int i = 0; i < columnCount; i++)
            {
                ref T[] rows = ref allColumnsOfType[i].TArray;
                int newRowCount = m_RowCount + numberOfNewRows;
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
                int newRowCount = m_RowCount - numberOfRowsToDelete;

                for (int j = removeAt + numberOfRowsToDelete; j < m_RowCount; j++)
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
            int column = m_ColumnIdentifierToDenseIndexMap[columnID].ColumnDenseIndex;
            int row = m_RowIdentifierToDenseIndexMap[rowID];
            return ref allColumnsOfType[column][row];
        }

        internal T GetCell<T>(int rowID, int columnID, ref ArrayHolder<T>[] allColumnsOfType)
        {
            AssertColumnIDValid(columnID);
            AssertRowIDValid(rowID);
            int column = m_ColumnIdentifierToDenseIndexMap[columnID].ColumnDenseIndex;
            int row = m_RowIdentifierToDenseIndexMap[rowID];
            return allColumnsOfType[column][row];
        }

        internal ulong SetCell<T>(int rowID, int columnID, ref ArrayHolder<T>[] allColumnsOfType, T value)
        {
            AssertColumnIDValid(columnID);
            AssertRowIDValid(rowID);
            int column = m_ColumnIdentifierToDenseIndexMap[columnID].ColumnDenseIndex;
            int row = m_RowIdentifierToDenseIndexMap[rowID];
            allColumnsOfType[column][row] = value;
            m_DataVersion++;
            return m_DataVersion;
        }

        internal T[] GetColumn<T>(int columnID, ref ArrayHolder<T>[] allColumnsOfType)
        {
            AssertColumnIDValid(columnID);
            int column = m_ColumnIdentifierToDenseIndexMap[columnID].ColumnDenseIndex;
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

            if (sortedColumnIDs.Length != m_SortedOrderToColumnIdentifierMap.Length)
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
            if (sortedOrder >= m_CombinedColumnCount || sortedOrder < 0)
            {
                throw new ArgumentException("Invalid column sort order argument: " + sortedOrder);
            }
        }

        internal void AssertRowSortOrderValid(int sortedOrder)
        {
            if (sortedOrder >= m_RowCount || sortedOrder < 0)
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

            if (sortedRowIDs.Length != m_RowDenseIndexToIDMap.Length)
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