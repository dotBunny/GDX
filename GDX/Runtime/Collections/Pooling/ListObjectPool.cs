// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace GDX.Collections.Pooling
{

    /// <summary>
    /// A <see cref="object"/> <see cref="List{T}"/> backed pool implementation.
    /// </summary>
    public sealed class ListObjectPool : IObjectPool
    {
        private const int AllowCreateMoreFlag = 0;
        private const int AllowManagedTeardownFlag = 1;
        private const int AllowReuseFlag = 2;
        private const int PrewarmPoolFlag = 3;

        public BitArray8 Flags;

        internal readonly object _baseObject;
        internal readonly object _containerObject;

        internal int _inCount;
        internal readonly List<object> _inItems;
        internal readonly int _maximumObjects;
        internal readonly int _minimumObjects;
        internal int _outCount;
        internal readonly List<object> _outItems;
        internal readonly int _uniqueID;

        public Func<object> CreateItemFunc;

        public Action<ListObjectPool> OnCreateItem;

        public Action<ListObjectPool> OnTearDownPostPoolItems;
        public Action<ListObjectPool> OnTearDownPrePoolItems;

        public Action<ListObjectPool, object> OnSpawnedFromPool;
        public Action<ListObjectPool, object> OnReturnedToPool;
        public Action<object> OnDestroyItem;

        /// <summary>
        ///     Create a <see cref="ListObjectPool"/>.
        /// </summary>
        /// <param name="uniqueID">An absolutely unique identifier for this pool.</param>
        /// <param name="baseObject">The object which going to be cloned.</param>
        /// <param name="minimumObjects">The minimum number of objects to be pooled.</param>
        /// <param name="maximumObjects">The maximum number of objects to be pooled.</param>
        /// <param name="containerObject">A reference to an object which should be used as the container for created items.</param>
        /// <param name="prewarmPool">Should this pool create its items during the constructor?</param>
        /// <param name="allowCreateMore">Can more items be created as needed when starved for items?</param>
        /// <param name="allowReuseWhenCapped">Should we reuse oldest items when starving for items?</param>
        /// <param name="allowManagedTearDown">Does the pool allow a managed tear down event call?</param>
        public ListObjectPool(
            int uniqueID,
            object baseObject,
            int minimumObjects = 10,
            int maximumObjects = 50,
            object containerObject = null,
            bool prewarmPool = true,
            bool allowCreateMore = true,
            bool allowReuseWhenCapped = false,
            bool allowManagedTearDown = false)
        {
            _uniqueID = uniqueID;
            _baseObject = baseObject;
            _minimumObjects = minimumObjects;
            _maximumObjects = maximumObjects;

            Flags[AllowCreateMoreFlag] = allowCreateMore;
            Flags[AllowManagedTeardownFlag] = allowManagedTearDown;
            Flags[AllowReuseFlag] = allowReuseWhenCapped;
            Flags[PrewarmPoolFlag] = prewarmPool;

            _containerObject = containerObject;

            ObjectPools.Register(this);

            _inItems = new List<object>(maximumObjects);
            _outItems = new List<object>(maximumObjects);

            if (!prewarmPool)
            {
                return;
            }

            for (int i = 0; i < minimumObjects; i++)
            {
                this.CreateItem();
            }
        }

        /// <inheritdoc />
        public void CreateItem()
        {
            OnCreateItem?.Invoke(this);
        }

        /// <inheritdoc />
        public void ForceRemove(object item)
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

        /// <inheritdoc />
        public object Get(bool triggerOnSpawnedFromPool = true)
        {
            // Are we empty, but have refills?
            if (_inCount == 0 && _outCount < _maximumObjects || _inCount == 0 && Flags[AllowCreateMoreFlag])
            {
                CreateItem();
            }

            if (_inCount > 0)
            {
                int targetIndex = _inCount - 1;

                // Make sure we don't pull badness
                object returnItem = _inItems[targetIndex];
                if (returnItem == null)
                {
                    Trace.Output(Trace.TraceLevel.Warning,
                        $"[ListObjectPool->Get] A null object was pulled from a pool ({_uniqueID.ToString()}).");
                    _inCount--;
                    return null;
                }

                // Handle counters
                _outItems.Add(returnItem);
                _outCount++;
                _inItems.RemoveAt(targetIndex);
                _inCount--;

                if (triggerOnSpawnedFromPool)
                {
                    OnSpawnedFromPool?.Invoke(this, returnItem);
                }
                return returnItem;
            }

            if (Flags[AllowReuseFlag])
            {
                object returnItem = _outItems[0];
                if (returnItem == null)
                {
                    Trace.Output(Trace.TraceLevel.Warning,
                        $"[ListObjectPool->Get] A null object was returned to the object pool ({_uniqueID.ToString()}).");
                    return null;
                }

                OnReturnedToPool?.Invoke(this, returnItem);
                if (triggerOnSpawnedFromPool)
                {
                    OnSpawnedFromPool?.Invoke(this, returnItem);
                }

                return returnItem;
            }

            Trace.Output(Trace.TraceLevel.Warning,
                $"[ListObjectPool->Get] Hit maximum object cap of {_maximumObjects.ToString()} for object pool ({_uniqueID.ToString()}).");
            return null;
        }

        /// <inheritdoc />
        public object GetBaseObject()
        {
            return _baseObject;
        }

        /// <inheritdoc />
        public int GetUniqueID()
        {
            return _uniqueID;
        }

        /// <inheritdoc />
        public bool HasMinimumPooledItems()
        {
            return _inCount >= _minimumObjects;
        }

        /// <inheritdoc />
        public bool IsAllowedManagedTearDown()
        {
            return Flags[AllowManagedTeardownFlag];
        }

        /// <inheritdoc />
        public bool IsManaged(object item)
        {
            return _outItems.Contains(item) || _inItems.Contains(item);
        }

        /// <inheritdoc />
        public bool IsPooled(object item)
        {
            return _inItems.Contains(item);
        }

        /// <inheritdoc />
        public void Pool(object item)
        {
            // Do we have the interface call?
            OnReturnedToPool?.Invoke(this, item);

            if (_outItems.Contains(item))
            {
                _outItems.Remove(item);
                _outCount--;
            }

            if (_inItems.Contains(item))
            {
                return;
            }

            _inItems.Add(item);
            _inCount++;
        }

        /// <inheritdoc />
        public void PoolAllItems(bool shouldShrink = true)
        {
            for (int i = _outCount - 1; i >= 0; i--)
            {
                Pool(_outItems[i]);
            }

            if (shouldShrink && _inCount > _maximumObjects)
            {
                return;
            }

            int removeCount = _inCount - _maximumObjects;
            for (int i = 0; i < removeCount; i++)
            {
                OnDestroyItem?.Invoke(_inItems[i]);
                _inItems.RemoveAt(i);
                _inCount--;
            }
        }

        /// <inheritdoc />
        public void TearDown()
        {
            OnTearDownPrePoolItems?.Invoke(this);

            // Return all items to the pool
            for (int i = _outCount - 1; i >= 0; i--)
            {
                if (_outItems[i] != null)
                {
                    Pool(_outItems[i]);
                }
            }

            _outItems.Clear();
            _outCount = 0;

            // Execute specific logic to the implementation
            OnTearDownPostPoolItems?.Invoke(this);

            // Removing references to the objects should make GC do its thing
            _inItems.Clear();
            _inCount = 0;
        }

        ~ListObjectPool()
        {
            // Unregister
            ObjectPools.Unregister(this);
        }
    }
}