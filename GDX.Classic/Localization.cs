// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace GDX.Classic
{
    /// <summary>
    ///     A collection of localization related helper utilities.
    /// </summary>
    [VisualScriptingCompatible(8)]
    // ReSharper disable once UnusedType.Global
    public static class Localization
    {

        /// <summary>
        ///     Get the <see cref="GDX.Localization.Language" /> equivalent of the <see cref="SystemLanguage" />.
        /// </summary>
        /// <returns>The appropriate <see cref="GDX.Localization.Language" />, or default.</returns>
        public static GDX.Localization.Language GetSystemLanguage()
        {
            SystemLanguage language = Application.systemLanguage;
            switch (language)
            {
                case SystemLanguage.German:
                    return GDX.Localization.Language.German;
                case SystemLanguage.Russian:
                    return GDX.Localization.Language.Russian;
                case SystemLanguage.Polish:
                    return GDX.Localization.Language.Polish;
                case SystemLanguage.French:
                    return GDX.Localization.Language.French;
                case SystemLanguage.Spanish:
                    return GDX.Localization.Language.Spanish;
                case SystemLanguage.English:
                    return GDX.Localization.Language.English;
                default:
                    return GDX.Localization.Language.Default;
            }
        }

        /// <summary>
        ///     Get the localized <see cref="System.DateTime" />.<see cref="System.DateTime.ToString(string)" /> for
        ///     <paramref name="targetLanguage" />.
        /// </summary>
        /// <param name="targetLanguage">The target <see cref="GDX.Localization.Language" />.</param>
        /// <returns>The format <see cref="System.String" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetTimestampFormat(this GDX.Localization.Language targetLanguage)
        {
            switch (targetLanguage)
            {
                case GDX.Localization.Language.English:
                    return "M/d/yyyy h:mm a";
                case GDX.Localization.Language.Russian:
                case GDX.Localization.Language.Spanish:
                case GDX.Localization.Language.Polish:
                case GDX.Localization.Language.German:
                case GDX.Localization.Language.French:
                    return "d/M/yyyy h:mm a";
                case GDX.Localization.Language.Development:
                    return GDX.Localization.UtcTimestampFormat;
                case GDX.Localization.Language.Default:
                    return GDX.Localization.LocalTimestampFormat;
                default:
                    return GDX.Localization.LocalTimestampFormat;
            }
        }

        /// <summary>
        ///     Sets the current threads culture to a defined setting in <see cref="GDXConfig" />.
        /// </summary>
        /// <remarks>
        ///     Can be used to avoid issues with culture settings without a Gregorian Calendar. Configurable to automatically
        ///     execute after assemblies are loaded.
        /// </remarks>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void SetDefaultCulture()
        {
            if (Core.Config.localizationSetDefaultCulture && GetSystemLanguage() == GDX.Localization.Language.Default)
            {
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(Core.Config.localizationDefaultCulture.GetIETF());
            }
        }
    }
}