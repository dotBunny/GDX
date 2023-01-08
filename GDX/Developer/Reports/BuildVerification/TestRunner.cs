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
        public static async Task EvaluateTestScene(int sceneBuildIndex, float loadTimeout = 10000, float testTimeout = 30000, float unloadTimeout = 10000)
        {
            if (sceneBuildIndex < 0)
            {
                Trace.Output(Trace.TraceLevel.Warning, $"[BVT] Invalid scene build index ({sceneBuildIndex.ToString()}.");
                Reset();
                return;
            }

            string scenePath = SceneUtility.GetScenePathByBuildIndex(sceneBuildIndex);

            Trace.Output(Trace.TraceLevel.Info, $"[BVT] Load {scenePath} ({sceneBuildIndex.ToString()})");
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Additive);
            s_Timer.Restart();
            while (!loadOperation.isDone)
            {
                if (s_Timer.ElapsedMilliseconds < loadTimeout)
                {
                    UnityEngine.Debug.Log($"Waiting on load ... {loadOperation.progress.ToString()}");
                   // await Task.Delay(1);
                }
                else
                {
                    Trace.Output(Trace.TraceLevel.Error, $"[BVT] Failed to load {scenePath} ({sceneBuildIndex.ToString()}).");
                    Reset();
                    return;
                }
            }


            // Restart timer for timeout
            s_Timer.Restart();
            while (HasRemainingTests())
            {
                if (s_Timer.ElapsedMilliseconds < testTimeout)
                {
                    UnityEngine.Debug.Log("Waiting on tests ...");
                  //  await Task.Delay(1);
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
            Trace.Output(Trace.TraceLevel.Info, $"[BVT] Unload {scenePath} ({sceneBuildIndex.ToString()})");
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(sceneBuildIndex);
            while (!unloadOperation.isDone)
            {
                if (s_Timer.ElapsedMilliseconds < unloadTimeout)
                {
                    UnityEngine.Debug.Log($"Waiting on unload ... {unloadOperation.progress.ToString()}");
                 //   await Task.Delay(1);
                }
                else
                {
                    Trace.Output(Trace.TraceLevel.Error, $"[BVT] Failed to unload {scenePath} ({sceneBuildIndex.ToString()}).");
                    Reset();
                    return;
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