// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using GDX.Collections.Pooling;
using NUnit.Framework;

// ReSharper disable HeapView.ObjectAllocation
// ReSharper disable UnusedVariable

namespace Runtime.Collections.Generic
{
    public class ArrayPoolTests
    {
        [Test]
        [Category(GDX.Core.TestCategory)]
        public void Constructor_InitializeWithMinimumAndMaximum()
        {
            int[] minimums = new int[] { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] maximums = new int[] { 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            ArrayPool<int> arrayPool = new ArrayPool<int>(minimums, maximums);

            bool evaluate = arrayPool.ArrayPools != null &&
                            arrayPool.ArrayPools.Length == 31 &&
                            arrayPool.ArrayPools[4].Count == 1 &&
                            arrayPool.ArrayPools[4].Pool != null &&
                            arrayPool.ArrayPools[4].Pool.Length == 2 &&
                            arrayPool.ArrayPools[4].Pool[0] != null &&
                            arrayPool.ArrayPools[4].Pool[0].Length == 16 &&
                            arrayPool.ArrayPools[4].Pool[1] == null;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void GetArrayFromPool_PowerOfTwoRequested_ArrayExistsInPool()
        {
            int[] minimums = new int[] { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] maximums = new int[] { 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            ArrayPool<int> arrayPool = new ArrayPool<int>(minimums, maximums);

            bool evaluate = arrayPool.ArrayPools != null &&
                            arrayPool.ArrayPools.Length == 31 &&
                            arrayPool.ArrayPools[4].Count == 1 &&
                            arrayPool.ArrayPools[4].Pool != null &&
                            arrayPool.ArrayPools[4].Pool.Length == 2 &&
                            arrayPool.ArrayPools[4].Pool[0] != null &&
                            arrayPool.ArrayPools[4].Pool[0].Length == 16 &&
                            arrayPool.ArrayPools[4].Pool[1] == null;

            if (evaluate)
            {
                int[] array = arrayPool.Get(16);

                evaluate = evaluate &&
                            array != null &&
                            array.Length == 16;
            }

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void GetArrayFromPool_PowerOfTwoRequested_PoolIsEmpty()
        {
            int[] minimums = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] maximums = new int[] { 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            ArrayPool<int> arrayPool = new ArrayPool<int>(minimums, maximums);

            bool evaluate = arrayPool.ArrayPools != null &&
                            arrayPool.ArrayPools.Length == 31 &&
                            arrayPool.ArrayPools[4].Count == 0 &&
                            arrayPool.ArrayPools[4].Pool != null &&
                            arrayPool.ArrayPools[4].Pool.Length == 2 &&
                            arrayPool.ArrayPools[4].Pool[0] == null &&
                            arrayPool.ArrayPools[4].Pool[1] == null;

            if (evaluate)
            {
                int[] array = arrayPool.Get(16);

                evaluate = evaluate &&
                            array != null &&
                            array.Length == 16;
            }

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void GetArrayFromPool_NonPowerOfTwoRequested_ArrayExistsInPool()
        {
            int[] minimums = new int[] { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] maximums = new int[] { 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            ArrayPool<int> arrayPool = new ArrayPool<int>(minimums, maximums);

            bool evaluate = arrayPool.ArrayPools != null &&
                            arrayPool.ArrayPools.Length == 31 &&
                            arrayPool.ArrayPools[4].Count == 1 &&
                            arrayPool.ArrayPools[4].Pool != null &&
                            arrayPool.ArrayPools[4].Pool.Length == 2 &&
                            arrayPool.ArrayPools[4].Pool[0] != null &&
                            arrayPool.ArrayPools[4].Pool[0].Length == 16 &&
                            arrayPool.ArrayPools[4].Pool[1] == null;

            if (evaluate)
            {
                int[] array = arrayPool.Get(13);

                evaluate = evaluate &&
                            array != null &&
                            array.Length == 16;
            }

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void ReturnArrayToPool_MaximumNotReached()
        {
            int[] arrayToPool = new int[16];
            int[] minimums = new int[] { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] maximums = new int[] { 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            ArrayPool<int> arrayPool = new ArrayPool<int>(minimums, maximums);

            bool evaluate = arrayPool.ArrayPools != null &&
                            arrayPool.ArrayPools.Length == 31 &&
                            arrayPool.ArrayPools[4].Count == 1 &&
                            arrayPool.ArrayPools[4].Pool != null &&
                            arrayPool.ArrayPools[4].Pool.Length == 2 &&
                            arrayPool.ArrayPools[4].Pool[0] != null &&
                            arrayPool.ArrayPools[4].Pool[0].Length == 16 &&
                            arrayPool.ArrayPools[4].Pool[1] == null;

            if (evaluate)
            {
                arrayPool.Return(arrayToPool);

                evaluate = evaluate &&
                            arrayPool.ArrayPools[4].Count == 2 &&
                            arrayPool.ArrayPools[4].Pool[1] == arrayToPool;
            }

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void ReturnArrayToPool_MaximumReached()
        {
            int[] arrayToPool = new int[16];
            int[] arrayToFail = new int[16];
            int[] minimums = new int[] { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] maximums = new int[] { 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            ArrayPool<int> arrayPool = new ArrayPool<int>(minimums, maximums);

            bool evaluate = arrayPool.ArrayPools != null &&
                            arrayPool.ArrayPools.Length == 31 &&
                            arrayPool.ArrayPools[4].Count == 1 &&
                            arrayPool.ArrayPools[4].Pool != null &&
                            arrayPool.ArrayPools[4].Pool.Length == 2 &&
                            arrayPool.ArrayPools[4].Pool[0] != null &&
                            arrayPool.ArrayPools[4].Pool[0].Length == 16 &&
                            arrayPool.ArrayPools[4].Pool[1] == null;

            if (evaluate)
            {
                arrayPool.Return(arrayToPool);
                arrayPool.Return(arrayToFail);
                evaluate = evaluate &&
                            arrayPool.ArrayPools[4].Count == 2 &&
                            arrayPool.ArrayPools[4].Pool[1] == arrayToPool &&
                            arrayPool.ArrayPools[4].Pool[0] != arrayToFail;
            }

            Assert.IsTrue(evaluate);
        }
    }
}
