// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace GDX.Collections.Generic
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>Stores a maximum of 18,446,744,073,709,551,615 elements.</remarks>
    /// <typeparam name="T"></typeparam>
    public struct CoalesceArray<T>
    {
        const int k_MaximumBucketSize = 2147483646;

        /// <summary>
        ///     The internal arrays storage
        /// </summary>
        SimpleList<T[]> m_Arrays;

        /// <summary>
        ///     The block size used to allocate new arrays.
        /// </summary>
        readonly int m_BucketSize;

        /// <summary>
        ///     Cached version of the bucket size, minus one used in <see cref="GetIndex"/>.
        /// </summary>
        readonly ulong m_BucketSizeMinusOne;

        /// <summary>
        ///     The number of elements the <see cref="CoalesceArray{T}"/> is capable of holding.
        /// </summary>
        ulong m_Length;

        public CoalesceArray(ulong length, int bucketSize = 65536)
        {
            // Ensure array size is power of 2 based.
            if (!math.ispow2(bucketSize))
            {
                bucketSize -= 1;
            }
            else if (bucketSize > k_MaximumBucketSize)
            {
                bucketSize = k_MaximumBucketSize;
            }

            m_BucketSize = bucketSize;
            m_BucketSizeMinusOne = (ulong)m_BucketSize - 1;

            int placesToShift = math.tzcnt(m_BucketSize);
            int arrayCount = (int)(length >> placesToShift) + 1;
            m_Arrays = new SimpleList<T[]>(arrayCount);
            for (int i = 0; i < arrayCount; i++)
            {
                m_Arrays.Array[i] = new T[m_BucketSize];
            }

            m_Length = length;
        }

        public T this[ulong index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                (int arrayIndex, int offset) info = GetIndex(index);
                return m_Arrays.Array[info.arrayIndex][info.offset];
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                (int arrayIndex, int offset) info = GetIndex(index);
                m_Arrays.Array[info.arrayIndex][info.offset] = value;
            }
        }

        public ulong Length
        {
            get => m_Length;
            set
            {
                ulong remainder = value & m_BucketSizeMinusOne;
                ulong arrayCount = ((value - remainder) / (ulong)m_BucketSize) + 1;
                int previousCount = m_Arrays.Count;
                if ((int)arrayCount <= previousCount)
                {
                    return;
                }

                int additionalCount = (int)arrayCount - previousCount;
                for (int i = 0; i < additionalCount; i++)
                {
                    m_Arrays.AddWithExpandCheck(new T[m_BucketSize]);
                }

                m_Length = value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        (int arrayIndex, int offset) GetIndex(ulong index)
        {
            int remainder = (int)(index & m_BucketSizeMinusOne);
            int placesToShift = math.tzcnt(m_BucketSize);
            int arrayIndex = (int)(index >> placesToShift);
            return (arrayIndex, remainder);
        }
    }
}