using System;

namespace GDX.Collections.Generic
{
    /// <summary>
    ///     An optimized <see cref="System.Collections.Generic.Dictionary{T,T}" />-like data structure with a
    ///     <see cref="string"/> key requirement.
    /// </summary>
    [Serializable]
    public struct IntKeyDictionary<TValue>
    {
        public int[] Buckets;
        public IntKeyEntry<TValue>[] Entries;
        public int FreeListHead;
        public int Count;

        /// <summary>
        /// Initializes the dictionary with at least <paramref name="minCapacity"/> capacity.
        /// </summary>
        /// <param name="minCapacity">The minimal initial capacity to reserve.</param>
        public IntKeyDictionary(int minCapacity)
        {
            int primeCapacity = DictionaryPrimes.GetPrime(minCapacity);

            Buckets = new int[primeCapacity];
            for (int i = 0; i < primeCapacity; i++)
            {
                Buckets[i] = int.MaxValue;
            }

            Entries = new IntKeyEntry<TValue>[primeCapacity];

            for (int i = 0; i < primeCapacity; i++)
            {
                Entries[i].Next = (1 << 31) | (i + 1);
            }

            Count = 0;
            FreeListHead = 0;
        }

        /// <summary>
        /// Adds the key value pair to the dictionary, expanding if necessary but not checking for duplicate entries.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        public void AddWithExpandCheck(int key, TValue value)
        {
            int freeIndex = FreeListHead;

            if (freeIndex >= Buckets.Length)
            {
                ExpandWhenFull();
            }

            int hashCode = key & 0x7FFFFFFF;
            int hashIndex = hashCode % Buckets.Length;
            int indexAtBucket = Buckets[hashIndex];
            ref IntKeyEntry<TValue> entry = ref Entries[freeIndex];

            FreeListHead = entry.Next & 0x7FFFFFFF;
            entry.Next = indexAtBucket;
            entry.Key = key;
            entry.Value = value;
            Buckets[hashIndex] = freeIndex;

            ++Count;
        }

        /// <summary>
        /// Adds the key value pair to the dictionary, checking for duplicates but not expanding if necessary.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        /// <returns>True if the entry was successfully created.</returns>
        public bool AddWithUniqueCheck(int key, TValue value)
        {
            int freeIndex = FreeListHead;
            int hashCode = key & 0x7FFFFFFF;
            int hashIndex = hashCode % Buckets.Length;
            int indexAtBucket = Buckets[hashIndex];

            int nextKeyIndex = indexAtBucket;

            while (nextKeyIndex != int.MaxValue)
            {
                ref IntKeyEntry<TValue> currEntry = ref Entries[nextKeyIndex];
                nextKeyIndex = currEntry.Next;
                if (currEntry.Key == key)
                {
                    return false;
                }
            }

            ref IntKeyEntry<TValue> entry = ref Entries[freeIndex];

            FreeListHead = entry.Next & 0x7FFFFFFF;
            entry.Next = indexAtBucket;
            entry.Key = key;
            entry.Value = value;
            Buckets[hashIndex] = freeIndex;

            ++Count;
            return true;
        }

        /// <summary>
        /// Adds the key value pair to the dictionary, checking for duplicate entries and expanding if necessary.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        /// <returns>True if the entry was successfully created.</returns>
        public bool AddSafe(int key, TValue value)
        {
            int freeIndex = FreeListHead;

            if (freeIndex >= Buckets.Length)
            {
                ExpandWhenFull();
            }

            int hashCode = key & 0x7FFFFFFF;
            int hashIndex = hashCode % Buckets.Length;
            int indexAtBucket = Buckets[hashIndex];

            int nextKeyIndex = indexAtBucket;

            while (nextKeyIndex != int.MaxValue)
            {
                IntKeyEntry<TValue> currEntry = Entries[nextKeyIndex];
                nextKeyIndex = currEntry.Next;
                if (currEntry.Key == key)
                {
                    return false;
                }
            }

            ref IntKeyEntry<TValue> entry = ref Entries[freeIndex];

            FreeListHead = entry.Next & 0x7FFFFFFF;
            entry.Next = indexAtBucket;
            entry.Key = key;
            entry.Value = value;
            Buckets[hashIndex] = freeIndex;

            ++Count;
            return true;
        }

        /// <summary>
        /// Adds the key value pair to the dictionary, without checking for available capacity or duplicate entries.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        public void AddUnchecked(int key, TValue value)
        {
            int dataIndex = FreeListHead;
            int hashCode = key & 0x7FFFFFFF;
            int bucketIndex = hashCode % Buckets.Length;
            int initialBucketValue = Buckets[bucketIndex];

            ref IntKeyEntry<TValue> entry = ref Entries[dataIndex];

            FreeListHead = entry.Next & 0x7FFFFFFF;
            entry.Next = initialBucketValue;
            entry.Key = key;
            entry.Value = value;
            Buckets[bucketIndex] = dataIndex;
        }


        /// <summary>
        /// Checks if the dictionary contains the given key.
        /// </summary>
        /// <param name="key">The key to check for.</param>
        /// <returns>True if the dictionary contains the key.</returns>
        public bool ContainsKey(int key)
        {
            int hashCode = key & 0x7FFFFFFF;
            int bucketIndex = hashCode % Buckets.Length;
            int nextKeyIndex = Buckets[bucketIndex];

            while (nextKeyIndex != int.MaxValue)
            {
                ref IntKeyEntry<TValue> currEntry = ref Entries[nextKeyIndex];

                if (currEntry.Key == key)
                {
                    return true;
                }

                nextKeyIndex = currEntry.Next;
            }

            return false;
        }

        /// <summary>
        /// Resizes the dictionary with the assumption that it is full. Do not use otherwise.
        /// </summary>
        public void ExpandWhenFull()
        {
            int oldCapacity = Buckets.Length;
            int nextPrimeCapacity = DictionaryPrimes.GetNextSize(oldCapacity);

            int[] newBuckets = new int[nextPrimeCapacity];
            for (int i = 0; i < nextPrimeCapacity; i++)
            {
                newBuckets[i] = int.MaxValue;
            }

            IntKeyEntry<TValue>[] newEntries = new IntKeyEntry<TValue>[nextPrimeCapacity];
            Array.Copy(Entries, 0, newEntries, 0, oldCapacity);

            for (int i = 0; i < oldCapacity; i++)
            {
                ref IntKeyEntry<TValue> entry = ref newEntries[i];

                int newBucketIndex = (entry.Key & 0x7FFFFFFF) % nextPrimeCapacity;

                int indexAtBucket = newBuckets[newBucketIndex];
                entry.Next = indexAtBucket;
                newBuckets[newBucketIndex] = i;
            }

            for (int i = oldCapacity; i < nextPrimeCapacity; i++)
            {
                newEntries[i].Next = (1 << 31) | (i + 1);
            }

            Buckets = newBuckets;
            Entries = newEntries;
        }

        /// <summary>
        /// Expands the dictionary if it does not have enough empty space for <paramref name="capacityToReserve"/>.
        /// </summary>
        /// <param name="capacityToReserve"></param>
        public void Reserve(int capacityToReserve)
        {
            int oldCapacity = Entries.Length;
            if (Count + capacityToReserve > oldCapacity)
            {
                int minCapacity = Count + capacityToReserve;
                int nextPrimeCapacity = DictionaryPrimes.GetNextSize(minCapacity);

                int[] newBuckets = new int[nextPrimeCapacity];
                for (int i = 0; i < nextPrimeCapacity; i++)
                {
                    newBuckets[i] = int.MaxValue;
                }

                IntKeyEntry<TValue>[] newEntries = new IntKeyEntry<TValue>[nextPrimeCapacity];
                Array.Copy(Entries, 0, newEntries, 0, oldCapacity);

                for (int i = 0; i < oldCapacity; i++)
                {
                    ref IntKeyEntry<TValue> entry = ref newEntries[i];

                    if (entry.Next >= 0)
                    {
                        int newBucketIndex = (entry.Key & 0x7FFFFFFF) % nextPrimeCapacity;

                        int indexAtBucket = newBuckets[newBucketIndex];
                        entry.Next = indexAtBucket;
                        newBuckets[newBucketIndex] = i;
                    }
                }

                for (int i = oldCapacity; i < nextPrimeCapacity; i++)
                {
                    newEntries[i].Next = (1 << 31) | (i + 1);
                }

                Buckets = newBuckets;
                Entries = newEntries;
            }
        }

        /// <summary>
        /// Finds the index of the entry corresponding to a key.
        /// </summary>
        /// <param name="key">The key to find the index of.</param>
        /// <returns>The index of the entry, or -1 if the entry does not exist.</returns>
        public int IndexOf(int key)
        {
            int hashCode = key & 0x7FFFFFFF;
            int bucketIndex = hashCode % Buckets.Length;
            int nextKeyIndex = Buckets[bucketIndex];

            while (nextKeyIndex != int.MaxValue)
            {
                ref IntKeyEntry<TValue> currEntry = ref Entries[nextKeyIndex];

                if (currEntry.Key == key)
                {
                    return nextKeyIndex;
                }

                nextKeyIndex = currEntry.Next;
            }

            return -1;
        }

        /// <summary>
        /// Replaces the value of the entry if the entry exists.
        /// </summary>
        /// <param name="key">The key of the entry to modify.</param>
        /// <param name="value">The new value of the entry.</param>
        /// <returns>True if the entry was found.</returns>
        public bool TryModifyValue(int key, TValue value)
        {
            int hashCode = key & 0x7FFFFFFF;
            int bucketIndex = hashCode % Buckets.Length;
            int nextKeyIndex = Buckets[bucketIndex];

            while (nextKeyIndex != int.MaxValue)
            {
                ref IntKeyEntry<TValue> currEntry = ref Entries[nextKeyIndex];
                nextKeyIndex = currEntry.Next;

                if (currEntry.Key == key)
                {
                    currEntry.Value = value;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes the entry if it exists.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        /// <returns>True if the entry was found.</returns>
        public bool TryRemove(int key)
        {
            int hashCode = key & 0x7FFFFFFF;
            int bucketIndex = hashCode % Buckets.Length;
            int indexAtBucket = Buckets[bucketIndex];
            int indexOfKey = indexAtBucket;
            int previousIndex = indexAtBucket;

            bool foundIndex = false;

            while (indexOfKey != int.MaxValue)
            {
                ref IntKeyEntry<TValue> currEntry = ref Entries[indexOfKey];

                if (currEntry.Key == key)
                {
                    foundIndex = true;
                    break;
                }

                previousIndex = indexOfKey;
                indexOfKey = currEntry.Next;
            }

            if (foundIndex)
            {
                ref IntKeyEntry<TValue> currEntry = ref Entries[indexOfKey];
                int nextUsedIndex = currEntry.Next;
                int nextFreeIndex = FreeListHead;

                currEntry.Value = default;
                currEntry.Next = nextFreeIndex | (1 << 31);
                Entries[indexOfKey] = currEntry;
                FreeListHead = indexOfKey;

                if (indexOfKey == indexAtBucket)
                {
                    Buckets[bucketIndex] = nextUsedIndex;
                }
                else
                {
                    Entries[previousIndex].Next = nextUsedIndex;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the entry if it exists, but does not remove the value of the key value pair.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        /// <returns>True if the entry was found.</returns>
        public bool TryRemoveNoValueClear(int key)
        {
            int hashCode = key & 0x7FFFFFFF;
            int bucketIndex = hashCode % Buckets.Length;
            int indexAtBucket = Buckets[bucketIndex];
            int indexOfKey = indexAtBucket;
            int previousIndex = indexAtBucket;

            bool foundIndex = false;

            while (indexOfKey != int.MaxValue)
            {
                ref IntKeyEntry<TValue> currEntry = ref Entries[indexOfKey];

                if (currEntry.Key == key)
                {
                    foundIndex = true;
                    break;
                }

                previousIndex = indexOfKey;
                indexOfKey = currEntry.Next;
            }

            if (foundIndex)
            {
                ref IntKeyEntry<TValue> currEntry = ref Entries[indexOfKey];
                int nextUsedIndex = currEntry.Next;
                int nextFreeIndex = FreeListHead;

                currEntry.Next = nextFreeIndex | (1 << 31);
                Entries[indexOfKey] = currEntry;
                FreeListHead = indexOfKey;

                if (indexOfKey == indexAtBucket)
                {
                    Buckets[bucketIndex] = nextUsedIndex;
                }
                else
                {
                    Entries[previousIndex].Next = nextUsedIndex;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to get the value for the given key; returns true if key was found, false otherwise.
        /// </summary>
        /// <param name="key">The key to retrieve.</param>
        /// <param name="value">The value of the entry found.</param>
        /// <returns>True if the entry was found; false otherwise.</returns>
        public bool TryGetValue(int key, out TValue value)
        {
            int hashCode = key & 0x7FFFFFFF;
            int bucketIndex = hashCode % Buckets.Length;
            int nextKeyIndex = Buckets[bucketIndex];

            while (nextKeyIndex != int.MaxValue)
            {
                ref IntKeyEntry<TValue> currEntry = ref Entries[nextKeyIndex];
                nextKeyIndex = currEntry.Next;

                if (currEntry.Key == key)
                {
                    value = currEntry.Value;
                    return true;
                }
            }

            value = default;
            return false;
        }

        /// <summary>
        ///     Directly access a value by key.
        /// </summary>
        /// <param name="key">The target key to look for a value identified by.</param>
        /// <exception cref="ArgumentNullException">Thrown when a null <paramref name="key"/> is provided to lookup.</exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when the <paramref name="key"/> is not found in the <see cref="StringKeyDictionary{TValue}"/>.</exception>
        public TValue this[int key]
        {
            get
            {
                int hashCode = key & 0x7FFFFFFF;
                int bucketIndex = hashCode % Buckets.Length;
                int nextKeyIndex = Buckets[bucketIndex];


                while (nextKeyIndex != int.MaxValue)
                {
                    ref IntKeyEntry<TValue> currEntry = ref Entries[nextKeyIndex];
                    nextKeyIndex = currEntry.Next;

                    if (currEntry.Key == key)
                    {
                        return currEntry.Value;
                    }
                }

                throw new System.Collections.Generic.KeyNotFoundException();
            }

            set
            {
                int freeIndex = FreeListHead;

                if (freeIndex >= Buckets.Length)
                {
                    ExpandWhenFull();
                }

                int hashCode = key & 0x7FFFFFFF;
                int bucketIndex = hashCode % Buckets.Length;
                int indexAtBucket = Buckets[bucketIndex];

                int nextKeyIndex = indexAtBucket;

                while (nextKeyIndex != int.MaxValue)
                {
                    ref IntKeyEntry<TValue> currEntry = ref Entries[nextKeyIndex];
                    nextKeyIndex = currEntry.Next;
                    if (currEntry.Key == key)
                    {
                        currEntry.Value = value;
                        return;
                    }
                }

                ref IntKeyEntry<TValue> entry = ref Entries[freeIndex];

                FreeListHead = entry.Next & 0x7FFFFFFF;
                entry.Next = indexAtBucket;
                entry.Key = key;
                entry.Value = value;
                Buckets[bucketIndex] = freeIndex;

                ++Count;
            }
        }

        /// <summary>
        /// Iterates the dictionary.
        /// </summary>
        /// <param name="iteratedIndexCount">The number of indices iterated so far - pass in 0 at the start of iteration.</param>
        /// <param name="iteratorVersion">The version when iteration started.</param>
        /// <param name="dictionaryVersion">The current version of the dictionary - update this on add, remove, or clear operations.</param>
        /// <param name="entry">The entry returned by the iterator</param>
        /// <returns>Whether the iterator found an entry, finished iteration, or could not continue due to an invalid version.</returns>
        public IteratorState MoveNext(ref int iteratedIndexCount, int iteratorVersion, in int dictionaryVersion, out IntKeyEntry<TValue> entry)
        {
            entry = default;

            if (iteratorVersion != dictionaryVersion)
            {
                return IteratorState.InvalidVersion;
            }

            while (iteratedIndexCount < Entries.Length)
            {
                ref IntKeyEntry<TValue> keyEntry = ref Entries[iteratedIndexCount];
                iteratedIndexCount++;

                if (keyEntry.Next >= 0)
                {
                    entry = keyEntry;
                    return IteratorState.FoundEntry;
                }
            }

            return IteratorState.Finished;
        }

        /// <summary>
        /// Iterates the dictionary.
        /// NOTE: if you suspect the dictionary might be modified while iterating, this will not catch the error -- use the other overload instead.
        /// </summary>
        /// <param name="iteratedIndexCount">The number of indices iterated so far - pass in 0 at the start of iteration.</param>
        /// <param name="entry">The entry returned by the iterator</param>
        /// <returns>Whether or not the iterator found an entry</returns>
        public bool MoveNext(ref int iteratedIndexCount, out IntKeyEntry<TValue> entry)
        {
            entry = default;

            while (iteratedIndexCount < Entries.Length)
            {
                ref IntKeyEntry<TValue> keyEntry = ref Entries[iteratedIndexCount];
                iteratedIndexCount++;

                if (keyEntry.Next >= 0)
                {
                    entry = keyEntry;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Iterates the dictionary.
        /// NOTE: if you suspect the dictionary might be modified while iterating, this will not catch the error -- use the other overload instead.
        /// </summary>
        /// <param name="iteratedIndexCount">The number of indices iterated so far - pass in 0 at the start of iteration.</param>
        /// <returns>Whether or not the iterator found an entry</returns>
        public bool MoveNext(ref int iteratedIndexCount)
        {
            while (iteratedIndexCount < Entries.Length)
            {
                ref IntKeyEntry<TValue> keyEntry = ref Entries[iteratedIndexCount];
                iteratedIndexCount++;

                if (keyEntry.Next >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Clears the dictionary.
        /// </summary>
        public void Clear()
        {
            int length = Entries.Length;

            for (int i = 0; i < length; i++)
            {
                Buckets[i] = -1;
            }

            for (int i = 0; i < length; i++)
            {
                ref IntKeyEntry<TValue> entryAt = ref Entries[i];
                entryAt.Next = (1 << 31) | (i + 1);
                entryAt.Value = default;
            }

            FreeListHead = 0;
            Count = 0;
        }
    }

    [Serializable]
    public struct IntKeyEntry<T>
    {
        public int Key;
        public int Next;
        public T Value;
    }
}
