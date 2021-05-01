// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.Developer.Reports
{
    public sealed class ShaderObjectInfo : ObjectInfo
    {
        public const string TypeDefinition = "GDX.Developer.Reports.ShaderObjectInfo, GDX";

#if UNITY_2019_1_OR_NEWER
        public int PassCount;
#endif
        public bool IsSupported;

        public override void Populate(UnityEngine.Object targetObject)
        {
            base.Populate(targetObject);
            Shader shaderAsset = (Shader)targetObject;

            // Useful shader information
#if UNITY_2019_1_OR_NEWER
            PassCount = shaderAsset.passCount;
#endif
            IsSupported = shaderAsset.isSupported;
        }
    }
}