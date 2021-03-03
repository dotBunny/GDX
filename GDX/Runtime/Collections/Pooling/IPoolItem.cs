// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GDX.Collections.Pooling
{
    /// <summary>
    ///     An interface describing the functionality needed for an item to be understood by a potential <see cref="IPool" />,
    ///     if callbacks are to be made..
    /// </summary>
    public interface IPoolItem<T>
    {
        T GetSelf();

        /// <summary>
        ///     Return the <see cref="IPool"/> which the <see cref="IPoolItem"/> is currently attached too.
        /// </summary>
        /// <remarks>It is possible that a pooled item may exist without a parent in some custom scenarios.</remarks>
        /// <returns>An <see cref="IPool"/> if the <see cref="IPoolItem{T}"/> is attached to a pool, otherwise null.</returns>
        IPool GetParentPool();

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        bool IsValidItem();
        void OnReturnedToPool();
        void OnSpawnedFromPool();
        void ReturnToPool();

        void SetParentPool(IPool targetPool);
    }
}