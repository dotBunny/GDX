// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using NUnit.Framework;

namespace GDX.Mathematics.Random
{
#pragma warning disable IDE1006
    // ReSharper disable once InconsistentNaming
    public class WELL1024aTests
#pragma warning restore IDE1006
    {
        public const string MockSeed = "TestSeed";

        [Test]
        [Category(Core.TestCategory)]
        public void WELL1024a_FromInteger_StrippedSeed()
        {
            WELL1024a mockWell = new WELL1024a(-999);

            bool evaluate = mockWell.OriginalSeed == 999u;

            mockWell.Dispose();

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void WELL1024a_FromString_NonAndForcedDifferent()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed, false);
            WELL1024a mockWellUpper = new WELL1024a(MockSeed);

            bool evaluate = mockWell.OriginalSeed != mockWellUpper.OriginalSeed;

            mockWell.Dispose();
            mockWellUpper.Dispose();

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void WELL1024a_FromStringUppercase_SameOriginalSeed()
        {
            WELL1024a mockWell = new WELL1024a("BOB", false);
            WELL1024a mockWellUpper = new WELL1024a("BoB");

            bool evaluate = mockWell.OriginalSeed == mockWellUpper.OriginalSeed;

            mockWell.Dispose();
            mockWellUpper.Dispose();

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void WELL1024a_FromUnsignedInteger_MatchedSeed()
        {
            WELL1024a mockWell = new WELL1024a(999u);

            bool evaluate = mockWell.OriginalSeed == 999u;

            mockWell.Dispose();

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void WELL1024a_FromRestoreState_Matched()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed);
            mockWell.Sample();
            mockWell.Sample();
            mockWell.Sample();
            WELL1024a.WellState saved = mockWell.GetState();

            WELL1024a restoreWell = new WELL1024a(saved);

            bool evaluate = mockWell.Index == restoreWell.Index &&
                            mockWell.OriginalSeed == restoreWell.OriginalSeed &&
                            mockWell.SampleCount == restoreWell.SampleCount;

            if (evaluate)
            {
                for (int i = 0; i < 32; i++)
                {
                    if (mockWell.State[i] != restoreWell.State[i])
                    {
                        evaluate = false;
                        break;
                    }
                }
            }


            mockWell.Dispose();
            restoreWell.Dispose();
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void NextBoolean_ZeroChance_ReturnsFalse()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed);

            bool evaluate = mockWell.NextBoolean(0f);

            mockWell.Dispose();

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void NextBoolean_PerfectChance_ReturnsTrue()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed);

            bool evaluate = mockWell.NextBoolean(1f);

            mockWell.Dispose();

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void NextBoolean_Simple_ReturnsDeterministicValue()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed);

            bool evaluate = mockWell.NextBoolean();

            mockWell.Dispose();
            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void NextInteger_Simple_ReturnsDeterministicValue()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed);
            int nextValue = mockWell.NextInteger();

            bool evaluate = nextValue == 1921911996;

            mockWell.Dispose();
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void NextInteger_SameRange_ReturnsSameValue()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed);
            int nextValue = mockWell.NextInteger(1, 1);

            bool evaluate = nextValue == 1;

            mockWell.Dispose();
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void NextInteger_ZeroValues_ReturnsZeroValue()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed);
            int nextValue = mockWell.NextInteger(0, 0);

            bool evaluate = nextValue == 0;

            mockWell.Dispose();
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void NextInteger_MinMaxValue_ReturnsDeterministicValue()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed);
            int nextValue = mockWell.NextInteger(11, 22);

            bool evaluate = nextValue >= 11 && nextValue <= 22;

            mockWell.Dispose();
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void NextInteger_Multiple_ReturnsDifferentValues()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed);

            int[] nextValues = new int[64];
            for (int i = 0; i < 64; i++)
            {
                nextValues[i] = mockWell.NextInteger();
            }

            bool evaluate = true;
            for (int i = 1; i < 64; i++)
            {
                if (nextValues[i - 1] == nextValues[i])
                {
                    evaluate = false;
                    break;
                }
            }

            mockWell.Dispose();
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void NextBytes_Simple_FillsBuffer()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed);
            byte[] buffer = new byte[10];
            mockWell.NextBytes(buffer);

            bool evaluate = false;
            foreach (byte b in buffer)
            {
                if (b != 0)
                {
                    evaluate = true;
                    break;
                }
            }

            mockWell.Dispose();
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void NextDouble_Multiple_ReturnsDifferentValues()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed);

            double[] nextValues = new double[64];
            for (int i = 0; i < 64; i++)
            {
                nextValues[i] = mockWell.NextDouble();
            }

            bool evaluate = true;
            for (int i = 1; i < 64; i++)
            {
                if (Math.Abs(nextValues[i - 1] - nextValues[i]) < Platform.DoubleTolerance)
                {
                    evaluate = false;
                    break;
                }
            }

            mockWell.Dispose();
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void NextDouble_Simple_ReturnsDeterministicValue()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed);
            double nextValue = mockWell.NextDouble();

            bool evaluate = Math.Abs(nextValue - 0.89496001484803855d) < Platform.DoubleTolerance;

            mockWell.Dispose();
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void NextSingle_Simple_ReturnsDeterministicValue()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed);
            float nextValue = mockWell.NextSingle();

            bool evaluate =  Math.Abs(nextValue - 0.894959986f) < Platform.DoubleTolerance;

            mockWell.Dispose();
            Assert.IsTrue(evaluate, nextValue.ToString(CultureInfo.InvariantCulture));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void NextSingle_Multiple_ReturnsDifferentValues()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed);

            float[] nextValues = new float[64];
            for (int i = 0; i < 64; i++)
            {
                nextValues[i] = mockWell.NextSingle();
            }

            bool evaluate = true;
            for (int i = 1; i < 64; i++)
            {
                if (Math.Abs(nextValues[i - 1] - nextValues[i]) < Platform.DoubleTolerance)
                {
                    evaluate = false;
                    break;
                }
            }

            mockWell.Dispose();
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void NextUnsignedInteger_Simple_ReturnsDeterministicValue()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed);
            uint nextValue = mockWell.NextUnsignedInteger();

            bool evaluate = nextValue == 3843823994;

            mockWell.Dispose();
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void NextUnsignedInteger_Multiple_ReturnsDifferentValues()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed);

            uint[] nextValues = new uint[64];
            for (int i = 0; i < 64; i++)
            {
                nextValues[i] = mockWell.NextUnsignedInteger();
            }

            bool evaluate = true;
            for (int i = 1; i < 64; i++)
            {
                if (nextValues[i - 1] == nextValues[i])
                {
                    evaluate = false;
                    break;
                }
            }

            mockWell.Dispose();
            Assert.IsTrue(evaluate);
        }
    }
}