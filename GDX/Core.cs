// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
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
        ///     An empty <see cref="object"/> array useful when things require it.
        /// </summary>
        // ReSharper disable once RedundantArrayCreationExpression, HeapView.ObjectAllocation.Evident
        public static readonly object[] EmptyObjectArray = new object[] { };

        /// <summary>
        ///     A pseudorandom number generated seeded with <see cref="StartTicks"/>.
        /// </summary>
        /// <remarks>Useful for generic randomness where determinism is not required.</remarks>
        public static WELL1024a Random;

        /// <summary>
        ///     The point in tick based time when the core was initialized.
        /// </summary>
        public static readonly long StartTicks;

        /// <summary>
        ///     Has the <see cref="Core"/> main thread initialization happened?
        /// </summary>
        static bool s_InitializedMainThread;

        /// <summary>
        ///     Static constructor.
        /// </summary>
        /// <remarks>Nothing in here can reference the Unity engine and must be thread-safe.</remarks>
        [ExcludeFromCodeCoverage]
        static Core()
        {
            // Record initialization time.
            StartTicks = DateTime.Now.Ticks;

            // The assemblies will change between editor time and compile time so we are going to unfortunately pay a
            // cost to iterate over them and try to find our settings class
            Reflection.InvokeStaticMethod($"GDX.{OverrideClass}", OverrideMethod);

            // Initialize a random provider
            Random = new WELL1024a((uint)StartTicks);

            DictionaryPrimes.SetDefaultPrimes();

            // ReSharper disable UnusedParameter.Local
            AppDomain.CurrentDomain.DomainUnload += (sender, args) =>
            {
                Random.Dispose();
            };
            // ReSharper restore UnusedParameter.Local
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

    }
}