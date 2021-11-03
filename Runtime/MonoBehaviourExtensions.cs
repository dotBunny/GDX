// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using UnityEngine;

namespace GDX
{
    /// <summary>
    ///     <see cref="UnityEngine.MonoBehaviour" /> Based Extension Methods
    /// </summary>
    /// <remarks>
    ///     Used for MonoBehaviour workflows.
    /// </remarks>
    [VisualScriptingCompatible(2)]
    public static class MonoBehaviourExtensions
    {
        /// <summary>
        ///     A slightly more complex version of <see cref="Component.GetComponentInChildren{T}(bool)" /> which allows for
        ///     different hinted search options.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         If the <paramref name="maxLevelsOfRecursion" /> is set to 1; this will search the
        ///         <paramref name="targetMonoBehaviour" /> and its immediate children only.
        ///     </para>
        ///     <para>
        ///         The internal <see cref="Component.GetComponentInChildren{T}(bool)" /> has optimizations internally in engine
        ///         code which make it faster in different scenarios.
        ///     </para>
        /// </remarks>
        /// <typeparam name="T">The target <see cref="UnityEngine.Component" /> type that is being looked for.</typeparam>
        /// <param name="targetMonoBehaviour">The target <see cref="MonoBehaviour" /> to use as the base for the search.</param>
        /// <param name="lookInChildrenFirst">
        ///     Should children <see cref="GameObject" /> be searched before the
        ///     <paramref name="targetMonoBehaviour" />'s <see cref="GameObject" />.
        /// </param>
        /// <param name="includeInactive">
        ///     Include inactive child <see cref="GameObject" />s when looking for the
        ///     <see cref="Component" />.
        /// </param>
        /// <param name="maxLevelsOfRecursion">
        ///     The maximum levels of recursion when looking for a <see cref="Component" />, -1 for
        ///     infinite recursion.
        /// </param>
        /// <returns>The first found <see cref="Component" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetFirstComponentInChildrenComplex<T>(this MonoBehaviour targetMonoBehaviour,
            bool includeInactive = false, bool lookInChildrenFirst = false, int maxLevelsOfRecursion = -1)
        {
            return targetMonoBehaviour.gameObject.GetFirstComponentInChildrenComplex<T>(includeInactive,
                lookInChildrenFirst, maxLevelsOfRecursion);
        }
    }
}