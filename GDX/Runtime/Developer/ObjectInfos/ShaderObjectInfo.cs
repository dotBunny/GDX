// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.Developer.ObjectInfos
{
    public sealed class ShaderObjectInfo : ObjectInfo
    {
        public const string TypeDefinition = "GDX.Developer.ObjectInfos.ShaderObjectInfo,GDX";
        public bool IsSupported;

#if UNITY_2019_1_OR_NEWER
        public int PassCount;
#endif

        public override void Populate(Object targetObject, TransientReference reference = null)
        {
            base.Populate(targetObject, reference);
            Shader shaderAsset = (Shader)targetObject;

            // Useful shader information
#if UNITY_2019_1_OR_NEWER
            PassCount = shaderAsset.passCount;
#endif
            IsSupported = shaderAsset.isSupported;
        }
    }
}