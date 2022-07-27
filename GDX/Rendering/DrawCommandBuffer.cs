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
        ///     When using <see cref="DrawMesh"/> this value is ignored.
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
        /// The associated <see cref="int"/> key with the <see cref="DrawCommandBuffer"/>.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This is useful when using the <see cref="GetInstance" /> and <see cref="RemoveInstance(int)"/>
        ///         calls to maximize reuse and limiting allocations.
        ///     </para>
        ///     <para>
        ///         A common pattern is to use the the <see cref="GameObject"/>'s InstanceID or an Entity Number to
        ///         create a unique indexer. Collisions can occur if you are not careful about how you index your
        ///         <see cref="DrawCommandBuffer"/>.
        ///     </para>
        /// </remarks>
        public readonly int Key;

        readonly CommandBuffer m_CommandBuffer;

        public readonly int MaximumVerticesPerMesh;
        IntKeyDictionary<int> m_DottedLineMaterials;
        IntKeyDictionary<int> m_LineMaterials;
        SimpleList<Material> m_Materials;


        // TODO: removing by index will adjust the index of others ;/
        // we should move this to a sparse set
        SimpleList<Mesh> m_Meshes;
        SimpleList<int> m_MeshMaterialIndex;

        IntKeyDictionary<int> m_WorkingIndex;
        IntKeyDictionary<SimpleList<Vector3>> m_WorkingPoints;
        IntKeyDictionary<SimpleList<int>> m_WorkingSegments;

        public DrawCommandBuffer(int key, int initialMaterialCount = 5,
            int verticesPerMesh = DefaultMaximumVerticesPerMesh, bool managed = false)
        {
            Key = key;

            // TODO: Do we wanna add to manager here?

            MaximumVerticesPerMesh = verticesPerMesh;

            m_Materials = new SimpleList<Material>(initialMaterialCount);
            m_LineMaterials = new IntKeyDictionary<int>(initialMaterialCount);
            m_DottedLineMaterials = new IntKeyDictionary<int>(initialMaterialCount);
            m_Meshes = new SimpleList<Mesh>(2);
            m_MeshMaterialIndex = new SimpleList<int>(2);

            m_WorkingPoints = new IntKeyDictionary<SimpleList<Vector3>>(initialMaterialCount);
            m_WorkingSegments = new IntKeyDictionary<SimpleList<int>>(initialMaterialCount);

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

        public bool Finalized
        {
            get;
            private set;
        }

        public static void AppendSegmentsToArray(ref int[] segmentArray, ref int[] segmentsToAdd,
            int segmentArrayStartIndex, int segmentToAddOffset)
        {
            int segmentsToAddCount = segmentsToAdd.Length;
            for (int i = 0; i < segmentsToAddCount; i++)
            {
                segmentArray[segmentArrayStartIndex + i] = segmentsToAdd[i] + segmentToAddOffset;
            }
        }

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
                if (pointList.Count > 0)
                {
                    DrawMesh(GetMaterialByHashCode(materialHashCode), ref pointList.Array, ref segmentList.Array);
                }
            }

            // Clear our working memory
            m_WorkingPoints.Clear();
            m_WorkingSegments.Clear();

            // Record our command buffer
            int meshCount = m_Meshes.Count;
            for (int i = 0; i < meshCount; i++)
            {
                int materialIndex = m_MeshMaterialIndex.Array[i];
                m_CommandBuffer.DrawMesh(m_Meshes.Array[i], Matrix4x4.identity, m_Materials.Array[materialIndex]);
            }

            Finalized = true;
        }

        public int DrawDottedCube(Color color, Vector3 center, Vector3 size)
        {
            Vector3 half = size / 2f;
            Vector3[] points =
            {
                new Vector3(center.x - half.x, center.y - half.y, center.z - half.z), // Front Bottom Left (0)
                new Vector3(center.x - half.x, center.y - half.y, center.z + half.z), // Back Bottom Left (1)
                new Vector3(center.x - half.x, center.y + half.y, center.z + half.z), // Back Top Left (2)
                new Vector3(center.x - half.x, center.y + half.y, center.z - half.z), // Front Top Left (3)
                new Vector3(center.x + half.x, center.y - half.y, center.z - half.z), // Front Bottom Right (4)
                new Vector3(center.x + half.x, center.y - half.y, center.z + half.z), // Back Bottom Right (5)
                new Vector3(center.x + half.x, center.y + half.y, center.z + half.z), // Back Top Right (6)
                new Vector3(center.x + half.x, center.y + half.y, center.z - half.z), // Front Top Right (7)
            };

            return DrawDottedLine(color, ref points, ref CubeSegmentIndices);
        }

        public int DrawDottedLine(Color color, ref Vector3[] vertices, ref int[] segments)
        {
            return DrawDottedLine(color,
                ref vertices, 0, vertices.Length,
                ref segments, 0, segments.Length);
        }

        public int DrawDottedLine(Color color,
            ref Vector3[] vertices, int verticesStartIndex, int verticesLength,
            ref int[] segments, int segmentsStartIndex, int segmentsLength)
        {
            return DrawLine(GetDottedLineMaterialByColor(color), ref vertices, verticesStartIndex, verticesLength,
                ref segments, segmentsStartIndex, segmentsLength);
        }

        public int DrawLine(Color color, ref Vector3[] vertices, ref int[] segments)
        {
            return DrawLine(color,
                ref vertices, 0, vertices.Length,
                ref segments, 0, segments.Length);
        }

        public int DrawLine(Color color,
            ref Vector3[] vertices, int verticesStartIndex, int verticesLength,
            ref int[] segments, int segmentsStartIndex, int segmentsLength)
        {
            return DrawLine(GetSolidLineMaterialByColor(color), ref vertices, verticesStartIndex, verticesLength,
                ref segments, segmentsStartIndex, segmentsLength);
        }

        public int DrawLine(Material material,
            ref Vector3[] vertices, int verticesStartIndex, int verticesLength,
            ref int[] segments, int segmentsStartIndex, int segmentsLength)
        {
            if (Finalized)
            {
                Debug.LogWarning("Finalized. You must invalidate the batch before adding anything.");
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
            }

            // Get data storage
            SimpleList<Vector3> pointList = m_WorkingPoints[materialHashCode];

            // Check for mesh conversion
            if (pointList.Array.Length + verticesLength >= MaximumVerticesPerMesh)
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

            if (m_Meshes.Count == 0)
            {
                return 0;
            }

            return m_Meshes.Count - 1;
        }

        public int DrawMesh(Material material, ref Vector3[] vertices, ref int[] segments,
            MeshTopology topology = MeshTopology.Lines, bool shouldOptimizeMesh = true)
        {
            Mesh batchMesh = new Mesh();

            batchMesh.indexFormat = IndexFormat.UInt32;
            batchMesh.SetVertices(vertices);
            batchMesh.SetIndices(segments, topology, 0);

#if UNITY_EDITOR
            if (shouldOptimizeMesh)
            {
                UnityEditor.MeshUtility.Optimize(batchMesh);
            }
#endif

            m_Meshes.AddWithExpandCheck(batchMesh);
            m_MeshMaterialIndex.AddWithExpandCheck(GetMaterialIndex(material));

            // Return mesh index
            return m_Meshes.Count - 1;
        }

        public int DrawWireCube(Color color, Vector3 center, Vector3 size)
        {
            Vector3 half = size / 2f;
            Vector3[] points =
            {
                new Vector3(center.x - half.x, center.y - half.y, center.z - half.z), // Front Bottom Left (0)
                new Vector3(center.x - half.x, center.y - half.y, center.z + half.z), // Back Bottom Left (1)
                new Vector3(center.x - half.x, center.y + half.y, center.z + half.z), // Back Top Left (2)
                new Vector3(center.x - half.x, center.y + half.y, center.z - half.z), // Front Top Left (3)
                new Vector3(center.x + half.x, center.y - half.y, center.z - half.z), // Front Bottom Right (4)
                new Vector3(center.x + half.x, center.y - half.y, center.z + half.z), // Back Bottom Right (5)
                new Vector3(center.x + half.x, center.y + half.y, center.z + half.z), // Back Top Right (6)
                new Vector3(center.x + half.x, center.y + half.y, center.z - half.z), // Front Top Right (7)
            };

            return DrawLine(color, ref points, ref CubeSegmentIndices);
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
        ///     Invalidates a specific mesh and forces the buffer to be refilled.
        /// </summary>
        /// <param name="meshID"></param>
        public void InvalidateMesh(int meshID)
        {
            // TODO: need to hand change over to sparseset
            m_Meshes.RemoveAt(meshID);
            m_MeshMaterialIndex.RemoveAt(meshID);
            Finalized = false;
        }

        /// <summary>
        ///     Resets the <see cref="DrawCommandBuffer"/>, as if it were newly created. However all fields are already
        ///     allocating their previous sizes.
        /// </summary>
        public void Reset()
        {
            m_WorkingPoints.Clear();
            m_WorkingSegments.Clear();

            m_Meshes.Clear();
            m_Materials.Clear();
            m_MeshMaterialIndex.Clear();
            m_LineMaterials.Clear();
            m_DottedLineMaterials.Clear();
            m_CommandBuffer.Clear();

            Finalized = false;
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
    }
}