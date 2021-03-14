// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

public struct ManagedArrayPool_PowerOfTwoLengths<T>
{
    public const int MAX_POWER_OF_TWO_SHIFT = 30;
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

        int index = 0;
        int i = 1;

        while ((i & nextPowerOfTwo) == 0)
        {
            i = i << 1;
            ++index;
        }

        ManagedArrayPool<T> arrayPool = ArrayPools[index];

        T[] array;
        if (arrayPool.Count < 1)
        {
            array = new T[nextPowerOfTwo];
        }
        else
        {
            array = arrayPool.Pool[arrayPool.Count - 1];
            arrayPool.Count--;
        }

        ArrayPools[index] = arrayPool;

        return array;
    }

    public void ReturnArrayToPool(T[] array)
    {
        int length = array.Length; // Counting on people to be cool and not pass in a non-power-of-two here.

        int index = 0;
        int i = 1;

        while ((i & length) == 0)
        {
            i = i << 1;
            ++index;
        }

        ManagedArrayPool<T> arrayPool = ArrayPools[index];
        int maxPoolCapacity = MaxPoolCapacities[i];
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
