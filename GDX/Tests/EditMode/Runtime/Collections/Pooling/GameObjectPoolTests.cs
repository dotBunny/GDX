// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using GDX.Collections.Pooling;
using NUnit.Framework;
using UnityEngine;

namespace Runtime.Collections.Pooling
{
    public class GameObjectPoolTests
    {
        private GameObject _baseObject;
        private Transform _parentTransform;

        [SetUp]
        public void Setup()
        {
            _baseObject = new GameObject();
            _parentTransform = new GameObject().transform;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_baseObject);
            Object.DestroyImmediate(_parentTransform.gameObject);
        }


        [Test]
        [Category("GDX.Tests")]
        public void True_GetOrCreatePool_SimpleValidation()
        {
            // Make pool
            IManagedPool pool = GameObjectPool.GetOrCreatePool(_baseObject,_parentTransform);
            uint cachePoolID = pool.GetKey();

            Assert.IsNotNull(pool, "Expected to have a pool");

            // Destroy pool
            pool.TearDown();

            Assert.IsFalse(ManagedPools.HasPool(cachePoolID), "Should not have found a pool at ID");
        }
    }
}
