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
        public void CreateItem();

        /// <summary>
        ///     Remove an <paramref name="item" /> from an <see cref="IManagedPool" /> immediately, removing it from tracking and
        ///     not calling any actions on it.
        /// </summary>
        /// <param name="item">The target <see cref="object" /> to attempt to remove.</param>
        public void ForceRemove(object item);

        /// <summary>
        ///     Get the next available item from an <see cref="IManagedPool" />.
        /// </summary>
        /// <returns>An item if available, otherwise null.</returns>
        public object Get(bool triggerOnSpawnedFromPool = true);

        /// <summary>
        ///     Return the <see cref="object" /> which the pool is built from.
        /// </summary>
        /// <returns>The <see cref="object" /> the <see cref="IManagedPool" /> is modelled around.</returns>
        public object GetBaseObject();

        /// <summary>
        ///     Get the unique identifier for the <see cref="IManagedPool" />.
        /// </summary>
        /// <remarks>This is used as the hash key for the <see cref="ManagedPools" />'s entry for this <see cref="IManagedPool" />.</remarks>
        /// <returns>A unique identifier.</returns>
        public int GetKey();

        /// <summary>
        ///     Does the <see cref="IManagedPool" /> contain the minimum number of items.
        /// </summary>
        /// <returns>true/false if enough items are found contained in the <see cref="IManagedPool" />.</returns>
        public bool HasMinimumPooledItems();

        /// <summary>
        ///     Should the<see cref="IManagedPool" /> destroy itself when <see cref="ManagedPools" /> requests a tear down?
        /// </summary>
        /// <returns>true/false if flagged to allow for managed tear down / clean up.</returns>
        public bool IsAllowedManagedTearDown();

        /// <summary>
        ///     Is the <paramref name="item" /> managed by this <see cref="IManagedPool" />?
        /// </summary>
        /// <param name="item">The <see cref="object" /> to evaluate.</param>
        /// <returns>true/false if the <paramref name="item" /> is managed by the <see cref="IManagedPool" />.</returns>
        public bool IsManaged(object item);

        /// <summary>
        ///     Is the provided <paramref name="item" /> found in the <see cref="IManagedPool" />'s internal available pool of
        ///     objects?
        /// </summary>
        /// <param name="item">The <paramref name="item" /> to evaluate.</param>
        public bool IsPooled(object item);

        /// <summary>
        ///     Return the <paramref name="item" /> to the <see cref="IManagedPool" />'s internal collection.
        /// </summary>
        /// <param name="item">The <paramref name="item" /> to return to the <see cref="IManagedPool" />.</param>
        public void Return(object item);

        /// <summary>
        ///     Return all spawned <see cref="object" />s to the <see cref="IManagedPool" />.
        /// </summary>
        /// <remarks>Shrinking the pools helps with pools that have grown out of necessity past their maximum size</remarks>
        /// <param name="shouldShrink">Should the <see cref="IManagedPool" /> be shrunk to its original maximum size?</param>
        public void ReturnAll(bool shouldShrink = true);

        /// <summary>
        ///     Destroy all <see cref="object" />s associated with the <see cref="IManagedPool" />.
        /// </summary>
        public void TearDown();
    }
}