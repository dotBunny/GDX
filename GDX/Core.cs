// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using GDX.Mathematics.Random;

namespace GDX
{
    public static class Core
    {
        public static readonly GDXConfig Config;
        public static readonly long StartTicks;
        public static WELL1024a Random;

        static Core()
        {
            // Record initialization time.
            StartTicks = DateTime.Now.Ticks;

            // Initialize a random provider
            Random = new WELL1024a((uint)StartTicks);

            // Create new config
            Config = new GDXConfig();
        }
    }
}