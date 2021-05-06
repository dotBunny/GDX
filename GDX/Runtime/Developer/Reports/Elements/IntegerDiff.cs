// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Developer.Reports.Elements
{
    public readonly struct IntegerDiff
    {
        public readonly float Percentage;
        public readonly int Change;

        public IntegerDiff(int lhs, int rhs)
        {
            Percentage = rhs / lhs;
            Change = rhs - lhs;
        }
    }

}