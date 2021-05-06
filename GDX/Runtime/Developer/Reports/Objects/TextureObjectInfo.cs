// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace GDX.Developer.Reports.Objects
{
    public sealed class TextureObjectInfo : ObjectInfo
    {
        public const string TypeDefinition = "GDX.Developer.Reports.Objects.TextureObjectInfo,GDX";
#if UNITY_2019_1_OR_NEWER
        public GraphicsFormat Format;
#endif
        public int Height;

        public bool IsReadable;
        public int Width;

        public override void Populate(Object targetObject, TransientReference reference = null)
        {
            base.Populate(targetObject, reference);
            Texture textureAsset = (Texture)targetObject;

            // Useful texture information
            Width = textureAsset.width;
            Height = textureAsset.height;
            IsReadable = textureAsset.isReadable;
#if UNITY_2019_1_OR_NEWER
            Format = textureAsset.graphicsFormat;
#endif
        }

        public override string GetDetailedInformation(int maximumWidth)
        {

            // Always a width of 11
            string size = $"{Width.ToString()}x{Height.ToString()}".PadRight(11);

#if UNITY_2019_1_OR_NEWER
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
#else
            return $"{size} {(IsReadable ? "RW":"")}";
#endif
        }
    }
}