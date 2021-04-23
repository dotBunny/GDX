// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using GDX.Licensed.Mathematics.Random;
using NUnit.Framework;

namespace Runtime.Mathematics.Random
{
    public class MersenneTwisterTests
    {
        private const string MockSeed = "TestSeed";

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void MersenneTwister_MockSeed_FromKeyArray()
        {
            int[] keys = {1, 2, 3};
            MersenneTwister mockTwister = new MersenneTwister(keys);

            bool evaluate = mockTwister != null && mockTwister.OriginalSeed == 19650218U;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void MersenneTwister_MockSeed_FromSystemRandom()
        {
            MersenneTwister mockTwister = new MersenneTwister();

            bool evaluate = mockTwister.OriginalSeed != 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void MersenneTwister_MockSeed_FromUInt()
        {
            MersenneTwister mockTwister = new MersenneTwister(2335793085U);

            bool evaluate = mockTwister.OriginalSeed == 2335793085U;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void MersenneTwister_MockSeed_FromInt()
        {
            MersenneTwister mockTwister = new MersenneTwister(2147483647);

            bool evaluate = mockTwister.OriginalSeed == 2147483647U;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void MersenneTwister_MockSeed_FromString()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);

            bool evaluate = mockTwister.OriginalSeed == 2335793085U;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void MersenneTwister_MockSeed_FromStringLeaveCase()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed, false);

            bool evaluate = mockTwister.OriginalSeed == 2193700029U;

            Assert.IsTrue(evaluate);
        }


        [Test]
        [Category("GDX.Licensed.Tests")]
        public void MersenneTwister_MockSeedPlus_FromString()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed + "Plus");

            bool evaluate = mockTwister.OriginalSeed != 2335793085U;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void MersenneTwister_NullKeys_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => { new MersenneTwister(null); });
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void Bias_ZeroChance_ReturnsFalse()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);

            bool evaluate = mockTwister.NextBias(0f);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void Bias_Simple_ReturnsDeterministic()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);

            bool evaluate = mockTwister.NextBias(0.5f);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void Next_Simple_ReturnsDeterministicValue()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);
            int nextValue = mockTwister.Next();

            bool evaluate = nextValue == 7985211;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void Next_MaxValue_ReturnsDeterministicValue()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);
            int nextValue = mockTwister.Next(11);

            bool evaluate = nextValue <= 11;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void Next_SameRange_ReturnsSame()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);
            int nextValue = mockTwister.Next(1, 1);

            bool evaluate = nextValue == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void Next_BadMaxValue_ThrowsException()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);
            Assert.Throws<ArgumentOutOfRangeException>(() => { mockTwister.Next(-1); });
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void Next_ZeroMaxValue_ReturnsZero()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);
            int nextValue = mockTwister.Next(0);

            bool evaluate = nextValue == 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void Next_MinMaxValue_ReturnsDeterministicValue()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);
            int nextValue = mockTwister.Next(11, 22);

            bool evaluate = nextValue >= 11 && nextValue <= 22;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void Next_BadMinMaxValue_ThrowsException()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);
            Assert.Throws<ArgumentOutOfRangeException>(() => { mockTwister.Next(90, 80); });
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void NextBoolean_Simple_ReturnsDeterministicValue()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);

            bool evaluate = mockTwister.NextBoolean();

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void NextBytes_Simple_FillsBuffer()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);
            byte[] buffer = new byte[10];
            mockTwister.NextBytes(buffer);

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
        [Category("GDX.Licensed.Tests")]
        public void NextBytes_NullBuffer_ThrowsException()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);
            Assert.Throws<ArgumentNullException>(() => { mockTwister.NextBytes(null); });
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void NextDouble_Simple_ReturnsDeterministicValue()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);
            double nextValue = mockTwister.NextDouble();

            bool evaluate = nextValue < 1f;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void NextDouble_IncludeOne_ReturnsDeterministicValue()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);
            double nextValue = mockTwister.NextDouble(true);

            bool evaluate = nextValue <= 1f;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void NextDouble_DontIncludeOne_ReturnsDeterministicValue()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);
            double nextValue = mockTwister.NextDouble(false);

            bool evaluate = nextValue < 1f;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void NextDoublePositive_ReturnsDeterministicValue()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);
            double nextValue = mockTwister.NextDoublePositive();

            bool evaluate = nextValue > 0f;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void NextUnsignedInteger_Simple_ReturnsDeterministicValue()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);
            uint nextValue = mockTwister.NextUnsignedInteger();

            bool evaluate = nextValue == 15970430;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void NextSingle_Simple_ReturnsDeterministicValue()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);
            float nextValue = mockTwister.NextSingle();

            bool evaluate = nextValue == 0.003718404f;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void NextSingle_IncludeOne_ReturnsDeterministicValue()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);
            float nextValue = mockTwister.NextSingle(true);

            bool evaluate = nextValue <= 1f;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void NextSingle_DontIncludeOne_ReturnsDeterministicValue()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);
            float nextValue = mockTwister.NextSingle(true);

            bool evaluate = nextValue < 1f;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void NextSingle_SameRange_ReturnsSame()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);
            float nextValue = mockTwister.NextSingle(1f, 1f);

            bool evaluate = nextValue == 1f;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void NextSingle_MinMaxValue_ReturnsDeterministicValue()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);
            float nextValue = mockTwister.NextSingle(2f, 10f);

            bool evaluate = nextValue >= 2f && nextValue <= 10f;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void NextSingle_BadMinMaxValue_ThrowsException()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);
            Assert.Throws<ArgumentOutOfRangeException>(() => { mockTwister.NextSingle(90f, 80f); });
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void NextSinglePositive_Simple_ReturnsDeterministicValue()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);
            float nextValue = mockTwister.NextSinglePositive();

            bool evaluate = nextValue > 0f;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void NextUnsignedInteger_MaxValue_ReturnsDeterministicValue()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);
            uint nextValue = mockTwister.NextUnsignedInteger(100);

            bool evaluate = nextValue <= 100;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void NextUnsignedInteger_MinMaxValue_ReturnsDeterministicValue()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);
            uint nextValue = mockTwister.NextUnsignedInteger(98, 100);

            bool evaluate = nextValue >= 98 && nextValue <= 100;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Licensed.Tests")]
        public void NextUnsignedInteger_BadMinMaxValue_ThrowsException()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);
            Assert.Throws<ArgumentOutOfRangeException>(() => { mockTwister.NextUnsignedInteger(90, 80); });
        }
    }
}