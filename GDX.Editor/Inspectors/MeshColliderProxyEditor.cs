// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.Inspectors
{
    [CustomEditor(typeof(MeshColliderProxy))]
    public class MeshColliderProxyEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate"))
            {
                MeshColliderProxy proxy = (MeshColliderProxy)target;
                proxy.Generate();
                EditorUtility.SetDirty(proxy.gameObject);
            }
            if (GUILayout.Button("Expand"))
            {

            }
            if (GUILayout.Button("Contract"))
            {

            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }
    }
}