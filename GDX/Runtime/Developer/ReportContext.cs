// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Developer
{
    public sealed class ReportContext
    {
        public readonly int CharacterWidth;
        public readonly int KeyValuePairWidth;
        public ReportContext(int characterWidth = 120)
        {
            CharacterWidth = characterWidth;
            KeyValuePairWidth = characterWidth / 3;

            float percent = characterWidth / 100f;

        }

        public readonly int ObjectTypeWidth = 19;
        public readonly int ObjectNameTotalWidth = 64;
        public readonly int ObjectSizeWidth = 14;
        public readonly int ObjectInfoWidth = 20;

        public static ReportContext Default = new ReportContext();
    }
}