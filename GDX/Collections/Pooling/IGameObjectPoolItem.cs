// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if !UNITY_DOTSRUNTIME

using UnityEngine;
using GDX.Collections.Pooling;

// ReSharper disable UnusedMember.Global
namespace GDX.Collections.Pooling
{
    /// <summary>
    ///     An interface describing the functionality needed for an item to be understood by <see cref="GameObjectPool" />,
    ///     if callbacks are to be made.
    /// </summary>
    /// <exception cref="UnsupportedRuntimeException">Not supported on DOTS Runtime.</exception>
    public interface IGameObjectPoolItem
    {
        GameObject GetGameObject();

        /// <summary>
        ///     Return the <see cref="IManagedPool" /> which the <see cref="IGameObjectPoolItem" /> is currently managed by.
        /// </summary>
        /// <remarks>It is possible that a pooled item may exist without a parent in some custom scenario.</remarks>
        /// <returns>
        ///     An <see cref="IManagedPool" /> if the <see cref="IGameObjectPoolItem" /> is attached to a pool, otherwise
        ///     null.
        /// </returns>
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

        /// <summary>
        ///     Called when this item is returned to the <see cref="IManagedPool" />.
        /// </summary>
        /// <remarks>
        ///     This should handle everything, including disabling the <see cref="GameObject" />.
        /// </remarks>
        void OnReturnedToPool();

        /// <summary>
        ///     Called when this item is spawned from the <see cref="IManagedPool" />.
        /// </summary>
        /// <remarks>
        ///     This should handle everything, including enabling the <see cref="GameObject" />.
        /// </remarks>
        void OnSpawnedFromPool();

        /// <summary>
        ///     Return the item to the <see cref="IManagedPool" /> which it is associated too.
        /// </summary>
        void ReturnToPool();

        /// <summary>
        ///     Set the <see cref="IManagedPool" /> which this item believes it belongs too.
        /// </summary>
        /// <remarks>This is used during creation to assign the parent that is creating the item.</remarks>
        /// <param name="targetManagedPool">The parent <see cref="IManagedPool" />.</param>
        void SetParentPool(IManagedPool targetManagedPool);
    }
}
#endif // !UNITY_DOTSRUNTIME