﻿// Copyright (c) 2020-2024 dotBunny Inc.
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
        ///     Cached version of the bucket size, minus one used in <see cref="GetBucketLocation" />.
        /// </summary>
        readonly ulong m_BucketSizeMinusOne;

        public CoalesceArray(ulong length)
        {
            int sizeOf = UnsafeUtility.SizeOf(typeof(T));
            ulong expectedSize = length * (ulong)sizeOf;

            // Check if we've wrapped around, if the size is bigger then our max data size, or if the length will exceed
            // the array address space.
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

            Length = length;
        }

        public T this[ulong index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                (int bucketIndex, int bucketOffset) info = GetBucketLocation(index);
                return m_Buckets.Array[info.bucketIndex][info.bucketOffset];
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                (int bucketIndex, int bucketOffset) info = GetBucketLocation(index);
                m_Buckets.Array[info.bucketIndex][info.bucketOffset] = value;
            }
        }

        /// <summary>
        ///     The number of elements the <see cref="CoalesceArray{T}" /> is capable of holding.
        /// </summary>
        public ulong Length { get; private set; }

        public ref T[] GetBucket(int bucketIndex)
        {
            return ref m_Buckets.Array[bucketIndex];
        }

        public int GetBucketCount()
        {
            return m_Buckets.Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int bucketIndex, int bucketOffset) GetBucketLocation(ulong index)
        {
            int bucketOffset = (int)(index & m_BucketSizeMinusOne);
            int placesToShift = math.tzcnt(m_BucketSize);
            int bucketIndex = (int)(index >> placesToShift);
            return (bucketIndex, bucketOffset);
        }

        public bool Resize(ulong desiredLength)
        {
            ulong remainder = desiredLength & m_BucketSizeMinusOne;
            int extraArray = 0;
            if (remainder > 0)
            {
                extraArray = 1;
            }

            int arrayCount = (int)((desiredLength - remainder) / m_BucketSize) + extraArray;
            int previousCount = m_Buckets.Count;

            // Grow
            if (arrayCount > previousCount)
            {
                int additionalCount = arrayCount - previousCount;
                for (int i = 0; i < additionalCount; i++)
                {
                    m_Buckets.AddWithExpandCheck(new T[m_BucketSize]);
                }

                Length = desiredLength;
                return true;
            }

            // Shrink
            if (arrayCount < previousCount)
            {
                int extraCount = previousCount - arrayCount;
                for (int i = 0; i < extraCount; i++)
                {
                    m_Buckets.RemoveFromBack();
                }

                Length = desiredLength;
                return true;
            }

            // Do nothing
            Length = desiredLength;
            return false;
        }
    }
}