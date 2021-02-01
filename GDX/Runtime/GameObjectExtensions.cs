// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX
{
    /// <summary>
    ///     <see cref="UnityEngine.GameObject" /> Based Extension Methods
    /// </summary>
    /// <remarks>
    ///     Used for MonoBehaviour workflows.
    /// </remarks>
    public static class GameObjectExtensions
    {
        /// <summary>
        ///     A slightly more complex version of <see cref="Component.GetComponentInChildren{T}(bool)" /> which allows for
        ///     different hinted search options.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         If the <paramref name="maxLevelsOfRecursion" /> is set to 1; this will search the
        ///         <paramref name="targetGameObject" /> and its immediate children only.
        ///     </para>
        ///     <para>
        ///         The internal <see cref="Component.GetComponentInChildren{T}(bool)" /> has optimizations internally in engine
        ///         code which make it faster in different scenarios.
        ///     </para>
        /// </remarks>
        /// <param name="targetGameObject">The target <see cref="GameObject" /> to use as the base for the search.</param>
        /// <param name="lookInChildrenFirst">
        ///     Should children <see cref="GameObject" /> be searched before the
        ///     <paramref name="targetGameObject" />'s <see cref="GameObject" />.
        /// </param>
        /// <param name="includeInactive">
        ///     Include inactive child <see cref="GameObject" />s when looking for the
        ///     <see cref="Component" />.
        /// </param>
        /// <param name="maxLevelsOfRecursion">
        ///     The maximum levels of recursion when looking for a <see cref="Component" />, -1 for
        ///     infinite recursion.
        /// </param>
        /// <typeparam name="T">The target <see cref="UnityEngine.Component" /> type that is being looked for.</typeparam>
        /// <returns>The first found <see cref="Component" />.</returns>
        public static T GetFirstComponentInChildrenComplex<T>(this GameObject targetGameObject,
            bool includeInactive = false, bool lookInChildrenFirst = false, int maxLevelsOfRecursion = -1)
        {
            // Make sure we have nothing to return if necessary.
            T returnComponent;

            // Early out looking at the immediate target first.
            if (!lookInChildrenFirst)
            {
                returnComponent = targetGameObject.GetComponent<T>();
                if (returnComponent != null)
                {
                    return returnComponent;
                }
            }

            returnComponent =
                FindFirstComponentRecursively<T>(targetGameObject.transform, includeInactive, 0, maxLevelsOfRecursion);
            if (returnComponent != null)
            {
                return returnComponent;
            }

            return !lookInChildrenFirst ? default : targetGameObject.GetComponent<T>();
        }

        /// <summary>
        ///     Search recursively for a <see cref="Component" /> on the <paramref name="targetTransform" />.
        /// </summary>
        /// <param name="targetTransform">The target <see cref="Transform" /> to use as the base for the search.</param>
        /// <param name="includeInactive">
        ///     Include inactive child <see cref="GameObject" />s when looking for the
        ///     <see cref="Component" />.
        /// </param>
        /// <param name="currentDepth">Current level of recursion.</param>
        /// <param name="maxLevelsOfRecursion">
        ///     The maximum levels of recursion when looking for a <see cref="Component" />, -1 for
        ///     infinite recursion.
        /// </param>
        /// <typeparam name="T">The target <see cref="UnityEngine.Component" /> type that is being looked for.</typeparam>
        /// <returns>The first found <see cref="Component" />.</returns>
        private static T FindFirstComponentRecursively<T>(this Transform targetTransform, bool includeInactive,
            int currentDepth, int maxLevelsOfRecursion)
        {
            // Make sure we have nothing to return if necessary.
            T returnComponent = default;

            // Increase depth count
            currentDepth++;

            if (maxLevelsOfRecursion >= 0 && currentDepth > maxLevelsOfRecursion)
            {
                return returnComponent;
            }

            int cachedChildCount = targetTransform.childCount;
            for (int i = 0; i < cachedChildCount; i++)
            {
                Transform transformToCheck = targetTransform.GetChild(i);

                // Don't include disabled transforms
                if (!transformToCheck.gameObject.activeSelf &&
                    !includeInactive)
                {
                    continue;
                }

                // Lets check the current transform for the component.
                returnComponent = transformToCheck.GetComponent<T>();
                if (returnComponent != null)
                {
                    return returnComponent;
                }

                // OK, time to deep dive.
                if (maxLevelsOfRecursion >= 0 && currentDepth >= maxLevelsOfRecursion)
                {
                    continue;
                }

                returnComponent = FindFirstComponentRecursively<T>(transformToCheck, includeInactive, currentDepth,
                    maxLevelsOfRecursion);
                if (returnComponent != null)
                {
                    return returnComponent;
                }
            }

            return returnComponent;
        }
    }
}