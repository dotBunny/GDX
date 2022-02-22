// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using GDX.Collections.Generic;

// ReSharper disable UnusedMember.Global

namespace GDX.Collections.Pooling
{
    /// <summary>
    ///     A managed pooling system implementation primarily meant for the object oriented patterns, based on the C# base
    ///     object.
    /// </summary>
    [VisualScriptingCompatible(1)]
    public static class ManagedPools
    {
        /// <summary>
        ///     The last issued pool key used by internal dictionary's <see cref="KeyValuePair{TKey,TValue}" /> when referencing an
        ///     <see cref="IManagedPool" />.
        /// </summary>
        /// <remarks>
        ///     This value resets on domain reload, and as such the keys should not be relied on through any sort of
        ///     serialization (including networking) or session based process.
        /// </remarks>
        static uint s_LastPoolKey;

        /// <summary>
        ///     An internal dictionary containing the <see cref="IManagedPool" />s, uniquely indexed by constant ticket-like
        ///     system.
        /// </summary>
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        static readonly Dictionary<uint, IManagedPool> s_Pools = new Dictionary<uint, IManagedPool>();

        /// <summary>
        ///     Get the next available pool key.
        /// </summary>
        /// <remarks>Increments the previously issued stored value, and returns the new value.</remarks>
        /// <returns>A unique pool identifying key.</returns>
        public static uint GetNextPoolKey()
        {
            s_LastPoolKey++;
            return s_LastPoolKey;
        }

        /// <summary>
        ///     Get a registered <see cref="IManagedPool" /> based on its <paramref name="key" />.
        /// </summary>
        /// <param name="key">The unique key to use when looking for the <see cref="IManagedPool" />.</param>
        /// <returns>An <see cref="IManagedPool" /> identified by the provided <paramref name="key" />.</returns>
        public static IManagedPool GetPool(uint key)
        {
            return s_Pools[key];
        }

        /// <summary>
        ///     Get a registered <see cref="IManagedPool" /> based on its <paramref name="key" />.
        /// </summary>
        /// <param name="key">The unique key to use when looking for the <see cref="IManagedPool" />.</param>
        /// <typeparam name="T">The type of the pool, used to cast the return pool</typeparam>
        /// <returns>A type casted pool identified by the provided <paramref name="key" />.</returns>
        public static T GetPool<T>(uint key)
        {
            return (T)s_Pools[key];
        }

        /// <summary>
        ///     Get a registered <see cref="IManagedPool" /> based on its <paramref name="key" />, first checking if it is
        ///     registered.
        /// </summary>
        /// <param name="key">The unique key to use when looking for the <see cref="IManagedPool" />.</param>
        /// <returns>An <see cref="IManagedPool" /> identified by the provided <paramref name="key" />, null if not found.</returns>
        public static IManagedPool GetPoolWithContainsCheck(uint key)
        {
            return s_Pools.ContainsKey(key) ? s_Pools[key] : null;
        }

        /// <summary>
        ///     Get a registered <see cref="IManagedPool" /> based on its <paramref name="key" />, first checking if it is
        ///     registered.
        /// </summary>
        /// <param name="key">The unique key to use when looking for the <see cref="IManagedPool" />.</param>
        /// <typeparam name="T">The type of the pool, used to cast the return pool</typeparam>
        /// <returns>A type casted pool identified by the provided <paramref name="key" />, null if not found.</returns>
        public static T GetPoolWithContainsCheck<T>(uint key) where T : class
        {
            if (s_Pools.ContainsKey(key))
            {
                return (T)s_Pools[key];
            }

            return null;
        }

        /// <summary>
        ///     Is an <see cref="IManagedPool" /> registered with the provided <paramref name="key" />?
        /// </summary>
        /// <param name="key">A unique pool key</param>
        /// <returns>true if a pool is found registered with this system, false otherwise.</returns>
        public static bool HasPool(uint key)
        {
            return s_Pools.ContainsKey(key);
        }

        /// <summary>
        ///     Attempts to return all spawned items to their original <see cref="IManagedPool" />s.
        /// </summary>
        /// <param name="shouldShrink">Should the pool be shrunk (destroying created items) to its original set minimum size?</param>
        public static void ReturnAll(bool shouldShrink = true)
        {
            foreach (KeyValuePair<uint, IManagedPool> p in s_Pools)
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
            uint key = managedPool.GetKey();
            if (!s_Pools.ContainsKey(key))
            {
                s_Pools.Add(key, managedPool);
            }
            else
            {
                Trace.Output(Trace.TraceLevel.Error,
                    "A managed pool attempted to register itself with the ManagedPools, but the provided key is already in use.");
            }
        }

        /// <summary>
        ///     Execute <see cref="IManagedPool.TearDown()" /> (destroying contents) on all registered <see cref="IManagedPool" />
        ///     which have been flagged to accept it, evaluated by <see cref="IManagedPool.IsAllowedManagedTearDown()" />.
        /// </summary>
        /// <remarks>
        ///     This will unregister the <see cref="IManagedPool" /> itself as well.
        /// </remarks>
        /// <param name="forceAll">
        ///     Execute <see cref="IManagedPool.TearDown()" /> regardless of the
        ///     <see cref="IManagedPool.IsAllowedManagedTearDown()" /> response.
        /// </param>
        public static void TearDown(bool forceAll = false)
        {
            // Make removal buffer
            SimpleList<uint> removeKeyBuffer = new SimpleList<uint>(s_Pools.Count);

            // Now we pay the cost, however its during a teardown so, its less bad.
            int poolCount = s_Pools.Count;

            foreach (KeyValuePair<uint, IManagedPool> pool in s_Pools)
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
                s_Pools.Remove(removeKeyBuffer.Array[r]);
            }

            Trace.Output(Trace.TraceLevel.Info,
                $"[PoolSystem::TearDown] Removed {removeCount.ToString()}/{poolCount.ToString()}");
        }

        /// <summary>
        ///     Attempt to get an <see cref="IManagedPool" /> based on the <paramref name="baseObject" />.
        /// </summary>
        /// <param name="baseObject">The <see cref="object" /> which was used to create the pool.</param>
        /// <param name="pool">The first found <see cref="IManagedPool" /> created with <paramref name="baseObject" />.</param>
        /// <returns>true/false if an <see cref="IManagedPool" /> was found.</returns>
        public static bool TryGetFirstPool(object baseObject, out IManagedPool pool)
        {
            pool = null;
            foreach (KeyValuePair<uint, IManagedPool> kvp in s_Pools)
            {
                if (kvp.Value.GetBaseObject() != baseObject)
                {
                    continue;
                }

                pool = kvp.Value;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Unregister a <see cref="IManagedPool" /> from with the management system.
        /// </summary>
        /// <param name="managedPool">Target <see cref="IManagedPool" /></param>
        public static void Unregister(IManagedPool managedPool)
        {
            // Well we cant be doing anything with that
            if (managedPool == null)
            {
                return;
            }

            // Get the apparent key
            uint key = managedPool.GetKey();

            // Checks key and the matching pool
            if (s_Pools.ContainsKey(key) && s_Pools[key] == managedPool)
            {
                // Remove it from the registry
                s_Pools.Remove(key);
            }
        }
    }
}