// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using GDX.Collections.Pooling;
using GDX.Mathematics.Shapes;
using Unity.Mathematics;
using UnityEngine;

namespace GDX.Mathematics
{
    public static class Shape
    {
        public static bool Overlaps(in AABB first, in AABB second)
        {
            bool overlapsX = (first.Max.x >= second.Min.x) & (first.Min.x <= second.Max.x);
            bool overlapsY = (first.Max.y >= second.Min.y) & (first.Min.y <= second.Max.y);
            bool overlapsZ = (first.Max.z >= second.Min.z) & (first.Min.z <= second.Max.z);
            return overlapsX & overlapsY & overlapsZ;
        }

#if GDX_MATHEMATICS
        public static int Overlaps(AABB[] boxes, int2[] results)
        {
            int collisionCount = 0;
            for (int i = 0; i < boxes.Length; i++)
            {
                ref readonly AABB first = ref boxes[i];
                for (int j = i + 1; j < boxes.Length; j++)
                {
                    ref readonly AABB second = ref boxes[j];
                    bool overlapsX = (first.Max.x >= second.Min.x) & (first.Min.x <= second.Max.x);
                    bool overlapsY = (first.Max.y >= second.Min.y) & (first.Min.y <= second.Max.y);
                    bool overlapsZ = (first.Max.z >= second.Min.z) & (first.Min.z <= second.Max.z);

                    bool collides = overlapsX & overlapsY & overlapsZ;

                    if (collides)
                    {
                        results[collisionCount] = new Unity.Mathematics.int2(i, j);
                        ++collisionCount;
                    }
                }
            }

            return collisionCount;
        }

        public static int Overlaps(AABB[] boxes, int2[] results, int index, int length)
        {
            int collisionCount = 0;
            for (int i = 0; i < length; i++)
            {
                ref readonly AABB first = ref boxes[index + i];
                for (int j = i + 1; j < length; j++)
                {
                    ref readonly AABB second = ref boxes[index + j];
                    bool overlapsX = (first.Max.x >= second.Min.x) & (first.Min.x <= second.Max.x);
                    bool overlapsY = (first.Max.y >= second.Min.y) & (first.Min.y <= second.Max.y);
                    bool overlapsZ = (first.Max.z >= second.Min.z) & (first.Min.z <= second.Max.z);

                    bool collides = overlapsX & overlapsY & overlapsZ;

                    if (collides)
                    {
                        results[collisionCount] = new int2(index + i, index + j);
                        ++collisionCount;
                    }
                }
            }

            return collisionCount;
        }

        public static int Overlaps(AABB[] boxes, ref int2[] results)
        {
            int resultsLength = results.Length;
            int collisionCount = 0;
            for (int i = 0; i < boxes.Length; i++)
            {
                ref readonly AABB first = ref boxes[i];
                for (int j = i + 1; j < boxes.Length; j++)
                {
                    ref readonly AABB second = ref boxes[j];
                    bool overlapsX = (first.Max.x >= second.Min.x) & (first.Min.x <= second.Max.x);
                    bool overlapsY = (first.Max.y >= second.Min.y) & (first.Min.y <= second.Max.y);
                    bool overlapsZ = (first.Max.z >= second.Min.z) & (first.Min.z <= second.Max.z);

                    bool collides = overlapsX & overlapsY & overlapsZ;

                    if (collides)
                    {
                        if (collisionCount >= resultsLength)
                        {
                            resultsLength *= 2;
                            System.Array.Resize(ref results, resultsLength);
                        }

                        results[collisionCount] = new Unity.Mathematics.int2(i, j);
                        ++collisionCount;
                    }
                }
            }

            return collisionCount;
        }

        public static int Overlaps(AABB[] boxes, ref int2[] results, int index, int length)
        {
            int resultsLength = results.Length;
            int collisionCount = 0;
            for (int i = 0; i < length; i++)
            {
                ref readonly AABB first = ref boxes[index + i];
                for (int j = i + 1; j < length; j++)
                {
                    ref readonly AABB second = ref boxes[index + j];
                    bool overlapsX = (first.Max.x >= second.Min.x) & (first.Min.x <= second.Max.x);
                    bool overlapsY = (first.Max.y >= second.Min.y) & (first.Min.y <= second.Max.y);
                    bool overlapsZ = (first.Max.z >= second.Min.z) & (first.Min.z <= second.Max.z);

                    bool collides = overlapsX & overlapsY & overlapsZ;

                    if (collides)
                    {
                        if (collisionCount >= resultsLength)
                        {
                            resultsLength *= 2;
                            System.Array.Resize(ref results, resultsLength);
                        }

                        results[collisionCount] = new int2(index + i, index + j);
                        ++collisionCount;
                    }
                }
            }

            return collisionCount;
        }

        public static int Overlaps(AABB[] boxes, ref int2[] results, in ArrayPool<int2> arrayPool)
        {
            int resultsLength = results.Length;
            int collisionCount = 0;
            for (int i = 0; i < boxes.Length; i++)
            {
                ref readonly AABB first = ref boxes[i];
                for (int j = i + 1; j < boxes.Length; j++)
                {
                    ref readonly AABB second = ref boxes[j];
                    bool overlapsX = (first.Max.x >= second.Min.x) & (first.Min.x <= second.Max.x);
                    bool overlapsY = (first.Max.y >= second.Min.y) & (first.Min.y <= second.Max.y);
                    bool overlapsZ = (first.Max.z >= second.Min.z) & (first.Min.z <= second.Max.z);

                    bool collides = overlapsX & overlapsY & overlapsZ;

                    if (collides)
                    {
                        if (collisionCount >= resultsLength)
                        {
                            Resize(ref results, in arrayPool, resultsLength);
                            resultsLength *= 2;
                        }

                        results[collisionCount] = new int2(i, j);
                        ++collisionCount;
                    }
                }
            }

            return collisionCount;
        }

        public static int Overlaps(AABB[] boxes, ref int2[] results, int index, int length, in ArrayPool<int2> arrayPool)
        {
            int resultsLength = results.Length;
            int collisionCount = 0;
            for (int i = 0; i < length; i++)
            {
                ref readonly AABB first = ref boxes[index + i];
                for (int j = i + 1; j < length; j++)
                {
                    ref readonly AABB second = ref boxes[index + j];
                    bool overlapsX = (first.Max.x >= second.Min.x) & (first.Min.x <= second.Max.x);
                    bool overlapsY = (first.Max.y >= second.Min.y) & (first.Min.y <= second.Max.y);
                    bool overlapsZ = (first.Max.z >= second.Min.z) & (first.Min.z <= second.Max.z);

                    bool collides = overlapsX & overlapsY & overlapsZ;

                    if (collides)
                    {
                        if (collisionCount >= resultsLength)
                        {
                            Resize(ref results, in arrayPool, resultsLength);
                            resultsLength *= 2;
                        }

                        results[collisionCount] = new int2(index + i, index + j);
                        ++collisionCount;
                    }
                }
            }

            return collisionCount;
        }

        public static void Overlaps(AABB[] firsts, AABB[] seconds, int2[] results)
        {
            int collisionCount = 0;
            int firstLength = firsts.Length;
            int secondLength = seconds.Length;
            for (int i = 0; i < firstLength; i++)
            {
                for (int j = 0; j < secondLength; j++)
                {
                    ref readonly AABB first = ref firsts[i];
                    ref readonly AABB second = ref seconds[j];
                    bool overlapsX = (first.Max.x >= second.Min.x) & (first.Min.x <= second.Max.x);
                    bool overlapsY = (first.Max.y >= second.Min.y) & (first.Min.y <= second.Max.y);
                    bool overlapsZ = (first.Max.z >= second.Min.z) & (first.Min.z <= second.Max.z);
                    bool overlaps = overlapsX & overlapsY & overlapsZ;

                    if (overlaps)
                    {
                        results[collisionCount] = new int2(i, j);
                        ++collisionCount;
                    }
                }
            }
        }

        public static int Overlaps(AABB[] firsts, AABB[] seconds, ref int2[] results)
        {
            int resultsLength = results.Length;
            int collisionCount = 0;
            int firstLength = firsts.Length;
            int secondLength = seconds.Length;
            for (int i = 0; i < firstLength; i++)
            {
                ref readonly AABB first = ref firsts[i];
                for (int j = 0; j < secondLength; j++)
                {
                    ref readonly AABB second = ref seconds[j];
                    bool overlapsX = (first.Max.x >= second.Min.x) & (first.Min.x <= second.Max.x);
                    bool overlapsY = (first.Max.y >= second.Min.y) & (first.Min.y <= second.Max.y);
                    bool overlapsZ = (first.Max.z >= second.Min.z) & (first.Min.z <= second.Max.z);

                    bool collides = overlapsX & overlapsY & overlapsZ;

                    if (collides)
                    {
                        if (collisionCount >= resultsLength)
                        {
                            resultsLength *= 2;
                            System.Array.Resize(ref results, resultsLength);
                        }

                        results[collisionCount] = new int2(i, j);
                        ++collisionCount;
                    }
                }
            }

            return collisionCount;
        }

        public static void Overlaps(AABB[] firsts, int firstIndex, int firstLength, AABB[] seconds, int secondIndex, int secondLength, int2[] results)
        {
            int collisionCount = 0;
            for (int i = 0; i < firstLength; i++)
            {
                for (int j = 0; j < secondLength; j++)
                {
                    ref readonly AABB first = ref firsts[firstIndex + i];
                    ref readonly AABB second = ref seconds[secondIndex + j];
                    bool overlapsX = (first.Max.x >= second.Min.x) & (first.Min.x <= second.Max.x);
                    bool overlapsY = (first.Max.y >= second.Min.y) & (first.Min.y <= second.Max.y);
                    bool overlapsZ = (first.Max.z >= second.Min.z) & (first.Min.z <= second.Max.z);
                    bool overlaps = overlapsX & overlapsY & overlapsZ;

                    if (overlaps)
                    {
                        results[collisionCount] = new Unity.Mathematics.int2(firstIndex + i, secondIndex + j);
                        ++collisionCount;
                    }
                }
            }
        }


        public static int Overlaps(AABB[] firsts, int firstIndex, int firstLength, AABB[] seconds, int secondIndex, int secondLength, ref int2[] results)
        {
            int resultsLength = results.Length;
            int collisionCount = 0;
            for (int i = 0; i < firstLength; i++)
            {
                ref readonly AABB first = ref firsts[firstIndex + i];
                for (int j = 0; j < secondLength; j++)
                {
                    ref readonly AABB second = ref seconds[secondIndex + j];
                    bool overlapsX = (first.Max.x >= second.Min.x) & (first.Min.x <= second.Max.x);
                    bool overlapsY = (first.Max.y >= second.Min.y) & (first.Min.y <= second.Max.y);
                    bool overlapsZ = (first.Max.z >= second.Min.z) & (first.Min.z <= second.Max.z);

                    bool collides = overlapsX & overlapsY & overlapsZ;

                    if (collides)
                    {
                        if (collisionCount >= resultsLength)
                        {
                            resultsLength *= 2;
                            System.Array.Resize(ref results, resultsLength);
                        }

                        results[collisionCount] = new int2(firstIndex + i, secondIndex + j);
                        ++collisionCount;
                    }
                }
            }

            return collisionCount;
        }

        public static int Overlaps(AABB[] firsts, AABB[] seconds, ref int2[] results, in ArrayPool<int2> arrayPool)
        {
            int resultsLength = results.Length;
            int collisionCount = 0;
            int firstLength = firsts.Length;
            int secondLength = seconds.Length;
            for (int i = 0; i < firstLength; i++)
            {
                ref readonly AABB first = ref firsts[i];
                for (int j = 0; j < secondLength; j++)
                {
                    ref readonly AABB second = ref seconds[j];
                    bool overlapsX = (first.Max.x >= second.Min.x) & (first.Min.x <= second.Max.x);
                    bool overlapsY = (first.Max.y >= second.Min.y) & (first.Min.y <= second.Max.y);
                    bool overlapsZ = (first.Max.z >= second.Min.z) & (first.Min.z <= second.Max.z);

                    bool collides = overlapsX & overlapsY & overlapsZ;

                    if (collides)
                    {
                        if (collisionCount >= resultsLength)
                        {
                            Resize(ref results, in arrayPool, resultsLength);
                            resultsLength *= 2;
                        }

                        results[collisionCount] = new int2(i, j);
                        ++collisionCount;
                    }
                }
            }

            return collisionCount;
        }

        public static int Overlaps(AABB[] firsts, int firstIndex, int firstLength, AABB[] seconds, int secondIndex, int secondLength, ref int2[] results, in ArrayPool<int2> arrayPool)
        {
            int resultsLength = results.Length;
            int collisionCount = 0;
            for (int i = 0; i < firstLength; i++)
            {
                ref readonly AABB first = ref firsts[firstIndex + i];
                for (int j = 0; j < secondLength; j++)
                {
                    ref readonly AABB second = ref seconds[secondIndex + j];
                    bool overlapsX = (first.Max.x >= second.Min.x) & (first.Min.x <= second.Max.x);
                    bool overlapsY = (first.Max.y >= second.Min.y) & (first.Min.y <= second.Max.y);
                    bool overlapsZ = (first.Max.z >= second.Min.z) & (first.Min.z <= second.Max.z);

                    bool collides = overlapsX & overlapsY & overlapsZ;

                    if (collides)
                    {
                        if (collisionCount >= resultsLength)
                        {
                            Resize(ref results, in arrayPool, resultsLength);
                            resultsLength *= 2;
                        }

                        results[collisionCount] = new int2(firstIndex + i, secondIndex + j);
                        ++collisionCount;
                    }
                }
            }

            return collisionCount;
        }

        public static void Resize(ref int2[] array, in ArrayPool<int2> arrayPool, int arrayLength)
        {
            int newLength = arrayLength * 2;
            int2[] newArray = arrayPool.Get(newLength);
            Array.Copy(array, 0, newArray, 0, newLength);
            arrayPool.Return(array);
            array = newArray;
        }


        #endif

        public static void Overlaps(AABB[] firsts, AABB[] seconds, bool[] results)
        {
            int firstLength = firsts.Length;
            int secondLength = seconds.Length;
            for (int i = 0; i < firstLength; i++)
            {
                for (int j = 0; j < secondLength; j++)
                {
                    ref readonly AABB first = ref firsts[i];
                    ref readonly AABB second = ref seconds[j];
                    bool overlapsX = (first.Max.x >= second.Min.x) & (first.Min.x <= second.Max.x);
                    bool overlapsY = (first.Max.y >= second.Min.y) & (first.Min.y <= second.Max.y);
                    bool overlapsZ = (first.Max.z >= second.Min.z) & (first.Min.z <= second.Max.z);
                    results[(i * firstLength) + j] = overlapsX & overlapsY & overlapsZ;
                }
            }
        }

        public static void Overlaps(AABB[] firsts, int firstIndex, int firstLength, AABB[] seconds, int secondIndex, int secondLength, bool[] results)
        {
            for (int i = 0; i < firstLength; i++)
            {
                for (int j = 0; j < secondLength; j++)
                {
                    ref readonly AABB first = ref firsts[firstIndex + i];
                    ref readonly AABB second = ref seconds[secondIndex + j];
                    bool overlapsX = (first.Max.x >= second.Min.x) & (first.Min.x <= second.Max.x);
                    bool overlapsY = (first.Max.y >= second.Min.y) & (first.Min.y <= second.Max.y);
                    bool overlapsZ = (first.Max.z >= second.Min.z) & (first.Min.z <= second.Max.z);
                    results[(i * firstLength) + j] = overlapsX & overlapsY & overlapsZ;
                }
            }
        }

        public static bool Overlaps(in Sphere first, in Sphere second)
        {
            float combinedRadius = first.Radius + second.Radius;
            float combinedRadiusSqr = combinedRadius * combinedRadius;
            float sqrDist = Vector3.SqrMagnitude(second.Position - first.Position);
            return sqrDist > combinedRadiusSqr;
        }
    }
}