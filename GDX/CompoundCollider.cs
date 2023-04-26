// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using GDX.Collections.Generic;
using UnityEngine;

namespace GDX
{
    public class CompoundCollider : MonoBehaviour
    {

        public const string GameObjectName = "_CompoundCollider";

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
            /// <summary>
            /// Can use geometric shapes and convex meshes to generate the compound collider.
            /// </summary>
            Advanced = 3
        }
      //  public Collider[] Colliders = Array.Empty<Collider>();

        public List<Collider> Colliders = new List<Collider>();
        public SerializableDictionary<Collider, Collider> Mappings = new SerializableDictionary<Collider, Collider>();

        public void Expand()
        {

        }

        public void Build()
        {

        }
        public static CompoundCollider Create(GameObject targetGameObject, CompositeStrategy strategy = CompositeStrategy.Cubes,
            bool useChildGameObject = true, bool collectColliders = true)
        {
            // Determine base object, all created colliders will be attached to this object
            GameObject baseGameObject = null;
            if (useChildGameObject)
            {
                int childCount = targetGameObject.transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    Transform child = targetGameObject.transform.GetChild(i);
                    if (child.name == GameObjectName)
                    {
                        baseGameObject = child.gameObject;
                        break;
                    }
                }

                if (baseGameObject == null)
                {
                    baseGameObject = new GameObject(GameObjectName);
                    baseGameObject.transform.SetParent(targetGameObject.transform, false);
                    baseGameObject.transform.localPosition = Vector3.zero;
                    baseGameObject.transform.localRotation = Quaternion.identity;
                }
            }
            else
            {
                baseGameObject = targetGameObject;
                // TODO: Do we need to zero this?
            }

            // Link our compound collider script
            CompoundCollider compoundCollider = baseGameObject.GetOrAddComponent<CompoundCollider>();


            Collider[] childrenColliders = targetGameObject.GetComponentsInChildren<Collider>(false);
            int childrenColliderCount = childrenColliders.Length;

            if (collectColliders)
            {
                for (int i = 0; i < childrenColliderCount; i++)
                {
                    Collider collider = childrenColliders[i];
                    if (compoundCollider.Mappings.TryGetValue(collider, out Collider mapping) || compoundCollider.Colliders.Contains(collider))
                    {
                        collider.Copy(mapping);
                        continue;
                    }

                    if (collider is BoxCollider boxSource)
                    {
                        BoxCollider boxCollider = baseGameObject.AddComponent<BoxCollider>();
                        boxSource.Copy(boxCollider);
                        compoundCollider.Mappings.Add(collider, boxCollider);
                        compoundCollider.Colliders.Add(boxCollider);
                        collider.enabled = false;
                    }
                    else if (collider is SphereCollider sphereSource)
                    {
                        SphereCollider sphereCollider = baseGameObject.AddComponent<SphereCollider>();
                        sphereSource.Copy(sphereCollider);
                        compoundCollider.Mappings.Add(collider, sphereCollider);
                        compoundCollider.Colliders.Add(sphereCollider);
                        collider.enabled = false;
                    }
                    else if (collider is CapsuleCollider capsuleSource)
                    {
                        CapsuleCollider capsuleCollider = baseGameObject.AddComponent<CapsuleCollider>();
                        capsuleSource.Copy(capsuleCollider);
                        compoundCollider.Mappings.Add(collider, capsuleCollider);
                        compoundCollider.Colliders.Add(capsuleCollider);
                        collider.enabled = false;
                    }
                }
            }

            for (int i = 0; i < childrenColliderCount; i++)
            {
                Collider collider = childrenColliders[i];
                if (!(collider is MeshCollider meshCollider) || compoundCollider.Mappings.ContainsKey(meshCollider))
                {
                    continue;
                }


                // TODO: Build idea of the bounds of the meshcollider?

            }

            return compoundCollider;
        }
    }
}