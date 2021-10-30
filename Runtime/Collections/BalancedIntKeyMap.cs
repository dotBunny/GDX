// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.


namespace GDX.Collections.Generic
{
	public struct BalancedIntKeyMap<TValue>
    {
        public IntKeyValuePair<TValue>[] Entries;
        public int EmptyKey;
        public int Count;
        public int LengthMinusOne; // Stored explicitly to save a little time during lookup.
        public int MaxProbeDistance;
        public byte FibonacciShift; // Equal to (32 - N), where N is the log of the current length, which for powers of two means the "bit index" of the current array length.
        public byte MaxSize; // The size beyond which the map should fall back to prioritizing memory over search time. Stored as (1 << MaxSize).
        public short Padding;

        public void Add(int key, TValue value)
        {
            if (Count > (LengthMinusOne >> 1))
            {
                Expand();
            }

            int emptyKey = EmptyKey;

            while (key != emptyKey)
            {
                int maxProbeDistance = MaxProbeDistance;
                int fibonacciShift = FibonacciShift;
                int lengthMinusOne = LengthMinusOne;
                int fibonacciHash = unchecked((int)(unchecked((uint)key) * 2654435769)); // The magic number is equal to (1 << 32) divided by the golden ratio.
                int idealIndex = fibonacciHash >> fibonacciShift;
                IntKeyValuePair<TValue>[] entries = Entries;

                for (int distance = 0; distance <= maxProbeDistance; distance++)
                {
                    int index = (idealIndex + distance) & lengthMinusOne;
                    int otherKey = entries[index].Key;
                    TValue otherValue = entries[index].Value;

                    int idealIndexForOther = otherKey & lengthMinusOne;

                    int distanceDefault = index - idealIndexForOther;
                    int distanceWrapped = (lengthMinusOne + 1 - index) + idealIndexForOther;

                    int distanceForOther = distanceDefault >= 0 ? distanceDefault : distanceWrapped;

                    bool isFurtherThanOther = distanceForOther < distance;

                    distance = isFurtherThanOther ? distanceForOther : distance;
                    distance = otherKey == emptyKey ? maxProbeDistance : distance;

                    if (otherKey == emptyKey || isFurtherThanOther)
                    {
                        entries[index].Key = key;
                        entries[index].Value = value;
                        key = otherKey;
                        value = otherValue;
                    }
                }

                if (key != emptyKey)
                {
                    Expand();
                }
            }

            ++Count;
        }

        public void Remove(int key)
        {
            int emptyKey = EmptyKey;
            int maxProbeDistance = MaxProbeDistance;
            int fibonacciShift = FibonacciShift;
            int lengthMinusOne = LengthMinusOne;
            int fibonacciHash = unchecked((int)(unchecked((uint)key) * 2654435769)); // The magic number is equal to (1 << 32) divided by the golden ratio.
            int idealIndex = fibonacciHash >> fibonacciShift;
            IntKeyValuePair<TValue>[] entries = Entries;

            for (int distance = 0; distance <= maxProbeDistance; distance++)
            {
                int index = (idealIndex + distance) & lengthMinusOne;

                if (entries[index].Key == key)
                {
                    entries[index].Key = EmptyKey;
                    entries[index].Value = default(TValue);
                    --Count;
                    return;
                }
            }
        }

        public void Expand()
        {
            int emptyKey = EmptyKey;
            int fibonacciShift = FibonacciShift;
            int oldLength = LengthMinusOne + 1;
            int newLength = oldLength;

            IntKeyValuePair<TValue>[] currEntries = Entries;
            IntKeyValuePair<TValue>[] newEntries = null;

            bool sizeFitsAllEntries = false;

            // Expand the array and try to place all entries in it within the max probe distance.
            // If they won't fit, expand and try again until they do.
            while (!sizeFitsAllEntries)
            {
                sizeFitsAllEntries = true;

                newLength <<= 1;
                Pooling.LongDoubleConversionUnion u;
                u.doubleValue = 0.0;
                u.longValue = 0x4330000000000000L + newLength;
                u.doubleValue -= 4503599627370496.0;
                int newMaxProbeDistance = (int)(u.longValue >> 52) - 0x3FF;

                fibonacciShift = 32 - newMaxProbeDistance;

                if (newLength >= (1 << MaxSize))
                {
                    newLength = MaxSize;
                    newMaxProbeDistance = newLength;
                }

                FibonacciShift = (byte)fibonacciShift;
                MaxProbeDistance = newMaxProbeDistance;
                LengthMinusOne = newLength - 1;

                newEntries = new IntKeyValuePair<TValue>[newLength];
                for (int i = 0; i < newLength; i++)
                {
                    newEntries[i].Key = emptyKey;
                }

                for (int i = 0; i < oldLength && sizeFitsAllEntries; i++)
                {
                    ref IntKeyValuePair<TValue> entry = ref currEntries[i];

                    int key = entry.Key;
                    TValue value = entry.Value;

                    int fibonacciHash = unchecked((int)(unchecked((uint)key) * 2654435769)); // The magic number is equal to (1 << 32) divided by the golden ratio.
                    int idealIndex = fibonacciHash >> fibonacciShift;

                    while (key != emptyKey && sizeFitsAllEntries)
                    {
                        bool found = false;
                        for (int j = 0; j < newMaxProbeDistance; j++)
                        {
                            int index = (idealIndex + j) & (newLength - 1);
                            int newKey = currEntries[index].Key;
                            TValue newValue = currEntries[index].Value;
                            int newKeyFibonacciHash = unchecked((int)(unchecked((uint)newKey) * 2654435769)); // The magic number is equal to (1 << 32) divided by the golden ratio.
                            int newKeyIdealIndex = newKeyFibonacciHash >> fibonacciShift;
                            int newKeyDistance = index >= newKeyIdealIndex ? index - newKeyIdealIndex : newKeyIdealIndex + (newLength - index);

                            if (newKey == emptyKey || newKeyDistance < j)
                            {
                                currEntries[index].Key = key;
                                currEntries[index].Value = value;
                                key = newKey;
                                value = newValue;
                                fibonacciHash = newKeyFibonacciHash;
                                idealIndex = newKeyIdealIndex;
                                found = true;
                                break;
                            }
                        }

                        sizeFitsAllEntries &= found;
                    }
                }
            }
        }
    }
}

