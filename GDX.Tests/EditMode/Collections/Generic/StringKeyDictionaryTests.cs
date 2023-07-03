// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;

namespace GDX.Collections.Generic
{
    public class StringKeyDictionaryTests
    {
        const string k_CollidingKey0 = "942";
        const string k_CollidingKey1 = "9331582";

        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_RequestZeroMinimum_PrimeCapacity()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(0);

            Assert.IsTrue(dictionary.Count == 0 && dictionary.Entries.Length == DictionaryPrimes.GetPrimeAtIndex(0));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_RequestPrimeMinimum_PrimeCapacity()
        {
            int prime = DictionaryPrimes.GetPrimeAtIndex(2);
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(prime);
            Assert.IsTrue(prime == dictionary.Entries.Length);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_RequestBelowPrimeMinimum_PrimeCapacity()
        {
            int prime = DictionaryPrimes.GetPrimeAtIndex(5);
            int minCapacity = prime - 1;
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(minCapacity);

            Assert.IsTrue(prime == dictionary.Entries.Length);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddSafe_CheckExists()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            dictionary.AddSafe(TestLiterals.Foo, TestLiterals.Bar);

            string storedValue = dictionary[TestLiterals.Foo];

            Assert.IsTrue(storedValue == TestLiterals.Bar);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IndexerAdd_CheckExists()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16)
            {
                [TestLiterals.Foo] = TestLiterals.Bar
            };

            string storedValue = dictionary[TestLiterals.Foo];

            Assert.IsTrue(storedValue == TestLiterals.Bar);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheck_CheckExists()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            dictionary.AddWithExpandCheck(TestLiterals.Foo, TestLiterals.Bar);

            string storedValue = dictionary[TestLiterals.Foo];

            Assert.IsTrue(storedValue == TestLiterals.Bar);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheck_CheckExpanded()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);

            char val = 'A';
            string addedKey = TestLiterals.Foo;
            int initialCapacity = dictionary.Entries.Length;
            for (int i = 0; i < initialCapacity; i++)
            {
                dictionary.AddWithExpandCheck(addedKey, TestLiterals.Bar);
                addedKey += val;
                val++;
            }

            dictionary.AddWithExpandCheck(TestLiterals.Bar, TestLiterals.Bar);

            int expandedCapacity = dictionary.Entries.Length;

            Assert.IsTrue(expandedCapacity > initialCapacity);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheck_CheckEntriesMaintainConnection()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            char val = 'A';

            string addedKey = TestLiterals.Foo;
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
                dictionary.AddWithExpandCheck(keys[i], TestLiterals.Bar);
            }

            dictionary.AddWithExpandCheck(TestLiterals.Bar, TestLiterals.Bar);

            bool allEntriesAreThere = true;
            for (int i = 0; i < initialCapacity; i++)
            {
                if (dictionary.IndexOf(keys[i]) == -1)
                {
                    allEntriesAreThere = false;
                    break;
                }
            }

            bool finalEntryIsThere = dictionary.IndexOf(TestLiterals.Bar) != -1;

            Assert.IsTrue(allEntriesAreThere && finalEntryIsThere);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddSafe_CheckExpanded()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);

            char val = 'A';

            string addedKey = TestLiterals.Foo;
            int initialCapacity = dictionary.Entries.Length;
            for (int i = 0; i < initialCapacity; i++)
            {
                dictionary.AddSafe(addedKey, TestLiterals.Bar);
                addedKey += val;
                val++;
            }

            dictionary.AddSafe(TestLiterals.Bar, TestLiterals.Bar);

            int expandedCapacity = dictionary.Entries.Length;

            Assert.IsTrue(expandedCapacity > initialCapacity);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddSafe_CheckEntriesMaintainConnection()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);

            char val = 'A';

            string addedKey = TestLiterals.Foo;
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
                dictionary.AddSafe(keys[i], TestLiterals.Bar);
            }

            dictionary.AddSafe(TestLiterals.Bar, TestLiterals.Bar);

            bool allEntriesAreThere = true;
            for (int i = 0; i < initialCapacity; i++)
            {
                if (dictionary.IndexOf(keys[i]) == -1)
                {
                    allEntriesAreThere = false;
                    break;
                }
            }

            bool finalEntryIsThere = dictionary.IndexOf(TestLiterals.Bar) != -1;

            Assert.IsTrue(allEntriesAreThere && finalEntryIsThere);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithIndexer_CheckExpanded()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            char val = 'A';

            string addedKey = TestLiterals.Foo;
            int initialCapacity = dictionary.Entries.Length;
            for (int i = 0; i < initialCapacity; i++)
            {
                dictionary[addedKey] = TestLiterals.Bar;
                addedKey += val;
                val++;
            }

            dictionary[TestLiterals.Bar] = TestLiterals.Bar;

            int expandedCapacity = dictionary.Entries.Length;

            Assert.IsTrue(expandedCapacity > initialCapacity);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithIndexer_CheckEntriesMaintainConnection()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            char val = 'A';

            string addedKey = TestLiterals.Foo;
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
                dictionary[keys[i]] = TestLiterals.Bar;
            }

            dictionary[TestLiterals.Bar] = TestLiterals.Bar;

            bool allEntriesAreThere = true;
            for (int i = 0; i < initialCapacity; i++)
            {
                if (dictionary.IndexOf(keys[i]) == -1)
                {
                    allEntriesAreThere = false;
                    break;
                }
            }

            bool finalEntryIsThere = dictionary.IndexOf(TestLiterals.Bar) != -1;

            Assert.IsTrue(allEntriesAreThere && finalEntryIsThere);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithUniqueCheck_CheckExists()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            dictionary.AddWithUniqueCheck(TestLiterals.Foo, TestLiterals.Bar);

            string storedValue = dictionary[TestLiterals.Foo];

            Assert.IsTrue(storedValue == TestLiterals.Bar);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_CheckExists()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            dictionary.AddUnchecked(TestLiterals.Foo, TestLiterals.Bar);

            string storedValue = dictionary[TestLiterals.Foo];

            Assert.IsTrue(storedValue == TestLiterals.Bar);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryRemove_CheckActuallyRemoved()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            dictionary.AddSafe(TestLiterals.Foo, TestLiterals.Bar);

            bool removedFirstTime = dictionary.TryRemove(TestLiterals.Foo);

            StringKeyEntry<string>[] entries = dictionary.Entries;
            int arrayLength = entries.Length;

            int indexOfKey = -1;
            for (int i = 0; i < arrayLength; i++)
            {
                StringKeyEntry<string> entry = entries[i];
                if (entry.Key == TestLiterals.Foo)
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
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);

            dictionary.AddSafe(TestLiterals.Foo, TestLiterals.Bar);

            bool removedFirstTime = dictionary.TryRemove(TestLiterals.Foo);
            bool removedSecondTime = dictionary.TryRemove(TestLiterals.Foo);

            Assert.IsTrue(removedFirstTime && !removedSecondTime);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithUniqueCheckTwice_CheckAddedOnlyOnce()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);

            dictionary.AddWithUniqueCheck(TestLiterals.Foo, TestLiterals.Bar);
            bool failedToCatchDuplicate = dictionary.AddWithUniqueCheck(TestLiterals.Foo, TestLiterals.Bar);

            StringKeyEntry<string>[] entries = dictionary.Entries;
            int arrayLength = entries.Length;

            int indexOfKey = -1;
            for (int i = 0; i < arrayLength; i++)
            {
                StringKeyEntry<string> entry = entries[i];
                if (entry.Key == TestLiterals.Foo)
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
                    if (entry.Key == TestLiterals.Foo)
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
            dictionary.AddUnchecked(TestLiterals.Foo, TestLiterals.Bar);
            int allegedIndex = dictionary.IndexOf(TestLiterals.Foo);

            StringKeyEntry<string>[] entries = dictionary.Entries;
            int arrayLength = entries.Length;

            int realIndex = -1;
            for (int i = 0; i < arrayLength; i++)
            {
                StringKeyEntry<string> entry = entries[i];
                if (entry.Key == TestLiterals.Foo)
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
            int allegedIndex = dictionary.IndexOf(TestLiterals.Foo);

            Assert.IsTrue(allegedIndex == -1);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddTwoCollidingEntries_CheckAccess()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            dictionary.AddUnchecked(k_CollidingKey0, TestLiterals.Foo);
            dictionary.AddUnchecked(k_CollidingKey1, TestLiterals.Bar);
            int allegedIndex0 = dictionary.IndexOf(k_CollidingKey0);
            int allegedIndex1 = dictionary.IndexOf(k_CollidingKey1);
            StringKeyEntry<string>[] entries = dictionary.Entries;
            int arrayLength = entries.Length;

            int realIndex0 = -1;

            for (int i = 0; i < arrayLength; i++)
            {
                StringKeyEntry<string> entry = entries[i];
                if (entry.Key == k_CollidingKey0 && entry.Value == TestLiterals.Foo)
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
                if (entry.Key == k_CollidingKey1 && entry.Value == TestLiterals.Bar)
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
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            dictionary.AddUnchecked(k_CollidingKey0, TestLiterals.Foo);
            dictionary.AddUnchecked(k_CollidingKey1, TestLiterals.Bar);

            int allegedIndex1 = dictionary.IndexOf(k_CollidingKey1);

            dictionary.TryRemove(k_CollidingKey0);

            int allegedIndex1After = dictionary.IndexOf(k_CollidingKey1);

            int stableHashCode0 = k_CollidingKey0.GetStableHashCode();
            int stableHashCode1 = k_CollidingKey1.GetStableHashCode();

            Assert.IsTrue(allegedIndex1 == allegedIndex1After && allegedIndex1After != -1 && stableHashCode0 == stableHashCode1);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddTwoCollidingEntries_RemoveSecond_CheckFirstExists()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            dictionary.AddUnchecked(k_CollidingKey0, TestLiterals.Foo);
            dictionary.AddUnchecked(k_CollidingKey1, TestLiterals.Bar);

            int allegedIndex0 = dictionary.IndexOf(k_CollidingKey0);

            dictionary.TryRemove(k_CollidingKey1);

            int allegedIndex0After = dictionary.IndexOf(k_CollidingKey0);

            int stableHashCode0 = k_CollidingKey0.GetStableHashCode();
            int stableHashCode1 = k_CollidingKey1.GetStableHashCode();

            Assert.IsTrue(allegedIndex0 == allegedIndex0After && allegedIndex0After != -1 && stableHashCode0 == stableHashCode1);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void MoveNext_FindAllEntries()
        {

            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);

            dictionary.AddUnchecked(TestLiterals.Foo, TestLiterals.Bar);
            dictionary.AddUnchecked(TestLiterals.Bar, TestLiterals.Bar);

            int foundCount = 0;
            bool foundFirst = false;
            bool foundSecond = false;
            int currentIndex = 0;

            while (dictionary.MoveNext(ref currentIndex, out StringKeyEntry<string> currentEntry))
            {
                if (currentEntry.Key == TestLiterals.Foo)
                {
                    foundFirst = true;
                }
                else if (currentEntry.Key == TestLiterals.Bar)
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
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);

            dictionary.AddUnchecked(TestLiterals.Foo, TestLiterals.Bar);
            dictionary.AddUnchecked(TestLiterals.Bar, TestLiterals.Bar);

            int foundCount = 0;
            bool foundFirst = false;
            bool foundSecond = false;
            int currentIndex = 0;
            int version = 0;
            int dictionaryVersion = 0;

            while (dictionary.MoveNext(ref currentIndex, version, in dictionaryVersion, out StringKeyEntry<string> currentEntry) == IteratorState.FoundEntry)
            {
                if (currentEntry.Key == TestLiterals.Foo)
                {
                    foundFirst = true;
                }
                else if (currentEntry.Key == TestLiterals.Bar)
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
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);

            dictionary.AddUnchecked(TestLiterals.Foo, TestLiterals.Bar);
            dictionary.AddUnchecked(TestLiterals.Bar, TestLiterals.Bar);

            int foundCount = 0;
            bool foundFirst = false;
            bool foundSecond = false;
            int currentIndex = 0;
            int version = 0;
            int dictionaryVersion = 0;

            IteratorState iteratorState = dictionary.MoveNext(ref currentIndex, version, in dictionaryVersion, out StringKeyEntry<string> currentEntry);

            while (iteratorState == IteratorState.FoundEntry)
            {
                if (currentEntry.Key == TestLiterals.Foo)
                {
                    foundFirst = true;
                }
                else if (currentEntry.Key == TestLiterals.Bar)
                {
                    foundSecond = true;
                }
                ++foundCount;

                dictionary.AddSafe(TestLiterals.Seed, TestLiterals.Bar);
                version++;

                iteratorState = dictionary.MoveNext(ref currentIndex, version, in dictionaryVersion, out currentEntry);
            }

            Assert.IsTrue(foundCount == 1 && foundFirst != foundSecond && iteratorState == IteratorState.InvalidVersion);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryGetValue()
        {
            StringKeyDictionary<int> dictionary = new StringKeyDictionary<int>(16) { [TestLiterals.Foo] = 42 };

            Assert.IsTrue(dictionary.TryGetValue(TestLiterals.Foo, out int fooValue));
            Assert.IsTrue(!dictionary.TryGetValue(TestLiterals.Bar, out int barValue));

            Assert.IsTrue(fooValue == 42);
            Assert.IsTrue(barValue == 0);
        }
    }
}