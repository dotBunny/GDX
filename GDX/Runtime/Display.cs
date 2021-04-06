// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using UnityEngine;

namespace GDX
{
    /// <summary>
    ///     A collection of display related helper utilities.
    /// </summary>
    /// <remarks>Requires UnityEngine.CoreModule.dll to function correctly.</remarks>
    [VisualScriptingUtility]
    public static class Display
    {
        /// <summary>
        ///     <para>Returns the actual screen height being rendered on the current platform.</para>
        /// </summary>
        /// <remarks>This resolves issues with scaled rendering.</remarks>
        /// <returns>The pixel height of the screen resolution.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetScreenHeight()
        {
            return Screen.currentResolution.height;
        }

        /// <summary>
        ///     <para>Returns the actual screen width being rendered on the current platform.</para>
        /// </summary>
        /// <remarks>This resolves issues with scaled rendering.</remarks>
        /// <returns>The pixel width of the screen resolution.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetScreenWidth()
        {
            return Screen.currentResolution.width;
        }

        /// <summary>
        ///     Does the current display device support HDR output?
        /// </summary>
        /// <returns>true/false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // ReSharper disable once InconsistentNaming
        public static bool IsHDRSupported()
        {
#if UNITY_PS4
            return ((UnityEngine.PS4.Utility.GetVideoOutDeviceCapability(UnityEngine.PS4.Utility.videoOutPortHandle) &
                    UnityEngine.PS4.Utility.VideoOutDeviceCapability.BT2020_PQ) != 0);
#else
            return SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.DefaultHDR);
#endif
        }

        /// <summary>
        ///     Is HDR output currently enabled (and actively being used)?
        /// </summary>
        /// <returns>true/false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // ReSharper disable once InconsistentNaming
        public static bool IsHDREnabled()
        {
#if UNITY_PS4
            UnityEngine.PS4.Utility.GetRequestedVideoOutMode(out videoMode)
            return ((videoMode.colorimetry == UnityEngine.PS4.Utility.VideoOutColorimetry.BT2020_PQ)||
                    (videoMode.colorimetry == UnityEngine.PS4.Utility.VideoOutColorimetry.RGB2020_PQ) ||
                    (videoMode.colorimetry == UnityEngine.PS4.Utility.VideoOutColorimetry.YCBCR2020_PQ));
#elif UNITY_XBOXONE
            return UnityEngine.XboxOne.Graphics.displayInHDR;
#else
            return SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.DefaultHDR);
#endif
        }
    }
}