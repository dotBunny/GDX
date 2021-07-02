// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

public static class DictionaryPerformanceTests
{
    public static void Test()
    {
        int illegalKey = int.MaxValue;

        int[] keys = new int[1_000_000];
        int[] buffer = new int[16_000_000]; // 64MB, make sure we clear the L3 cache.
        for (int i = 0; i < 1_000_000; i++)
        {
            keys[i] = UnityEngine.Random.Range(int.MinValue, int.MaxValue - 1);
        }

        // First up, IntKeyChainingDictionary

    }
}

namespace GDX.Collections.Generic
{
}


/// <summary>
/// A collection similar to the standard C# Dictionary, but optimized for lookup of existing keys (i.e. keys that are actually in the dictionary).
/// Looking up a key that does not exist will iterate through every key, as will checking for duplicates when adding a new entry.
///
/// This collection is sized at powers of two. If your key ranges are a particularly bad match for powers of two,
/// consider using the prime variant of this struct.
/// </summary>
/// <typeparam name="TValue">The type of value associated with the integer key.</typeparam>
public struct IntKeyConfidentLookupTable<TValue>
{
    public int[] keys;
    public TValue[] values;
    public int emptyKey;
    public int count;
    public int currentProbeDistance;

    public TValue this[int key]
    {
        get
        {
            int length = keys.Length;
            int idealIndex = key & (length - 1);

            int keyAtIndex = keys[idealIndex];

            for (int i = 0; i < length; i++)
            {
                int index = idealIndex + i & (length - 1);

                int currKey = keys[index];
                if (currKey == key)
                {
                    // found
                    return values[index];
                }
            }

            return default(TValue);
        }

        set
        {
            int length = keys.Length;
            int idealIndex = key & (length - 1);

            for (int i = 0; i < length; i++)
            {
                int index = idealIndex + i & (length - 1);

                int currKey = keys[index];
                if (currKey == key)
                {
                    // found
                    values[index] = value;
                    return;
                }
            }
        }
    }

    public TValue this[int key, int maxProbeDistance]
    {
        get
        {
            int length = keys.Length;
            int idealIndex = key & (length - 1);

            int keyAtIndex = keys[idealIndex];

            for (int i = 0; i < maxProbeDistance; i++)
            {
                int index = idealIndex + i & (length - 1);

                int currKey = keys[index];
                if (currKey == key)
                {
                    // found
                    return values[index];
                }
            }

            return default(TValue);
        }
    }

    public void AddUnchecked(int key, TValue value)
    {
        int length = keys.Length; 
        int idealIndex = key & (length - 1);

        for (int i = 0; i < length; i++)
        {
            int index = (idealIndex + i) & (length - 1);
            if (keys[index] == emptyKey)
            {
                keys[index] = key;
                values[index] = value;
                return;
            }
        }
    }

    public void AddWithExpandCheck(int key, TValue value)
    {
        int currLength = keys.Length;
        if (count >= (currLength >> 1))
        {
            Expand();
        }

        int length = keys.Length;
        int idealIndex = key & (length - 1);

        for (int i = 0; i < length; i++)
        {
            int index = (idealIndex + i) & (length - 1);
            if (keys[index] == emptyKey)
            {
                keys[index] = key;
                values[index] = value;
                return;
            }
        }
    }

    /// <summary>
    /// Adds the key value pair to the table.
    /// If a free slot is not found within the max probe distance, the table expands and tries again.
    /// If a free slot is not found after reaching maximum size, it falls back to finding the first available slot beyond the maximum probe distance.
    /// </summary>
    /// <param name="key">The key to insert.</param>
    /// <param name="value">The value to insert.</param>
    /// <param name="maxProbeDistance">The maximum distance any entry can be from its ideal index.</param>
    /// <param name="maxSize">The maximum size the dictionary can reach.</param>
    public void AddWithExpandCheck(int key, TValue value, ref int maxProbeDistance, int maxSize)
    {
        int length = keys.Length;
        
        if (length < maxSize && count >= (length >> 1))
        {
            Expand();
        }

        length = keys.Length;
        int idealIndex = key & (length - 1);

        int probeDistance = maxProbeDistance > length ? length : maxProbeDistance;

        for (int j = 0; j <= probeDistance; j++)
        {
            int index = (idealIndex + j) & (length - 1);
            if (keys[index] == emptyKey)
            {
                keys[index] = key;
                values[index] = value;
                return;
            }
        }

        while (length < maxSize)
        {
            Expand();

            length = keys.Length;
            idealIndex = key & (length - 1);

            probeDistance = maxProbeDistance > length ? length : maxProbeDistance;

            for (int j = 0; j <= maxProbeDistance; j++)
            {
                int index = (idealIndex + j) & (length - 1);
                if (keys[index] == emptyKey)
                {
                    keys[index] = key;
                    values[index] = value;
                    return;
                }
            }
        }

        length = keys.Length;
        idealIndex = key & (length - 1);
        int loopLength = length - probeDistance;

        for (int i = 0; i <= loopLength; i++)
        {
            int index = (idealIndex + i) & (length - 1);
            if (keys[index] == emptyKey)
            {
                keys[index] = key;
                values[index] = value;
                maxProbeDistance = i;
                return;
            }
        }
    }

    public void Example()
    {
        IntKeyConfidentLookupTable<float> table = default;
        int maxProbeDistance = 4;
        int maxSize = 4096;

        // Later...

        for (int i = 0; i < 500; i++)
        {
            int someKeyToInsert = default;
            float someValueToInsert = default;

            int someKeyToLookup = default;

            // Will slow down after max size is reached.
            table.AddWithExpandCheck(someKeyToInsert, someValueToInsert, ref maxProbeDistance, maxSize);

            // Will slow down after max size is reached.
            float someValueToLookup = table[someKeyToLookup, maxProbeDistance];
        }
    }

    /// <summary>
    /// Use this insertion method if insertions, removals, successful lookups, and unsuccessful lookups are all likely to occur.
    /// Inserts the key value pair, expanding if the dictionary grows beyond half capacity or if a free space does not exist within the probe limit.
    /// Balances the dictionary as it iterates in order to keep entries close to their ideal indices.
    /// </summary>
    /// <param name="key">The key to insert.</param>
    /// <param name="value">The value to insert.</param>
    /// <param name="maxProbeDistance">The maximum distance any entry can be from its ideal index.</param>
    public void AddWithExpandCheckAndBalance(int key, TValue value, int maxProbeDistance)
    {
        int currLength = keys.Length;
        if (count >= (currLength >> 1))
        {
            Expand();
        }

        int length = keys.Length;
        int idealIndex = key & (length - 1);

        while (key != emptyKey)
        {
            for (int distance = 0; distance <= maxProbeDistance; distance++)
            {
                int index = (idealIndex + distance) & (length - 1);
                int otherKey = keys[index];

                int idealIndexForOther = otherKey & (length - 1);

                int distanceDefault = index - idealIndexForOther;
                int distanceWrapped = (length - index) + idealIndexForOther;

                int distanceForOther = distanceDefault >= 0 ? distanceDefault : distanceWrapped;

                bool isFurtherThanOther = distanceForOther < distance;

                distance = isFurtherThanOther ? distanceForOther : distance;
                distance = otherKey == emptyKey ? maxProbeDistance : distance;

                if (otherKey == emptyKey || isFurtherThanOther)
                {
                    keys[index] = key;
                    TValue otherValue = values[index];
                    
                    values[index] = value;
                    key = otherKey;
                    value = otherValue;
                }
            }

            if (key != emptyKey)
            {
                Expand();
            }
        }
    }

    public void AddWithExpandCheckAndBalance(int key, TValue value)
    {
        int currLength = keys.Length;
        if (count >= (currLength >> 1))
        {
            Expand();
        }

        int length = keys.Length;
        GDX.Collections.Pooling.LongDoubleConversionUnion u;
        u.doubleValue = 0.0;
        u.longValue = 0x4330000000000000L + length;
        u.doubleValue -= 4503599627370496.0;
        int maxProbeDistance = (int)(u.longValue >> 52) - 0x3FF;

        int idealIndex = key & (length - 1);

        // Variants:
        // Readonly lookup with GREAT distribution and key/value pairs separated    ReadOnlyAlignedMap
        // Readonly lookup with GREAT distribution and key/value pairs together     ReadOnlyMap

        // More robust lookup with good distribution and key/value pairs separated  SimpleAlignedMap
        // More robust lookup with good distribution and key/value pairs together   SimpleMap

        // Balanced lookup with good distribution and key/value pairs separated     BalancedAlignedMap
        // Balanced lookup with good distribution and key/value pairs together      BalancedMap

        // Paranoid lookup with bad distribution and key/value pairs separated      ParanoidAlignedMap
        // Paranoid lookup with bad distribution and key/value pairs together       ParanoidMap

        while (key != emptyKey)
        {
            for (int distance = 0; distance <= maxProbeDistance; distance++)
            {
                int index = (idealIndex + distance) & (length - 1);
                int otherKey = keys[index];

                int idealIndexForOther = otherKey & (length - 1);

                int distanceDefault = index - idealIndexForOther;
                int distanceWrapped = (length - index) + idealIndexForOther;

                int distanceForOther = distanceDefault >= 0 ? distanceDefault : distanceWrapped;

                bool isFurtherThanOther = distanceForOther < distance;

                distance = isFurtherThanOther ? distanceForOther : distance;
                distance = otherKey == emptyKey ? maxProbeDistance : distance;

                if (otherKey == emptyKey || isFurtherThanOther)
                {
                    keys[index] = key;
                    TValue otherValue = values[index];

                    values[index] = value;
                    key = otherKey;
                    value = otherValue;
                }
            }

            if (key != emptyKey)
            {
                Expand();
            }
        }
    }
    public void Expand()
    {
        int oldLength = keys.Length;
        int newLength = oldLength * 2;
        int[] newKeys = new int[newLength];
        TValue[] newValues = new TValue[newLength];

        for (int i = 0; i < oldLength; i++)
        {
            int key = keys[i];
            TValue value = values[i];

            if (key != emptyKey)
            {
                int idealIndex = key & (newLength - 1);

                for (int j = 0; j < newLength; j++)
                {
                    int index = (idealIndex + j) & (newLength - 1);
                    if (keys[index] == emptyKey)
                    {
                        newKeys[index] = key;
                        newValues[index] = value;
                        break;
                    }
                }
            }
        }
    }
}

public struct IntKeyLookupTable<TValue>
{
    public int[] keys;
    public TValue[] values;
    public int illegalKey;
    public int count;

    public TValue this[int key]
    {
        get
        {
            int length = keys.Length;
            int idealIndex = key & (length - 1);

            int keyAtIndex = keys[idealIndex];

            for (int i = 0; i < keys.Length; i++)
            {
                int index = idealIndex + i & (length - 1);

                int currKey = keys[index];
                if (currKey == key)
                {
                    // found
                    return values[index];
                }
            }

            return default(TValue);
        }

        set
        {
            int length = keys.Length;
            int idealIndex = key & (length - 1);

            for (int i = 0; i < keys.Length; i++)
            {
                int index = idealIndex + i & (length - 1);

                int currKey = keys[index];
                if (currKey == key)
                {
                    // found
                    values[index] = value;
                    return;
                }
            }
        }
    }

    public void AddUnchecked(int key, TValue value)
    {
        int length = keys.Length;
        int idealIndex = key & (length - 1);

        for (int i = 0; i < length; i++)
        {
            int index = (idealIndex + i) & (length - 1);
            if (keys[index] == illegalKey)
            {
                keys[index] = key;
                values[index] = value;
                return;
            }
        }
    }

    public void AddWithExpandCheck(int key, TValue value)
    {
        int currLength = keys.Length;
        if (count >= (currLength >> 1))
        {
            Expand();
        }

        int length = keys.Length;
        int idealIndex = key & (length - 1);

        for (int i = 0; i < length; i++)
        {
            int index = (idealIndex + i) & (length - 1);
            if (keys[index] == illegalKey)
            {
                keys[index] = key;
                values[index] = value;
                return;
            }
        }
    }

    public void Expand()
    {
        int oldLength = keys.Length;
        int newLength = oldLength * 2;
        int[] newKeys = new int[newLength];
        TValue[] newValues = new TValue[newLength];

        for (int i = 0; i < oldLength; i++)
        {
            int key = keys[i];
            TValue value = values[i];

            if (key != illegalKey)
            {
                int idealIndex = key & (newLength - 1);

                for (int j = 0; j < newLength; j++)
                {
                    int index = (idealIndex + j) & (newLength - 1);
                    if (keys[index] == illegalKey)
                    {
                        newKeys[index] = key;
                        newValues[index] = value;
                        break;
                    }
                }
            }
        }
    }
}
//public struct IntKeyDictionary2<T>
//{
//    public int[] intArrays; // Stores buckets, then keys, then next indices. Buckets portion is 2x bigger than keys and next indices.

//    public T[] values;
//    public int[] keys;  
//    public int[] buckets;
//    public int[] nextIndices;
//    public byte[] keyStates;

//    public int version;
//    public int freeIndex;
//    public int freeCount;
//    public int capacity;
//    public int usedIndices;

//    public IntKeyDictionary2(int minCapacity)
//    {
//        minCapacity = minCapacity < 1 ? 1 : minCapacity;
//        minCapacity--;
//        minCapacity |= minCapacity >> 1;
//        minCapacity |= minCapacity >> 2;
//        minCapacity |= minCapacity >> 4;
//        minCapacity |= minCapacity >> 8;
//        minCapacity |= minCapacity >> 16;
//        int arrayLength = 2 * (minCapacity + 1);

//        keys = new int[arrayLength];   
//        nextIndices = new int[arrayLength];
//        values = new T[arrayLength];
//        buckets = new int[arrayLength];

//        for (int i = 0; i < arrayLength; i++)
//        {
//            buckets[i] = -1;
//        }

//        freeIndex = -1;
//        freeCount = 0;
//        version = 0;
//        capacity = arrayLength;
//        usedIndices = 0;
//    }

//    public int Count()
//    {
//        int min = capacity < freeCount ? capacity : freeCount;
//        return Math.Min(capacity, usedIndices) - freeCount;
//    }

//    public int FindEntry(int key)
//    {
//        if (buckets != null)
//        {
//            int hashCode = key & 0x7FFFFFFF;
//            int capacityMask = buckets.Length - 1;
//            for (int i = buckets[hashCode % buckets.Length]; i >= 0; i = entries[i].next)
//            {
//                if (entries[i].hashCode == hashCode && entries[i].key == key) return i;
//            }
//        }
//        return -1;
//    }

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public static int Ceilpow2(int x)
//    {
//        x -= 1;
//        x |= x >> 1;
//        x |= x >> 2;
//        x |= x >> 4;
//        x |= x >> 8;
//        x |= x >> 16;
//        return x + 1;
//    }
//}

