// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Collections.Generic;

namespace GDX.Developer
{
    /// <summary>
    ///     A debug draw system useful in both the editor and at runtime.
    /// </summary>
    public static class DebugDraw
    {
        // /// <summary>
        // ///     A dictionary of known <see cref="DebugDrawBuffer"/> and their ID which are automatically executed.
        // /// </summary>
        // static IntKeyDictionary<DebugDrawBuffer> s_ManagedBuffers = new IntKeyDictionary<DebugDrawBuffer>(10);

        /// <summary>
        ///     A dictionary of known <see cref="DebugDrawBuffer"/> and their ID which are not automatically executed.
        /// </summary>
        static IntKeyDictionary<DebugDrawBuffer> s_UnmanagedBuffers = new IntKeyDictionary<DebugDrawBuffer>(10);

        /// <summary>
        /// Get an instance of <see cref="DebugDrawBuffer"/> based on the provided <paramref name="key"/> that is not managed.
        /// </summary>
        /// <example>
        ///     By checking the <see cref="DebugDrawBuffer.Finalized"/> property we can skip over the expensive building step.
        ///     <code>
        ///         DebugDrawBuffer buffer = DebugDraw.GetUnmanagedBuffer(gameObjet.GetInstanceID());
        ///         if (!buffer.Finalized)
        ///         {
        ///             /// Draw lots of stuff ...
        ///             buffer.DrawWireCube(Color.white, worldPosition, size);
        ///         }
        ///         buffer.Execute();
        ///     </code>
        /// </example>
        /// <param name="key">
        ///     A value based key used to reference a <see cref="DebugDrawBuffer"/> in a
        ///     <see cref="IntKeyDictionary{TValue}"/>.
        /// </param>
        /// <param name="initialColorCount">Initial number of internal materials to allocate (x2).</param>
        /// <param name="verticesPerMesh">The number of vertices to split batched meshes on.</param>
        /// <returns>
        ///     A newly created <see cref="DebugDrawBuffer"/> if the provided key is not found, or the previously
        ///     created <see cref="DebugDrawBuffer"/> identified by the <paramref name="key"/>.
        /// </returns>
        public static DebugDrawBuffer GetUnmanagedBuffer(int key, int initialColorCount = 5,
            int verticesPerMesh = DebugDrawBuffer.DefaultMaximumVerticesPerMesh)
        {
            if (s_UnmanagedBuffers.ContainsKey(key))
            {
                return s_UnmanagedBuffers[key];
            }

            DebugDrawBuffer newBuffer = new DebugDrawBuffer(key, initialColorCount, verticesPerMesh);
            s_UnmanagedBuffers.AddWithExpandCheck(key, newBuffer);
            return newBuffer;
        }

        /// <summary>
        ///     Returns if the provided key has an unmanaged <see cref="DebugDrawBuffer"/> referenced.
        /// </summary>
        /// <param name="key">
        ///     The key used to reference the <see cref="DebugDrawBuffer"/>.
        /// </param>
        /// <returns>true/false if the key has a <see cref="DebugDrawBuffer"/> associated with it.</returns>
        public static bool HasUnmanagedBuffer(int key)
        {
            return s_UnmanagedBuffers.ContainsKey(key);
        }

        /// <summary>
        ///     Dereference the indicated unmanaged <see cref="DebugDrawBuffer"/> from the provider.
        /// </summary>
        /// <param name="key">
        ///     The key used to reference the <see cref="DebugDrawBuffer"/>.
        /// </param>
        public static void RemoveUnmanagedBuffer(int key)
        {
            s_UnmanagedBuffers.TryRemove(key);
        }

        // public static void Render()
        // {
        //     int currentIndex = 0;
        //     while (s_ManagedBuffers.MoveNext(ref currentIndex))
        //     {
        //         DebugDrawBuffer buffer = s_ManagedBuffers.Entries[currentIndex - 1].Value;
        //         buffer.Execute();
        //     }
        // }
    }
}