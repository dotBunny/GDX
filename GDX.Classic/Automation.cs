// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

// ReSharper disable UnusedMember.Global

namespace GDX.Classic
{
    // ReSharper disable once UnusedType.Global
    public class Automation
    {
        // ReSharper disable once MemberCanBePrivate.Global
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

        // ReSharper disable once MemberCanBePrivate.Global
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

    }
}