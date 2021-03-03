// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace GDX.Collections.Pooling
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T0">Type of stored object</typeparam>
    /// <typeparam name="T1">Type to return when spawned</typeparam>
    public abstract class ListPoolBase<T0, T1> : IPoolBase<T1>, IPool where T0 : notnull
    {
        protected bool _allowCreateMore = true;
        protected bool _allowManagedTearDown = true;
        protected bool _allowReuse = false;

        protected int _inCount;
        protected List<T1> _inItems;
        protected int _outCount;
        protected List<T1> _outItems;

        protected int _maximumObjects = 20;
        protected int _minimumObjects = 5;

        protected int _uniqueID;

        /// <inheritdoc />
        public int GetUniqueID()
        {
            return _uniqueID;
        }

        /// <inheritdoc />
        public bool IsAllowedManagedTearDown()
        {
            return _allowManagedTearDown;
        }

        /// <inheritdoc />
        public abstract void TearDown();

        /// <inheritdoc />
        public abstract void PoolAllItems(bool shouldShrink = true);

        /// <inheritdoc />
        public abstract void CreateItem();

        /// <inheritdoc />
        public bool HasMinimumPooledItems()
        {
            return _inCount >= _minimumObjects;
        }

        ~ListPoolBase()
        {
            // Unregister
            Pools.Unregister(this);
        }

        public abstract T0 Get();


        /// <inheritdoc />
        public bool IsPooled(T1 item)
        {
            return _inItems.Contains(item);
        }

        /// <inheritdoc />
        public bool IsManaged(T1 item)
        {
            return _outItems.Contains(item) || _inItems.Contains(item);
        }

        /// <inheritdoc />
        public void ForceRemove(T1 item)
        {
            if (_outItems.Contains(item))
            {
                _outItems.Remove(item);
                _outCount--;
            }

            if (!_inItems.Contains(item))
            {
                return;
            }

            _inItems.Remove(item);
            _inCount--;
        }
    }
}