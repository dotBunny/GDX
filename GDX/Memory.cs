// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

// ReSharper disable UnusedMember.Global, MemberCanBePrivate.Global

namespace GDX
{
    /// <summary>
    ///     A collection of memory related helper utilities.
    /// </summary>
    [VisualScriptingCompatible(8)]
    // ReSharper disable once UnusedType.Global
    public static class Memory
    {
        /// <summary>
        ///     <para>Cleanup Memory</para>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>Mono Heap (Garbage Collection)</description>
        ///         </item>
        ///         <item>
        ///             <description>Unity Resources (when not deployed on DOTS Runtime.</description>
        ///         </item>
        ///     </list>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CleanUp()
        {
            // Fire off a first pass collection
            GC.Collect();

            // Tell Unity to clean up any assets that it no longer wants to have loaded
#if !UNITY_DOTSRUNTIME
            Resources.UnloadUnusedAssets();
#endif

            // Fire off second pass collection
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

#if !UNITY_DOTSRUNTIME
        /// <inheritdoc cref="CleanUp" />
        /// <exception cref="UnsupportedRuntimeException">Not supported on DOTS Runtime.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async void CleanUpAsync()
        {
            // Fire off a first pass collection
            GC.Collect();

            // Tell Unity to clean up any assets that it no longer wants to have loaded
            AsyncOperation handle = Resources.UnloadUnusedAssets();
            while (!handle.isDone)
            {
                await Task.Delay(1000);
            }

            // Fire off second pass collection
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
#endif // !UNITY_DOTSRUNTIME
    }
}