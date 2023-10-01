// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if !UNITY_DOTSRUNTIME

using System.IO;
using UnityEngine;

namespace GDX
{
    /// <summary>
    ///     <see cref="UnityEngine.Camera" /> Based Extension Methods
    /// </summary>
    /// <exception cref="UnsupportedRuntimeException">Not supported on DOTS Runtime.</exception>
    [VisualScriptingCompatible(2)]
    public static class CameraExtensions
    {
        /// <summary>
        ///     Forces a <see cref="Camera" /> to render it's view into a texture.
        /// </summary>
        /// <param name="targetCamera">The target <see cref="Camera" /> to use.</param>
        /// <param name="width">The desired width of the rendered texture.</param>
        /// <param name="height">The desired height of the rendered texture.</param>
        /// <param name="depthBuffer">The desired depth of the rendered texture.</param>
        /// <remarks>This behaves differently then using <see cref="ScreenCapture" />.</remarks>
        /// <returns>The rendered view.</returns>
        public static Texture2D RenderToTexture(this Camera targetCamera, int width = 1920, int height = 1080,
            int depthBuffer = 24)
        {
            // Get a temporary render texture from the pool since its gonna be rapid.
            RenderTexture screenshotRenderTexture = RenderTexture.GetTemporary(width, height, depthBuffer);

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

        /// <summary>
        ///     Forces a <see cref="Camera" /> through <see cref="RenderToTexture" /> encoding to PNG.
        /// </summary>
        /// <param name="targetCamera">The target <see cref="Camera" /> to use.</param>
        /// <param name="outputPath">The full path to output the PNG bytes.</param>
        /// <param name="width">The desired width of the rendered texture.</param>
        /// <param name="height">The desired height of the rendered texture.</param>
        /// <param name="depthBuffer">The desired depth of the rendered texture.</param>
        /// <returns>true/false if the capture was successful.</returns>
        /// <remarks>This does not indicate if the writing of the PNG was successful.</remarks>
#pragma warning disable IDE1006
        // ReSharper disable once InconsistentNaming
        public static bool RenderToPNG(this Camera targetCamera, string outputPath, int width = 1920, int height = 1080,
            int depthBuffer = 24)
#pragma warning restore IDE1006
        {
            Texture2D captureTexture = RenderToTexture(targetCamera, width, height, depthBuffer);
            if (captureTexture == null)
            {
                return false;
            }

            File.WriteAllBytes(outputPath, captureTexture.EncodeToPNG());
            return true;
        }
    }
}
#endif // !UNITY_DOTSRUNTIME