// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Mathematics.Random
{
    /// <summary>
    ///     Adapter to utilize a <see cref="IRandomProvider" /> with <see cref="System.Random" /> based systems.
    /// </summary>
    public class RandomAdapter : System.Random
    {
        private readonly IRandomProvider _provider;

        public RandomAdapter(IRandomProvider provider)
        {
            _provider = provider;
        }

        protected override double Sample()
        {
            return _provider.Sample();
        }

        public override int Next()
        {
            return _provider.NextInteger(0);
        }

        public override int Next(int minValue, int maxValue)
        {
            if (maxValue != int.MaxValue)
            {
                maxValue++;
            }
            return _provider.NextInteger(minValue, maxValue);
        }

        public override int Next(int maxValue)
        {
            if (maxValue != int.MaxValue)
            {
                maxValue++;
            }
            return _provider.NextInteger(0, maxValue);
        }

        public override double NextDouble()
        {
            return _provider.NextDouble(0, 1d);
        }

        public override void NextBytes(byte[] buffer)
        {
            _provider.NextBytes(buffer);
        }
    }
}