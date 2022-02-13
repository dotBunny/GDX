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

        private static bool s_Initialized;
        private static bool s_InitializedMainThread;

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

        // ReSharper disable once MemberCanBePrivate.Global
        public static void Initialize()
        {
            if (s_Initialized) return;

            // Initialize a random provider
            Random = new WELL1024a((uint)StartTicks);

            DictionaryPrimes.SetDefaultPrimes();

            s_Initialized = true;
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.AfterAssembliesLoaded)]
#endif
        public static void InitializeOnMainThread()
        {
            if (s_InitializedMainThread) return;
            s_InitializedMainThread = true;
        }
    }
}