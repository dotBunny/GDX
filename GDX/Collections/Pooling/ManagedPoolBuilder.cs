// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if !UNITY_DOTSRUNTIME

using System.Collections.Generic;
using UnityEngine;

namespace GDX.Collections.Pooling
{
    /// <summary>
    ///     A time-slicing builder behaviour for <see cref="IManagedPool" />.
    /// </summary>
    /// <remarks>A demonstration of usage can be found in <see cref="GameObjectPool" />.</remarks>
    /// <exception cref="UnsupportedRuntimeException">Not supported on DOTS Runtime.</exception>
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
        static GameObject s_BuilderObject;

        /// <summary>
        ///     A <see cref="List{T}" /> of <see cref="IManagedPool" /> which are being built out.
        /// </summary>
        static readonly List<IManagedPool> k_TargetPools = new List<IManagedPool>();

        /// <summary>
        ///     A cached numerical count of the number of <see cref="IManagedPool" /> contained in <see cref="k_TargetPools" />.
        /// </summary>
        static int s_TargetPoolsCount;

        /// <summary>
        ///     Unity's LateUpdate event.
        /// </summary>
#pragma warning disable IDE0051
        // ReSharper disable UnusedMember.Local
        void LateUpdate()
        {
            Tick();

            // Do we have work to be done?
            if (s_TargetPoolsCount != 0)
            {
                return;
            }

            // Nothing left to do, get rid of myself
            s_BuilderObject = null;
            Destroy(gameObject);
        }
        // ReSharper restore UnusedMember.Local
#pragma warning restore IDE0051

        /// <summary>
        ///     Add an <see cref="IManagedPool" /> to the builder to be built out.
        /// </summary>
        /// <param name="targetManagedPool">The <see cref="IManagedPool" /> to build out.</param>
        public static void AddManagedPool(IManagedPool targetManagedPool)
        {
            if (k_TargetPools.Contains(targetManagedPool))
            {
                return;
            }

            k_TargetPools.Add(targetManagedPool);
            s_TargetPoolsCount++;

            // We already have a builder, no need to make one.
            if (s_BuilderObject != null)
            {
                return;
            }

            s_BuilderObject = new GameObject("GDX.GameObjectPoolBuilder");
            s_BuilderObject.AddComponent<ManagedPoolBuilder>();
        }

        /// <summary>
        ///     Remove an <see cref="IManagedPool" /> from the builder.
        /// </summary>
        /// <param name="targetManagedPool">The <see cref="IManagedPool" /> to be removed.</param>
        public static void RemoveManagedPool(IManagedPool targetManagedPool)
        {
            if (!k_TargetPools.Contains(targetManagedPool))
            {
                return;
            }

            k_TargetPools.Remove(targetManagedPool);
            s_TargetPoolsCount--;

            // We still have pools, no sense destroying anything yet
            if (s_TargetPoolsCount > 0 || !DestroyBuilderOnFinish)
            {
                return;
            }
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Destroy(s_BuilderObject);
            }
            else
            {
                DestroyImmediate(s_BuilderObject);
            }
#else
            Destroy(s_BuilderObject);
#endif
            s_BuilderObject = null;
        }

        /// <summary>
        ///     Extracted tick update for the builder; creating a limited number of items per tick.
        /// </summary>
        static void Tick()
        {
            int spawnsThisUpdate = 0;

            for (int i = s_TargetPoolsCount - 1; i >= 0; i--)
            {
                if (k_TargetPools[i] == null)
                {
                    continue;
                }

                if (k_TargetPools[i].HasMinimumPooledItems())
                {
                    k_TargetPools.RemoveAt(i);
                    s_TargetPoolsCount--;
                }
                else
                {
                    // Build Item
                    k_TargetPools[i].CreateItem();

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
#endif // !UNITY_DOTSRUNTIME