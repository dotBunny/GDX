// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Collections.Generic
{
    /// <summary>
    /// A HashMap optimized for getting and setting values assigned to an Int32 key.
    /// Not intended for insertion, deletion, or speculatlive lookup of entries that may not be in the HashMap.
    /// This HashMap is sized at powers of two, and performs poorly when many keys share the same lower bits relative to the array's capacity.
    /// Keys and values are stored together in this variant, which is better when a key is close to its ideal index but worse when it is far away.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public struct ReadOnlyIntKeyMap<TValue>
    {
        public IntKeyValuePair<TValue>[] Entries;
        public int EmptyKey;
        public int Count;

        public int LengthMinusOne; // Stored explicitly to save a little time during lookup.
        public byte FibonacciShift; // Equal to (32 - N), where N is the log of the current length, which for powers of two means the "bit index" of the current array length.
        public byte pad0;
        public short pad1;

        public TValue this[int key]
        {
            get
            {
                IntKeyValuePair<TValue>[] entries = Entries;
                int lengthMinusOne = LengthMinusOne;
                int fibonacciShift = FibonacciShift;
                int fibonacciHash = unchecked((int)(unchecked((uint)key) * 2654435769)); // The magic number is equal to (1 << 32) divided by the golden ratio.
                int idealIndex = fibonacciHash >> fibonacciShift;

                for (int i = 0; i <= lengthMinusOne; i++)
                {
                    int index = idealIndex + i & lengthMinusOne;

                    ref IntKeyValuePair<TValue> currEntry = ref entries[index];
                    if (currEntry.Key == key)
                    {
                        // found
                        return currEntry.Value;
                    }
                }

                return default(TValue);
            }

            set
            {
                IntKeyValuePair<TValue>[] entries = Entries;
                int lengthMinusOne = LengthMinusOne;
                int fibonacciShift = FibonacciShift;
                int fibonacciHash = unchecked((int)(unchecked((uint)key) * 2654435769));
                int idealIndex = fibonacciHash >> fibonacciShift;

                for (int i = 0; i <= lengthMinusOne; i++)
                {
                    int index = idealIndex + i & lengthMinusOne;

                    ref IntKeyValuePair<TValue> currEntry = ref entries[index];
                    if (currEntry.Key == key)
                    {
                        // found
                        currEntry.Value = value;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// A HashMap optimized for getting and setting values assigned to an Int32 key.
        /// Not intended for insertion, deletion, or speculatlive lookup of entries that may not be in the HashMap.
        /// This HashMap is sized at powers of two, and performs poorly when many keys share the same lower bits relative to the array's capacity.
        /// Keys and values are stored separately in this variant, which is worse when a key is close to its ideal index but better when it is far away.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        public struct Split
        {
            public int[] Keys;
            public TValue[] Values;
            public int EmptyKey;
            public int Count;

            public int LengthMinusOne; // Stored explicitly to save a little time during lookup.
            public byte FibonacciShift; // The log of the current length, which for powers of two means the "bit index" of the current array length.
            public byte pad0;
            public short pad1;

            public TValue this[int key]
            {
                get
                {
                    int[] keys = Keys;
                    int lengthMinusOne = LengthMinusOne;
                    int fibonacciShift = FibonacciShift;
                    //int idealIndex = key & lengthMinusOne;
                    int fibonacciHash = unchecked((int)(unchecked((uint)key) * 2654435769));
                    int idealIndex = fibonacciHash >> fibonacciShift;

                    int keyAtIndex = keys[idealIndex];

                    for (int i = 0; i <= lengthMinusOne; i++)
                    {
                        int index = idealIndex + i & lengthMinusOne;

                        int currKey = keys[index];
                        if (currKey == key)
                        {
                            // found
                            return Values[index];
                        }
                    }

                    return default(TValue);
                }

                set
                {
                    int[] keys = Keys;
                    int lengthMinusOne = LengthMinusOne;
                    int fibonacciShift = FibonacciShift;
                    //int idealIndex = key & (length - 1);
                    int fibonacciHash = unchecked((int)(unchecked((uint)key) * 2654435769));
                    int idealIndex = fibonacciHash >> fibonacciShift;

                    for (int i = 0; i <= lengthMinusOne; i++)
                    {
                        int index = idealIndex + i & lengthMinusOne;

                        int currKey = keys[index];
                        if (currKey == key)
                        {
                            // found
                            Values[index] = value;
                            return;
                        }
                    }
                }
            }
        }
    }
}