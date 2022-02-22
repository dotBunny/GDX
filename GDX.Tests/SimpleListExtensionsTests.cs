// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using GDX.Collections.Generic;
using NUnit.Framework;

// ReSharper disable HeapView.ObjectAllocation.Evident

namespace GDX
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="SimpleListExtensions" />
    ///     class.
    /// </summary>
    public class SimpleListExtensionsTests
    {
        SimpleList<int> m_MockData;

        [SetUp]
        public void Setup()
        {
            m_MockData = new SimpleList<int>(5);
            m_MockData.AddUnchecked(0);
            m_MockData.AddUnchecked(1);
            m_MockData.AddUnchecked(2);
            m_MockData.AddUnchecked(3);
            m_MockData.AddUnchecked(4);
        }

        [TearDown]
        public void TearDown()
        {
            m_MockData.Clear();
            m_MockData = default;
        }


        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheckUniqueItem_NonUniqueString_ReturnsFalse()
        {
            SimpleList<string> mockData = new SimpleList<string>(3);

            mockData.AddWithExpandCheckUniqueItem("test1");
            mockData.AddWithExpandCheckUniqueItem("test2");
            mockData.AddWithExpandCheckUniqueItem("test3");

            bool evaluate = mockData.AddWithExpandCheckUniqueItem("test1");

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheckUniqueItem_UniqueString_ReturnsTrue()
        {
            SimpleList<string> mockData = new SimpleList<string>(3);

            mockData.AddWithExpandCheckUniqueItem("test1");
            mockData.AddWithExpandCheckUniqueItem("test2");
            mockData.AddWithExpandCheckUniqueItem("test3");

            bool evaluate = mockData.AddWithExpandCheckUniqueItem("test4");

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUncheckedUniqueItem_NonUniqueString_ReturnsFalse()
        {
            SimpleList<string> listOfStrings = new SimpleList<string>(3);
            listOfStrings.AddUncheckedUniqueItem("test1");
            listOfStrings.AddUncheckedUniqueItem("test2");

            bool evaluate = listOfStrings.AddUncheckedUniqueItem("test2");

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUncheckedUniqueItem_UniqueStringWithRoom_NoException()
        {
            SimpleList<string> listOfStrings = new SimpleList<string>(3);
            listOfStrings.AddUncheckedUniqueItem("test1");
            listOfStrings.AddUncheckedUniqueItem("test2");

            Assert.DoesNotThrow(() => { listOfStrings.AddUncheckedUniqueItem("test3"); });
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUncheckedUniqueItem_UniqueStringWithNoRoom_ThrowsException()
        {
            SimpleList<string> listOfStrings = new SimpleList<string>(2);
            listOfStrings.AddUncheckedUniqueItem("test1");
            listOfStrings.AddUncheckedUniqueItem("test2");

            Assert.Throws<IndexOutOfRangeException>(() => { listOfStrings.AddUncheckedUniqueItem("test3"); });
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ContainsItem_String_ReturnsTrue()
        {
            const string k_SearchItem = "Hello";
            SimpleList<string> listOfStrings = new SimpleList<string>(3);
            listOfStrings.AddUnchecked(k_SearchItem);
            listOfStrings.AddUnchecked("World");
            listOfStrings.AddUnchecked("!");

            bool evaluate = listOfStrings.ContainsItem(k_SearchItem);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ContainsItem_CircularBuffer_ReturnsTrue()
        {
            object searchItem = new object();
            SimpleList<object> listItems = new SimpleList<object>(5);

            // Build test rig
            listItems.AddUnchecked(new object());
            listItems.AddUnchecked(searchItem);
            listItems.AddUnchecked(new object());
            listItems.AddUnchecked(new object());
            listItems.AddUnchecked(new object());

            bool evaluate = listItems.ContainsItem(searchItem);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveFirstItem_MockData_RemovedItem()
        {
            object searchItem = new object();
            SimpleList<object> listItems = new SimpleList<object>(6);

            // Build test rig
            listItems.AddUnchecked(new object());
            listItems.AddUnchecked(searchItem);
            listItems.AddUnchecked(new object());
            listItems.AddUnchecked(new object());
            listItems.AddUnchecked(searchItem);
            listItems.AddUnchecked(new object());

            listItems.RemoveFirstItem(searchItem);

            bool evaluate = listItems.Array[1] != searchItem &&
                            listItems.ContainsItem(searchItem);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveFirstItem_MockData_NoItemReturnsFalse()
        {
            object searchItem = new object();
            SimpleList<object> listItems = new SimpleList<object>(6);

            // Build test rig
            listItems.AddUnchecked(new object());
            listItems.AddUnchecked(searchItem);
            listItems.AddUnchecked(new object());
            listItems.AddUnchecked(new object());
            listItems.AddUnchecked(searchItem);
            listItems.AddUnchecked(new object());

            bool evaluate = listItems.RemoveFirstItem(new object());

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveItems_MockData_RemovedItems()
        {
            object searchItem = new object();
            SimpleList<object> listItems = new SimpleList<object>(6);

            // Build test rig
            listItems.AddUnchecked(new object());
            listItems.AddUnchecked(searchItem);
            listItems.AddUnchecked(new object());
            listItems.AddUnchecked(new object());
            listItems.AddUnchecked(searchItem);
            listItems.AddUnchecked(new object());

            listItems.RemoveItems(searchItem);

            bool evaluate = listItems.ContainsItem(searchItem);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveLastItem_MockData_RemovedItem()
        {
            object searchItem = new object();
            SimpleList<object> listItems = new SimpleList<object>(6);

            // Build test rig
            listItems.AddUnchecked(new object());
            listItems.AddUnchecked(searchItem);
            listItems.AddUnchecked(new object());
            listItems.AddUnchecked(new object());
            listItems.AddUnchecked(searchItem);
            listItems.AddUnchecked(new object());

            listItems.RemoveLastItem(searchItem);

            bool evaluate = listItems.Array[4] != searchItem &&
                            listItems.ContainsItem(searchItem);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveLastItem_MockData_NoItemReturnsFalse()
        {
            object searchItem = new object();
            SimpleList<object> listItems = new SimpleList<object>(6);

            // Build test rig
            listItems.AddUnchecked(new object());
            listItems.AddUnchecked(searchItem);
            listItems.AddUnchecked(new object());
            listItems.AddUnchecked(new object());
            listItems.AddUnchecked(searchItem);
            listItems.AddUnchecked(new object());

            bool evaluate = listItems.RemoveLastItem(new object());

            Assert.IsFalse(evaluate);
        }
    }
}