// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace GDX
{
    /// <summary>
    ///     A Semantic Versioning structure.
    /// </summary>
    /// <remarks>https://semver.org/</remarks>
    public readonly struct SemanticVersion
    {
        // ReSharper disable MemberCanBePrivate.Global

        /// <summary>
        ///     Major Version.
        /// </summary>
        /// <remarks>Is incremented when you make incompatible API changes.</remarks>
        public readonly int Major;

        /// <summary>
        ///     Minor Version.
        /// </summary>
        /// <remarks>Is incremented when you add functionality in a backwards-compatible manner.</remarks>
        public readonly int Minor;

        /// <summary>
        ///     Patch Version
        /// </summary>
        /// <remarks>Is incremented when you make backwards-compatible fixes.</remarks>
        public readonly int Patch;

        // ReSharper restore MemberCanBePrivate.Global

        public SemanticVersion(string version)
        {
            string[] split = version.Split(Strings.VersionIndicators);
            switch (split.Length)
            {
                case 3:
                    int.TryParse(split[0], out Major);
                    int.TryParse(split[1], out Minor);
                    int.TryParse(split[2], out Patch);
                    break;
                case 2:
                    int.TryParse(split[0], out Major);
                    int.TryParse(split[1], out Minor);
                    Patch = 0;
                    break;
                case 1:
                    int.TryParse(split[0], out Major);
                    Minor = 0;
                    Patch = 0;
                    break;
                default:
                    Major = 0;
                    Minor = 0;
                    Patch = 0;
                    break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(SemanticVersion lhs, SemanticVersion rhs)
        {
            if (lhs.Major > rhs.Major)
            {
                return true;
            }

            if (lhs.Major == rhs.Major && lhs.Minor > rhs.Minor)
            {
                return true;
            }

            return lhs.Major == rhs.Major && lhs.Minor == rhs.Minor && lhs.Patch > rhs.Patch;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(SemanticVersion lhs, SemanticVersion rhs)
        {
            if (lhs.Major < rhs.Major)
            {
                return true;
            }

            if (lhs.Major == rhs.Major && lhs.Minor < rhs.Minor)
            {
                return true;
            }

            return lhs.Major == rhs.Major && lhs.Minor == rhs.Minor && lhs.Patch < rhs.Patch;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(SemanticVersion lhs, SemanticVersion rhs)
        {
            return lhs.Major == rhs.Major && lhs.Minor == rhs.Minor && lhs.Patch == rhs.Patch;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(SemanticVersion lhs, SemanticVersion rhs)
        {
            return lhs.Major != rhs.Major || lhs.Minor != rhs.Minor || lhs.Patch != rhs.Patch;
        }

        public override bool Equals(object obj)
        {
            return obj is SemanticVersion other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Major;
                hashCode = (hashCode * 397) ^ Minor;
                hashCode = (hashCode * 397) ^ Patch;
                return hashCode;
            }
        }

        private bool Equals(SemanticVersion other)
        {
            return Major == other.Major && Minor == other.Minor && Patch == other.Patch;
        }
    }
}