// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;
using Unity.Collections;

namespace GDX.Collections.Generic
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="NativeUniformArray3D{T}" />.
    /// </summary>
    public class NativeUniformArray3DTests
    {
        const int k_TargetStride = 3;
        const int k_TargetLength = 27;
        const int k_TargetValue = 42;

        NativeUniformArray3D<int> m_MockArray;

        [SetUp]
        public void Setup()
        {
            m_MockArray = new NativeUniformArray3D<int>(k_TargetStride, Allocator.Temp, NativeArrayOptions.ClearMemory);
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
            Assert.IsTrue(m_MockArray.Stride == k_TargetStride);
            Assert.IsTrue(m_MockArray.Length == k_TargetLength);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetByIndex_LastSpot_ValidEntry()
        {
            m_MockArray.Array[k_TargetLength - 1] = k_TargetValue;

            int getLast = m_MockArray[k_TargetStride - 1, k_TargetStride - 1, k_TargetStride - 1];

            Assert.IsTrue(getLast == k_TargetValue);
        }
    }
}