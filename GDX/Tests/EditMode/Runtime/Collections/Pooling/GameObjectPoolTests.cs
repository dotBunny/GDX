// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using GDX.Collections.Pooling;
using NUnit.Framework;
using UnityEngine;

namespace Runtime.Collections.Pooling
{
    public class GameObjectPoolTests
    {
        private GameObject _mockBaseObject;
        private Transform _mockTransform;

        [SetUp]
        public void Setup()
        {
            _mockBaseObject = new GameObject();
            _mockTransform = new GameObject().transform;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_mockBaseObject);
            Object.DestroyImmediate(_mockTransform.gameObject);
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
