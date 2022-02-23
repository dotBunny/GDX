// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

#if !UNITY_DOTSRUNTIME
namespace GDX
{
    /// <summary>
    ///     <see cref="UnityEngine.Rigidbody" /> Based Extension Methods
    /// </summary>
    /// <exception cref="UnsupportedRuntimeException">Not supported on DOTS Runtime.</exception>
    [VisualScriptingCompatible(2)]
    public static class RigidbodyExtensions
    {
        /// <summary>
        ///     Get a <see cref="Rigidbody" />'s moment of inertia for a <paramref name="targetAxis" />.
        /// </summary>
        /// <remarks>
        ///     Provided <paramref name="targetAxis" /> must not be <see cref="Vector3.zero" />.
        /// </remarks>
        /// <param name="targetRigidbody">The <see cref="Rigidbody" /> to evaluate.</param>
        /// <param name="targetAxis">The axis use to calculate the moment of inertia.</param>
        /// <returns>The moment of inertia for the <paramref name="targetAxis" />.</returns>
        public static float MomentOfInertia(this Rigidbody targetRigidbody, Vector3 targetAxis)
        {
            // Normalize axis
            targetAxis = targetAxis.normalized;

            return targetAxis.sqrMagnitude == 0f
                ? float.NaN
                : Vector3.Scale(
                    Quaternion.Inverse(targetRigidbody.transform.rotation * targetRigidbody.inertiaTensorRotation) *
                    targetAxis, targetRigidbody.inertiaTensor).magnitude;
        }
    }
}
#endif // !UNITY_DOTSRUNTIME