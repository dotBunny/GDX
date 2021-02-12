// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using GDX.Developer;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

#if !GDX_PLATFORMS
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif

// ReSharper disable MemberCanBePrivate.Global

namespace GDX.Editor.Build.Classic
{
    /// <summary>
    ///     A build step for both the legacy and scriptable build pipeline's in Unity. This class will alter itself
    ///     according to available packages and pipelines. This step ensures the build its built with 'GDX' as a
    ///     scripting define.
    /// </summary
#if GDX_PLATFORMS
        public class ScriptingDefinesProvider : Unity.Build.Classic.ClassicBuildPipelineCustomizer
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
#else
        public class ScriptingDefinesProvider : IPreprocessBuildWithReport
        {
            /// <summary>
            ///     The priority for the processor to be executed, before defaults.
            /// </summary>
            /// <value>The numerical value used to sort callbacks, lowest to highest.</value>
            public int callbackOrder { get; }

            /// <summary>
            ///     Ensures the build is made with GDX scripting define
            /// </summary>
            /// <param name="report">Build process reported information.</param>
            public void OnPreprocessBuild(BuildReport report)
            {
                // Make sure that the project has the GDX preprocessor added
                if (ConfigProvider.Get().environmentScriptingDefineSymbol)
                {
                    PackageProvider.EnsureScriptingDefineSymbol();
                }
            }
        }
#endif
}