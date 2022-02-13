// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

// ReSharper disable UnusedMember.Global

namespace GDX.Mathematics.Random
{
    public class RandomWrapper : IRandomProvider
    {
        private readonly System.Random _random;

        public RandomWrapper()
        {
            _random = new System.Random();
        }
        public RandomWrapper(int seed)
        {
            _random = new System.Random(seed);
        }

        /// <inheritdoc />
        public bool NextBoolean(float chance = 0.5f)
        {
            return _random.NextDouble() <= chance;
        }

        /// <inheritdoc />
        public void NextBytes(byte[] buffer)
        {
            _random.NextBytes(buffer);
        }

        /// <inheritdoc />
        public double NextDouble(double minValue = 0, double maxValue = 1)
        {
            return Range.GetDouble(Sample(), minValue, maxValue);
        }

        /// <inheritdoc />
        public int NextInteger(int minValue = 0, int maxValue = int.MaxValue)
        {
            return Range.GetInteger(Sample(), minValue, maxValue);
        }

        /// <inheritdoc />
        public int NextIntegerExclusive(int minValue = 0, int maxValue = int.MaxValue)
        {
            return Range.GetInteger(Sample(), minValue + 1, maxValue - 1);
        }

        /// <inheritdoc />
        public float NextSingle(float minValue = 0, float maxValue = 1)
        {
            return Range.GetSingle(Sample(), minValue, maxValue);
        }

        /// <inheritdoc />
        public uint NextUnsignedInteger(uint minValue = uint.MinValue, uint maxValue = uint.MaxValue)
        {
            return Range.GetUnsignedInteger(Sample(), minValue, maxValue);
        }

        /// <inheritdoc />
        public uint NextUnsignedIntegerExclusive(uint minValue = uint.MinValue, uint maxValue = uint.MaxValue)
        {
            return Range.GetUnsignedInteger(Sample(), minValue + 1, maxValue - 1);
        }

        /// <inheritdoc />
        public double Sample()
        {
            return _random.NextDouble();
        }
    }
}