using System.Collections.Generic;
using UnityEngine;

namespace GDX.Collections.Pooling
{
    public interface IPool
    {
        /// <summary>
        /// Should the<see cref="IPool" /> destroy itself when the <see cref="PoolingSystem"/> requests a tear down?
        /// </summary>
        bool ShouldTearDown { get; }

        /// <summary>
        /// Create a pooled item and add it to the <see cref="IPool"/>.
        /// </summary>
        void CreateItem();

        /// <summary>
        /// Get the unique identifier for this <see cref="IPool"/>.
        /// </summary>
        int GetUniqueID();

        /// <summary>
        /// Return all spawned items to the <see cref="IPool"/>.
        /// </summary>
        /// <param name="shouldShrink">Should the <see cref="IPool"/> be shrunk to its original minimum size?</param>
        void PoolAll(bool shouldShrink = true);

        /// <summary>
        /// Destroy all pooled items.
        /// </summary>
        void TearDown();

        /// <summary>
        /// Does the <see cref="IPool"/> contain the minimum number of items.
        /// </summary>
        /// <returns>true/false</returns>
        bool HasMinimumPooledItems();
    }
}