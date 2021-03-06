// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace GDX.Collections.Pooling
{
    /// <summary>
    ///     A <see cref="object" /> <see cref="List{T}" /> backed pool implementation.
    /// </summary>
    public sealed class ListManagedPool : IManagedPool
    {
        /// <summary>
        ///     The <see cref="Flags" /> index used to determine if the <see cref="ListManagedPool" /> is able to create
        ///     more items as necessary.
        /// </summary>
        private const int AllowCreateMoreFlag = 0;

        /// <summary>
        ///     The <see cref="Flags" /> index used to if <see cref="TearDown" /> can be called by a manager.
        /// </summary>
        private const int AllowManagedTeardownFlag = 1;

        /// <summary>
        ///     The <see cref="Flags" /> index used to determine if items should be reused when the pool is starved.
        /// </summary>
        private const int AllowReuseFlag = 2;

        /// <summary>
        ///     The <see cref="Flags" /> index used to determine if the pool should create items during its constructor.
        /// </summary>
        private const int PrewarmPoolFlag = 3;

        /// <summary>
        ///     The object which the pool is based off of, used as a model when creating new items.
        /// </summary>
        internal readonly object _baseObject;

        /// <summary>
        ///     The object which serves as a container for all objects of the pool.
        /// </summary>
        /// <remarks>Used more by implementations of pools, then this base class.</remarks>
        internal readonly object _containerObject;

        /// <summary>
        ///     A defined function to create items for the pool.
        /// </summary>
        private readonly Func<ListManagedPool, object> _createItemFunc;

        /// <summary>
        ///     A collection of items that are currently contained in the pool for use when spawning items upon request.
        /// </summary>
        internal readonly List<object> _inItems;

        /// <summary>
        ///     The absolutely unique identifier for this pool.
        /// </summary>
        private readonly uint _key;

        /// <summary>
        ///     The Maximum number of objects to be managed by the pool.
        /// </summary>
        private readonly int _maximumObjects;

        /// <summary>
        ///     The minimum number of objects to be managed by the pool.
        /// </summary>
        private readonly int _minimumObjects;

        /// <summary>
        ///     A collection of items that are currently considered out of the pool, that have been spawned.
        /// </summary>
        private readonly List<object> _outItems;

        /// <summary>
        ///     A cached count of the number of items contained in <see cref="_inItems" />.
        /// </summary>
        internal int _inCount;

        /// <summary>
        ///     A cached count of the number of items contained in <see cref="_outItems" />.
        /// </summary>
        internal int _outCount;

        /// <summary>
        ///     A <see cref="BitArray8" /> used to store pool based flags, as well as provide additional spots for implementations.
        /// </summary>
        /// <remarks>
        ///     Index 0-3 (<see cref="AllowCreateMoreFlag" />, <see cref="AllowManagedTeardownFlag" />,
        ///     <see cref="AllowReuseFlag" />, and <see cref="PrewarmPoolFlag" />) are used by the
        ///     <see cref="ListManagedPool" /> itself, leaving 4-7 for additional use.
        /// </remarks>
        public BitArray8 Flags;

        /// <summary>
        ///     A <c>delegate</c> call made when an item is destroyed by the <see cref="ListManagedPool" />.
        /// </summary>
        public Action<object> OnDestroyItem;

        /// <summary>
        ///     A <c>delegate</c> call made when an item is returned to the <see cref="ListManagedPool" />.
        /// </summary>
        public Action<ListManagedPool, object> OnReturnedToPool;

        /// <summary>
        ///     A <c>delegate</c> call made when an item is spawned from the <see cref="ListManagedPool" />.
        /// </summary>
        public Action<ListManagedPool, object> OnSpawnedFromPool;

        /// <summary>
        ///     A <c>delegate</c> call made when a pool is tearing down, after the items are returned to the pool.
        /// </summary>
        public Action<ListManagedPool> OnTearDownPostPoolItems;

        /// <summary>
        ///     A <c>delegate</c> call made when a pool is tearing down, before the items are pooled.
        /// </summary>
        public Action<ListManagedPool> OnTearDownPrePoolItems;

        /// <summary>
        ///     Create a <see cref="ListManagedPool" />.
        /// </summary>
        /// <param name="baseObject">The object which going to be cloned.</param>
        /// <param name="createItemFunc">The function used to create new items for the pool.</param>
        /// <param name="minimumObjects">The minimum number of objects to be managed by the pool.</param>
        /// <param name="maximumObjects">The maximum number of objects to be managed by the pool.</param>
        /// <param name="containerObject">A reference to an object which should be used as the container for created items.</param>
        /// <param name="prewarmPool">Should this pool create its items during the constructor?</param>
        /// <param name="allowCreateMore">Can more items be created as needed when starved for items?</param>
        /// <param name="allowReuseWhenCapped">Should we reuse oldest items when starving for items?</param>
        /// <param name="allowManagedTearDown">Does the pool allow a managed tear down event call?</param>
        public ListManagedPool(
            object baseObject,
            Func<ListManagedPool, object> createItemFunc,
            int minimumObjects = 10,
            int maximumObjects = 50,
            object containerObject = null,
            bool prewarmPool = true,
            bool allowCreateMore = true,
            bool allowReuseWhenCapped = false,
            bool allowManagedTearDown = false)
        {
            // Get pool ID ticket
            _key = ManagedPools.GetNextPoolKey();

            _baseObject = baseObject;
            _createItemFunc = createItemFunc;
            _minimumObjects = minimumObjects;
            _maximumObjects = maximumObjects;

            Flags[AllowCreateMoreFlag] = allowCreateMore;
            Flags[AllowManagedTeardownFlag] = allowManagedTearDown;
            Flags[AllowReuseFlag] = allowReuseWhenCapped;
            Flags[PrewarmPoolFlag] = prewarmPool;

            _containerObject = containerObject;

            ManagedPools.Register(this);

            _inItems = new List<object>(maximumObjects);
            _outItems = new List<object>(maximumObjects);

            if (!prewarmPool)
            {
                return;
            }

            for (int i = 0; i < minimumObjects; i++)
            {
                CreateItem();
            }
        }

        /// <inheritdoc />
        public void CreateItem()
        {
            _createItemFunc(this);
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
                        $"[ListObjectPool->Get] A null object was pulled from a pool ({_key.ToString()}).");
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
                        $"[ListObjectPool->Get] A null object was returned to the object pool ({_key.ToString()}).");
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
                $"[ListObjectPool->Get] Hit maximum object cap of {_maximumObjects.ToString()} for object pool ({_key.ToString()}).");
            return null;
        }

        /// <inheritdoc />
        public object GetBaseObject()
        {
            return _baseObject;
        }

        /// <inheritdoc />
        public uint GetKey()
        {
            return _key;
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
        public void Return(object item)
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
        public void ReturnAll(bool shouldShrink = true)
        {
            for (int i = _outCount - 1; i >= 0; i--)
            {
                Return(_outItems[i]);
            }

            if (shouldShrink && _inCount <= _maximumObjects)
            {
                return;
            }

            int removeCount = _inCount - _maximumObjects;
            for (int i = 0; i < removeCount; i++)
            {
                // Trigger specific logic, like Object.Destroy
                OnDestroyItem?.Invoke(_inItems[i]);

                // Dereferencing
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
                    Return(_outItems[i]);
                }
            }

            _outItems.Clear();
            _outCount = 0;

            // Execute specific logic to the implementation
            OnTearDownPostPoolItems?.Invoke(this);

            // Removing references to the objects should make GC do its thing
            _inItems.Clear();
            _inCount = 0;

            // Unregister
            ManagedPools.Unregister(this);
        }

        /// <summary>
        ///     The <see cref="ListManagedPool" /> destructor which unregisters itself from <see cref="ManagedPools" />.
        /// </summary>
        ~ListManagedPool()
        {
            // Unregister
            ManagedPools.Unregister(this);
        }
    }
}