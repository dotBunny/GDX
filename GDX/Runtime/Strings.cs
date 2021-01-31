// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// ReSharper disable HeapView.ObjectAllocation.Evident

namespace GDX
{
    /// <summary>
    ///     A collection of string related helper utilities.
    /// </summary>
    public static class Strings
    {
        /// <summary>
        ///      Useful prefix when dealing with asset paths.
        /// </summary>
        public const string AssetPathPrefix = "Assets/";

        /// <summary>
        ///     The public URI of the package's documentation.
        /// </summary>
        public const string DocumentationUri = "https://gdx.dotbunny.com/";

        /// <summary>
        ///     The public URI of the latest changes, as Markdown.
        /// </summary>
        /// <remarks>The main branch is used to contain released versions only, so if it is found there, it is the latest release.</remarks>
        public const string GitHubChangelogUri = "https://github.com/dotBunny/GDX/blob/main/CHANGELOG.md";

        /// <summary>
        ///    The public URI to report issues on GitHub.
        /// </summary>
        public const string GitHubIssuesUri = "https://github.com/dotBunny/GDX/issues";


        /// <summary>
        ///     The public URI of the package repository.
        /// </summary>
        public const string GitHubUri = "https://github.com/dotBunny/GDX/";

        /// <summary>
        ///     A null value string useful for default method parameters.
        /// </summary>
        public const string Null = null;

        /// <summary>
        ///     Reference to the Unity package name for GDX.
        /// </summary>
        public const string PackageName = "com.dotbunny.gdx";

        /// <summary>
        ///     Useful <see cref="string"/> to move up to a parent in pathing.
        /// </summary>
        public const string PreviousFolder = "..";

        /// <summary>
        ///     An array of <see cref="System.Char" /> used to indicate newlines.
        /// </summary>
        public static readonly char[] NewLineIndicators = {'\n', '\r'};

        /// <summary>
        ///     An array of <see cref="System.Char" /> used to split versions.
        /// </summary>
        public static readonly char[] VersionIndicators = {'.', ',', '_', 'f'};
    }
}