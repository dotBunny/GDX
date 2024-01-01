// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if !UNITY_DOTSRUNTIME

using UnityEngine;

namespace GDX.Developer.Reports.Resource
{
    [HideFromDocFX]
    public readonly struct LongDiff
    {
        public readonly float Percentage;
        public readonly long Change;
        public readonly long LeftHandSide;
        public readonly long RightHandSide;

        public LongDiff(long lhs, long rhs)
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
                Percentage = 100f * ((float)Change / lhs);
            }
        }


        public string GetSizeOutput(ResourceReportContext context, bool fileSize = true, bool fullWidth = false)
        {
            if (Change == 0)
            {
                return GetBeforeAndAfterOutput();
            }

            // We dont have an idea of the width
            if (!fullWidth && context != null)
            {
                return fileSize
                    ? $"{GetBeforeAndAfterOutput().PadRight(context.KeyValuePairInfoWidth)} {ResourceReport.PositiveSign(Change)}{Localization.GetHumanReadableFileSize(Change).PadRight(12)} {OptionalPercentageOutput()}"
                    : $"{GetBeforeAndAfterOutput().PadRight(context.KeyValuePairInfoWidth)} {ResourceReport.PositiveSign(Change)}{Change.ToString().PadRight(12)} {OptionalPercentageOutput()}";
            }

            if (fileSize)
            {
                return LeftHandSide == 0
                    ? $"{GetBeforeAndAfterOutput()} = {ResourceReport.PositiveSign(Change)}{Localization.GetHumanReadableFileSize(Change)}"
                    : $"{GetBeforeAndAfterOutput()} = {ResourceReport.PositiveSign(Change)}{Localization.GetHumanReadableFileSize(Change),-12} {OptionalPercentageOutput()}";
            }

            return LeftHandSide == 0
                ? $"{GetBeforeAndAfterOutput()} = {ResourceReport.PositiveSign(Change)}{Change.ToString()}"
                : $"{GetBeforeAndAfterOutput()} = {ResourceReport.PositiveSign(Change)}{Change.ToString(),-12} {OptionalPercentageOutput()}";
        }

        string GetBeforeAndAfterOutput()
        {
            return $"{LeftHandSide.ToString()} => {RightHandSide.ToString()}";
        }

        string OptionalPercentageOutput()
        {
            if (LeftHandSide == 0)
            {
                return null;
            }

            if (Percentage > 0)
            {
                return $" +{Mathf.RoundToInt(Percentage).ToString()}%";
            }

            if (Percentage < 0)
            {
                return $" {Mathf.RoundToInt(Percentage).ToString()}%";
            }

            return null;
        }
    }
}
#endif // !UNITY_DOTSRUNTIME