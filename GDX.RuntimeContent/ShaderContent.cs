// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.RuntimeContent
{
    [CreateAssetMenu]
    public class ShaderContent : ScriptableObject
    {
        public static readonly int ColorPropertyID = Shader.PropertyToID("_Color");
        
        public Shader DottedLine;
        public Shader UnlitColor;
    }
}