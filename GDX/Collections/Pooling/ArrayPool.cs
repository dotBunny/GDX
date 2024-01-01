// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Mathematics;

namespace GDX.Collections.Pooling
{
    /// <summary>
    ///     An object pool for arrays with power-of-two lengths.
    /// </summary>
    /// <typeparam name="T">The data type contained by pooled arrays.</typeparam>
    public struct ArrayPool<T>
    {
        public JaggedArrayWithCount<T>[] ArrayPools;
        public int[] MaxPoolCapacities;

        /// <summary>
        ///     Initialize the array pool with initial and maximum sizes for each power-of-two, 0 through 30 inclusive (the maximum
        ///     power-of-two length supported in C#).
        /// </summary>
        /// <param name="initialPoolCounts"></param>
        /// <param name="maxPoolCapacities"></param>
        public ArrayPool(int[] initialPoolCounts, int[] maxPoolCapacities)
        {
            ArrayPools = new JaggedArrayWithCount<T>[31];
            MaxPoolCapacities = maxPoolCapacities;

            for (int i = 0; i < 31; i++)
            {
                int initialArrayCount = initialPoolCounts[i];
                int maxArraysForSize = maxPoolCapacities[i];
                T[][] arrayPoolForSize = new T[maxArraysForSize][];

                for (int j = 0; j < initialArrayCount; j++)
                {
                    arrayPoolForSize[j] = new T[1 << i];
                }

                JaggedArrayWithCount<T> pool =
                    new JaggedArrayWithCount<T> { Pool = arrayPoolForSize, Count = initialArrayCount };

                ArrayPools[i] = pool;
            }
        }

        /// <summary>
        ///     Allocates an array from the pool. Finds an array of the smallest power-of-two length larger than or equal to the
        ///     requested size.
        /// </summary>
        /// <param name="requestedSize">
        ///     The desired array length. The returned array will be the smallest power-of-two larger than
        ///     or equal to this size.
        /// </param>
        /// <returns></returns>
        public T[] Get(int requestedSize)
        {
            requestedSize = requestedSize < 1 ? 1 : requestedSize;
            requestedSize--;
            requestedSize |= requestedSize >> 1;
            requestedSize |= requestedSize >> 2;
            requestedSize |= requestedSize >> 4;
            requestedSize |= requestedSize >> 8;
            requestedSize |= requestedSize >> 16;
            int nextPowerOfTwo = requestedSize + 1;

            LongDoubleConversionUnion u;
            u.DoubleValue = 0.0;
            u.LongValue = 0x4330000000000000L + nextPowerOfTwo;
            u.DoubleValue -= 4503599627370496.0;
            int index = (int)(u.LongValue >> 52) - 0x3FF;

            JaggedArrayWithCount<T> arrayPool = ArrayPools[index];

            T[] array;
            if (arrayPool.Count < 1)
            {
                array = new T[nextPowerOfTwo];
            }
            else
            {
                array = arrayPool.Pool[arrayPool.Count - 1];
                arrayPool.Pool[arrayPool.Count - 1] = null;
                arrayPool.Count--;
            }

            ArrayPools[index] = arrayPool;

            return array;
        }

        /// <summary>
        ///     Return a power-of-two sized array to the pool. Only pass power-of-two sized arrays to this function. Does not clear
        ///     the array.
        /// </summary>
        /// <param name="array">The power-of-two sized array to return to the pool. Power-of-two sizes only.</param>
        public void Return(T[] array)
        {
            uint length =
                unchecked((uint)array.Length); // Counting on people to be cool and not pass in a non-power-of-two here.

            LongDoubleConversionUnion u;
            u.DoubleValue = 0.0;
            u.LongValue = 0x4330000000000000L + length;
            u.DoubleValue -= 4503599627370496.0;
            int index = (int)(u.LongValue >> 52) - 0x3FF;

            JaggedArrayWithCount<T> arrayPool = ArrayPools[index];
            int maxPoolCapacity = MaxPoolCapacities[index];

            if (arrayPool.Count < maxPoolCapacity)
            {
                arrayPool.Pool[arrayPool.Count] = array;
                arrayPool.Count++;
            }

            ArrayPools[index] = arrayPool;
        }
    }
}