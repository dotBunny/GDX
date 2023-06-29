// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.IO;
using GDX.Experimental;
using UnityEngine;

namespace GDX.Editor
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public static class MenuItems
    {
#if GDX_TOOLS
        [UnityEditor.MenuItem("Tools/GDX/Developer/Output Resource Audit", false, 100)]
#endif // GDX_TOOLS
        static void ManagedMemoryReport()
        {
            string outputPath =
 Platform.GetUniqueOutputFilePath("GDX_ManagedMemory_", ".log", Config.PlatformCacheFolder);
            File.WriteAllLines(outputPath,  Developer.Reports.ResourcesAuditReport.GetAll().Output());
            ManagedLog.Info($"Resource Audit written to {outputPath}.");
            Application.OpenURL(outputPath);
        }

#if GDX_TOOLS
        [UnityEditor.MenuItem("Tools/GDX/Developer/Force Reserialize Assets", false, 100)]
#endif // GDX_TOOLS
        static void ForceReserializeAssets()
        {
            UnityEditor.AssetDatabase.ForceReserializeAssets();
        }

#if UNITY_2022_2_OR_NEWER
#if GDX_TOOLS
        [UnityEditor.MenuItem("Assets/Find References In Project", false, 25)]
#endif // GDX_TOOLS
        static void FindReferencesInProject()
        {
            // Check providers
            bool hasDependencies = false;
            foreach (UnityEditor.Search.ISearchDatabase database in UnityEditor.Search.SearchService.EnumerateDatabases())
            {
                if (database.indexingOptions.HasFlags(UnityEditor.Search.IndexingOptions.Dependencies))
                {
                    hasDependencies = true;
                    break;
                }
            }
            if (!hasDependencies)
            {
                Debug.LogWarning(
                    "You are searching for dependencies, but currently none of your Search Databases are configured with dependency indexing.\n" +
                    "You need to configure this option in the Search Index Manager.");
                return;
            }

            UnityEditor.Search.SearchContext context =
                UnityEditor.Search.SearchService.CreateContext(
                    $"ref=\"{UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject)}\"");
            UnityEditor.Search.SearchService.ShowWindow(context);
        }

        [UnityEditor.MenuItem("Assets/Find References In Project", true, 25)]
        static bool FindReferencesInProjectValidate()
        {
            Object selectedObject = UnityEditor.Selection.activeObject;
            return selectedObject != null && UnityEditor.AssetDatabase.Contains(selectedObject);
        }
#endif // UNITY_2022_2_OR_NEWER
    }
}