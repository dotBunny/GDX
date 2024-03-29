// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

// ReSharper disable CommentTypo, IdentifierTypo

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
#endif // GDX_ADDRESSABLES

        /// <summary>
        ///     Is a compatible version of the Platforms package present in the project?
        /// </summary>
#if GDX_PLATFORMS
        public const bool HasPlatformsPackage = true;
#else
        public const bool HasPlatformsPackage = false;
#endif // GDX_PLATFORMS


        /// <summary>
        ///     Is a compatible version of the Visual Scripting package present in the project?
        /// </summary>
#if GDX_VISUALSCRIPTING
        public const bool HasVisualScriptingPackage = true;
#else
        public const bool HasVisualScriptingPackage = false;
#endif // GDX_VISUALSCRIPTING
    }
}