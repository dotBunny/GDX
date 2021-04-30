// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace GDX.Developer.Reports
{
    public class TextureObjectInfo : ObjectInfo
    {
        public bool IsReadable;
        public int Height;
        public int Width;
        public GraphicsFormat Format;

        public override void Populate(UnityEngine.Object targetObject)
        {
            base.Populate(targetObject);
            Texture textureAsset = (Texture)targetObject;

            // Useful texture information
            Width = textureAsset.width;
            Height = textureAsset.height;
            IsReadable = textureAsset.isReadable;
            Format = textureAsset.graphicsFormat;
        }
    }
}