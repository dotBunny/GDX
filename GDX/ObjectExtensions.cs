// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using UnityEngine;

namespace GDX
{
    /// <summary>
    ///     <see cref="UnityEngine.Object" /> Based Extension Methods
    /// </summary>
    [VisualScriptingCompatible(2)]
    public static class ObjectExtensions
    {
        /// <summary>
        ///     Destroy a <see cref="UnityEngine.Object"/> appropriately based on the current state of the Editor/Player.
        /// </summary>
        /// <param name="targetObject">The target <see cref="UnityEngine.Object"/> to be destroyed.</param>
        /// <param name="delay">How long should be waited before the <paramref name="targetObject"/> is destroyed?</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SafeDestroy(this Object targetObject, float delay = 0f)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Object.Destroy(targetObject, delay);
            }
            else
            {
                Object.DestroyImmediate(targetObject);
            }
#else
            Object.Destroy(targetObject, delay);
#endif // UNITY_EDITOR
        }
    }
}