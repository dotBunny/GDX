// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using GDX.Mathematics.Random;
using NUnit.Framework;

namespace Runtime.Mathematics.Random
{
    public class RandomAdaptorTests
    {
        [Test]
        [Category(GDX.Core.TestCategory)]
        public void RandomAdaptor_FromWELL1024a_Created()
        {
            WELL1024a mockWell = new WELL1024a(WELL1024aTests.MockSeed);

            RandomAdaptor random = new RandomAdaptor(mockWell);

            bool evaluate = random != null;

            mockWell.Dispose();
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void Next_MockData_DifferentValues()
        {
            WELL1024a mockWell = new WELL1024a(WELL1024aTests.MockSeed);

            RandomAdaptor random1 = new RandomAdaptor(mockWell);

            int a = random1.Next();
            int b = random1.Next();
            int c = random1.Next();

            WELL1024a mockWell2 = new WELL1024a(mockWell.GetState());

            RandomAdaptor random2 = new RandomAdaptor(mockWell2);

            int d = random2.Next();
            int e = random2.Next();
            int f = random2.Next();

            bool evaluate =
                mockWell.OriginalSeed == mockWell2.OriginalSeed &&
                a != b &&
                b != c &&
                c != d &&
                d != e &&
                e != f;

            mockWell.Dispose();
            mockWell2.Dispose();
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void Next_MaxValue_ValuesInRange()
        {
            WELL1024a mockWell = new WELL1024a(WELL1024aTests.MockSeed);

            RandomAdaptor random1 = new RandomAdaptor(mockWell);

            int a = random1.Next(10);
            int b = random1.Next(10);
            int c = random1.Next(10);

            bool evaluate = a >= 0 && a <= 10 &&
                            b >= 0 && b <= 10 &&
                            c >= 0 && c <= 10;

            mockWell.Dispose();
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void Next_Range_ValuesInRange()
        {
            WELL1024a mockWell = new WELL1024a(WELL1024aTests.MockSeed);

            RandomAdaptor random1 = new RandomAdaptor(mockWell);

            int a = random1.Next(0,10);
            int b = random1.Next(0,10);
            int c = random1.Next(0,10);

            bool evaluate = a >= 0 && a <= 10 &&
                            b >= 0 && b <= 10 &&
                            c >= 0 && c <= 10;

            mockWell.Dispose();
            Assert.IsTrue(evaluate);
        }
    }
}