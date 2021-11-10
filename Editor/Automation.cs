// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
#if UNITY_2019_1_OR_NEWER
using Unity.CodeEditor;
#endif

// TODO: Add conditional entities world ticking/update duration?
// TODO: Add loading scene helper?

namespace GDX.Editor
{
    /// <summary>
    /// A collection of helper methods used for automation processes.
    /// </summary>
    public static class Automation
    {
        public static Texture2D CaptureCamera(Camera targetCamera, int width = 1920, int height = 1080)
        {
            // Get a temporary render texture from the pool since its gonna be rapid.
            RenderTexture screenshotRenderTexture = RenderTexture.GetTemporary(width, height, 24);

            // Cache a few previous things to restore after we are done
            RenderTexture previousTargetTexture = targetCamera.targetTexture;
            RenderTexture previousActiveTarget = RenderTexture.active;

            // Tell the camera to render to the render texture.
            targetCamera.targetTexture = screenshotRenderTexture;
            targetCamera.Render();

            Texture2D screenshotTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
            RenderTexture.active = screenshotRenderTexture;
            screenshotTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            screenshotTexture.Apply();

            // Release our render texture.
            RenderTexture.active = previousActiveTarget;
            targetCamera.targetTexture = previousTargetTexture;
            screenshotRenderTexture.Release();

            return screenshotTexture;
        }

        public static bool CaptureCameraToPNG(Camera targetCamera, string outputPath, int width = 1920, int height = 1080 )
        {
            Texture2D captureTexture = CaptureCamera(targetCamera, width, height);
            if (captureTexture == null)
            {
                return false;
            }
            System.IO.File.WriteAllBytes(outputPath, captureTexture.EncodeToPNG());
            return true;
        }

        public static Texture2D CaptureCamera(Vector3 position, Quaternion rotation, int width = 1920, int height = 1080)
        {
            GameObject cameraObject = new GameObject {hideFlags = HideFlags.HideAndDontSave};
            Camera captureCamera = cameraObject.AddComponent<Camera>();

            // TODO: Add stack of stuff based on game content?

            // Move and rotate the camera
            Transform cameraTransform = captureCamera.gameObject.transform;
            cameraTransform.position = position;
            cameraTransform.rotation = rotation;

            Texture2D captureTexture = CaptureCamera(captureCamera, width, height);

            Object.DestroyImmediate(cameraObject);

            return captureTexture;
        }

        public static bool CaptureCameraToPNG(Vector3 position, Quaternion rotation, string outputPath, int width = 1920, int height = 1080)
        {
            Texture2D captureTexture = CaptureCamera(position, rotation, width, height);
            if (captureTexture == null)
            {
                return false;
            }
            System.IO.File.WriteAllBytes(outputPath, captureTexture.EncodeToPNG());
            return true;
        }

        /// <summary>
        /// Capture a <see cref="Texture2D"/> of the designated <see cref="EditorWindow"/>.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="EditorWindow"/> to be captured.</typeparam>
        /// <returns>The <see cref="Texture2D"/> captured.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Texture2D CaptureEditorWindow<T>() where T : EditorWindow
        {
            T window = GetWindow<T>();
            return window != null ? CaptureFocusedEditorWindow() : null;
        }

        /// <summary>
        /// Capture a PNG image of the designated <see cref="EditorWindow"/>.
        /// </summary>
        /// <param name="outputPath">The absolute path for the image file.</param>
        /// <typeparam name="T">The type of <see cref="EditorWindow"/> to be captured.</typeparam>
        /// <returns>true/false if the capture was successful.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CaptureEditorWindowToPNG<T>(string outputPath) where T : EditorWindow
        {
            T window = GetWindow<T>();
            if (window != null)
            {
                return CaptureFocusedEditorWindowToPNG(outputPath);
            }

            return false;
        }

        /// <summary>
        /// Capture a <see cref="Texture2D"/> of the focused editor window.
        /// </summary>
        /// <returns>The <see cref="Texture2D"/> captured.</returns>
        public static Texture2D CaptureFocusedEditorWindow()
        {
            Rect windowRect = EditorWindow.focusedWindow.position;
            int width = (int)windowRect.width;
            int height = (int)windowRect.height;
            Color[] screenPixels = InternalEditorUtility.ReadScreenPixel(windowRect.min, width, height);
            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels(screenPixels);
            return texture;
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
            System.IO.File.WriteAllBytes(outputPath, texture.EncodeToPNG());
            return true;
        }

        /// <summary>
        /// Capture a <see cref="Texture2D"/> of the GameView window.
        /// </summary>
        /// <returns>The <see cref="Texture2D"/> captured.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Texture2D CaptureGameViewWindow()
        {
            InternalEditorUtility.OnGameViewFocus(true);
            return CaptureFocusedEditorWindow();
        }

        /// <summary>
        /// Capture a PNG image of the GameView window.
        /// </summary>
        /// <param name="outputPath">The absolute path for the image file.</param>
        /// <returns>true/false if the capture was successful.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CaptureGameViewWindowToPNG(string outputPath)
        {
            Texture2D texture = CaptureGameViewWindow();
            if (texture == null)
            {
                return false;
            }
            System.IO.File.WriteAllBytes(outputPath, texture.EncodeToPNG());
            return true;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ResetEditor()
        {
            InternalEditorUtility.LoadDefaultLayout();
        }
    }
}