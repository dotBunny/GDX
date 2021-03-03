// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.Collections.Pooling
{
    public class PrefabPoolItem : MonoBehaviour, IPoolItem<GameObject>
    {
        private IPool _parent;

        /// <inheritdoc />
        public IPool GetParentPool()
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
            (_parent as PrefabPool)?.Pool(this);
        }

        /// <inheritdoc />
        public void SetParentPool(IPool targetPool)
        {
            _parent = targetPool;
        }
    }
}