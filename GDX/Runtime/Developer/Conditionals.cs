// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo

namespace GDX.Developer
{
    /// <summary>
    ///     Constant indicators about conditional supports within the GDX package. Helpful indicators of which features are
    ///     available through preprocessor gating.
    /// </summary>
    public static class Conditionals
    {
        /// <summary>
        ///     Is a compatible version of the Addressables package present in the project?
        /// </summary>
#if GDX_ADDRESSABLES
        public const bool HasAddressablesPackage = true;
#else
        public const bool HasAddressablesPackage = false;
#endif

        /// <summary>
        ///     Is a compatible version of the Burst package present in the project?
        /// </summary>
#if GDX_BURST
        public const bool HasBurstPackage = true;
#else
        public const bool HasBurstPackage = false;
#endif

        /// <summary>
        ///     Is a compatible version of the Mathematics package present in the project?
        /// </summary>
#if GDX_MATHEMATICS
        public const bool HasMathematicsPackage = true;
#else
        public const bool HasMathematicsPackage = false;
#endif

#if GDX_PLATFORMS
        public const bool HasPlatformsPackage = true;
#else
        public const bool HasPlatformsPackage = false;
#endif
    }
}