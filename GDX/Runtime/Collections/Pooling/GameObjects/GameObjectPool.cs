// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using UnityEngine;

namespace GDX.Collections.Pooling.GameObjects
{
    public class GameObjectPool : ListObjectPool
    {
        private readonly Transform _cachedParent;
        private readonly bool _isSliced;

        /// <summary>
        ///     Create a <see cref="GameObject" /> based <see cref="ListObjectPool"/> for the provided <paramref name="prefab"/>.
        /// </summary>
        /// <param name="parent">Default transform parent for any created items.</param>
        /// <param name="prefab">The prefab OR <see cref="GameObject"/> which going to be cloned.</param>
        /// <param name="minimumObjects">The minimum number of objects to be pooled.</param>
        /// <param name="maximumObjects">The maximum number of objects to be pooled.</param>
        /// <param name="allowCreateMore">Can more items
        /// be created as needed?</param>
        /// <param name="allowReuseWhenCapped">Should we reuse oldest items when capped?</param>
        /// <param name="sliceInitialCreation">Should the initial spawning be spread across frames using a builder?</param>
        /// <param name="shouldTearDown">Should the object pool tear itself down during scene transitions?</param>
        public GameObjectPool(Transform parent, GameObject prefab, int minimumObjects = 10, int maximumObjects = 50,
            bool allowCreateMore = true, bool allowReuseWhenCapped = false, bool sliceInitialCreation = true,
            bool shouldTearDown = false)
        {
            // Get the hash code cashed before we register
            _cachedParent = parent;
            _baseObject = prefab;
            _allowReuse = allowReuseWhenCapped;
            _allowManagedTearDown = shouldTearDown;

            // Create our unique ID
            _uniqueID = GetUniqueID(prefab);

            // Register with system
            ObjectPools.Register(this);

            _minimumObjects = minimumObjects;
            _maximumObjects = maximumObjects;
            _allowCreateMore = allowCreateMore;

            if (prefab.GetComponent<GameObjectObjectPoolItem>() != null)
            {
                HasItemInterface = true;
            }

            _inItems = new List<object>(maximumObjects);
            _outItems = new List<object>(maximumObjects);

            _isSliced = sliceInitialCreation;
            if (!sliceInitialCreation)
            {
                for (int i = 0; i < minimumObjects; i++)
                {
                    this.CreateItem();
                }
            }
            else
            {
                GameObjectPoolBuilder.AddObjectPool(this);
            }

            _outCount = 0;

            // Assign actions
            OnCreateItem += OnCreateItemAction;
            OnTearDownPrePoolItems += OnTearDownPrePoolItemsAction;
            OnTearDownPostPoolItems += OnTearDownPostPoolItemsAction;
            OnPoolAllItems += OnPoolAllItemsActions;
        }

        public bool HasItemInterface { get; }

        public static int GetUniqueID(GameObject source)
        {
            return $"PrefabPool_{source.GetInstanceID().ToString()}".GetHashCode();
        }



        public Transform GetPoolTransform()
        {
            return _cachedParent;
        }



        // TODO: Maybe this needs to be a create Func?
        void OnCreateItemAction()
        {
            GameObject spawnedObject = Object.Instantiate((GameObject)_baseObject, _cachedParent, false);

            if (!HasItemInterface)
            {
                GameObjectObjectPoolItem item = spawnedObject.AddComponent<GameObjectObjectPoolItem>();
                item.SetParentPool(this);
                item.OnReturnedToPool();
                _inItems.Add(item);
            }
            else
            {
                IObjectPoolItem item = spawnedObject.GetComponent<IObjectPoolItem>();
                item.SetParentPool(this);
                item.OnReturnedToPool();
                _inItems.Add(item);
            }

            _inCount++;
        }

        void OnPoolAllItemsActions(bool shouldShrink = true)
        {
            for (int i = _outCount - 1; i >= 0; i--)
            {
                Pool(_outItems[i]);
            }

            if (shouldShrink && _inCount > _maximumObjects)
            {
                return;
            }

            int removeCount = _inCount - _maximumObjects;
            for (int i = 0; i < removeCount; i++)
            {
                _inItems.RemoveAt(i);
                Object unityObject = (Object)_inItems[i];
                if (unityObject != null)
                {
                    Object.Destroy(unityObject, 0f);
                }
                _inCount--;
            }
        }

        void OnTearDownPrePoolItemsAction()
        {
            if (_isSliced)
            {
                GameObjectPoolBuilder.RemoveObjectPool(this);
            }
        }

        void OnTearDownPostPoolItemsAction()
        {
            for (int i = 0; i < _inCount; i++)
            {
                object inItem = _inItems[i];
                if (inItem == null)
                {
                    continue;
                }

                IObjectPoolItem poolItem = (IObjectPoolItem)inItem;
                if (poolItem.IsValidItem())
                {
                    Object.Destroy((Object)inItem);
                }
            }
        }

        public GameObject Get(Transform parent, bool worldPositionStays = false, bool zeroPosition = true)
        {
            // Get an item but dont trigger OnSpawnedFromPool logic
            object item = Get(false);
            GameObject returnObject = null;
            if (item == null) return null;

            // Return GO
            if (item is MonoBehaviour monoBehaviour)
            {
                Transform transform = monoBehaviour.transform;
                returnObject = transform.gameObject;
                transform.SetParent(parent, worldPositionStays);
                if (!worldPositionStays && zeroPosition)
                {
                    transform.localPosition = Vector3.zero;
                }
            }

            if (item is IObjectPoolItem objectPoolItem)
            {
                objectPoolItem.OnSpawnedFromPool();
            }

            return returnObject;
        }

        /*
        public GameObject Get(Transform parent, Vector3 localPosition, Quaternion localRotation)
        {
            // Are we empty, but have refills?
            if (_inCount == 0 && _outCount < _maximumObjects || _inCount == 0 && _allowCreateMore)
            {
                CreateItem();
            }

            if (_inCount > 0)
            {
                int targetIndex = _inCount - 1;

                // Make sure we don't pull badness
                IObjectPoolItem returnItem = _inItems[targetIndex];
                if (returnItem == null)
                {
                    Trace.Output(Trace.TraceLevel.Warning,
                        $"[GameObjectPool->Get] A null object was pulled from the object pool for {_prefab.name}.");
                    _inCount--;
                    return null;
                }

                // Handle counters
                _outItems.Add(_inItems[targetIndex]);
                _outCount++;
                _inItems.RemoveAt(targetIndex);
                _inCount--;

                // Return GO
                GameObject returnObject = returnItem.GetSelf();
                returnObject.transform.SetParent(parent);
                returnObject.transform.localPosition = localPosition;
                returnObject.transform.localRotation = localRotation;
                returnItem.OnSpawnedFromPool();
                return returnObject;
            }

            if (_allowReuse)
            {
                IObjectPoolItem returnItem = _outItems[0];
                if (returnItem == null)
                {
                    Trace.Output(Trace.TraceLevel.Warning,
                        $"[GameObjectPool->Get] A null object was returned to object pool for {_prefab.name}.");
                    return null;
                }

                returnItem.OnReturnedToPool();

                GameObject returnObject = returnItem.GetSelf();
                returnObject.transform.SetParent(parent);
                returnObject.transform.localPosition = localPosition;
                returnObject.transform.localRotation = localRotation;
                returnItem.OnSpawnedFromPool();
                return returnObject;
            }

            Trace.Output(Trace.TraceLevel.Warning,
                $"[GameObjectPool->Get] Hit maximum object cap of {_maximumObjects.ToString()} for {_prefab.name}.");
            return null;
        }

        public GameObject Get(Transform parent, Vector3 localPosition, Vector3 worldLookAtPosition)
        {
            // Are we empty, but have refills?
            if (_inCount == 0 && _outCount < _maximumObjects || _inCount == 0 && _allowCreateMore)
            {
                CreateItem();
            }

            if (_inCount > 0)
            {
                int targetIndex = _inCount - 1;

                // Make sure we don't pull badness
                IObjectPoolItem returnItem = _inItems[targetIndex];
                if (returnItem == null)
                {
                    Trace.Output(Trace.TraceLevel.Warning,
                        $"[GameObjectPool->Get] A null object was pulled from the object pool for {_prefab.name}.");
                    _inCount--;
                    return null;
                }

                // Handle counters
                _outItems.Add(_inItems[targetIndex]);
                _outCount++;
                _inItems.RemoveAt(targetIndex);
                _inCount--;

                // Return GO
                GameObject returnObject = returnItem.GetSelf();
                returnObject.transform.SetParent(parent);
                returnObject.transform.localPosition = localPosition;
                returnObject.transform.LookAt(worldLookAtPosition);
                returnItem.OnSpawnedFromPool();
                return returnObject;
            }

            if (_allowReuse)
            {
                IObjectPoolItem returnItem = _outItems[0];
                if (returnItem == null)
                {
                    Trace.Output(Trace.TraceLevel.Warning,
                        $"[GameObjectPool->Get] A null object was returned to object pool for {_prefab.name}.");
                    return null;
                }

                returnItem.OnReturnedToPool();

                GameObject returnObject = returnItem.GetSelf();
                returnObject.transform.SetParent(parent);
                returnObject.transform.localPosition = localPosition;
                returnObject.transform.LookAt(worldLookAtPosition);
                returnItem.OnSpawnedFromPool();
                return returnObject;
            }

            Trace.Output(Trace.TraceLevel.Warning,
                $"[GameObjectPool->Get] Hit maximum object cap of {_maximumObjects.ToString()} for {_prefab.name}.");
            return null;
        }

        public GameObject Get(Vector3 worldPosition, Vector3 worldLookAtPosition)
        {
            // Are we empty, but have refills?
            if (_inCount == 0 && _outCount < _maximumObjects || _inCount == 0 && _allowCreateMore)
            {
                CreateItem();
            }

            if (_inCount > 0)
            {
                int targetIndex = _inCount - 1;

                // Make sure we don't pull badness
                IObjectPoolItem returnItem = _inItems[targetIndex];
                if (returnItem == null)
                {
                    Trace.Output(Trace.TraceLevel.Warning,
                        $"[GameObjectPool->Get] A null object was pulled from the object pool for {_prefab.name}.");
                    _inCount--;
                    return null;
                }

                // Handle counters
                _outItems.Add(_inItems[targetIndex]);
                _outCount++;
                _inItems.RemoveAt(targetIndex);
                _inCount--;

                // Return GO
                GameObject returnObject = returnItem.GetSelf();
                returnObject.transform.position = worldPosition;
                returnObject.transform.LookAt(worldLookAtPosition);
                returnItem.OnSpawnedFromPool();
                return returnObject;
            }

            if (_allowReuse)
            {
                IObjectPoolItem returnItem = _outItems[0];
                if (returnItem == null)
                {
                    Trace.Output(Trace.TraceLevel.Warning,
                        $"[GameObjectPool->Get] A null object was returned to object pool for {_prefab.name}.");
                    return null;
                }

                returnItem.OnReturnedToPool();

                GameObject returnObject = returnItem.GetSelf();
                returnObject.transform.position = worldPosition;
                returnObject.transform.LookAt(worldLookAtPosition);
                returnItem.OnSpawnedFromPool();
                return returnObject;
            }

            Trace.Output(Trace.TraceLevel.Warning,
                $"[GameObjectPool->Get] Hit maximum object cap of {_maximumObjects.ToString()} for {_prefab.name}.");
            return null;
        }

        public GameObject Get(Vector3 worldPosition, Quaternion worldRotation)
        {
            // Are we empty, but have refills?
            if (_inCount == 0 && _outCount < _maximumObjects || _inCount == 0 && _allowCreateMore)
            {
                CreateItem();
            }

            if (_inCount > 0)
            {
                int targetIndex = _inCount - 1;

                // Make sure we don't pull badness
                IObjectPoolItem returnItem = _inItems[targetIndex];
                if (returnItem == null)
                {
                    Trace.Output(Trace.TraceLevel.Warning,
                        $"[GameObjectPool->Get] A null object was pulled from the object pool for {_prefab.name}.");
                    _inCount--;
                    return null;
                }

                // Handle counters
                _outItems.Add(_inItems[targetIndex]);
                _outCount++;
                _inItems.RemoveAt(targetIndex);
                _inCount--;

                // Return GO
                GameObject returnObject = returnItem.GetSelf();
                returnObject.transform.SetPositionAndRotation(worldPosition, worldRotation);
                returnItem.OnSpawnedFromPool();
                return returnObject;
            }

            if (_allowReuse)
            {
                IObjectPoolItem returnItem = _outItems[0];
                if (returnItem == null)
                {
                    Trace.Output(Trace.TraceLevel.Warning,
                        $"[GameObjectPool->Get] A null object was returned to object pool for {_prefab.name}.");
                    return null;
                }

                returnItem.OnReturnedToPool();

                GameObject returnObject = returnItem.GetSelf();
                returnObject.transform.SetPositionAndRotation(worldPosition, worldRotation);
                returnItem.OnSpawnedFromPool();
                return returnObject;
            }

            Trace.Output(Trace.TraceLevel.Warning,
                $"[GameObjectPool->Get] Hit maximum object cap of {_maximumObjects.ToString()} for {_prefab.name}.");
            return null;
        }

        public GameObject Get(Vector3 worldPosition, Quaternion worldRotation, Transform parent)
        {
            // Are we empty, but have refills?
            if (_inCount == 0 && _outCount < _maximumObjects || _inCount == 0 && _allowCreateMore)
            {
                CreateItem();
            }

            if (_inCount > 0)
            {
                int targetIndex = _inCount - 1;

                // Make sure we don't pull badness
                IObjectPoolItem returnItem = _inItems[targetIndex];
                if (returnItem == null)
                {
                    Trace.Output(Trace.TraceLevel.Warning,
                        $"[GameObjectPool->Get] A null object was pulled from the object pool for {_prefab.name}.");
                    _inCount--;
                    return null;
                }

                // Handle counters
                _outItems.Add(_inItems[targetIndex]);
                _outCount++;
                _inItems.RemoveAt(targetIndex);
                _inCount--;

                // Return GO
                GameObject returnObject = returnItem.GetSelf();
                returnObject.transform.SetPositionAndRotation(worldPosition, worldRotation);
                returnObject.transform.SetParent(parent, true);
                returnItem.OnSpawnedFromPool();
                return returnObject;
            }

            if (_allowReuse)
            {
                IObjectPoolItem returnItem = _outItems[0];
                if (returnItem == null)
                {
                    Trace.Output(Trace.TraceLevel.Warning,
                        $"[GameObjectPool->Get] A null object was returned to object pool for {_prefab.name}.");
                    return null;
                }

                returnItem.OnReturnedToPool();

                GameObject returnObject = returnItem.GetSelf();
                returnObject.transform.SetPositionAndRotation(worldPosition, worldRotation);
                returnObject.transform.SetParent(parent, true);
                returnItem.OnSpawnedFromPool();
                return returnObject;
            }

            Trace.Output(Trace.TraceLevel.Warning,
                $"[GameObjectPool->Get] Hit maximum object cap of {_maximumObjects.ToString()} for {_prefab.name}.");
            return null;
        }

        public GameObject Get(Vector3 worldPosition, Vector3 worldLookAtPosition, Transform parent)
        {
            // Are we empty, but have refills?
            if (_inCount == 0 && _outCount < _maximumObjects || _inCount == 0 && _allowCreateMore)
            {
                CreateItem();
            }

            if (_inCount > 0)
            {
                int targetIndex = _inCount - 1;

                // Make sure we don't pull badness
                IObjectPoolItem returnItem = _inItems[targetIndex];
                if (returnItem == null)
                {
                    Trace.Output(Trace.TraceLevel.Warning,
                        $"[GameObjectPool->Get] A null object was pulled from the object pool for {_prefab.name}.");
                    _inCount--;
                    return null;
                }

                // Handle counters
                _outItems.Add(_inItems[targetIndex]);
                _outCount++;
                _inItems.RemoveAt(targetIndex);
                _inCount--;

                // Return GO
                GameObject returnObject = returnItem.GetSelf();
                returnObject.transform.position = worldPosition;
                returnObject.transform.SetParent(parent, true);
                returnObject.transform.LookAt(worldLookAtPosition);
                returnItem.OnSpawnedFromPool();
                return returnObject;
            }

            if (_allowReuse)
            {
                IObjectPoolItem returnItem = _outItems[0];
                if (returnItem == null)
                {
                    Trace.Output(Trace.TraceLevel.Warning,
                        $"[GameObjectPool->Get] A null object was returned to object pool for {_prefab.name}.");
                    return null;
                }

                returnItem.OnReturnedToPool();

                GameObject returnObject = returnItem.GetSelf();
                returnObject.transform.position = worldPosition;
                returnObject.transform.SetParent(parent, true);
                returnObject.transform.LookAt(worldLookAtPosition);
                returnItem.OnSpawnedFromPool();
                return returnObject;
            }

            Trace.Output(Trace.TraceLevel.Warning,
                $"[GameObjectPool->Get] Hit maximum object cap of {_maximumObjects.ToString()} for {_prefab.name}.");
            return null;
        }
        */
    }
}