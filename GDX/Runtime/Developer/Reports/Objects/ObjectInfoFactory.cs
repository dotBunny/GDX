﻿// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using UnityEngine;

namespace GDX.Developer.Reports.Objects
{
    public static class ObjectInfoFactory
    {
        public static ObjectInfo GetObjectInfo(Type targetType)
        {
            if (targetType == typeof(Texture2D) ||
                targetType == typeof(Texture3D) ||
                targetType == typeof(Texture2DArray) ||
                targetType == typeof(RenderTexture) ||
                targetType == typeof(Cubemap) ||
                targetType == typeof(CubemapArray))
            {
                return new TextureObjectInfo();
            }

            if (targetType == typeof(Mesh))
            {
                return new MeshObjectInfo();
            }

            if (targetType == typeof(Shader))
            {
                return new ShaderObjectInfo();
            }

            if (targetType == typeof(AssetBundle))
            {
                return new AssetBundleObjectInfo();
            }

            return new ObjectInfo();
        }
        public static Type GetObjectInfoType(Type targetType)
        {
            if (targetType == typeof(Texture2D) ||
                targetType == typeof(Texture3D) ||
                targetType == typeof(Texture2DArray) ||
                targetType == typeof(RenderTexture) ||
                targetType == typeof(Cubemap) ||
                targetType == typeof(CubemapArray))
            {
                return typeof(TextureObjectInfo);
            }

            if (targetType == typeof(Mesh))
            {
                return typeof(MeshObjectInfo);
            }

            if (targetType == typeof(Shader))
            {
                return typeof(ShaderObjectInfo);
            }

            if (targetType == typeof(AssetBundle))
            {
                return typeof(AssetBundleObjectInfo);
            }

            return typeof(ObjectInfo);
        }
    }
}