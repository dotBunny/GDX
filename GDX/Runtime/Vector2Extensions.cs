// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using UnityEngine;
#if GDX_MATHEMATICS
using Unity.Mathematics;

#endif

namespace GDX
{
    /// <summary>
    ///     <see cref="Vector2" /> Based Extension Methods
    /// </summary>
    [VisualScriptingCompatible(12)]
    public static class Vector2Extensions
    {
        /// <summary>
        ///     Is one <see cref="Vector2" /> approximately similar to another <see cref="Vector2" />?
        /// </summary>
        /// <remarks>Includes optimized Unity.Mathematics approach.</remarks>
        /// <param name="targetVector2">Point A</param>
        /// <param name="otherVector2">Point B</param>
        /// <returns>Are the two <see cref="Vector2" /> approximately the same?</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Approximately(this Vector2 targetVector2, Vector2 otherVector2)
        {
            float moddedEpsilon = Mathf.Epsilon * 8f;
            const float baseMultiplier = 1E-06f;

#if GDX_MATHEMATICS
            return math.abs(otherVector2.x - targetVector2.x) < math.max(
                       baseMultiplier * math.max(math.abs(targetVector2.x), math.abs(otherVector2.x)),
                       moddedEpsilon) &&
                   math.abs(otherVector2.y - targetVector2.y) < math.max(
                       baseMultiplier * math.max(math.abs(targetVector2.y), math.abs(otherVector2.y)),
                       moddedEpsilon);
#else
            return Mathf.Abs(otherVector2.x - targetVector2.x) < (double)Mathf.Max(
                       baseMultiplier * Mathf.Max(Mathf.Abs(targetVector2.x), Mathf.Abs(otherVector2.x)),
                       moddedEpsilon) &&
                   Mathf.Abs(otherVector2.y - targetVector2.y) < (double)Mathf.Max(
                       baseMultiplier * Mathf.Max(Mathf.Abs(targetVector2.y), Mathf.Abs(otherVector2.y)),
                       moddedEpsilon);
#endif
        }

        /// <summary>
        /// Get the slope of a <see cref="Vector2"/>.
        /// </summary>
        /// <param name="targetVector2">The <see cref="Vector2"/> to evaluate.</param>
        /// <returns>The slope value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Slope(this Vector2 targetVector2)
        {
            if (targetVector2.x == 0f)
            {
                return 0f;
            }
            return targetVector2.y / targetVector2.x;
        }

        /// <summary>
        ///     Get the midpoint between two <see cref="Vector2" />s.
        /// </summary>
        /// <param name="targetVector2">Point A</param>
        /// <param name="otherVector2">Point B</param>
        /// <returns>The midpoint between <paramref name="targetVector2" /> and <paramref name="otherVector2" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Midpoint(this Vector2 targetVector2, Vector2 otherVector2)
        {
            const float halfMultiplier = 0.5f;
            return new Vector2(
                targetVector2.x + (otherVector2.x - targetVector2.x) * halfMultiplier,
                targetVector2.y + (otherVector2.y - targetVector2.y) * halfMultiplier
            );
        }

        /// <summary>
        ///     Find the index of the <see cref="Vector2" /> in <paramref name="otherVector2" /> that is nearest to the
        ///     <paramref name="targetVector2" />.
        /// </summary>
        /// <param name="targetVector2">The <see cref="Vector2" /> to use as the point of reference.</param>
        /// <param name="otherVector2">An array of <see cref="Vector2" /> positions to evaluate for which one is nearest.</param>
        /// <returns>
        ///     The index of the nearest <paramref name="otherVector2" /> element to <paramref name="targetVector2" />.
        ///     Returning -1 if the the <paramref name="otherVector2" /> has no elements or is null.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NearestIndex(this Vector2 targetVector2, Vector2[] otherVector2)
        {
            // We found nothing to compare against
            if (otherVector2 == null || otherVector2.Length == 0)
            {
                return -1;
            }

            float closestSquareMagnitude = float.PositiveInfinity;
            int closestIndex = -1;
            int otherVector2Length = otherVector2.Length;

            // Loop through the provided points and figure out what is closest (close enough).
            for (int i = 0; i < otherVector2Length; i++)
            {
                float squareDistance = (otherVector2[i] - targetVector2).sqrMagnitude;
                if (float.IsNaN(squareDistance) || !(squareDistance < closestSquareMagnitude))
                {
                    continue;
                }

                closestSquareMagnitude = squareDistance;
                closestIndex = i;
            }

            return closestIndex;
        }
    }
}