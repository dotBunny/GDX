﻿// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.Inspectors
{
    [CustomEditor(typeof(CompoundCollider))]
    public class CapsuleColliderInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Expand"))
            {
                CompoundCollider collider = (CompoundCollider)serializedObject.targetObject;
                collider.Expand();
                EditorUtility.SetDirty(collider.gameObject);
            }

            if (GUILayout.Button("Contract"))
            {
                CompoundCollider collider = (CompoundCollider)serializedObject.targetObject;
                collider.Build();
                EditorUtility.SetDirty(collider.gameObject);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }
    }
}