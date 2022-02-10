// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Collections.Generic;
using NUnit.Framework;
using GDX;
// ReSharper disable HeapView.ObjectAllocation
// ReSharper disable UnusedVariable

namespace Runtime.Collections.Generic
{
    public class StringKeyDictionaryTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_RequestZeroMinimum_PrimeCapacity()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(0);

            Assert.IsTrue(dictionary.Count == 0 && dictionary.Entries.Length == DictionaryPrimes.primes[0]);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_RequestPrimeMinimum_PrimeCapacity()
        {
            int prime = DictionaryPrimes.primes[2];
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(prime);
            Assert.IsTrue(prime == dictionary.Entries.Length);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_RequestBelowPrimeMinimum_PrimeCapacity()
        {
            int prime = DictionaryPrimes.primes[5];
            int minCapacity = prime - 1;
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(minCapacity);

            Assert.IsTrue(prime == dictionary.Entries.Length);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddSafe_CheckExists()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            string myKey = "myKey";
            string myValue = "myValue";
            dictionary.AddSafe(myKey, myValue);

            string storedValue = dictionary[myKey];

            Assert.IsTrue(storedValue == myValue);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IndexerAdd_CheckExists()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            string myKey = "myKey";
            string myValue = "myValue";
            dictionary[myKey] = myValue;

            string storedValue = dictionary[myKey];

            Assert.IsTrue(storedValue == myValue);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheck_CheckExists()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            string myKey = "myKey";
            string myValue = "myValue";
            dictionary.AddWithExpandCheck(myKey, myValue);

            string storedValue = dictionary[myKey];

            Assert.IsTrue(storedValue == myValue);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheck_CheckExpanded()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            string initialKey = "myKey";
            string myValue = "myValue";
            char val = 'A';

            string addedKey = initialKey;
            int initialCapacity = dictionary.Entries.Length;
            for (int i = 0; i < initialCapacity; i++)
            {
                dictionary.AddWithExpandCheck(addedKey, myValue);
                addedKey += val;
                val++;
            }

            string causesExpansion = "causesExpansion";

            dictionary.AddWithExpandCheck(causesExpansion, myValue);

            int expandedCapacity = dictionary.Entries.Length;

            Assert.IsTrue(expandedCapacity > initialCapacity);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheck_CheckEntriesMaintainConnection()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            string initialKey = "myKey";
            string myValue = "myValue";
            char val = 'A';

            string addedKey = initialKey;
            int initialCapacity = dictionary.Entries.Length;

            string[] keys = new string[initialCapacity];

            for (int i = 0; i < initialCapacity; i++)
            {
                keys[i] = addedKey;
                addedKey += val;
                val++;
            }

            for (int i = 0; i < initialCapacity; i++)
            {
                dictionary.AddWithExpandCheck(keys[i], myValue);
                addedKey += val;
                val++;
            }

            string causesExpansion = "causesExpansion";

            dictionary.AddWithExpandCheck(causesExpansion, myValue);

            bool allEntriesAreThere = true;
            for (int i = 0; i < initialCapacity; i++)
            {
                if (dictionary.IndexOf(keys[i]) == -1)
                {
                    allEntriesAreThere = false;
                    break;
                }
            }

            bool finalEntryIsThere = dictionary.IndexOf(causesExpansion) != -1;

            Assert.IsTrue(allEntriesAreThere && finalEntryIsThere);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddSafe_CheckExpanded()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            string initialKey = "myKey";
            string myValue = "myValue";
            char val = 'A';

            string addedKey = initialKey;
            int initialCapacity = dictionary.Entries.Length;
            for (int i = 0; i < initialCapacity; i++)
            {
                dictionary.AddSafe(addedKey, myValue);
                addedKey += val;
                val++;
            }

            string causesExpansion = "causesExpansion";

            dictionary.AddSafe(causesExpansion, myValue);

            int expandedCapacity = dictionary.Entries.Length;

            Assert.IsTrue(expandedCapacity > initialCapacity);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddSafe_CheckEntriesMaintainConnection()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            string initialKey = "myKey";
            string myValue = "myValue";
            char val = 'A';

            string addedKey = initialKey;
            int initialCapacity = dictionary.Entries.Length;

            string[] keys = new string[initialCapacity];

            for (int i = 0; i < initialCapacity; i++)
            {
                keys[i] = addedKey;
                addedKey += val;
                val++;
            }

            for (int i = 0; i < initialCapacity; i++)
            {
                dictionary.AddSafe(keys[i], myValue);
            }

            string causesExpansion = "causesExpansion";

            dictionary.AddSafe(causesExpansion, myValue);

            bool allEntriesAreThere = true;
            for (int i = 0; i < initialCapacity; i++)
            {
                if (dictionary.IndexOf(keys[i]) == -1)
                {
                    allEntriesAreThere = false;
                    break;
                }
            }

            bool finalEntryIsThere = dictionary.IndexOf(causesExpansion) != -1;

            Assert.IsTrue(allEntriesAreThere && finalEntryIsThere);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithIndexer_CheckExpanded()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            string initialKey = "myKey";
            string myValue = "myValue";
            char val = 'A';

            string addedKey = initialKey;
            int initialCapacity = dictionary.Entries.Length;
            for (int i = 0; i < initialCapacity; i++)
            {
                dictionary[addedKey] = myValue;
                addedKey += val;
                val++;
            }

            string causesExpansion = "causesExpansion";

            dictionary[causesExpansion] = myValue;

            int expandedCapacity = dictionary.Entries.Length;

            Assert.IsTrue(expandedCapacity > initialCapacity);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithIndexer_CheckEntriesMaintainConnection()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            string initialKey = "myKey";
            string myValue = "myValue";
            char val = 'A';

            string addedKey = initialKey;
            int initialCapacity = dictionary.Entries.Length;

            string[] keys = new string[initialCapacity];

            for (int i = 0; i < initialCapacity; i++)
            {
                keys[i] = addedKey;
                addedKey += val;
                val++;
            }

            for (int i = 0; i < initialCapacity; i++)
            {
                dictionary[keys[i]] = myValue;
            }

            string causesExpansion = "causesExpansion";

            dictionary[causesExpansion] = myValue;

            bool allEntriesAreThere = true;
            for (int i = 0; i < initialCapacity; i++)
            {
                if (dictionary.IndexOf(keys[i]) == -1)
                {
                    allEntriesAreThere = false;
                    break;
                }
            }

            bool finalEntryIsThere = dictionary.IndexOf(causesExpansion) != -1;

            Assert.IsTrue(allEntriesAreThere && finalEntryIsThere);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithUniqueCheck_CheckExists()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            string myKey = "myKey";
            string myValue = "myValue";
            dictionary.AddWithUniqueCheck(myKey, myValue);

            string storedValue = dictionary[myKey];

            Assert.IsTrue(storedValue == myValue);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_CheckExists()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            string myKey = "myKey";
            string myValue = "myValue";
            dictionary.AddUnchecked(myKey, myValue);

            string storedValue = dictionary[myKey];

            Assert.IsTrue(storedValue == myValue);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryRemove_CheckActuallyRemoved()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            string myKey = "myKey";
            string myValue = "myValue";
            dictionary.AddSafe(myKey, myValue);

            bool removedFirstTime = dictionary.TryRemove(myKey);

            StringKeyEntry<string>[] entries = dictionary.Entries;
            int arrayLength = entries.Length;

            int indexOfKey = -1;
            for (int i = 0; i < arrayLength; i++)
            {
                StringKeyEntry<string> entry = entries[i];
                if (entry.key == myKey)
                {
                    indexOfKey = i;
                    break;
                }
            }

            Assert.IsTrue(indexOfKey == -1);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryRemoveTwice_CheckCorrectReturnValues()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            string myKey = "myKey";
            string myValue = "myValue";
            dictionary.AddSafe(myKey, myValue);

            bool removedFirstTime = dictionary.TryRemove(myKey);
            bool removedSecondTime = dictionary.TryRemove(myKey);

            StringKeyEntry<string>[] entries = dictionary.Entries;
            int arrayLength = entries.Length;

            Assert.IsTrue(removedFirstTime && !removedSecondTime);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithUniqueCheckTwice_CheckAddedOnlyOnce()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            string myKey = "myKey";
            string myValue = "myValue";
            dictionary.AddWithUniqueCheck(myKey, myValue);
            bool failedToCatchDuplicate = dictionary.AddWithUniqueCheck(myKey, myValue);

            StringKeyEntry<string>[] entries = dictionary.Entries;
            int arrayLength = entries.Length;

            int indexOfKey = -1;
            for (int i = 0; i < arrayLength; i++)
            {
                StringKeyEntry<string> entry = entries[i];
                if (entry.key == myKey)
                {
                    indexOfKey = i;
                    break;
                }
            }

            int otherIndexOfKey = -1;

            if (indexOfKey != -1)
            {
                for (int i = indexOfKey + 1; i < arrayLength; i++)
                {
                    StringKeyEntry<string> entry = entries[i];
                    if (entry.key == myKey)
                    {
                        otherIndexOfKey = i;
                        break;
                    }
                }
            }

            Assert.IsTrue(!failedToCatchDuplicate && indexOfKey != -1 && otherIndexOfKey == -1);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Reserve_CheckExpandedCapacity()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);

            int initialCapacity = dictionary.Entries.Length;

            int reserveSize = initialCapacity + 50;

            dictionary.Reserve(reserveSize);

            Assert.IsTrue(dictionary.Entries.Length >= reserveSize);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IndexOf_CheckCorrectIndex()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            string myKey = "myKey";
            string myValue = "myValue";
            dictionary.AddUnchecked(myKey, myValue);
            int allegedIndex = dictionary.IndexOf(myKey);

            StringKeyEntry<string>[] entries = dictionary.Entries;
            int arrayLength = entries.Length;

            int realIndex = -1;
            for (int i = 0; i < arrayLength; i++)
            {
                StringKeyEntry<string> entry = entries[i];
                if (entry.key == myKey)
                {
                    realIndex = i;
                    break;
                }
            }

            Assert.IsTrue(realIndex != -1 && allegedIndex == realIndex);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IndexOf_CheckEntryDoesNotExist()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            string myKey = "myKey";
            int allegedIndex = dictionary.IndexOf(myKey);

            Assert.IsTrue(allegedIndex == -1);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddTwoCollidingEntries_CheckAccess()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            string collidingKey0 = "942";
            string collidingKey1 = "9331582";
            string myValue0 = "value0";
            string myValue1 = "value1";
            dictionary.AddUnchecked(collidingKey0, myValue0);
            dictionary.AddUnchecked(collidingKey1, myValue1);
            int allegedIndex0 = dictionary.IndexOf(collidingKey0);
            int allegedIndex1 = dictionary.IndexOf(collidingKey1);
            StringKeyEntry<string>[] entries = dictionary.Entries;
            int arrayLength = entries.Length;

            int realIndex0 = -1;

            for (int i = 0; i < arrayLength; i++)
            {
                StringKeyEntry<string> entry = entries[i];
                if (entry.key == collidingKey0 && entry.value == myValue0)
                {
                    realIndex0 = i;
                    break;
                }
            }

            int realIndex1 = -1;
            int nextIndex = -1;
            for (int i = 0; i < arrayLength; i++)
            {
                StringKeyEntry<string> entry = entries[i];
                if (entry.key == collidingKey1 && entry.value == myValue1)
                {
                    realIndex1 = i;
                    nextIndex = entry.next;
                    break;
                }
            }

            int stableHashCode0 = collidingKey0.GetStableHashCode();
            int stableHashCode1 = collidingKey1.GetStableHashCode();

            Assert.IsTrue(allegedIndex0 == realIndex0 && allegedIndex1 == realIndex1 && allegedIndex0 != allegedIndex1 && nextIndex == allegedIndex0 && stableHashCode0 == stableHashCode1);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddTwoCollidingEntries_RemoveFirst_CheckSecondExists()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            string collidingKey0 = "942";
            string collidingKey1 = "9331582";
            string myValue0 = "value0";
            string myValue1 = "value1";
            dictionary.AddUnchecked(collidingKey0, myValue0);
            dictionary.AddUnchecked(collidingKey1, myValue1);

            int allegedIndex1 = dictionary.IndexOf(collidingKey1);

            dictionary.TryRemove(collidingKey0);

            int allegedIndex1After = dictionary.IndexOf(collidingKey1);

            int stableHashCode0 = collidingKey0.GetStableHashCode();
            int stableHashCode1 = collidingKey1.GetStableHashCode();

            Assert.IsTrue(allegedIndex1 == allegedIndex1After && allegedIndex1After != -1 && stableHashCode0 == stableHashCode1);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddTwoCollidingEntries_RemoveSecond_CheckFirstExists()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            string collidingKey0 = "942";
            string collidingKey1 = "9331582";
            string myValue0 = "value0";
            string myValue1 = "value1";
            dictionary.AddUnchecked(collidingKey0, myValue0);
            dictionary.AddUnchecked(collidingKey1, myValue1);

            int allegedIndex0 = dictionary.IndexOf(collidingKey0);

            dictionary.TryRemove(collidingKey1);

            int allegedIndex0After = dictionary.IndexOf(collidingKey0);

            int stableHashCode0 = collidingKey0.GetStableHashCode();
            int stableHashCode1 = collidingKey1.GetStableHashCode();

            Assert.IsTrue(allegedIndex0 == allegedIndex0After && allegedIndex0After != -1 && stableHashCode0 == stableHashCode1);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void MoveNext_FindAllEntries()
        {
            string firstKey = "firstKey";
            string secondKey = "secondKey";
            string myValue = "myValue";

            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);

            dictionary.AddUnchecked(firstKey, myValue);
            dictionary.AddUnchecked(secondKey, myValue);

            int foundCount = 0;
            bool foundFirst = false;
            bool foundSecond = false;
            int currentIndex = 0;

            while (dictionary.MoveNext(ref currentIndex, out StringKeyEntry<string> currentEntry))
            {
                if (currentEntry.key == firstKey)
                {
                    foundFirst = true;
                }
                else if (currentEntry.key == secondKey)
                {
                    foundSecond = true;
                }
                ++foundCount;
            }

            Assert.IsTrue(foundCount == 2 && foundFirst && foundSecond);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void MoveNextWithVersion_FindAllEntries()
        {
            string firstKey = "firstKey";
            string secondKey = "secondKey";
            string myValue = "myValue";

            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);

            dictionary.AddUnchecked(firstKey, myValue);
            dictionary.AddUnchecked(secondKey, myValue);

            int foundCount = 0;
            bool foundFirst = false;
            bool foundSecond = false;
            int currentIndex = 0;
            int version = 0;
            int dictionaryVersion = 0;

            while (dictionary.MoveNext(ref currentIndex, version, in dictionaryVersion, out StringKeyEntry<string> currentEntry) == IteratorState.FoundEntry)
            {
                if (currentEntry.key == firstKey)
                {
                    foundFirst = true;
                }
                else if (currentEntry.key == secondKey)
                {
                    foundSecond = true;
                }
                ++foundCount;
            }

            Assert.IsTrue(foundCount == 2 && foundFirst && foundSecond);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void MoveNextWithVersion_CheckEarlyOutFromInvalidation()
        {
            string firstKey = "firstKey";
            string secondKey = "secondKey";
            string myValue = "myValue";

            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);

            dictionary.AddUnchecked(firstKey, myValue);
            dictionary.AddUnchecked(secondKey, myValue);

            int foundCount = 0;
            bool foundFirst = false;
            bool foundSecond = false;
            int currentIndex = 0;
            int version = 0;
            int dictionaryVersion = 0;

            IteratorState iteratorState = dictionary.MoveNext(ref currentIndex, version, in dictionaryVersion, out StringKeyEntry<string> currentEntry);

            while (iteratorState == IteratorState.FoundEntry)
            {
                if (currentEntry.key == firstKey)
                {
                    foundFirst = true;
                }
                else if (currentEntry.key == secondKey)
                {
                    foundSecond = true;
                }
                ++foundCount;

                dictionary.AddSafe("addingInvalidatesIterators", myValue);
                version++;

                iteratorState = dictionary.MoveNext(ref currentIndex, version, in dictionaryVersion, out currentEntry);
            }

            Assert.IsTrue(foundCount == 1 && foundFirst != foundSecond && iteratorState == IteratorState.InvalidVersion);
        }
    }
}