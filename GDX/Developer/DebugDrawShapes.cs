﻿// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;
using Unity.Mathematics;

namespace GDX.Developer
{
    public static class DebugDrawShapes
    {

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
            float radiansInterval = Mathf.PI * 2f / circleVertexCount;

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

        public static int DrawWireCapsule(this DebugDrawBuffer buffer, Color color, Vector3 startSpherePosition, Vector3 endSpherePosition, Quaternion rotation, float radius, int circleVertexCount = DefaultCircleVertexCount)
        {
            // Vector3[] vertices = new Vector3[8];
            // int[] segments = new int[16];
            //
            // Vector3 xRadius = new Vector3(radius, 0, 0);
            // Vector3 zRadius = new Vector3(0, 0, radius);
            //
            // vertices[0] = bottom - xRadius;
            // vertices[1] = bottom + xRadius;
            // segments[0] = 0;
            // segments[1] = 1;
            // vertices[2] = top + xRadius;
            // segments[2] = 1;
            // segments[3] = 2;
            // vertices[3] = top - xRadius;
            // segments[4] = 2;
            // segments[5] = 3;
            // segments[6] = 3;
            // segments[7] = 0;
            //
            // vertices[4] = bottom - zRadius;
            // vertices[5] = bottom + zRadius;
            // segments[8] = 4;
            // segments[9] = 5;
            // vertices[6] = top + zRadius;
            // segments[10] = 5;
            // segments[11] = 6;
            // vertices[7] = top - zRadius;
            // segments[12] = 6;
            // segments[13] = 7;
            // segments[14] = 7;
            // segments[15] = 4;

            //return buffer.DrawLines(color, ref vertices, ref segments);

            // int centerCapsuleHits = Physics.CapsuleCastNonAlloc(
            //     centerPointOfContactSphere,
            //     centerPointOfContactTopSphere,
            //     playerRadius, Vector3.up, raycastHits);


            int pointCount = circleVertexCount * 4;
            Vector3[] vertices = new Vector3[pointCount];

            GetCircleVertices(ref vertices,  0, startSpherePosition, Space.Axis.X.ToRotation() * rotation, radius, circleVertexCount);
            GetCircleVertices(ref vertices, circleVertexCount, startSpherePosition, Space.Axis.Y.ToRotation() * rotation, radius, circleVertexCount);
            GetCircleVertices(ref vertices, circleVertexCount * 2, endSpherePosition, Space.Axis.X.ToRotation() * rotation, radius, circleVertexCount);
            GetCircleVertices(ref vertices, circleVertexCount * 3, endSpherePosition, Space.Axis.Y.ToRotation() * rotation, radius, circleVertexCount);

            int[] segments = new int[pointCount * 2];
            int segmentCount = segments.Length;
            int baseCount = 0;

            for (int i = 0; i < segmentCount; i+=2)
            {
                segments[i] = baseCount;
                baseCount++;
                segments[i + 1] = baseCount;
            }

            segments[circleVertexCount - 1] = 0;
            segments[(circleVertexCount * 2) - 1] = circleVertexCount;
            segments[(circleVertexCount*3) - 1] = circleVertexCount * 2;
            segments[segmentCount - 1] = circleVertexCount * 3;

            return buffer.DrawLines(color, ref vertices, ref segments);
        }

        public static int DrawWireCircle(this DebugDrawBuffer buffer, Color color, Vector3 center, Quaternion rotation, float radius, int circleVertexCount = DefaultCircleVertexCount)
        {
            Vector3[] vertices = new Vector3[circleVertexCount];
            float radiansInterval = Mathf.PI * 2f / circleVertexCount;

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

            float radiansInterval = Mathf.PI * 2f / circleVertexCount;
            Quaternion xRotation = Space.Axis.X.ToRotation();
            Quaternion yRotation = Space.Axis.Y.ToRotation();

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