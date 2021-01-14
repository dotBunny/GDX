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
        ///     Reference to the Unity package name for GDX.
        /// </summary>
        public const string PackageName = "com.dotbunny.gdx";

        /// <summary>
        ///     The GDX test suite preprocessor define.
        /// </summary>
        public const string TestDefine = "GDX_TESTS";

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