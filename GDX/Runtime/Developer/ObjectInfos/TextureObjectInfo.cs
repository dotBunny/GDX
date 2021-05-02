// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace GDX.Developer.ObjectInfos
{
    public sealed class TextureObjectInfo : ObjectInfo
    {
        public const string TypeDefinition = "GDX.Developer.ObjectInfos.TextureObjectInfo,GDX";
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

#if UNITY_2019_1_OR_NEWER
        public override string GetDetailedInformation()
        {
            string format = Format.ToString();
            if (format.IsNumeric())
            {
                format = $"Unknown ({format})";
            }

            return $"{Width.ToString()}x{Height.ToString()} {format} R: {IsReadable.ToString()}";
        }
#else
        public override string GetDetailedInformation()
        {
            return $"{Width.ToString()}x{Height.ToString()} R: {IsReadable.ToString()}";
        }
#endif

    }
}