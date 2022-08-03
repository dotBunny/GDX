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
        /// <summary>
        ///     A dictionary of known <see cref="DebugLineBuffer"/> and their ID.
        /// </summary>
        static IntKeyDictionary<DebugLineBuffer> s_LineBuffers = new IntKeyDictionary<DebugLineBuffer>(10);

        /// <summary>
        /// Get an instance of <see cref="DebugLineBuffer"/> based on the provided <paramref name="key"/>.
        /// </summary>
        /// <example>
        ///     By checking the <see cref="DebugLineBuffer.Finalized"/> property we can skip over the expensive building step.
        ///     <code>
        ///         DebugLineBuffer buffer = DebugDraw.GetLineBuffer(gameObjet.GetInstanceID());
        ///         if (!buffer.Finalized)
        ///         {
        ///             /// Draw lots of stuff ...
        ///             buffer.DrawWireCube(Color.white, worldPosition, size);
        ///         }
        ///         buffer.Execute();
        ///     </code>
        /// </example>
        /// <param name="key">
        ///     A value based key used to reference a <see cref="DebugLineBuffer"/> in a
        ///     <see cref="IntKeyDictionary{TValue}"/>.
        /// </param>
        /// <param name="initialColorCount">Initial number of internal materials to allocate (x2).</param>
        /// <param name="verticesPerMesh">The number of vertices to split batched meshes on.</param>
        /// <returns>
        ///     A newly created <see cref="DebugLineBuffer"/> if the provided key is not found, or the previously
        ///     created <see cref="DebugLineBuffer"/> identified by the <paramref name="key"/>.
        /// </returns>
        public static DebugLineBuffer GetLineBuffer(int key, int initialColorCount = 5,
            int verticesPerMesh = DebugLineBuffer.DefaultMaximumVerticesPerMesh)
        {
            if (s_LineBuffers.ContainsKey(key))
            {
                return s_LineBuffers[key];
            }

            DebugLineBuffer newBuffer = new DebugLineBuffer(key, initialColorCount, verticesPerMesh);
            s_LineBuffers.AddWithExpandCheck(key, newBuffer);
            return newBuffer;
        }

        /// <summary>
        ///     Returns if the provided key has a <see cref="DebugLineBuffer"/> referenced.
        /// </summary>
        /// <param name="key">
        ///     The key used to reference the <see cref="DebugLineBuffer"/>.
        /// </param>
        /// <returns>true/false if the key has a <see cref="DebugLineBuffer"/> associated with it.</returns>
        public static bool HasLineBuffer(int key)
        {
            return s_LineBuffers.ContainsKey(key);
        }

        /// <summary>
        ///     Dereference the indicated <see cref="DebugLineBuffer"/> from the provider.
        /// </summary>
        /// <param name="key">
        ///     The key used to reference the <see cref="DebugLineBuffer"/>.
        /// </param>
        public static void RemoveLineBuffer(int key)
        {
            s_LineBuffers.TryRemove(key);
        }
    }
}