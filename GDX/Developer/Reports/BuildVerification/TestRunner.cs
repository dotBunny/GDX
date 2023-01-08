// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;
using System.Globalization;
using GDX.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GDX.Developer.Reports.BuildVerification
{
    public static class TestRunner
    {
        public readonly struct TestScene
        {
            public readonly int BuildIndex;
            public readonly string ScenePath;
            public readonly int LoadTimeout;
            public readonly int TestTimeout;
            public readonly int UnloadTimeout;

            public TestScene(int buildIndex, int loadTimeout = 10000, int testTimeout = 30000, int unloadTimeout = 10000)
            {
                BuildIndex = buildIndex;
                ScenePath = SceneUtility.GetScenePathByBuildIndex(buildIndex);

                LoadTimeout = loadTimeout;
                TestTimeout = testTimeout;
                UnloadTimeout = unloadTimeout;
            }

            public bool IsValid()
            {
                return !string.IsNullOrEmpty(ScenePath);
            }
        }

        static SimpleList<ITestBehaviour> s_KnownTest = new SimpleList<ITestBehaviour>(10);
        static readonly Stopwatch s_Timer = new Stopwatch();

        public static void AddTest(SimpleTestBehaviour simpleTest)
        {
            if (!s_KnownTest.ContainsReference(simpleTest))
            {
                s_KnownTest.AddWithExpandCheck(simpleTest);
            }
        }

        public static IEnumerator Process(TestScene[] scenes)
        {
            for (int i = 0; i < scenes.Length; i++)
            {
                yield return Process(scenes[i]);
            }
        }

        public static IEnumerator Process(TestScene testScene)
        {
            WaitForEndOfFrame cachedEndOfFrameWait = new WaitForEndOfFrame();

            if (!testScene.IsValid())
            {
                Trace.Output(Trace.TraceLevel.Warning, $"[BVT] Invalid scene {testScene.BuildIndex.ToString()}.");
                Reset();
                yield break;
            }

            Trace.Output(Trace.TraceLevel.Info, $"[BVT] Load {testScene.ScenePath} ({testScene.BuildIndex.ToString()})");
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(testScene.BuildIndex, LoadSceneMode.Additive);
            s_Timer.Restart();
            while (!loadOperation.isDone)
            {
                if (s_Timer.ElapsedMilliseconds < testScene.LoadTimeout)
                {
                    yield return cachedEndOfFrameWait;
                }
                else
                {
                    Trace.Output(Trace.TraceLevel.Error, $"[BVT] Failed to load {testScene.ScenePath} ({testScene.BuildIndex.ToString()}).");
                    Reset();
                    yield break;
                }
            }


            // Restart timer for timeout
            s_Timer.Restart();
            while (HasRemainingTests())
            {
                if (s_Timer.ElapsedMilliseconds < testScene.TestTimeout)
                {
                    yield return cachedEndOfFrameWait;
                }
                else
                {
                    Trace.Output(Trace.TraceLevel.Warning, $"[BVT] Test run timed out after {(s_Timer.ElapsedMilliseconds/1000f).ToString(CultureInfo.CurrentCulture)} seconds.");
                    for (int i = 0; i < s_KnownTest.Count; i++)
                    {
                        BuildVerificationReport.Assert(s_KnownTest.Array[i].GetIdentifier(), false, "Test timed out.");
                    }
                    s_KnownTest.Clear();
                    break;
                }
            }

            s_Timer.Restart();
            Trace.Output(Trace.TraceLevel.Info, $"[BVT] Unload {testScene.ScenePath} ({testScene.BuildIndex.ToString()})");
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(testScene.BuildIndex);
            while (!unloadOperation.isDone)
            {
                if (s_Timer.ElapsedMilliseconds < testScene.UnloadTimeout)
                {
                    yield return cachedEndOfFrameWait;
                }
                else
                {
                    Trace.Output(Trace.TraceLevel.Error, $"[BVT] Failed to unload {testScene.ScenePath} ({testScene.BuildIndex.ToString()}).");
                    Reset();
                    yield break;
                }
            }

            // Make sure we remove all registered as a safety precaution / will also stop the timer
            Reset();
        }

        public static void RemoveTest(SimpleTestBehaviour simpleTest)
        {
            s_KnownTest.RemoveFirstItem(simpleTest);
        }

        public static void Reset()
        {
            s_KnownTest.Clear();
            s_Timer.Stop();
        }

        static bool HasRemainingTests()
        {
            return s_KnownTest.Count > 0;
        }


    }
}