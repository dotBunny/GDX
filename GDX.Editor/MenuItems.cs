﻿// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.IO;
using GDX.Developer.Reports;
using UnityEditor;
using UnityEngine;

namespace GDX.Editor
{
    [ExcludeFromCodeCoverage]
    public static class MenuItems
    {
#if GDX_TOOLS
        [MenuItem("Tools/GDX/Developer/Output Resource Audit", false, 100)]
#endif // GDX_TOOLS
        static void ManagedMemoryReport()
        {
            string outputPath =
                Platform.GetUniqueOutputFilePath("GDX_ManagedMemory_", ".log", Config.PlatformCacheFolder);
            File.WriteAllLines(outputPath, ResourcesAuditReport.GetAll().Output());
            Debug.Log($"Resource Audit written to {outputPath}.");
            Application.OpenURL(outputPath);
        }

#if GDX_TOOLS
        [MenuItem("Tools/GDX/Developer/Force Reserialize Assets", false, 100)]
#endif // GDX_TOOLS
        static void ForceReserializeAssets()
        {
            if (EditorUtility.DisplayDialog("Reserialize All Assets",
                    "Are you sure you want to do this? It will take some time!", "Yes", "No"))
            {
                AssetDatabase.ForceReserializeAssets();
            }
        }

#if GDX_TOOLS
        [MenuItem("Tools/GDX/Developer/Force Domain Reload", false, 100)]
#endif // GDX_TOOLS
        static void ForceDomainReload()
        {
            EditorUtility.RequestScriptReload();
        }

#if UNITY_2022_2_OR_NEWER && !UNITY_2023_2_OR_NEWER
#if GDX_TOOLS
        [MenuItem("Assets/Find References In Project", false, 25)]
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
                    $"ref=\"{AssetDatabase.GetAssetPath(Selection.activeObject)}\"");
            UnityEditor.Search.SearchService.ShowWindow(context);
        }

        [MenuItem("Assets/Find References In Project", true, 25)]
        static bool FindReferencesInProjectValidate()
        {
            Object selectedObject = Selection.activeObject;
            return selectedObject != null && AssetDatabase.Contains(selectedObject);
        }
#endif // UNITY_2022_2_OR_NEWER
    }
}