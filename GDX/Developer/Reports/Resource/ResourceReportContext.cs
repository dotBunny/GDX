// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if !UNITY_DOTSRUNTIME

namespace GDX.Developer.Reports.Resource
{
    /// <summary>
    ///     Context for generating a <see cref="ResourceReport" /> based report.
    /// </summary>
    /// <exception cref="UnsupportedRuntimeException">Not supported on DOTS Runtime.</exception>
    public sealed class ResourceReportContext
    {
        public readonly int CharacterWidth;
        public readonly int KeyValuePairInfoWidth;
        public readonly int KeyValuePairWidth;
        public readonly int ObjectInfoWidth;
        public readonly int ObjectNameWidth;
        public readonly int ObjectSizeWidth;

        public readonly int ObjectTypeWidth;

        public ResourceReportContext(int characterWidth = 120)
        {
            CharacterWidth = characterWidth;
            KeyValuePairWidth = characterWidth / 3;
            KeyValuePairInfoWidth = (CharacterWidth - KeyValuePairWidth) / 2;

            float percent = characterWidth / 100f;

            ObjectTypeWidth = (int)percent * 15;
            ObjectNameWidth = (int)(percent * 33.333f);
            ObjectSizeWidth = (int)percent * 11;

            ObjectInfoWidth = characterWidth - (ObjectTypeWidth + ObjectNameWidth + ObjectSizeWidth + 3);
        }
    }
}
#endif // !UNITY_DOTSRUNTIME