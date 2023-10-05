// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Logging
{
    public static class LogCategory
    {
        public const string UndefinedLabel = "UNDEFINED";
        public const int MinimumIdentifier = 32;
        public const int MaximumIdentifier = 63;

        // ReSharper disable InconsistentNaming
        public const int DEFAULT = 0;
        public const int GDX = 1;
        public const int PLATFORM = 2;
        public const int UNITY = 3;
        public const int INPUT = 4;
        public const int GAMEPLAY = 5;
        public const int TEST = 6;
        public const int UI = 10;
        // ReSharper restore InconsistentNaming
    }
}