// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;

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
        ///     An array of <see cref="System.Char" /> used to indicate newlines.
        /// </summary>
        public static readonly char[] NewLineCharacters = {'\n', '\r'};

        /// <summary>
        ///     A reusable <see cref="System.Text.StringBuilder" />.
        /// </summary>
        public static readonly StringBuilder Builder = new StringBuilder();
    }
}