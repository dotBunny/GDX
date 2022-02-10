// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using GDX.Mathematics.Random;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Unity.CodeEditor;

namespace GDX.Editor
{
    /// <summary>
    /// A collection of helper methods used for automation processes.
    /// </summary>
    public static class Automation
    {
        public static object[] s_EmptyParametersArray = new object[] { };

        private static string LayoutStashPath()
        {
            return Path.Combine(Application.dataPath, "..", "GDX.layout");
        }
        /// <summary>
        /// Capture a <see cref="Texture2D"/> of the designated <see cref="EditorWindow"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="EditorWindow"/> to be captured.</typeparam>
        /// <returns>The <see cref="Texture2D"/> captured.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Texture2D CaptureEditorWindow<T>(bool shouldCloseWindow = true) where T : EditorWindow
        {
            T window = GetWindow<T>();
            //EditorWindow.FocusWindowIfItsOpen<T>();
            Texture2D returnTexture = null;
            if (window != null)
            {
                returnTexture = CaptureEditorWindow(window);
                if (shouldCloseWindow)
                {
                    window.Close();
                }
            }

            return returnTexture;
        }

        /// <summary>
        /// Capture a PNG image of the designated <see cref="EditorWindow"/>.
        /// </summary>
        /// <param name="outputPath">The absolute path for the image file.</param>
        /// <param name="shouldCloseWindow">Should the gotten window be closed after the capture?</param>
        /// <typeparam name="T">The type of <see cref="EditorWindow"/> to be captured.</typeparam>
        /// <returns>true/false if the capture was successful.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CaptureEditorWindowToPNG<T>(string outputPath, bool shouldCloseWindow = true) where T : EditorWindow
        {
            bool result = false;
            T window = GetWindow<T>();
            //EditorWindow.FocusWindowIfItsOpen<T>();
            if (window != null)
            {
                result = CaptureEditorWindowToPNG(window, outputPath);
                if (shouldCloseWindow)
                {
                    window.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// Capture a <see cref="Texture2D"/> of the editor window.
        /// </summary>
        /// <returns>The <see cref="Texture2D"/> captured.</returns>
        public static Texture2D CaptureEditorWindow(EditorWindow window)
        {
            if (window == null)
            {
                return null;
            }

            // Bring to front
            window.Show();

            Rect windowRect = window.position;
            int width = (int)windowRect.width;
            int height = (int)windowRect.height;
            Color[] screenPixels = InternalEditorUtility.ReadScreenPixel(windowRect.min, width, height);
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.SetPixels(screenPixels);
            return texture;
        }

        /// <summary>
        /// Capture a <see cref="Texture2D"/> of the focused editor window.
        /// </summary>
        /// <returns>The <see cref="Texture2D"/> captured.</returns>
        public static Texture2D CaptureFocusedEditorWindow()
        {
            return CaptureEditorWindow(EditorWindow.focusedWindow);
        }

        /// <summary>
        /// Capture a PNG image of the provided window.
        /// </summary>
        /// <param name="window">The target window.</param>
        /// <param name="outputPath">The absolute path for the image file.</param>
        /// <returns>true/false if the capture was successful.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CaptureEditorWindowToPNG(EditorWindow window, string outputPath)
        {
            Texture2D texture = CaptureEditorWindow(window);
            if (texture == null)
            {
                return false;
            }
            File.WriteAllBytes(outputPath, texture.EncodeToPNG());
            return true;
        }

        /// <summary>
        /// Capture a PNG image of the currently focused window.
        /// </summary>
        /// <param name="outputPath">The absolute path for the image file.</param>
        /// <returns>true/false if the capture was successful.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CaptureFocusedEditorWindowToPNG(string outputPath)
        {
            Texture2D texture = CaptureFocusedEditorWindow();
            if (texture == null)
            {
                return false;
            }
            File.WriteAllBytes(outputPath, texture.EncodeToPNG());
            return true;
        }

        /// <summary>
        /// Clear the automation temporary folder.
        /// </summary>
        /// <param name="deleteFolder">Should the automation folder be deleted?</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearTempFolder(bool deleteFolder = false)
        {
            string path = GetTempFolder(false);
            if (!Directory.Exists(path))
            {
                return;
            }

            DirectoryInfo di = new DirectoryInfo(path);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            if (deleteFolder)
            {
                Directory.Delete(path);
            }
        }

        /// <summary>
        /// Generate the project's solution and associated project files.
        /// </summary>
        public static void GenerateProjectFiles()
        {
            Trace.Output(Trace.TraceLevel.Info, "Syncing Project Files ...");

            AssetDatabase.Refresh();

            // We haven't actually opened up Unity on this machine, so no editor has been set
            if (string.IsNullOrEmpty(CodeEditor.CurrentEditorInstallation))
            {
#if UNITY_EDITOR_WIN
                string[] paths = {
                    "C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\Common7\\IDE\\devenv.exe",
                    "C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Community\\Common7\\IDE\\devenv.exe",
                    "C:\\Program Files (x86)\\Microsoft Visual Studio\\2017\\Community\\Common7\\IDE\\devenv.exe",
                    "C:\\Program Files\\Microsoft Visual Studio\\2022\\Professional\\Common7\\IDE\\devenv.exe",
                    "C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Professional\\Common7\\IDE\\devenv.exe",
                    "C:\\Program Files (x86)\\Microsoft Visual Studio\\2017\\Professional\\Common7\\IDE\\devenv.exe",
                    "C:\\Program Files\\Microsoft Visual Studio\\2022\\Enterprise\\Common7\\IDE\\devenv.exe",
                    "C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Enterprise\\Common7\\IDE\\devenv.exe",
                    "C:\\Program Files (x86)\\Microsoft Visual Studio\\2017\\Enterprise\\Common7\\IDE\\devenv.exe"
                };

                foreach (string path in paths)
                {
                    if (!File.Exists(path))
                    {
                        continue;
                    }

                    CodeEditor.SetExternalScriptEditor(path);
                    break;
                }
#else
                CodeEditor.SetExternalScriptEditor("/Applications/MonoDevelop.app");
#endif // UNITY_EDITOR_WIN
            }

            CodeEditor.CurrentEditor.SyncAll();
        }

        /// <summary>
        /// Returns a reference to the first available GameView.
        /// </summary>
        /// <returns>An reference to an instance of the GameView</returns>
        public static EditorWindow GetGameView()
        {
            System.Type gameView = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            return gameView != null ? EditorWindow.GetWindow(gameView, false) : null;
        }

        /// <summary>
        /// Gets and ensures the temporary GDX automation folder exists.
        /// </summary>
        /// <param name="ensureExists"></param>
        /// <returns>The fully evaluated path to a temporary GDX folder</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetTempFolder(bool ensureExists = true)
        {
            string tempPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "GDX_Automation"));
            if (ensureExists)
            {
                Platform.EnsureFolderHierarchyExists(tempPath);
            }
            return tempPath;
        }

        public static string GetTempFilePath(string prefix = "tmp", string extension = ".tmp", bool ensureFolderExists = true)
        {
            string tempFolder = GetTempFolder(ensureFolderExists);
            StringBuilder tmpFileName = new StringBuilder(260);
            tmpFileName.Append(prefix);
            RandomWrapper random = new RandomWrapper(
                System.DateTime.Now.Ticks.ToString().GetStableHashCode());

            tmpFileName.Append(Platform.GetRandomSafeCharacter(random));
            tmpFileName.Append(Platform.GetRandomSafeCharacter(random));
            tmpFileName.Append(Platform.GetRandomSafeCharacter(random));
            tmpFileName.Append(Platform.GetRandomSafeCharacter(random));
            tmpFileName.Append(Platform.GetRandomSafeCharacter(random));

            while (true)
            {
                tmpFileName.Append(Platform.GetRandomSafeCharacter(random));
                string filePath = Path.Combine(tempFolder, $"{tmpFileName}{extension}");
                if (!File.Exists(filePath))
                {
                    return filePath;
                }

                if (tmpFileName.Length > 260)
                {
                    tmpFileName.Clear();
                    tmpFileName.Append(prefix);
                    tmpFileName.Append(Platform.GetRandomSafeCharacter(random));
                    tmpFileName.Append(Platform.GetRandomSafeCharacter(random));
                    tmpFileName.Append(Platform.GetRandomSafeCharacter(random));
                    tmpFileName.Append(Platform.GetRandomSafeCharacter(random));
                    tmpFileName.Append(Platform.GetRandomSafeCharacter(random));
                }
            }
        }

        /// <summary>
        /// Get the existing or open a new window with the indicated size / flags.
        /// </summary>
        /// <remarks>This will undock a window.  It is important to wait for the window to paint itself.</remarks>
        /// <param name="shouldMaximize">Should the window be maximized?</param>
        /// <param name="width">The desired window pixel width.</param>
        /// <param name="height">The desired window pixel height.</param>
        /// <typeparam name="T">The type of the window requested.</typeparam>
        /// <returns>The instantiated window, or null.</returns>
        public static T GetWindow<T>(bool shouldMaximize = false, int width = 800, int height = 600) where T : EditorWindow
        {
            T window = EditorWindow.GetWindowWithRect<T>(new Rect(0, 0, width, height), false, typeof(T).ToString(), true);

            if (window != null)
            {
                // Enforce the size of the window through setting its position. It's not great but works.
                window.position = new Rect(0, 0, width, height);
                window.Show(true);

                // Do we want it to me maximized?
                if (shouldMaximize)
                {
                    window.maximized = true;
                }

                // Lets do it proper
                window.Repaint();

                // We need to force some internal repainting without having the API surface area to do so.
                // We'll exploit reflection a bit to get around this for now.
                MethodInfo repaintMethod = window.GetType().GetMethod("RepaintImmediately",
                    BindingFlags.NonPublic | BindingFlags.Instance);

                // The exact sequence is frustrating and I'm not entirely sure that it will cover the dreaded white
                // screen that happens from time to time, but as we expanded out the number of steps we haven't seen
                // a fail yet from testing.
                if (repaintMethod != null)
                {
                    repaintMethod.Invoke(window, s_EmptyParametersArray);
                }
            }
            return window;
        }

        /// <summary>
        /// Reset the editor to a default state.
        /// </summary>
        /// <remarks>Using this method inside of a unit test will break the runner/chain.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ResetEditor()
        {
            InternalEditorUtility.LoadDefaultLayout();
        }

        public static void StashWindowLayout()
        {
            System.Type windowLayout = System.Type.GetType("UnityEditor.WindowLayout,UnityEditor");
            if (windowLayout != null)
            {
                MethodInfo saveMethod = windowLayout.GetMethod("SaveWindowLayout",BindingFlags.Public | BindingFlags.Static);
                if (saveMethod != null)
                {
                    saveMethod.Invoke(null,new object[]{LayoutStashPath()});
                }
            }
        }

        public static void RestoreWindowLayout()
        {
            string path = LayoutStashPath();
            if (File.Exists(path))
            {
                System.Type windowLayout = System.Type.GetType("UnityEditor.WindowLayout,UnityEditor");
                if (windowLayout != null)
                {
                    MethodInfo loadMethod = windowLayout.GetMethod("LoadWindowLayout", new System.Type[] {typeof(string), typeof(bool), typeof(bool), typeof(bool)});
                    if (loadMethod != null)
                    {
                        loadMethod.Invoke(null,new object[]{LayoutStashPath(), false, false, true});
                    }
                }
                GDX.Platform.ForceDeleteFile(path);
            }
        }
    }
}
