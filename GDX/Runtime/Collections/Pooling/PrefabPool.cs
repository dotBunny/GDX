// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using UnityEngine;

namespace GDX.Collections.Pooling
{
    public class PrefabPool : ListPoolBase<GameObject, IPoolItem<GameObject>>
    {
        private readonly Transform _cachedParent;
        private readonly bool _isSliced;
        private readonly GameObject _prefab;

        /// <summary>
        ///     Prefab Pool Constructor
        /// </summary>
        /// <param name="parent">Default transform parent for any created items.</param>
        /// <param name="prefab">Target Prefab</param>
        /// <param name="minimumObjects">The minimum number of objects to be pooled.</param>
        /// <param name="maximumObjects">The maximum number of objects to be pooled.</param>
        /// <param name="allowCreateMore">Can more items be created as needed?</param>
        /// <param name="allowReuseWhenCapped">Should we reuse oldest items when capped?</param>
        /// <param name="sliceInitialCreation">Should the initial spawning be spread across frames using a builder?</param>
        /// <param name="shouldTearDown">Should the object pool tear itself down during scene transitions?</param>
        public PrefabPool(Transform parent, GameObject prefab, int minimumObjects = 10, int maximumObjects = 50,
            bool allowCreateMore = true, bool allowReuseWhenCapped = false, bool sliceInitialCreation = true,
            bool shouldTearDown = false)
        {
            // Get the hash code cashed before we register
            _cachedParent = parent;
            _prefab = prefab;
            _allowReuse = allowReuseWhenCapped;
            _allowManagedTearDown = shouldTearDown;

            // Create our unique ID
            _uniqueID = GetUniqueID(prefab);

            // Register with system
            Pools.Register(this);

            _minimumObjects = minimumObjects;
            _maximumObjects = maximumObjects;
            _allowCreateMore = allowCreateMore;

            if (prefab.GetComponent<PrefabPoolItem>() != null)
            {
                HasItemInterface = true;
            }

            _inItems = new List<IPoolItem<GameObject>>(maximumObjects);
            _outItems = new List<IPoolItem<GameObject>>(maximumObjects);

            _isSliced = sliceInitialCreation;
            if (!sliceInitialCreation)
            {
                for (int i = 0; i < minimumObjects; i++)
                {
                    CreateItem();
                }
            }
            else
            {
                PrefabPoolBuilder.AddObjectPool(this);
            }

            _outCount = 0;
        }

        public bool HasItemInterface { get; }

        public static int GetUniqueID(GameObject source)
        {
            return $"PrefabPool_{source.GetInstanceID().ToString()}".GetHashCode();
        }

        public GameObject GetBaseItem()
        {
            return _prefab;
        }


        public Transform GetPoolTransform()
        {
            return _cachedParent;
        }


        public void Pool(IPoolItem<GameObject> targetObject)
        {
            targetObject.OnReturnedToPool();

            if (_outItems.Contains(targetObject))
            {
                _outItems.Remove(targetObject);
                _outCount--;
            }

            if (!_inItems.Contains(targetObject))
            {
                _inItems.Add(targetObject);
                _inCount++;
            }
        }


        /// <inheritdoc />
        public sealed override void CreateItem()
        {
            GameObject spawnedObject = Object.Instantiate(_prefab, _cachedParent, false);

            if (!HasItemInterface)
            {
                PrefabPoolItem item = spawnedObject.AddComponent<PrefabPoolItem>();
                item.SetParentPool(this);
                item.OnReturnedToPool();
                _inItems.Add(item);
            }
            else
            {
                IPoolItem<GameObject> item = spawnedObject.GetComponent<IPoolItem<GameObject>>();
                item.SetParentPool(this);
                item.OnReturnedToPool();
                _inItems.Add(item);
            }

            _inCount++;
        }

        /// <inheritdoc />
        public override void PoolAllItems(bool shouldShrink = true)
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
                IPoolItem<GameObject> destroyObject = _inItems[i];
                _inItems.RemoveAt(i);
                Object.Destroy(destroyObject.GetSelf());
                _inCount--;
            }
        }

        /// <inheritdoc />
        public override void TearDown()
        {
            if (_isSliced)
            {
                PrefabPoolBuilder.RemoveObjectPool(this);
            }

            for (int i = _outCount - 1; i >= 0; i--)
            {
                if (_outItems[i] != null)
                {
                    Pool(_outItems[i]);
                }
            }

            _outItems.Clear();
            _outCount = 0;

            for (int i = 0; i < _inCount; i++)
            {
                if (_inItems[i] != null && _inItems[i].IsValidItem())
                {
                    Object.Destroy(_inItems[i].GetSelf());
                }
            }

            _inItems.Clear();
            _inCount = 0;
        }

        public override GameObject Get()
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
                IPoolItem<GameObject> returnItem = _inItems[targetIndex];
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
                returnItem.OnSpawnedFromPool();
                return returnItem.GetSelf();
            }

            if (_allowReuse)
            {
                IPoolItem<GameObject> returnItem = _outItems[0];
                if (returnItem == null)
                {
                    Trace.Output(Trace.TraceLevel.Warning,
                        $"[GameObjectPool->Get] A null object was returned to object pool for {_prefab.name}.");
                    return null;
                }

                returnItem.OnReturnedToPool();
                returnItem.OnSpawnedFromPool();
                return returnItem.GetSelf();
            }

            Trace.Output(Trace.TraceLevel.Warning,
                $"[GameObjectPool->Get] Hit maximum object cap of {_maximumObjects.ToString()} for {_prefab.name}.");
            return null;
        }

        public GameObject Get(Transform parent, bool worldPositionStays = false, bool zeroPosition = true)
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
                IPoolItem<GameObject> returnItem = _inItems[targetIndex];
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
                returnObject.transform.SetParent(parent, worldPositionStays);
                if (!worldPositionStays && zeroPosition)
                {
                    returnObject.transform.localPosition = Vector3.zero;
                }

                returnItem.OnSpawnedFromPool();
                return returnObject;
            }

            if (_allowReuse)
            {
                IPoolItem<GameObject> returnItem = _outItems[0];
                if (returnItem == null)
                {
                    Trace.Output(Trace.TraceLevel.Warning,
                        $"[GameObjectPool->Get] A null object was returned to object pool for {_prefab.name}.");
                    return null;
                }

                returnItem.OnReturnedToPool();

                GameObject returnObject = returnItem.GetSelf();
                returnObject.transform.SetParent(parent, worldPositionStays);
                returnItem.OnSpawnedFromPool();
                return returnObject;
            }

            Trace.Output(Trace.TraceLevel.Warning,
                $"[GameObjectPool->Get] Hit maximum object cap of {_maximumObjects.ToString()} for {_prefab.name}.");
            return null;
        }

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
                IPoolItem<GameObject> returnItem = _inItems[targetIndex];
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
                IPoolItem<GameObject> returnItem = _outItems[0];
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
                IPoolItem<GameObject> returnItem = _inItems[targetIndex];
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
                IPoolItem<GameObject> returnItem = _outItems[0];
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
                IPoolItem<GameObject> returnItem = _inItems[targetIndex];
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
                IPoolItem<GameObject> returnItem = _outItems[0];
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
                IPoolItem<GameObject> returnItem = _inItems[targetIndex];
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
                IPoolItem<GameObject> returnItem = _outItems[0];
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
                IPoolItem<GameObject> returnItem = _inItems[targetIndex];
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
                IPoolItem<GameObject> returnItem = _outItems[0];
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
                IPoolItem<GameObject> returnItem = _inItems[targetIndex];
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
                IPoolItem<GameObject> returnItem = _outItems[0];
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
    }
}