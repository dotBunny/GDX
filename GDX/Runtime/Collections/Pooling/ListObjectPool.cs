// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace GDX.Collections.Pooling
{
    /// <summary>
    /// </summary>
    public class ListObjectPool : IObjectPool
    {
        protected bool _allowCreateMore = true;
        protected bool _allowManagedTearDown = true;
        protected bool _allowReuse = false;
        protected object _baseObject;
        protected int _inCount;
        protected List<object> _inItems;
        protected int _maximumObjects = 20;
        protected int _minimumObjects = 5;
        protected int _outCount;
        protected List<object> _outItems;
        protected int _uniqueID;

        public Action OnCreateItem;
        public Action<bool> OnPoolAllItems;
        public Action OnTearDownPostPoolItems;
        public Action OnTearDownPrePoolItems;

        /// <inheritdoc />
        public void CreateItem()
        {
            OnCreateItem?.Invoke();
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
            if (_inCount == 0 && _outCount < _maximumObjects || _inCount == 0 && _allowCreateMore)
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
                        $"[ListObjectPool->Get] A null object was pulled from a pool ({_uniqueID}).");
                    _inCount--;
                    return null;
                }

                // Handle counters
                _outItems.Add(_inItems[targetIndex]);
                _outCount++;
                _inItems.RemoveAt(targetIndex);
                _inCount--;

                // Return Item
                if (triggerOnSpawnedFromPool && returnItem is IObjectPoolItem objectPoolItem)
                {
                    objectPoolItem.OnSpawnedFromPool();
                }

                return returnItem;
            }

            if (_allowReuse)
            {
                object returnItem = _outItems[0];
                if (returnItem == null)
                {
                    Trace.Output(Trace.TraceLevel.Warning,
                        $"[ListObjectPool->Get] A null object was returned to the object pool ({_uniqueID}).");
                    return null;
                }

                // Call events on item
                if (returnItem is IObjectPoolItem objectPoolItem)
                {
                    objectPoolItem.OnReturnedToPool();
                    if (triggerOnSpawnedFromPool)
                    {
                        objectPoolItem.OnSpawnedFromPool();
                    }
                }

                return returnItem;
            }

            Trace.Output(Trace.TraceLevel.Warning,
                $"[ListObjectPool->Get] Hit maximum object cap of {_maximumObjects.ToString()} for object pool ({_uniqueID}).");
            return null;
        }

        /// <inheritdoc />
        public T Get<T>(bool triggerOnSpawnedFromPool = true)
        {
            return (T)Get(triggerOnSpawnedFromPool);
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
            return _allowManagedTearDown;
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
            if (item is IObjectPoolItem objectPoolItem)
            {
                objectPoolItem.OnReturnedToPool();
            }

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
            OnPoolAllItems?.Invoke(shouldShrink);
        }

        /// <inheritdoc />
        public void TearDown()
        {
            OnTearDownPrePoolItems?.Invoke();

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
            OnTearDownPostPoolItems?.Invoke();

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