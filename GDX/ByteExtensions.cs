// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX
{
    /// <summary>
    ///     <see cref="byte" /> Based Extension Methods
    /// </summary>
    [VisualScriptingCompatible(2)]
    public static class ByteExtensions
    {
        /// <summary>
        ///     <para>Get the stable hash code of <paramref name="targetBytes" />, an array of <see cref="System.Byte" />.</para>
        /// </summary>
        /// <remarks>Does NOT get the object's hashcode.</remarks>
        /// <param name="targetBytes">The target array of <see cref="byte" />.</param>
        /// <returns>A <see cref="System.Int32" /> value.</returns>
        public static int GetStableHashCode(this byte[] targetBytes)
        {
            unchecked
            {
                const int p = 16777619;
                int hash = (int)2166136261;
                int length = targetBytes.Length;

                for (int i = 0; i < length; i++)
                {
                    hash = (hash ^ targetBytes[i]) * p;
                }

                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;

                return hash;
            }
        }

        /// <summary>
        ///     Are the two provided <see cref="byte" /> arrays the same.
        /// </summary>
        /// <param name="sourceBytes">The left hand side <see cref="byte" /> array to compare.</param>
        /// <param name="targetBytes">The right hand side <see cref="byte" /> array to compare.</param>
        /// <returns>true if they are identical, will also return true if both are null.</returns>
        public static bool IsSame(this byte[] sourceBytes, byte[] targetBytes)
        {
            if (sourceBytes == null)
            {
                return targetBytes == null;
            }

            if (targetBytes == null)
            {
                return false;
            }

            if (sourceBytes.Length != targetBytes.Length)
            {
                return false;
            }

            int count = sourceBytes.Length;
            for (int i = 0; i < count; i++)
            {
                if (sourceBytes[i] != targetBytes[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}