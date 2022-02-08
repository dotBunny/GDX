﻿// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.


namespace GDX.Classic.Developer.Reports
{
    [HideFromDocFX]
    public readonly struct IntegerDiff
    {
        public readonly float Percentage;
        public readonly int Change;
        public readonly int LeftHandSide;
        public readonly int RightHandSide;

        public IntegerDiff(int lhs, int rhs)
        {
            LeftHandSide = lhs;
            RightHandSide = rhs;


            Change = rhs - lhs;
            if (lhs == 0)
            {
                Percentage = Change;
            }
            else
            {
                // ReSharper disable once PossibleLossOfFraction
                Percentage = 100f * (Change / lhs);
            }
        }

        public string GetOutput(ReportContext context, bool fullWidth = false)
        {
            if (Change == 0)
            {
                return GetBeforeAndAfterOutput();
            }

            // We dont have an idea of the width
            if (fullWidth || context == null)
            {
                return LeftHandSide == 0
                    ? $"{GetBeforeAndAfterOutput()} = {ReportExtensions.PositiveSign(Change)}{Change.ToString()}"
                    : $"{GetBeforeAndAfterOutput()} = {ReportExtensions.PositiveSign(Change)}{Change.ToString().PadRight(12)} {OptionalPercentageOutput()}";
            }

            return
                $"{GetBeforeAndAfterOutput().PadRight(context.KeyValuePairInfoWidth)} {ReportExtensions.PositiveSign(Change)}{Change.ToString().PadRight(12)} {OptionalPercentageOutput()}";
        }

        private string GetBeforeAndAfterOutput()
        {
            return $"{LeftHandSide.ToString()} => {RightHandSide.ToString()}";
        }

        private string OptionalPercentageOutput()
        {
            if (LeftHandSide == 0)
            {
                return null;
            }

            if (Percentage > 0)
            {
                return $" +{UnityEngine.Mathf.RoundToInt(Percentage).ToString()}%";
            }

            if (Percentage < 0)
            {
                return $" {UnityEngine.Mathf.RoundToInt(Percentage).ToString()}%";
            }

            return null;
        }
    }

}