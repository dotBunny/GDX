// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using UnityEditor;

namespace GDX.Editor
{
    // ReSharper disable once InconsistentNaming
    public static class GDXSettings
    {
        /// <summary>
        ///     A list of keywords to flag when searching project settings.
        /// </summary>
        // ReSharper disable HeapView.ObjectAllocation.Evident
        private static readonly HashSet<string> s_settingsKeywords = new HashSet<string>(new[] {"gdx"});
        // ReSharper restore HeapView.ObjectAllocation.Evident

        /// <summary>
        ///     Get <see cref="SettingsProvider" /> for <see cref="GDX"/>.
        /// </summary>
        /// <returns>A provider for project settings.</returns>
        [SettingsProvider]
        public static SettingsProvider SettingsProvider()
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            return new SettingsProvider("Project/GDX", SettingsScope.Project)
            {
                label = "GDX",
                guiHandler = searchContext =>
                {
                    GDXStyles.BeginGUILayout();

                    GDXStyles.EndGUILayout();
                },
                keywords = s_settingsKeywords
            };
        }
    }
}