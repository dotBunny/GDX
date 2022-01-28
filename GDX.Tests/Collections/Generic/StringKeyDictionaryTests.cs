// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using GDX.Collections.Generic;
using NUnit.Framework;

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
        public void AddSafe_TryRemove_CheckExists()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            string myKey = "myKey";
            string myValue = "myValue";
            dictionary.AddSafe(myKey, myValue);

            bool removedFirstTime = dictionary.TryRemove(myKey);
            bool removedSecondTime = dictionary.TryRemove(myKey);
            Assert.IsTrue(removedFirstTime && !removedSecondTime);
        }

        [Test]
        [Category("GDX.Tests")]
        public void AddWithUniqueCheckTwice_CheckAddedTwice()
        {
            StringKeyDictionary<string> dictionary = new StringKeyDictionary<string>(16);
            string myKey = "myKey";
            string myValue = "myValue";
            dictionary.AddWithUniqueCheck(myKey, myValue);
            bool caughtDuplicate = dictionary.AddWithUniqueCheck(myKey, myValue);

            bool removedFirst = dictionary.TryRemove(myKey);
            bool removedSecond = dictionary.TryRemove(myKey);

            Assert.IsTrue(removedFirst && !removedSecond);
        }
    }
}