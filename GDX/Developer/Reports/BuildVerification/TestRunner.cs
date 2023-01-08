// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using GDX.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GDX.Developer.Reports.BuildVerification
{
    public static class TestRunner
    {
        /// <summary>
        ///
        /// </summary>
        const int SafeDelayTime = 1;
        static readonly object s_lockKnownTests = new object();
        static SimpleList<ITestBehaviour> s_KnownTest = new SimpleList<ITestBehaviour>(10);

        public static void AddTest(SimpleTestBehaviour simpleTest)
        {
            if (simpleTest == null) return;

            lock (s_lockKnownTests)
            {
                if (!s_KnownTest.ContainsReference(simpleTest))
                {
                    s_KnownTest.AddWithExpandCheck(simpleTest);
                }
            }
        }

        public static async Task Execute(TestScene[] scenes)
        {
            for (int testSceneIndex = 1; testSceneIndex < scenes.Length; testSceneIndex++)
            {
                await Execute(scenes[testSceneIndex]);
            }
        }

        public static async Task Execute(TestScene testScene)
        {
            if (!testScene.IsValid())
            {
                Trace.Output(Trace.TraceLevel.Warning, $"[BVT] Invalid scene {testScene.BuildIndex.ToString()}.");
                Reset();
                return;
            }

            Stopwatch timeoutTimer = new Stopwatch();

            Trace.Output(Trace.TraceLevel.Info, $"[BVT] Load {testScene.ScenePath} ({testScene.BuildIndex.ToString()})");
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(testScene.BuildIndex, LoadSceneMode.Additive);
            timeoutTimer.Restart();
            while (!loadOperation.isDone)
            {
                if (timeoutTimer.ElapsedMilliseconds < testScene.LoadTimeout)
                {
                    await Task.Delay(SafeDelayTime);
                }
                else
                {
                    Trace.Output(Trace.TraceLevel.Error, $"[BVT] Failed to load {testScene.ScenePath} ({testScene.BuildIndex.ToString()}).");
                    Reset();
                    return;
                }
            }

            // Restart timer for timeout
            timeoutTimer.Restart();
            while (HasRemainingTests())
            {
                if (timeoutTimer.ElapsedMilliseconds < testScene.TestTimeout)
                {
                    await Task.Delay(SafeDelayTime);
                }
                else
                {
                    Trace.Output(Trace.TraceLevel.Warning, $"[BVT] Test run timed out after {(timeoutTimer.ElapsedMilliseconds/1000f).ToString(CultureInfo.CurrentCulture)} seconds.");
                    for (int i = 0; i < s_KnownTest.Count; i++)
                    {
                        BuildVerificationReport.Assert(s_KnownTest.Array[i].GetIdentifier(), false, "Test timed out.");
                    }
                    Reset();
                    break;
                }
            }


            Trace.Output(Trace.TraceLevel.Info, $"[BVT] Unload {testScene.ScenePath} ({testScene.BuildIndex.ToString()})");
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(testScene.BuildIndex, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
            timeoutTimer.Restart();
            while (!unloadOperation.isDone)
            {
                if (timeoutTimer.ElapsedMilliseconds < testScene.UnloadTimeout)
                {
                    await Task.Delay(SafeDelayTime);
                }
                else
                {
                    Trace.Output(Trace.TraceLevel.Error, $"[BVT] Failed to unload {testScene.ScenePath} ({testScene.BuildIndex.ToString()}).");
                    Reset();
                    return;
                }
            }

            // Make sure we remove all registered as a safety precaution / will also stop the timer
            Reset();
        }


        public static void RemoveTest(SimpleTestBehaviour simpleTest)
        {
            if (simpleTest == null) return;

            lock (s_lockKnownTests)
            {
                s_KnownTest.RemoveFirstItem(simpleTest);
            }
        }

        static bool HasRemainingTests()
        {
            lock (s_lockKnownTests)
            {
                return s_KnownTest.Count > 0;
            }
        }

        static void Reset()
        {
            lock (s_lockKnownTests)
            {
                s_KnownTest.Clear();
            }
        }
    }
}