// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.Developer.Reports.Objects
{
    public sealed class AssetBundleObjectInfo : ObjectInfo
    {
        public const string TypeDefinition = "GDX.Developer.Reports.Objects.AssetBundleObjectInfo,GDX";

        public bool IsStreamedSceneAssetBundle;
        public int AssetCount;

        public override void Populate(Object targetObject, TransientReference reference = null)
        {
            base.Populate(targetObject, reference);
            AssetBundle assetBundle = (AssetBundle)targetObject;
            IsStreamedSceneAssetBundle = assetBundle.isStreamedSceneAssetBundle;
            AssetCount = assetBundle.GetAllAssetNames().Length;
        }

        public override string GetDetailedInformation(int maximumWidth)
        {
            return $"Asset Count:{AssetCount.ToString()}";
        }
    }
}