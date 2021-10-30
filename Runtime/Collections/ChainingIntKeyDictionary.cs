// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;

namespace GDX.Collections.Generic
{
    public struct ChainingIntKeyDictionary<TValue>
    {
        public int[] Buckets;
        public IntKeyEntry[] Keys; // TODO: conditionally make this an int2 array if using Unity.Mathematics.
        public TValue[] Values;
        public int FreeListHead;
        public int Count;

        public ChainingIntKeyDictionary(int minCapacity)
        {
            const int maxPrime = int.MaxValue;
            int primeCapacity = maxPrime;
            int[] primes = DictionaryPrimes.Primes;
            for (int i = 0; i < primes.Length; i++)
            {
                int prime = primes[i];
                if (prime >= minCapacity)
                {
                    primeCapacity = prime;
                    break;
                }
            }

            Buckets = new int[primeCapacity];
            for (int i = 0; i < primeCapacity; i++)
            {
                Buckets[i] = int.MaxValue;
            }

            Keys = new IntKeyEntry[primeCapacity];

            for (int i = 0; i < primeCapacity; i++)
            {
                Keys[i].next = (1 << 31) | (i + 1);
            }

            Values = new TValue[primeCapacity];
            Count = 0;
            FreeListHead = 0;
        }

        public void AddWithExpandCheck(int key, TValue value)
        {
            int freeIndex = FreeListHead;

            if (freeIndex > Buckets.Length)
            {
                Expand();
            }

            int hashCode = key & 0x7FFFFFFF;
            int hashIndex = hashCode % Buckets.Length;
            int indexAtBucket = Buckets[hashIndex];
            IntKeyEntry keyEntry = Keys[freeIndex];

            FreeListHead = keyEntry.next & 0x7FFFFFFF;
            keyEntry.next = indexAtBucket;
            keyEntry.key = key;
            Buckets[hashIndex] = freeIndex;

            Values[freeIndex] = value;
            Keys[freeIndex] = keyEntry;

            ++Count;
        }

        public bool AddWithUniqueCheck(int key, TValue value)
        {
            int freeIndex = FreeListHead;
            int hashCode = key & 0x7FFFFFFF;
            int hashIndex = hashCode % Buckets.Length;
            int indexAtBucket = Buckets[hashIndex];

            int nextKeyIndex = indexAtBucket;

            while (nextKeyIndex > -1)
            {
                IntKeyEntry currEntry = Keys[nextKeyIndex];
                nextKeyIndex = currEntry.next;
                if (currEntry.key == key)
                {
                    return false;
                }
            }

            IntKeyEntry keyEntry = Keys[freeIndex];

            FreeListHead = keyEntry.next & 0x7FFFFFFF;
            keyEntry.next = indexAtBucket;
            keyEntry.key = key;
            Buckets[hashIndex] = freeIndex;

            Values[freeIndex] = value;
            Keys[freeIndex] = keyEntry;

            ++Count;
            return true;
        }

        public bool AddSafe(int key, TValue value)
        {
            int freeIndex = FreeListHead;

            if (freeIndex > Buckets.Length)
            {
                Expand();
            }

            int hashCode = key & 0x7FFFFFFF;
            int hashIndex = hashCode % Buckets.Length;
            int indexAtBucket = Buckets[hashIndex];

            int nextKeyIndex = indexAtBucket;

            while (nextKeyIndex != -1)
            {
                IntKeyEntry currEntry = Keys[nextKeyIndex];
                nextKeyIndex = currEntry.next;
                if (currEntry.key == key)
                {
                    return false;
                }
            }

            IntKeyEntry keyEntry = Keys[freeIndex];

            FreeListHead = keyEntry.next & 0x7FFFFFFF;
            keyEntry.next = indexAtBucket;
            keyEntry.key = key;
            Buckets[hashIndex] = freeIndex;

            Values[freeIndex] = value;
            Keys[freeIndex] = keyEntry;

            ++Count;
            return true;
        }

        public void AddUnchecked(int key, TValue value)
        {
            int dataIndex = FreeListHead;
            int hashCode = key & 0x7FFFFFFF;
            int bucketIndex = hashCode % Buckets.Length;
            int initialBucketValue = Buckets[bucketIndex];
            IntKeyEntry keyEntry = Keys[dataIndex];

            FreeListHead = keyEntry.next & 0x7FFFFFFF;
            keyEntry.next = initialBucketValue;
            keyEntry.key = key;
            Buckets[bucketIndex] = dataIndex;

            Values[dataIndex] = value;
            Keys[dataIndex] = keyEntry;
        }

        public void Expand()
        {
            int oldCapacity = Buckets.Length;
            int nextPrimeCapacity = DictionaryPrimes.GetNextSize(oldCapacity);

            int[] newBuckets = new int[nextPrimeCapacity];
            for (int i = 0; i < nextPrimeCapacity; i++)
            {
                newBuckets[i] = int.MaxValue;
            }

            TValue[] newValues = new TValue[nextPrimeCapacity];
            Array.Copy(Values, 0, newValues, 0, oldCapacity);
            IntKeyEntry[] newKeys = new IntKeyEntry[nextPrimeCapacity];
            Array.Copy(Keys, 0, newKeys, 0, oldCapacity);

            for (int i = oldCapacity; i < nextPrimeCapacity; i++)
            {
                newKeys[i].next = (i + 1) | (1 << 31);
            }

            for (int i = 0; i < oldCapacity; i++)
            {
                IntKeyEntry entry = newKeys[i];

                if (entry.next >= 0)
                {
                    int newBucketIndex = (entry.key & 0x7FFFFFFF) % nextPrimeCapacity;

                    int indexAtBucket = newBuckets[newBucketIndex];
                    entry.next = indexAtBucket;
                    newBuckets[newBucketIndex] = i;
                    newKeys[i] = entry;
                }
            }

            Buckets = newBuckets;
            Keys = newKeys;
            Values = newValues;
        }

        // Right now this doesn't do anything different/faster, it just doesn't return a bool.
        // TODO: look into if this should even exist/if there is a way to do this faster.
        public void ModifyValueUnchecked(int key, TValue value)
        {
            int hashCode = key & 0x7FFFFFFF;
            int bucketIndex = hashCode % Buckets.Length;
            int nextKeyIndex = Buckets[bucketIndex];

            while (unchecked((uint)nextKeyIndex) < unchecked((uint)int.MaxValue))
            {
                IntKeyEntry currEntry = Keys[nextKeyIndex];
                if (currEntry.key == key)
                {
                    Values[nextKeyIndex] = value;
                    return;
                }

                nextKeyIndex = currEntry.next & 0x7FFFFFFF;
            }
        }

        public bool TryModifyValue(int key, TValue value)
        {
            int hashCode = key & 0x7FFFFFFF;
            int bucketIndex = hashCode % Buckets.Length;
            int nextKeyIndex = Buckets[bucketIndex];

            while (unchecked((uint)nextKeyIndex) < unchecked((uint)int.MaxValue))
            {
                IntKeyEntry currEntry = Keys[nextKeyIndex];
                if (currEntry.key == key)
                {
                    Values[nextKeyIndex] = value;
                    return true;
                }

                nextKeyIndex = currEntry.next & 0x7FFFFFFF;
            }

            return false;
        }

        public void Remove(int key)
        {
            int hashCode = key & 0x7FFFFFFF;
            int bucketIndex = hashCode % Buckets.Length;
            int nextKeyIndex = Buckets[bucketIndex];

            while (unchecked((uint)nextKeyIndex) < unchecked((uint)int.MaxValue))
            {
                IntKeyEntry currEntry = Keys[nextKeyIndex];
                if (currEntry.key == key)
                {
                    Values[nextKeyIndex] = default;
                    int nextFreeIndex = FreeListHead;
                    currEntry.next = nextFreeIndex | (1 << 31);
                    FreeListHead = nextKeyIndex;
                    Keys[nextKeyIndex] = currEntry;
                    return;
                }

                nextKeyIndex = currEntry.next & 0x7FFFFFFF;
            }
        }

        public bool TryRemove(int key)
        {
            int hashCode = key & 0x7FFFFFFF;
            int bucketIndex = hashCode % Buckets.Length;
            int nextKeyIndex = Buckets[bucketIndex];

            while (unchecked((uint)nextKeyIndex) < unchecked((uint)int.MaxValue))
            {
                IntKeyEntry currEntry = Keys[nextKeyIndex];
                if (currEntry.key == key)
                {
                    Values[nextKeyIndex] = default;
                    int nextFreeIndex = FreeListHead;
                    currEntry.next = nextFreeIndex | (1 << 31);
                    FreeListHead = nextKeyIndex;
                    Keys[nextKeyIndex] = currEntry;
                    return true;
                }

                nextKeyIndex = currEntry.next & 0x7FFFFFFF;
            }

            return false;
        }

        public TValue this[int key]
        {
            get
            {
                int hashCode = key & 0x7FFFFFFF;
                int bucketIndex = hashCode % Buckets.Length;
                int nextKeyIndex = Buckets[bucketIndex];


                while (unchecked((uint)nextKeyIndex) < unchecked((uint)int.MaxValue))
                {
                    IntKeyEntry currEntry = Keys[nextKeyIndex];
                    if (currEntry.key == key)
                    {
                        return Values[nextKeyIndex];
                    }

                    nextKeyIndex = currEntry.next & 0x7FFFFFFF;
                }

                throw new System.Collections.Generic.KeyNotFoundException();
            }

            set
            {
                int hashCode = key & 0x7FFFFFFF;
                int bucketIndex = hashCode % Buckets.Length;
                int nextKeyIndex = Buckets[bucketIndex];

                while (unchecked((uint)nextKeyIndex) < unchecked((uint)int.MaxValue))
                {
                    IntKeyEntry currEntry = Keys[nextKeyIndex];
                    if (currEntry.key == key)
                    {
                        Values[nextKeyIndex] = value;
                        return;
                    }

                    nextKeyIndex = currEntry.next & 0x7FFFFFFF;
                }

                int freeIndex = FreeListHead;

                if (freeIndex > Buckets.Length)
                {
                    Expand();
                }

                int indexAtBucket = Buckets[bucketIndex];
                IntKeyEntry keyEntry = Keys[freeIndex];

                FreeListHead = keyEntry.next & 0x7FFFFFFF;
                keyEntry.next = indexAtBucket;
                keyEntry.key = key;
                Buckets[bucketIndex] = freeIndex;

                Values[freeIndex] = value;
                Keys[freeIndex] = keyEntry;

                ++Count;
            }
        }

        // TODO: figure out enumeration for foreach syntax, or if it's even worth it.
        public DictionaryMoveNextCode MoveNext(int index, int iteratorVersion, in int dictionaryVersion, out int key, out TValue value)
        {
            if (iteratorVersion != dictionaryVersion)
            {
                key = 0;
                value = default(TValue);
                return DictionaryMoveNextCode.InvalidVersion;
            }

            while (index < Keys.Length)
            {
                IntKeyEntry keyEntry = Keys[index];

                if ((keyEntry.key & 0x7FFFFFFF) >= 0)
                {
                    key = keyEntry.key;
                    value = Values[index];
                    index++;
                    return DictionaryMoveNextCode.FoundEntry;
                }
                index++;
            }

            index = Keys.Length + 1;
            key = 0;
            value = default(TValue);
            return DictionaryMoveNextCode.NoEntryFound;
        }
    }

    public struct IntKeyEntry
    {
        public int key;
        public int next;
    }

    public enum DictionaryMoveNextCode : byte
    {
        NoEntryFound = 0,
        FoundEntry = 1,
        InvalidVersion = 2
    }
}