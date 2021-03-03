// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GDX.Collections.Pooling
{
    /// <summary>
    ///     An interface describing the functionality needed for an item to be understood by a potential
    ///     <see cref="IObjectPool" />, if callbacks are to be made.
    /// </summary>
    public interface IObjectPoolItem
    {
        /// <summary>
        ///     Return the <see cref="IObjectPool" /> which the <see cref="IObjectPoolItem" /> is currently managed by.
        /// </summary>
        /// <remarks>It is possible that a pooled item may exist without a parent in some custom scenario.</remarks>
        /// <returns>An <see cref="IObjectPool" /> if the <see cref="IObjectPoolItem" /> is attached to a pool, otherwise null.</returns>
        IObjectPool GetParentPool();

        /// <summary>
        ///     Is the <see cref="IObjectPoolItem"/>
        /// </summary>
        /// <returns></returns>
        bool IsValidItem();

        void OnReturnedToPool();

        void OnSpawnedFromPool();

        void ReturnToPool();

        void SetParentPool(IObjectPool targetObjectPool);
    }
}