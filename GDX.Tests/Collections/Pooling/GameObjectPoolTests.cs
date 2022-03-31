// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;
using UnityEngine;

namespace GDX.Collections.Pooling
{
    public class GameObjectPoolTests
    {
        GameObject m_MockBaseObject;
        GameObject m_MockBaseObjectWithInterface;
        Transform m_MockBaseObjectWithInterfaceTransform;
        Transform m_MockTransform;

        [SetUp]
        public void Setup()
        {
            m_MockBaseObject = new GameObject();
            m_MockTransform = new GameObject().transform;


            m_MockBaseObjectWithInterface = new GameObject();
            m_MockBaseObjectWithInterface.AddComponent<GameObjectPoolItemTest>();
            m_MockBaseObjectWithInterfaceTransform = new GameObject().transform;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(m_MockBaseObject);
            Object.DestroyImmediate(m_MockTransform.gameObject);

            Object.DestroyImmediate(m_MockBaseObjectWithInterface);
            Object.DestroyImmediate(m_MockBaseObjectWithInterfaceTransform.gameObject);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void OnDestroyItemAction_MockData_DestroysGameObject()
        {
            SimpleListManagedPool pool = (SimpleListManagedPool)GameObjectPool.GetOrCreatePool(m_MockBaseObject,
                m_MockTransform);
            GameObject gameObject = GameObjectPool.Get(pool, false);
            pool.TearDown();

            bool evaluate = gameObject == null;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void OnDestroyItemAction_MockData_DestroysInterface()
        {
            SimpleListManagedPool pool = (SimpleListManagedPool)GameObjectPool.GetOrCreatePool(m_MockBaseObjectWithInterface,
                m_MockBaseObjectWithInterfaceTransform);
            GameObject gameObject = GameObjectPool.Get(pool, false);
            pool.TearDown();

            bool evaluate = gameObject == null;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Get_MockData_DontTrigger()
        {
            SimpleListManagedPool pool = (SimpleListManagedPool)GameObjectPool.GetOrCreatePool(m_MockBaseObjectWithInterface,
                m_MockBaseObjectWithInterfaceTransform);

            GameObject gameObject = GameObjectPool.Get(pool, false);

            bool evaluate = !gameObject.GetComponent<GameObjectPoolItemTest>().TriggeredOnSpawn;

            Assert.IsTrue(evaluate);

            pool.TearDown();
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Get_MockData_LocalPositionWorldLookAtParented()
        {
            SimpleListManagedPool pool = (SimpleListManagedPool)GameObjectPool.GetOrCreatePool(m_MockBaseObjectWithInterface,
                m_MockBaseObjectWithInterfaceTransform);

            GameObject gameObject = GameObjectPool.Get(pool, m_MockTransform, new Vector3(3f,0f,0f), Vector3.zero);


            bool evaluate = gameObject.transform.position == new Vector3(3, 0, 0) &&
                            gameObject.transform.eulerAngles.Approximately(
                                new Vector3(0f, 270f, 0f)) &&
                            gameObject.transform.parent == m_MockTransform;

            Assert.IsTrue(evaluate);

            pool.TearDown();
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Get_MockData_LocalPositionLocalRotationParented()
        {
            SimpleListManagedPool pool = (SimpleListManagedPool)GameObjectPool.GetOrCreatePool(m_MockBaseObjectWithInterface,
                m_MockBaseObjectWithInterfaceTransform);

            GameObject gameObject = GameObjectPool.Get(pool, m_MockTransform, new Vector3(3f,0f,0f), new Quaternion(0,90,0,0));


            bool evaluate = gameObject.transform.position == new Vector3(3, 0, 0) &&
                            gameObject.transform.eulerAngles.Approximately(
                                new Vector3(0f, 180f, 0f)) &&
                            gameObject.transform.parent == m_MockTransform;

            Assert.IsTrue(evaluate);

            pool.TearDown();
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Get_MockData_ParentedWorldPositionStay()
        {
            GameObject movedObject = new GameObject { transform = { position = new Vector3(1, 0, 0) } };
            GameObject movedTransform = new GameObject { transform = { position = new Vector3(10, 0, 0) } };

            SimpleListManagedPool pool = (SimpleListManagedPool)GameObjectPool.GetOrCreatePool(movedObject, movedTransform.transform);

            GameObject gameObject = GameObjectPool.Get(pool, m_MockTransform, true);

            bool evaluate = gameObject.transform.position == new Vector3(1,0,0);

            Assert.IsTrue(evaluate);

            pool.TearDown();
            Object.DestroyImmediate(movedObject);
            Object.DestroyImmediate(movedTransform);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Get_MockData_ParentedZeroLocalPosition()
        {
            GameObject movedObject = new GameObject { transform = { position = new Vector3(1, 0, 0) } };
            GameObject movedTransform = new GameObject { transform = { position = new Vector3(10, 0, 0) } };

            SimpleListManagedPool pool = (SimpleListManagedPool)GameObjectPool.GetOrCreatePool(movedObject, movedTransform.transform);

            GameObject gameObject = GameObjectPool.Get(pool, m_MockTransform);

            bool evaluate = gameObject.transform.position == m_MockTransform.transform.position;

            Assert.IsTrue(evaluate);

            pool.TearDown();
            Object.DestroyImmediate(movedObject);
            Object.DestroyImmediate(movedTransform);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Get_MockData_Trigger()
        {
            SimpleListManagedPool pool = (SimpleListManagedPool)GameObjectPool.GetOrCreatePool(m_MockBaseObjectWithInterface,
                m_MockBaseObjectWithInterfaceTransform);

            GameObject gameObject = GameObjectPool.Get(pool);

            bool evaluate = gameObject.GetComponent<GameObjectPoolItemTest>().TriggeredOnSpawn;

            Assert.IsTrue(evaluate);

            pool.TearDown();
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Get_MockData_WorldPositionWorldLookAt()
        {
            SimpleListManagedPool pool = (SimpleListManagedPool)GameObjectPool.GetOrCreatePool(m_MockBaseObjectWithInterface,
                m_MockBaseObjectWithInterfaceTransform);

            GameObject gameObject = GameObjectPool.Get(pool, new Vector3(2, 6, 10), new Vector3(0, 0, 0));

            bool evaluate = gameObject.transform.position == new Vector3(2, 6, 10) &&
                            gameObject.transform.eulerAngles.Approximately(
                                new Vector3(30.4703617f, 191.309921f, 2.47645289E-07f));

            Assert.IsTrue(evaluate);

            pool.TearDown();
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Get_MockData_WorldPositionWorldLookAtParented()
        {
            SimpleListManagedPool pool = (SimpleListManagedPool)GameObjectPool.GetOrCreatePool(m_MockBaseObjectWithInterface,
                m_MockBaseObjectWithInterfaceTransform);

            GameObject gameObject = GameObjectPool.Get(pool, new Vector3(2, 6, 10), new Vector3(0, 0, 0),
                m_MockBaseObjectWithInterfaceTransform);

            bool evaluate = gameObject.transform.position == new Vector3(2, 6, 10) &&
                            gameObject.transform.eulerAngles.Approximately(
                                new Vector3(30.4703617f, 191.309921f, 2.47645289E-07f)) &&
                            gameObject.transform.parent == m_MockBaseObjectWithInterfaceTransform;

            Assert.IsTrue(evaluate);

            pool.TearDown();
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Get_MockData_WorldPositionWorldRotation()
        {
            SimpleListManagedPool pool = (SimpleListManagedPool)GameObjectPool.GetOrCreatePool(m_MockBaseObjectWithInterface,
                m_MockBaseObjectWithInterfaceTransform);
            Quaternion mockRotation = new Quaternion(90f, 90f, 45f, 1f);

            GameObject gameObject = GameObjectPool.Get(pool, new Vector3(2, 6, 10), mockRotation);

            bool evaluate = gameObject.transform.position == new Vector3(2, 6, 10) &&
                            gameObject.transform.rotation == mockRotation;

            Assert.IsTrue(evaluate);

            pool.TearDown();
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Get_MockData_WorldPositionWorldRotationParented()
        {
            SimpleListManagedPool pool = (SimpleListManagedPool)GameObjectPool.GetOrCreatePool(m_MockBaseObjectWithInterface,
                m_MockBaseObjectWithInterfaceTransform);
            Quaternion mockRotation = new Quaternion(90f, 90f, 45f, 1f);

            GameObject gameObject = GameObjectPool.Get(pool, new Vector3(2, 6, 10), mockRotation,
                m_MockBaseObjectWithInterfaceTransform);

            bool evaluate = gameObject.transform.position == new Vector3(2, 6, 10) &&
                            gameObject.transform.rotation == mockRotation &&
                            gameObject.transform.parent == m_MockBaseObjectWithInterfaceTransform;

            Assert.IsTrue(evaluate);

            pool.TearDown();
        }


        [Test]
        [Category(Core.TestCategory)]
        public void GetOrCreatePool_MockData_ReturnsPool()
        {
            IManagedPool pool = GameObjectPool.GetOrCreatePool(m_MockBaseObject, m_MockTransform);

            Assert.IsNotNull(pool);

            pool.TearDown();
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TearDown_Pool_UnregistersPool()
        {
            IManagedPool pool = GameObjectPool.GetOrCreatePool(m_MockBaseObject, m_MockTransform);
            uint cachePoolKey = pool.GetKey();

            pool.TearDown();

            Assert.IsFalse(ManagedPools.HasPool(cachePoolKey));
        }

        class GameObjectPoolItemTest : MonoBehaviour, IGameObjectPoolItem
        {
            public bool TriggeredOnSpawn { get; private set; }
            IManagedPool m_ParentPool;

            /// <inheritdoc />
            public GameObject GetGameObject()
            {
                return gameObject;
            }

            /// <inheritdoc />
            public IManagedPool GetParentPool()
            {
                return m_ParentPool;
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
                TriggeredOnSpawn = true;
            }

            /// <inheritdoc />
            public void ReturnToPool()
            {
                m_ParentPool.Return(this);
            }

            /// <inheritdoc />
            public void SetParentPool(IManagedPool targetManagedPool)
            {
                m_ParentPool = targetManagedPool;
            }
        }
    }
}