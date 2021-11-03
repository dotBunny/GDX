// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if UNITY_2019_1_OR_NEWER
using Unity.CodeEditor;
#endif

namespace GDX.Editor
{
    public static class Automation
    {
        public static void GenerateProjectFiles()
        {
            Trace.Output(Trace.TraceLevel.Info, "Syncing Project Files ...");

            UnityEditor.AssetDatabase.Refresh();

#if UNITY_2019_1_OR_NEWER
            // We haven't actually opened up Unity on this machine, so no editor has been set
            if (string.IsNullOrEmpty(CodeEditor.CurrentEditorInstallation))
            {
#if UNITY_EDITOR_WIN
                // TODO: Add Rider support?
                string[] paths = {
                    "C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Community\\Common7\\IDE\\devenv.exe",
                    "C:\\Program Files (x86)\\Microsoft Visual Studio\\2017\\Community\\Common7\\IDE\\devenv.exe"
                };

                foreach (string path in paths)
                {
                    if (!System.IO.File.Exists(path))
                    {
                        continue;
                    }

                    CodeEditor.SetExternalScriptEditor(path);
                    break;
                }
#else
                // TODO: Maybe do something for VSCode?
                CodeEditor.SetExternalScriptEditor("/Applications/MonoDevelop.app");
#endif // UNITY_EDITOR_WIN
            }

            CodeEditor.CurrentEditor.SyncAll();
#else
            System.Type T = System.Type.GetType("UnityEditor.SyncVS,UnityEditor");
            System.Reflection.MethodInfo SyncSolution = T.GetMethod("SyncSolution", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            SyncSolution.Invoke(null, null);
#endif // UNITY_2019_1_OR_NEWER


        }
    }
}