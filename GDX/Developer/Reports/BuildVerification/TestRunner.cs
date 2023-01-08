// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections;
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

        static SimpleList<ITestBehaviour> s_KnownTest = new SimpleList<ITestBehaviour>(10);
        static Stopwatch s_Timer = new Stopwatch();

        public static void AddTest(SimpleTestBehaviour simpleTest)
        {
            if (!s_KnownTest.ContainsReference(simpleTest))
            {
                s_KnownTest.AddWithExpandCheck(simpleTest);
            }
        }

        public static async Task AsTask(TestScene[] scenes)
        {
            for (int testSceneIndex = 1; testSceneIndex < scenes.Length; testSceneIndex++)
            {
                await AsTask(scenes[testSceneIndex]);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        ///     Use <see cref="UnityEngine.SceneManagement.SceneUtility.GetBuildIndexByScenePath"/> to get the build
        ///     index.
        /// </remarks>
        /// <param name="sceneBuildIndex"></param>
        /// <param name="loadTimeout"></param>
        /// <param name="testTimeout"></param>
        /// <param name="unloadTimeout"></param>
        public static async Task AsTask(TestScene testScene)
        {
            if (!testScene.IsValid())
            {
                Trace.Output(Trace.TraceLevel.Warning, $"[BVT] Invalid scene {testScene.BuildIndex.ToString()}.");
                Reset();
                return;
            }

            Trace.Output(Trace.TraceLevel.Info, $"[BVT] Load {testScene.ScenePath} ({testScene.BuildIndex.ToString()})");
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(testScene.BuildIndex, LoadSceneMode.Additive);
            s_Timer.Restart();
            while (!loadOperation.isDone)
            {
                if (s_Timer.ElapsedMilliseconds < testScene.LoadTimeout)
                {
                    UnityEngine.Debug.Log($"Waiting on load ... {loadOperation.progress.ToString()}");
                    await Task.Delay(1);
                }
                else
                {
                    Trace.Output(Trace.TraceLevel.Error, $"[BVT] Failed to load {testScene.ScenePath} ({testScene.BuildIndex.ToString()}).");
                    Reset();
                    return;
                }
            }


            // Restart timer for timeout
            s_Timer.Restart();
            while (HasRemainingTests())
            {
                if (s_Timer.ElapsedMilliseconds < testScene.TestTimeout)
                {
                    UnityEngine.Debug.Log("Waiting on tests ...");
                    await Task.Delay(1);
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
                    UnityEngine.Debug.Log($"Waiting on unload ... {unloadOperation.progress.ToString()}");
                    await Task.Delay(1);
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


        public static IEnumerator AsIEnumerator(TestScene[] scenes)
        {
            for (int i = 0; i < scenes.Length; i++)
            {
                yield return AsIEnumerator(scenes[i]);
            }
        }

        public static IEnumerator AsIEnumerator(TestScene testScene)
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