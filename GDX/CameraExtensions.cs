// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if !UNITY_DOTSRUNTIME

using UnityEngine;

// ReSharper disable UnusedMember.Global
namespace GDX
{
    /// <summary>
    ///     <see cref="UnityEngine.Camera" /> Based Extension Methods
    /// </summary>
    /// <exception cref="UnsupportedRuntimeException">Not supported on DOTS Runtime.</exception>
    [VisualScriptingCompatible(2)]
    // ReSharper disable once UnusedType.Global
    public static class CameraExtensions
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public static Texture2D Capture(this Camera targetCamera, int width = 1920, int height = 1080)
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

        public static bool CaptureToPNG(this Camera targetCamera, string outputPath, int width = 1920, int height = 1080 )
        {
            Texture2D captureTexture = Capture(targetCamera, width, height);
            if (captureTexture == null)
            {
                return false;
            }
            System.IO.File.WriteAllBytes(outputPath, captureTexture.EncodeToPNG());
            return true;
        }
    }
}
#endif // !UNITY_DOTSRUNTIME