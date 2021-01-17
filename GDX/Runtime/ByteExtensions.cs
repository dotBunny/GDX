// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace GDX
{
    /// <summary>
    ///     <see cref="byte" /> Based Extension Methods
    /// </summary>
    public static class ByteExtensions
    {
        /// <summary>
        ///     <para>Get the hash code value of <paramref name="targetBytes" />, an array of <see cref="System.Byte" />.</para>
        /// </summary>
        /// <remarks>Does NOT get the object's hashcode.</remarks>
        /// <param name="targetBytes">The target array of <see cref="byte" />.</param>
        /// <returns>A <see cref="System.Int32" /> value.</returns>
        public static int GetValueHashCode(this byte[] targetBytes)
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
    }
}