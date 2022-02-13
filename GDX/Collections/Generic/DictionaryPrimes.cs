namespace GDX.Collections.Generic
{
    // ReSharper disable UnusedMember.Global
    // ReSharper disable MemberCanBePrivate.Global

    public static class DictionaryPrimes
    {
        private static int[] s_primes;
        private static int s_primesLength;

#if UNITY_EDITOR
        /// <summary>
        ///     Initialize the known primes.
        /// </summary>
        static DictionaryPrimes()
#else
        /// <remarks>
        ///     This is kept isolated to attempt to hint to the compiler to avoid adding static initialization checks.
        /// </remarks>
        [UnityEngine.RuntimeInitializeOnLoadMethod]
        static void InitializeOnMainThread()
#endif
        {
            SetDefaultPrimes();
        }

        public static void SetDefaultPrimes()
        {
            SetPrimes( new []{
                17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
                1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
                17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
                187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
                1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369, 12582917, 25165843,
                50331653, 100663319, 201326611, 402653189, 805306457, 1610612741 });
        }

        public static void SetPrimes(int[] primes)
        {
            s_primes = primes;
            s_primesLength = primes.Length;
        }

        public static int GetPrime(int min)
        {
            for (int i = 0; i < s_primesLength; i++)
            {
                int prime = s_primes[i];
                if (prime >= min) return prime;
            }
            return int.MaxValue;
        }

        public static int GetPrimeAtIndex(int index)
        {
            return s_primes[index];
        }

        public static int GetPrimesLength()
        {
            return s_primesLength;
        }


        /// <summary>
        /// Returns size of hashtable to grow to.
        /// </summary>
        /// <param name="oldSize"></param>
        /// <returns></returns>
        public static int GetNextSize(int oldSize)
        {
            uint newSize = 2U * unchecked((uint)oldSize);

            const int MaxPrime = int.MaxValue;
            newSize = (newSize > MaxPrime) ? MaxPrime : newSize;

            for (int i = 0; i < s_primesLength; i++)
            {
                int prime = s_primes[i];
                if (prime >= newSize) return prime;
            }

            return MaxPrime;
        }
    }
}
