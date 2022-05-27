// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;
using UnityEngine;

namespace GDX.Collections.Generic
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="SerializableDictionary{TKey,TValue}" />.
    /// </summary>
    public class SerializableDictionaryTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void IsSerializableType_SystemObject_ReturnsFalse()
        {
            bool evaluate = SerializableDictionary<object, string>.IsSerializableType(typeof(object));

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SaveSerializedData_MockData_ReturnsSerializedDataLength()
        {
            SerializableDictionary<int, string> mockDictionary =
                new SerializableDictionary<int, string> {{1, TestLiterals.Foo}, {2, TestLiterals.Bar}};

            mockDictionary.SaveSerializedData();

            bool evaluate = mockDictionary.GetSerializedDataLength() == 2;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IsSerializableType_UnityObject_ReturnsTrue()
        {
            bool evaluate = SerializableDictionary<Object, string>.IsSerializableType(typeof(Object));

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void OverwriteSerializedData_MockData_PopulatesDictionary()
        {
            SerializableDictionary<int, string> mockDictionary = new SerializableDictionary<int, string>();
            mockDictionary.OverwriteSerializedData(new[] {1, 2}, new [] { TestLiterals.Foo, TestLiterals.Bar});
            mockDictionary.LoadSerializedData();

            bool evaluate = mockDictionary.Count == 2 && mockDictionary[2] == TestLiterals.Bar;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SaveSerializedData_NoData_ReturnsZeroSerializedDataLength()
        {
            SerializableDictionary<int, string> mockDictionary = new SerializableDictionary<int, string>();
            mockDictionary.SaveSerializedData();

            bool evaluate = mockDictionary.GetSerializedDataLength() == 0;

            Assert.IsTrue(evaluate);
        }
    }
}