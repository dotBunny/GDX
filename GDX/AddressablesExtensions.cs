// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if GDX_ADDRESSABLES && !UNITY_DOTSRUNTIME

using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// ReSharper disable CommentTypo, IdentifierTypo
namespace GDX
{
    /// <summary>
    ///     Addressables Based Extension Methods
    /// </summary>
    /// <remarks>
    ///     <para>Requires UnityEngine.CoreModule.dll to function correctly.</para>
    ///     <para>Requires <c>com.unity.addressables</c> Package.</para>
    /// </remarks>
    [VisualScriptingCompatible(2)]
    public static class AddressablesExtensions
    {
        /// <summary>
        ///     An empty instance of an <see cref="UnityEngine.AddressableAssets.AssetReference" /> to be used for comparison.
        /// </summary>
        static readonly AssetReference k_EmptyAssetReference = new AssetReference();

        /// <summary>
        ///     <para>Can <paramref name="targetAssetReference" /> be instantiated at runtime?</para>
        /// </summary>
        /// <remarks>Checks that it is not empty, has a runtime key, and makes sure the key is valid.</remarks>
        /// <param name="targetAssetReference">The target <see cref="UnityEngine.AddressableAssets.AssetReference" />.</param>
        /// <returns>true/false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CanInstantiate(this AssetReference targetAssetReference)
        {
            return !IsEmpty(targetAssetReference) && HasRuntimeKey(targetAssetReference) &&
                   targetAssetReference.RuntimeKeyIsValid();
        }

        /// <summary>
        ///     Can the <paramref name="targetAsyncOperationHandle" /> be released?
        /// </summary>
        /// <param name="targetAsyncOperationHandle">
        ///     A target <see cref="UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle{T}" />
        ///     typed as <see cref="UnityEngine.GameObject" />.
        /// </param>
        /// <param name="autoRelease">If it can, should the handle release?</param>
        /// <returns>true/false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CanRelease(this AsyncOperationHandle<GameObject> targetAsyncOperationHandle,
            bool autoRelease = false)
        {
            if (!targetAsyncOperationHandle.IsValid())
            {
                return false;
            }

            if (targetAsyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
            {
                if (autoRelease)
                {
                    // Not entirely sure about this one, as it makes sense that we should be moving to a instanced version,
                    // however games have shipped with the older method (.Release)
                    Addressables.ReleaseInstance(targetAsyncOperationHandle);
                }

                return true;
            }

            if (targetAsyncOperationHandle.Result == null)
            {
                return false;
            }

            if (autoRelease)
            {
                Addressables.ReleaseInstance(targetAsyncOperationHandle);
            }

            return true;
        }

        /// <summary>
        ///     <para>Does <paramref name="targetAssetReference" /> have a runtime key?</para>
        /// </summary>
        /// <remarks>Will return false if the reference is <see langword="null" />.</remarks>
        /// <param name="targetAssetReference">The target <see cref="UnityEngine.AddressableAssets.AssetReference" />.</param>
        /// <returns>true/false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasRuntimeKey(this AssetReference targetAssetReference)
        {
            if (targetAssetReference == null)
            {
                return false;
            }

            return !string.IsNullOrEmpty((string)targetAssetReference.RuntimeKey);
        }

        /// <summary>
        ///     Is <paramref name="targetAssetReference" /> empty?
        /// </summary>
        /// <param name="targetAssetReference">The target <see cref="UnityEngine.AddressableAssets.AssetReference" />.</param>
        /// <returns>true/false</returns>
        public static bool IsEmpty(this AssetReference targetAssetReference)
        {
            return targetAssetReference.AssetGUID == k_EmptyAssetReference.AssetGUID;
        }
    }
}
#endif // GDX_ADDRESSABLES && !UNITY_DOTSRUNTIME