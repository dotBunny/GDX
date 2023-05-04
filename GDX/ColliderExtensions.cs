// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX
{
    // TODO: handle scale
    // better descriptions/ cant be rotated?
    public static class ColliderExtensions
    {
        public const string CreatedGameObjectName = "Collider_";

        static void BaseCopy(Collider sourceCollider, Collider destinationCollider)
        {
            destinationCollider.contactOffset = sourceCollider.contactOffset;
            destinationCollider.isTrigger = sourceCollider.isTrigger;
            destinationCollider.sharedMaterial = sourceCollider.sharedMaterial;
        }

        public static void Copy(this Collider sourceCollider, Collider destinationCollider, bool keepWorldPosition = true)
        {
            if (sourceCollider is BoxCollider sourceBoxCollider &&
                destinationCollider is BoxCollider destinationBoxCollider)
            {
                sourceBoxCollider.Copy(destinationBoxCollider, keepWorldPosition);
            }
            else if (sourceCollider is SphereCollider sourceSphereCollider &&
                     destinationCollider is SphereCollider destinationSphereCollider)
            {
                sourceSphereCollider.Copy(destinationSphereCollider, keepWorldPosition);
            }
            else if (sourceCollider is CapsuleCollider sourceCapsuleCollider &&
                     destinationCollider is CapsuleCollider destinationCapsuleCollider)
            {
                sourceCapsuleCollider.Copy(destinationCapsuleCollider, keepWorldPosition);
            }
            else if (sourceCollider is MeshCollider sourceMeshCollider &&
                     destinationCollider is MeshCollider destinationMeshCollider)
            {
                sourceMeshCollider.Copy(destinationMeshCollider, keepWorldPosition);
            }
            else
            {
                BaseCopy(sourceCollider, destinationCollider);
            }
        }
        public static void Copy(this BoxCollider sourceCollider, BoxCollider destinationCollider, bool keepWorldPosition = true)
        {
            BaseCopy(sourceCollider, destinationCollider);

            if (keepWorldPosition)
            {
                destinationCollider.center = (sourceCollider.transform.position - destinationCollider.transform.position) + sourceCollider.center;
            }
            else
            {
                destinationCollider.center = sourceCollider.center;
            }

            destinationCollider.size = sourceCollider.size;
        }
        public static void Copy(this SphereCollider sourceCollider, SphereCollider destinationCollider, bool keepWorldPosition = true)
        {
            BaseCopy(sourceCollider, destinationCollider);

            if (keepWorldPosition)
            {
                destinationCollider.center = (sourceCollider.transform.position - destinationCollider.transform.position) + sourceCollider.center;
            }
            else
            {
                destinationCollider.center = sourceCollider.center;
            }
            destinationCollider.radius = sourceCollider.radius;
        }
        public static void Copy(this CapsuleCollider sourceCollider, CapsuleCollider destinationCollider, bool keepWorldPosition = true)
        {
            BaseCopy(sourceCollider, destinationCollider);

            if (keepWorldPosition)
            {
                destinationCollider.center = (sourceCollider.transform.position - destinationCollider.transform.position) + sourceCollider.center;
            }
            else
            {
                destinationCollider.center = sourceCollider.center;
            }
            destinationCollider.radius = sourceCollider.radius;
            destinationCollider.direction = sourceCollider.direction;
            destinationCollider.height = sourceCollider.height;
        }

        public static void Copy(this MeshCollider sourceCollider, MeshCollider destinationCollider, bool keepWorldPosition = true)
        {
            BaseCopy(sourceCollider, destinationCollider);

            destinationCollider.convex = sourceCollider.convex;
            destinationCollider.cookingOptions = sourceCollider.cookingOptions;
            destinationCollider.sharedMaterial = sourceCollider.sharedMaterial;
        }
    }
}