// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Developer.Reports
{
    public readonly struct IntegerDiff
    {
        public readonly float Percentage;
        public readonly int Change;
        public readonly int A;
        public readonly int B;

        public IntegerDiff(int lhs, int rhs)
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