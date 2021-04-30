// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.Developer.Reports
{
    public class ShaderObjectInfo : ObjectInfo
    {
        public int PassCount;
        public bool IsSupported;

        public override void Populate(UnityEngine.Object targetObject)
        {
            base.Populate(targetObject);
            Shader shaderAsset = (Shader)targetObject;

            // Useful shader information
            PassCount = shaderAsset.passCount;
            IsSupported = shaderAsset.isSupported;
        }
    }
}