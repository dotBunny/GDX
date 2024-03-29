﻿// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if !UNITY_DOTSRUNTIME

using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace GDX.Developer.Reports.Resource.Objects
{
    /// <exception cref="UnsupportedRuntimeException">Not supported on DOTS Runtime.</exception>
    public sealed class TextureObjectInfo : ObjectInfo
    {
        public new const string TypeDefinition = "GDX.Developer.Reports.Resource.Objects.TextureObjectInfo,GDX";
        public GraphicsFormat Format;
        public int Height;

        public bool IsReadable;
        public int Width;

        /// <summary>
        ///     Create a clone of this object.
        /// </summary>
        /// <returns></returns>
        public override ObjectInfo Clone()
        {
            return new TextureObjectInfo
            {
                CopyCount = CopyCount,
                MemoryUsage = MemoryUsage,
                Name = Name,
                Reference = Reference,
                TotalMemoryUsage = TotalMemoryUsage,
                Type = Type,
                Height = Height,
                IsReadable = IsReadable,
                Width = Width,
                Format = Format
            };
        }

        public override void Populate(Object targetObject, TransientReference reference = null)
        {
            base.Populate(targetObject, reference);
            Texture textureAsset = (Texture)targetObject;

            // Useful texture information
            Width = textureAsset.width;
            Height = textureAsset.height;
            IsReadable = textureAsset.isReadable;
            Format = textureAsset.graphicsFormat;
        }

        public override string GetDetailedInformation(int maximumWidth)
        {
            // Always a width of 11
            string size = $"{Width.ToString()}x{Height.ToString()}".PadRight(11);

            int formatWidth = maximumWidth - 15;
            string format = Format.ToString();
            if (format.IsNumeric())
            {
                format = $"Unknown ({format})";
            }

            format = format.PadRight(formatWidth);
            if (format.Length > formatWidth)
            {
                format = format.Substring(0, formatWidth);
            }

            return $"{size} {format} {(IsReadable ? "RW" : "")}";
        }
    }
}
#endif // !UNITY_DOTSRUNTIME