// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;

namespace GDX.Collections.Generic
{
    public class IntKeyDictionaryTests
    {
        const string k_CollidingKey0 = "942";
        const string k_CollidingKey1 = "9331582";

        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_RequestZeroMinimum_PrimeCapacity()
        {
            IntKeyDictionary<string> dictionary = new IntKeyDictionary<string>(0);

            Assert.IsTrue(dictionary.Count == 0 && dictionary.Entries.Length == DictionaryPrimes.GetPrimeAtIndex(0));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_RequestPrimeMinimum_PrimeCapacity()
        {
            int prime = DictionaryPrimes.GetPrimeAtIndex(2);
            IntKeyDictionary<string> dictionary = new IntKeyDictionary<string>(prime);
            Assert.IsTrue(prime == dictionary.Entries.Length);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_RequestBelowPrimeMinimum_PrimeCapacity()
        {
            int prime = DictionaryPrimes.GetPrimeAtIndex(5);
            int minCapacity = prime - 1;
            IntKeyDictionary<string> dictionary = new IntKeyDictionary<string>(minCapacity);

            Assert.IsTrue(prime == dictionary.Entries.Length);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddSafe_CheckExists()
        {
            IntKeyDictionary<string> dictionary = new IntKeyDictionary<string>(16);
            dictionary.AddSafe(34, TestLiterals.Bar);

            string storedValue = dictionary[34];

            Assert.IsTrue(storedValue == TestLiterals.Bar);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IndexerAdd_CheckExists()
        {
            IntKeyDictionary<string> dictionary = new IntKeyDictionary<string>(16);
            dictionary[0] = TestLiterals.Bar;

            string storedValue = dictionary[0];

            Assert.IsTrue(storedValue == TestLiterals.Bar);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheck_CheckExists()
        {
            IntKeyDictionary<string> dictionary = new IntKeyDictionary<string>(16);
            dictionary.AddWithExpandCheck(0, TestLiterals.Bar);

            string storedValue = dictionary[0];

            Assert.IsTrue(storedValue == TestLiterals.Bar);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheck_CheckExpanded()
        {
            IntKeyDictionary<string> dictionary = new IntKeyDictionary<string>(16);

            int addedKey = 0;
            int initialCapacity = dictionary.Entries.Length;
            for (int i = 0; i < initialCapacity; i++)
            {
                dictionary.AddWithExpandCheck(addedKey, TestLiterals.Bar);
                addedKey++;
            }

            dictionary.AddWithExpandCheck(addedKey + 1, TestLiterals.Bar);

            int expandedCapacity = dictionary.Entries.Length;

            Assert.IsTrue(expandedCapacity > initialCapacity);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheck_CheckEntriesMaintainConnection()
        {
            IntKeyDictionary<string> dictionary = new IntKeyDictionary<string>(16);

            int addedKey = 0;
            int initialCapacity = dictionary.Entries.Length;

            int[] keys = new int[initialCapacity];

            for (int i = 0; i < initialCapacity; i++)
            {
                keys[i] = addedKey;
                addedKey++;
            }

            for (int i = 0; i < initialCapacity; i++)
            {
                dictionary.AddWithExpandCheck(keys[i], TestLiterals.Bar);
            }

            dictionary.AddWithExpandCheck(addedKey + 1, TestLiterals.Bar);

            bool allEntriesAreThere = true;
            for (int i = 0; i < initialCapacity; i++)
            {
                if (dictionary.IndexOf(keys[i]) == -1)
                {
                    allEntriesAreThere = false;
                    break;
                }
            }

            bool finalEntryIsThere = dictionary.IndexOf(addedKey + 1) != -1;

            Assert.IsTrue(allEntriesAreThere && finalEntryIsThere);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddSafe_CheckExpanded()
        {
            IntKeyDictionary<string> dictionary = new IntKeyDictionary<string>(16);

            int addedKey = 0;
            int initialCapacity = dictionary.Entries.Length;
            for (int i = 0; i < initialCapacity; i++)
            {
                dictionary.AddSafe(addedKey, TestLiterals.Bar);
                addedKey ++;
            }

            dictionary.AddSafe(addedKey + 1, TestLiterals.Bar);

            int expandedCapacity = dictionary.Entries.Length;

            Assert.IsTrue(expandedCapacity > initialCapacity);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddSafe_CheckEntriesMaintainConnection()
        {
            IntKeyDictionary<string> dictionary = new IntKeyDictionary<string>(16);

            int addedKey = 0;
            int initialCapacity = dictionary.Entries.Length;

            int[] keys = new int[initialCapacity];

            for (int i = 0; i < initialCapacity; i++)
            {
                keys[i] = addedKey;
                addedKey++;
            }

            for (int i = 0; i < initialCapacity; i++)
            {
                dictionary.AddSafe(keys[i], TestLiterals.Bar);
            }

            dictionary.AddSafe(addedKey + 1, TestLiterals.Bar);

            bool allEntriesAreThere = true;
            for (int i = 0; i < initialCapacity; i++)
            {
                if (dictionary.IndexOf(keys[i]) == -1)
                {
                    allEntriesAreThere = false;
                    break;
                }
            }

            bool finalEntryIsThere = dictionary.IndexOf(addedKey + 1) != -1;

            Assert.IsTrue(allEntriesAreThere && finalEntryIsThere);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithIndexer_CheckExpanded()
        {
            IntKeyDictionary<string> dictionary = new IntKeyDictionary<string>(16);

            int addedKey = 0;
            int initialCapacity = dictionary.Entries.Length;
            for (int i = 0; i < initialCapacity; i++)
            {
                dictionary[addedKey] = TestLiterals.Bar;
                addedKey++;
            }

            dictionary[0] = TestLiterals.Bar;

            int expandedCapacity = dictionary.Entries.Length;

            Assert.IsTrue(expandedCapacity > initialCapacity);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithIndexer_CheckEntriesMaintainConnection()
        {
            IntKeyDictionary<string> dictionary = new IntKeyDictionary<string>(16);

            int addedKey = 0;
            int initialCapacity = dictionary.Entries.Length;

            int[] keys = new int[initialCapacity];

            for (int i = 0; i < initialCapacity; i++)
            {
                keys[i] = addedKey;
                addedKey++;
            }

            for (int i = 0; i < initialCapacity; i++)
            {
                dictionary[keys[i]] = TestLiterals.Bar;
            }

            dictionary[addedKey + 1] = TestLiterals.Bar;

            bool allEntriesAreThere = true;
            for (int i = 0; i < initialCapacity; i++)
            {
                if (dictionary.IndexOf(keys[i]) == -1)
                {
                    allEntriesAreThere = false;
                    break;
                }
            }

            bool finalEntryIsThere = dictionary.IndexOf(addedKey + 1) != -1;

            Assert.IsTrue(allEntriesAreThere && finalEntryIsThere);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithUniqueCheck_CheckExists()
        {
            IntKeyDictionary<string> dictionary = new IntKeyDictionary<string>(16);
            dictionary.AddWithUniqueCheck(0, TestLiterals.Bar);

            string storedValue = dictionary[0];

            Assert.IsTrue(storedValue == TestLiterals.Bar);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_CheckExists()
        {
            IntKeyDictionary<string> dictionary = new IntKeyDictionary<string>(16);
            dictionary.AddUnchecked(0, TestLiterals.Bar);

            string storedValue = dictionary[0];

            Assert.IsTrue(storedValue == TestLiterals.Bar);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryRemove_CheckActuallyRemoved()
        {
            IntKeyDictionary<string> dictionary = new IntKeyDictionary<string>(16);
            dictionary.AddSafe(0, TestLiterals.Bar);

            bool removedFirstTime = dictionary.TryRemove(0);

            IntKeyEntry<string>[] entries = dictionary.Entries;
            int arrayLength = entries.Length;

            int indexOfKey = -1;
            for (int i = 0; i < arrayLength; i++)
            {
                IntKeyEntry<string> entry = entries[i];
                if (entry.Key == 0 && entry.Next >= 0)
                {
                    indexOfKey = i;
                    break;
                }
            }

            Assert.IsTrue(removedFirstTime && indexOfKey == -1);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryRemoveTwice_CheckCorrectReturnValues()
        {
            IntKeyDictionary<string> dictionary = new IntKeyDictionary<string>(16);

            dictionary.AddSafe(0, TestLiterals.Bar);

            bool removedFirstTime = dictionary.TryRemove(0);
            bool removedSecondTime = dictionary.TryRemove(0);

            Assert.IsTrue(removedFirstTime && !removedSecondTime);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithUniqueCheckTwice_CheckAddedOnlyOnce()
        {
            IntKeyDictionary<string> dictionary = new IntKeyDictionary<string>(16);

            dictionary.AddWithUniqueCheck(0, TestLiterals.Bar);
            bool failedToCatchDuplicate = dictionary.AddWithUniqueCheck(0, TestLiterals.Bar);

            IntKeyEntry<string>[] entries = dictionary.Entries;
            int arrayLength = entries.Length;

            int indexOfKey = -1;
            for (int i = 0; i < arrayLength; i++)
            {
                IntKeyEntry<string> entry = entries[i];
                if (entry.Key == 0 && entry.Next >= 0)
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
                    IntKeyEntry<string> entry = entries[i];
                    if (entry.Key == 0 && entry.Next >= 0)
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
            IntKeyDictionary<string> dictionary = new IntKeyDictionary<string>(16);

            int initialCapacity = dictionary.Entries.Length;

            int reserveSize = initialCapacity + 50;

            dictionary.Reserve(reserveSize);

            Assert.IsTrue(dictionary.Entries.Length >= reserveSize);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IndexOf_CheckCorrectIndex()
        {
            IntKeyDictionary<string> dictionary = new IntKeyDictionary<string>(16);
            dictionary.AddUnchecked(0, TestLiterals.Bar);
            int allegedIndex = dictionary.IndexOf(0);

            IntKeyEntry<string>[] entries = dictionary.Entries;
            int arrayLength = entries.Length;

            int realIndex = -1;
            for (int i = 0; i < arrayLength; i++)
            {
                IntKeyEntry<string> entry = entries[i];
                if (entry.Key == 0 && entry.Next >= 0)
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
            IntKeyDictionary<string> dictionary = new IntKeyDictionary<string>(16);
            int allegedIndex = dictionary.IndexOf(0);

            Assert.IsTrue(allegedIndex == -1);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddTwoCollidingEntries_CheckAccess()
        {
            IntKeyDictionary<string> dictionary = new IntKeyDictionary<string>(16);
            dictionary.AddUnchecked(8, TestLiterals.Foo);
            dictionary.AddUnchecked(8 + dictionary.Entries.Length, TestLiterals.Bar);
            int allegedIndex0 = dictionary.IndexOf(8);
            int allegedIndex1 = dictionary.IndexOf(8 + dictionary.Entries.Length);
            IntKeyEntry<string>[] entries = dictionary.Entries;
            int arrayLength = entries.Length;

            int realIndex0 = -1;

            for (int i = 0; i < arrayLength; i++)
            {
                IntKeyEntry<string> entry = entries[i];
                if (entry.Key == 8 && entry.Value == TestLiterals.Foo)
                {
                    realIndex0 = i;
                    break;
                }
            }

            int realIndex1 = -1;
            int nextIndex = -1;
            for (int i = 0; i < arrayLength; i++)
            {
                IntKeyEntry<string> entry = entries[i];
                if (entry.Key == 8 + dictionary.Entries.Length && entry.Value == TestLiterals.Bar)
                {
                    realIndex1 = i;
                    nextIndex = entry.Next;
                    break;
                }
            }

            int stableHashCode0 = k_CollidingKey0.GetStableHashCode();
            int stableHashCode1 = k_CollidingKey1.GetStableHashCode();

            Assert.IsTrue(allegedIndex0 == realIndex0 && allegedIndex1 == realIndex1 && allegedIndex0 != allegedIndex1 && nextIndex == allegedIndex0 && stableHashCode0 == stableHashCode1);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddTwoCollidingEntries_RemoveFirst_CheckSecondExists()
        {
            IntKeyDictionary<string> dictionary = new IntKeyDictionary<string>(16);
            dictionary.AddUnchecked(8, TestLiterals.Foo);
            dictionary.AddUnchecked(8 + dictionary.Entries.Length, TestLiterals.Bar);

            int allegedIndex1 = dictionary.IndexOf(8 + dictionary.Entries.Length);

            dictionary.TryRemove(8);

            int allegedIndex1After = dictionary.IndexOf(8 + dictionary.Entries.Length);

            int stableHashCode0 = k_CollidingKey0.GetStableHashCode();
            int stableHashCode1 = k_CollidingKey1.GetStableHashCode();

            Assert.IsTrue(allegedIndex1 == allegedIndex1After && allegedIndex1After != -1 && stableHashCode0 == stableHashCode1);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddTwoCollidingEntries_RemoveSecond_CheckFirstExists()
        {
            IntKeyDictionary<string> dictionary = new IntKeyDictionary<string>(16);
            dictionary.AddUnchecked(8, TestLiterals.Foo);
            dictionary.AddUnchecked(8 + dictionary.Entries.Length, TestLiterals.Bar);

            int allegedIndex0 = dictionary.IndexOf(8);

            dictionary.TryRemove(8 + dictionary.Entries.Length);

            int allegedIndex0After = dictionary.IndexOf(8);

            Assert.IsTrue(allegedIndex0 == allegedIndex0After && allegedIndex0After != -1);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void MoveNext_FindAllEntries()
        {

            IntKeyDictionary<string> dictionary = new IntKeyDictionary<string>(16);

            dictionary.AddUnchecked(0, TestLiterals.Bar);
            dictionary.AddUnchecked(1, TestLiterals.Bar);

            int foundCount = 0;
            bool foundFirst = false;
            bool foundSecond = false;
            int currentIndex = 0;

            while (dictionary.MoveNext(ref currentIndex, out IntKeyEntry<string> currentEntry))
            {
                if (currentEntry.Key == 0)
                {
                    foundFirst = true;
                }
                else if (currentEntry.Key == 1)
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
            IntKeyDictionary<string> dictionary = new IntKeyDictionary<string>(16);

            dictionary.AddUnchecked(0, TestLiterals.Bar);
            dictionary.AddUnchecked(1, TestLiterals.Bar);

            int foundCount = 0;
            bool foundFirst = false;
            bool foundSecond = false;
            int currentIndex = 0;
            int version = 0;
            int dictionaryVersion = 0;

            while (dictionary.MoveNext(ref currentIndex, version, in dictionaryVersion, out IntKeyEntry<string> currentEntry) == IteratorState.FoundEntry)
            {
                if (currentEntry.Key == 0)
                {
                    foundFirst = true;
                }
                else if (currentEntry.Key == 1)
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
            IntKeyDictionary<string> dictionary = new IntKeyDictionary<string>(16);

            dictionary.AddUnchecked(0, TestLiterals.Bar);
            dictionary.AddUnchecked(1, TestLiterals.Bar);

            int foundCount = 0;
            bool foundFirst = false;
            bool foundSecond = false;
            int currentIndex = 0;
            int version = 0;
            int dictionaryVersion = 0;

            IteratorState iteratorState = dictionary.MoveNext(ref currentIndex, version, in dictionaryVersion, out IntKeyEntry<string> currentEntry);

            while (iteratorState == IteratorState.FoundEntry)
            {
                if (currentEntry.Key == 0)
                {
                    foundFirst = true;
                }
                else if (currentEntry.Key == 1)
                {
                    foundSecond = true;
                }
                ++foundCount;

                dictionary.AddSafe(2, TestLiterals.Bar);
                version++;

                iteratorState = dictionary.MoveNext(ref currentIndex, version, in dictionaryVersion, out currentEntry);
            }

            Assert.IsTrue(foundCount == 1 && foundFirst != foundSecond && iteratorState == IteratorState.InvalidVersion);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryGetValue()
        {
            IntKeyDictionary<int> dictionary = new IntKeyDictionary<int>(16) { [0] = 42 };

            Assert.IsTrue(dictionary.TryGetValue(0, out int fooValue));
            Assert.IsTrue(!dictionary.TryGetValue(1, out int barValue));

            Assert.IsTrue(fooValue == 42);
            Assert.IsTrue(barValue == 0);
        }
    }
}