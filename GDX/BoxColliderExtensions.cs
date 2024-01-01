// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if !UNITY_DOTSRUNTIME

using UnityEngine;

namespace GDX
{
    /// <summary>
    ///     <see cref="UnityEngine.BoxCollider" /> Based Extension Methods
    /// </summary>
    /// <exception cref="UnsupportedRuntimeException">Not supported on DOTS Runtime.</exception>
    [VisualScriptingCompatible(2)]
    public static class BoxColliderExtensions
    {
        /// <summary>
        ///     Is the <paramref name="worldPosition" /> inside of the <paramref name="targetBoxCollider" />?
        /// </summary>
        /// <param name="targetBoxCollider">The <see cref="BoxCollider" /> to use for evaluation.</param>
        /// <param name="worldPosition">A <see cref="Vector3" /> point in world space.</param>
        /// <returns>true/false if the world position is contained within the <paramref name="targetBoxCollider" />.</returns>
        public static bool ContainsPosition(this BoxCollider targetBoxCollider, Vector3 worldPosition)
        {
            worldPosition = targetBoxCollider.transform.InverseTransformPoint(worldPosition) - targetBoxCollider.center;

            Vector3 cachedSize = targetBoxCollider.size;

            float halfX = cachedSize.x * 0.5f;
            float halfY = cachedSize.y * 0.5f;
            float halfZ = cachedSize.z * 0.5f;

            return worldPosition.x < halfX && worldPosition.x > -halfX &&
                   worldPosition.y < halfY && worldPosition.y > -halfY &&
                   worldPosition.z < halfZ && worldPosition.z > -halfZ;
        }
    }
}
#endif // !UNITY_DOTSRUNTIME