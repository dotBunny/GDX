// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if GDX_PERFORMANCETESTING
using NUnit.Framework;
using Unity.PerformanceTesting;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;

namespace GDX.Experimental.Logging
{
    public class ManagedLogPerformanceTests
    {
        string LogTestMessage = "Hello World!";
        const int k_WarmupCount = 10;
        const int k_MeasurementCount = 20;
        const int k_IterationsPerMeasurement = 100;

        static readonly SampleGroup k_LogGroup = new SampleGroup("Log");
        static readonly SampleGroup k_WarningGroup = new SampleGroup("Warning");

        [Test]
        [Performance]
        [Category(Core.PerformanceCategory)]
        public void Debug_Simple()
        {
            Measure.Method(() =>
                {
                    Debug.Log(LogTestMessage);
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement).SampleGroup(k_LogGroup)
                .Run();

            Measure.Method(() =>
                {
                    Debug.LogWarning(LogTestMessage);
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement).SampleGroup(k_WarningGroup)
                .Run();
        }
        [Test]
        [Performance]
        [Category(Core.PerformanceCategory)]
        public void ManagedLog_Simple_WithOutput()
        {
            Profiler.BeginSample("ManagedLog_Simple_WithOutput");
                Measure.Method(() =>
                {
                    ManagedLog.Info(LogCategory.Test, LogTestMessage);
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement).SampleGroup(k_LogGroup)
                .Run();
            Measure.Method(() =>
                {
                    ManagedLog.Warning(LogCategory.Test, LogTestMessage);
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement).SampleGroup(k_WarningGroup)
                .Run();
            Profiler.EndSample();
        }
        [Test]
        [Performance]
        [Category(Core.PerformanceCategory)]
        public void ManagedLog_Simple_NoOutput()
        {
            ManagedLog.SetUnityOutput(LogCategory.Test, false);
            Measure.Method(() =>
                {
                    ManagedLog.Info(LogCategory.Test, LogTestMessage);
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement).SampleGroup(k_LogGroup)
                .Run();
            Measure.Method(() =>
                {
                    ManagedLog.Warning(LogCategory.Test, LogTestMessage);
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement).SampleGroup(k_WarningGroup)
                .Run();
            ManagedLog.SetUnityOutput(LogCategory.Test, true);
        }
    }
}
#endif // GDX_PERFORMANCETESTING