// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GDX.Collections.Pooling
{
    public struct ManagedArrayPool_PowerOfTwoLengths<T>
    {
        public ManagedArrayPool<T>[] ArrayPools;
        public int[] MaxPoolCapacities;

        public ManagedArrayPool_PowerOfTwoLengths(int[] initialArrayStorageCounts, int[] maxPoolCapacities)
        {
            ArrayPools = new ManagedArrayPool<T>[30];
            MaxPoolCapacities = maxPoolCapacities;

            for (int i = 0; i < 30; i++)
            {
                int initialArrayCount = initialArrayStorageCounts[i];
                int maxArraysForSize = maxPoolCapacities[i];
                T[][] arrayPoolForSize = new T[maxArraysForSize][];

                for (int j = 0; j < initialArrayCount; j++)
                {
                    arrayPoolForSize[j] = new T[1 << i];
                }

                ManagedArrayPool<T> pool = new ManagedArrayPool<T>();

                pool.Pool = arrayPoolForSize;
                pool.Count = maxPoolCapacities[i];
            }
        }

        public T[] GetArrayFromPool(int requestedSize)
        {
            requestedSize = requestedSize < 1 ? 1 : requestedSize;
            requestedSize--;
            requestedSize |= requestedSize >> 1;
            requestedSize |= requestedSize >> 2;
            requestedSize |= requestedSize >> 4;
            requestedSize |= requestedSize >> 8;
            requestedSize |= requestedSize >> 16;
            int nextPowerOfTwo = requestedSize++;

            LongDoubleConversionUnion u;
            u.doubleValue = 0.0;
            u.longValue = 0x4330000000000000L + nextPowerOfTwo;
            u.doubleValue -= 4503599627370496.0;
            int index = (int)(u.longValue >> 52) - 0x3FF;

            ManagedArrayPool<T> arrayPool = ArrayPools[index];

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

        public void ReturnArrayToPool(T[] array)
        {
            uint length = unchecked((uint)array.Length); // Counting on people to be cool and not pass in a non-power-of-two here.

            LongDoubleConversionUnion u;
            u.doubleValue = 0.0;
            u.longValue = 0x4330000000000000L + length;
            u.doubleValue -= 4503599627370496.0;
            int index = (int)(u.longValue >> 52) - 0x3FF;

            ManagedArrayPool<T> arrayPool = ArrayPools[index];
            int maxPoolCapacity = MaxPoolCapacities[index];
            int currentPoolCapacity = arrayPool.Pool.Length;
            int nextPoolCapacity = currentPoolCapacity * 2;
            nextPoolCapacity = (nextPoolCapacity > maxPoolCapacity) ? maxPoolCapacity : nextPoolCapacity;

            if (arrayPool.Count < maxPoolCapacity)
            {
                arrayPool.Pool[arrayPool.Count] = array;
                arrayPool.Count++;
            }

            ArrayPools[index] = arrayPool;
        }
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
    public struct LongDoubleConversionUnion
    {
       [System.Runtime.InteropServices.FieldOffset(0)] public long longValue;
       [System.Runtime.InteropServices.FieldOffset(0)] public double doubleValue;
    }
}
