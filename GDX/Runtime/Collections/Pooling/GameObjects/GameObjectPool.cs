﻿// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using UnityEngine;

namespace GDX.Collections.Pooling.GameObjects
{
    public static class GameObjectPool
    {
        private const int HasInterfaceFlag = 5;

        /// <summary>
        ///     Create a <see cref="GameObject" /> based <see cref="ListManagedPool" /> for the provided <paramref name="prefab" />.
        /// </summary>
        /// <param name="parent">The container object.</param>
        /// <param name="prefab">The object which going to be cloned.</param>
        /// <param name="minimumObjects">The minimum number of objects to be pooled.</param>
        /// <param name="maximumObjects">The maximum number of objects to be pooled.</param>
        /// <param name="allowCreateMore">Can more items be created as needed when starved for items?</param>
        /// <param name="allowReuseWhenCapped">Should we reuse oldest items when starving for items?</param>
        /// <param name="allowManagedTearDown">Does the pool allow a managed tear down event call?</param>
        public static IManagedPool Create(
            Transform parent,
            UnityEngine.GameObject prefab,
            int minimumObjects = 10,
            int maximumObjects = 50,
            bool allowCreateMore = true,
            bool allowReuseWhenCapped = false,
            bool allowManagedTearDown = false)
        {
            // We already have a pool for this ID
            int uniqueID = GetUniqueID(prefab);
            if (ManagedPools.HasPool(uniqueID))
            {
                return ManagedPools.GetPool<ListManagedPool>(uniqueID);
            }

            // Create our new pool
            ListManagedPool newGameManagedPool = new ListManagedPool(
                uniqueID,
                prefab,
                minimumObjects,
                maximumObjects,
                parent,
                false,
                allowCreateMore,
                allowReuseWhenCapped,
                allowManagedTearDown)
            {
                Flags = {[HasInterfaceFlag] = prefab.GetComponent<IGameObjectPoolItem>() != null}
            };

            GameObjectPoolBuilder.AddObjectPool(newGameManagedPool);

            newGameManagedPool._outCount = 0;

            // Assign actions
            newGameManagedPool.OnCreateItem += OnCreateItemAction;
            newGameManagedPool.OnDestroyItem += OnDestroyItemAction;

            newGameManagedPool.OnTearDownPrePoolItems += OnTearDownPrePoolItemsAction;
            newGameManagedPool.OnTearDownPostPoolItems += OnTearDownPostPoolItemsAction;
            newGameManagedPool.OnSpawnedFromPool += OnSpawnedFromPoolAction;
            newGameManagedPool.OnReturnedToPool += OnReturnedToPoolAction;


            return newGameManagedPool;
        }

        public static int GetUniqueID(UnityEngine.GameObject source)
        {
            return $"GameObject_{source.GetInstanceID().ToString()}".GetHashCode();
        }

        private static void OnCreateItemAction(ListManagedPool pool)
        {
            GameObject spawnedObject =
                Object.Instantiate((GameObject)pool._baseObject, (Transform)pool._containerObject, false);

            if (pool.Flags[HasInterfaceFlag])
            {
                // The old swap for the interface instead of the GameObject
                IGameObjectPoolItem item = spawnedObject.GetComponent<IGameObjectPoolItem>();
                item.SetParentPool(pool);
                item.OnReturnedToPool();
                pool._inItems.Add(item);
            }
            else
            {
                spawnedObject.SetActive(false);
                pool._inItems.Add(spawnedObject);
            }

            pool._inCount++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void OnSpawnedFromPoolAction(ListManagedPool pool, object item)
        {
            if (!pool.Flags[HasInterfaceFlag])
            {
                (item as GameObject)?.SetActive(true);
                return;
            }

            (item as IGameObjectPoolItem)?.OnSpawnedFromPool();
        }

        private static void OnReturnedToPoolAction(ListManagedPool pool, object item)
        {
            if (!pool.Flags[HasInterfaceFlag])
            {
                (item as GameObject)?.SetActive(false);
                return;
            }

            (item as IGameObjectPoolItem)?.OnReturnedToPool();
        }

        private static void OnDestroyItemAction(object item)
        {
            Object unityObject = (Object)item;
            if (unityObject != null)
            {
                Object.Destroy(unityObject, 0f);
            }
        }

        private static void OnTearDownPrePoolItemsAction(ListManagedPool pool)
        {
            GameObjectPoolBuilder.RemoveObjectPool(pool);
        }

        private static void OnTearDownPostPoolItemsAction(ListManagedPool pool)
        {
            for (int i = 0; i < pool._inCount; i++)
            {
                object inItem = pool._inItems[i];
                if (inItem == null)
                {
                    continue;
                }

                if (inItem is IGameObjectPoolItem gameObjectItem && gameObjectItem.IsValidItem())
                {
                    Object.Destroy((Object)inItem);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Get(ListManagedPool pool)
        {
            // Get an item but dont trigger OnSpawnedFromPool logic
            object item = pool.Get(false);
            if (item == null)
            {
                return null;
            }

            GameObject returnObject = pool.Flags[HasInterfaceFlag] && item is IGameObjectPoolItem gameObjectPoolItem
                ? gameObjectPoolItem.GetGameObject()
                : (GameObject)item;

            return returnObject;
        }

        public static GameObject Get(ListManagedPool pool, Transform parent, bool worldPositionStays = false,
            bool zeroPosition = true)
        {
            // Get an item but dont trigger OnSpawnedFromPool logic
            object item = pool.Get(false);
            if (item == null)
            {
                return null;
            }

            GameObject returnObject = pool.Flags[HasInterfaceFlag] && item is IGameObjectPoolItem gameObjectPoolItem
                ? gameObjectPoolItem.GetGameObject()
                : (GameObject)item;

            if (returnObject == null)
            {
                return returnObject;
            }

            Transform transform = returnObject.transform;
            transform.SetParent(parent, worldPositionStays);
            if (!worldPositionStays && zeroPosition)
            {
                transform.localPosition = Vector3.zero;
            }

            OnSpawnedFromPoolAction(pool, item);
            return returnObject;
        }

        public static GameObject Get(ListManagedPool pool, Transform parent, Vector3 localPosition,
            Quaternion localRotation)
        {
            // Get an item but dont trigger OnSpawnedFromPool logic
            object item = pool.Get(false);
            if (item == null)
            {
                return null;
            }

            GameObject returnObject = pool.Flags[HasInterfaceFlag] && item is IGameObjectPoolItem gameObjectPoolItem
                ? gameObjectPoolItem.GetGameObject()
                : (GameObject)item;

            if (returnObject == null)
            {
                return returnObject;
            }

            Transform transform = returnObject.transform;
            transform.SetParent(parent);
            transform.localPosition = localPosition;
            transform.localRotation = localRotation;
            OnSpawnedFromPoolAction(pool, item);
            return returnObject;
        }

        public static GameObject Get(ListManagedPool pool, Transform parent, Vector3 localPosition,
            Vector3 worldLookAtPosition)
        {
            // Get an item but dont trigger OnSpawnedFromPool logic
            object item = pool.Get(false);
            if (item == null)
            {
                return null;
            }

            GameObject returnObject = pool.Flags[HasInterfaceFlag] && item is IGameObjectPoolItem gameObjectPoolItem
                ? gameObjectPoolItem.GetGameObject()
                : (GameObject)item;

            if (returnObject == null)
            {
                return returnObject;
            }

            Transform transform = returnObject.transform;
            transform.SetParent(parent);
            transform.localPosition = localPosition;
            transform.LookAt(worldLookAtPosition);
            OnSpawnedFromPoolAction(pool, item);
            return returnObject;
        }

        public static GameObject Get(ListManagedPool pool, Vector3 worldPosition, Vector3 worldLookAtPosition)
        {
            // Get an item but dont trigger OnSpawnedFromPool logic
            object item = pool.Get(false);
            if (item == null)
            {
                return null;
            }

            GameObject returnObject = pool.Flags[HasInterfaceFlag] && item is IGameObjectPoolItem gameObjectPoolItem
                ? gameObjectPoolItem.GetGameObject()
                : (GameObject)item;

            if (returnObject == null)
            {
                return returnObject;
            }

            Transform transform = returnObject.transform;
            transform.position = worldPosition;
            transform.LookAt(worldLookAtPosition);
            OnSpawnedFromPoolAction(pool, item);
            return returnObject;
        }

        public static GameObject Get(ListManagedPool pool, Vector3 worldPosition, Quaternion worldRotation)
        {
            // Get an item but dont trigger OnSpawnedFromPool logic
            object item = pool.Get(false);
            if (item == null)
            {
                return null;
            }

            GameObject returnObject = pool.Flags[HasInterfaceFlag] && item is IGameObjectPoolItem gameObjectPoolItem
                ? gameObjectPoolItem.GetGameObject()
                : (GameObject)item;

            if (returnObject == null)
            {
                return returnObject;
            }

            Transform transform = returnObject.transform;
            transform.SetPositionAndRotation(worldPosition, worldRotation);
            OnSpawnedFromPoolAction(pool, item);
            return returnObject;
        }

        public static GameObject Get(ListManagedPool pool, Vector3 worldPosition, Quaternion worldRotation,
            Transform parent)
        {
            // Get an item but dont trigger OnSpawnedFromPool logic
            object item = pool.Get(false);
            if (item == null)
            {
                return null;
            }

            GameObject returnObject = pool.Flags[HasInterfaceFlag] && item is IGameObjectPoolItem gameObjectPoolItem
                ? gameObjectPoolItem.GetGameObject()
                : (GameObject)item;

            if (returnObject == null)
            {
                return returnObject;
            }

            Transform transform = returnObject.transform;
            transform.SetPositionAndRotation(worldPosition, worldRotation);
            transform.SetParent(parent, true);
            OnSpawnedFromPoolAction(pool, item);
            return returnObject;
        }

        public static GameObject Get(ListManagedPool pool, Vector3 worldPosition, Vector3 worldLookAtPosition,
            Transform parent)
        {
            // Get an item but dont trigger OnSpawnedFromPool logic
            object item = pool.Get(false);
            if (item == null)
            {
                return null;
            }

            GameObject returnObject = pool.Flags[HasInterfaceFlag] && item is IGameObjectPoolItem gameObjectPoolItem
                ? gameObjectPoolItem.GetGameObject()
                : (GameObject)item;

            if (returnObject == null)
            {
                return returnObject;
            }

            Transform transform = returnObject.transform;
            transform.position = worldPosition;
            transform.SetParent(parent, true);
            transform.LookAt(worldLookAtPosition);
            OnSpawnedFromPoolAction(pool, item);
            return returnObject;
        }
    }
}