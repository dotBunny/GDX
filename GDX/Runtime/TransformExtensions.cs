// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace GDX
{
    /// <summary>
    ///     <see cref="UnityEngine.Transform" /> Based Extension Methods
    /// </summary>
    /// <remarks>
    ///     Used for MonoBehaviour workflows.
    /// </remarks>
    [VisualScriptingExtension]
    public static class TransformExtensions
    {
        /// <summary>
        ///     Destroy child <see cref="Transform"/>.
        /// </summary>
        /// <param name="targetTransform">The parent <see cref="Transform"/> to look at.</param>
        /// <param name="deactivateBeforeDestroy">Should the <paramref name="targetTransform"/> children's <see cref="GameObject"/>s be deactivated before destroying? This can be used to immediately hide an object, that will be destroyed at the end of the frame.</param>
        /// <param name="destroyInactive">Should inactive <see cref="GameObject"/> be destroyed as well?</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DestroyChildren(this Transform targetTransform, bool deactivateBeforeDestroy = true, bool destroyInactive = true)
        {
            int count = targetTransform.childCount;
            for (int i = 0; i < count; i++)
            {
                GameObject childObject = targetTransform.GetChild(i).gameObject;
                if (!destroyInactive && !childObject.activeInHierarchy)
                {
                    continue;
                }

                if (deactivateBeforeDestroy)
                {
                    childObject.SetActive(false);
                }

                Object.Destroy(childObject);
            }
        }

        /// <summary>
        /// Get the number of immediate children active.
        /// </summary>
        /// <param name="targetTransform">The transform to look at's children.</param>
        /// <returns>The number of active children transforms.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetActiveChildCount(this Transform targetTransform)
        {
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
        ///     Get an in scene path to the <paramref name="targetTransform"/>.
        /// </summary>
        /// <param name="targetTransform">The <see cref="Transform"/> which to derive a path from.</param>
        /// <returns>A created path <see cref="System.String"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetScenePath(this Transform targetTransform)
        {
            StringBuilder stringBuilder = new StringBuilder();
            while ( targetTransform != null )
            {
                stringBuilder.Insert(0, targetTransform.name);
                stringBuilder.Insert(0, '/');
                targetTransform = targetTransform.parent;
            }
#if UNITY_EDITOR
            if (targetTransform &&
                UnityEditor.EditorUtility.IsPersistent(targetTransform) )
            {
                stringBuilder.Append( " [P]" );
            }
#endif
            return stringBuilder.ToString();
        }
    }
}