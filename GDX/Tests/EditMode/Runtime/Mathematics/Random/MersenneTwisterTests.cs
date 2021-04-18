// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using GDX.Mathematics.Random;
using NUnit.Framework;

namespace Runtime.Mathematics.Random
{
    public class MersenneTwisterTests
    {
        private const string MockSeed = "TestSeed";

        [Test]
        [Category("GDX.Tests")]
        public void MersenneTwister_MockSeed_FromSystemRandom()
        {
            MersenneTwister mockTwister = new MersenneTwister();

            bool evaluate = (mockTwister.OriginalSeed != 0);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void MersenneTwister_MockSeed_FromUInt()
        {
            MersenneTwister mockTwister = new MersenneTwister(2335793085U);

            bool evaluate = (mockTwister.OriginalSeed == 2335793085U);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void MersenneTwister_MockSeed_FromInt()
        {
            MersenneTwister mockTwister = new MersenneTwister(2147483647);

            bool evaluate = (mockTwister.OriginalSeed == 2147483647U);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void MersenneTwister_MockSeed_FromString()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed);

            bool evaluate = (mockTwister.OriginalSeed == 2335793085U);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void MersenneTwister_MockSeed_FromStringLeaveCase()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed, false);

            bool evaluate = (mockTwister.OriginalSeed == 2193700029U);

            Assert.IsTrue(evaluate);
        }


        [Test]
        [Category("GDX.Tests")]
        public void GenerateSeed_MockSeedPlus_FromString()
        {
            MersenneTwister mockTwister = new MersenneTwister(MockSeed + "Plus");

            bool evaluate = (mockTwister.OriginalSeed != 2335793085U);

            Assert.IsTrue(evaluate);
        }
    }
}