// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
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
        [Category("GDX.Tests")]
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
        [Category("GDX.Tests")]
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
        [Category("GDX.Tests")]
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
        [Category("GDX.Tests")]
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
        [Category("GDX.Tests")]
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
        [Category("GDX.Tests")]
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
        [Category("GDX.Tests")]
        public void TryRemoveTwice_CheckCorrectReturnvalues()
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
        [Category("GDX.Tests")]
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
        [Category("GDX.Tests")]
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
        [Category("GDX.Tests")]
        public void IndexOf_CheckEntryDoesNotExist()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            string myKey = "myKey";
            int allegedIndex = dictionary.IndexOf(myKey);

            Assert.IsTrue(allegedIndex == -1);
        }

        public void AddTwoCollidingEntries_CheckAccess()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            string collidingKey0 = "942";
            string collidingKey1 = "9331582";
            string myValue0 = "value0";
            string myValue1 = "value1";
            dictionary.AddUnchecked(collidingKey0, myValue0);
            dictionary.AddUnchecked(collidingKey1, myValue1);
            int allegedIndex0 = dictionary.IndexOf(myValue0);
            int allegedIndex1 = dictionary.IndexOf(myValue1);
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
    }
}