// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Text;
using NUnit.Framework;

namespace GDX.Mathematics
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of <see cref="Murmur3" />, against the published
    ///     MurmurHash3_x86_32 values.
    /// </summary>
    public class Murmur3Tests
    {
        const uint k_Seed = 0x9747b28cu;

        [Test]
        [Category(Core.TestCategory)]
        [TestCase("", 0u, 0x00000000u)]
        [TestCase("", 1u, 0x514E28B7u)]
        [TestCase("", 0xffffffffu, 0x81F16F39u)]
        [TestCase("a", k_Seed, 0x7FA09EA6u)]
        [TestCase("aaaa", k_Seed, 0x5A97808Au)]
        [TestCase("abcd", k_Seed, 0xF0478627u)]
        [TestCase("Hello, world!", k_Seed, 0x24884CBAu)]
        [TestCase("The quick brown fox jumps over the lazy dog", k_Seed, 0x2FA826CDu)]
        public void Hash_Bytes_MatchesPublished(string text, uint seed, uint expected)
        {
            uint hash = Murmur3.Hash(Encoding.ASCII.GetBytes(text), seed);

            Assert.AreEqual(expected, hash);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Hash_Words_MatchLittleEndianBytes()
        {
            uint[] words = { 0x00000000u, 0x00000001u, 0x87654321u, 0xffffffffu, 0x7fffffffu, 0xdeadbeefu };
            uint seed = 0;
            foreach (uint a in words)
            {
                foreach (uint b in words)
                {
                    seed = Murmur3.Mix(seed + 0x9e3779b9u);
                    byte[] one = Bytes(a);
                    byte[] two = Bytes(a, b);
                    byte[] three = Bytes(a, b, a ^ b);
                    byte[] four = Bytes(a, b, a ^ b, ~a);

                    Assert.AreEqual(Murmur3.Hash(one, seed), Murmur3.Hash(a, seed));
                    Assert.AreEqual(Murmur3.Hash(two, seed), Murmur3.Hash(a, b, seed));
                    Assert.AreEqual(Murmur3.Hash(three, seed), Murmur3.Hash(a, b, a ^ b, seed));
                    Assert.AreEqual(Murmur3.Hash(four, seed), Murmur3.Hash(a, b, a ^ b, ~a, seed));
                }
            }
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Hash_Offset_MatchesCopy()
        {
            byte[] data = Encoding.ASCII.GetBytes("--The quick brown fox jumps over the lazy dog--");

            uint hash = Murmur3.Hash(data, 2, data.Length - 4, k_Seed);

            Assert.AreEqual(0x2FA826CDu, hash);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Hash_WordOrder_Matters()
        {
            bool evaluate = Murmur3.Hash(1u, 2u, 0u) != Murmur3.Hash(2u, 1u, 0u);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Mix_Zero_StaysZero()
        {
            Assert.AreEqual(0u, Murmur3.Mix(0u));
        }

        static byte[] Bytes(params uint[] words)
        {
            byte[] bytes = new byte[words.Length * 4];
            for (int i = 0; i < words.Length; i++)
            {
                bytes[i * 4] = (byte)words[i];
                bytes[i * 4 + 1] = (byte)(words[i] >> 8);
                bytes[i * 4 + 2] = (byte)(words[i] >> 16);
                bytes[i * 4 + 3] = (byte)(words[i] >> 24);
            }

            return bytes;
        }
    }
}
