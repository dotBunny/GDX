﻿// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using UnityEngine;

namespace GDX.Collections.Pooling
{
    /// <summary>
    ///     A time-slicing builder behaviour for <see cref="IManagedPool" />.
    /// </summary>
    /// <remarks>A demonstration of usage can be found in <see cref="GameObjectPool" />.</remarks>
    public class ManagedPoolBuilder : MonoBehaviour
    {
        /// <summary>
        ///     The number of instantiates to make per frame.
        /// </summary>
        /// <remarks>
        ///     During defined loading periods this value could be increased for faster allocations,
        ///     and then returned to a much more performant value afterwards.
        /// </remarks>
        public static int InstantiatesPerFrame = 5;

        /// <summary>
        ///     Should the <see cref="ManagedPoolBuilder" /> destroy itself when finished?
        /// </summary>
        public static bool DestroyBuilderOnFinish = true;

        /// <summary>
        ///     A cached reference to the <see cref="GameObject" /> the builder created for itself.
        /// </summary>
        private static GameObject s_builderObject;

        /// <summary>
        ///     A <see cref="List{T}" /> of <see cref="IManagedPool" /> which are being built out.
        /// </summary>
        private static readonly List<IManagedPool> s_targetPools = new List<IManagedPool>();

        /// <summary>
        ///     A cached numerical count of the number of <see cref="IManagedPool" /> contained in <see cref="s_targetPools" />.
        /// </summary>
        private static int s_targetPoolsCount;

        /// <summary>
        ///     Unity's LateUpdate event
        /// </summary>
        private void LateUpdate()
        {
            Tick();

            // Do we have work to be done?
            if (s_targetPoolsCount != 0)
            {
                return;
            }

            // Nothing left to do, get rid of myself
            s_builderObject = null;
            Destroy(gameObject);
        }

        /// <summary>
        ///     Add an <see cref="IManagedPool" /> to the builder to be built out.
        /// </summary>
        /// <param name="targetManagedPool">The <see cref="IManagedPool" /> to build out.</param>
        public static void AddManagedPool(IManagedPool targetManagedPool)
        {
            if (s_targetPools.Contains(targetManagedPool))
            {
                return;
            }

            s_targetPools.Add(targetManagedPool);
            s_targetPoolsCount++;

            // We already have a builder, no need to make one.
            if (s_builderObject != null)
            {
                return;
            }

            s_builderObject = new GameObject("GDX.GameObjectPoolBuilder");
            s_builderObject.AddComponent<ManagedPoolBuilder>();
        }

        /// <summary>
        ///     Remove an <see cref="IManagedPool" /> from the builder.
        /// </summary>
        /// <param name="targetManagedPool">The <see cref="IManagedPool" /> to be removed.</param>
        public static void RemoveManagedPool(IManagedPool targetManagedPool)
        {
            if (!s_targetPools.Contains(targetManagedPool))
            {
                return;
            }

            s_targetPools.Remove(targetManagedPool);
            s_targetPoolsCount--;

            // We still have pools, no sense destroying anything yet
            if (s_targetPoolsCount > 0 || !DestroyBuilderOnFinish)
            {
                return;
            }
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Destroy(s_builderObject);
            }
            else
            {
                DestroyImmediate(s_builderObject);
            }
#else
            Destroy(s_builderObject);
#endif
            s_builderObject = null;
        }

        /// <summary>
        ///     Extracted tick update for the builder; creating a limited number of items per tick.
        /// </summary>
        private void Tick()
        {
            int spawnsThisUpdate = 0;

            for (int i = s_targetPoolsCount - 1; i >= 0; i--)
            {
                if (s_targetPools[i] == null)
                {
                    continue;
                }

                if (s_targetPools[i].HasMinimumPooledItems())
                {
                    s_targetPools.RemoveAt(i);
                    s_targetPoolsCount--;
                }
                else
                {
                    // Build Item
                    s_targetPools[i].CreateItem();

                    spawnsThisUpdate++;
                    if (spawnsThisUpdate > InstantiatesPerFrame)
                    {
                        break;
                    }
                }
            }
        }
    }
}