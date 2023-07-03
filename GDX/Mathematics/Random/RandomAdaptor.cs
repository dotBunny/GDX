// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace GDX.Mathematics.Random
{
    // ReSharper disable CommentTypo
    /// <summary>
    ///     Adapter to utilize a <see cref="IRandomProvider" /> with <see cref="System.Random" /> based systems, wrapping the
    ///     provider in a class object with expected overrides.
    /// </summary>
    /// <remarks>This will create IL <c>callvert</c> operation codes! Try not to use this.</remarks>
    // ReSharper restore CommentTypo
    [VisualScriptingCompatible(4)]
    public class RandomAdaptor : System.Random
    {
        readonly IRandomProvider m_Provider;

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
            m_Provider = provider;
        }

        /// <summary>
        ///     Is the provider present, and not null?
        /// </summary>
        /// <returns>true/false a provider is not null.</returns>
        public bool HasProvider()
        {
            return m_Provider != null;
        }

        [ExcludeFromCodeCoverage]
        protected override double Sample()
        {
            return m_Provider.Sample();
        }

        public override int Next()
        {
            return m_Provider.NextInteger();
        }

        public override int Next(int minValue, int maxValue)
        {
            if (maxValue != int.MaxValue)
            {
                maxValue++;
            }

            return m_Provider.NextInteger(minValue, maxValue);
        }

        public override int Next(int maxValue)
        {
            if (maxValue != int.MaxValue)
            {
                maxValue++;
            }

            return m_Provider.NextInteger(0, maxValue);
        }

        public override double NextDouble()
        {

            return m_Provider.NextDouble();
        }

        public override void NextBytes(byte[] buffer)
        {
            m_Provider.NextBytes(buffer);
        }
    }
}