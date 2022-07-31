// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GDX.Rendering
{
    /// <summary>
    /// An optimized method for drawing static procedural content.
    /// </summary>
    public class DrawCommandBuffer
    {
        /// <summary>
        ///     The default maximum number of vertices per mesh when dynamically creating meshes.
        /// </summary>
        /// <remarks>
        ///     When using a mesh is manually added via
        ///     <see cref="DrawMesh(UnityEngine.Material,UnityEngine.Mesh)"/> this value is ignored.
        /// </remarks>
        public const int DefaultMaximumVerticesPerMesh = 512;

        /// <summary>
        ///     The ordered segment index pairs used to describe a cube.
        /// </summary>
        /// <remarks>
        ///     This effectively wraps the left side, then the right, then connects the two sides.
        /// </remarks>
        public static int[] CubeSegmentIndices =
        {
            0, 1, 1, 2, 2, 3, 3, 0, 4, 5, 5, 6, 6, 7, 7, 4, 0, 4, 1, 5, 2, 6, 3, 7
        };

        /// <summary>
        ///     The base instance of the default dotted line material.
        /// </summary>
        /// <remarks>
        ///     This will be cloned when a new color value is provided and not found by
        ///     <see cref="GetDottedLineMaterialByColor"/>.
        /// </remarks>
        static Material s_DottedLineMaterial;

        /// <summary>
        ///     The base instance of the default line material.
        /// </summary>
        /// <remarks>
        ///     This will be cloned when a new color value is provided and not found by
        ///     <see cref="GetSolidLineMaterialByColor" />.
        /// </remarks>
        static Material s_LineMaterial;

        /// <summary>
        ///     The ordered segment index pairs used to describe a line.
        /// </summary>
        static int[] s_LineSegmentIndices =
        {
            0, 1
        };

        /// <summary>
        /// The associated <see cref="int"/> key with the <see cref="DrawCommandBuffer"/>.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This is useful for identifying the <see cref="DrawCommandBuffer" /> in different contexts; its
        ///         specific use is meant for being able to recall a <see cref="DrawCommandBuffer" /> from the
        ///         <see cref="CommandBufferProvider" />.
        ///     </para>
        ///     <para>
        ///         A common pattern is to use the the <see cref="GameObject"/>'s InstanceID or an Entity Number to
        ///         create a unique indexer. Collisions can occur if you are not careful about how you index your
        ///         <see cref="DrawCommandBuffer"/>.
        ///     </para>
        /// </remarks>
        public readonly int Key;

        /// <summary>
        ///     The actual allocated <see cref="CommandBuffer"/> used by the <see cref="DrawCommandBuffer"/>.
        /// </summary>
        readonly CommandBuffer m_CommandBuffer;

        /// <summary>
        ///     The established maximum number of vertices per mesh for this particular <see cref="DrawCommandBuffer"/>.
        /// </summary>
        /// <remarks>
        ///     Once this is set in the constructor it cannot be changed. Arbitrary mesh adds are not effected by this
        /// </remarks>
        readonly int m_MaximumVerticesPerMesh;

        /// <summary>
        ///     The current incremental token used when associating draw commands.
        /// </summary>
        /// <remarks>
        ///     This is used to provide a stable index in an extremely simple form. While it will eventually roll over,
        ///     at that threshold you should be considering if multiple <see cref="DrawCommandBuffer"/> may be more
        ///     optimal.
        /// </remarks>
        int m_CurrentToken;

        /// <summary>
        ///     An indexed dictionary of all dotted line materials, referenced by the hashcode of the color.
        /// </summary>
        IntKeyDictionary<int> m_DottedLineMaterials;

        /// <summary>
        ///     An indexed dictionary of all of the draw commands to use with the buffer.
        /// </summary>
        /// <remarks>
        ///     This includes the mesh and an index of the material to use with that mesh when drawing. As items are
        ///     added, the <see cref="m_CurrentToken"/> is incremented to simulate a stable ID.
        /// </remarks>
        IntKeyDictionary<DrawCommand> m_DrawCommands;

        /// <summary>
        ///     An indexed dictionary of all line materials, referenced by the hashcode of the color.
        /// </summary>
        IntKeyDictionary<int> m_LineMaterials;

        /// <summary>
        ///     An ever expanding list of materials used with the <see cref="DrawCommandBuffer"/>.
        /// </summary>
        /// <remarks>
        ///     Both <see cref="m_DottedLineMaterials"/> and <see cref="m_LineMaterials"/> store indexes of
        ///     <see cref="Material"/>s inside of this list.
        /// </remarks>
        SimpleList<Material> m_Materials;

        /// <summary>
        ///     A storage of working vertices information indexed based on the hashcode of the material it is meant to
        ///     be drawn with.
        /// </summary>
        IntKeyDictionary<SimpleList<Vector3>> m_WorkingPoints;

        /// <summary>
        ///     A storage of working segment indices pairs indexed based on the hashcode of the material it is meant to
        ///     be drawn with.
        /// </summary>
        IntKeyDictionary<SimpleList<int>> m_WorkingSegments;

        /// <summary>
        ///     A storage of working expected tokens of meshes to be created in the future.
        /// </summary>
        IntKeyDictionary<int> m_WorkingTokens;

        /// <summary>
        ///     Create a <see cref="DrawCommandBuffer"/>.
        /// </summary>
        /// <param name="key">The internally cached key associated with this buffer.</param>
        /// <param name="initialMaterialCount">
        ///     An initial allocation of the expected number of materials that will be used.
        /// </param>
        /// <param name="verticesPerMesh">The number of vertices to ingest before a mesh is split.</param>
        public DrawCommandBuffer(int key, int initialMaterialCount = 5,
            int verticesPerMesh = DefaultMaximumVerticesPerMesh)
        {
            Key = key;

            m_MaximumVerticesPerMesh = verticesPerMesh;

            m_Materials = new SimpleList<Material>(initialMaterialCount);
            m_LineMaterials = new IntKeyDictionary<int>(initialMaterialCount);
            m_DottedLineMaterials = new IntKeyDictionary<int>(initialMaterialCount);

            m_WorkingPoints = new IntKeyDictionary<SimpleList<Vector3>>(initialMaterialCount);
            m_WorkingSegments = new IntKeyDictionary<SimpleList<int>>(initialMaterialCount);
            m_WorkingTokens = new IntKeyDictionary<int>(initialMaterialCount);

            m_DrawCommands = new IntKeyDictionary<DrawCommand>(2);
            m_CurrentToken = 0;

            m_CommandBuffer = new CommandBuffer();

            // Make sure our statics have their desired default materials atm
            if (s_LineMaterial == null)
            {
                s_LineMaterial = new Material(ShaderProvider.UnlitColor);
            }

            if (s_DottedLineMaterial == null)
            {
                s_DottedLineMaterial = new Material(ShaderProvider.DottedLine);
            }
        }

        /// <summary>
        ///     Has the <see cref="DrawCommandBuffer"/> been converged?
        /// </summary>
        /// <remarks>
        ///     A finalized <see cref="DrawCommandBuffer"/> has had its command buffer filled with the fixed draw calls
        ///     based on the meshes/materials outlined. If a mesh is invalidated by <see cref="Invalidate"/>, the
        ///     <see cref="DrawCommandBuffer"/> will become not finalized and will re-converge itself next
        ///     <see cref="Execute"/>.
        /// </remarks>
        public bool Finalized
        {
            get;
            private set;
        }

        /// <summary>
        ///     Ensure that we dispose associated resources.
        /// </summary>
        ~DrawCommandBuffer()
        {
            m_CommandBuffer?.Dispose();
        }

        /// <summary>
        ///     Converges all working vertices/material additions into finalized mesh forms and fills the command
        ///     buffer with the appropriate data.
        /// </summary>
        public void Converge()
        {
            if (Finalized) return;

            //  Finalize each material we have something in process for
            int materialCount = m_Materials.Count;
            for (int i = 0; i < materialCount; i++)
            {
                int materialHashCode = m_Materials.Array[i].GetHashCode();
                SimpleList<Vector3> pointList = m_WorkingPoints[materialHashCode];
                SimpleList<int> segmentList = m_WorkingSegments[materialHashCode];
                int token = m_WorkingTokens[materialHashCode];
                if (pointList.Count > 0)
                {
                    DrawMesh(GetMaterialByHashCode(materialHashCode), ref pointList.Array, ref segmentList.Array,
                        MeshTopology.Lines, token);
                }
            }

            // Clear our working memory
            m_WorkingPoints.Clear();
            m_WorkingSegments.Clear();

            // Record our command buffer
            int currentIndex = 0;
            while (m_DrawCommands.MoveNext(ref currentIndex))
            {
                IntKeyEntry<DrawCommand> currentEntry = m_DrawCommands.Entries[currentIndex - 1];
                m_CommandBuffer.DrawMesh(currentEntry.Value.ImmutableMesh, Matrix4x4.identity,
                    m_Materials.Array[currentEntry.Value.MaterialIndex]);
            }
            Finalized = true;
        }

        /// <summary>
        ///     Draw a dotted line cube of a specific color to the buffer.
        /// </summary>
        /// <param name="color">The color which to draw the dotted line cube with.</param>
        /// <param name="center">The center world position of the cube.</param>
        /// <param name="size">The unit size of the cube</param>
        /// <returns>The created cube's invalidation token.</returns>
        public int DrawDottedCube(Color color, Vector3 center, Vector3 size)
        {
            Vector3[] vertices = GetCubeVertices(center, size);
            return DrawDottedLines(color, ref vertices, ref CubeSegmentIndices);
        }

        /// <summary>
        ///     Draw a dotted line of a specific color as defined to the buffer.
        /// </summary>
        /// <remarks>
        ///     If multiple lines are being drawn it is much more performant to use
        ///     <see cref="DrawDottedLines(UnityEngine.Color,ref UnityEngine.Vector3[],ref int[])"/>.
        /// </remarks>
        /// <param name="color">The color which to draw the dotted line with.</param>
        /// <param name="startPoint">The start of the line in world space.</param>
        /// <param name="endPoint">The end of the line in world space.</param>
        /// <returns>The dotted line's invalidation token.</returns>
        public int DrawDottedLine(Color color, Vector3 startPoint, Vector3 endPoint)
        {
            Vector3[] points = new Vector3[] { startPoint, endPoint };
            return DrawDottedLines(color, ref points, 0, 2,
                ref s_LineSegmentIndices, 0, 2);
        }

        /// <summary>
        ///     Draw dotted lines of a specific color as defined to the buffer.
        /// </summary>
        /// <param name="color">The color which to draw the dotted lines with.</param>
        /// <param name="vertices">The vertices of the dotted lines.</param>
        /// <param name="segments">The segment pairs based on <paramref name="vertices"/>.</param>
        /// <returns>The dotted lines' invalidation token.</returns>
        public int DrawDottedLines(Color color, ref Vector3[] vertices, ref int[] segments)
        {
            return DrawDottedLines(color,
                ref vertices, 0, vertices.Length,
                ref segments, 0, segments.Length);
        }

        /// <summary>
        ///     Draw dotted lines of a specific color as defined to the buffer.
        /// </summary>
        /// <param name="color">The color which to draw the dotted lines with.</param>
        /// <param name="vertices">The vertices of the dotted lines.</param>
        /// <param name="verticesStartIndex">The index to start at in the <paramref name="vertices"/> array.</param>
        /// <param name="verticesLength">The number of elements in the <paramref name="vertices"/> array to use.</param>
        /// <param name="segments">The segment pairs based on <paramref name="vertices"/>.</param>
        /// <param name="segmentsStartIndex">The index to start at in the <paramref name="segments"/> array.</param>
        /// <param name="segmentsLength">The number of elements in the <paramref name="segments"/> array to use.</param>
        /// <returns>The dotted lines' invalidation token.</returns>
        public int DrawDottedLines(Color color,
            ref Vector3[] vertices, int verticesStartIndex, int verticesLength,
            ref int[] segments, int segmentsStartIndex, int segmentsLength)
        {
            return DrawLines(GetDottedLineMaterialByColor(color), ref vertices, verticesStartIndex, verticesLength,
                ref segments, segmentsStartIndex, segmentsLength);
        }

        /// <summary>
        ///     Draw a line of a specific color as defined to the buffer.
        /// </summary>
        /// <remarks>
        ///     If multiple lines are being drawn it is much more performant to use
        ///     <see cref="DrawLines(UnityEngine.Color,ref UnityEngine.Vector3[],ref int[])"/>.
        /// </remarks>
        /// <param name="color">The color which to draw the line with.</param>
        /// <param name="startPoint">The start of the line in world space.</param>
        /// <param name="endPoint">The end of the line in world space.</param>
        /// <returns>The line's invalidation token.</returns>
        public int DrawLine(Color color, Vector3 startPoint, Vector3 endPoint)
        {
            Vector3[] points = new Vector3[] { startPoint, endPoint };
            return DrawLines(color, ref points, 0, 2,
                ref s_LineSegmentIndices, 0, 2);
        }

        /// <summary>
        ///     Draw lines of a specific color as defined to the buffer.
        /// </summary>
        /// <param name="color">The color which to draw the lines with.</param>
        /// <param name="vertices">The vertices of the lines.</param>
        /// <param name="segments">The segment pairs based on <paramref name="vertices"/>.</param>
        /// <returns>The lines' invalidation token.</returns>
        public int DrawLines(Color color, ref Vector3[] vertices, ref int[] segments)
        {
            return DrawLines(color,
                ref vertices, 0, vertices.Length,
                ref segments, 0, segments.Length);
        }

        /// <summary>
        ///     Draw lines of a specific color as defined to the buffer.
        /// </summary>
        /// <param name="color">The color which to draw the lines with.</param>
        /// <param name="vertices">The vertices of the lines.</param>
        /// <param name="verticesStartIndex">The index to start at in the <paramref name="vertices"/> array.</param>
        /// <param name="verticesLength">The number of elements in the <paramref name="vertices"/> array to use.</param>
        /// <param name="segments">The segment pairs based on <paramref name="vertices"/>.</param>
        /// <param name="segmentsStartIndex">The index to start at in the <paramref name="segments"/> array.</param>
        /// <param name="segmentsLength">The number of elements in the <paramref name="segments"/> array to use.</param>
        /// <returns>The lines' invalidation token.</returns>
        public int DrawLines(Color color,
            ref Vector3[] vertices, int verticesStartIndex, int verticesLength,
            ref int[] segments, int segmentsStartIndex, int segmentsLength)
        {
            return DrawLines(GetSolidLineMaterialByColor(color), ref vertices, verticesStartIndex, verticesLength,
                ref segments, segmentsStartIndex, segmentsLength);
        }

        /// <summary>
        ///     Draw lines with a specific material to the buffer.
        /// </summary>
        /// <param name="material">The material which to draw the lines with.</param>
        /// <param name="vertices">The vertices of the lines.</param>
        /// <param name="verticesStartIndex">The index to start at in the <paramref name="vertices"/> array.</param>
        /// <param name="verticesLength">The number of elements in the <paramref name="vertices"/> array to use.</param>
        /// <param name="segments">The segment pairs based on <paramref name="vertices"/>.</param>
        /// <param name="segmentsStartIndex">The index to start at in the <paramref name="segments"/> array.</param>
        /// <param name="segmentsLength">The number of elements in the <paramref name="segments"/> array to use.</param>
        /// <returns>The lines' invalidation token.</returns>
        public int DrawLines(Material material,
            ref Vector3[] vertices, int verticesStartIndex, int verticesLength,
            ref int[] segments, int segmentsStartIndex, int segmentsLength)
        {
            if (Finalized)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Finalized. You must invalidate the batch before adding anything.");
#endif
                return -1;
            }

            // Figure out our identifier for the material we will be drawing against
            int materialHashCode = material.GetHashCode();
            if (!m_Materials.ContainsItem(material))
            {
                m_Materials.AddWithExpandCheck(material);
            }

            if (!m_WorkingPoints.ContainsKey(materialHashCode))
            {
                m_WorkingPoints.AddWithExpandCheck(materialHashCode, new SimpleList<Vector3>(verticesLength));
                m_WorkingSegments.AddWithExpandCheck(materialHashCode, new SimpleList<int>(segmentsLength));
                m_WorkingTokens.AddWithExpandCheck(materialHashCode, ReserveToken());
            }

            // Get data storage
            SimpleList<Vector3> pointList = m_WorkingPoints[materialHashCode];
            int token = m_WorkingTokens[materialHashCode];

            // Check for mesh conversion
            if (pointList.Array.Length + verticesLength >= m_MaximumVerticesPerMesh)
            {
                // Create mesh!
                SimpleList<int> segmentList = m_WorkingSegments[materialHashCode];
                DrawMesh(material, ref pointList.Array, ref segmentList.Array);

                // Reset storage
                pointList.Clear();
                m_WorkingSegments[materialHashCode] = new SimpleList<int>(pointList.Array.Length);
            }

            int verticesBaseIndex = pointList.Count;
            if (verticesLength > 0)
            {
                pointList.Reserve(verticesLength);
                int verticesStopIndex = verticesStartIndex + verticesLength;
                for (int i = verticesStartIndex; i < verticesStopIndex; i++)
                {
                    pointList.AddUnchecked(vertices[i]);
                }

                m_WorkingPoints[materialHashCode] = pointList;
            }

            if (segmentsLength > 0)
            {
                SimpleList<int> segmentList = m_WorkingSegments[materialHashCode];
                segmentList.Reserve(segmentsLength);
                int segmentsStopIndex = segmentsStartIndex + segmentsLength;
                for (int i = segmentsStartIndex; i < segmentsStopIndex; i++)
                {
                    segmentList.AddUnchecked(verticesBaseIndex + segments[i]);
                }

                m_WorkingSegments[materialHashCode] = segmentList;
            }

            return token;
        }

        /// <summary>
        ///     Draw a given mesh to the buffer.
        /// </summary>
        /// <param name="material">The material to use when drawing the mesh.</param>
        /// <param name="mesh">The mesh to draw to the buffer.</param>
        /// <returns>The mesh's invalidation token.</returns>
        public int DrawMesh(Material material, Mesh mesh)
        {
            int token = ReserveToken();
            m_DrawCommands.AddWithExpandCheck(token,
                new DrawCommand(mesh, GetMaterialIndex(material)));
            return token;
        }

        /// <summary>
        ///     Draw a given mesh (as defined) to the buffer.
        /// </summary>
        /// <param name="material">The material to use when drawing the created mesh.</param>
        /// <param name="vertices">The vertices of the created mesh.</param>
        /// <param name="segments">The segment pairs based on <paramref name="vertices"/>.</param>
        /// <param name="topology">The <see cref="MeshTopology"/> mode to use when drawing the created mesh.</param>
        /// <param name="token">Force a specific token for the mesh. Don't use this.</param>
        /// <returns>The created mesh's invalidation token.</returns>
        public int DrawMesh(Material material, ref Vector3[] vertices, ref int[] segments,
            MeshTopology topology = MeshTopology.Lines, int token = -1)
        {

            Mesh batchMesh = new Mesh();
            batchMesh.indexFormat = IndexFormat.UInt32;
            batchMesh.SetVertices(vertices);
            batchMesh.SetIndices(segments, topology, 0);
#if UNITY_EDITOR
            batchMesh.name = $"Mesh_{material.name}_{m_CurrentToken}";
#endif
            if (token == -1)
            {
                token = ReserveToken();
            }

            m_DrawCommands.AddWithExpandCheck(token,
                new DrawCommand(batchMesh, GetMaterialIndex(material)));
            return token;
        }

        /// <summary>
        ///     Draw a wireframe cube of a specific color to the buffer.
        /// </summary>
        /// <param name="color">The color which to draw the wire cube with.</param>
        /// <param name="center">The center world position of the cube.</param>
        /// <param name="size">The unit size of the cube</param>
        /// <returns>The created cube's invalidation token.</returns>
        public int DrawWireCube(Color color, Vector3 center, Vector3 size)
        {
            Vector3[] vertices = GetCubeVertices(center, size);
            return DrawLines(color, ref vertices, ref CubeSegmentIndices);
        }

        /// <summary>
        ///     Execute the <see cref="DrawCommandBuffer"/>, rendering its outputs to the screen.
        /// </summary>
        /// <remarks>
        ///     This will finalize the command buffer, converging all data into meshes, etc. In order to change the
        ///     buffer, you will need to
        /// </remarks>
        public void Execute()
        {
            if (!Finalized)
            {
                Converge();
            }

            Graphics.ExecuteCommandBuffer(m_CommandBuffer);
        }

        /// <summary>
        /// Is the given <paramref name="token"/> present in the draw commands buffer.
        /// </summary>
        /// <param name="token">The token of the draw commands to check for.</param>
        /// <returns>Returns <b>true</b> if the token is found in the existing draw commands.</returns>
        public bool HasToken(int token)
        {
            return m_DrawCommands.ContainsKey(token);
        }

        /// <summary>
        ///     Invalidates a <see cref="DrawCommand"/> based on the provided token, forcing the buffer to be refilled.
        /// </summary>
        /// <param name="token">The token of the draw commands to invalidate.</param>
        public void Invalidate(int token)
        {
            if (m_DrawCommands.TryRemove(token))
            {
                Finalized = false;
            }
        }

        /// <summary>
        ///     Resets the <see cref="DrawCommandBuffer"/>, as if it were newly created. However all fields are already
        ///     allocating their previous sizes.
        /// </summary>
        public void Reset()
        {
            m_WorkingPoints.Clear();
            m_WorkingSegments.Clear();
            m_WorkingTokens.Clear();

            m_CurrentToken = 0;
            m_DrawCommands.Clear();

            m_Materials.Clear();
            m_LineMaterials.Clear();
            m_DottedLineMaterials.Clear();

            m_CommandBuffer.Clear();

            Finalized = false;
        }

        /// <summary>
        ///     Get the vertices that make up a cube.
        /// </summary>
        /// <remarks>
        ///     Ordered based on <see cref="CubeSegmentIndices"/>.
        /// </remarks>
        /// <param name="center">The world space center location of the cube.</param>
        /// <param name="size">The size of the cube.</param>
        /// <returns>An array of ordered vertices.</returns>
        static Vector3[] GetCubeVertices(Vector3 center, Vector3 size)
        {
            Vector3 half = size / 2f;

            float centerMinusHalfX = center.x - half.x;
            float centerMinusHalfY = center.y - half.y;
            float centerMinusHalfZ = center.z - half.z;
            float centerPlusHalfX = center.x + half.x;
            float centerPlusHalfY = center.y + half.y;
            float centerPlusHalfZ = center.z + half.z;

            Vector3[] points =
            {
                new Vector3(centerMinusHalfX, centerMinusHalfY, centerMinusHalfZ), // Front Bottom Left (0)
                new Vector3(centerMinusHalfX, centerMinusHalfY, centerPlusHalfZ), // Back Bottom Left (1)
                new Vector3(centerMinusHalfX, centerPlusHalfY, centerPlusHalfZ), // Back Top Left (2)
                new Vector3(centerMinusHalfX, centerPlusHalfY, centerMinusHalfZ), // Front Top Left (3)
                new Vector3(centerPlusHalfX, centerMinusHalfY, centerMinusHalfZ), // Front Bottom Right (4)
                new Vector3(centerPlusHalfX, centerMinusHalfY, centerPlusHalfZ), // Back Bottom Right (5)
                new Vector3(centerPlusHalfX, centerPlusHalfY, centerPlusHalfZ), // Back Top Right (6)
                new Vector3(centerPlusHalfX, centerPlusHalfY, centerMinusHalfZ), // Front Top Right (7)
            };

            return points;
        }

        /// <summary>
        ///     Gets the cached dotted line material, or creates one for the given <paramref name="color"/>.
        /// </summary>
        /// <param name="color">A defined draw color.</param>
        /// <returns>The qualified dotted line material of the color.</returns>
        Material GetDottedLineMaterialByColor(Color color)
        {
            int requestedHashCode = color.GetHashCode();
            if (m_DottedLineMaterials.ContainsKey(requestedHashCode))
            {
                return m_Materials.Array[m_DottedLineMaterials[requestedHashCode]];
            }

            Material newMaterial = new Material(s_DottedLineMaterial);
            newMaterial.SetColor(ShaderProvider.ColorPropertyID, color);

            m_Materials.AddWithExpandCheck(newMaterial);
            m_DottedLineMaterials.AddWithExpandCheck(requestedHashCode, m_Materials.Count - 1);
            return newMaterial;
        }

        /// <summary>
        ///     Gets a material from the internal cache based on its <see cref="Material.GetHashCode"/>.
        /// </summary>
        /// <remarks>A warning will be thrown in the editor if the material is not found.</remarks>
        /// <param name="hashCode">The integer based hash code of the material.</param>
        /// <returns>The material if found, otherwise a default material will be returned.</returns>
        Material GetMaterialByHashCode(int hashCode)
        {
            int count = m_Materials.Count;
            for (int i = 0; i < count; i++)
            {
                if (m_Materials.Array[i].GetHashCode() == hashCode) return m_Materials.Array[i];
            }
#if UNITY_EDITOR
            Debug.LogWarning(
                "A set of lines have been added to the MergedDraw where the Material hash code that was provided has not previously been used/registered. Using the default line material instead.");
#endif
            // Ensure default material
            GetMaterialIndex(s_LineMaterial);
            return s_LineMaterial;
        }

        /// <summary>
        ///     Gets the index of a material in the internal cache, or adds it and returns the new index.
        /// </summary>
        /// <param name="material">The material index to find.</param>
        /// <returns>The index of the given material in the internal cache.</returns>
        int GetMaterialIndex(Material material)
        {
            int count = m_Materials.Count;
            for (int i = 0; i < count; i++)
            {
                if (m_Materials.Array[i] == material) return i;
            }

            m_Materials.AddWithExpandCheck(material);
            return m_Materials.Count - 1;
        }

        /// <summary>
        ///     Gets the cached solid line material, or creates one for the given <paramref name="color"/>.
        /// </summary>
        /// <param name="color">A defined draw color.</param>
        /// <returns>The qualified line material of the color.</returns>
        Material GetSolidLineMaterialByColor(Color color)
        {
            int requestedHashCode = color.GetHashCode();
            if (m_LineMaterials.ContainsKey(requestedHashCode))
            {
                return m_Materials.Array[m_LineMaterials[requestedHashCode]];
            }

            Material newMaterial = new Material(s_LineMaterial);
            newMaterial.SetColor(ShaderProvider.ColorPropertyID, color);

            m_Materials.AddWithExpandCheck(newMaterial);
            m_LineMaterials.AddWithExpandCheck(requestedHashCode, m_Materials.Count - 1);
            return newMaterial;
        }

        int ReserveToken()
        {
            int returnValue = m_CurrentToken;
            m_CurrentToken++;
            return returnValue;
        }

        /// <summary>
        ///     A structure describing a finalized mesh/material and its draw operation.
        /// </summary>
        struct DrawCommand
        {
            public readonly int MaterialIndex;
            public readonly Mesh ImmutableMesh;

            public DrawCommand(Mesh mesh, int materialIndex)
            {
                MaterialIndex = materialIndex;
                ImmutableMesh = mesh;
            }
        }
    }
}