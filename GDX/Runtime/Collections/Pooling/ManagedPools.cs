using System.Collections.Generic;
using GDX.Collections.Generic;

namespace GDX.Collections.Pooling
{
    /// <summary>
    ///     A pooling system implementation primarily meant for the object oriented patterns, based on the C# base object.
    /// </summary>
    public static class ManagedPools
    {
        /// <summary>
        ///     An internal dictionary containing the pools, uniquely index by constant ID provider.
        /// </summary>
        /// <remarks>
        ///     In a GameObject sense, a Prefab's instance ID would be used, for entities, this is still debatable and should
        ///     be managed by the implementation.
        /// </remarks>
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // TODO: Change over to IntDictionary
        private static readonly Dictionary<int, IManagedPool> s_pools = new Dictionary<int, IManagedPool>();

        // TODO: Should autocreate?
        /// <summary>
        ///     Get a registered <see cref="IManagedPool" /> based on its <paramref name="uniqueID" />.
        /// </summary>
        /// <param name="uniqueID">The unique identifier provided to the system when registering the pool.</param>
        /// <returns>An <see cref="IManagedPool" /> identified by the provided <paramref name="uniqueID" />.</returns>
        public static IManagedPool GetPool(int uniqueID)
        {
            return s_pools[uniqueID];
        }

        public static T GetPool<T>(int uniqueID)
        {
            return (T)s_pools[uniqueID];
        }

        public static IManagedPool GetPool(string key)
        {
            return s_pools[key.GetHashCode()];
        }

        public static T GetPool<T>(string key)
        {
            return (T)s_pools[key.GetHashCode()];
        }

        public static IManagedPool GetPoolWithContainsCheck(int uniqueID)
        {
            if (s_pools.ContainsKey(uniqueID))
            {
                return s_pools[uniqueID];
            }

            return null;
        }

        public static T GetPoolWithContainsCheck<T>(int uniqueID) where T : class
        {
            if (s_pools.ContainsKey(uniqueID))
            {
                return (T)s_pools[uniqueID];
            }
            return null;
        }

        public static IManagedPool GetPoolWithContainsCheck(string key)
        {
            int uniqueID = key.GetHashCode();
            if (s_pools.ContainsKey(uniqueID))
            {
                return s_pools[uniqueID];
            }
            return null;
        }

        public static T GetPoolWithContainsCheck<T>(string key) where T : class
        {
            int uniqueID = key.GetHashCode();
            if (s_pools.ContainsKey(uniqueID))
            {
                return (T)s_pools[uniqueID];
            }
            return null;
        }

        /// <summary>
        ///     Is a <see cref="IManagedPool" /> created with the provided <paramref name="uniqueID" />?
        /// </summary>
        /// <param name="uniqueID">A unique pool identifier</param>
        /// <returns>true if a pool is found registered with this system, false otherwise.</returns>
        public static bool HasPool(int uniqueID)
        {
            return s_pools.ContainsKey(uniqueID);
        }

        public static bool HasPool(string key)
        {
            return HasPool(key.GetHashCode());
        }

        /// <summary>
        ///     Attempts to return all *spawned* items to their original pools.
        /// </summary>
        /// <param name="shouldShrink">Should the pool be shrunk (destroying created items) to its original set minimum size?</param>
        public static void PoolAllItem(bool shouldShrink = true)
        {
            foreach (KeyValuePair<int, IManagedPool> p in s_pools)
            {
                p.Value.PoolAllItems(shouldShrink);
            }
        }

        /// <summary>
        ///     Register a <see cref="IManagedPool" /> with the global management system.
        /// </summary>
        /// <param name="managedPool">Target <see cref="IManagedPool" /></param>
        public static void Register(IManagedPool managedPool)
        {
            if (!s_pools.ContainsKey(managedPool.GetUniqueID()))
            {
                s_pools.Add(managedPool.GetUniqueID(), managedPool);
            }
        }

        /// <summary>
        ///     Execute <see cref="IManagedPool.TearDown()" /> (destroying contents) on all registered <see cref="IManagedPool" /> which have
        ///     been flagged to accept it, evaluated by <see cref="IManagedPool.IsAllowedManagedTearDown()" />.
        /// </summary>
        /// <remarks>This will unregister the <see cref="IManagedPool" /> itself as well, as all of the content will have been destroyed.</remarks>
        /// <param name="forceAll">
        ///     Execute <see cref="IManagedPool.TearDown()" /> regardless of the <see cref="IManagedPool.IsAllowedManagedTearDown()" /> response.
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

            if (s_pools.ContainsKey(managedPool.GetUniqueID()))
            {
                s_pools.Remove(managedPool.GetUniqueID());
            }
        }
    }
}