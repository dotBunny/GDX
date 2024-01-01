// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Mathematics.Random
{
    public class RandomWrapper : IRandomProvider
    {
        readonly System.Random m_Random;

        public RandomWrapper()
        {
            m_Random = new System.Random();
        }

        public RandomWrapper(int seed)
        {
            m_Random = new System.Random(seed);
        }

        /// <inheritdoc />
        public bool NextBoolean(float chance = 0.5f)
        {
            return m_Random.NextDouble() <= chance;
        }

        /// <inheritdoc />
        public void NextBytes(byte[] buffer)
        {
            m_Random.NextBytes(buffer);
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
            return m_Random.NextDouble();
        }
    }
}