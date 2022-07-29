// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Collections.Generic;

namespace GDX.Rendering
{
    /// <summary>
    ///     A provider of GDX specific command buffers, indexed for reuse.
    /// </summary>
    public static class CommandBufferProvider
    {
        /// <summary>
        ///     A dictionary of known <see cref="DrawCommandBuffer"/> and their ID.
        /// </summary>
        static IntKeyDictionary<DrawCommandBuffer> s_DrawCommandBuffers = new IntKeyDictionary<DrawCommandBuffer>(10);

        /// <summary>
        /// Get an instance of <see cref="DrawCommandBuffer"/> based on the provided <paramref name="key"/>.
        /// </summary>
        /// <example>
        ///     <code>
        ///     DrawCommandBuffer buffer = CommandBufferProvider.GetDrawCommandBuffer(gameObjet.GetInstanceID());
        ///     if (!buffer.Finalized)
        ///     {
        ///         /// Draw lots of stuff ...
        ///         buffer.DrawWireCube(Color.white, worldPosition, size);
        ///     }
        ///     buffer.Execute();
        ///     </code>
        /// </example>
        /// <param name="key">
        ///     A value based key used to reference a <see cref="DrawCommandBuffer"/> in a
        ///     <see cref="IntKeyDictionary{TValue}"/>.
        /// </param>
        /// <param name="initialColorCount">Initial number of internal materials to allocate (x2).</param>
        /// <param name="verticesPerMesh">The number of vertices to split batched meshes on.</param>
        /// <returns>
        ///     A newly created <see cref="DrawCommandBuffer"/> if the provided key is not found, or the previously
        ///     created <see cref="DrawCommandBuffer"/> identified by the <paramref name="key"/>.
        /// </returns>
        public static DrawCommandBuffer GetDrawCommandBuffer(int key, int initialColorCount = 5,
            int verticesPerMesh = DrawCommandBuffer.DefaultMaximumVerticesPerMesh)
        {
            if (s_DrawCommandBuffers.ContainsKey(key))
            {
                return s_DrawCommandBuffers[key];
            }

            DrawCommandBuffer newBuffer = new DrawCommandBuffer(key, initialColorCount, verticesPerMesh);
            s_DrawCommandBuffers.AddWithExpandCheck(key, newBuffer);
            return newBuffer;
        }

        /// <summary>
        ///     Returns if the provided key has a <see cref="DrawCommandBuffer"/> referenced.
        /// </summary>
        /// <param name="key">
        ///     The key used to reference the <see cref="DrawCommandBuffer"/>.
        /// </param>
        /// <returns>true/false if the key has a <see cref="DrawCommandBuffer"/> associated with it.</returns>
        public static bool HasDrawCommandBuffer(int key)
        {
            return s_DrawCommandBuffers.ContainsKey(key);
        }

        /// <summary>
        ///     Dereference the indicated <see cref="DrawCommandBuffer"/> from the provider.
        /// </summary>
        /// <param name="key">
        ///     The key used to reference the <see cref="DrawCommandBuffer"/>.
        /// </param>
        public static void RemoveDrawCommandBuffer(int key)
        {
            s_DrawCommandBuffers.TryRemove(key);
        }
    }
}