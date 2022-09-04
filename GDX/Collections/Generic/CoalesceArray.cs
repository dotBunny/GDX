// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace GDX.Collections.Generic
{
    /// <summary>
    ///     Multiple arrays acting as one uniform coalesced array.
    /// </summary>
    /// <remarks>Stores a maximum of 18,446,744,073,709,551,615 elements.</remarks>
    /// <typeparam name="T">The type of data to be stored.</typeparam>
    public struct CoalesceArray<T>
    {
        const ulong k_MaxByteSize = 2147483648;
        /// <summary>
        ///     The internal arrays storage
        /// </summary>
        SimpleList<T[]> m_Buckets;

        /// <summary>
        ///     The block size used to allocate new arrays.
        /// </summary>
        readonly ulong m_BucketSize;

        /// <summary>
        ///     Cached version of the bucket size, minus one used in <see cref="GetIndex"/>.
        /// </summary>
        readonly ulong m_BucketSizeMinusOne;

        /// <summary>
        ///     The number of elements the <see cref="CoalesceArray{T}"/> is capable of holding.
        /// </summary>
        ulong m_Length;

        public CoalesceArray(ulong length)
        {
            int sizeOf = UnsafeUtility.SizeOf(typeof(T));
            ulong expectedSize = length * (ulong)sizeOf;

            // Wrapped over the limit or bigger then allowed bucket
            if (expectedSize < length || expectedSize > k_MaxByteSize || length >= k_MaxByteSize)
            {
                m_BucketSize = k_MaxByteSize / (ulong)sizeOf;
            }
            else
            {
                m_BucketSize = math.ceilpow2(length);
            }
            m_BucketSizeMinusOne = m_BucketSize - 1;

            // Build our empty arrays
            ulong remainder = length & m_BucketSizeMinusOne;
            int extraArray = 0;
            if (remainder > 0)
            {
                extraArray = 1;
            }
            int placesToShift = math.tzcnt(m_BucketSize);
            int arrayCount = (int)(length >> placesToShift) + extraArray;
            m_Buckets = new SimpleList<T[]>(arrayCount);
            for (int i = 0; i < arrayCount; i++)
            {
                m_Buckets.Array[i] = new T[(int)m_BucketSize];
            }

            m_Length = length;
        }

        public T this[ulong index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                (int arrayIndex, int offset) info = GetIndex(index);
                return m_Buckets.Array[info.arrayIndex][info.offset];
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                (int arrayIndex, int offset) info = GetIndex(index);
                m_Buckets.Array[info.arrayIndex][info.offset] = value;
            }
        }

        public ulong Length
        {
            get => m_Length;
            set
            {
                ulong remainder = value & m_BucketSizeMinusOne;
                int extraArray = 0;
                if (remainder > 0)
                {
                    extraArray = 1;
                }
                int arrayCount = (int)((value - remainder) / m_BucketSize) + extraArray;
                int previousCount = m_Buckets.Count;

                if (arrayCount > previousCount)
                {
                    int additionalCount = arrayCount - previousCount;
                    for (int i = 0; i < additionalCount; i++)
                    {
                        m_Buckets.AddWithExpandCheck(new T[m_BucketSize]);
                    }
                }
                else if (arrayCount < previousCount)
                {
                    int extraCount = previousCount - arrayCount;
                    for (int i = 0; i < extraCount; i++)
                    {
                        m_Buckets.RemoveFromBack();
                    }
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