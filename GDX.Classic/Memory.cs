﻿// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global

namespace GDX.Classic
{
    /// <summary>
    ///     A collection of memory related helper utilities.
    /// </summary>
    /// <remarks>Requires UnityEngine.CoreModule.dll to function correctly.</remarks>
    [VisualScriptingCompatible(8)]
    public static class Memory
    {
        /// <summary>
        ///     <para>Cleanup Memory</para>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>Mono Heap (Garbage Collection)</description>
        ///         </item>
        ///         <item>
        ///             <description>Unity Resources</description>
        ///         </item>
        ///     </list>
        /// </summary>
        /// <remarks>
        ///     <para>Requires UnityEngine.CoreModule.dll to function correctly.</para>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CleanUp()
        {
            // Fire off a first pass collection
            GC.Collect();

            // Tell Unity to clean up any assets that it no longer wants to have loaded
            Resources.UnloadUnusedAssets();

            // Fire off second pass collection
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        /// <inheritdoc cref="CleanUp" />
        /// <remarks>Requires UnityEngine.CoreModule.dll to function correctly.</remarks>
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
    }
}