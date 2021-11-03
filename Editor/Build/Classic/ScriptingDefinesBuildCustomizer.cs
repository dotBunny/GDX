// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

// ReSharper disable MemberCanBePrivate.Global

namespace GDX.Editor.Build.Classic
{
#if GDX_PLATFORMS
    /// <summary>
    ///     A customizer for the <c>ClassicBuildPipeline</c> that ensures the build is built with 'GDX' as a
    ///     scripting define.
    /// </summary>
    public class ScriptingDefinesBuildCustomizer : Unity.Build.Classic.ClassicBuildPipelineCustomizer
    {
        /// <summary>
        /// Provide GDX as a scripting define for the built player.
        /// </summary>
        /// <returns>An array containing GDX.</returns>
        public override string[] ProvidePlayerScriptingDefines()
        {
            return new [] {"GDX"};
        }
    }
#endif
}