// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;

namespace GDX.Collections.Generic
{
#if UNITY_2021_2_OR_NEWER
    public struct ExpandingArray<T>
    {
        T[] m_Array;
        int m_Head;
        int m_Size;

        public ExpandingArray(int size)
        {
            m_Array = new T[size];
            m_Head = 0;
            m_Size = size;
        }

        public void Add(T item)
        {
            Ensure(1);
            m_Array[m_Head] = item;
            m_Head++;
        }

        public void AddRange(T[] items)
        {
            int size = items.Length;
            Ensure(size);
            Array.Copy(items, 0, m_Array, m_Head, size);
            m_Head += size;
        }

        public void AddRangeUnchecked(T[] items)
        {
            int size = items.Length;
            Array.Copy(items, 0, m_Array, m_Head, size);
            m_Head += size;
        }
        public void AddUnchecked(T item)
        {
            m_Array[m_Head] = item;
            m_Head++;
        }

        public void Clear()
        {
            for (int i = 0; i < m_Size; i++)
            {
                m_Array[i] = default;
            }
            m_Head = 0;
        }

        void Ensure(int needed)
        {
            int space = m_Size - m_Head;
            if (space >= needed)
            {
                return;
            }

            m_Size += needed;
            Array.Resize(ref m_Array, m_Size);
        }

        public Span<T> GetSpan()
        {
            return new Span<T>(m_Array, 0, m_Head);
        }

        public Span<T> GetSpan(int startIndex, int count)
        {
            return new Span<T>(m_Array, startIndex, count);
        }

        public ReadOnlySpan<T> GetReadOnlySpan()
        {
            return new ReadOnlySpan<T>(m_Array, 0, m_Head);
        }

        public ReadOnlySpan<T> GetReadOnlySpan(int startIndex, int count)
        {
            return new ReadOnlySpan<T>(m_Array, startIndex, count);
        }

        public bool HasData()
        {
            return m_Head > 0;
        }

        public void Reset()
        {
            m_Head = 0;
        }
    }
#endif // UNITY_2021_2_OR_NEWER
}