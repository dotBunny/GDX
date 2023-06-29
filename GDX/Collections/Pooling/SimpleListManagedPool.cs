// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using GDX.Collections.Generic;
using GDX.Experimental;
using GDX.Experimental.Logging;
using UnityEngine;

namespace GDX.Collections.Pooling
{
    /// <summary>
    ///     A <see cref="object" /> <see cref="SimpleList{T}" /> backed pool implementation.
    /// </summary>
    public sealed class SimpleListManagedPool : IManagedPool
    {
        /// <summary>
        ///     The <see cref="Flags" /> index used to determine if the <see cref="SimpleListManagedPool" /> is able to create
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
        readonly Func<SimpleListManagedPool, object> m_CreateItemFunc;

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
        SimpleList<object> m_OutItems;

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
        ///     An <c>event</c> invoked when an item is destroyed by the <see cref="SimpleListManagedPool" />.
        /// </summary>
        public Action<object> destroyedItem;

        /// <summary>
        ///     A collection of items that are currently contained in the pool for use when spawning items upon request.
        /// </summary>
        public SimpleList<object> InItems;

        /// <summary>
        ///     A <see cref="BitArray8" /> used to store pool based flags, as well as provide additional spots for implementations.
        /// </summary>
        /// <remarks>
        ///     Index 0-3 (<see cref="k_AllowCreateMoreFlag" />, <see cref="k_AllowManagedTeardownFlag" />,
        ///     <see cref="k_AllowReuseFlag" />, and <see cref="k_PrewarmPoolFlag" />) are used by the
        ///     <see cref="SimpleListManagedPool" /> itself, leaving 4-7 for additional use.
        /// </remarks>
        public BitArray8 Flags;

        /// <summary>
        ///     An <c>event</c> invoked when an item is returned to the <see cref="SimpleListManagedPool" />.
        /// </summary>
        public Action<SimpleListManagedPool, object> returnedItem;

        /// <summary>
        ///     An <c>event</c> invoked when an item is spawned from the <see cref="SimpleListManagedPool" />.
        /// </summary>
        public Action<SimpleListManagedPool, object> spawnedItem;

        /// <summary>
        ///     An <c>event</c> invoked when a pool is tearing down, before the items are pooled.
        /// </summary>
        public Action<SimpleListManagedPool> tearingDown;

        /// <summary>
        ///     Create a <see cref="SimpleListManagedPool" />.
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
        public SimpleListManagedPool(
            object baseObject,
            Func<SimpleListManagedPool, object> createItemFunc,
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

            InItems = new SimpleList<object>(maximumObjects);
            m_OutItems = new SimpleList<object>(maximumObjects);

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
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation, Unity.ExpensiveCode
            m_CreateItemFunc(this);
        }

        /// <inheritdoc />
        public void ForceRemove(object item)
        {
            int outCount = m_OutItems.Count;
            for (int i = 0; i < outCount; i++)
            {
                if (m_OutItems.Array[i] != item)
                {
                    continue;
                }
                m_OutItems.RemoveAtSwapBack(i);
                OutCachedCount--;
                break;
            }

            int inCount = InItems.Count;
            for (int i = 0; i < inCount; i++)
            {
                if (InItems.Array[i] != item)
                {
                    continue;
                }

                InItems.RemoveAtSwapBack(i);
                InCachedCount--;
                break;
            }
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
                object returnItem = InItems.Array[targetIndex];
                if (returnItem == null)
                {
                    ManagedLog.Warning(LogCategory.GDX, $"[ListObjectPool->Get] A null object was pulled from a pool ({m_Key.ToString()}).");
                    InCachedCount--;
                    return null;
                }

                // Handle counters
                m_OutItems.AddUnchecked(returnItem);
                OutCachedCount++;
                InItems.RemoveAt(targetIndex);
                InCachedCount--;

                if (triggerOnSpawnedFromPool)
                {
                    spawnedItem?.Invoke(this, returnItem);
                }

                return returnItem;
            }

            if (Flags[k_AllowReuseFlag])
            {
                object returnItem = m_OutItems.Array[0];
                if (returnItem == null)
                {
                    ManagedLog.Warning(LogCategory.GDX, $"[ListObjectPool->Get] A null object was returned to the object pool ({m_Key.ToString()}).");
                    return null;
                }

                returnedItem?.Invoke(this, returnItem);
                if (triggerOnSpawnedFromPool)
                {
                    spawnedItem?.Invoke(this, returnItem);
                }

                return returnItem;
            }

            ManagedLog.Warning(LogCategory.GDX, $"[ListObjectPool->Get] Hit maximum object cap of {m_MaximumObjects.ToString()} for object pool ({m_Key.ToString()}).");
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
            int outCount = m_OutItems.Count;
            for (int i = 0; i < outCount; i++)
            {
                if (m_OutItems.Array[i] == item)
                {
                    return true;
                }
            }
            int inCount = InItems.Count;
            for (int i = 0; i < inCount; i++)
            {
                if (InItems.Array[i] == item)
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public bool IsPooled(object item)
        {
            int inCount = InItems.Count;
            for (int i = 0; i < inCount; i++)
            {
                if (InItems.Array[i] == item)
                {
                    return true;
                }
            }
            return false;
        }

        /// <inheritdoc />
        public void Return(object item)
        {
            // Do we have the interface call?
            returnedItem?.Invoke(this, item);

            int outCount = m_OutItems.Count;
            for (int i = 0; i < outCount; i++)
            {
                if (m_OutItems.Array[i] != item)
                {
                    continue;
                }

                m_OutItems.RemoveAtSwapBack(i);
                OutCachedCount--;
                break;
            }

            int inCount = InItems.Count;
            for (int i = 0; i < inCount; i++)
            {
                if (InItems.Array[i] == item)
                {
                    return;
                }
            }

            InItems.AddUnchecked(item);
            InCachedCount++;
        }

        /// <inheritdoc />
        public void ReturnAll(bool shouldShrink = true)
        {
            for (int i = OutCachedCount - 1; i >= 0; i--)
            {
                Return(m_OutItems.Array[i]);
            }

            if (shouldShrink && InCachedCount <= m_MaximumObjects)
            {
                return;
            }

            int removeCount = InCachedCount - m_MaximumObjects;
            for (int i = 0; i < removeCount; i++)
            {
                // Trigger specific logic, like Object.Destroy
                destroyedItem?.Invoke(InItems.Array[i]);

                // Dereferencing
                InItems.RemoveAt(i);
                InCachedCount--;
            }
        }

        /// <inheritdoc />
        public void TearDown()
        {
            tearingDown?.Invoke(this);

            // Return all items to the pool
            for (int i = OutCachedCount - 1; i >= 0; i--)
            {
                if (m_OutItems.Array[i] != null)
                {
                    Return(m_OutItems.Array[i]);

                }
            }

            m_OutItems.Clear();
            OutCachedCount = 0;

            // Wipe internals
            for (int i = InCachedCount - 1; i >= 0; i--)
            {
                if (InItems.Array[i] != null)
                {
                    destroyedItem?.Invoke(InItems.Array[i]);
                }
            }
            InItems.Clear();
            InCachedCount = 0;

            // Unregister
            ManagedPools.Unregister(this);
        }

        /// <summary>
        ///     The <see cref="SimpleListManagedPool" /> destructor which unregisters itself from <see cref="ManagedPools" />.
        /// </summary>
        ~SimpleListManagedPool()
        {
            // Unregister
            ManagedPools.Unregister(this);
        }
    }
}