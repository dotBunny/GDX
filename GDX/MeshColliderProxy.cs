// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GDX
{
    public class MeshColliderProxy : MonoBehaviour
    {
        public enum CompositeStrategy
        {
             /// <summary>
             /// Only use cubes to generate the compound collider.
             /// </summary>
             Cubes = 0,
             /// <summary>
             /// Only use spheres to generate the compound collider.
             /// </summary>
             Spheres = 1,
             /// <summary>
             /// Use simple geometric shapes only to generate the compound collider.
             /// </summary>
             Simple = 2,
             // /// <summary>
             // /// Can use geometric shapes and convex meshes to generate the compound collider.
             // /// </summary>
             // Advanced = 3
        }

#if UNITY_EDITOR
        public MeshCollider TargetMeshCollider;
        public Collider[] GeneratedColliders;
#endif

        public void Generate()
        {
            // Remove previous
            if (GeneratedColliders != null)
            {
                int knownColliders = GeneratedColliders.Length;
                for (int i = 0; i < knownColliders; i++)
                {
                    GeneratedColliders[i].SafeDestroy();
                }
                GeneratedColliders = null;
            }

            if (TargetMeshCollider == null)
            {
                TargetMeshCollider = gameObject.GetComponent<MeshCollider>();
                EditorUtility.SetDirty(this);
            }

            if (TargetMeshCollider != null)
            {
                GeneratedColliders = Generate(TargetMeshCollider);
            }
        }

        public static Collider[] Generate(MeshCollider meshCollider, CompositeStrategy strategy = CompositeStrategy.Cubes)
        {
            Bounds existingBounds = meshCollider.bounds;
            List<Collider> generatedColliders = new List<Collider>(4);


            return generatedColliders.ToArray();
        }
    }
}