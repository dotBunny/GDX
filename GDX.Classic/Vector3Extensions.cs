// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Mathematics;

namespace GDX.Classic
{
    /// <summary>
    ///     <see cref="Vector3" /> Based Extension Methods
    /// </summary>
    [VisualScriptingCompatible(2)]
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
            const float BaseMultiplier = 1E-06f;
            return math.abs(otherVector3.x - targetVector3.x) < math.max(
                       BaseMultiplier * math.max(math.abs(targetVector3.x), math.abs(otherVector3.x)),
                       moddedEpsilon) &&
                   math.abs(otherVector3.y - targetVector3.y) < math.max(
                       BaseMultiplier * math.max(math.abs(targetVector3.y), math.abs(otherVector3.y)),
                       moddedEpsilon) &&
                   math.abs(otherVector3.z - targetVector3.z) < math.max(
                       BaseMultiplier * math.max(math.abs(targetVector3.z), math.abs(otherVector3.z)),
                       moddedEpsilon);

        }

        /// <summary>
        ///     Calculate the squared distance between two <see cref="Vector3"/>.
        /// </summary>
        /// <remarks>
        ///     <para>Based on https://en.wikipedia.org/wiki/Euclidean_distance#Squared_Euclidean_distance.</para>
        /// </remarks>
        /// <param name="targetVector3">Point A</param>
        /// <param name="otherVector3">Point B</param>
        /// <returns>The squared distance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceSqr(this Vector3 targetVector3, Vector3 otherVector3)
        {
            float x = targetVector3.x - otherVector3.x;
            float y = targetVector3.y - otherVector3.y;
            float z = targetVector3.z - otherVector3.z;
            return x * x + y * y + z * z;
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
            return (float)math.sqrt(num1 * (double)num1 + num2 * (double)num2);
        }

        /// <summary>
        /// Calculate the distance from a <see cref="Vector3"/> to a <see cref="Ray"/>.
        /// </summary>
        /// <param name="targetVector3">The position.</param>
        /// <param name="targetRay">The line.</param>
        /// <returns>The distance.</returns>
        public static float DistanceToRay(this Vector3 targetVector3, Ray targetRay)
        {
            return Vector3.Cross(targetRay.direction, targetVector3 - targetRay.origin).magnitude;
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

        /// <summary>
        ///     Attempt to parse a <see cref="string" /> into a <see cref="Vector3" />.
        /// </summary>
        /// <remarks>This isn't great for runtime performance, it should be used predominantly when reconstructing data.</remarks>
        /// <param name="targetString">The <see cref="string" /> to convert into a <see cref="Vector3" /> if possible.</param>
        /// <param name="outputVector3">The outputted <see cref="Vector3" />.</param>
        /// <returns>true/false if the conversion was successful.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryParseVector3(this string targetString, out Vector3 outputVector3)
        {
            // Find split points
            int firstSplit = targetString.IndexOf(',', 0);
            if (firstSplit == -1)
            {
                outputVector3 = Vector3.zero;
                return false;
            }

            int secondSplit = targetString.IndexOf(',', firstSplit + 1);
            if (secondSplit == -1)
            {
                outputVector3 = Vector3.zero;
                return false;
            }

            // Get source parts
            string sourceX = targetString.Substring(0, firstSplit);
            string sourceY = targetString.Substring(firstSplit + 1, secondSplit - (firstSplit + 1));
            string sourceZ = targetString.Substring(secondSplit + 1);

            // Parse components
            if (!float.TryParse(sourceX, out float parsedX))
            {
                outputVector3 = Vector3.zero;
                return false;
            }

            if (!float.TryParse(sourceY, out float parsedY))
            {
                outputVector3 = Vector3.zero;
                return false;
            }

            if (!float.TryParse(sourceZ, out float parsedZ))
            {
                outputVector3 = Vector3.zero;
                return false;
            }

            // Everything looks good, assign the values
            outputVector3 = new Vector3(parsedX, parsedY, parsedZ);
            return true;
        }
    }
}