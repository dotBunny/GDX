// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
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
        public Collider[] Colliders = Array.Empty<Collider>();

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
                    baseGameObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                }
            }
            else
            {
                baseGameObject = targetGameObject;
                // TODO: Do we need to zero this?
            }

            // Link our compound collider script
            CompoundCollider compoundCollider = baseGameObject.GetOrAddComponent<CompoundCollider>();


            if (collectColliders)
            {
                Collider[] childrenColliders = targetGameObject.GetComponentsInChildren<Collider>(false);
                int childrenColliderCount = childrenColliders.Length;
                for (int i = 0; i < childrenColliderCount; i++)
                {
                  //  childrenColliders[i]
                }
            }


            return compoundCollider;
        }
    }
}