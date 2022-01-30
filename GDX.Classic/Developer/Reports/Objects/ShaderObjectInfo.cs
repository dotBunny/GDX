﻿// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.Classic.Developer.Reports.Objects
{
    public sealed class ShaderObjectInfo : ObjectInfo
    {
        public new const string TypeDefinition = "GDX.Developer.Reports.Objects.ShaderObjectInfo,GDX";
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
                CopyCount = this.CopyCount,
                MemoryUsage = this.MemoryUsage,
                Name = this.Name,
                Reference = this.Reference,
                TotalMemoryUsage = this.TotalMemoryUsage,
                Type = this.Type,
                IsSupported = this.IsSupported,
                PassCount = this.PassCount
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