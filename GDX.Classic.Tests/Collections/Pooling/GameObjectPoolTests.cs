// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Collections.Pooling;
using GDX.Classic;
using GDX.Classic.Collections.Pooling;
using NUnit.Framework;
using UnityEngine;

namespace Runtime.Classic.Collections.Pooling
{
    public class GameObjectPoolTests
    {
        private GameObject _mockBaseObject;
        private GameObject _mockBaseObjectWithInterface;
        private Transform _mockBaseObjectWithInterfaceTransform;
        private Transform _mockTransform;

        [SetUp]
        public void Setup()
        {
            _mockBaseObject = new GameObject();
            _mockTransform = new GameObject().transform;


            _mockBaseObjectWithInterface = new GameObject();
            _mockBaseObjectWithInterface.AddComponent<GameObjectPoolItemTest>();
            _mockBaseObjectWithInterfaceTransform = new GameObject().transform;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_mockBaseObject);
            Object.DestroyImmediate(_mockTransform.gameObject);

            Object.DestroyImmediate(_mockBaseObjectWithInterface);
            Object.DestroyImmediate(_mockBaseObjectWithInterfaceTransform.gameObject);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void OnDestroyItemAction_MockData_DestroysGameObject()
        {
            ListManagedPool pool = (ListManagedPool)GameObjectPool.GetOrCreatePool(_mockBaseObject,
                _mockTransform);
            GameObject gameObject = GameObjectPool.Get(pool, false);
            pool.TearDown();

            bool evaluate = gameObject == null;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void OnDestroyItemAction_MockData_DestroysInterface()
        {
            ListManagedPool pool = (ListManagedPool)GameObjectPool.GetOrCreatePool(_mockBaseObjectWithInterface,
                _mockBaseObjectWithInterfaceTransform);
            GameObject gameObject = GameObjectPool.Get(pool, false);
            pool.TearDown();

            bool evaluate = gameObject == null;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void Get_MockData_DontTrigger()
        {
            ListManagedPool pool = (ListManagedPool)GameObjectPool.GetOrCreatePool(_mockBaseObjectWithInterface,
                _mockBaseObjectWithInterfaceTransform);

            GameObject gameObject = GameObjectPool.Get(pool, false);

            bool evaluate = !gameObject.GetComponent<GameObjectPoolItemTest>().triggeredOnSpawn;

            Assert.IsTrue(evaluate);

            pool.TearDown();
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void Get_MockData_LocalPositionWorldLookAtParented()
        {
            ListManagedPool pool = (ListManagedPool)GameObjectPool.GetOrCreatePool(_mockBaseObjectWithInterface,
                _mockBaseObjectWithInterfaceTransform);

            GameObject gameObject = GameObjectPool.Get(pool, _mockTransform, new Vector3(3f,0f,0f), Vector3.zero);


            bool evaluate = gameObject.transform.position == new Vector3(3, 0, 0) &&
                            gameObject.transform.eulerAngles.Approximately(
                                new Vector3(0f, 270f, 0f)) &&
                            gameObject.transform.parent == _mockTransform;

            Assert.IsTrue(evaluate);

            pool.TearDown();
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void Get_MockData_LocalPositionLocalRotationParented()
        {
            ListManagedPool pool = (ListManagedPool)GameObjectPool.GetOrCreatePool(_mockBaseObjectWithInterface,
                _mockBaseObjectWithInterfaceTransform);

            GameObject gameObject = GameObjectPool.Get(pool, _mockTransform, new Vector3(3f,0f,0f), new Quaternion(0,90,0,0));


            bool evaluate = gameObject.transform.position == new Vector3(3, 0, 0) &&
                            gameObject.transform.eulerAngles.Approximately(
                                new Vector3(0f, 180f, 0f)) &&
                            gameObject.transform.parent == _mockTransform;

            Assert.IsTrue(evaluate);

            pool.TearDown();
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void Get_MockData_ParentedWorldPositionStay()
        {
            GameObject movedObject = new GameObject { transform = { position = new Vector3(1, 0, 0) } };
            GameObject movedTransform = new GameObject { transform = { position = new Vector3(10, 0, 0) } };

            ListManagedPool pool = (ListManagedPool)GameObjectPool.GetOrCreatePool(movedObject, movedTransform.transform);

            GameObject gameObject = GameObjectPool.Get(pool, _mockTransform, true);

            bool evaluate = gameObject.transform.position == new Vector3(1,0,0);

            Assert.IsTrue(evaluate);

            pool.TearDown();
            Object.DestroyImmediate(movedObject);
            Object.DestroyImmediate(movedTransform);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void Get_MockData_ParentedZeroLocalPosition()
        {
            GameObject movedObject = new GameObject { transform = { position = new Vector3(1, 0, 0) } };
            GameObject movedTransform = new GameObject { transform = { position = new Vector3(10, 0, 0) } };

            ListManagedPool pool = (ListManagedPool)GameObjectPool.GetOrCreatePool(movedObject, movedTransform.transform);

            GameObject gameObject = GameObjectPool.Get(pool, _mockTransform);

            bool evaluate = gameObject.transform.position == _mockTransform.transform.position;

            Assert.IsTrue(evaluate);

            pool.TearDown();
            Object.DestroyImmediate(movedObject);
            Object.DestroyImmediate(movedTransform);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void Get_MockData_Trigger()
        {
            ListManagedPool pool = (ListManagedPool)GameObjectPool.GetOrCreatePool(_mockBaseObjectWithInterface,
                _mockBaseObjectWithInterfaceTransform);

            GameObject gameObject = GameObjectPool.Get(pool);

            bool evaluate = gameObject.GetComponent<GameObjectPoolItemTest>().triggeredOnSpawn;

            Assert.IsTrue(evaluate);

            pool.TearDown();
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void Get_MockData_WorldPositionWorldLookAt()
        {
            ListManagedPool pool = (ListManagedPool)GameObjectPool.GetOrCreatePool(_mockBaseObjectWithInterface,
                _mockBaseObjectWithInterfaceTransform);

            GameObject gameObject = GameObjectPool.Get(pool, new Vector3(2, 6, 10), new Vector3(0, 0, 0));

            bool evaluate = gameObject.transform.position == new Vector3(2, 6, 10) &&
                            gameObject.transform.eulerAngles.Approximately(
                                new Vector3(30.4703617f, 191.309921f, 2.47645289E-07f));

            Assert.IsTrue(evaluate);

            pool.TearDown();
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void Get_MockData_WorldPositionWorldLookAtParented()
        {
            ListManagedPool pool = (ListManagedPool)GameObjectPool.GetOrCreatePool(_mockBaseObjectWithInterface,
                _mockBaseObjectWithInterfaceTransform);

            GameObject gameObject = GameObjectPool.Get(pool, new Vector3(2, 6, 10), new Vector3(0, 0, 0),
                _mockBaseObjectWithInterfaceTransform);

            bool evaluate = gameObject.transform.position == new Vector3(2, 6, 10) &&
                            gameObject.transform.eulerAngles.Approximately(
                                new Vector3(30.4703617f, 191.309921f, 2.47645289E-07f)) &&
                            gameObject.transform.parent == _mockBaseObjectWithInterfaceTransform;

            Assert.IsTrue(evaluate);

            pool.TearDown();
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void Get_MockData_WorldPositionWorldRotation()
        {
            ListManagedPool pool = (ListManagedPool)GameObjectPool.GetOrCreatePool(_mockBaseObjectWithInterface,
                _mockBaseObjectWithInterfaceTransform);
            Quaternion mockRotation = new Quaternion(90f, 90f, 45f, 1f);

            GameObject gameObject = GameObjectPool.Get(pool, new Vector3(2, 6, 10), mockRotation);

            bool evaluate = gameObject.transform.position == new Vector3(2, 6, 10) &&
                            gameObject.transform.rotation == mockRotation;

            Assert.IsTrue(evaluate);

            pool.TearDown();
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void Get_MockData_WorldPositionWorldRotationParented()
        {
            ListManagedPool pool = (ListManagedPool)GameObjectPool.GetOrCreatePool(_mockBaseObjectWithInterface,
                _mockBaseObjectWithInterfaceTransform);
            Quaternion mockRotation = new Quaternion(90f, 90f, 45f, 1f);

            GameObject gameObject = GameObjectPool.Get(pool, new Vector3(2, 6, 10), mockRotation,
                _mockBaseObjectWithInterfaceTransform);

            bool evaluate = gameObject.transform.position == new Vector3(2, 6, 10) &&
                            gameObject.transform.rotation == mockRotation &&
                            gameObject.transform.parent == _mockBaseObjectWithInterfaceTransform;

            Assert.IsTrue(evaluate);

            pool.TearDown();
        }


        [Test]
        [Category(GDX.Core.TestCategory)]
        public void GetOrCreatePool_MockData_ReturnsPool()
        {
            IManagedPool pool = GameObjectPool.GetOrCreatePool(_mockBaseObject, _mockTransform);

            Assert.IsNotNull(pool);

            pool.TearDown();
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void TearDown_Pool_UnregistersPool()
        {
            IManagedPool pool = GameObjectPool.GetOrCreatePool(_mockBaseObject, _mockTransform);
            uint cachePoolID = pool.GetKey();

            pool.TearDown();

            Assert.IsFalse(ManagedPools.HasPool(cachePoolID));
        }

        private class GameObjectPoolItemTest : MonoBehaviour, IGameObjectPoolItem
        {
            public bool triggeredOnSpawn;
            private IManagedPool _parentPool;

            /// <inheritdoc />
            public GameObject GetGameObject()
            {
                return gameObject;
            }

            /// <inheritdoc />
            public IManagedPool GetParentPool()
            {
                return _parentPool;
            }

            /// <inheritdoc />
            public bool IsValidItem()
            {
                return gameObject != null;
            }

            /// <inheritdoc />
            public void OnReturnedToPool()
            {
            }

            /// <inheritdoc />
            public void OnSpawnedFromPool()
            {
                triggeredOnSpawn = true;
            }

            /// <inheritdoc />
            public void ReturnToPool()
            {
                _parentPool.Return(this);
            }

            /// <inheritdoc />
            public void SetParentPool(IManagedPool targetManagedPool)
            {
                _parentPool = targetManagedPool;
            }
        }
    }
}