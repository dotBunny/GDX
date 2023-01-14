// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace GDX.Editor.Build
{
    /// <summary>
    ///     A process step for the legacy build pipeline in Unity which ensures, if enabled that the GDX scripting
    ///     define symbol is present for the target.
    /// </summary>
    public class ScriptingDefinesBuildProcessor  : IPreprocessBuildWithReport
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
            PackageProvider.UpdateScriptingDefineSymbols();
        }
    }
}