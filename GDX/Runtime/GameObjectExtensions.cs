// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
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
        ///     Destroy child <see cref="GameObject" />.
        /// </summary>
        /// <param name="targetGameObject">The parent <see cref="GameObject" /> to look at.</param>
        /// <param name="deactivateBeforeDestroy">
        ///     Should the <paramref name="targetGameObject" />'s children be deactivated before
        ///     destroying? This can be used to immediately hide an object, that will be destroyed at the end of the frame.
        /// </param>
        /// <param name="destroyInactive">Should inactive <see cref="GameObject" /> be destroyed as well?</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DestroyChildren(this GameObject targetGameObject, bool deactivateBeforeDestroy = true,
            bool destroyInactive = true)
        {
            targetGameObject.transform.DestroyChildren(deactivateBeforeDestroy, destroyInactive);
        }

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
                targetGameObject.transform.GetFirstComponentInChildrenComplex<T>(includeInactive, 0,
                    maxLevelsOfRecursion);
            if (returnComponent != null)
            {
                return returnComponent;
            }

            return !lookInChildrenFirst ? default : targetGameObject.GetComponent<T>();
        }

        /// <summary>
        ///     Get a component by type from a <paramref name="targetGameObject" />, if it is not found add and return it.
        /// </summary>
        /// <remarks>Adding a component at runtime is a performance nightmare. Use with caution!</remarks>
        /// <param name="targetGameObject">The <see cref="GameObject" /> that we should query for the component.</param>
        /// <typeparam name="T">The type of component.</typeparam>
        /// <returns>The component on the <paramref name="targetGameObject" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetOrAddComponent<T>(this GameObject targetGameObject) where T : Component
        {
            T instance = targetGameObject.GetComponent<T>();
            return instance ? instance : targetGameObject.AddComponent<T>();
        }

        /// <summary>
        ///     Get an in scene path to the <paramref name="targetGameObject" />.
        /// </summary>
        /// <param name="targetGameObject">The <see cref="GameObject" /> which to derive a path from.</param>
        /// <returns>A created path <see cref="System.String" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetScenePath(this GameObject targetGameObject)
        {
            return targetGameObject.transform.GetScenePath();
        }
    }
}