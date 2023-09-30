// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX
{
    public static class RectExtensions
    {
        public static Rect GetCenteredRect(this Rect parentWindowPosition, Vector2 size, float safeZone = 0.9f)
        {
            Rect centeredWindowPosition = new Rect()
            {
                x = 0.0f,
                y = 0.0f,
                width = Mathf.Min(size.x, parentWindowPosition.width * safeZone),
                height = Mathf.Min(size.y, parentWindowPosition.height * safeZone)
            };
            float num1 = (float) (((double) parentWindowPosition.width - (double) centeredWindowPosition.width) * 0.5);
            float num2 = (float) (((double) parentWindowPosition.height - (double) centeredWindowPosition.height) * 0.5);
            centeredWindowPosition.x = parentWindowPosition.x + num1;
            centeredWindowPosition.y = parentWindowPosition.y + num2;
            return centeredWindowPosition;
        }
    }
}