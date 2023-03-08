// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;
using Unity.Mathematics;

namespace GDX.Developer
{
    public static class DebugDrawShapes
    {
        const float PI = 3.1415927f;
        const float Deg2Rad = 0.017453292f;
        const int DefaultCircleVertexCount = 32;

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


        static void GetCircleVertices(ref Vector3[] vertices, int startIndex, Vector3 center, Quaternion rotation, float radius, int circleVertexCount = DefaultCircleVertexCount)
        {
            float radiansInterval = PI * 2f / circleVertexCount;

            // Loop through and figure out the points
            for (int i = 0; i < circleVertexCount; i++)
            {
                float angle = i * radiansInterval;

                // Create base point
                vertices[i+startIndex] = (rotation * new Vector3(0, math.sin(angle) * radius, math.cos(angle) * radius)) + center;
            }
        }

        static int[] GetCubeSegmentOffset(int offset)
        {
            int[] newIndices = new int[24]
            {
                CubeSegmentIndices[0] + offset, CubeSegmentIndices[1] + offset, CubeSegmentIndices[2] + offset,
                CubeSegmentIndices[3] + offset, CubeSegmentIndices[4] + offset, CubeSegmentIndices[5] + offset,
                CubeSegmentIndices[6] + offset, CubeSegmentIndices[7] + offset, CubeSegmentIndices[8] + offset,
                CubeSegmentIndices[9] + offset, CubeSegmentIndices[10] + offset, CubeSegmentIndices[11] + offset,
                CubeSegmentIndices[12] + offset, CubeSegmentIndices[13] + offset, CubeSegmentIndices[14] + offset,
                CubeSegmentIndices[15] + offset, CubeSegmentIndices[16] + offset, CubeSegmentIndices[17] + offset,
                CubeSegmentIndices[18] + offset, CubeSegmentIndices[19] + offset, CubeSegmentIndices[20] + offset,
                CubeSegmentIndices[21] + offset, CubeSegmentIndices[22] + offset, CubeSegmentIndices[23] + offset
            };
            return newIndices;
        }

        static void GetCubeSegmentOffsetNonAlloc(int[] indices, int startIndex, int offset)
        {
            indices[startIndex] = CubeSegmentIndices[0] + offset;
            indices[startIndex+1] = CubeSegmentIndices[1] + offset;
            indices[startIndex+2] = CubeSegmentIndices[2] + offset;
            indices[startIndex+3] = CubeSegmentIndices[3] + offset;
            indices[startIndex+4] = CubeSegmentIndices[4] + offset;
            indices[startIndex+5] = CubeSegmentIndices[5] + offset;
            indices[startIndex+6] = CubeSegmentIndices[6] + offset;
            indices[startIndex+7] = CubeSegmentIndices[7] + offset;
            indices[startIndex+8] = CubeSegmentIndices[8] + offset;
            indices[startIndex+9] = CubeSegmentIndices[9] + offset;
            indices[startIndex+10] = CubeSegmentIndices[10] + offset;
            indices[startIndex+11] = CubeSegmentIndices[11] + offset;
            indices[startIndex+12] = CubeSegmentIndices[12] + offset;
            indices[startIndex+13] = CubeSegmentIndices[13] + offset;
            indices[startIndex+14] = CubeSegmentIndices[14] + offset;
            indices[startIndex+15] = CubeSegmentIndices[15] + offset;
            indices[startIndex+16] = CubeSegmentIndices[16] + offset;
            indices[startIndex+17] = CubeSegmentIndices[17] + offset;
            indices[startIndex+18] = CubeSegmentIndices[18] + offset;
            indices[startIndex+19] = CubeSegmentIndices[19] + offset;
            indices[startIndex+20] = CubeSegmentIndices[20] + offset;
            indices[startIndex+21] = CubeSegmentIndices[21] + offset;
            indices[startIndex+22] = CubeSegmentIndices[22] + offset;
            indices[startIndex+23] = CubeSegmentIndices[23] + offset;
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
        static Vector3[] GetCubeVertices(Vector3 center, Quaternion rotation, Vector3 size)
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
                rotation * new Vector3(centerMinusHalfX, centerMinusHalfY, centerMinusHalfZ), // Front Bottom Left (0)
                rotation * new Vector3(centerMinusHalfX, centerMinusHalfY, centerPlusHalfZ), // Back Bottom Left (1)
                rotation * new Vector3(centerMinusHalfX, centerPlusHalfY, centerPlusHalfZ), // Back Top Left (2)
                rotation * new Vector3(centerMinusHalfX, centerPlusHalfY, centerMinusHalfZ), // Front Top Left (3)
                rotation * new Vector3(centerPlusHalfX, centerMinusHalfY, centerMinusHalfZ), // Front Bottom Right (4)
                rotation * new Vector3(centerPlusHalfX, centerMinusHalfY, centerPlusHalfZ), // Back Bottom Right (5)
                rotation * new Vector3(centerPlusHalfX, centerPlusHalfY, centerPlusHalfZ), // Back Top Right (6)
                rotation * new Vector3(centerPlusHalfX, centerPlusHalfY, centerMinusHalfZ), // Front Top Right (7)
            };

            return points;
        }

        static void GetCubeVerticesNonAlloc(ref Vector3[] points, int startIndex, Vector3 center, Quaternion rotation, Vector3 size)
        {
            Vector3 half = size / 2f;

            float centerMinusHalfX = center.x - half.x;
            float centerMinusHalfY = center.y - half.y;
            float centerMinusHalfZ = center.z - half.z;
            float centerPlusHalfX = center.x + half.x;
            float centerPlusHalfY = center.y + half.y;
            float centerPlusHalfZ = center.z + half.z;

            points[startIndex] = rotation * new Vector3(centerMinusHalfX, centerMinusHalfY, centerMinusHalfZ); // Front Bottom Left (0)
            points[startIndex + 1] = rotation * new Vector3(centerMinusHalfX, centerMinusHalfY, centerPlusHalfZ); // Back Bottom Left (1)
            points[startIndex + 2] = rotation * new Vector3(centerMinusHalfX, centerPlusHalfY, centerPlusHalfZ); // Back Top Left (2)
            points[startIndex + 3] = rotation * new Vector3(centerMinusHalfX, centerPlusHalfY, centerMinusHalfZ); // Front Top Left (3)
            points[startIndex + 4] = rotation * new Vector3(centerPlusHalfX, centerMinusHalfY, centerMinusHalfZ); // Front Bottom Right (4)
            points[startIndex + 5] = rotation * new Vector3(centerPlusHalfX, centerMinusHalfY, centerPlusHalfZ); // Back Bottom Right (5)
            points[startIndex + 6] = rotation * new Vector3(centerPlusHalfX, centerPlusHalfY, centerPlusHalfZ); // Back Top Right (6)
            points[startIndex + 7] = rotation * new Vector3(centerPlusHalfX, centerPlusHalfY, centerMinusHalfZ); // Front Top Right (7)
        }

        /// <summary>
        ///     Draw a dotted line cube of a specific color to the buffer.
        /// </summary>
        /// <param name="color">The color which to draw the dotted line cube with.</param>
        /// <param name="center">The center world position of the cube.</param>
        /// <param name="size">The unit size of the cube</param>
        /// <returns>The created cube's invalidation token.</returns>
        public static int DrawDottedCube(this DebugDrawBuffer buffer, Color color, Vector3 center, Quaternion rotation, Vector3 size)
        {
            Vector3[] vertices = GetCubeVertices(center, rotation, size);
            return buffer.DrawDottedLines(color, ref vertices, ref CubeSegmentIndices);
        }

        static readonly Quaternion k_RotationPrimaryTopLoop = Quaternion.Euler(0, 90, 0);
        static readonly Quaternion k_RotationPrimaryBottomLoop = Quaternion.Euler(0, 90, 180);

        static readonly Quaternion k_RotationSecondaryTopLoop = Quaternion.Euler(0, 180, 0);
        static readonly Quaternion k_RotationSecondaryBottomLoop = Quaternion.Euler(0, 180, 180);


        public static int DrawWireCapsule(this DebugDrawBuffer buffer, Color color, Vector3 bottomSpherePosition, Vector3 topSpherePosition, Quaternion rotation, float radius, int arcVertexCount = DefaultCircleVertexCount / 2)
        {
            // Calculate total vertices
            int totalVertices = (arcVertexCount + 1) * 4;
            // TODO: add circles?

            Vector3[] vertices = new Vector3[totalVertices];
            float baseAngle = 0f;
            float arcLength = 180f;


            int bottomPrimaryStartIndex = arcVertexCount + 1;
            int topSecondaryStartIndex = bottomPrimaryStartIndex * 2;
            int bottomSecondaryStartIndex = bottomPrimaryStartIndex * 3;

            Quaternion primaryTopRotation = rotation * k_RotationPrimaryTopLoop;
            Quaternion primaryBottomRotation = rotation * k_RotationPrimaryBottomLoop;
            Quaternion secondaryTopRotation = rotation * k_RotationSecondaryTopLoop;
            Quaternion secondaryBottomRotation = rotation * k_RotationSecondaryBottomLoop;


            for (int i = 0; i <= arcVertexCount; i++)
            {
                float currentAngle = Deg2Rad * baseAngle;
                Vector3 basePosition = new Vector3(0, Mathf.Sin(currentAngle) * radius, Mathf.Cos(currentAngle) * radius);

                vertices[i] = primaryTopRotation * basePosition + topSpherePosition;
                vertices[i+bottomPrimaryStartIndex] = primaryBottomRotation * basePosition + bottomSpherePosition;
                vertices[i+topSecondaryStartIndex] = secondaryTopRotation * basePosition + topSpherePosition;
                vertices[i+bottomSecondaryStartIndex] = secondaryBottomRotation * basePosition + bottomSpherePosition;

                baseAngle += (arcLength / arcVertexCount);
            }


            // TODO SEGEMENT MAPPING
            // Create segment connections
            int[] segments = new int[(totalVertices * 2) + 8];

            int segmentCount = segments.Length;
            int baseCount = 0;
            for (int i = 0; i < segmentCount; i+=2)
            {
                segments[i] = baseCount;
                baseCount++;
                segments[i + 1] = baseCount;
            }

            // Link top to bottom

            return buffer.DrawLines(color, ref vertices, ref segments);
        }

        public static int DrawWireArc(this DebugDrawBuffer buffer, Color color, Vector3 center, Quaternion rotation, float radius, float startAngle = 0f, float endAngle = 180f, int arcVertexCount = DefaultCircleVertexCount / 2)
        {
            // We do the plus one to complete the full arc segment, otherwise it would not be every peice
            Vector3[] vertices = new Vector3[arcVertexCount + 1];
            float baseAngle = startAngle;
            float arcLength = endAngle - startAngle;
            for (int i = 0; i <= arcVertexCount; i++)
            {
                float currentAngle = Deg2Rad * baseAngle;
                vertices[i] = (rotation * new Vector3(0, Mathf.Sin(currentAngle) * radius,Mathf.Cos(currentAngle) * radius)) + center;
                baseAngle += (arcLength / arcVertexCount);
            }

            // Create segment connections
            int[] segments = new int[arcVertexCount * 2];
            int segmentCount = segments.Length;
            int baseCount = 0;
            for (int i = 0; i < segmentCount; i+=2)
            {
                segments[i] = baseCount;
                baseCount++;
                segments[i + 1] = baseCount;
            }
            return buffer.DrawLines(color, ref vertices, ref segments);
        }

        public static int DrawWireCircle(this DebugDrawBuffer buffer, Color color, Vector3 center, Quaternion rotation, float radius, int circleVertexCount = DefaultCircleVertexCount)
        {
            Vector3[] vertices = new Vector3[circleVertexCount];
            float radiansInterval = PI * 2f / circleVertexCount;

            // Loop through and figure out the points
            for (int i = 0; i < circleVertexCount; i++)
            {
                float angle = i * radiansInterval;

                // Create base point
                vertices[i] = (rotation * new Vector3(0, math.sin(angle) * radius, math.cos(angle) * radius)) + center;
            }

            int[] segments = new int[circleVertexCount * 2];
            int segmentCount = segments.Length;
            int baseCount = 0;

            for (int i = 0; i < segmentCount; i+=2)
            {
                segments[i] = baseCount;
                baseCount++;
                segments[i + 1] = baseCount;
            }
            segments[segmentCount - 1] = 0;

            return buffer.DrawLines(color, ref vertices, ref segments);
        }

        /// <summary>
        ///     Draw a wireframe cube of a specific color to the buffer.
        /// </summary>
        /// <param name="color">The color which to draw the wire cube with.</param>
        /// <param name="center">The center world position of the cube.</param>
        /// <param name="size">The unit size of the cube</param>
        /// <returns>The created cube's invalidation token.</returns>
        public static int DrawWireCube(this DebugDrawBuffer buffer, Color color, Vector3 center, Quaternion rotation, Vector3 size)
        {
            Vector3[] vertices = GetCubeVertices(center, rotation, size);
            return buffer.DrawLines(color, ref vertices, ref CubeSegmentIndices);
        }

        public static int DrawWireSphere(this DebugDrawBuffer buffer, Color color, Vector3 center, Quaternion rotation, float radius, int circleVertexCount = DefaultCircleVertexCount)
        {
            int pointCount = circleVertexCount * 2;
            Vector3[] vertices = new Vector3[pointCount];

            float radiansInterval = PI * 2f / circleVertexCount;
            Quaternion xRotation = Space.Axis.X.ToRotation() * rotation;
            Quaternion yRotation = Space.Axis.Y.ToRotation() * rotation;

            // Loop through and figure out the points
            for (int i = 0; i < circleVertexCount; i++)
            {
                float angle = i * radiansInterval;

                // Create base points
                vertices[i] = (xRotation * new Vector3(0, math.sin(angle) * radius, math.cos(angle) * radius)) + center;
                vertices[i+circleVertexCount] = (yRotation * new Vector3(0, math.sin(angle) * radius, math.cos(angle) * radius)) + center;
            }

            // Create segment connections
            int[] segments = new int[pointCount * 2];
            int segmentCount = segments.Length;
            int baseCount = 0;
            for (int i = 0; i < segmentCount; i+=2)
            {
                segments[i] = baseCount;
                baseCount++;
                segments[i + 1] = baseCount;
            }
            segments[pointCount - 1] = 0;
            segments[segmentCount - 1] = circleVertexCount;

            return buffer.DrawLines(color, ref vertices, ref segments);
        }
    }
}