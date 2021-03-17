// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using UnityEngine;

namespace GDX.Collections.Pooling
{
    /// <summary>
    ///     <see cref="GameObject" /> based functionality extending the <see cref="ListManagedPool" /> to better support
    ///     <see cref="GameObject" /> patterns.
    /// </summary>
    public static class GameObjectPool
    {
        /// <summary>
        ///     The <see cref="ListManagedPool" /> flags index used to determine if the object which is used to create new objects
        ///     has the <see cref="IGameObjectPoolItem" /> interface on a root component.
        /// </summary>
        private const int HasInterfaceFlag = 5;

        /// <summary>
        ///     Create a <see cref="GameObject" /> based <see cref="ListManagedPool" /> for the provided
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
            ListManagedPool newGameManagedPool = new ListManagedPool(
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
                Flags = {[HasInterfaceFlag] = gameObject.GetComponent<IGameObjectPoolItem>() != null}
            };

            ManagedPoolBuilder.AddManagedPool(newGameManagedPool);

            newGameManagedPool._outCount = 0;

            // Assign actions
            newGameManagedPool.OnDestroyItem += OnDestroyItemAction;
            newGameManagedPool.OnTearDown += OnTearDownAction;
            newGameManagedPool.OnSpawnedFromPool += OnSpawnedFromPoolAction;
            newGameManagedPool.OnReturnedToPool += OnReturnedToPoolAction;

            return newGameManagedPool;
        }

        /// <summary>
        ///     Get the next available item from the <paramref name="pool"/>.
        /// </summary>
        /// <param name="pool">The <see cref="ListManagedPool"/> created with <see cref="GameObjectPool"/> to pull an item from.</param>
        /// <param name="triggerOnSpawnedFromPool">Should the <see cref="OnSpawnedFromPoolAction"/> be called when getting this item.</param>
        /// <returns>A <see cref="GameObject" /> from the <see cref="ListManagedPool"/>, or null if no item is available.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Get(this ListManagedPool pool, bool triggerOnSpawnedFromPool = true)
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
        /// <param name="pool">The <see cref="ListManagedPool"/> created with <see cref="GameObjectPool"/> to pull an item from.</param>
        /// <param name="parent">The transform parent on the item pulled from the <see cref="ListManagedPool"/>.</param>
        /// <param name="worldPositionStays">Ensure that the world position of the item pulled from the <see cref="ListManagedPool"/> remains the same through parenting.</param>
        /// <param name="zeroLocalPosition">Set the local position of the item pulled from the <see cref="ListManagedPool"/> to being <see cref="Vector3.zero"/> after parenting.</param>
        /// <param name="triggerOnSpawnedFromPool">Should the <see cref="OnSpawnedFromPoolAction"/> be called when getting this item.</param>
        /// <returns>A <see cref="GameObject" /> from the <see cref="ListManagedPool"/>, or null if no item is available.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Get(ListManagedPool pool, Transform parent, bool worldPositionStays = false, bool zeroLocalPosition = true, bool triggerOnSpawnedFromPool = true)
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
                return returnObject;
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
        /// <param name="pool">The <see cref="ListManagedPool"/> created with <see cref="GameObjectPool"/> to pull an item from.</param>
        /// <param name="parent">The transform parent on the item pulled from the <see cref="ListManagedPool"/>.</param>
        /// <param name="localPosition">The local position to set on the item pulled from the <see cref="ListManagedPool"/> after parenting.</param>
        /// <param name="localRotation">The local rotation to set on the item pulled from the <see cref="ListManagedPool"/> after parenting.</param>
        /// <param name="triggerOnSpawnedFromPool">Should the <see cref="OnSpawnedFromPoolAction"/> be called when getting this item.</param>
        /// <returns>A <see cref="GameObject" /> from the <see cref="ListManagedPool"/>, or null if no item is available.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Get(ListManagedPool pool, Transform parent, Vector3 localPosition, Quaternion localRotation, bool triggerOnSpawnedFromPool = true)
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
                return returnObject;
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
        /// <param name="pool">The <see cref="ListManagedPool"/> created with <see cref="GameObjectPool"/> to pull an item from.</param>
        /// <param name="parent">The transform parent on the item pulled from the <see cref="ListManagedPool"/>.</param>
        /// <param name="localPosition">The local position to set on the item pulled from the <see cref="ListManagedPool"/> after parenting.</param>
        /// <param name="worldLookAtPosition">The world position to have the item pulled from the <see cref="ListManagedPool"/> look at</param>
        /// <param name="triggerOnSpawnedFromPool">Should the <see cref="OnSpawnedFromPoolAction"/> be called when getting this item.</param>
        /// <returns>A <see cref="GameObject" /> from the <see cref="ListManagedPool"/>, or null if no item is available.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Get(ListManagedPool pool, Transform parent, Vector3 localPosition, Vector3 worldLookAtPosition, bool triggerOnSpawnedFromPool = true)
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
                return returnObject;
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
        /// <param name="pool">The <see cref="ListManagedPool"/> created with <see cref="GameObjectPool"/> to pull an item from.</param>
        /// <param name="worldPosition">The world position to set on the item pulled from the <see cref="ListManagedPool"/>.</param>
        /// <param name="worldLookAtPosition">The world position to have the item pulled from the <see cref="ListManagedPool"/> look at</param>
        /// <param name="triggerOnSpawnedFromPool">Should the <see cref="OnSpawnedFromPoolAction"/> be called when getting this item.</param>
        /// <returns>A <see cref="GameObject" /> from the <see cref="ListManagedPool"/>, or null if no item is available.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Get(ListManagedPool pool, Vector3 worldPosition, Vector3 worldLookAtPosition, bool triggerOnSpawnedFromPool = true)
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
                return returnObject;
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
        /// <param name="pool">The <see cref="ListManagedPool"/> created with <see cref="GameObjectPool"/> to pull an item from.</param>
        /// <param name="worldPosition">The world position to set on the item pulled from the <see cref="ListManagedPool"/>.</param>
        /// <param name="worldRotation">The world rotation to set on the item pulled from the <see cref="ListManagedPool"/>.</param>
        /// <param name="triggerOnSpawnedFromPool">Should the <see cref="OnSpawnedFromPoolAction"/> be called when getting this item.</param>
        /// <returns>A <see cref="GameObject" /> from the <see cref="ListManagedPool"/>, or null if no item is available.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Get(ListManagedPool pool, Vector3 worldPosition, Quaternion worldRotation, bool triggerOnSpawnedFromPool = true)
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
                return returnObject;
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
        /// <param name="pool">The <see cref="ListManagedPool"/> created with <see cref="GameObjectPool"/> to pull an item from.</param>
        /// <param name="worldPosition">The world position to set on the item pulled from the <see cref="ListManagedPool"/> after parenting.</param>
        /// <param name="worldRotation">The world rotation to set on the item pulled from the <see cref="ListManagedPool"/> after parenting.</param>
        /// <param name="parent">The transform parent on the item pulled from the <see cref="ListManagedPool"/>.</param>
        /// <param name="triggerOnSpawnedFromPool">Should the <see cref="OnSpawnedFromPoolAction"/> be called when getting this item.</param>
        /// <returns>A <see cref="GameObject" /> from the <see cref="ListManagedPool"/>, or null if no item is available.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Get(ListManagedPool pool, Vector3 worldPosition, Quaternion worldRotation, Transform parent, bool triggerOnSpawnedFromPool = true)
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
                return returnObject;
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
        /// <param name="pool">The <see cref="ListManagedPool"/> created with <see cref="GameObjectPool"/> to pull an item from.</param>
        /// <param name="worldPosition">The world position to set on the item pulled from the <see cref="ListManagedPool"/> after parenting.</param>
        /// <param name="worldLookAtPosition">The world position to have the item pulled from the <see cref="ListManagedPool"/> look at after parenting.</param>
        /// <param name="parent">The transform parent on the item pulled from the <see cref="ListManagedPool"/>.</param>
        /// <param name="triggerOnSpawnedFromPool">Should the <see cref="OnSpawnedFromPoolAction"/> be called when getting this item.</param>
        /// <returns>A <see cref="GameObject" /> from the <see cref="ListManagedPool"/>, or null if no item is available.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameObject Get(ListManagedPool pool, Vector3 worldPosition, Vector3 worldLookAtPosition, Transform parent, bool triggerOnSpawnedFromPool = true)
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
                return returnObject;
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
        ///     Gets a pool for the <paramref name="gameObject"/>, or creates a new <see cref="ListManagedPool" /> for it.
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
        /// <param name="pool">The <see cref="ListManagedPool"/> to create an item for, and assign too.</param>
        /// <returns>The newly created item.</returns>
        private static object CreateItem(ListManagedPool pool)
        {
            GameObject spawnedObject =
                Object.Instantiate((GameObject)pool._baseObject, (Transform)pool._containerObject, true);

            if (pool.Flags[HasInterfaceFlag])
            {
                // The old swap for the interface instead of the GameObject
                IGameObjectPoolItem item = spawnedObject.GetComponent<IGameObjectPoolItem>();
                item.SetParentPool(pool);
                item.OnReturnedToPool();
                pool._inItems.Add(item);
                pool._inCount++;
                return item;
            }

            spawnedObject.SetActive(false);
            pool._inItems.Add(spawnedObject);
            pool._inCount++;
            return spawnedObject;
        }

        /// <summary>
        ///     The subscribed action called when an item is requested to be destroyed..
        /// </summary>
        /// <param name="item">The item being destroyed.</param>
        private static void OnDestroyItemAction(object item)
        {
            if (item == null) return;

            Object unityObject = null;
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
        /// <param name="pool">The <see cref="ListManagedPool"/> which the <paramref name="item"/> is being returned to.</param>
        /// <param name="item">The item being returned to the <paramref name="pool"/>.</param>
        private static void OnReturnedToPoolAction(ListManagedPool pool, object item)
        {
            if (!pool.Flags[HasInterfaceFlag])
            {
                (item as GameObject)?.SetActive(false);
                return;
            }

            (item as IGameObjectPoolItem)?.OnReturnedToPool();
        }

        /// <summary>
        ///     The subscribed action called when an item is spawned from the <paramref name="pool"/>.
        /// </summary>
        /// <param name="pool">The <see cref="ListManagedPool"/> which has had the <paramref name="item"/> spawned from.</param>
        /// <param name="item">The spawned item.</param>
        private static void OnSpawnedFromPoolAction(ListManagedPool pool, object item)
        {
            if (!pool.Flags[HasInterfaceFlag])
            {
                (item as GameObject)?.SetActive(true);
                return;
            }

            (item as IGameObjectPoolItem)?.OnSpawnedFromPool();
        }

        /// <summary>
        ///     The subscribed action called when the <paramref name="pool"/> is asked to <see cref="IManagedPool.TearDown()"/> before items were returned to the pool.
        /// </summary>
        /// <param name="pool">The <see cref="ListManagedPool"/> being torn down.</param>
        private static void OnTearDownAction(ListManagedPool pool)
        {
            ManagedPoolBuilder.RemoveManagedPool(pool);
        }
    }
}