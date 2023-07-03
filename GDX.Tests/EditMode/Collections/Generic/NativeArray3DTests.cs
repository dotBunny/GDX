// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;
using Unity.Collections;
using Unity.Mathematics;

namespace GDX.Collections.Generic
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="NativeArray3D{T}" />.
    /// </summary>
    public class NativeArray3DTests
    {
        const int k_TargetWidth = 3;
        const int k_TargetHeight = 2;
        const int k_TargetDepth = 4;
        const int k_TargetLength = 24;

        const int k_TargetValue = 42;

        NativeArray3D<int> m_MockArray;

        [SetUp]
        public void Setup()
        {
            m_MockArray = new NativeArray3D<int>(k_TargetWidth, k_TargetHeight, k_TargetDepth,
                Allocator.Temp, NativeArrayOptions.ClearMemory);
        }

        [TearDown]
        public void TearDown()
        {
            m_MockArray.Dispose();
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_CorrectSizes()
        {
            Assert.IsTrue(m_MockArray.Width == k_TargetWidth);
            Assert.IsTrue(m_MockArray.Height == k_TargetHeight);
            Assert.IsTrue(m_MockArray.Depth == k_TargetDepth);
            Assert.IsTrue(m_MockArray.Length == k_TargetLength);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetByIndex_LastSpot_ValidEntry()
        {
            m_MockArray.Array[k_TargetLength - 1] = k_TargetValue;
            int getLast = m_MockArray[k_TargetWidth - 1, k_TargetHeight - 1, k_TargetDepth - 1];

            Assert.IsTrue(getLast == k_TargetValue);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SetByIndex_LastSpot_ValidEntry()
        {
            m_MockArray[k_TargetWidth - 1, k_TargetHeight - 1, k_TargetDepth - 1] = k_TargetValue;
            int getLast = m_MockArray.Array[k_TargetLength - 1];
            Assert.IsTrue(getLast == k_TargetValue);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetByInt3_LastSpot_ValidEntry()
        {
            int3 index = new int3(k_TargetWidth - 1, k_TargetHeight - 1, k_TargetDepth - 1);
            m_MockArray.Array[k_TargetLength - 1] = k_TargetValue;

            Assert.IsTrue(m_MockArray[index] == k_TargetValue);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SetByInt3_LastSpot_ValidEntry()
        {
            int3 index = new int3(k_TargetWidth - 1, k_TargetHeight - 1, k_TargetDepth - 1);
            m_MockArray[index] = k_TargetValue;
            Assert.IsTrue(m_MockArray.Array[k_TargetLength - 1] == k_TargetValue);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetFromIndex_LastSpot_Valid()
        {
            int3 expected = new int3(k_TargetWidth - 1, k_TargetHeight - 1, k_TargetDepth - 1);
            int3 eval = m_MockArray.GetFromIndex(k_TargetLength - 1);

            Assert.IsTrue(eval.Equals(expected));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetFromIndex_FirstSpot_Valid()
        {
            int3 expected = new int3(0, 0, 0);
            int3 eval = m_MockArray.GetFromIndex(0);

            Assert.IsTrue(eval.Equals(expected));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetFromIndex_LastFirstRow_Valid()
        {
            int3 expected = new int3(k_TargetWidth - 1, 0, 0);
            int3 eval = m_MockArray.GetFromIndex(k_TargetWidth - 1);

            Assert.IsTrue(eval.Equals(expected));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetFromIndex_LastBottomRow_Valid()
        {
            int3 expected = new int3(k_TargetWidth - 1, k_TargetHeight - 1, 0);
            int3 eval = m_MockArray.GetFromIndex(k_TargetWidth * k_TargetHeight - 1);

            Assert.IsTrue(eval.Equals(expected));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetFromIndex_SecondTopRowAtBack_Valid()
        {
            int3 expected = new int3(1, 0, k_TargetDepth - 1);
            int3 eval = m_MockArray.GetFromIndex((k_TargetWidth * k_TargetHeight) * (k_TargetDepth - 1) + 2 - 1);

            Assert.IsTrue(eval.Equals(expected));
        }
    }
}