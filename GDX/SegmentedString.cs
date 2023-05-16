// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using Unity.Mathematics;

namespace GDX
{
    /// <summary>
    ///     A segmented collection of <see cref="char"/>.
    /// </summary>
    public struct SegmentedString
    {
        /// <summary>
        ///     The initial array of characters.
        /// </summary>
        char[] m_Characters;

        /// <summary>
        ///     Datastore of word segment information.
        /// </summary>
        /// <remarks>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Axis</term>
        ///             <description>Typical Usage</description>
        ///         </listheader>
        ///         <item>
        ///             <term>x</term>
        ///             <description>The start offset in <see cref="m_Characters"/> of a word.</description>
        ///         </item>
        ///         <item>
        ///             <term>y</term>
        ///             <description>The length of the word.</description>
        ///         </item>
        ///         <item>
        ///             <term>z</term>
        ///             <description>The calculated <see cref="StringExtensions.GetStableHashCode"/> for the word.</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        int3[] m_Segments;

        /// <summary>
        ///     The number of words.
        /// </summary>
        int m_Count;

        /// <summary>
        ///     The calculated <see cref="StringExtensions.GetStableHashCode"/> for the entirety of <see cref="m_Characters"/>.
        /// </summary>
        int m_HashCode;

        /// <summary>
        ///     Get the <see cref="m_Characters"/> array.
        /// </summary>
        /// <returns>A <see cref="char"/> array.</returns>
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

        public string AsString()
        {
            return new string(m_Characters);
        }

        public string AsString(int segmentIndex)
        {
            return new string(m_Characters, m_Segments[segmentIndex].x, m_Segments[segmentIndex].y);
        }


        public int GetCount()
        {
            return m_Count;
        }


        public override int GetHashCode()
        {
            return m_HashCode;
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

        public static SegmentedString SplitOnNonAlphaNumericToLower(string targetString)
        {
            SegmentedString returnValue = new SegmentedString
            {
                // Copy to a new character array that we will maintain
                m_Characters = targetString.ToCharArray()
            };

            int charactersLength = returnValue.m_Characters.Length;
            returnValue.m_Segments = new int3[charactersLength];

            bool isInsideSegment = false;

            for (int i = 0; i < charactersLength; i++)
            {
                // Convert our character to its ascii value
                int c = returnValue.m_Characters[i];

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
                    // Mark start spot
                    returnValue.m_Segments[returnValue.m_Count].x = i;

                    isInsideSegment = true;
                }

                if (!isValid && isInsideSegment)
                {
                    // Close out this iteration of a segment
                    isInsideSegment = false;
                    returnValue.m_Segments[returnValue.m_Count].y = i - returnValue.m_Segments[returnValue.m_Count].x;
                    returnValue.m_Count++;
                }
            }

            // Finish segment if we didnt before
            if (isInsideSegment)
            {
                returnValue.m_Segments[returnValue.m_Count].y =
                    charactersLength - returnValue.m_Segments[returnValue.m_Count].x;
                returnValue.m_Count++;
            }

            return returnValue;
        }

        public static SegmentedString SplitOnNonAlphaNumericToLowerHashed(string targetString)
        {
            SegmentedString returnValue = new SegmentedString
            {
                // Copy to a new character array that we will maintain
                m_Characters = targetString.ToCharArray()
            };

            int charactersLength = returnValue.m_Characters.Length;
            returnValue.m_Segments = new int3[charactersLength];

            int segmentHashA = 5381;
            int segmentHashB = segmentHashA;
            int hashA = 5381;
            int hashB = hashA;
            bool useAlternateHash = false;
            bool useAlternateSegmentHash = false;
            bool isInsideSegment = false;

            for (int i = 0; i < charactersLength; i++)
            {
                // Convert our character to its ascii value
                int c = returnValue.m_Characters[i];

                // Check character value and shift it if necessary (32)
                if (c >= StringExtensions.AsciiUpperCaseStart && c <= StringExtensions.AsciiUpperCaseEnd)
                {
                    c ^= StringExtensions.AsciiCaseShift;

                    // Update value
                    returnValue.m_Characters[i] = (char)c;
                }


                // Hash character for overall hashing
                // Flopping hash
                if (!useAlternateHash)
                {
                    hashA = ((hashA << 5) + hashA) ^ c;
                    useAlternateHash = true;
                }
                else
                {
                    hashB = ((hashB << 5) + hashB) ^ c;
                    useAlternateHash = false;
                }

                // Check our first character
                bool isValid =
                    (c >= StringExtensions.AsciiLowerCaseStart && c <= StringExtensions.AsciiLowerCaseEnd) ||
                    (c >= StringExtensions.AsciiNumberStart && c <= StringExtensions.AsciiNumberEnd);

                // If we are valid, but not in a segment
                if (isValid && !isInsideSegment)
                {
                    // Reset hashes
                    segmentHashA = 5381;
                    segmentHashB = segmentHashA;
                    useAlternateSegmentHash = false;

                    // Mark start spot
                    returnValue.m_Segments[returnValue.m_Count].x = i;

                    isInsideSegment = true;
                }

                if (isValid)
                {
                    // Flopping hash
                    if (!useAlternateSegmentHash)
                    {
                        segmentHashA = ((segmentHashA << 5) + segmentHashA) ^ c;
                        useAlternateSegmentHash = true;
                    }
                    else
                    {
                        segmentHashB = ((segmentHashB << 5) + segmentHashB) ^ c;
                        useAlternateSegmentHash = false;
                    }
                }

                if (!isValid && isInsideSegment)
                {
                    // Close out this iteration of a segment
                    isInsideSegment = false;
                    returnValue.m_Segments[returnValue.m_Count].y = i - returnValue.m_Segments[returnValue.m_Count].x;
                    returnValue.m_Segments[returnValue.m_Count].z = segmentHashA + segmentHashB * 1566083941;
                    returnValue.m_Count++;
                }
            }

            // Finish segment if we didnt before
            if (isInsideSegment)
            {
                returnValue.m_Segments[returnValue.m_Count].y =
                    charactersLength - returnValue.m_Segments[returnValue.m_Count].x;
                returnValue.m_Segments[returnValue.m_Count].z = segmentHashA + segmentHashB * 1566083941;
                returnValue.m_Count++;
            }

            // Save final hash
            returnValue.m_HashCode = hashA + hashB * 1566083941;

            return returnValue;
        }
    }
}