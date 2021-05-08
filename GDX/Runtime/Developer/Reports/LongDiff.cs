// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Developer.Reports
{
    public readonly struct LongDiff
    {
        public readonly float Percentage;
        public readonly long Change;
        public readonly long A;
        public readonly long B;

        public LongDiff(long lhs, long rhs)
        {
            A = lhs;
            B = rhs;

            Change = rhs - lhs;
            if (lhs == 0)
            {
                Percentage = Change;
            }
            else
            {
                Percentage = Change / lhs;
            }
        }
    }
}