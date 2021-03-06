using System;
using System.Collections.Generic;
using GDX.Collections.Generic;

namespace GDX.Collections.Pooling
{
    /// <summary>
    ///     A pooling system implementation primarily meant for the object oriented patterns, based on the C# base object.
    /// </summary>
    public static class ManagedPools
    {
        // TODO: Ticket approach? since we need something to find pool
        [NonSerialized] private static int s_nextPoolID;

        // TODO: Need a way to get a pool by the object base?

        /// <summary>
        ///     An internal dictionary containing the pools, uniquely index by constant ID provider.
        /// </summary>
        /// <remarks>
        ///     In a GameObject sense, a Prefab's instance ID would be used, for entities, this is still debatable and should
        ///     be managed by the implementation.
        /// </remarks>
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // TODO: Change over to IntDictionary
        [NonSerialized]
        private static readonly Dictionary<int, IManagedPool> s_pools = new Dictionary<int, IManagedPool>();

        public static int ReservePoolID()
        {
            return ++s_nextPoolID;
        }

        /// <summary>
        ///     Get a registered <see cref="IManagedPool" /> based on its <paramref name="key" />.
        /// </summary>
        /// <param name="key">The unique identifier provided to the system when registering the pool.</param>
        /// <returns>An <see cref="IManagedPool" /> identified by the provided <paramref name="key" />.</returns>
        public static IManagedPool GetPool(int key)
        {
            return s_pools[key];
        }

        public static bool TryGetPoolKey(object baseObject, out int key)
        {
            key = -1;
            int poolCount = s_pools.Count;

            for (int i = 0; i < poolCount; i++)
            {
                if (s_pools[i].GetBaseObject() == baseObject)
                {
                    key = s_pools[i].GetKey();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Get a registered <see cref="IManagedPool" /> based on its <paramref name="key" />.
        /// </summary>
        /// <param name="key">The unique identifier provided to the system when registering the pool.</param>
        /// <typeparam name="T">The type of the pool, used to cast the return pool</typeparam>
        /// <returns>A type casted pool identified by the provided <paramref name="key" />.</returns>
        public static T GetPool<T>(int key)
        {
            return (T)s_pools[key];
        }

        /// <summary>
        ///     Get a registered <see cref="IManagedPool" /> based on its <paramref name="key" />, first checking if it is
        ///     registered.
        /// </summary>
        /// <param name="key">The unique identifier provided to the system when registering the pool.</param>
        /// <returns>An <see cref="IManagedPool" /> identified by the provided <paramref name="key" />, null if not found.</returns>
        public static IManagedPool GetPoolWithContainsCheck(int key)
        {
            return s_pools.ContainsKey(key) ? s_pools[key] : null;
        }

        /// <summary>
        ///     Get a registered <see cref="IManagedPool" /> based on its <paramref name="key" />, first checking if it is
        ///     registered.
        /// </summary>
        /// <param name="key">The unique identifier provided to the system when registering the pool.</param>
        /// <typeparam name="T">The type of the pool, used to cast the return pool</typeparam>
        /// <returns>A type casted pool identified by the provided <paramref name="key" />, null if not found.</returns>
        public static T GetPoolWithContainsCheck<T>(int key) where T : class
        {
            if (s_pools.ContainsKey(key))
            {
                return (T)s_pools[key];
            }

            return null;
        }

        /// <summary>
        ///     Is a <see cref="IManagedPool" /> created with the provided <paramref name="key" />?
        /// </summary>
        /// <param name="key">A unique pool identifier</param>
        /// <returns>true if a pool is found registered with this system, false otherwise.</returns>
        public static bool HasPool(int key)
        {
            return s_pools.ContainsKey(key);
        }

        /// <summary>
        ///     Attempts to return all spawned items to their original pools.
        /// </summary>
        /// <param name="shouldShrink">Should the pool be shrunk (destroying created items) to its original set minimum size?</param>
        public static void ReturnAll(bool shouldShrink = true)
        {
            foreach (KeyValuePair<int, IManagedPool> p in s_pools)
            {
                p.Value.ReturnAll(shouldShrink);
            }
        }

        /// <summary>
        ///     Register a <see cref="IManagedPool" /> with the global management system.
        /// </summary>
        /// <param name="managedPool">Target <see cref="IManagedPool" /></param>
        public static void Register(IManagedPool managedPool)
        {
            // Add to numerical index
            int key = managedPool.GetKey();
            if (!s_pools.ContainsKey(key))
            {
                s_pools.Add(key, managedPool);
            }
            else
            {
                Trace.Output(Trace.TraceLevel.Error,
                    "A managed pool attempted to register itself with the ManagedPools, but the provided ID is already in use.");
            }
        }

        /// <summary>
        ///     Execute <see cref="IManagedPool.TearDown()" /> (destroying contents) on all registered <see cref="IManagedPool" />
        ///     which have
        ///     been flagged to accept it, evaluated by <see cref="IManagedPool.IsAllowedManagedTearDown()" />.
        /// </summary>
        /// <remarks>
        ///     This will unregister the <see cref="IManagedPool" /> itself as well, as all of the content will have been
        ///     destroyed.
        /// </remarks>
        /// <param name="forceAll">
        ///     Execute <see cref="IManagedPool.TearDown()" /> regardless of the
        ///     <see cref="IManagedPool.IsAllowedManagedTearDown()" /> response.
        /// </param>
        public static void TearDown(bool forceAll = false)
        {
            // Make removal buffer
            SimpleList<int> removeKeyBuffer = new SimpleList<int>(s_pools.Count);

            // Now we pay the cost, however its during a teardown so, its less bad.
            int poolCount = s_pools.Count;
            // TODO: Implement MoveNext when available
            foreach (KeyValuePair<int, IManagedPool> pool in s_pools)
            {
                if (!pool.Value.IsAllowedManagedTearDown() && !forceAll)
                {
                    continue;
                }

                pool.Value.TearDown();
                removeKeyBuffer.AddUnchecked(pool.Key);
            }

            // Can't modify the dictionary while iterating over it, so here we are.
            int removeCount = removeKeyBuffer.Count;
            for (int r = 0; r < removeCount; r++)
            {
                s_pools.Remove(removeKeyBuffer.Array[r]);
            }

            Trace.Output(Trace.TraceLevel.Info,
                $"[PoolSystem::TearDown] Removed {removeCount.ToString()}/{poolCount.ToString()}");
        }

        /// <summary>
        ///     Unregister a <see cref="IManagedPool" /> with the global management system.
        /// </summary>
        /// <param name="managedPool">Target <see cref="IManagedPool" /></param>
        public static void Unregister(IManagedPool managedPool)
        {
            if (managedPool == null)
            {
                return;
            }

            int key = managedPool.GetKey();
            if (s_pools.ContainsKey(key))
            {
                s_pools.Remove(key);
            }
        }
    }
}