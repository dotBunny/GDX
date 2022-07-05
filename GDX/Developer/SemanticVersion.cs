// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace GDX.Developer
{
    /// <summary>
    ///     A Semantic Versioning structure.
    /// </summary>
    /// <remarks>https://semver.org/</remarks>
    public readonly struct SemanticVersion
    {
        /// <summary>
        ///     An array of <see cref="char" /> used to split versions.
        /// </summary>
        static readonly char[] k_VersionIndicators = {'.', ',', '_', 'f'};

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

        /// <summary>
        ///     Create a <see cref="SemanticVersion" /> based on a formatted <see cref="string" />.
        /// </summary>
        /// <param name="version">A formatted version semantic version string (2020.1.0).</param>
        public SemanticVersion(string version)
        {
            string[] split = version.Split(k_VersionIndicators);
            switch (split.Length)
            {
                case 4:
                    int.TryParse(split[0], out Major);
                    int.TryParse(split[1], out Minor);
                    int.TryParse(split[2], out Patch);
                    break;
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

        /// <summary>
        ///     Determine if <see cref="SemanticVersion" /> is greater than another <see cref="SemanticVersion" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="SemanticVersion" />.</param>
        /// <param name="rhs">Right-hand side <see cref="SemanticVersion" />.</param>
        /// <returns>Returns the result of a GREATER THAN operation on two <see cref="SemanticVersion" /> values.</returns>
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

        /// <summary>
        ///     Determine if <see cref="SemanticVersion" /> is greater than or equal to another <see cref="SemanticVersion" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="SemanticVersion" />.</param>
        /// <param name="rhs">Right-hand side <see cref="SemanticVersion" />.</param>
        /// <returns>
        ///     Returns the result of a GREATER THAN OR EQUAL operation on two <see cref="SemanticVersion" />
        ///     values.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(SemanticVersion lhs, SemanticVersion rhs)
        {
            if (lhs.Major > rhs.Major)
            {
                return true;
            }

            if (lhs.Major == rhs.Major && lhs.Minor > rhs.Minor)
            {
                return true;
            }

            if (lhs.Major == rhs.Major && lhs.Minor == rhs.Minor && lhs.Patch > rhs.Patch)
            {
                return true;
            }

            return lhs.Major == rhs.Major && lhs.Minor == rhs.Minor && lhs.Patch == rhs.Patch;
        }

        /// <summary>
        ///     Determine if <see cref="SemanticVersion" /> is less than or equal to another <see cref="SemanticVersion" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="SemanticVersion" />.</param>
        /// <param name="rhs">Right-hand side <see cref="SemanticVersion" />.</param>
        /// <returns>
        ///     Returns the result of a LESS THAN OR EQUAL operation on two <see cref="SemanticVersion" />
        ///     values.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(SemanticVersion lhs, SemanticVersion rhs)
        {
            if (lhs.Major < rhs.Major)
            {
                return true;
            }

            if (lhs.Major == rhs.Major && lhs.Minor < rhs.Minor)
            {
                return true;
            }

            if (lhs.Major == rhs.Major && lhs.Minor == rhs.Minor && lhs.Patch < rhs.Patch)
            {
                return true;
            }

            return lhs.Major == rhs.Major && lhs.Minor == rhs.Minor && lhs.Patch == rhs.Patch;
        }

        /// <summary>
        ///     Determine if <see cref="SemanticVersion" /> is less than another <see cref="SemanticVersion" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="SemanticVersion" />.</param>
        /// <param name="rhs">Right-hand side <see cref="SemanticVersion" />.</param>
        /// <returns>Returns the result of a LESS THAN operation on two <see cref="SemanticVersion" /> values.</returns>
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

        /// <summary>
        ///     Determine if <see cref="SemanticVersion" /> is equal to another <see cref="SemanticVersion" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="SemanticVersion" />.</param>
        /// <param name="rhs">Right-hand side <see cref="SemanticVersion" />.</param>
        /// <returns>Returns the result of a EQUALITY operation on two <see cref="SemanticVersion" /> values.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(SemanticVersion lhs, SemanticVersion rhs)
        {
            return lhs.Major == rhs.Major && lhs.Minor == rhs.Minor && lhs.Patch == rhs.Patch;
        }

        /// <summary>
        ///     Determine if <see cref="SemanticVersion" /> does not equal than another <see cref="SemanticVersion" />.
        /// </summary>
        /// <param name="lhs">Left-hand side <see cref="SemanticVersion" />.</param>
        /// <param name="rhs">Right-hand side <see cref="SemanticVersion" />.</param>
        /// <returns>Returns the result of a NOT EQUAL operation on two <see cref="SemanticVersion" /> values.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(SemanticVersion lhs, SemanticVersion rhs)
        {
            return lhs.Major != rhs.Major || lhs.Minor != rhs.Minor || lhs.Patch != rhs.Patch;
        }

        /// <summary>
        ///     Does the <paramref name="obj" /> equal this <see cref="SemanticVersion" />.
        /// </summary>
        /// <param name="obj">An <see cref="object" /> to compare against.</param>
        /// <returns>Returns the result of an EQUALITY operation.</returns>
        public override bool Equals(object obj)
        {
            return obj is SemanticVersion other && Equals(other);
        }

        /// <summary>
        ///     Get the hash code of the <see cref="SemanticVersion" />.
        /// </summary>
        /// <returns>A <see cref="int" /> value.</returns>
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

        /// <summary>
        ///     Does the <paramref name="otherSemanticVersion" /> equal the <see cref="SemanticVersion" />.
        /// </summary>
        /// <param name="otherSemanticVersion"></param>
        /// <returns>
        ///     The results of checking the <see cref="SemanticVersion.Major" />/<see cref="SemanticVersion.Minor" />/
        ///     <see cref="SemanticVersion.Patch" /> for equality.
        /// </returns>
        bool Equals(SemanticVersion otherSemanticVersion)
        {
            return Major == otherSemanticVersion.Major &&
                   Minor == otherSemanticVersion.Minor &&
                   Patch == otherSemanticVersion.Patch;
        }
    }
}