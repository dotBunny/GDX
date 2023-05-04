// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace GDX.Editor.Inspectors
{
    [CustomEditor(typeof(MeshCollider))]
    public class MeshColliderEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement baseElement = serializedObject.GetDefaultInspector();
            MeshCollider meshCollider = (MeshCollider)target;

            if (!meshCollider.convex && meshCollider.GetComponent<MeshColliderProxy>() == null)
            {
                GameObject meshObject = meshCollider.gameObject;
                baseElement.Insert(0,UIElementsProvider.ActionableHelpBox(
                    "Non-convex MeshColliders are sub-optimal.\nPlease consider manually building a similar shape with primitives, use of a convex hull alternative, or utilize the MeshColliderProxy component.",
                    HelpBoxMessageType.Warning, "Fix", () =>
                    {
                        meshObject.AddComponent<MeshColliderProxy>();
                    }));
            }

            return baseElement;
        }
    }
}