namespace GDX.Collections.Pooling
{
    /// <summary>
    ///     An interface describing the functionality needed for a pool to be understood by <see cref="ObjectPools" />.
    /// </summary>
    public interface IObjectPool
    {
        /// <summary>
        ///     Create a pooled item and add it to the <see cref="IObjectPool" />.
        /// </summary>
        public void CreateItem();

        /// <summary>
        ///     Remove an <paramref name="item" /> from an <see cref="IObjectPool" /> immediately, removing it from tracking.
        /// </summary>
        /// <param name="item">The target <see cref="object" /> to attempt to remove.</param>
        public void ForceRemove(object item);

        /// <summary>
        ///     Get the next available item from an <see cref="IObjectPool" />.
        /// </summary>
        /// <returns>An item if available, otherwise null.</returns>
        public object Get(bool triggerOnSpawnedFromPool = true);

        /// <summary>
        ///     Get the next available item from an <see cref="IObjectPool" />, and cast it to the indicated type.
        /// </summary>
        /// <typeparam name="T">The type to cast the <see cref="object" /> return to.</typeparam>
        /// <returns>An item if available, otherwise null.</returns>
        public T Get<T>(bool triggerOnSpawnedFromPool = true);

        /// <summary>
        ///     Return the <see cref="object"/> which the pool is built from.
        /// </summary>
        /// <returns>The <see cref="object"/> the <see cref="IObjectPool"/> is modelled around.</returns>
        public object GetBaseObject();

        /// <summary>
        ///     Get the unique identifier for the <see cref="IObjectPool" />.
        /// </summary>
        /// <remarks>This is used as the hash key for the <see cref="ObjectPools" />'s entry for this <see cref="IObjectPool" />.</remarks>
        /// <returns>A unique identifier.</returns>
        public int GetUniqueID();

        /// <summary>
        ///     Does the <see cref="IObjectPool" /> contain the minimum number of items.
        /// </summary>
        /// <returns>true/false if enough items are found contained in the <see cref="IObjectPool" />.</returns>
        public bool HasMinimumPooledItems();

        /// <summary>
        ///     Should the<see cref="IObjectPool" /> destroy itself when <see cref="ObjectPools" /> requests a tear down?
        /// </summary>
        /// <returns>true/false if flagged to allow for managed tear down / clean up.</returns>
        public bool IsAllowedManagedTearDown();

        /// <summary>
        ///     Is the <paramref name="item" /> managed by this <see cref="IObjectPool" />?
        /// </summary>
        /// <param name="item">The <see cref="object" /> to evaluate.</param>
        /// <returns>true/false if the <paramref name="item" /> is managed by the <see cref="IObjectPool" />.</returns>
        public bool IsManaged(object item);

        /// <summary>
        ///     Is the provided <paramref name="item" /> found in the <see cref="IObjectPool" />'s internal available pool of
        ///     objects?
        /// </summary>
        /// <param name="item">The <paramref name="item" /> to evaluate.</param>
        public bool IsPooled(object item);

        /// <summary>
        ///     Return the <paramref name="item" /> to the <see cref="IObjectPool" />'s internal collection.
        /// </summary>
        /// <param name="item">The <paramref name="item" /> to return to the <see cref="IObjectPool" />.</param>
        public void Pool(object item);

        /// <summary>
        ///     Return all spawned <see cref="object" />s to the <see cref="IObjectPool" />.
        /// </summary>
        /// <param name="shouldShrink">Should the <see cref="IObjectPool" /> be shrunk to its original minimum size?</param>
        public void PoolAllItems(bool shouldShrink = true);

        /// <summary>
        ///     Destroy all <see cref="object" /> associated with the <see cref="IObjectPool" />.
        /// </summary>
        public void TearDown();
    }
}