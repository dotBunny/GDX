// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using UnityEngine;

namespace GDX.Collections.Pooling.GameObjects
{
    public class GameObjectPoolBuilder : MonoBehaviour
    {
        private const int InstantiatesPerFrame = 5;
        private static GameObject s_builderObject;
        private static readonly List<IManagedPool> s_targetPools = new List<IManagedPool>();
        private static int s_targetPoolsCount;

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

        public static void AddObjectPool(IManagedPool targetManagedPool)
        {
            if (s_targetPools.Contains(targetManagedPool))
            {
                return;
            }

            s_targetPools.Add(targetManagedPool);
            s_targetPoolsCount++;

            if (s_builderObject == null)
            {
                CreateBuilder();
            }
        }

        public static void RemoveObjectPool(IManagedPool targetManagedPool)
        {
            if (!s_targetPools.Contains(targetManagedPool))
            {
                return;
            }

            s_targetPools.Remove(targetManagedPool);
            s_targetPoolsCount--;
        }

        private static void CreateBuilder()
        {
            s_builderObject = new GameObject("GDX.GameObjectPoolBuilder");
            s_builderObject.AddComponent<GameObjectPoolBuilder>();
        }

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