// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using GDX.Collections.Generic;
using NUnit.Framework;

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
        [Category("GDX.Tests")]
        public void False_IsSerializableType_SystemObject()
        {
            SerializableDictionary<object, string> testDictionary = new SerializableDictionary<object, string>();
            Assert.IsFalse(testDictionary.IsSerializableType(typeof(object)), "Expected to flag type as invalid.");
        }

        [Test]
        [Category("GDX.Tests")]
        public void True_IsSerializableType_UnityObject()
        {
            SerializableDictionary<UnityEngine.Object, string> testDictionary = new SerializableDictionary<UnityEngine.Object, string>();
            Assert.IsTrue(testDictionary.IsSerializableType(typeof(UnityEngine.Object)), "Expected to flag type as valid.");
        }

        [Test]
        [Category("GDX.Tests")]
        public void True_SaveSerializedData_NoData()
        {
            SerializableDictionary<int, string> testDictionary = new SerializableDictionary<int, string>();
            testDictionary.SaveSerializedData();

            Assert.IsTrue(testDictionary.GetSerializedDataLength() == 0, "Expected no length");
        }

        [Test]
        [Category("GDX.Tests")]
        public void True_SaveSerializedData_Simple()
        {
            SerializableDictionary<int, string> testDictionary = new SerializableDictionary<int, string>();
            testDictionary.Add(1, "one");
            testDictionary.Add(2, "two");
            testDictionary.SaveSerializedData();
            Assert.IsTrue(testDictionary.GetSerializedDataLength() == 2, "Expected a length of 2.");
        }

        [Test]
        [Category("GDX.Tests")]
        public void True_OverwriteSerializedData_LoadSerializedData_Simple()
        {
            SerializableDictionary<int, string> testDictionary = new SerializableDictionary<int, string>();
            testDictionary.OverwriteSerializedData(new int[2] {1, 2}, new string[2] {"one", "two"});
            testDictionary.LoadSerializedData();
            Assert.IsTrue(testDictionary.Count == 2 && testDictionary[2] == "two",
                $"Expected a length of 2, and the elements to match (found {testDictionary[1]}).");
        }
    }
}