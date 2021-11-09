// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
#if UNITY_2019_1_OR_NEWER
using Unity.CodeEditor;
#endif

namespace GDX.Editor
{
    /// <summary>
    /// A collection of helper methods used for automation processes.
    /// </summary>
    public static class Automation
    {
        /// <summary>
        /// Capture a PNG image of the designated <see cref="EditorWindow"/>.
        /// </summary>
        /// <param name="outputPath">The absolute path for the image file.</param>
        /// <typeparam name="T">The type of <see cref="EditorWindow"/> to be captured.</typeparam>
        public static void CaptureEditorWindow<T>(string outputPath) where T : EditorWindow
        {
            T window = GetWindow<T>();
            if (window != null)
            {
                CaptureFocusedEditorWindow(outputPath);
            }
        }

        /// <summary>
        /// Capture a PNG image of the currently focused window.
        /// </summary>
        /// <param name="outputPath">The absolute path for the image file.</param>
        public static void CaptureFocusedEditorWindow(string outputPath)
        {
            Rect windowRect = EditorWindow.focusedWindow.position;
            int width = (int)windowRect.width;
            int height = (int)windowRect.height;
            Color[] screenPixels = InternalEditorUtility.ReadScreenPixel(windowRect.min, width, height);
            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels(screenPixels);
            System.IO.File.WriteAllBytes(outputPath, texture.EncodeToPNG());
        }

        /// <summary>
        /// Capture a PNG image of the GameView
        /// </summary>
        /// <param name="outputPath">The absolute path for the image file.</param>
        public static void CaptureGameView(string outputPath)
        {
            InternalEditorUtility.OnGameViewFocus(true);
            CaptureFocusedEditorWindow(outputPath);
        }

        /// <summary>
        /// Generate the project's solution and associated project files.
        /// </summary>
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

        /// <summary>
        /// Get the existing or open a new window with the indicated size / flags.
        /// </summary>
        /// <param name="autoFocus">Should the window get focus?</param>
        /// <param name="shouldMaximize">Should the window be maximized?</param>
        /// <param name="width">The desired window pixel width.</param>
        /// <param name="height">The desired window pixel height.</param>
        /// <typeparam name="T">The type of the window requested.</typeparam>
        /// <returns>The instantiated window, or null.</returns>
        public static T GetWindow<T>(bool autoFocus = true, bool shouldMaximize = false, int width = 800, int height = 600) where T : EditorWindow
        {
            Rect windowRect = new Rect(0, 0, width, height);
            T window = EditorWindow.GetWindowWithRect<T>(windowRect, false);
            if (window != null)
            {
                // Do we want it to me maximized?
                if (shouldMaximize)
                {
                    window.maximized = true;
                }

                // Lets make sure that its focused
                if (autoFocus)
                {
                    window.Focus();
                }
            }
            return window;
        }

        /// <summary>
        /// Reset the editor to a default state.
        /// </summary>
        public static void ResetEditor()
        {
            InternalEditorUtility.LoadDefaultLayout();
        }
    }
}