// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace GDX.Editor.Inspectors
{
    // C# example:
// Automatically creates a game object with a primitive mesh renderer and appropriate collider.
    [CustomEditor(typeof(MeshColliderProxy))]
    public class MeshColliderProxyEditor : UnityEditor.Editor
    {
        public enum CompositeStrategy
        {
            /// <summary>
            ///     Only use cubes to generate the compound collider.
            /// </summary>
            Cubes = 0,

            /// <summary>
            ///     Only use spheres to generate the compound collider.
            /// </summary>
            Spheres = 1,

            /// <summary>
            ///     Use simple geometric shapes only to generate the compound collider.
            /// </summary>
            Simple = 2
            // /// <summary>
            // /// Can use geometric shapes and convex meshes to generate the compound collider.
            // /// </summary>
            // Advanced = 3
        }

        [PostProcessScene(2)]
        public static void OnPostProcessScene()
        {
            MeshColliderProxy[] proxies =
                FindObjectsByType<MeshColliderProxy>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            int foundProxiesCount = proxies.Length;
            for (int i = 0; i < foundProxiesCount; i++)
            {
                MeshColliderProxy proxy = proxies[i];
                if (proxy.enabled)
                {
                    if (proxy.TargetMeshCollider != null)
                    {
                        proxy.TargetMeshCollider.SafeDestroy();
                    }
                }

                // We never want a proxy to live in a build
                Collapse(proxy);
                proxy.SafeDestroy();
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate"))
            {
                MeshColliderProxy proxy = (MeshColliderProxy)target;
                Generate(proxy);
                EditorUtility.SetDirty(proxy.gameObject);
            }

            if (GUILayout.Button("Expand"))
            {
            }

            if (GUILayout.Button("Contract"))
            {
            }

            if (GUILayout.Button("Collapse"))
            {
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        public static void Collapse(MeshColliderProxy proxy)
        {
        }

        public static void Generate(MeshColliderProxy proxy)
        {
            // Remove previous
            if (proxy.GeneratedColliders != null)
            {
                int knownColliders = proxy.GeneratedColliders.Length;
                for (int i = 0; i < knownColliders; i++)
                {
                    proxy.GeneratedColliders[i].SafeDestroy();
                }

                proxy.GeneratedColliders = null;
            }

            // Find target mesh if needed
            if (proxy.TargetMeshCollider == null)
            {
                proxy.TargetMeshCollider = proxy.gameObject.GetComponent<MeshCollider>();
            }

            if (proxy.TargetMeshCollider != null)
            {
                proxy.GeneratedColliders = GenerateColliders(proxy.TargetMeshCollider);
            }

            EditorUtility.SetDirty(proxy);
        }

        public static Collider[] GenerateColliders(MeshCollider meshCollider,
            CompositeStrategy strategy = CompositeStrategy.Cubes)
        {
            Bounds existingBounds = meshCollider.bounds;


            List<Collider> generatedColliders = new List<Collider>(4);

            const int rayCount = 10000;


            return generatedColliders.ToArray();
        }
    }
}