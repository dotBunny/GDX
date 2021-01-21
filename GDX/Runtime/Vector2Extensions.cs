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
    ///     <see cref="Vector2" /> Based Extension Methods
    /// </summary>
    /// <remarks>
    ///     Unit testing found in GDX.Tests.EditMode, under Runtime.Vector2ExtensionsTests.
    /// </remarks>
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
    }
}