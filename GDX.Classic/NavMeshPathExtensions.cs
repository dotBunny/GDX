// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using Unity.Mathematics;

// ReSharper disable UnusedMember.Global

#if GDX_AI

namespace GDX.Classic
{
    /// <summary>
    ///     <see cref="UnityEngine.AI.NavMeshPath" /> Based Extension Methods
    /// </summary>
    [VisualScriptingCompatible(2)]
    // ReSharper disable once UnusedType.Global
    public static class NavMeshPathExtensions
    {
        /// <summary>
        ///     Get the total travel distance, from start to finish of a calculated <see cref="UnityEngine.AI.NavMeshPath" />.
        /// </summary>
        /// <remarks>The <see cref="UnityEngine.AI.NavMeshPath.corners" /> does allocate internally.</remarks>
        /// <param name="targetNavMeshPath">The calculated path to evaluate for its length.</param>
        /// <returns>The total distance of a calculated path.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetTotalDistance(this NavMeshPath targetNavMeshPath)
        {
            float calculatedLength = 0;

            // The iterator looks ahead one position
            int count = targetNavMeshPath.corners.Length - 1;

            // Look at points in the NavMeshPath
            for (int i = 0; i < count; i++)
            {
                calculatedLength += math.distance(targetNavMeshPath.corners[i], targetNavMeshPath.corners[i + 1]);
            }

            return calculatedLength;
        }

        /// <summary>
        ///     Get the total squared distance, from start to finish of a calculated <see cref="UnityEngine.AI.NavMeshPath" />.
        /// </summary>
        /// <remarks>The <see cref="UnityEngine.AI.NavMeshPath.corners" /> does allocate internally.</remarks>
        /// <param name="targetNavMeshPath">The calculated path to evaluate for its squared length.</param>
        /// <returns>The total squared distance of a calculated path.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetTotalDistanceSqr(this NavMeshPath targetNavMeshPath)
        {
            float calculatedLength = 0;

            // The iterator looks ahead one position
            int count = targetNavMeshPath.corners.Length - 1;

            // Look at points in the NavMeshPath
            for (int i = 0; i < count; i++)
            {
                calculatedLength += targetNavMeshPath.corners[i].DistanceSqr(targetNavMeshPath.corners[i + 1]);
            }

            return calculatedLength;
        }

        /// <summary>
        ///     Get a position along a <see cref="UnityEngine.AI.NavMeshPath" /> based on the travel distance along it.
        /// </summary>
        /// <param name="targetNavMeshPath">The calculated path to evaluate for the position.</param>
        /// <param name="distance">The distance along the calculated path to find the position at.</param>
        /// <returns>The position found on the <see cref="UnityEngine.AI.NavMeshPath" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetPositionAtDistance(this NavMeshPath targetNavMeshPath, float distance)
        {
            float calculatedLength = 0;

            // The iterator looks ahead one position
            int count = targetNavMeshPath.corners.Length - 1;
            for (int i = 0; i < count - 1; i++)
            {
                // Figure out the next corners length
                float nextCornerLength = calculatedLength +
                                         math.distance(targetNavMeshPath.corners[i], targetNavMeshPath.corners[i + 1]);

                if (math.abs(nextCornerLength - distance) < math.EPSILON)
                {
                    return targetNavMeshPath.corners[i + 1];
                }

                // Figure out how much further we would need to go, preventing overshot
                if (nextCornerLength > distance)
                {
                    return targetNavMeshPath.corners[i] +
                           (targetNavMeshPath.corners[i + 1] - targetNavMeshPath.corners[i]) *
                           (distance - calculatedLength) /
                           (nextCornerLength - calculatedLength);
                }

                // Stash calculated length for next path, as point is not in this iteration.
                calculatedLength = nextCornerLength;
            }

            return targetNavMeshPath.corners[count];
        }

        /// <summary>
        ///     Get a position along a <see cref="UnityEngine.AI.NavMeshPath" /> based on the travel square distance along it.
        /// </summary>
        /// <param name="targetNavMeshPath">The calculated path to evaluate for the position.</param>
        /// <param name="distance">The distance along the calculated path to find the position at.</param>
        /// <returns>The position found on the <see cref="UnityEngine.AI.NavMeshPath" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 GetPositionAtDistanceSqr(this NavMeshPath targetNavMeshPath, float distance)
        {
            float calculatedLength = 0;

            // The iterator looks ahead one position
            int count = targetNavMeshPath.corners.Length - 1;
            for (int i = 0; i < count - 1; i++)
            {
                // Figure out the next corners length
                float nextCornerLength = calculatedLength + targetNavMeshPath.corners[i].DistanceSqr(targetNavMeshPath.corners[i + 1]);
                if (math.abs(nextCornerLength - distance) < math.EPSILON)
                {
                    return targetNavMeshPath.corners[i + 1];
                }

                // Figure out how much further we would need to go, preventing overshot
                if (nextCornerLength > distance)
                {
                    return targetNavMeshPath.corners[i] +
                           (targetNavMeshPath.corners[i + 1] - targetNavMeshPath.corners[i]) *
                           (distance - calculatedLength) /
                           (nextCornerLength - calculatedLength);
                }

                // Stash calculated length for next path, as point is not in this iteration.
                calculatedLength = nextCornerLength;
            }

            return targetNavMeshPath.corners[count];
        }
    }
}

#endif // GDX_AI