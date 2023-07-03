// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Experimental.Logging
{
    public static class LogCategory
    {
        public const string UndefinedLabel = "Undefined";
        public const int MinimumIdentifier = 32;
        public const int MaximumIdentifier = 63;
        public const int Default = 0;
        public const int GDX = 1;
        public const int Platform = 2;
        public const int Unity = 3;
        public const int Input = 4;
        public const int Gameplay = 5;
        public const int Test = 6;
        public const int UI = 10;
    }
}