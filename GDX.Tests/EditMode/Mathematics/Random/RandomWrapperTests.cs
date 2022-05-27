// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using NUnit.Framework;

namespace GDX.Mathematics.Random
{
    public class RandomWrapperTests
    {
        const int k_MockSeed = 42;

        [Test]
        [Category(Core.TestCategory)]
        public void RandomWrapper_NoSeed_Created()
        {
            RandomWrapper wrapper = new RandomWrapper();
            Assert.IsTrue(wrapper != null);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RandomWrapper_WithSeed_Created()
        {
            RandomWrapper wrapper = new RandomWrapper(k_MockSeed);
            Assert.IsTrue(wrapper != null);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void NextBoolean_FixedSeed_ReturnsTrue()
        {
            RandomWrapper wrapper = new RandomWrapper(212121);
            Assert.IsTrue(wrapper.NextBoolean());
        }

        [Test]
        [Category(Core.TestCategory)]
        public void NextBoolean_FixedSeed_ReturnsFalse()
        {
            RandomWrapper wrapper = new RandomWrapper(4);
            Assert.IsTrue(!wrapper.NextBoolean());
        }

        [Test]
        [Category(Core.TestCategory)]
        public void NextBytes_MockData_FillsBuffer()
        {
            RandomWrapper random1 = new RandomWrapper(k_MockSeed);

            byte[] buffer = new byte[10];
            random1.NextBytes(buffer);

            bool evaluate = false;
            foreach (byte b in buffer)
            {
                if (b != 0)
                {
                    evaluate = true;
                    break;
                }
            }
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void NextDouble_MockData_InRange()
        {
            RandomWrapper wrapper = new RandomWrapper(k_MockSeed);
            double value = wrapper.NextDouble(2, 4);
            Assert.IsTrue(value >= 2 && value <= 4);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void NextInteger_MockData_InRange()
        {
            RandomWrapper wrapper = new RandomWrapper(k_MockSeed);
            int value = wrapper.NextInteger(2, 4);
            Assert.IsTrue(value >= 2 && value <= 4);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void NextIntegerExclusive_MockData_InRange()
        {
            RandomWrapper wrapper = new RandomWrapper(k_MockSeed);
            int value = wrapper.NextIntegerExclusive(2, 4);
            Assert.IsTrue(value >= 2 && value < 4);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void NextSingle_MockData_InRange()
        {
            RandomWrapper wrapper = new RandomWrapper(k_MockSeed);
            float value = wrapper.NextSingle(2, 4);
            Assert.IsTrue(value >= 2 && value < 4);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void NextUnsignedInteger_MockData_InRange()
        {
            RandomWrapper wrapper = new RandomWrapper(k_MockSeed);
            uint value = wrapper.NextUnsignedInteger(2, 4);
            Assert.IsTrue(value >= 2 && value <= 4);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void NextUnsignedIntegerExclusive_MockData_InRange()
        {
            RandomWrapper wrapper = new RandomWrapper(k_MockSeed);
            uint value = wrapper.NextUnsignedIntegerExclusive(2, 4);
            Assert.IsTrue(value > 2 && value < 4);
        }
    }
}