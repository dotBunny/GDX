// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.Collections.Pooling
{
    /// <summary>
    ///     An interface describing the functionality needed for an item to be understood by the <see cref=""/>,
    ///     if callbacks are to be made.
    /// </summary>
    public interface IGameObjectPoolItem
    {
        GameObject GetGameObject();

        /// <summary>
        ///     Return the <see cref="IManagedPool" /> which the <see cref="IGameObjectPoolItem" /> is currently managed by.
        /// </summary>
        /// <remarks>It is possible that a pooled item may exist without a parent in some custom scenario.</remarks>
        /// <returns>An <see cref="IManagedPool" /> if the <see cref="IGameObjectPoolItem" /> is attached to a pool, otherwise null.</returns>
        IManagedPool GetParentPool();

        /// <summary>
        ///     Is the <see cref="IGameObjectPoolItem" /> thought to be valid?
        /// </summary>
        /// <remarks>
        ///     Sometimes a <see cref="UnityEngine.GameObject" /> may get destroyed without the pool knowing, this contains
        ///     checks to validate if the item can be returned to a pool and reused appropriately.
        /// </remarks>
        /// <returns>true/false if the item is found to be valid.</returns>
        bool IsValidItem();

        void OnReturnedToPool();

        void OnSpawnedFromPool();

        void ReturnToPool();

        void SetParentPool(IManagedPool targetManagedPool);
    }
}