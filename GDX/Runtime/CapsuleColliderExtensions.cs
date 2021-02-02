// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using UnityEngine;

namespace GDX
{
    /// <summary>
    ///     <see cref="UnityEngine.CapsuleCollider" /> Based Extension Methods
    /// </summary>
    public static class CapsuleColliderExtensions
    {
        /// <summary>
        ///     Get a <see cref="Vector3" /> based orientation of the <paramref name="targetCapsuleCollider"/>.
        /// </summary>
        /// <param name="targetCapsuleCollider">The capsule collider</param>
        /// <returns>The direction of a <see cref="CapsuleCollider"/> in its local space.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Direction(this CapsuleCollider targetCapsuleCollider)
        {
            int direction = targetCapsuleCollider.direction;
            if(direction == 0) return Vector3.right;
            return direction == 1 ? Vector3.up : Vector3.forward;
        }
    }
}