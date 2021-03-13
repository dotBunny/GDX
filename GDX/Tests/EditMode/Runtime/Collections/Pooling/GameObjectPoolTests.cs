// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using GDX.Collections.Pooling;
using NUnit.Framework;
using UnityEngine;

namespace Runtime.Collections.Pooling
{
    public class GameObjectPoolTests
    {
        private class GameObjectPoolItemTest : MonoBehaviour, IGameObjectPoolItem
        {
            /// <inheritdoc />
            public GameObject GetGameObject()
            {
                throw new System.NotImplementedException();
            }

            /// <inheritdoc />
            public IManagedPool GetParentPool()
            {
                throw new System.NotImplementedException();
            }

            /// <inheritdoc />
            public bool IsValidItem()
            {
                throw new System.NotImplementedException();
            }

            /// <inheritdoc />
            public void OnReturnedToPool()
            {
                throw new System.NotImplementedException();
            }

            /// <inheritdoc />
            public void OnSpawnedFromPool()
            {
                throw new System.NotImplementedException();
            }

            /// <inheritdoc />
            public void ReturnToPool()
            {
                throw new System.NotImplementedException();
            }

            /// <inheritdoc />
            public void SetParentPool(IManagedPool targetManagedPool)
            {
                throw new System.NotImplementedException();
            }
        }

        private GameObject _mockBaseObject;
        private GameObject _mockBaseObjectWithInterface;
        private Transform _mockTransform;
        private Transform _mockBaseObjectWithInterfaceTransform;

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
        [Category("GDX.Tests")]
        public void Get_MockData_DontTrigger()
        {
            IManagedPool pool = GameObjectPool.GetOrCreatePool(_mockBaseObjectWithInterface, _mockBaseObjectWithInterfaceTransform);

            pool.TearDown();
        }

        [Test]
        [Category("GDX.Tests")]
        public void Get_MockData_Trigger()
        {
            IManagedPool pool = GameObjectPool.GetOrCreatePool(_mockBaseObjectWithInterface, _mockBaseObjectWithInterfaceTransform);

            pool.TearDown();
        }

        [Test]
        [Category("GDX.Tests")]
        public void GetOrCreatePool_MockData_ReturnsPool()
        {
            IManagedPool pool = GameObjectPool.GetOrCreatePool(_mockBaseObject,_mockTransform);

            Assert.IsNotNull(pool);

            pool.TearDown();
        }

        [Test]
        [Category("GDX.Tests")]
        public void TearDown_Pool_UnregistersPool()
        {
            IManagedPool pool = GameObjectPool.GetOrCreatePool(_mockBaseObject,_mockTransform);
            uint cachePoolID = pool.GetKey();

            pool.TearDown();

            Assert.IsFalse(ManagedPools.HasPool(cachePoolID));
        }
    }
}
