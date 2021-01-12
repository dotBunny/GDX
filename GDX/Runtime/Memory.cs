// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global

namespace GDX
{
    /// <summary>
    ///     A collection of memory related helper utilities.
    /// </summary>
    /// <remarks>Requires UnityEngine.CoreModule.dll to function correctly.</remarks>
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
        /// <remarks>Requires UnityEngine.CoreModule.dll to function correctly.</remarks>
        public static void CleanUp()
        {
            // Fire off a first pass collection
            GC.Collect();

            // Tell Unity to clean up any assets that it no longer wants to have loaded
            while (!Resources.UnloadUnusedAssets().isDone)
            {
            }

            // Fire off second pass collection
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        /// <inheritdoc cref="CleanUp" />
        /// <remarks>Requires UnityEngine.CoreModule.dll to function correctly.</remarks>
        public static async void CleanUpAsync()
        {
            // ReSharper disable once HeapView.DelegateAllocation
            await Task.Run(CleanUp);
        }
    }
}