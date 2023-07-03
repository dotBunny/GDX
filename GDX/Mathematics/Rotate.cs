// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace GDX.Mathematics
{
    /// <summary>
    /// A set of functionality to extend on Unity's rotation based methods.
    /// </summary>
    [VisualScriptingCompatible(8)]
    public static class Rotate
    {
#if !UNITY_DOTSRUNTIME
        /// <summary>
        /// Create a quaternion based on a rotation from <paramref name="targetQuaternion"/> to <paramref name="otherQuaternion"/>.
        /// </summary>
        /// <param name="targetQuaternion">The source <see cref="Quaternion"/>.</param>
        /// <param name="otherQuaternion">The destination <see cref="Quaternion"/>.</param>
        /// <param name="rotationRate">How fast should the rotation occur.</param>
        /// <param name="elapsedTime">How long has elapsed since the rotation started.</param>
        /// <returns>A rotational value.</returns>
        /// <exception cref="UnsupportedRuntimeException">Not supported on DOTS Runtime.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Quaternion Towards(Quaternion targetQuaternion, Quaternion otherQuaternion, float rotationRate, float elapsedTime)
        {
            float rotatedAmount = rotationRate * elapsedTime;
            Quaternion rotationDelta = Quaternion.Inverse(targetQuaternion) * otherQuaternion;
            rotationDelta.ToAngleAxis(out float rotatedAngle, out Vector3 axis);
            rotatedAngle = math.min(rotatedAngle, rotatedAmount);
            return targetQuaternion * Quaternion.AngleAxis(rotatedAngle, axis);
        }
#endif // !UNITY_DOTSRUNTIME
    }
}