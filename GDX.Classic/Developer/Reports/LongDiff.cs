// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.Classic.Developer.Reports
{
    [HideFromDocFX]
    public readonly struct LongDiff
    {
        // ReSharper disable MemberCanBePrivate.Global
        public readonly float Percentage;
        public readonly long Change;
        public readonly long LeftHandSide;
        public readonly long RightHandSide;
        // ReSharper restore MemberCanBePrivate.Global

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
                // ReSharper disable once PossibleLossOfFraction
                Percentage = 100f * (Change / lhs);
            }
        }

        public string GetSizeOutput(ReportContext context, bool fileSize = true, bool fullWidth = false)
        {
            if (Change == 0)
            {
                return GetBeforeAndAfterOutput();
            }

            // We dont have an idea of the width
            if (!fullWidth && context != null)
            {
                return fileSize
                    ? $"{GetBeforeAndAfterOutput().PadRight(context.KeyValuePairInfoWidth)} {ReportExtensions.PositiveSign(Change)}{GDX.Localization.GetHumanReadableFileSize(Change).PadRight(12)} {OptionalPercentageOutput()}"
                    : $"{GetBeforeAndAfterOutput().PadRight(context.KeyValuePairInfoWidth)} {ReportExtensions.PositiveSign(Change)}{Change.ToString().PadRight(12)} {OptionalPercentageOutput()}";
            }

            if (fileSize)
            {
                return LeftHandSide == 0
                    ? $"{GetBeforeAndAfterOutput()} = {ReportExtensions.PositiveSign(Change)}{GDX.Localization.GetHumanReadableFileSize(Change)}"
                    : $"{GetBeforeAndAfterOutput()} = {ReportExtensions.PositiveSign(Change)}{GDX.Localization.GetHumanReadableFileSize(Change),-12} {OptionalPercentageOutput()}";
            }

            return LeftHandSide == 0
                ? $"{GetBeforeAndAfterOutput()} = {ReportExtensions.PositiveSign(Change)}{Change.ToString()}"
                : $"{GetBeforeAndAfterOutput()} = {ReportExtensions.PositiveSign(Change)}{Change.ToString(),-12} {OptionalPercentageOutput()}";
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