// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

namespace GDX
{
    public static class VisualElementStyles
    {
        public const string AlignmentTopLeftClass = "top-left";
        public const string AlignmentTopRightClass = "top-right";
        public const string AlignmentBottomLeftClass = "bottom-left";
        public const string AlignmentBottomRightClass = "bottom-right";

        public static readonly StyleEnum<Position> PositionAbsolute = Position.Absolute;
        public static readonly StyleEnum<Position> PositionRelative = Position.Relative;
        public static readonly StyleEnum<DisplayStyle> DisplayHidden = DisplayStyle.None;
        public static readonly StyleEnum<DisplayStyle> DisplayVisible = DisplayStyle.Flex;
        public static readonly StyleLength LengthOneHundredPercent = new StyleLength(new Length(100f, LengthUnit.Percent));
        public static readonly StyleLength LengthZeroPixel = new StyleLength(new Length(0, LengthUnit.Pixel));

        public enum Alignment
        {
            TopLeft = 0,
            TopRight = 1,
            BottomLeft = 2,
            BottomRight = 3
        }

        public static void ApplyAlignment(this VisualElement element, Alignment alignment)
        {
            switch (alignment)
            {
                case Alignment.TopLeft:
                    element.EnableInClassList(AlignmentTopRightClass, false);
                    element.EnableInClassList(AlignmentBottomLeftClass, false);
                    element.EnableInClassList(AlignmentBottomRightClass, false);

                    element.EnableInClassList(AlignmentTopLeftClass, true);
                    break;
                case Alignment.TopRight:
                    element.EnableInClassList(AlignmentTopLeftClass, false);
                    element.EnableInClassList(AlignmentBottomLeftClass, false);
                    element.EnableInClassList(AlignmentBottomRightClass, false);

                    element.EnableInClassList(AlignmentTopRightClass, true);
                    break;
                case Alignment.BottomLeft:
                    element.EnableInClassList(AlignmentTopLeftClass, false);
                    element.EnableInClassList(AlignmentTopRightClass, false);
                    element.EnableInClassList(AlignmentBottomRightClass, false);

                    element.EnableInClassList(AlignmentBottomLeftClass, true);
                    break;
                case Alignment.BottomRight:
                    element.EnableInClassList(AlignmentTopLeftClass, false);
                    element.EnableInClassList(AlignmentTopRightClass, false);
                    element.EnableInClassList(AlignmentBottomLeftClass, false);

                    element.EnableInClassList(AlignmentBottomRightClass, true);
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Hide(this VisualElement element)
        {
            element.style.display = DisplayHidden;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Fullscreen(this VisualElement element)
        {
            element.style.position = PositionAbsolute;
            element.style.width = LengthOneHundredPercent;
            element.style.height = LengthOneHundredPercent;
            element.style.left = LengthZeroPixel;
            element.style.top = LengthZeroPixel;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Show(this VisualElement element)
        {
            element.style.display = DisplayVisible;
        }
    }
}