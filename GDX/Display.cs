// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using UnityEngine;

// ReSharper disable UnusedMember.Global

namespace GDX
{
    /// <summary>
    ///     A collection of display related helper utilities.
    /// </summary>
    [VisualScriptingCompatible(8)]
    // ReSharper disable once UnusedType.Global
    public static class Display
    {
        /// <summary>
        ///     <para>Returns the actual screen height being rendered on the current platform.</para>
        /// </summary>
        /// <remarks>This resolves issues with scaled rendering.</remarks>
        /// <returns>The pixel height of the screen resolution.</returns>
        /// <exception cref="UnsupportedRuntimeException">Not supported on DOTS Runtime.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetScreenHeight()
        {
#if DOTS_RUNTIME
            throw new UnsupportedRuntimeException();
#else
            return Screen.currentResolution.height;
#endif
        }

        /// <summary>
        ///     <para>Returns the actual screen width being rendered on the current platform.</para>
        /// </summary>
        /// <remarks>This resolves issues with scaled rendering.</remarks>
        /// <returns>The pixel width of the screen resolution.</returns>
        /// <exception cref="UnsupportedRuntimeException">Not supported on DOTS Runtime.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetScreenWidth()
        {
#if DOTS_RUNTIME
            throw new UnsupportedRuntimeException();
#else
            return Screen.currentResolution.width;
#endif
        }

        /// <summary>
        ///     Does the current display device support HDR output?
        /// </summary>
        /// <returns>true/false</returns>
        /// <exception cref="UnsupportedRuntimeException">Not supported on DOTS Runtime.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // ReSharper disable once InconsistentNaming
        public static bool IsHDRSupported()
        {
#if DOTS_RUNTIME
            throw new UnsupportedRuntimeException();
#elif UNITY_PS4
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
        /// <exception cref="UnsupportedRuntimeException">Not supported on DOTS Runtime.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // ReSharper disable once InconsistentNaming
        public static bool IsHDREnabled()
        {
#if DOTS_RUNTIME
            throw new UnsupportedRuntimeException();
#elif UNITY_PS4
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