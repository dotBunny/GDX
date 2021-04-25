// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using UnityEngine;
#if GDX_MATHEMATICS
using Unity.Mathematics;
#endif


namespace GDX.Mathematics
{
    /// <summary>
    /// A set of functionality to extend on Unity's on rotation based methods.
    /// </summary>
    [VisualScriptingUtility]
    public static class Rotate
    {
        /// <summary>
        /// Create a quaternion based on a rotation from <paramref name="targetQuaternion"/> to <paramref name="otherQuaternion"/>.
        /// </summary>
        /// <param name="targetQuaternion">The source <see cref="Quaternion"/>.</param>
        /// <param name="otherQuaternion">The destination <see cref="Quaternion"/>.</param>
        /// <param name="rotationRate">How fast should the rotation occur.</param>
        /// <param name="elapsedTime">How long has elapsed since the rotation started.</param>
        /// <returns>A rotational value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion Towards(Quaternion targetQuaternion, Quaternion otherQuaternion, float rotationRate, float elapsedTime)
        {
            float rotatedAmount = rotationRate * elapsedTime;
            Quaternion rotationDelta = Quaternion.Inverse(targetQuaternion) * otherQuaternion;
            rotationDelta.ToAngleAxis(out float rotatedAngle, out Vector3 axis);
#if GDX_MATHEMATICS
            rotatedAngle = math.min(rotatedAngle, rotatedAmount);
#else
            rotatedAngle = Mathf.Min(rotatedAngle, rotatedAmount);
#endif
            return targetQuaternion * Quaternion.AngleAxis(rotatedAngle, axis);
        }
    }
}