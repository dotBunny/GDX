// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using UnityEngine;
#if GDX_MATHEMATICS
using Unity.Mathematics;

#endif

// ReSharper disable MemberCanBePrivate.Global

namespace GDX.Mathematics
{
    /// <summary>
    ///     Some helpful interpolation functionality.
    /// </summary>
    public static class Smooth
    {
        // ReSharper disable CommentTypo
        /// <summary>
        ///     Smooths between <paramref name="previousValue"/> and <paramref name="targetValue"/> based on time since the last sample and a given half-life.
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
        // ReSharper enable CommentType
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Exponential(float previousValue, float targetValue, float halfLife,
            float elapsedTime = float.NaN)
        {
            return halfLife <= 0f
                ? targetValue
#if GDX_MATHEMATICS
                : math.lerp(previousValue, targetValue, HalfLifeToSmoothingFactor(halfLife, elapsedTime));
#else
                : Mathf.Lerp(previousValue, targetValue, HalfLifeToSmoothingFactor(halfLife, elapsedTime));
#endif
        }

        /// <summary>
        ///     Takes a <paramref name="halfLife" /> value, and outputs a factor based on <paramref name="elapsedTime" />.
        /// </summary>
        /// <remarks>Not providing a value for <paramref name="elapsedTime" /> will result in using <c>Time.deltaTime</c>.</remarks>
        /// <param name="halfLife">The desired halflife.</param>
        /// <param name="elapsedTime">The time since the last sample.</param>
        /// <returns>The coefficient value applied to the weight(t) of a lerp.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float HalfLifeToSmoothingFactor(float halfLife, float elapsedTime = float.NaN)
        {
            if (halfLife <= 0f)
            {
                return float.NaN;
            }

            // If no elapsed time was passed in, we should fallback on Unity's frame time.
            if (float.IsNaN(elapsedTime))
            {
                elapsedTime = Time.deltaTime;
            }

            float count = elapsedTime / halfLife;
#if GDX_MATHEMATICS
            return 1f - math.pow(0.5f, count);
#else
            return 1f - Mathf.Pow(0.5f, count);
#endif
        }
    }
}