// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.ConstrainedExecution;
using System.Security;
using Unity.Mathematics;

namespace GDX
{
    public struct SegmentedString
    {
        public char[] Characters;

        /// <summary>
        /// x: Start Index
        /// y: Length
        /// z: Stable Hash Code
        /// </summary>
        public int3[] Definitions;
        public int Count;

        public string GetSegment(int index)
        {
            return new string(Characters, Definitions[index].x, Definitions[index].y);
        }
        public int GetSegmentHashCode(int index)
        {
            return Definitions[index].z;
        }
        public int GetSegmentStartIndex(int index)
        {
            return Definitions[index].x;
        }
        public int GetSegmentLength(int index)
        {
            return Definitions[index].y;
        }

        [SecuritySafeCritical]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static SegmentedString SplitOnNonAlphaNumericToLowerHashed(string targetString)
        {
            SegmentedString returnValue = new SegmentedString
            {
                // Copy to a new character array that we will maintain
                Characters = targetString.ToCharArray()
            };

            int charactersLength = returnValue.Characters.Length;
            returnValue.Definitions = new int3[charactersLength];

            int hash1 = 5381;
            int hash2 = hash1;
            bool useAlternateHash = false;
            bool isInsideSegment = false;

            int c;
            for (int i = 0; i < charactersLength; i++)
            {
                // Convert our character to its ascii value
                c = returnValue.Characters[i];

                // Check character value and shift it if necessary (32)
                if (c >= StringExtensions.AsciiUpperCaseStart && c <= StringExtensions.AsciiUpperCaseEnd)
                {
                    c ^= StringExtensions.AsciiCaseShift;

                    // Update value
                    returnValue.Characters[i] = (char)c;
                }

                // Check our first character
                bool isValid =
                    (c >= StringExtensions.AsciiLowerCaseStart && c <= StringExtensions.AsciiLowerCaseEnd) ||
                    (c >= StringExtensions.AsciiNumberStart && c <= StringExtensions.AsciiNumberEnd);

                // If we are valid, but not in a segment
                if (isValid && !isInsideSegment)
                {
                    // Reset hashes
                    hash1 = 5381;
                    hash2 = hash1;
                    useAlternateHash = false;

                    // Mark start spot
                    returnValue.Definitions[returnValue.Count].x = i;

                    isInsideSegment = true;
                }

                if (isValid)
                {
                    // Flopping hash
                    if (!useAlternateHash)
                    {
                        hash1 = ((hash1 << 5) + hash1) ^ c;
                        useAlternateHash = true;
                    }
                    else
                    {
                        hash2 = ((hash2 << 5) + hash2) ^ c;
                        useAlternateHash = false;
                    }
                }

                if (!isValid && isInsideSegment)
                {
                    // Close out this iteration of a segment
                    isInsideSegment = false;
                    returnValue.Definitions[returnValue.Count].y = i - returnValue.Definitions[returnValue.Count].x;
                    returnValue.Definitions[returnValue.Count].z = hash1 + hash2 * 1566083941;
                    returnValue.Count++;
                }
            }

            // Finish segment if we didnt before
            if (isInsideSegment)
            {
                returnValue.Definitions[returnValue.Count].y = charactersLength - returnValue.Definitions[returnValue.Count].x;
                returnValue.Definitions[returnValue.Count].z = hash1 + hash2 * 1566083941;
                returnValue.Count++;
            }
            return returnValue;
        }
    }
}