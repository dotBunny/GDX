// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Collections.Generic
{
    public static class DictionaryPrimes
    {
        /// <remarks>
        /// This is kept isolated to attempt to hint to the compiler to avoid adding static initialization checks.
        /// </remarks>
        private static int[] s_primes;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#elif UNITY_2019_1_OR_NEWER
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.AfterAssembliesLoaded)]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
        public static void Init()
        {
            s_primes = new int[]{ 53, 97, 193, 389, 769, 1543, 3079, 6151, 12289, 24593, 49157, 98317, 196613, 393241, 786433, 1572869,
                            3145739, 6291469, 12582917, 25165843, 50331653, 100663319, 201326611, 402653189, 805306457, 1610612741};
        }

        public static int GetPrime(int min)
        {
            for (int i = 0; i < s_primes.Length; i++)
            {
                int prime = s_primes[i];
                if (prime >= min) return prime;
            }
            return int.MaxValue;
        }

        // Returns size of hashtable to grow to.
        public static int GetNextSize(int oldSize)
        {
            uint newSize = 2U * unchecked((uint)oldSize);

            const int maxPrime = int.MaxValue;
            newSize = (newSize > maxPrime) ? maxPrime : newSize;

            for (int i = 0; i < s_primes.Length; i++)
            {
                int prime = s_primes[i];
                if (prime >= newSize) return prime;
            }

            return maxPrime;
        }
    }
}
