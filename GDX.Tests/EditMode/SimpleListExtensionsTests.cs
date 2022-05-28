// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using GDX.Collections.Generic;
using NUnit.Framework;

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
        [Category(Literals.TestCategory)]
        public void AddWithExpandCheckUniqueItem_NonUniqueString_ReturnsFalse()
        {
            SimpleList<string> mockData = new SimpleList<string>(3);

            mockData.AddWithExpandCheckUniqueItem(TestLiterals.Foo);
            mockData.AddWithExpandCheckUniqueItem(TestLiterals.Bar);
            mockData.AddWithExpandCheckUniqueItem(TestLiterals.TestSeed);

            Assert.IsFalse(mockData.AddWithExpandCheckUniqueItem(TestLiterals.Foo));
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void AddWithExpandCheckUniqueItem_UniqueString_ReturnsTrue()
        {
            SimpleList<string> mockData = new SimpleList<string>(3);

            mockData.AddWithExpandCheckUniqueItem(TestLiterals.Foo);
            mockData.AddWithExpandCheckUniqueItem(TestLiterals.Bar);
            mockData.AddWithExpandCheckUniqueItem(TestLiterals.TestSeed);

            Assert.IsTrue(mockData.AddWithExpandCheckUniqueItem(TestLiterals.HelloWorld));
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void AddUncheckedUniqueItem_NonUniqueString_ReturnsFalse()
        {
            SimpleList<string> listOfStrings = new SimpleList<string>(3);
            listOfStrings.AddUncheckedUniqueItem(TestLiterals.Foo);
            listOfStrings.AddUncheckedUniqueItem(TestLiterals.Bar);

            Assert.IsFalse(listOfStrings.AddUncheckedUniqueItem(TestLiterals.Bar));
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void AddUncheckedUniqueItem_UniqueStringWithRoom_NoException()
        {
            SimpleList<string> listOfStrings = new SimpleList<string>(3);
            listOfStrings.AddUncheckedUniqueItem(TestLiterals.Foo);
            listOfStrings.AddUncheckedUniqueItem(TestLiterals.Bar);

            Assert.DoesNotThrow(() => { listOfStrings.AddUncheckedUniqueItem(TestLiterals.HelloWorld); });
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void AddUncheckedUniqueItem_UniqueStringWithNoRoom_ThrowsException()
        {
            SimpleList<string> listOfStrings = new SimpleList<string>(2);
            listOfStrings.AddUncheckedUniqueItem(TestLiterals.Foo);
            listOfStrings.AddUncheckedUniqueItem(TestLiterals.Bar);

            Assert.Throws<IndexOutOfRangeException>(() => { listOfStrings.AddUncheckedUniqueItem(TestLiterals.HelloWorld); });
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void AddWithExpandCheckUniqueReference_NonUniqueString_ReturnsFalse()
        {
            SimpleList<string> mockData = new SimpleList<string>(3);

            mockData.AddWithExpandCheckUniqueReference(TestLiterals.Foo);
            mockData.AddWithExpandCheckUniqueReference(TestLiterals.Bar);
            mockData.AddWithExpandCheckUniqueReference(TestLiterals.HelloWorld);

            Assert.IsFalse(mockData.AddWithExpandCheckUniqueReference(TestLiterals.Foo));
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void AddWithExpandCheckUniqueReference_UniqueString_ReturnsTrue()
        {
            SimpleList<string> mockData = new SimpleList<string>(3);

            mockData.AddWithExpandCheckUniqueReference(TestLiterals.Foo);
            mockData.AddWithExpandCheckUniqueReference(TestLiterals.Bar);
            mockData.AddWithExpandCheckUniqueReference(TestLiterals.HelloWorld);

            Assert.IsTrue(mockData.AddWithExpandCheckUniqueReference(TestLiterals.TestSeed));
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void AddUncheckedUniqueReference_NonUniqueString_ReturnsFalse()
        {
            SimpleList<string> listOfStrings = new SimpleList<string>(3);

            listOfStrings.AddUncheckedUniqueReference(TestLiterals.Foo);
            listOfStrings.AddUncheckedUniqueReference(TestLiterals.Bar);

            Assert.IsFalse(listOfStrings.AddUncheckedUniqueReference(TestLiterals.Bar));
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void AddUncheckedUniqueReference_UniqueStringWithRoom_NoException()
        {
            SimpleList<string> listOfStrings = new SimpleList<string>(3);
            listOfStrings.AddUncheckedUniqueReference(TestLiterals.Foo);
            listOfStrings.AddUncheckedUniqueReference(TestLiterals.Bar);

            Assert.DoesNotThrow(() => { listOfStrings.AddUncheckedUniqueReference(TestLiterals.HelloWorld); });
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void AddUncheckedUniqueReference_UniqueStringWithNoRoom_ThrowsException()
        {
            SimpleList<string> listOfStrings = new SimpleList<string>(2);
            listOfStrings.AddUncheckedUniqueReference(TestLiterals.Foo);
            listOfStrings.AddUncheckedUniqueReference(TestLiterals.Bar);

            Assert.Throws<IndexOutOfRangeException>(() => { listOfStrings.AddUncheckedUniqueReference(TestLiterals.HelloWorld); });
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void ContainsItem_String_ReturnsTrue()
        {
            SimpleList<string> listOfStrings = new SimpleList<string>(3);
            listOfStrings.AddUnchecked(TestLiterals.Foo);
            listOfStrings.AddUnchecked(TestLiterals.Bar);
            listOfStrings.AddUnchecked(TestLiterals.HelloWorld);

            bool evaluate = listOfStrings.ContainsItem(TestLiterals.Foo);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void ContainsItem_Object_ReturnsTrue()
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
        [Category(Literals.TestCategory)]
        public void ContainsReference_String_ReturnsTrue()
        {
            SimpleList<string> listOfStrings = new SimpleList<string>(3);
            listOfStrings.AddUnchecked(TestLiterals.Foo);
            listOfStrings.AddUnchecked(TestLiterals.Bar);
            listOfStrings.AddUnchecked(TestLiterals.TestSeed);

            bool evaluate = listOfStrings.ContainsReference(TestLiterals.Foo);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void ContainsReference_Object_ReturnsTrue()
        {
            object searchItem = new object();
            SimpleList<object> listItems = new SimpleList<object>(5);

            // Build test rig
            listItems.AddUnchecked(new object());
            listItems.AddUnchecked(searchItem);
            listItems.AddUnchecked(new object());
            listItems.AddUnchecked(new object());
            listItems.AddUnchecked(new object());

            bool evaluate = listItems.ContainsReference(searchItem);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
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
        [Category(Literals.TestCategory)]
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
        [Category(Literals.TestCategory)]
        public void RemoveFirstReference_MockData_RemovedItem()
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

            listItems.RemoveFirstReference(searchItem);

            bool evaluate = listItems.Array[1] != searchItem &&
                            listItems.ContainsReference(searchItem);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void RemoveFirstReference_MockData_NoItemReturnsFalse()
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

            bool evaluate = listItems.RemoveFirstReference(new object());

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
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
        [Category(Literals.TestCategory)]
        public void RemoveReferences_MockData_RemovedItems()
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

            listItems.RemoveReferences(searchItem);

            bool evaluate = listItems.ContainsReference(searchItem);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
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
        [Category(Literals.TestCategory)]
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

        [Test]
        [Category(Literals.TestCategory)]
        public void RemoveLastReference_MockData_RemovedItem()
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

            listItems.RemoveLastReference(searchItem);

            bool evaluate = listItems.Array[4] != searchItem &&
                            listItems.ContainsReference(searchItem);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void RemoveLastReference_MockData_NoItemReturnsFalse()
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

            bool evaluate = listItems.RemoveLastReference(new object());

            Assert.IsFalse(evaluate);
        }
    }
}