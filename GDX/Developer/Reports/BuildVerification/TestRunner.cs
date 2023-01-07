﻿// Copyright (c) 2020-2022 dotBunny Inc.
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
        static SimpleList<ITestBehaviour> s_KnownTest = new SimpleList<ITestBehaviour>(10);
        static Stopwatch s_Timer = new Stopwatch();

        public static void AddTest(SimpleTestBehaviour simpleTest)
        {
            if (!s_KnownTest.ContainsReference(simpleTest))
            {
                s_KnownTest.AddWithExpandCheck(simpleTest);
            }
        }


        public static async Task EvaluateTestScene(string scenePath, float millisecondTimeout = 30000)
        {
            int sceneBuildIndex = SceneUtility.GetBuildIndexByScenePath(scenePath);
            if (sceneBuildIndex < 0)
            {
                Trace.Output(Trace.TraceLevel.Error, $"[BVT] Unable to find scene: {scenePath}");
                return;
            }
            await EvaluateTestScene(sceneBuildIndex, millisecondTimeout);
        }

        public static async Task EvaluateTestScene(int sceneBuildIndex, float millisecondTimeout = 30000)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(sceneBuildIndex);

            Trace.Output(Trace.TraceLevel.Info, $"[BVT] Load {scenePath}");
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Additive);
            while (!loadOperation.isDone)
            {
                await Task.Delay(1);
            }

            // Restart timer for timeout
            s_Timer.Restart();
            while (HasRemainingTests())
            {
                if (s_Timer.ElapsedMilliseconds < millisecondTimeout)
                {
                    await Task.Delay(100);
                }
                else
                {
                    Trace.Output(Trace.TraceLevel.Warning, $"[BVT] Test run timed out after {(s_Timer.ElapsedMilliseconds/1000f).ToString(CultureInfo.CurrentCulture)} seconds.");
                    for (int i = 0; i < s_KnownTest.Count; i++)
                    {
                        BuildVerificationReport.Assert(s_KnownTest.Array[i].GetIdentifier(), false, "Test timed out.");
                    }
                    s_KnownTest.Clear();
                }
            }

            // Make sure we remove all registered as a safety precaution / will also stop the timer
            Reset();

            Trace.Output(Trace.TraceLevel.Info, $"[BVT] Unload {scenePath}");
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(sceneBuildIndex);
            while (!unloadOperation.isDone)
            {
                await Task.Delay(1);
            }
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