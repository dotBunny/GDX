// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using GDX.Collections.Generic;
using UnityEngine;

namespace GDX.Collections
{
    public class SimpleTable
    {
        public enum GrowthStrategy
        {
            Singles,
            Double,
            Triple
        }

        Array2D<object> m_Data;
        readonly GrowthStrategy m_GrowthStrategy;
        readonly SparseSet m_Layout;
        readonly Type[] m_Types;
        readonly int m_TypesLength;

        public SimpleTable(Type[] columnTypes, int preallocateRowCount = 10,
            GrowthStrategy growthStrategy = GrowthStrategy.Double)
        {
            m_Types = columnTypes;
            m_TypesLength = m_Types.Length;
            m_Layout = new SparseSet(preallocateRowCount);
            m_Data = new Array2D<object>(10, m_TypesLength);
            m_GrowthStrategy = growthStrategy;
        }

        public int RowCount => m_Layout.Count;

        /// <summary>
        ///     Get an object at a specific 2-dimensional index in <see cref="m_Data" />.
        /// </summary>
        /// <param name="rowIndex">The row number.</param>
        /// <param name="columnIndex">The column number.</param>
        public object this[int rowIndex, int columnIndex]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_Data[rowIndex, columnIndex];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_Data[rowIndex, columnIndex] = value;
        }

        public bool AddRow(object[] values)
        {
            //m_Layout.GetDenseIndexWithBoundsCheck()
            //
            m_Data.AddRows(1);
            int rowIndex = m_Data.RowCount - 1;
            int valueCount = values.Length;
            for (int i = 0; i < valueCount; i++)
            {
                m_Data[rowIndex, i] = values[i];
            }

            return true;
        }

        public Type GetColumnType(int columnIndex)
        {
            return m_Types[columnIndex];
        }

        public object[] GetColumn(int columnIndex)
        {
            int count = RowCount;
            object[] returnArray = new object[RowCount];
            for (int i = 0; i < count; i++)
            {
                returnArray[i] = m_Data[i, columnIndex];
            }

            return returnArray;
        }

        public ReadOnlySpan<object> GetRow(int rowIndex)
        {
            return new ReadOnlySpan<object>(m_Data.Array, m_Data.GetRowIndex(rowIndex), m_TypesLength);
        }


        public void RemoveRow(int index)
        {
        }

        public bool InsertRow(int index, object[] values)
        {
            if (values.Length > m_Data.ColumnCount)
            {
                Debug.LogError("Attempted to add items in a row, then columns in the DataTable");
                return false;
            }

            if (index >= m_Data.RowCount)
            {
                return AddRow(values);
            }

            int valueCount = values.Length;
            for (int i = 0; i < valueCount; i++)
            {
                m_Data[index, i] = values[i];
            }

            return true;
        }
    }
}