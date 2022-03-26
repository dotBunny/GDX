// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using GDX.Collections.Generic;
using GDX.Mathematics.Random;

namespace GDX
{
    public static class Core
    {
        /// <summary>
        /// Core Destructor
        /// </summary>
        sealed class CoreSentinel
        {
            ~CoreSentinel()
            {
                // Dispose native arrays
                Random.Dispose();
            }
        }

        /// <summary>
        /// Utilizes the <see cref="CoreSentinel"/> to ensure the static has a destructor of sorts.
        /// </summary>
#pragma warning disable IDE0052, IDE0090
// ReSharper disable UnusedMember.Local, ArrangeObjectCreationWhenTypeEvident
        static readonly CoreSentinel k_DisposeSentinel = new CoreSentinel();
// ReSharper restore UnusedMember.Local, ArrangeObjectCreationWhenTypeEvident
#pragma warning restore IDE0052, IDE0090
        public const string OverrideClass = "GDXSettings";
        public const string OverrideMethod = "Init";

        public const string TestCategory = "GDX.Tests";
        public const string PerformanceCategory = "GDX.Performance";
        public static readonly GDXConfig Config;
        public static readonly long StartTicks;
        public static WELL1024a Random;

        static bool s_Initialized;
        static bool s_InitializedMainThread;

        /// <summary>
        ///     Static constructor.
        /// </summary>
        /// <remarks>Nothing in here can reference the Unity engine and must be thread-safe.</remarks>
        static Core()
        {
            // Record initialization time.
            StartTicks = DateTime.Now.Ticks;

            // Create new config
            Config = new GDXConfig();

            // Immediately execute the proper initialization process
            Initialize();
        }

        /// <summary>
        ///     Static initializer
        /// </summary>
        /// <remarks>Nothing in here can reference the Unity engine and must be thread-safe.</remarks>
        public static void Initialize()
        {
            if (s_Initialized) return;

            // The assemblies will change between editor time and compile time so we are going to unfortunately pay a
            // cost to iterate over them and try to find our settings class
            foreach (Assembly targetAssembly in Platform.GetLoadedAssemblies())
            {
                Type overrideType = targetAssembly.GetType($"GDX.{OverrideClass}");
                if (overrideType == null)
                {
                    continue;
                }

                MethodInfo initMethod = overrideType.GetMethod(OverrideMethod, BindingFlags.Static|BindingFlags.Public);
                if (initMethod != null)
                {
                    initMethod.Invoke(null, new object[] {});
                }

                break;
            }

            // Initialize a random provider
            Random = new WELL1024a((uint)StartTicks);

            DictionaryPrimes.SetDefaultPrimes();

            s_Initialized = true;
        }

        /// <summary>
        ///     Main-thread initializer.
        /// </summary>
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