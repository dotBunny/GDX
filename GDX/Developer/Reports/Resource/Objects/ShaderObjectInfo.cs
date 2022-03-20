// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if !UNITY_DOTSRUNTIME

using UnityEngine;

namespace GDX.Developer.Reports.Resource.Objects
{
    /// <exception cref="UnsupportedRuntimeException">Not supported on DOTS Runtime.</exception>
    public sealed class ShaderObjectInfo : ObjectInfo
    {
        public new const string TypeDefinition = "GDX.Developer.Reports.Resource.Objects.ShaderObjectInfo,GDX";
        public bool IsSupported;
        public int PassCount;


        /// <summary>
        /// Create a clone of this object.
        /// </summary>
        /// <returns></returns>
        public override ObjectInfo Clone()
        {
            return new ShaderObjectInfo()
            {
                CopyCount = CopyCount,
                MemoryUsage = MemoryUsage,
                Name = Name,
                Reference = Reference,
                TotalMemoryUsage = TotalMemoryUsage,
                Type = Type,
                IsSupported = IsSupported,
                PassCount = PassCount
            };
        }

        public override void Populate(Object targetObject, TransientReference reference = null)
        {
            base.Populate(targetObject, reference);
            Shader shaderAsset = (Shader)targetObject;

            // Useful shader information
            PassCount = shaderAsset.passCount;
            IsSupported = shaderAsset.isSupported;
        }
        /// <inheritdoc />
        public override string GetDetailedInformation(int maximumWidth)
        {
            return $"Passes:{PassCount}";
        }
    }
}
#endif // !UNITY_DOTSRUNTIME