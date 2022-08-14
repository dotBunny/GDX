// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if !UNITY_DOTSRUNTIME

using System.Runtime.CompilerServices;
using UnityEngine;

namespace GDX.Collections.Pooling
{
    /// <summary>
    ///     <see cref="GameObject" /> based functionality extending the <see cref="SimpleListManagedPool" /> to better support
    ///     <see cref="GameObject" /> patterns.
    /// </summary>
    /// <exception cref="UnsupportedRuntimeException">Not supported on DOTS Runtime.</exception>
    [VisualScriptingCompatible(1)]
    public static class GameObjectPool
    {
        /// <summary>
        ///     The <see cref="SimpleListManagedPool" /> flags index used to determine if the object which is used to create new objects
        ///     has the <see cref="IGameObjectPoolItem" /> interface on a root component.
        /// </summary>
        const int k_HasInterfaceFlag = 5;

        /// <summary>
        ///     Create a <see cref="GameObject" /> based <see cref="SimpleListManagedPool" /> for the provided
        ///     <paramref name="gameObject" />.
        /// </summary>
        /// <param name="gameObject">The object which going to be cloned.</param>
        /// <param name="parent">The container object.</param>
        /// <param name="minimumObjects">The minimum number of objects to be pooled.</param>
        /// <param name="maximumObjects">The maximum number of objects to be pooled.</param>
        /// <param name="allowCreateMore">Can more items be created as needed when starved for items?</param>
        /// <param name="allowReuseWhenCapped">Should we reuse oldest items when starving for items?</param>
        /// <param name="allowManagedTearDown">Does the pool allow a managed tear down event call?</param>
        public static IManagedPool CreatePool(
            GameObject gameObject,
            Transform parent,
            int minimumObjects = 10,
            int maximumObjects = 50,
            bool allowCreateMore = true,
            bool allowReuseWhenCapped = false,
            bool allowManagedTearDown = false)
        {
            // Create our new pool
            SimpleListManagedPool newGameManagedPool = new SimpleListManagedPool(
                gameObject,
                CreateItem,
                minimumObjects,
                maximumObjects,
                parent,
                false,
                allowCreateMore,
                allowReuseWhenCapped,
                allowManagedTearDown)
            {
                Flags = {[k_HasInterfaceFlag] = gameObject.GetComponent<IGameObjectPoolItem>() != null}
            };

            ManagedPoolBuilder.AddManagedPool(newGameManagedPool);

            newGameManagedPool.OutCachedCount = 0;

            // Assign actions
            newGameManagedPool.destroyedItem += OnDestroyItemAction;
            newGameManagedPool.tearingDown += OnTearDownAction;
            newGameManagedPool.spawnedItem += OnSpawnedFromPoolAction;
            newGameManagedPool.returnedItem += OnReturnedToPoolAction;

            return newGameManagedPool;
        }

        /// <summary>
        ///     Get the next available item from the <paramref name="pool"/>.
        /// </summary>
        /// <param name="pool">The <see cref="SimpleListManagedPool"/> created with <see cref="GameObjectPool"/> to pull an item from.</param>
        /// <param name="triggerOnSpawnedFromPool">Should the <see cref="OnSpawnedFromPoolAction"/> be called when getting this item.</param>
        /// <returns>A <see cref="GameObject" /> from the <see cref="SimpleListManagedPool"/>, or null if no item is available.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Get(SimpleListManagedPool pool, bool triggerOnSpawnedFromPool = true)
        {
            // Pull
            object item = pool.Get(false);
            if (item == null)
            {
                return null;
            }

            // Actions
            if (triggerOnSpawnedFromPool)
            {
                OnSpawnedFromPoolAction(pool, item);
            }

            // Return
            return item is IGameObjectPoolItem gameObjectPoolItem ? gameObjectPoolItem.GetGameObject() : (GameObject)item;
        }

        /// <summary>
        ///     Get the next available item from the <paramref name="pool"/> and parent it to a <see cref="Transform"/>.
        /// </summary>
        /// <param name="pool">The <see cref="SimpleListManagedPool"/> created with <see cref="GameObjectPool"/> to pull an item from.</param>
        /// <param name="parent">The transform parent on the item pulled from the <see cref="SimpleListManagedPool"/>.</param>
        /// <param name="worldPositionStays">Ensure that the world position of the item pulled from the <see cref="SimpleListManagedPool"/> remains the same through parenting.</param>
        /// <param name="zeroLocalPosition">Set the local position of the item pulled from the <see cref="SimpleListManagedPool"/> to being <see cref="Vector3.zero"/> after parenting.</param>
        /// <param name="triggerOnSpawnedFromPool">Should the <see cref="OnSpawnedFromPoolAction"/> be called when getting this item.</param>
        /// <returns>A <see cref="GameObject" /> from the <see cref="SimpleListManagedPool"/>, or null if no item is available.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Get(SimpleListManagedPool pool, Transform parent, bool worldPositionStays = false, bool zeroLocalPosition = true, bool triggerOnSpawnedFromPool = true)
        {
            // Pull
            object item = pool.Get(false);
            if (item == null)
            {
                return null;
            }
            GameObject returnObject = item is IGameObjectPoolItem gameObjectPoolItem ? gameObjectPoolItem.GetGameObject() : (GameObject)item;
            if (returnObject == null)
            {
                return null;
            }

            // Translate
            Transform transform = returnObject.transform;
            transform.SetParent(parent, worldPositionStays);
            if (!worldPositionStays && zeroLocalPosition)
            {
                transform.localPosition = Vector3.zero;
            }

            // Actions
            if (triggerOnSpawnedFromPool)
            {
                OnSpawnedFromPoolAction(pool, item);
            }

            // Return
            return returnObject;
        }

        /// <summary>
        ///     Get the next available item from the <paramref name="pool"/>, parent it to a <see cref="Transform"/>, and then set it's local position and rotation.
        /// </summary>
        /// <param name="pool">The <see cref="SimpleListManagedPool"/> created with <see cref="GameObjectPool"/> to pull an item from.</param>
        /// <param name="parent">The transform parent on the item pulled from the <see cref="SimpleListManagedPool"/>.</param>
        /// <param name="localPosition">The local position to set on the item pulled from the <see cref="SimpleListManagedPool"/> after parenting.</param>
        /// <param name="localRotation">The local rotation to set on the item pulled from the <see cref="SimpleListManagedPool"/> after parenting.</param>
        /// <param name="triggerOnSpawnedFromPool">Should the <see cref="OnSpawnedFromPoolAction"/> be called when getting this item.</param>
        /// <returns>A <see cref="GameObject" /> from the <see cref="SimpleListManagedPool"/>, or null if no item is available.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Get(SimpleListManagedPool pool, Transform parent, Vector3 localPosition, Quaternion localRotation, bool triggerOnSpawnedFromPool = true)
        {
            // Pull
            object item = pool.Get(false);
            if (item == null)
            {
                return null;
            }
            GameObject returnObject = item is IGameObjectPoolItem gameObjectPoolItem ? gameObjectPoolItem.GetGameObject() : (GameObject)item;
            if (returnObject == null)
            {
                return null;
            }

            // Translate
            Transform transform = returnObject.transform;
            transform.SetParent(parent);
            transform.localPosition = localPosition;
            transform.localRotation = localRotation;

            // Actions
            if (triggerOnSpawnedFromPool)
            {
                OnSpawnedFromPoolAction(pool, item);
            }

            // Return
            return returnObject;
        }

        /// <summary>
        ///     Get the next available item from the <paramref name="pool"/>, parent it to a <see cref="Transform"/>, and then setting it's local position and where it is looking.
        /// </summary>
        /// <param name="pool">The <see cref="SimpleListManagedPool"/> created with <see cref="GameObjectPool"/> to pull an item from.</param>
        /// <param name="parent">The transform parent on the item pulled from the <see cref="SimpleListManagedPool"/>.</param>
        /// <param name="localPosition">The local position to set on the item pulled from the <see cref="SimpleListManagedPool"/> after parenting.</param>
        /// <param name="worldLookAtPosition">The world position to have the item pulled from the <see cref="SimpleListManagedPool"/> look at</param>
        /// <param name="triggerOnSpawnedFromPool">Should the <see cref="OnSpawnedFromPoolAction"/> be called when getting this item.</param>
        /// <returns>A <see cref="GameObject" /> from the <see cref="SimpleListManagedPool"/>, or null if no item is available.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Get(SimpleListManagedPool pool, Transform parent, Vector3 localPosition, Vector3 worldLookAtPosition, bool triggerOnSpawnedFromPool = true)
        {
            // Pull
            object item = pool.Get(false);
            if (item == null)
            {
                return null;
            }
            GameObject returnObject = item is IGameObjectPoolItem gameObjectPoolItem ? gameObjectPoolItem.GetGameObject() : (GameObject)item;
            if (returnObject == null)
            {
                return null;
            }

            // Translate
            Transform transform = returnObject.transform;
            transform.SetParent(parent);
            transform.localPosition = localPosition;
            transform.LookAt(worldLookAtPosition);

            // Actions
            if (triggerOnSpawnedFromPool)
            {
                OnSpawnedFromPoolAction(pool, item);
            }

            // Return
            return returnObject;
        }

        /// <summary>
        ///     Get the next available item from the <paramref name="pool"/>, and set its world position and where it is looking.
        /// </summary>
        /// <param name="pool">The <see cref="SimpleListManagedPool"/> created with <see cref="GameObjectPool"/> to pull an item from.</param>
        /// <param name="worldPosition">The world position to set on the item pulled from the <see cref="SimpleListManagedPool"/>.</param>
        /// <param name="worldLookAtPosition">The world position to have the item pulled from the <see cref="SimpleListManagedPool"/> look at</param>
        /// <param name="triggerOnSpawnedFromPool">Should the <see cref="OnSpawnedFromPoolAction"/> be called when getting this item.</param>
        /// <returns>A <see cref="GameObject" /> from the <see cref="SimpleListManagedPool"/>, or null if no item is available.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Get(SimpleListManagedPool pool, Vector3 worldPosition, Vector3 worldLookAtPosition, bool triggerOnSpawnedFromPool = true)
        {
            // Pull
            object item = pool.Get(false);
            if (item == null)
            {
                return null;
            }
            GameObject returnObject = item is IGameObjectPoolItem gameObjectPoolItem ? gameObjectPoolItem.GetGameObject() : (GameObject)item;
            if (returnObject == null)
            {
                return null;
            }

            // Translate
            Transform transform = returnObject.transform;
            transform.position = worldPosition;
            transform.LookAt(worldLookAtPosition);

            // Actions
            if (triggerOnSpawnedFromPool)
            {
                OnSpawnedFromPoolAction(pool, item);
            }

            // Return
            return returnObject;
        }

        /// <summary>
        ///     Get the next available item from the <paramref name="pool"/>, and set its world position and rotation.
        /// </summary>
        /// <param name="pool">The <see cref="SimpleListManagedPool"/> created with <see cref="GameObjectPool"/> to pull an item from.</param>
        /// <param name="worldPosition">The world position to set on the item pulled from the <see cref="SimpleListManagedPool"/>.</param>
        /// <param name="worldRotation">The world rotation to set on the item pulled from the <see cref="SimpleListManagedPool"/>.</param>
        /// <param name="triggerOnSpawnedFromPool">Should the <see cref="OnSpawnedFromPoolAction"/> be called when getting this item.</param>
        /// <returns>A <see cref="GameObject" /> from the <see cref="SimpleListManagedPool"/>, or null if no item is available.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Get(SimpleListManagedPool pool, Vector3 worldPosition, Quaternion worldRotation, bool triggerOnSpawnedFromPool = true)
        {
            // Pull
            object item = pool.Get(false);
            if (item == null)
            {
                return null;
            }
            GameObject returnObject = item is IGameObjectPoolItem gameObjectPoolItem ? gameObjectPoolItem.GetGameObject() : (GameObject)item;
            if (returnObject == null)
            {
                return null;
            }

            // Translate
            Transform transform = returnObject.transform;
            transform.SetPositionAndRotation(worldPosition, worldRotation);

            // Actions
            if (triggerOnSpawnedFromPool)
            {
                OnSpawnedFromPoolAction(pool, item);
            }

            // Return
            return returnObject;
        }

        /// <summary>
        ///     Get the next available item from the <paramref name="pool"/>, and set its world position and rotation after parenting.
        /// </summary>
        /// <param name="pool">The <see cref="SimpleListManagedPool"/> created with <see cref="GameObjectPool"/> to pull an item from.</param>
        /// <param name="worldPosition">The world position to set on the item pulled from the <see cref="SimpleListManagedPool"/> after parenting.</param>
        /// <param name="worldRotation">The world rotation to set on the item pulled from the <see cref="SimpleListManagedPool"/> after parenting.</param>
        /// <param name="parent">The transform parent on the item pulled from the <see cref="SimpleListManagedPool"/>.</param>
        /// <param name="triggerOnSpawnedFromPool">Should the <see cref="OnSpawnedFromPoolAction"/> be called when getting this item.</param>
        /// <returns>A <see cref="GameObject" /> from the <see cref="SimpleListManagedPool"/>, or null if no item is available.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Get(SimpleListManagedPool pool, Vector3 worldPosition, Quaternion worldRotation, Transform parent, bool triggerOnSpawnedFromPool = true)
        {
            // Pull
            object item = pool.Get(false);
            if (item == null)
            {
                return null;
            }
            GameObject returnObject = item is IGameObjectPoolItem gameObjectPoolItem ? gameObjectPoolItem.GetGameObject() : (GameObject)item;
            if (returnObject == null)
            {
                return null;
            }

            // Translate
            Transform transform = returnObject.transform;
            transform.SetPositionAndRotation(worldPosition, worldRotation);
            transform.SetParent(parent, true);

            // Actions
            if (triggerOnSpawnedFromPool)
            {
                OnSpawnedFromPoolAction(pool, item);
            }

            // Return
            return returnObject;
        }

        /// <summary>
        ///     Get the next available item from the <paramref name="pool"/>, and set its world position and look at position after parenting.
        /// </summary>
        /// <param name="pool">The <see cref="SimpleListManagedPool"/> created with <see cref="GameObjectPool"/> to pull an item from.</param>
        /// <param name="worldPosition">The world position to set on the item pulled from the <see cref="SimpleListManagedPool"/> after parenting.</param>
        /// <param name="worldLookAtPosition">The world position to have the item pulled from the <see cref="SimpleListManagedPool"/> look at after parenting.</param>
        /// <param name="parent">The transform parent on the item pulled from the <see cref="SimpleListManagedPool"/>.</param>
        /// <param name="triggerOnSpawnedFromPool">Should the <see cref="OnSpawnedFromPoolAction"/> be called when getting this item.</param>
        /// <returns>A <see cref="GameObject" /> from the <see cref="SimpleListManagedPool"/>, or null if no item is available.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Get(SimpleListManagedPool pool, Vector3 worldPosition, Vector3 worldLookAtPosition, Transform parent, bool triggerOnSpawnedFromPool = true)
        {
            // Pull
            object item = pool.Get(false);
            if (item == null)
            {
                return null;
            }
            GameObject returnObject = item is IGameObjectPoolItem gameObjectPoolItem ? gameObjectPoolItem.GetGameObject() : (GameObject)item;
            if (returnObject == null)
            {
                return null;
            }

            // Translate
            Transform transform = returnObject.transform;
            transform.position = worldPosition;
            transform.SetParent(parent, true);
            transform.LookAt(worldLookAtPosition);

            // Actions
            if(triggerOnSpawnedFromPool)
            {
                OnSpawnedFromPoolAction(pool, item);
            }

            // Return
            return returnObject;
        }

        /// <summary>
        ///     Gets a pool for the <paramref name="gameObject"/>, or creates a new <see cref="SimpleListManagedPool" /> for it.
        /// </summary>
        /// <param name="gameObject">The object which going to be cloned.</param>
        /// <param name="parent">The container object.</param>
        /// <param name="minimumObjects">The minimum number of objects to be pooled.</param>
        /// <param name="maximumObjects">The maximum number of objects to be pooled.</param>
        /// <param name="allowCreateMore">Can more items be created as needed when starved for items?</param>
        /// <param name="allowReuseWhenCapped">Should we reuse oldest items when starving for items?</param>
        /// <param name="allowManagedTearDown">Does the pool allow a managed tear down event call?</param>
        public static IManagedPool GetOrCreatePool(
            GameObject gameObject,
            Transform parent,
            int minimumObjects = 10,
            int maximumObjects = 50,
            bool allowCreateMore = true,
            bool allowReuseWhenCapped = false,
            bool allowManagedTearDown = false)
        {
            if (ManagedPools.TryGetFirstPool(gameObject, out IManagedPool checkPool))
            {
                return checkPool;
            }
            return CreatePool(gameObject, parent, minimumObjects, maximumObjects, allowCreateMore, allowReuseWhenCapped,
                allowManagedTearDown);
        }

        /// <summary>
        ///     Create a new item for the <paramref name="pool"/>.
        /// </summary>
        /// <param name="pool">The <see cref="SimpleListManagedPool"/> to create an item for, and assign too.</param>
        /// <returns>The newly created item.</returns>
        static object CreateItem(SimpleListManagedPool pool)
        {
            GameObject spawnedObject =
                Object.Instantiate((GameObject)pool.BaseObject, (Transform)pool.ContainerObject, true);

            if (pool.Flags[k_HasInterfaceFlag])
            {
                // The old swap for the interface instead of the GameObject
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation, Unity.ExpensiveCode
                IGameObjectPoolItem item = spawnedObject.GetComponent<IGameObjectPoolItem>();
                item.SetParentPool(pool);
                item.OnReturnedToPool();
                pool.InItems.AddWithExpandCheck(item);
                pool.InCachedCount++;
                return item;
            }

            spawnedObject.SetActive(false);
            pool.InItems.AddWithExpandCheck(spawnedObject);
            pool.InCachedCount++;
            return spawnedObject;
        }

        /// <summary>
        ///     The subscribed action called when an item is requested to be destroyed..
        /// </summary>
        /// <param name="item">The item being destroyed.</param>
        static void OnDestroyItemAction(object item)
        {
            if (item == null) return;

            Object unityObject;
            if (item is IGameObjectPoolItem poolItem && poolItem.IsValidItem())
            {
                unityObject = poolItem.GetGameObject();
            }
            else
            {
                unityObject = (Object)item;
            }

            if (unityObject != null)
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    Object.Destroy(unityObject, 0f);
                }
                else
                {
                    Object.DestroyImmediate(unityObject);
                }
#else
                Object.Destroy(unityObject, 0f);
#endif

            }

        }

        /// <summary>
        ///     The subscribed action called when an item is returned to the <paramref name="pool"/>.
        /// </summary>
        /// <param name="pool">The <see cref="SimpleListManagedPool"/> which the <paramref name="item"/> is being returned to.</param>
        /// <param name="item">The item being returned to the <paramref name="pool"/>.</param>
        static void OnReturnedToPoolAction(SimpleListManagedPool pool, object item)
        {
            if (!pool.Flags[k_HasInterfaceFlag])
            {
                (item as GameObject)?.SetActive(false);
                return;
            }

            (item as IGameObjectPoolItem)?.OnReturnedToPool();
        }

        /// <summary>
        ///     The subscribed action called when an item is spawned from the <paramref name="pool"/>.
        /// </summary>
        /// <param name="pool">The <see cref="SimpleListManagedPool"/> which has had the <paramref name="item"/> spawned from.</param>
        /// <param name="item">The spawned item.</param>
        static void OnSpawnedFromPoolAction(SimpleListManagedPool pool, object item)
        {
            if (!pool.Flags[k_HasInterfaceFlag])
            {
                (item as GameObject)?.SetActive(true);
                return;
            }

            (item as IGameObjectPoolItem)?.OnSpawnedFromPool();
        }

        /// <summary>
        ///     The subscribed action called when the <paramref name="pool"/> is asked to <see cref="IManagedPool.TearDown()"/> before items were returned to the pool.
        /// </summary>
        /// <param name="pool">The <see cref="SimpleListManagedPool"/> being torn down.</param>
        static void OnTearDownAction(SimpleListManagedPool pool)
        {
            ManagedPoolBuilder.RemoveManagedPool(pool);
        }
    }
}
#endif