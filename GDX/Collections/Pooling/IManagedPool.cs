// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Collections.Pooling
{
    /// <summary>
    ///     An interface describing the functionality needed for a pool to be understood by <see cref="ManagedPools" />.
    /// </summary>
    public interface IManagedPool
    {
        /// <summary>
        ///     Create a pooled item and add it to the <see cref="IManagedPool" />.
        /// </summary>
        void CreateItem();

        /// <summary>
        ///     Remove an <paramref name="item" /> from an <see cref="IManagedPool" /> immediately, removing it from tracking and
        ///     not calling any actions on it.
        /// </summary>
        /// <param name="item">The target <see cref="object" /> to attempt to remove.</param>
        void ForceRemove(object item);

        /// <summary>
        ///     Get the next available item from an <see cref="IManagedPool" />.
        /// </summary>
        /// <returns>An item if available, otherwise null.</returns>
        object Get(bool triggerOnSpawnedFromPool = true);

        /// <summary>
        ///     Return the <see cref="object" /> which the pool is built from.
        /// </summary>
        /// <returns>The <see cref="object" /> the <see cref="IManagedPool" /> is modelled around.</returns>
        object GetBaseObject();

        /// <summary>
        ///     Get the unique identifier for the <see cref="IManagedPool" />.
        /// </summary>
        /// <returns>A unique identifier.</returns>
        uint GetKey();

        /// <summary>
        ///     Does the <see cref="IManagedPool" /> contain the minimum number of items.
        /// </summary>
        /// <returns>true/false if enough items are found contained in the <see cref="IManagedPool" />.</returns>
        bool HasMinimumPooledItems();

        /// <summary>
        ///     Should the<see cref="IManagedPool" /> destroy itself when <see cref="ManagedPools" /> requests a tear down?
        /// </summary>
        /// <returns>true/false if flagged to allow for managed tear down / clean up.</returns>
        bool IsAllowedManagedTearDown();

        /// <summary>
        ///     Is the <paramref name="item" /> managed by this <see cref="IManagedPool" />?
        /// </summary>
        /// <param name="item">The <see cref="object" /> to evaluate.</param>
        /// <returns>true/false if the <paramref name="item" /> is managed by the <see cref="IManagedPool" />.</returns>
        bool IsManaged(object item);

        /// <summary>
        ///     Is the provided <paramref name="item" /> found in the <see cref="IManagedPool" />'s internal available pool of
        ///     objects?
        /// </summary>
        /// <param name="item">The <paramref name="item" /> to evaluate.</param>
        bool IsPooled(object item);

        /// <summary>
        ///     Return the <paramref name="item" /> to the <see cref="IManagedPool" />'s internal collection.
        /// </summary>
        /// <param name="item">The <paramref name="item" /> to return to the <see cref="IManagedPool" />.</param>
        void Return(object item);

        /// <summary>
        ///     Return all spawned <see cref="object" />s to the <see cref="IManagedPool" />.
        /// </summary>
        /// <remarks>Shrinking the pools helps with pools that have grown out of necessity past their maximum size</remarks>
        /// <param name="shouldShrink">Should the <see cref="IManagedPool" /> be shrunk to its original maximum size?</param>
        void ReturnAll(bool shouldShrink = true);

        /// <summary>
        ///     Destroy all <see cref="object" />s associated with the <see cref="IManagedPool" />.
        /// </summary>
        void TearDown();
    }
}