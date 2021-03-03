// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.Collections.Pooling.GameObjects
{
    public class GameObjectObjectPoolItem : MonoBehaviour, IObjectPoolItem
    {
        private IObjectPool _parent;

        /// <inheritdoc />
        public IObjectPool GetParentPool()
        {
            return _parent;
        }

        public GameObject GetSelf()
        {
            return gameObject;
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
            (_parent as GameObjectPool)?.Pool(this);
        }

        /// <inheritdoc />
        public void SetParentPool(IObjectPool targetObjectPool)
        {
            _parent = targetObjectPool;
        }
    }
}