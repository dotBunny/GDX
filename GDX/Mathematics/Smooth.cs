// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace GDX.Mathematics
{
    /// <summary>
    ///     Some helpful interpolation functionality.
    /// </summary>
    [VisualScriptingCompatible(8)]
    public static class Smooth
    {
        // ReSharper disable CommentTypo
        /// <summary>
        ///     Smooths between <paramref name="previousValue" /> and <paramref name="targetValue" /> based on time since
        ///     the last sample and a given half-life.
        /// </summary>
        /// <remarks>Assumes wibbly wobbly, timey wimey.</remarks>
        /// <param name="previousValue">Ideally, the previous output value.</param>
        /// <param name="targetValue">The target value.</param>
        /// <param name="halfLife">
        ///     Half of the time it would take to go from <paramref name="previousValue" /> to
        ///     <paramref name="targetValue" /> if time were constant.
        /// </param>
        /// <param name="elapsedTime">
        ///     The amount of time that has transpired since the <paramref name="previousValue" /> was
        ///     generated.
        /// </param>
        /// <returns>A smoothed value.</returns>
        // ReSharper restore CommentTypo
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Exponential(float previousValue, float targetValue, float halfLife,
            float elapsedTime = float.NaN)
        {
            return halfLife <= 0f
                ? targetValue
                : math.lerp(previousValue, targetValue, HalfLifeToSmoothingFactor(halfLife, elapsedTime));
        }

        /// <summary>
        ///     Takes a <paramref name="halfLife" /> value, and outputs a factor based on <paramref name="elapsedTime" />.
        /// </summary>
        /// <remarks>Not providing a value for <paramref name="elapsedTime" /> will result in using <c>Time.deltaTime</c>.</remarks>
        /// <param name="halfLife">The desired half-life.</param>
        /// <param name="elapsedTime">The time since the last sample.</param>
        /// <returns>The coefficient value applied to the weight(t) of a lerp.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float HalfLifeToSmoothingFactor(float halfLife, float elapsedTime)
        {
            if (halfLife <= 0f)
            {
                return float.NaN;
            }

            float count = elapsedTime / halfLife;
            return 1f - math.pow(0.5f, count);
        }
    }
}