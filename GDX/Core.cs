// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using GDX.Collections.Generic;
using GDX.Mathematics.Random;

namespace GDX
{
    public static class Core
    {
        public const string TestCategory = "GDX.Tests";
        public const string PerformanceCategory = "GDX.Performance";
        public static readonly GDXConfig Config;
        // ReSharper disable once MemberCanBePrivate.Global
        public static readonly long StartTicks;
        public static WELL1024a Random;

        private static bool s_initialized;
        private static bool s_initializedMainThread;

        /// <summary>
        ///     Static initializer.
        /// </summary>
        /// <remarks>Nothing in here can reference the Unity engine and must be thread-safe.</remarks>
        static Core()
        {
            // Record initialization time.
            StartTicks = DateTime.Now.Ticks;

            // Create new config
            Config = new GDXConfig();

            Initialize();
        }

        public static void Initialize()
        {
            if (s_initialized) return;

            // Initialize a random provider
            Random = new WELL1024a((uint)StartTicks);

            DictionaryPrimes.SetDefaultPrimes();

            s_initialized = true;
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
#endif
        public static void InitializeOnMainThread()
        {
            if (s_initializedMainThread) return;
            s_initializedMainThread = true;
        }
    }
}