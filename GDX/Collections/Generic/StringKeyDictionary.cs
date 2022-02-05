using System;

namespace GDX.Collections.Generic
{
    [Serializable]
    public struct StringKeyDictionary<TValue>
    {
        public int[] Buckets;
        public StringKeyEntry<TValue>[] Entries;
        public int FreeListHead;
        public int Count;

        public StringKeyDictionary(int minCapacity)
        {
            int primeCapacity = DictionaryPrimes.GetPrime(minCapacity);

            Buckets = new int[primeCapacity];
            for (int i = 0; i < primeCapacity; i++)
            {
                Buckets[i] = -1;
            }

            Entries = new StringKeyEntry<TValue>[primeCapacity];

            for (int i = 0; i < primeCapacity; i++)
            {
                Entries[i].next = (1 << 31) | (i + 1);
            }

            Count = 0;
            FreeListHead = 0;
        }

        public void AddWithExpandCheck(string key, TValue value)
        {
            if (key == null) throw new ArgumentNullException();

            int freeIndex = FreeListHead;

            if (freeIndex >= Buckets.Length)
            {
                ExpandWhenFull();
            }

            int hashCode = key.GetStableHashCode() & 0x7FFFFFFF;
            int hashIndex = hashCode % Buckets.Length;
            int indexAtBucket = Buckets[hashIndex];
            ref StringKeyEntry<TValue> entry = ref Entries[freeIndex];

            FreeListHead = entry.next & 0x7FFFFFFF;
            entry.next = indexAtBucket;
            entry.key = key;
            entry.value = value;
            entry.hashCode = hashCode;
            Buckets[hashIndex] = freeIndex;

            ++Count;
        } 

        public bool AddWithUniqueCheck(string key, TValue value)
        {
            if (key == null) throw new ArgumentNullException();

            int freeIndex = FreeListHead;
            int hashCode = key.GetStableHashCode() & 0x7FFFFFFF;
            int hashIndex = hashCode % Buckets.Length;
            int indexAtBucket = Buckets[hashIndex];

            int nextKeyIndex = indexAtBucket;

            while (nextKeyIndex > -1)
            {
                ref StringKeyEntry<TValue> currEntry = ref Entries[nextKeyIndex];
                nextKeyIndex = currEntry.next;
                if (currEntry.key == key)
                {
                    return false;
                }
            }

            ref StringKeyEntry<TValue> entry = ref Entries[freeIndex];

            FreeListHead = entry.next & 0x7FFFFFFF;
            entry.next = indexAtBucket;
            entry.key = key;
            entry.value = value;
            entry.hashCode = hashCode;
            Buckets[hashIndex] = freeIndex;

            ++Count;
            return true;
        }

        public bool AddSafe(string key, TValue value)
        {
            if (key == null) throw new ArgumentNullException();

            int freeIndex = FreeListHead;

            if (freeIndex >= Buckets.Length)
            {
                ExpandWhenFull();
            }

            int hashCode = key.GetStableHashCode() & 0x7FFFFFFF;
            int hashIndex = hashCode % Buckets.Length;
            int indexAtBucket = Buckets[hashIndex];

            int nextKeyIndex = indexAtBucket;

            while (nextKeyIndex != -1)
            {
                StringKeyEntry<TValue> currEntry = Entries[nextKeyIndex];
                nextKeyIndex = currEntry.next;
                if (currEntry.key == key)
                {
                    return false;
                }
            }

            ref StringKeyEntry<TValue> entry = ref Entries[freeIndex];

            FreeListHead = entry.next & 0x7FFFFFFF;
            entry.next = indexAtBucket;
            entry.key = key;
            entry.value = value;
            entry.hashCode = hashCode;
            Buckets[hashIndex] = freeIndex;

            ++Count;
            return true;
        }

        public void AddUnchecked(string key, TValue value)
        {
            int dataIndex = FreeListHead;
            int hashCode = key.GetStableHashCode() & 0x7FFFFFFF;
            int bucketIndex = hashCode % Buckets.Length;
            int initialBucketValue = Buckets[bucketIndex];

            ref StringKeyEntry<TValue> entry = ref Entries[dataIndex];

            FreeListHead = entry.next & 0x7FFFFFFF;
            entry.next = initialBucketValue;
            entry.key = key;
            entry.value = value;
            entry.hashCode = hashCode;
            Buckets[bucketIndex] = dataIndex;
        }

        public void ExpandWhenFull()
        {
            int oldCapacity = Buckets.Length;
            int nextPrimeCapacity = DictionaryPrimes.GetNextSize(oldCapacity);

            int[] newBuckets = new int[nextPrimeCapacity];
            for (int i = 0; i < nextPrimeCapacity; i++)
            {
                newBuckets[i] = -1;
            }

            StringKeyEntry<TValue>[] newEntries = new StringKeyEntry<TValue>[nextPrimeCapacity];
            Array.Copy(Entries, 0, newEntries, 0, oldCapacity);

            for (int i = 0; i < oldCapacity; i++)
            {
                ref StringKeyEntry<TValue> entry = ref newEntries[i];

                int newBucketIndex = (entry.hashCode & 0x7FFFFFFF) % nextPrimeCapacity;

                int indexAtBucket = newBuckets[newBucketIndex];
                entry.next = indexAtBucket;
                newBuckets[newBucketIndex] = i;
            }

            for (int i = oldCapacity; i < nextPrimeCapacity; i++)
            {
                newEntries[i].next = (1 << 31) | (i + 1);
            }

            Buckets = newBuckets;
            Entries = newEntries;
        }

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
                    newBuckets[i] = -1;
                }

                StringKeyEntry<TValue>[] newEntries = new StringKeyEntry<TValue>[nextPrimeCapacity];
                Array.Copy(Entries, 0, newEntries, 0, oldCapacity);

                for (int i = 0; i < oldCapacity; i++)
                {
                    ref StringKeyEntry<TValue> entry = ref newEntries[i];

                    if (entry.key != null)
                    {
                        int newBucketIndex = (entry.hashCode & 0x7FFFFFFF) % nextPrimeCapacity;

                        int indexAtBucket = newBuckets[newBucketIndex];
                        entry.next = indexAtBucket;
                        newBuckets[newBucketIndex] = i;
                    }
                }

                for (int i = oldCapacity; i < nextPrimeCapacity; i++)
                {
                    newEntries[i].next = (1 << 31) | (i + 1);
                }

                Buckets = newBuckets;
                Entries = newEntries;
            }
        }

        public int IndexOf(string key)
        {
            if (key == null) throw new ArgumentNullException();

            int hashCode = key.GetStableHashCode() & 0x7FFFFFFF;
            int bucketIndex = hashCode % Buckets.Length;
            int nextKeyIndex = Buckets[bucketIndex];

            while (nextKeyIndex != -1)
            {
                ref StringKeyEntry<TValue> currEntry = ref Entries[nextKeyIndex];

                if (currEntry.key == key)
                {
                    return nextKeyIndex;
                }

                nextKeyIndex = currEntry.next;
            }

            return -1;
        }

        public bool TryModifyValue(string key, TValue value)
        {
            if (key == null) throw new ArgumentNullException();

            int hashCode = key.GetStableHashCode() & 0x7FFFFFFF;
            int bucketIndex = hashCode % Buckets.Length;
            int nextKeyIndex = Buckets[bucketIndex];

            while (nextKeyIndex != -1)
            {
                ref StringKeyEntry<TValue> currEntry = ref Entries[nextKeyIndex];
                nextKeyIndex = currEntry.next;

                if (currEntry.key == key)
                {
                    currEntry.value = value;
                    return true;
                }
            }

            return false;
        }

        public bool TryRemove(string key)
        {
            if (key == null) throw new ArgumentNullException();

            int hashCode = key.GetStableHashCode() & 0x7FFFFFFF;
            int bucketIndex = hashCode % Buckets.Length;
            int indexAtBucket = Buckets[bucketIndex];
            int indexOfKey = indexAtBucket;
            int previousIndex = indexAtBucket;

            bool foundIndex = false;

            while (indexOfKey != -1)
            {
                ref StringKeyEntry<TValue> currEntry = ref Entries[indexOfKey];

                if (currEntry.key == key)
                {
                    foundIndex = true;
                    break;
                }

                previousIndex = indexOfKey;
                indexOfKey = currEntry.next;
            }

            if (foundIndex)
            {
                ref StringKeyEntry<TValue> currEntry = ref Entries[indexOfKey];
                int nextUsedIndex = currEntry.next;
                int nextFreeIndex = FreeListHead;

                currEntry.key = null;
                currEntry.value = default(TValue);
                currEntry.hashCode = 0;
                currEntry.next = nextFreeIndex | (1 << 31);
                Entries[indexOfKey] = currEntry;
                FreeListHead = indexOfKey;

                if (indexOfKey == indexAtBucket)
                {
                    Buckets[bucketIndex] = nextUsedIndex;
                }
                else
                {
                    Entries[previousIndex].next = nextUsedIndex;
                }

                return true;
            }

            return false;
        }

        public TValue this[string key]
        {
            get
            {
                if (key == null) throw new ArgumentNullException();

                int hashCode = key.GetStableHashCode() & 0x7FFFFFFF;
                int bucketIndex = hashCode % Buckets.Length;
                int nextKeyIndex = Buckets[bucketIndex];


                while (nextKeyIndex != -1)
                {
                    ref StringKeyEntry<TValue> currEntry = ref Entries[nextKeyIndex];
                    nextKeyIndex = currEntry.next;

                    if (currEntry.key == key)
                    {
                        return currEntry.value;
                    }
                }

                throw new System.Collections.Generic.KeyNotFoundException();
            }

            set
            {
                if (key == null) throw new ArgumentNullException();

                int freeIndex = FreeListHead;

                if (freeIndex >= Buckets.Length)
                {
                    ExpandWhenFull();
                }

                int hashCode = key.GetStableHashCode() & 0x7FFFFFFF;
                int bucketIndex = hashCode % Buckets.Length;
                int indexAtBucket = Buckets[bucketIndex];

                int nextKeyIndex = indexAtBucket;

                while (nextKeyIndex != -1)
                {
                    ref StringKeyEntry<TValue> currEntry = ref Entries[nextKeyIndex];
                    nextKeyIndex = currEntry.next;
                    if (currEntry.key == key)
                    {
                        currEntry.value = value;
                        return;
                    }
                }

                ref StringKeyEntry<TValue> entry = ref Entries[freeIndex];

                FreeListHead = entry.next & 0x7FFFFFFF;
                entry.next = indexAtBucket;
                entry.key = key;
                entry.value = value;
                entry.hashCode = hashCode;
                Buckets[bucketIndex] = freeIndex;

                ++Count;
            }
        }

        /// <summary>
        /// Iterates the dictionary.
        /// </summary>
        /// <param name="index">The number of indices iterated so far - pass in 0 at the start of iteration.</param>
        /// <param name="iteratorVersion">The version when iteration started.</param>
        /// <param name="dictionaryVersion">The current version of the dictionary - update this on add, remove, or clear operations.</param>
        /// <param name="entry">The entry returned by the iterator</param>
        /// <returns>Whether the iterator found an entry, finished iteration, or could not continue due to an invalid version.</returns>
        public IteratorState MoveNext(ref int index, int iteratorVersion, in int dictionaryVersion, out StringKeyEntry<TValue> entry)
        {
            entry = default(StringKeyEntry<TValue>);

            if (iteratorVersion != dictionaryVersion)
            {
                return IteratorState.InvalidVersion;
            }

            while (index < Entries.Length)
            {
                ref StringKeyEntry<TValue> keyEntry = ref Entries[index];
                index++;

                if (keyEntry.key != null)
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
        /// <param name="index">The number of indices iterated so far - pass in 0 at the start of iteration.</param>
        /// <param name="entry">The entry returned by the iterator</param>
        /// <returns>Whether or not the iterator found an entry</returns>
        public bool MoveNext(ref int index, out StringKeyEntry<TValue> entry)
        {
            entry = default(StringKeyEntry<TValue>);

            while (index < Entries.Length)
            {
                ref StringKeyEntry<TValue> keyEntry = ref Entries[index];
                index++;

                if (keyEntry.key != null)
                {
                    entry = keyEntry;

                    return true;
                }
            }

            return false;
        }
    }

    [Serializable]
    public struct StringKeyEntry<T>
    {
        public string key;
        public T value;
        public int next;
        public int hashCode;
    }
}
