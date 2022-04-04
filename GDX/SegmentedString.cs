// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.ConstrainedExecution;
using System.Security;
using Unity.Mathematics;

namespace GDX
{
    public struct SegmentedString
    {
        public int Count;
        char[] m_Characters;

        /// <summary>
        /// x: Start Index
        /// y: Length
        /// z: Stable Hash Code
        /// </summary>
        int3[] m_Segments;


        public string AsString()
        {
            return new string(m_Characters);
        }

        public string AsString(int segmentIndex)
        {
            return new string(m_Characters, m_Segments[segmentIndex].x, m_Segments[segmentIndex].y);
        }

        public char[] AsCharArray()
        {
            return m_Characters;
        }

        public char[] AsCharArray(int segmentIndex)
        {
            char[] returnArray = new char[m_Segments[segmentIndex].y];
            Array.Copy(m_Characters, m_Segments[segmentIndex].x,
                returnArray, 0, m_Segments[segmentIndex].y);
            return returnArray;
        }

        public int GetHashCode(int segmentIndex)
        {
            return m_Segments[segmentIndex].z;
        }
        public int GetOffset(int segmentIndex)
        {
            return m_Segments[segmentIndex].x;
        }
        public int GetSegmentLength(int segmentIndex)
        {
            return m_Segments[segmentIndex].y;
        }

        [SecuritySafeCritical]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static SegmentedString SplitOnNonAlphaNumericToLower(string targetString)
        {
            SegmentedString returnValue = new SegmentedString
            {
                // Copy to a new character array that we will maintain
                m_Characters = targetString.ToCharArray()
            };

            int charactersLength = returnValue.m_Characters.Length;
            returnValue.m_Segments = new int3[charactersLength];

            int hash1 = 5381;
            int hash2 = hash1;
            bool useAlternateHash = false;
            bool isInsideSegment = false;

            int c;
            for (int i = 0; i < charactersLength; i++)
            {
                // Convert our character to its ascii value
                c = returnValue.m_Characters[i];

                // Check character value and shift it if necessary (32)
                if (c >= StringExtensions.AsciiUpperCaseStart && c <= StringExtensions.AsciiUpperCaseEnd)
                {
                    c ^= StringExtensions.AsciiCaseShift;

                    // Update value
                    returnValue.m_Characters[i] = (char)c;
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
                    returnValue.m_Segments[returnValue.Count].x = i;

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
                    returnValue.m_Segments[returnValue.Count].y = i - returnValue.m_Segments[returnValue.Count].x;
                    returnValue.m_Segments[returnValue.Count].z = hash1 + hash2 * 1566083941;
                    returnValue.Count++;
                }
            }

            // Finish segment if we didnt before
            if (isInsideSegment)
            {
                returnValue.m_Segments[returnValue.Count].y = charactersLength - returnValue.m_Segments[returnValue.Count].x;
                returnValue.m_Segments[returnValue.Count].z = hash1 + hash2 * 1566083941;
                returnValue.Count++;
            }
            return returnValue;
        }
    }
}