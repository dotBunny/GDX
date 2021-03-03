namespace GDX.Collections.Pooling
{
    /// <summary>
    ///     An interface describing the functionality needed for a pool to be understood in the <see cref="Pools" />.
    /// </summary>
    public interface IPool
    {
        /// <summary>
        ///     Create a pooled item and add it to the <see cref="IPool" />.
        /// </summary>
        void CreateItem();

        /// <summary>
        ///     Get the unique identifier for the <see cref="IPool" />.
        /// </summary>
        /// <remarks>This is used as the hash key for the <see cref="Pools" />'s entry for this <see cref="IPool" />.</remarks>
        /// <returns>A unique identifier.</returns>
        int GetUniqueID();

        /// <summary>
        ///     Does the <see cref="IPool" /> contain the minimum number of items.
        /// </summary>
        /// <returns>true/false</returns>
        bool HasMinimumPooledItems();

        /// <summary>
        ///     Should the<see cref="IPool" /> destroy itself when the <see cref="Pools" /> requests a tear down?
        /// </summary>
        bool IsAllowedManagedTearDown();

        /// <summary>
        ///     Return all spawned items to the <see cref="IPool" />.
        /// </summary>
        /// <param name="shouldShrink">Should the <see cref="IPool" /> be shrunk to its original minimum size?</param>
        void PoolAllItems(bool shouldShrink = true);

        /// <summary>
        ///     Destroy all items associated with the <see cref="IPool" />.
        /// </summary>
        void TearDown();
    }
}