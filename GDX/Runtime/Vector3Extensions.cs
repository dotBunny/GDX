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
    }
}