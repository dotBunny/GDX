// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if !UNITY_DOTSRUNTIME

using System.Runtime.CompilerServices;
using UnityEngine;

namespace GDX
{
    /// <summary>
    ///     <see cref="UnityEngine.CapsuleCollider" /> Based Extension Methods
    /// </summary>
    /// <exception cref="UnsupportedRuntimeException">Not supported on DOTS Runtime.</exception>
    [VisualScriptingCompatible(2)]
    public static class CapsuleColliderExtensions
    {
        /// <summary>
        ///     Get a <see cref="Vector3" /> based orientation of the <paramref name="targetCapsuleCollider" />.
        /// </summary>
        /// <param name="targetCapsuleCollider">The capsule collider</param>
        /// <returns>The direction of a <see cref="CapsuleCollider" /> in its local space.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Direction(this CapsuleCollider targetCapsuleCollider)
        {
            int direction = targetCapsuleCollider.direction;
            if (direction == 0)
            {
                return Vector3.right;
            }

            return direction == 1 ? Vector3.up : Vector3.forward;
        }

        /// <summary>
        ///     Return into <paramref name="topPosition" /> and <paramref name="bottomPosition" />, the respective world-space
        ///     position of a <see cref="CapsuleCollider" />'s spheres centers.
        /// </summary>
        /// <param name="targetCapsuleCollider">The <see cref="CapsuleCollider" /> having its spheres evaluated.</param>
        /// <param name="topPosition">The determined top spheres center position in world-space.</param>
        /// <param name="bottomPosition">The determined bottom spheres center position in world-space.</param>
        public static void OutSphereCenters(CapsuleCollider targetCapsuleCollider, out Vector3 topPosition,
            out Vector3 bottomPosition)
        {
            // Bring it local
            Vector3 cachedCenter = targetCapsuleCollider.center;
            topPosition = cachedCenter;

            // Calculate offset based on height/radius to center
            switch (targetCapsuleCollider.direction)
            {
                case 0:
                    topPosition.x = targetCapsuleCollider.height * 0.5f - targetCapsuleCollider.radius;
                    break;
                case 1:
                    topPosition.y = targetCapsuleCollider.height * 0.5f - targetCapsuleCollider.radius;
                    break;
                case 2:
                    topPosition.z = targetCapsuleCollider.height * 0.5f - targetCapsuleCollider.radius;
                    break;
            }

            // Invert bottom because the top was positive, now we need negative
            bottomPosition = -topPosition;

            // Convert positions to world-space
            topPosition = targetCapsuleCollider.transform.TransformPoint(topPosition);
            bottomPosition = targetCapsuleCollider.transform.TransformPoint(bottomPosition);
        }
    }
}
#endif // !UNITY_DOTSRUNTIME