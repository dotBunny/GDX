// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX
{
    /// <summary>
    ///     <see cref="UnityEngine.Transform" /> Based Extension Methods
    /// </summary>
    /// <remarks>
    ///     Used for MonoBehaviour workflows.
    /// </remarks>
    public static class TransformExtensions
    {
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
        public static T GetFirstComponentInChildrenComplex<T>(this Transform targetTransform, bool includeInactive,
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

                returnComponent = GetFirstComponentInChildrenComplex<T>(transformToCheck, includeInactive, currentDepth,
                    maxLevelsOfRecursion);
                if (returnComponent != null)
                {
                    return returnComponent;
                }
            }

            return returnComponent;
        }


        /// <summary>
        /// Get the number of immediate children active.
        /// </summary>
        /// <param name="targetTransform">The transform to look at's children.</param>
        /// <returns>The number of active children transforms.</returns>
        public static int GetActiveChildCount(this Transform targetTransform)
        {
            if (targetTransform == null)
            {
                return 0;
            }
            int counter = 0;
            int childCount = targetTransform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                if (targetTransform.GetChild(i).gameObject.activeSelf)
                {
                    counter++;
                }
            }

            return counter;
        }
    }
}