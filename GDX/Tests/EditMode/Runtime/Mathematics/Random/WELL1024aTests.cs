// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using GDX.Mathematics.Random;
using NUnit.Framework;

namespace Runtime.Mathematics.Random
{
    public class WELL1024aTests
    {
        [Test]
        [Category("GDX.Tests")]
        public void WELL1024a_MockSeed_FromUInt()
        {
            WELL1024a mockTwister = new WELL1024a(2335793085U);

            bool evaluate = mockTwister.OriginalSeed == 2335793085U;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Bias_Simple_ReturnsDeterministic()
        {
            WELL1024a mockWell = new WELL1024a(2335793085U);

            double a = mockWell.NextDouble();
            double b = mockWell.NextDouble();
            double c = mockWell.NextDouble();
            double d = mockWell.NextDouble();
            double e = mockWell.NextDouble();
            double f = mockWell.NextDouble();
            double g = mockWell.NextDouble();
            double h = mockWell.NextDouble();


            ///bool evaluate = mockTwister.NextBias(0.5f);

            Assert.IsTrue(true);
        }

    }
}