// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;

namespace GDX.Collections.Generic
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="CircularBuffer{T}" />.
    /// </summary>
    public class CircularBufferTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void Add_MockData_Normal()
        {
            CircularBuffer<int> mockData = new CircularBuffer<int>(4);
            mockData.Add(4);
            mockData.Add(3);
            mockData.Add(2);
            mockData.Add(1);

            bool evaluate = mockData.Array[0] == 4 &&
                            mockData.Array[1] == 3 &&
                            mockData.Array[2] == 2 &&
                            mockData.Array[3] == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Add_MockData_Wrapped()
        {
            CircularBuffer<int> mockData = new CircularBuffer<int>(4);
            mockData.Add(4);
            mockData.Add(3);
            mockData.Add(2);
            mockData.Add(1);
            mockData.Add(0);

            bool evaluate = mockData.Array[0] == 0 &&
                            mockData.Array[1] == 3 &&
                            mockData.Array[2] == 2 &&
                            mockData.Array[3] == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Clear_MockData_Empty()
        {
            CircularBuffer<int> mockData = new CircularBuffer<int>(4);
            mockData.Add(4);
            mockData.Add(3);
            mockData.Add(2);
            mockData.Add(1);

            mockData.Clear();

            bool evaluate = mockData.Count == 0 && mockData.StartIndex == 0 && mockData.EndIndex == 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Get_MockData_First()
        {
            CircularBuffer<int> mockData = new CircularBuffer<int>(4);
            mockData.Add(1);
            mockData.Add(0);
            mockData.Add(0);
            mockData.Add(0);

            bool evaluate = mockData[0] == 1;
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Get_MockData_Last()
        {
            CircularBuffer<int> mockData = new CircularBuffer<int>(4);
            mockData.Add(1);
            mockData.Add(0);
            mockData.Add(0);
            mockData.Add(2);

            bool evaluate = mockData[3] == 2;
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Get_MockData_Wrapped()
        {
            CircularBuffer<int> mockData = new CircularBuffer<int>(4);
            mockData.Add(1);
            mockData.Add(2);
            mockData.Add(3);
            mockData.Add(4);
            mockData.Add(5);

            bool evaluate = mockData[0] == 2 &&
                            mockData[3] == 5;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetBack_MockData_Valid()
        {
            CircularBuffer<int> mockData = new CircularBuffer<int>(4);
            mockData.Add(4);
            mockData.Add(3);
            mockData.Add(2);
            mockData.Add(1);
            mockData.Add(5);

            bool evaluate = mockData.GetBack() == 5;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetFront_MockData_Valid()
        {
            CircularBuffer<int> mockData = new CircularBuffer<int>(4);
            mockData.Add(4);
            mockData.Add(3);
            mockData.Add(2);
            mockData.Add(1);
            mockData.Add(5);

            bool evaluate = mockData.GetFront() == 3;

            Assert.IsTrue(evaluate);
        }


        [Test]
        [Category(Core.TestCategory)]
        public void IsEmpty_MockData_Empty()
        {
            CircularBuffer<int> mockData = new CircularBuffer<int>(4);
            bool evaluate = mockData.IsEmpty();
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IsEmpty_MockData_ClearedEmpty()
        {
            CircularBuffer<int> mockData = new CircularBuffer<int>(4);
            mockData.Add(4);
            mockData.Add(3);
            mockData.Add(2);
            mockData.Add(1);
            mockData.Add(5);

            mockData.Clear();

            bool evaluate = mockData.IsEmpty();
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IsEmpty_MockData_NotEmpty()
        {
            CircularBuffer<int> mockData = new CircularBuffer<int>(4);
            mockData.Add(4);
            mockData.Add(3);
            mockData.Add(2);
            mockData.Add(1);
            mockData.Add(5);

            bool evaluate = mockData.IsEmpty();
            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IsFull_MockData_Full()
        {
            CircularBuffer<int> mockData = new CircularBuffer<int>(4);
            mockData.Add(4);
            mockData.Add(3);
            mockData.Add(2);
            mockData.Add(1);

            bool evaluate = mockData.IsFull();
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IsFull_MockData_NotFull()
        {
            CircularBuffer<int> mockData = new CircularBuffer<int>(4);
            mockData.Add(4);
            mockData.Add(3);
            mockData.Add(2);

            bool evaluate = mockData.IsFull();
            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void PopBack_MockData_GetValue()
        {
            CircularBuffer<int> mockData = new CircularBuffer<int>(4);
            mockData.Add(1);
            mockData.Add(2);
            mockData.Add(3);
            mockData.Add(4);
            mockData.Add(5);

            mockData.PopBack();
            bool evaluate = mockData.Count == 3 &&
                            mockData[2] == 4;
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void PopFront_MockData_GetValue()
        {
            CircularBuffer<int> mockData = new CircularBuffer<int>(4);
            mockData.Add(1);
            mockData.Add(2);
            mockData.Add(3);
            mockData.Add(4);
            mockData.Add(5);

            mockData.PopFront();
            bool evaluate = mockData.Count == 3 &&
                            mockData[0] == 3;
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void PushBack_MockData_InjectedValue()
        {
            CircularBuffer<int> mockData = new CircularBuffer<int>(4);
            mockData.Add(1);
            mockData.Add(2);
            mockData.Add(3);
            mockData.Add(4);
            mockData.Add(5);

            mockData.PushBack(0);
            bool evaluate = mockData.Count == 4 &&
                            mockData[3] == 0;
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void PushFront_MockData_InjectedValue()
        {
            CircularBuffer<int> mockData = new CircularBuffer<int>(4);
            mockData.Add(1);
            mockData.Add(2);
            mockData.Add(3);
            mockData.Add(4);
            mockData.Add(5);

            mockData.PushFront(0);
            bool evaluate = mockData.Count == 4 &&
                            mockData[0] == 0;
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Set_MockData_First()
        {
            CircularBuffer<int> mockData = new CircularBuffer<int>(4);
            mockData.Add(1);
            mockData.Add(0);
            mockData.Add(0);
            mockData.Add(0);

            mockData[0] = 5;
            bool evaluate = mockData[0] == 5;
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Set_MockData_Last()
        {
            CircularBuffer<int> mockData = new CircularBuffer<int>(4);
            mockData.Add(1);
            mockData.Add(0);
            mockData.Add(0);
            mockData.Add(2);

            mockData[3] = 5;
            bool evaluate = mockData[3] == 5;
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Set_MockData_Wrapped()
        {
            CircularBuffer<int> mockData = new CircularBuffer<int>(4);
            mockData.Add(1);
            mockData.Add(2);
            mockData.Add(3);
            mockData.Add(4);
            mockData.Add(5);

            mockData[3] = 6;
            bool evaluate = mockData[3] == 6;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ToArray_MockData_NonWrapped()
        {
            CircularBuffer<int> mockData = new CircularBuffer<int>(4);
            mockData.Add(0);
            mockData.Add(0);
            mockData.Add(0);
            mockData.Add(1);

            int[] data = mockData.ToArray();
            bool evaluate = data[0] == 0 &&
                            data[1] == 0 &&
                            data[2] == 0 &&
                            data[3] == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ToArray_MockData_Wrapped()
        {
            CircularBuffer<int> mockData = new CircularBuffer<int>(4);
            mockData.Add(0);
            mockData.Add(0);
            mockData.Add(0);
            mockData.Add(1);

            // Should now write over the oldest value (at index 0)
            mockData.Add(1);

            int[] data = mockData.ToArray();
            bool evaluate = mockData.Array[0] == 1 &&
                            mockData.Array[1] == 0 &&
                            mockData.Array[2] == 0 &&
                            mockData.Array[3] == 1 &&
                            data[0] == 0 &&
                            data[1] == 0 &&
                            data[2] == 1 &&
                            data[3] == 1;

            Assert.IsTrue(evaluate);
        }
    }
}