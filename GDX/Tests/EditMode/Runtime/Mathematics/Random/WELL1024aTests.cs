// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using GDX.Mathematics.Random;
using NUnit.Framework;

namespace Runtime.Mathematics.Random
{
    public class WELL1024aTests
    {
         private const string MockSeed = "TestSeed";
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void WELL1024a_MockSeed_FromKeyArray()
        // {
        //     int[] keys = {1, 2, 3};
        //     WELL1024a mockWell = new WELL1024a(keys);
        //
        //     bool evaluate = mockWell != null && mockWell.OriginalSeed == 19650218U;
        //
        //     Assert.IsTrue(evaluate);
        // }
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void WELL1024a_MockSeed_FromSystemRandom()
        // {
        //     WELL1024a mockWell = new WELL1024a();
        //
        //     bool evaluate = mockWell.OriginalSeed != 0;
        //
        //     Assert.IsTrue(evaluate);
        // }
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void WELL1024a_MockSeed_FromUInt()
        // {
        //     WELL1024a mockWell = new WELL1024a(2335793085U);
        //
        //     bool evaluate = mockWell.OriginalSeed == 2335793085U;
        //
        //     Assert.IsTrue(evaluate);
        // }
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void WELL1024a_MockSeed_FromInt()
        // {
        //     WELL1024a mockWell = new WELL1024a(2147483647);
        //
        //     bool evaluate = mockWell.OriginalSeed == 2147483647U;
        //
        //     Assert.IsTrue(evaluate);
        // }
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void WELL1024a_MockSeed_FromString()
        // {
        //     WELL1024a mockWell = new WELL1024a(MockSeed);
        //
        //     bool evaluate = mockWell.OriginalSeed == 2335793085U;
        //
        //     Assert.IsTrue(evaluate);
        // }
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void WELL1024a_MockSeed_FromStringLeaveCase()
        // {
        //     WELL1024a mockWell = new WELL1024a(MockSeed, false);
        //
        //     bool evaluate = mockWell.OriginalSeed == 2193700029U;
        //
        //     Assert.IsTrue(evaluate);
        // }
        //
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void WELL1024a_MockSeedPlus_FromString()
        // {
        //     WELL1024a mockWell = new WELL1024a(MockSeed + "Plus");
        //
        //     bool evaluate = mockWell.OriginalSeed != 2335793085U;
        //
        //     Assert.IsTrue(evaluate);
        // }
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void WELL1024a_NullKeys_ThrowsException()
        // {
        //     Assert.Throws<ArgumentNullException>(() => { new WELL1024a(null); });
        // }



        // [Test]
        // [Category("GDX.Tests")]
        // public void Bias_ZeroChance_ReturnsFalse()
        // {
        //     WELL1024a mockWell = new WELL1024a(MockSeed);
        //
        //     bool evaluate = mockWell.NextBias(0f);
        //
        //     Assert.IsFalse(evaluate);
        // }
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void Bias_Simple_ReturnsDeterministic()
        // {
        //     WELL1024a mockWell = new WELL1024a(MockSeed);
        //
        //     bool evaluate = mockWell.NextBias(0.5f);
        //
        //     Assert.IsTrue(evaluate);
        // }
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void NextInteger_Simple_ReturnsDeterministicValue()
        // {
        //     WELL1024a mockWell = new WELL1024a(MockSeed);
        //     int nextValue = mockWell.NextInteger();
        //
        //     bool evaluate = nextValue == 7985211;
        //
        //     Assert.IsTrue(evaluate);
        // }
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void NextInteger_MaxValue_ReturnsDeterministicValue()
        // {
        //     WELL1024a mockWell = new WELL1024a(MockSeed);
        //     int nextValue = mockWell.NextInteger(-11, 11);
        //
        //     bool evaluate = nextValue <= 11;
        //
        //     Assert.IsTrue(evaluate);
        // }
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void NextInteger_SameRange_ReturnsSame()
        // {
        //     WELL1024a mockWell = new WELL1024a(MockSeed);
        //     int nextValue = mockWell.NextInteger(1, 1);
        //
        //     bool evaluate = nextValue == 1;
        //
        //     Assert.IsTrue(evaluate);
        // }
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void NextInteger_ZeroValues_ReturnsZero()
        // {
        //     WELL1024a mockWell = new WELL1024a(MockSeed);
        //     int nextValue = mockWell.NextInteger(0, 0);
        //
        //     bool evaluate = nextValue == 0;
        //
        //     Assert.IsTrue(evaluate);
        // }
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void NextInteger_MinMaxValue_ReturnsDeterministicValue()
        // {
        //     WELL1024a mockWell = new WELL1024a(MockSeed);
        //     int nextValue = mockWell.NextInteger(11, 22);
        //
        //     bool evaluate = nextValue >= 11 && nextValue <= 22;
        //
        //     Assert.IsTrue(evaluate);
        // }
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void Next_BadMinMaxValue_ThrowsException()
        // {
        //     WELL1024a mockWell = new WELL1024a(MockSeed);
        //     Assert.Throws<ArgumentOutOfRangeException>(() => { mockWell.NextInteger(90, 80); });
        // }
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void NextBoolean_Simple_ReturnsDeterministicValue()
        // {
        //     WELL1024a mockWell = new WELL1024a(MockSeed);
        //
        //     bool evaluate = mockWell.NextBoolean();
        //
        //     Assert.IsFalse(evaluate);
        // }
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void NextBytes_Simple_FillsBuffer()
        // {
        //     WELL1024a mockWell = new WELL1024a(MockSeed);
        //     byte[] buffer = new byte[10];
        //     mockWell.NextBytes(buffer);
        //
        //     bool evaluate = false;
        //     foreach (byte b in buffer)
        //     {
        //         if (b != 0)
        //         {
        //             evaluate = true;
        //             break;
        //         }
        //     }
        //
        //     Assert.IsTrue(evaluate);
        // }
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void NextBytes_NullBuffer_ThrowsException()
        // {
        //     WELL1024a mockWell = new WELL1024a(MockSeed);
        //     Assert.Throws<ArgumentNullException>(() => { mockWell.NextBytes(null); });
        // }
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void NextDouble_Simple_ReturnsDeterministicValue()
        // {
        //     WELL1024a mockWell = new WELL1024a(MockSeed);
        //     double nextValue = mockWell.NextDouble();
        //
        //     bool evaluate = nextValue < 1f;
        //
        //     Assert.IsTrue(evaluate);
        // }
        //
        //
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void NextDoublePositive_ReturnsDeterministicValue()
        // {
        //     WELL1024a mockWell = new WELL1024a(MockSeed);
        //     double nextValue = mockWell.NextDouble(0);
        //
        //     bool evaluate = nextValue > 0f;
        //
        //     Assert.IsTrue(evaluate);
        // }
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void NextUnsignedInteger_Simple_ReturnsDeterministicValue()
        // {
        //     WELL1024a mockWell = new WELL1024a(MockSeed);
        //     uint nextValue = mockWell.NextUnsignedInteger();
        //
        //     bool evaluate = nextValue == 15970430;
        //
        //     Assert.IsTrue(evaluate);
        // }
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void NextSingle_Simple_ReturnsDeterministicValue()
        // {
        //     WELL1024a mockWell = new WELL1024a(MockSeed);
        //     float nextValue = mockWell.NextSingle();
        //
        //     bool evaluate = nextValue == 0.003718404f;
        //
        //     Assert.IsTrue(evaluate);
        // }
        //
        //
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void NextSingle_SameRange_ReturnsSame()
        // {
        //     WELL1024a mockWell = new WELL1024a(MockSeed);
        //     float nextValue = mockWell.NextSingle(1f, 1f);
        //
        //     bool evaluate = nextValue == 1f;
        //
        //     Assert.IsTrue(evaluate);
        // }
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void NextSingle_MinMaxValue_ReturnsDeterministicValue()
        // {
        //     WELL1024a mockWell = new WELL1024a(MockSeed);
        //     float nextValue = mockWell.NextSingle(2f, 10f);
        //
        //     bool evaluate = nextValue >= 2f && nextValue <= 10f;
        //
        //     Assert.IsTrue(evaluate);
        // }
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void NextSingle_BadMinMaxValue_ThrowsException()
        // {
        //     WELL1024a mockWell = new WELL1024a(MockSeed);
        //     Assert.Throws<ArgumentOutOfRangeException>(() => { mockWell.NextSingle(90f, 80f); });
        // }
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void NextSinglePositive_Simple_ReturnsDeterministicValue()
        // {
        //     WELL1024a mockWell = new WELL1024a(MockSeed);
        //     float nextValue = mockWell.NextSingle();
        //
        //     bool evaluate = nextValue > 0f;
        //
        //     Assert.IsTrue(evaluate);
        // }
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void NextUnsignedInteger_MaxValue_ReturnsDeterministicValue()
        // {
        //     WELL1024a mockWell = new WELL1024a(MockSeed);
        //     uint nextValue = mockWell.NextUnsignedInteger(100);
        //
        //     bool evaluate = nextValue <= 100;
        //
        //     Assert.IsTrue(evaluate);
        // }
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void NextUnsignedInteger_MinMaxValue_ReturnsDeterministicValue()
        // {
        //     WELL1024a mockWell = new WELL1024a(MockSeed);
        //     uint nextValue = mockWell.NextUnsignedInteger(98, 100);
        //
        //     bool evaluate = nextValue >= 98 && nextValue <= 100;
        //
        //     Assert.IsTrue(evaluate);
        // }
        //
        // [Test]
        // [Category("GDX.Tests")]
        // public void NextUnsignedInteger_BadMinMaxValue_ThrowsException()
        // {
        //     WELL1024a mockWell = new WELL1024a(MockSeed);
        //     Assert.Throws<ArgumentOutOfRangeException>(() => { mockWell.NextUnsignedInteger(90, 80); });
        // }
    }
}