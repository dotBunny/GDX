﻿// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if !UNITY_DOTSRUNTIME

using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace GDX
{
    /// <summary>
    ///     <see cref="Vector2" /> Based Extension Methods
    /// </summary>
    /// <exception cref="UnsupportedRuntimeException">Not supported on DOTS Runtime.</exception>
    [VisualScriptingCompatible(2)]
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
            const float k_BaseMultiplier = 1E-06f;

            return math.abs(otherVector2.x - targetVector2.x) < math.max(
                       k_BaseMultiplier * math.max(math.abs(targetVector2.x), math.abs(otherVector2.x)),
                       moddedEpsilon) &&
                   math.abs(otherVector2.y - targetVector2.y) < math.max(
                       k_BaseMultiplier * math.max(math.abs(targetVector2.y), math.abs(otherVector2.y)),
                       moddedEpsilon);
        }

        /// <summary>
        ///     Get the slope of a <see cref="Vector2" />.
        /// </summary>
        /// <param name="targetVector2">The <see cref="Vector2" /> to evaluate.</param>
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
            const float k_HalfMultiplier = 0.5f;
            return new Vector2(
                targetVector2.x + (otherVector2.x - targetVector2.x) * k_HalfMultiplier,
                targetVector2.y + (otherVector2.y - targetVector2.y) * k_HalfMultiplier
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

        /// <summary>
        ///     Attempt to parse a <see cref="string" /> into a <see cref="Vector2" />.
        /// </summary>
        /// <remarks>This isn't great for runtime performance, it should be used predominantly when reconstructing data.</remarks>
        /// <param name="targetString">The <see cref="string" /> to convert into a <see cref="Vector2" /> if possible.</param>
        /// <param name="outputVector2">The outputted <see cref="Vector2" />.</param>
        /// <returns>true/false if the conversion was successful.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParseVector2(this string targetString, out Vector2 outputVector2)
        {
            // Find split points
            int splitIndex = targetString.IndexOf(',', 0);
            if (splitIndex == -1)
            {
                outputVector2 = Vector2.zero;
                return false;
            }

            // Get source parts
            string sourceX = targetString.Substring(0, splitIndex);
            string sourceY = targetString.Substring(splitIndex + 1);

            // Parse components
            if (!float.TryParse(sourceX, out float parsedX))
            {
                outputVector2 = Vector2.zero;
                return false;
            }

            if (!float.TryParse(sourceY, out float parsedY))
            {
                outputVector2 = Vector2.zero;
                return false;
            }

            // Everything looks good, assign the values
            outputVector2 = new Vector2(parsedX, parsedY);
            return true;
        }
    }
}
#endif // !UNITY_DOTSRUNTIME