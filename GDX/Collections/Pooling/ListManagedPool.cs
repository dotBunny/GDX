// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
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
        const int k_AllowCreateMoreFlag = 0;

        /// <summary>
        ///     The <see cref="Flags" /> index used to if <see cref="TearDown" /> can be called by a manager.
        /// </summary>
        const int k_AllowManagedTeardownFlag = 1;

        /// <summary>
        ///     The <see cref="Flags" /> index used to determine if items should be reused when the pool is starved.
        /// </summary>
        const int k_AllowReuseFlag = 2;

        /// <summary>
        ///     The <see cref="Flags" /> index used to determine if the pool should create items during its constructor.
        /// </summary>
        const int k_PrewarmPoolFlag = 3;

        /// <summary>
        ///     A defined function to create items for the pool.
        /// </summary>
        readonly Func<ListManagedPool, object> m_CreateItemFunc;

        /// <summary>
        ///     The absolutely unique identifier for this pool.
        /// </summary>
        readonly uint m_Key;

        /// <summary>
        ///     The Maximum number of objects to be managed by the pool.
        /// </summary>
        readonly int m_MaximumObjects;

        /// <summary>
        ///     The minimum number of objects to be managed by the pool.
        /// </summary>
        readonly int m_MinimumObjects;

        /// <summary>
        ///     A collection of items that are currently considered out of the pool, that have been spawned.
        /// </summary>
        readonly List<object> m_OutItems;

        /// <summary>
        ///     A cached count of the number of items contained in <see cref="InItems" />.
        /// </summary>
        public int InCachedCount;

        /// <summary>
        ///     A cached count of the number of items contained in <see cref="m_OutItems" />.
        /// </summary>
        public int OutCachedCount;

        /// <summary>
        ///     The object which the pool is based off of, used as a model when creating new items.
        /// </summary>
        public readonly object BaseObject;

        /// <summary>
        ///     The object which serves as a container for all objects of the pool.
        /// </summary>
        /// <remarks>Used more by implementations of pools, then this base class.</remarks>
        public readonly object ContainerObject;

        /// <summary>
        ///     A collection of items that are currently contained in the pool for use when spawning items upon request.
        /// </summary>
        public readonly List<object> InItems;

        /// <summary>
        ///     A <see cref="BitArray8" /> used to store pool based flags, as well as provide additional spots for implementations.
        /// </summary>
        /// <remarks>
        ///     Index 0-3 (<see cref="k_AllowCreateMoreFlag" />, <see cref="k_AllowManagedTeardownFlag" />,
        ///     <see cref="k_AllowReuseFlag" />, and <see cref="k_PrewarmPoolFlag" />) are used by the
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
        ///     A <c>delegate</c> call made when a pool is tearing down, before the items are pooled.
        /// </summary>
        public Action<ListManagedPool> OnTearDown;

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
            m_Key = ManagedPools.GetNextPoolKey();

            BaseObject = baseObject;
            m_CreateItemFunc = createItemFunc;
            m_MinimumObjects = minimumObjects;
            m_MaximumObjects = maximumObjects;

            Flags[k_AllowCreateMoreFlag] = allowCreateMore;
            Flags[k_AllowManagedTeardownFlag] = allowManagedTearDown;
            Flags[k_AllowReuseFlag] = allowReuseWhenCapped;
            Flags[k_PrewarmPoolFlag] = prewarmPool;

            ContainerObject = containerObject;

            ManagedPools.Register(this);

            InItems = new List<object>(maximumObjects);
            m_OutItems = new List<object>(maximumObjects);

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
            m_CreateItemFunc(this);
        }

        /// <inheritdoc />
        public void ForceRemove(object item)
        {
            if (m_OutItems.Contains(item))
            {
                m_OutItems.Remove(item);
                OutCachedCount--;
            }

            if (!InItems.Contains(item))
            {
                return;
            }

            InItems.Remove(item);
            InCachedCount--;
        }

        /// <inheritdoc />
        public object Get(bool triggerOnSpawnedFromPool = true)
        {
            // Are we empty, but have refills?
            if (InCachedCount == 0 && OutCachedCount < m_MaximumObjects || InCachedCount == 0 && Flags[k_AllowCreateMoreFlag])
            {
                CreateItem();
            }

            if (InCachedCount > 0)
            {
                int targetIndex = InCachedCount - 1;

                // Make sure we don't pull badness
                object returnItem = InItems[targetIndex];
                if (returnItem == null)
                {
                    Trace.Output(Trace.TraceLevel.Warning,
                        $"[ListObjectPool->Get] A null object was pulled from a pool ({m_Key.ToString()}).");
                    InCachedCount--;
                    return null;
                }

                // Handle counters
                m_OutItems.Add(returnItem);
                OutCachedCount++;
                InItems.RemoveAt(targetIndex);
                InCachedCount--;

                if (triggerOnSpawnedFromPool)
                {
                    OnSpawnedFromPool?.Invoke(this, returnItem);
                }

                return returnItem;
            }

            if (Flags[k_AllowReuseFlag])
            {
                object returnItem = m_OutItems[0];
                if (returnItem == null)
                {
                    Trace.Output(Trace.TraceLevel.Warning,
                        $"[ListObjectPool->Get] A null object was returned to the object pool ({m_Key.ToString()}).");
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
                $"[ListObjectPool->Get] Hit maximum object cap of {m_MaximumObjects.ToString()} for object pool ({m_Key.ToString()}).");
            return null;
        }

        /// <inheritdoc />
        public object GetBaseObject()
        {
            return BaseObject;
        }

        /// <inheritdoc />
        public uint GetKey()
        {
            return m_Key;
        }

        /// <inheritdoc />
        public bool HasMinimumPooledItems()
        {
            return InCachedCount >= m_MinimumObjects;
        }

        /// <inheritdoc />
        public bool IsAllowedManagedTearDown()
        {
            return Flags[k_AllowManagedTeardownFlag];
        }

        /// <inheritdoc />
        public bool IsManaged(object item)
        {
            return m_OutItems.Contains(item) || InItems.Contains(item);
        }

        /// <inheritdoc />
        public bool IsPooled(object item)
        {
            return InItems.Contains(item);
        }

        /// <inheritdoc />
        public void Return(object item)
        {
            // Do we have the interface call?
            OnReturnedToPool?.Invoke(this, item);

            if (m_OutItems.Contains(item))
            {
                m_OutItems.Remove(item);
                OutCachedCount--;
            }

            if (InItems.Contains(item))
            {
                return;
            }

            InItems.Add(item);
            InCachedCount++;
        }

        /// <inheritdoc />
        public void ReturnAll(bool shouldShrink = true)
        {
            for (int i = OutCachedCount - 1; i >= 0; i--)
            {
                Return(m_OutItems[i]);
            }

            if (shouldShrink && InCachedCount <= m_MaximumObjects)
            {
                return;
            }

            int removeCount = InCachedCount - m_MaximumObjects;
            for (int i = 0; i < removeCount; i++)
            {
                // Trigger specific logic, like Object.Destroy
                OnDestroyItem?.Invoke(InItems[i]);

                // Dereferencing
                InItems.RemoveAt(i);
                InCachedCount--;
            }
        }

        /// <inheritdoc />
        public void TearDown()
        {
            OnTearDown?.Invoke(this);

            // Return all items to the pool
            for (int i = OutCachedCount - 1; i >= 0; i--)
            {
                if (m_OutItems[i] != null)
                {
                    Return(m_OutItems[i]);

                }
            }

            m_OutItems.Clear();
            OutCachedCount = 0;

            // Wipe internals
            for (int i = InCachedCount - 1; i >= 0; i--)
            {
                if (InItems[i] != null)
                {
                    OnDestroyItem?.Invoke(InItems[i]);
                }
            }
            InItems.Clear();
            InCachedCount = 0;

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