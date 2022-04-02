// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using GDX.Collections.Generic;
using GDX.Mathematics.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GDX
{
    public static class Core
    {
        public const string OverrideClass = "CustomConfig";
        public const string OverrideMethod = "Init";
        public const string PerformanceCategory = "GDX.Performance";
        public const string TestCategory = "GDX.Tests";

        /// <summary>
        ///     Utilizes the <see cref="CoreSentinel" /> to ensure the static has a destructor of sorts.
        /// </summary>
#pragma warning disable IDE0052, IDE0090
        // ReSharper disable UnusedMember.Local, ArrangeObjectCreationWhenTypeEvident
        static readonly CoreSentinel k_DisposeSentinel = new CoreSentinel();
        // ReSharper restore UnusedMember.Local, ArrangeObjectCreationWhenTypeEvident
#pragma warning restore IDE0052, IDE0090
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

            // Immediately execute the proper initialization process
            Initialize();
        }

        /// <summary>
        ///     Static initializer
        /// </summary>
        /// <remarks>Nothing in here can reference the Unity engine and must be thread-safe.</remarks>
        public static void Initialize()
        {
            if (s_Initialized)
            {
                return;
            }

            // The assemblies will change between editor time and compile time so we are going to unfortunately pay a
            // cost to iterate over them and try to find our settings class
            Reflection.InvokeStaticMethod($"GDX.{OverrideClass}", OverrideMethod);

            // Initialize a random provider
            Random = new WELL1024a((uint)StartTicks);

            DictionaryPrimes.SetDefaultPrimes();

            s_Initialized = true;
        }

        /// <summary>
        ///     Main-thread initializer.
        /// </summary>
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.AfterAssembliesLoaded)]
#endif
        public static void InitializeOnMainThread()
        {
            if (s_InitializedMainThread)
            {
                return;
            }

            Localization.SetDefaultCulture();

            s_InitializedMainThread = true;
        }

        /// <summary>
        ///     Core Destructor
        /// </summary>
        sealed class CoreSentinel
        {
            ~CoreSentinel()
            {
                // Dispose native arrays
                Random.Dispose();
            }
        }
    }
}