// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.Rendering
{
    /// <summary>
    ///     A provider of GDX built-in shader references and properties.
    /// </summary>
    public static class ShaderProvider
    {
        /// <summary>
        ///     The cached reference to the dotted line shader.
        /// </summary>
        static Shader s_DottedLineShader;

        /// <summary>
        ///     The cached reference to the unlit colored shader.
        /// </summary>
        static Shader s_UnlitColorShader;

        /// <summary>
        ///     The cached "_Color" property ID.
        /// </summary>
        public static readonly int ColorPropertyID = Shader.PropertyToID("_Color");

        /// <summary>
        ///     A reference to a clip-space sin wave discard based shader, which cheaply provides a dotted line effect.
        /// </summary>
        public static Shader DottedLine
        {
            get
            {
                if (s_DottedLineShader == null)
                {
                    s_DottedLineShader = Shader.Find("GDX/DottedLine");
                }

                return s_DottedLineShader;
            }
        }

        /// <summary>
        ///     A reference to a unlit single color shader.
        /// </summary>
        public static Shader UnlitColor
        {
            get
            {
                if (s_UnlitColorShader == null)
                {
                    s_UnlitColorShader = Shader.Find("GDX/UnlitColor");
                }

                return s_UnlitColorShader;
            }
        }

        /// <summary>
        ///     Get all shaders which <see cref="ShaderProvider"/> is aware of.
        /// </summary>
        /// <returns>An array of the known shaders to <see cref="ShaderProvider"/>.</returns>
        public static Shader[] GetProvidedShaders()
        {
            return new[] { DottedLine, UnlitColor };
        }
    }
}