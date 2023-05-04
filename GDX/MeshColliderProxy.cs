// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX
{
#if UNITY_EDITOR
    public class MeshColliderProxy : MonoBehaviour
    {
        public MeshCollider TargetMeshCollider;
        public Collider[] GeneratedColliders;
        public GameObject ProxyGameObject;
    }
#endif
}