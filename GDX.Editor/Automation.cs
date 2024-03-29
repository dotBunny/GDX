﻿// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.CodeEditor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Profiling;

namespace GDX.Editor
{
    /// <summary>
    ///     A collection of helper methods used for automation processes.
    /// </summary>
    public static class Automation
    {
        static string LayoutStashPath()
        {
            return Path.Combine(Application.dataPath, "..", Config.PlatformAutomationFolder, "GDX.layout");
        }

        /// <summary>
        ///     Capture a <see cref="Texture2D" /> of all open <see cref="EditorWindow" />.
        /// </summary>
        /// <returns>The <see cref="Texture2D" /> captured.</returns>
        public static Texture2D CaptureAllWindows()
        {
            Bounds windows = GetEditorBounds();
            int width = (int)windows.size.x;
            int height = (int)windows.size.y;
            Color[] screenPixels =
                InternalEditorUtility.ReadScreenPixel(windows.min, width, height);

            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.SetPixels(screenPixels);
            return texture;
        }

        /// <summary>
        ///     Captures a PNG image of all open <see cref="EditorWindow" />.
        /// </summary>
        /// <param name="outputPath">The absolute path for the image file.</param>
        /// <returns>true/false if the capture was successful.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable IDE1006
        // ReSharper disable once InconsistentNaming
        public static bool CaptureAllWindowsToPNG(string outputPath)
#pragma warning restore IDE1006
        {
            Texture2D texture = CaptureAllWindows();
            if (texture == null)
            {
                return false;
            }

            File.WriteAllBytes(outputPath, texture.EncodeToPNG());
            return true;
        }

        /// <summary>
        ///     Capture a <see cref="Texture2D" /> of the designated <see cref="EditorWindow" />.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="EditorWindow" /> to be captured.</typeparam>
        /// <returns>The <see cref="Texture2D" /> captured.</returns>
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
        ///     Capture a PNG image of the designated <see cref="EditorWindow" />.
        /// </summary>
        /// <param name="outputPath">The absolute path for the image file.</param>
        /// <param name="shouldCloseWindow">Should the gotten window be closed after the capture?</param>
        /// <typeparam name="T">The type of <see cref="EditorWindow" /> to be captured.</typeparam>
        /// <returns>true/false if the capture was successful.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable IDE1006
        // ReSharper disable once InconsistentNaming
        public static bool CaptureEditorWindowToPNG<T>(string outputPath, bool shouldCloseWindow = true)
            where T : EditorWindow
#pragma warning restore IDE1006
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
        ///     Capture a <see cref="Texture2D" /> of the editor window.
        /// </summary>
        /// <returns>The <see cref="Texture2D" /> captured.</returns>
        public static Texture2D CaptureEditorWindow(EditorWindow window)
        {
            if (window == null)
            {
                Debug.LogWarning("No window was passed to be captured.");
                return null;
            }

            // Bring to front
            window.Show(true);


            Rect windowRect = window.position;
            int width = (int)windowRect.width;
            int height = (int)windowRect.height;

            if (width == 0 || height == 0)
            {
                Debug.LogError($"The acquired window has a size of {width.ToString()}x{height.ToString()}.");
                return null;
            }

            Color[] screenPixels = InternalEditorUtility.ReadScreenPixel(windowRect.min, width, height);
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.SetPixels(screenPixels);
            return texture;
        }

        /// <summary>
        ///     Capture a <see cref="Texture2D" /> of the focused editor window.
        /// </summary>
        /// <returns>The <see cref="Texture2D" /> captured.</returns>
        public static Texture2D CaptureFocusedEditorWindow()
        {
            return CaptureEditorWindow(EditorWindow.focusedWindow);
        }

        /// <summary>
        ///     Capture a PNG image of the provided window.
        /// </summary>
        /// <param name="window">The target window.</param>
        /// <param name="outputPath">The absolute path for the image file.</param>
        /// <returns>true/false if the capture was successful.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable IDE1006
        // ReSharper disable once InconsistentNaming
        public static bool CaptureEditorWindowToPNG(EditorWindow window, string outputPath)
#pragma warning restore IDE1006
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
        ///     Capture a PNG image of the currently focused window.
        /// </summary>
        /// <param name="outputPath">The absolute path for the image file.</param>
        /// <returns>true/false if the capture was successful.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#pragma warning disable IDE1006
        // ReSharper disable once InconsistentNaming
        public static bool CaptureFocusedEditorWindowToPNG(string outputPath)
#pragma warning restore IDE1006
        {
            if (EditorWindow.focusedWindow == null)
            {
                Debug.Log("No editor window is focused.");
                return false;
            }


            Texture2D texture = CaptureFocusedEditorWindow();
            if (texture == null)
            {
                return false;
            }

            File.WriteAllBytes(outputPath, texture.EncodeToPNG());
            return true;
        }

        /// <summary>
        ///     Clear the automation temporary folder.
        /// </summary>
        /// <param name="deleteFolder">Should the automation folder be deleted?</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearTempFolder(bool deleteFolder = false)
        {
            string path = Platform.GetOutputFolder(Config.PlatformAutomationFolder);
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

        // ReSharper disable CommentTypo
        /// <summary>
        ///     Stop an existing profile, this is most often used to end a command line profile of the Editor startup.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The editor needs to be started with profiling arguments; more info can be found on
        ///         https://docs.unity3d.com/Manual/EditorCommandLineArguments.html.
        ///     </para>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Argument</term>
        ///             <description>Description</description>
        ///         </listheader>
        ///         <item>
        ///             <term>-profiler-enable</term>
        ///             <description>Enable profile at start.</description>
        ///         </item>
        ///         <item>
        ///             <term>-profiler-log-file [path/to/file.raw]</term>
        ///             <description>The absolute path to where the profiling data should be stored.</description>
        ///         </item>
        ///         <item>
        ///             <term>-profiler-maxusedmemory [bytesize]</term>
        ///             <description>The maximum amount of memory for the profiler to use.</description>
        ///         </item>
        ///         <item>
        ///             <term>-deepprofiling</term>
        ///             <description>Should a deep profile be done?</description>
        ///         </item>
        ///         <item>
        ///             <term>-executeMethod GDX.Editor.Automation.FinishProfile</term>
        ///             <description>Executes this method to stop the profiling.</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        // ReSharper restore CommentTypo
        public static void FinishProfile()
        {
            // We often use this method to profile different parts of the editor during startup, so we have this little
            // helper at the end to make sure to close of the profiling session.
            if (!Profiler.enabled)
            {
                Debug.LogWarning("Unity was not profiling.");
            }
            else
            {
                Profiler.enabled = false;
                if (Profiler.logFile != null)
                {
                    Debug.Log($"Profile stopped: {Profiler.logFile}");
                }
            }

            if (Application.isBatchMode)
            {
                Application.Quit(0);
            }
        }

        /// <summary>
        ///     Generate the project's solution and associated project files.
        /// </summary>
        public static void GenerateProjectFiles()
        {
            Debug.Log("Syncing Project Files ...");

            AssetDatabase.Refresh();

            // We haven't actually opened up Unity on this machine, so no editor has been set
            if (string.IsNullOrEmpty(CodeEditor.CurrentEditorInstallation))
            {
                Debug.Log("Setting CodeEditor.CurrentEditorInstallation ...");

#if UNITY_EDITOR_WIN
                string[] paths =
                {
                    "C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\Common7\\IDE\\devenv.exe",
                    "C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Community\\Common7\\IDE\\devenv.exe",
                    "C:\\Program Files (x86)\\Microsoft Visual Studio\\2017\\Community\\Common7\\IDE\\devenv.exe",
                    "C:\\Program Files\\Microsoft Visual Studio\\2022\\Professional\\Common7\\IDE\\devenv.exe",
                    "C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Professional\\Common7\\IDE\\devenv.exe",
                    "C:\\Program Files (x86)\\Microsoft Visual Studio\\2017\\Professional\\Common7\\IDE\\devenv.exe",
                    "C:\\Program Files\\Microsoft Visual Studio\\2022\\Enterprise\\Common7\\IDE\\devenv.exe",
                    "C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Enterprise\\Common7\\IDE\\devenv.exe",
                    "C:\\Program Files (x86)\\Microsoft Visual Studio\\2017\\Enterprise\\Common7\\IDE\\devenv.exe",
                    $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Programs\\Microsoft VS Code"
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
#elif UNITY_EDITOR_OSX
                string[] paths = {
                    "/Applications/Visual Studio.app",
                    "/Applications/Visual Studio Code.app",
                    "/Applications/MonoDevelop.app"
                };

                foreach (string path in paths)
                {
                    if (!Directory.Exists(path))
                    {
                        continue;
                    }
                    CodeEditor.SetExternalScriptEditor(path);
                    break;
                }
#else
                // Linux Support?
#endif // UNITY_EDITOR_WIN
            }

            CodeEditor.CurrentEditor.SyncAll();
        }

        /// <summary>
        ///     Accumulate the screen space bounds of all editor windows.
        /// </summary>
        /// <returns>The accumulated bounds.</returns>
        public static Bounds GetEditorBounds()
        {
            Bounds accumulatedBounds = new Bounds();
            EditorWindow[] windows = Resources.FindObjectsOfTypeAll<EditorWindow>();
            int windowCount = windows.Length;
            for (int i = 0; i < windowCount; i++)
            {
                accumulatedBounds.Encapsulate(new Bounds(windows[i].position.center, windows[i].position.size));
            }
            return accumulatedBounds;
        }

        /// <summary>
        ///     Returns a reference to the first available GameView.
        /// </summary>
        /// <returns>An reference to an instance of the GameView</returns>
        public static EditorWindow GetGameView()
        {
            // We will cause an exception if there is no device, so return early.
            if (Platform.IsHeadless())
            {
                Debug.LogWarning("No graphics device, unable to acquire GameView.");
                return null;
            }

            EditorWindow returnWindow = null;

            // Use internal reflection for first try
            Type gameView = Type.GetType("UnityEditor.GameView,UnityEditor");
            if (gameView != null)
            {
                returnWindow = EditorWindow.GetWindow(gameView, false);
            }

            // Second try
            if (returnWindow == null)
            {
                returnWindow = (EditorWindow)Reflection.InvokeStaticMethod("UnityEditor.PlayModeView",
                    "GetRenderingView", null,
                    Reflection.PrivateStaticFlags);
            }

            if (returnWindow == null)
            {
                Debug.LogError("Unable to get editor window for Game View");
            }

            return returnWindow;
        }

        /// <summary>
        ///     Get the existing or open a new window with the indicated size / flags.
        /// </summary>
        /// <remarks>Maintains old behaviour.</remarks>
        /// <param name="shouldMaximize">Should the window be maximized?</param>
        /// <param name="width">The desired window pixel width.</param>
        /// <param name="height">The desired window pixel height.</param>
        /// <typeparam name="T">The type of the window requested.</typeparam>
        /// <returns>The instantiated window, or null.</returns>
        public static T GetWindow<T>(bool shouldMaximize = false, int width = 800, int height = 600)
            where T : EditorWindow
        {
            return GetWindow<T>(
                new EditorWindowExtensions.EditorWindowSetup(false, true, shouldMaximize, true, width, height, true));
        }

        public static T GetWindow<T>(EditorWindowExtensions.EditorWindowSetup setup) where T : EditorWindow
        {
            T window;
            if (setup.UseExisting && EditorWindow.HasOpenInstances<T>())
            {
                window = EditorWindow.GetWindow<T>(typeof(T).ToString());
            }
            else
            {
                window = EditorWindow.CreateWindow<T>(typeof(T).ToString());
            }

            window.ApplySetup(setup);
            return window;
        }

        /// <summary>
        ///     Reset the editor to a default state.
        /// </summary>
        /// <remarks>Using this method inside of a unit test will break the runner/chain.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ResetEditor()
        {
            InternalEditorUtility.LoadDefaultLayout();
        }

        public static void StashWindowLayout()
        {
            Type windowLayout = Type.GetType("UnityEditor.WindowLayout,UnityEditor");
            if (windowLayout != null)
            {
                MethodInfo saveMethod =
                    windowLayout.GetMethod("SaveWindowLayout", BindingFlags.Public | BindingFlags.Static);
                if (saveMethod != null)
                {
                    saveMethod.Invoke(null, new object[] { LayoutStashPath() });
                }
            }
        }

        public static void RestoreWindowLayout()
        {
            string path = LayoutStashPath();
            if (File.Exists(path))
            {
                Type windowLayout = Type.GetType("UnityEditor.WindowLayout,UnityEditor");
                if (windowLayout != null)
                {
                    MethodInfo loadMethod = windowLayout.GetMethod("LoadWindowLayout",
                        new[] { typeof(string), typeof(bool), typeof(bool), typeof(bool) });
                    if (loadMethod != null)
                    {
                        loadMethod.Invoke(null, new object[] { path, false, false, true });
                    }
                }

                Platform.ForceDeleteFile(path);
            }
        }
    }
}