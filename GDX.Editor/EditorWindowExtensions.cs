// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GDX.Editor
{
    /// <summary>
    ///     <see cref="UnityEditor.EditorWindow" /> Based Extension Methods
    /// </summary>
    public static class EditorWindowExtensions
    {
        public static void ForceRepaint(this EditorWindow window)
        {
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
                repaintMethod.Invoke(window, Core.EmptyObjectArray);
            }
        }


        public static void ApplySetup(this EditorWindow window, EditorWindowSetup setup)
        {
            if (window == null)
                return;

            if (setup.HasFixedSize)
            {
                if ( setup.Width < window.minSize.x || setup.Height < window.minSize.y)
                {
                    Debug.LogWarning("Requested window size is too small compared to the windows existing minSize setting.");
                }
                if (setup.Width > window.maxSize.x || setup.Height > window.maxSize.y)
                {
                    Debug.LogWarning("Requested window size is too big compared to the windows existing maxSize setting.");
                }
            }

            Rect newPosition = window.position;
            if (setup.HasFixedSize)
            {
                newPosition.width = setup.Width;
                newPosition.height = setup.Height;
            }
            if (setup.HasPosition)
            {
                newPosition.x = setup.X;
                newPosition.y = setup.Y;
            }
            if (setup.HasFixedSize || setup.HasPosition)
            {
                // This fixes a bug with Unity when you ask for a window that is previously docked,
                // the initial position will be that of before it was docked. So we just set something first.
                window.position = new Rect(0, 0, 0, 0);
                window.position = newPosition;
            }

            window.Show(true);

            // Do we want it to me maximized?
            if (setup.Maximized)
            {
                window.maximized = true;
            }

            if (setup.Focus)
            {
                window.Focus();
            }

            window.ForceRepaint();
        }

        public struct EditorWindowSetup
        {
            /// <summary>
            ///     Should an existing version of the window be used instead of creating a new instance?
            /// </summary>
            public bool UseExisting;

            /// <summary>
            ///     Should we attempt to maximize the window?
            /// </summary>
            public bool Maximized;

            /// <summary>
            ///     Should we apply focus to the window?
            /// </summary>
            /// <remarks>
            ///     Newly created windows automatically get focus.
            /// </remarks>
            public bool Focus;

            /// <summary>
            ///     The size of the window in pixels, this will automatically adjust for OS level scaling
            /// </summary>
            public bool HasFixedSize;

            /// <summary>
            ///     The desired width of the window in pixels.
            /// </summary>
            /// <remarks>
            ///     This value will automatically be scaled based on display scaling.
            /// </remarks>
            public int Width;

            /// <summary>
            ///     The desired height of the window in pixels.
            /// </summary>
            /// <remarks>
            ///     This value will automatically be scaled based on display scaling.
            /// </remarks>
            public int Height;

            public bool HasPosition;// Position forces undocked
            public int X;
            public int Y;

            public EditorWindowSetup(bool useExisting = false, bool setFocus = true, bool maximized = false, bool setFixedSize = false,
                int width = 800, int height = 600, bool setPosition = false, int x = 0, int y = 0)
            {
                UseExisting = useExisting;
                Maximized = maximized;

                Focus = setFocus;

                HasFixedSize = setFixedSize;
                float ppp = EditorGUIUtility.pixelsPerPoint;
                Width = (int)(width / ppp);
                Height = (int)(height / ppp);

                HasPosition = setPosition;
                X = x;
                Y = y;
            }
        }
    }
}