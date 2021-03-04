using System.Collections.Generic;
using GDX.Collections.Generic;

namespace GDX.Collections.Pooling
{
    /// <summary>
    ///     A pooling system implementation primarily meant for the object oriented patterns, based on the C# base object.
    /// </summary>
    public static class ObjectPools
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
        private static readonly Dictionary<int, IObjectPool> s_pools = new Dictionary<int, IObjectPool>();

        // TODO: Should autocreate?
        /// <summary>
        ///     Get a registered <see cref="IObjectPool" /> based on its <paramref name="uniqueID" />.
        /// </summary>
        /// <param name="uniqueID">The unique identifier provided to the system when registering the pool.</param>
        /// <returns>An <see cref="IObjectPool" /> identified by the provided <paramref name="uniqueID" />.</returns>
        public static IObjectPool GetPool(int uniqueID)
        {
            return s_pools[uniqueID];
        }

        public static T GetPool<T>(int uniqueID)
        {
            return (T)s_pools[uniqueID];
        }

        public static IObjectPool GetPool(string key)
        {
            return s_pools[key.GetHashCode()];
        }

        public static T GetPool<T>(string key)
        {
            return (T)s_pools[key.GetHashCode()];
        }

        public static IObjectPool GetPoolWithContainsCheck(int uniqueID)
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

        public static IObjectPool GetPoolWithContainsCheck(string key)
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
        ///     Is a <see cref="IObjectPool" /> created with the provided <paramref name="uniqueID" />?
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
            foreach (KeyValuePair<int, IObjectPool> p in s_pools)
            {
                p.Value.PoolAllItems(shouldShrink);
            }
        }

        /// <summary>
        ///     Register a <see cref="IObjectPool" /> with the global management system.
        /// </summary>
        /// <param name="objectPool">Target <see cref="IObjectPool" /></param>
        public static void Register(IObjectPool objectPool)
        {
            if (!s_pools.ContainsKey(objectPool.GetUniqueID()))
            {
                s_pools.Add(objectPool.GetUniqueID(), objectPool);
            }
        }

        /// <summary>
        ///     Execute <see cref="IObjectPool.TearDown()" /> (destroying contents) on all registered <see cref="IObjectPool" /> which have
        ///     been flagged to accept it, evaluated by <see cref="IObjectPool.IsAllowedManagedTearDown()" />.
        /// </summary>
        /// <remarks>This will unregister the <see cref="IObjectPool" /> itself as well, as all of the content will have been destroyed.</remarks>
        /// <param name="forceAll">
        ///     Execute <see cref="IObjectPool.TearDown()" /> regardless of the <see cref="IObjectPool.IsAllowedManagedTearDown()" /> response.
        /// </param>
        public static void TearDown(bool forceAll = false)
        {
            // Make removal buffer
            SimpleList<int> removeKeyBuffer = new SimpleList<int>(s_pools.Count);

            // Now we pay the cost, however its during a teardown so, its less bad.
            int poolCount = s_pools.Count;
            // TODO: Implement MoveNext when available
            foreach (KeyValuePair<int, IObjectPool> pool in s_pools)
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
        ///     Unregister a <see cref="IObjectPool" /> with the global management system.
        /// </summary>
        /// <param name="objectPool">Target <see cref="IObjectPool" /></param>
        public static void Unregister(IObjectPool objectPool)
        {
            if (objectPool == null)
            {
                return;
            }

            if (s_pools.ContainsKey(objectPool.GetUniqueID()))
            {
                s_pools.Remove(objectPool.GetUniqueID());
            }
        }
    }
}