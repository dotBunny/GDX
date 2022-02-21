// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

// ReSharper disable HeapView.ObjectAllocation
// ReSharper disable UnusedVariable

namespace Runtime.Collections.Generic
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="SerializableDictionary{TKey,TValue}" />.
    /// </summary>
    public class SerializableDictionaryTests
    {
        [Test]
        [Category(GDX.Core.TestCategory)]
        public void IsSerializableType_SystemObject_ReturnsFalse()
        {
            SerializableDictionary<object, string> mockDictionary = new SerializableDictionary<object, string>();

            bool evaluate = mockDictionary.IsSerializableType(typeof(object));

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void SaveSerializedData_MockData_ReturnsSerializedDataLength()
        {
            SerializableDictionary<int, string> mockDictionary =
                new SerializableDictionary<int, string> {{1, "one"}, {2, "two"}};

            mockDictionary.SaveSerializedData();

            bool evaluate = mockDictionary.GetSerializedDataLength() == 2;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void IsSerializableType_UnityObject_ReturnsTrue()
        {
            SerializableDictionary<Object, string> mockDictionary = new SerializableDictionary<Object, string>();

            bool evaluate = mockDictionary.IsSerializableType(typeof(Object));

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void OverwriteSerializedData_MockData_PopulatesDictionary()
        {
            SerializableDictionary<int, string> mockDictionary = new SerializableDictionary<int, string>();
            mockDictionary.OverwriteSerializedData(new[] {1, 2}, new [] {"one", "two"});
            mockDictionary.LoadSerializedData();

            bool evaluate = mockDictionary.Count == 2 && mockDictionary[2] == "two";

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void SaveSerializedData_NoData_ReturnsZeroSerializedDataLength()
        {
            SerializableDictionary<int, string> mockDictionary = new SerializableDictionary<int, string>();
            mockDictionary.SaveSerializedData();

            bool evaluate = mockDictionary.GetSerializedDataLength() == 0;

            Assert.IsTrue(evaluate);
        }
    }
}