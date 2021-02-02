// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using UnityEngine;
#if GDX_MATHEMATICS
using Unity.Mathematics;

#endif

namespace GDX
{
    /// <summary>
    ///     <see cref="Vector3" /> Based Extension Methods
    /// </summary>
    /// <remarks>
    ///     <i>Unit tests are found in GDX.Tests.EditMode, under Runtime.Vector3ExtensionsTests.</i>
    /// </remarks>
    public static class Vector3Extensions
    {
        /// <summary>
        ///     Is one <see cref="Vector3" /> approximately similar to another <see cref="Vector3" />?
        /// </summary>
        /// <remarks>Includes optimized Unity.Mathematics approach.</remarks>
        /// <param name="targetVector3">Point A</param>
        /// <param name="otherVector3">Point B</param>
        /// <returns>Are the two <see cref="Vector3" /> approximately the same?</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Approximately(this Vector3 targetVector3, Vector3 otherVector3)
        {
            float moddedEpsilon = Mathf.Epsilon * 8f;
            const float baseMultiplier = 1E-06f;

#if GDX_MATHEMATICS
            return math.abs(otherVector3.x - targetVector3.x) < math.max(
                       baseMultiplier * math.max(math.abs(targetVector3.x), math.abs(otherVector3.x)),
                       moddedEpsilon) &&
                   math.abs(otherVector3.y - targetVector3.y) < math.max(
                       baseMultiplier * math.max(math.abs(targetVector3.y), math.abs(otherVector3.y)),
                       moddedEpsilon) &&
                   math.abs(otherVector3.z - targetVector3.z) < math.max(
                       baseMultiplier * math.max(math.abs(targetVector3.z), math.abs(otherVector3.z)),
                       moddedEpsilon);
#else
            return Mathf.Abs(otherVector3.x - targetVector3.x) < (double)Mathf.Max(
                       baseMultiplier * Mathf.Max(Mathf.Abs(targetVector3.x), Mathf.Abs(otherVector3.x)),
                       moddedEpsilon) &&
                   Mathf.Abs(otherVector3.y - targetVector3.y) < (double)Mathf.Max(
                       baseMultiplier * Mathf.Max(Mathf.Abs(targetVector3.y), Mathf.Abs(otherVector3.y)),
                       moddedEpsilon) &&
                   Mathf.Abs(otherVector3.z - targetVector3.z) < (double)Mathf.Max(
                       baseMultiplier * Mathf.Max(Mathf.Abs(targetVector3.z), Mathf.Abs(otherVector3.z)),
                       moddedEpsilon);
#endif
        }

        /// <summary>
        ///     Get the horizontal distance between two <see cref="Vector3"/> points.
        /// </summary>
        /// <remarks>Ignores the Y-axis completely.</remarks>
        /// <param name="targetVector3">Point A</param>
        /// <param name="otherVector3">Point B</param>
        /// <returns>The horizontal distance.</returns>
        public static float HorizontalDistance(this Vector3 targetVector3, Vector3 otherVector3)
        {
            float num1 = targetVector3.x - otherVector3.x;
            float num2 = targetVector3.z - otherVector3.z;
#if GDX_MATHEMATICS
            return (float)math.sqrt(num1 * (double)num1 + num2 * (double)num2);
#else
            return Mathf.Sqrt(num1 * num1 + num2 * num2);
#endif
        }

        /// <summary>
        ///     Get the midpoint between two <see cref="Vector3" />s.
        /// </summary>
        /// <param name="targetVector3">Point A</param>
        /// <param name="otherVector3">Point B</param>
        /// <returns>The midpoint between <paramref name="targetVector3" /> and <paramref name="otherVector3" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Midpoint(this Vector3 targetVector3, Vector3 otherVector3)
        {
            return new Vector3(
                targetVector3.x + (otherVector3.x - targetVector3.x) * 0.5f,
                targetVector3.y + (otherVector3.y - targetVector3.y) * 0.5f,
                targetVector3.z + (otherVector3.z - targetVector3.z) * 0.5f
            );
        }

        /// <summary>
        ///     Find the index of the <see cref="Vector3" /> in <paramref name="otherVector3" /> that is nearest to the
        ///     <paramref name="targetVector3" />.
        /// </summary>
        /// <param name="targetVector3">The <see cref="Vector3" /> to use as the point of reference.</param>
        /// <param name="otherVector3">An array of <see cref="Vector3" /> positions to evaluate for which one is nearest.</param>
        /// <returns>
        ///     The index of the nearest <paramref name="otherVector3" /> element to <paramref name="targetVector3" />.
        ///     Returning -1 if the the <paramref name="otherVector3" /> has no elements or is null.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NearestIndex(this Vector3 targetVector3, Vector3[] otherVector3)
        {
            // We found nothing to compare against
            if (otherVector3 == null || otherVector3.Length == 0)
            {
                return -1;
            }

            float closestSquareMagnitude = float.PositiveInfinity;
            int closestIndex = -1;
            int otherVector3Length = otherVector3.Length;

            // Loop through the provided points and figure out what is closest (close enough).
            for (int i = 0; i < otherVector3Length; i++)
            {
                float squareDistance = (otherVector3[i] - targetVector3).sqrMagnitude;
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