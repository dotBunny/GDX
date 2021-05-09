// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.Developer.Reports.Objects
{
    public sealed class ShaderObjectInfo : ObjectInfo
    {
        public new const string TypeDefinition = "GDX.Developer.Reports.Objects.ShaderObjectInfo,GDX";
        public bool IsSupported;

#if UNITY_2019_1_OR_NEWER
        public int PassCount;
#endif

        /// <summary>
        /// Create a clone of this object.
        /// </summary>
        /// <returns></returns>
        public override ObjectInfo Clone()
        {
            return new ShaderObjectInfo()
            {
                CopyCount = this.CopyCount,
                MemoryUsage = this.MemoryUsage,
                Name = this.Name,
                Reference = this.Reference,
                TotalMemoryUsage = this.TotalMemoryUsage,
                Type = this.Type,
                IsSupported = this.IsSupported,
#if UNITY_2019_1_OR_NEWER
                PassCount = this.PassCount
#endif
            };
        }

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

#if UNITY_2019_1_OR_NEWER
        /// <inheritdoc />
        public override string GetDetailedInformation(int maximumWidth)
        {
            return $"Passes:{PassCount}";
        }
#endif
    }
}