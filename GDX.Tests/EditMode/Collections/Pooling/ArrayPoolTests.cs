// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Collections.Pooling;
using NUnit.Framework;

namespace GDX.Collections.Generic
{
    public class ArrayPoolTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_InitializeWithMinimumAndMaximum()
        {
            int[] minimums = new int[] { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] maximums = new int[] { 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            ArrayPool<int> arrayPool = new ArrayPool<int>(minimums, maximums);

            bool evaluate = arrayPool.ArrayPools is { Length: 31 } && arrayPool.ArrayPools[4].Count == 1 &&
                            arrayPool.ArrayPools[4].Pool is { Length: 2 } && arrayPool.ArrayPools[4].Pool[0] != null &&
                            arrayPool.ArrayPools[4].Pool[0].Length == 16 && arrayPool.ArrayPools[4].Pool[1] == null;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetArrayFromPool_PowerOfTwoRequested_ArrayExistsInPool()
        {
            int[] minimums = new int[] { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] maximums = new int[] { 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            ArrayPool<int> arrayPool = new ArrayPool<int>(minimums, maximums);

            bool evaluate = arrayPool.ArrayPools is { Length: 31 } && arrayPool.ArrayPools[4].Count == 1 &&
                            arrayPool.ArrayPools[4].Pool is
                            {
                                Length: 2
                            } && arrayPool.ArrayPools[4].Pool[0] != null &&
                            arrayPool.ArrayPools[4].Pool[0].Length == 16 && arrayPool.ArrayPools[4].Pool[1] == null;

            if (evaluate)
            {
                int[] array = arrayPool.Get(16);
                evaluate =  array is { Length: 16 };
            }

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetArrayFromPool_PowerOfTwoRequested_PoolIsEmpty()
        {
            int[] minimums = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] maximums = new int[] { 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            ArrayPool<int> arrayPool = new ArrayPool<int>(minimums, maximums);

            bool evaluate = arrayPool.ArrayPools is { Length: 31 } && arrayPool.ArrayPools[4].Count == 0 &&
                            arrayPool.ArrayPools[4].Pool is { Length: 2 } && arrayPool.ArrayPools[4].Pool[0] == null &&
                            arrayPool.ArrayPools[4].Pool[1] == null;

            if (evaluate)
            {
                int[] array = arrayPool.Get(16);
                evaluate = array is { Length: 16 };
            }

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetArrayFromPool_NonPowerOfTwoRequested_ArrayExistsInPool()
        {
            int[] minimums = new int[] { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] maximums = new int[] { 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            ArrayPool<int> arrayPool = new ArrayPool<int>(minimums, maximums);

            bool evaluate = arrayPool.ArrayPools is { Length: 31 } && arrayPool.ArrayPools[4].Count == 1 &&
                            arrayPool.ArrayPools[4].Pool is { Length: 2 } && arrayPool.ArrayPools[4].Pool[0] != null &&
                            arrayPool.ArrayPools[4].Pool[0].Length == 16 && arrayPool.ArrayPools[4].Pool[1] == null;

            if (evaluate)
            {
                int[] array = arrayPool.Get(13);
                evaluate = array is { Length: 16 };
            }

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ReturnArrayToPool_MaximumNotReached()
        {
            int[] arrayToPool = new int[16];
            int[] minimums = new int[] { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] maximums = new int[] { 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            ArrayPool<int> arrayPool = new ArrayPool<int>(minimums, maximums);

            bool evaluate = arrayPool.ArrayPools is { Length: 31 } && arrayPool.ArrayPools[4].Count == 1 &&
                            arrayPool.ArrayPools[4].Pool is
                            {
                                Length: 2
                            } && arrayPool.ArrayPools[4].Pool[0] != null &&
                            arrayPool.ArrayPools[4].Pool[0].Length == 16 && arrayPool.ArrayPools[4].Pool[1] == null;

            if (evaluate)
            {
                arrayPool.Return(arrayToPool);
                evaluate = arrayPool.ArrayPools[4].Count == 2 &&
                           arrayPool.ArrayPools[4].Pool[1] == arrayToPool;
            }

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ReturnArrayToPool_MaximumReached()
        {
            int[] arrayToPool = new int[16];
            int[] arrayToFail = new int[16];
            int[] minimums = new int[] { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] maximums = new int[] { 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            ArrayPool<int> arrayPool = new ArrayPool<int>(minimums, maximums);

            bool evaluate = arrayPool.ArrayPools is { Length: 31 } && arrayPool.ArrayPools[4].Count == 1 &&
                            arrayPool.ArrayPools[4].Pool is { Length: 2 } && arrayPool.ArrayPools[4].Pool[0] != null &&
                            arrayPool.ArrayPools[4].Pool[0].Length == 16 && arrayPool.ArrayPools[4].Pool[1] == null;

            if (evaluate)
            {
                arrayPool.Return(arrayToPool);
                arrayPool.Return(arrayToFail);
                evaluate = arrayPool.ArrayPools[4].Count == 2 &&
                           arrayPool.ArrayPools[4].Pool[1] == arrayToPool &&
                           arrayPool.ArrayPools[4].Pool[0] != arrayToFail;
            }

            Assert.IsTrue(evaluate);
        }
    }
}
