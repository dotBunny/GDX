// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.Classic.Developer.Reports.Objects
{
    public sealed class AssetBundleObjectInfo : ObjectInfo
    {
        public new const string TypeDefinition = "GDX.Developer.Reports.Objects.AssetBundleObjectInfo,GDX";

        public bool IsStreamedSceneAssetBundle;
        public int AssetCount;

        /// <summary>
        /// Create a clone of this object.
        /// </summary>
        /// <returns></returns>
        public override ObjectInfo Clone()
        {
            return new AssetBundleObjectInfo()
            {
                CopyCount = this.CopyCount,
                MemoryUsage = this.MemoryUsage,
                Name = this.Name,
                Reference = this.Reference,
                TotalMemoryUsage = this.TotalMemoryUsage,
                Type = this.Type,
                IsStreamedSceneAssetBundle =  this.IsStreamedSceneAssetBundle,
                AssetCount = this.AssetCount
            };
        }

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