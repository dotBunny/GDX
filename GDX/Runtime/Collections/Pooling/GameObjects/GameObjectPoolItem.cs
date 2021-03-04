// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.Collections.Pooling.GameObjects
{
    /// <summary>
    ///     A simple <see cref="GameObject""/>
    /// </summary>
    public class GameObjectPoolItem : MonoBehaviour, IGameObjectPoolItem
    {
        private IObjectPool _parent;

        /// <inheritdoc />
        public GameObject GetGameObject()
        {
            return transform.gameObject;
        }

        /// <inheritdoc />
        public IObjectPool GetParentPool()
        {
            return _parent;
        }

        /// <inheritdoc />
        public bool IsValidItem()
        {
            return this != null && gameObject != null;
        }

        /// <inheritdoc />
        public void OnReturnedToPool()
        {
            gameObject.SetActive(false);
        }

        /// <inheritdoc />
        public void OnSpawnedFromPool()
        {
            gameObject.SetActive(true);
        }

        /// <inheritdoc />
        public void ReturnToPool()
        {
            _parent.Pool(this);
        }

        /// <inheritdoc />
        public void SetParentPool(IObjectPool targetObjectPool)
        {
            _parent = targetObjectPool;
        }
    }
}