﻿// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using GDX.Experimental;
using GDX.Experimental.Logging;
using GDX.Mathematics.Random;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

namespace GDX
{
    [UnityEngine.Scripting.Preserve]
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
        ///     The main threads identifier.
        /// </summary>
        public static int MainThreadID = -1;

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

        static bool s_InitializedRuntime;

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

            //DictionaryPrimes.SetDefaultPrimes();

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
        /// <remarks>
        ///     <para>
        ///         It might be important to call this function if you are using GDX related configurations inside of
        ///         another <see cref="UnityEngine.RuntimeInitializeOnLoadMethod"/> decorated static method.
        ///     </para>
        ///     <para>
        ///         An example of this sort of usage is in the <see cref="GDX.Threading.TaskDirectorSystem"/>.
        ///     </para>
        /// </remarks>
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.AfterAssembliesLoaded)]
#endif // UNITY_EDITOR
        public static void InitializeOnMainThread()
        {
            if (s_InitializedMainThread)
            {
                return;
            }

            MainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;

            Localization.SetDefaultCulture();

            s_InitializedMainThread = true;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void InitializeAtRuntime()
        {
            if (s_InitializedRuntime) return;

            PlayerLoopSystem systemRoot = PlayerLoop.GetCurrentPlayerLoop();

            // Subscribe our ManagedLog system
            if (Config.EnvironmentManagedLog)
            {
                systemRoot.AddSubSystemToFirstSubSystemOfType(
                    typeof(Initialization),
                    typeof(ManagedLog), ManagedLog.Tick);
            }

            // Subscribe our developer console
#if UNITY_2021_3_OR_NEWER
            if (Config.EnvironmentDeveloperConsole)
            {
                systemRoot.AddSubSystemToFirstSubSystemOfType(
                    typeof(Initialization),
                    typeof(DeveloperConsole), DeveloperConsoleTick);
            }
#endif // UNITY_2021_3_OR_NEWER

            PlayerLoop.SetPlayerLoop(systemRoot);

            s_InitializedRuntime = true;
        }

        static void DeveloperConsoleTick()
        {
#if UNITY_2021_3_OR_NEWER
            // We need to feed in the deltaTime, this could be the previous frames if were being honest about it
            DeveloperConsole.Tick(Time.deltaTime);
#endif // UNITY_2021_3_OR_NEWER
        }
    }
}