// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Mathematics.Random
{
    /// <summary>
    ///     Adapter to utilize a <see cref="IRandomProvider" /> with <see cref="System.Random" /> based systems, wrapping the
    ///     provider in a class object with expected overrides.
    /// </summary>
    /// <remarks>This will create IL <c>callvert</c> operation codes! Try not to use this.</remarks>
    [VisualScriptingType]
    public class RandomAdaptor : System.Random
    {
        private readonly IRandomProvider _provider;

        /// <summary>
        ///     Create an adaptor object which behaves like <see cref="System.Random" />.
        /// </summary>
        /// <remarks>
        ///     Will cause boxing of <see langword="struct" /> based types like <see cref="WELL1024a" />.
        ///     This adaptor really should only be used where a method is expecting a <see cref="System.Random" />.
        /// </remarks>
        /// <param name="provider">A qualified <see cref="IRandomProvider" />.</param>
        public RandomAdaptor(IRandomProvider provider)
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