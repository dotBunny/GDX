// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using GDX.Mathematics.Random;
using NUnit.Framework;

namespace Runtime.Mathematics.Random
{
    public class WELL1024aTests
    {
        private const string MockSeed = "TestSeed";

        [Test]
        [Category("GDX.Tests")]
        public void WELL1024a_FromInteger_StrippedSeed()
        {
            WELL1024a mockWell = new WELL1024a(-999);

            bool evaluate = mockWell != null && mockWell.OriginalSeed == 999u;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void WELL1024a_FromString_NonAndForcedDifferent()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed, false);
            WELL1024a mockWellUpper = new WELL1024a(MockSeed);

            bool evaluate = mockWell != null && mockWellUpper != null &&
                            mockWell.OriginalSeed != mockWellUpper.OriginalSeed;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void WELL1024a_FromStringUppercase_SameOriginalSeed()
        {
            WELL1024a mockWell = new WELL1024a("BOB", false);
            WELL1024a mockWellUpper = new WELL1024a("BoB");

            bool evaluate = mockWell != null && mockWellUpper != null &&
                            mockWell.OriginalSeed == mockWellUpper.OriginalSeed;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void WELL1024a_FromUnsignedInteger_MatchedSeed()
        {
            WELL1024a mockWell = new WELL1024a(999u);

            bool evaluate = mockWell != null && mockWell.OriginalSeed == 999u;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void WELL1024a_FromRestoreState_Matched()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed);
            mockWell.Sample();
            mockWell.Sample();
            mockWell.Sample();
            WELL1024a.WellState saved = mockWell.GetState();
            WELL1024a restoreWell = new WELL1024a(saved);

            bool evaluate = mockWell != null && restoreWell != null &&
                            mockWell.Index == restoreWell.Index &&
                            mockWell.OriginalSeed == restoreWell.OriginalSeed;

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

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void NextBoolean_ZeroChance_ReturnsFalse()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed);

            bool evaluate = mockWell.NextBoolean(0f);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void NextBoolean_PerfectChance_ReturnsTrue()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed);

            bool evaluate = mockWell.NextBoolean(1f);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void NextBoolean_Simple_ReturnsDeterministicValue()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed);

            bool evaluate = mockWell.NextBoolean();

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void NextInteger_Simple_ReturnsDeterministicValue()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed);
            int nextValue = mockWell.NextInteger();

            bool evaluate = nextValue == 1696340346;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void NextInteger_SameRange_ReturnsSameValue()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed);
            int nextValue = mockWell.NextInteger(1, 1);

            bool evaluate = nextValue == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void NextInteger_ZeroValues_ReturnsZeroValue()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed);
            int nextValue = mockWell.NextInteger(0, 0);

            bool evaluate = nextValue == 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void NextInteger_MinMaxValue_ReturnsDeterministicValue()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed);
            int nextValue = mockWell.NextInteger(11, 22);

            bool evaluate = nextValue >= 11 && nextValue <= 22;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
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

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
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

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
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
                if (nextValues[i - 1] == nextValues[i])
                {
                    evaluate = false;
                    break;
                }
            }

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void NextDouble_Simple_ReturnsDeterministicValue()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed);
            double nextValue = mockWell.NextDouble();

            bool evaluate = nextValue == 1.4200338144748743E+308;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void NextSingle_Simple_ReturnsDeterministicValue()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed);
            float nextValue = mockWell.NextSingle();

            bool evaluate =  nextValue.ToString().Substring(0,5) == "2.687";

            Assert.IsTrue(evaluate, nextValue.ToString());
        }

        [Test]
        [Category("GDX.Tests")]
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
                if (nextValues[i - 1] == nextValues[i])
                {
                    evaluate = false;
                    break;
                }
            }

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void NextUnsignedInteger_Simple_ReturnsDeterministicValue()
        {
            WELL1024a mockWell = new WELL1024a(MockSeed);
            uint nextValue = mockWell.NextUnsignedInteger();

            bool evaluate = nextValue == 3843823994;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
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

            Assert.IsTrue(evaluate);
        }


        // [Test]
        // [Category("GDX.Tests")]
        // public void RangeAnalysis()
        // {
        //     WELL1024a mockWell = new WELL1024a(MockSeed);
        //     int valueCount = 10000000;
        //     StringBuilder fileContent = new StringBuilder();
        //     for (int i = 0; i < valueCount; i++)
        //     {
        //         fileContent.AppendLine($"{mockWell.NextUnsignedInteger(uint.MinValue, uint.MaxValue).ToString()}");
        //     }
        //
        //     System.IO.File.WriteAllText(
        //         System.IO.Path.Combine(UnityEngine.Application.dataPath, "ranges.txt"),
        //         fileContent.ToString()
        //         );
        // }

    }
}