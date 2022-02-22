namespace GDX.Collections.Generic
{
    // ReSharper disable UnusedMember.Global
    // ReSharper disable MemberCanBePrivate.Global

    public static class DictionaryPrimes
    {
        static int[] s_Primes;
        static int s_PrimesLength;

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
            s_Primes = primes;
            s_PrimesLength = primes.Length;
        }

        public static int GetPrime(int min)
        {
            for (int i = 0; i < s_PrimesLength; i++)
            {
                int prime = s_Primes[i];
                if (prime >= min) return prime;
            }
            return int.MaxValue;
        }

        public static int GetPrimeAtIndex(int index)
        {
            return s_Primes[index];
        }

        public static int GetPrimesLength()
        {
            return s_PrimesLength;
        }


        /// <summary>
        /// Returns size of hashtable to grow to.
        /// </summary>
        /// <param name="oldSize"></param>
        /// <returns></returns>
        public static int GetNextSize(int oldSize)
        {
            uint newSize = 2U * unchecked((uint)oldSize);

            const int k_MaxPrime = int.MaxValue;
            newSize = (newSize > k_MaxPrime) ? k_MaxPrime : newSize;

            for (int i = 0; i < s_PrimesLength; i++)
            {
                int prime = s_Primes[i];
                if (prime >= newSize) return prime;
            }

            return k_MaxPrime;
        }
    }
}
