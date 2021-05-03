// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Developer
{
    public sealed class ReportContext
    {
        public readonly int CharacterWidth;
        public readonly int KeyValuePairWidth;

        public readonly int ObjectTypeWidth;
        public readonly int ObjectNameWidth;
        public readonly int ObjectSizeWidth;
        public readonly int ObjectInfoWidth;

        public ReportContext(int characterWidth = 120)
        {
            CharacterWidth = characterWidth;
            KeyValuePairWidth = characterWidth / 3;

            float percent = characterWidth / 100f;

            ObjectTypeWidth = (int)percent * 15;
            ObjectNameWidth = (int)(percent * 33.333f);
            ObjectSizeWidth = (int)percent * 11;

            ObjectInfoWidth = characterWidth - (ObjectTypeWidth + ObjectNameWidth + ObjectSizeWidth + 3);
        }
    }
}