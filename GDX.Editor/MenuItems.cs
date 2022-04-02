// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.IO;
using UnityEditor;
using UnityEngine;

namespace GDX.Editor
{
    static class MenuItems
    {
        [MenuItem("Tools/GDX/Developer/Output Resource Audit", false, 100)]
        static void ManagedMemoryReport()
        {
            string outputPath = Platform.GetUniqueOutputFilePath("GDX_ManagedMemory_", ".log", Config.PlatformCacheFolder);
            File.WriteAllLines(outputPath,  Developer.Reports.ResourcesAuditReport.GetAll().Output());
            Trace.Output(Trace.TraceLevel.Info, $"Resource Audit written to {outputPath}.");
            Application.OpenURL(outputPath);
        }
    }
}