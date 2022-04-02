// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GDX
{
    /// <summary>
    ///     A collection of localization related helper utilities.
    /// </summary>
    [VisualScriptingCompatible(8)]
    public static class Localization
    {
        /// <summary>
        ///     A list of supported languages.
        /// </summary>
        /// <remarks>
        ///     This does not differentiate between things like French Canadian and French.
        /// </remarks>
        public enum Language : ushort
        {
            Development = 0,
            Default = 1,
            English = 5,
            German = 10,
            Spanish = 15,
            French = 20,
            Polish = 25,
            Russian = 30
        }

        /// <summary>
        ///     The UTC ISO 8601 compliant <see cref="DateTime.ToString(System.String)" />.
        /// </summary>
        public const string UtcTimestampFormat = "yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz";

        /// <summary>
        ///     The local ISO 8601 compliant <see cref="DateTime.ToString(System.String)" />.
        /// </summary>
        public const string LocalTimestampFormat = "yyyy-MM-dd HH\\:mm\\:ss";

        /// <summary>
        ///     An array of strings representative for file size formats.
        /// </summary>
        static readonly string[] k_ReadableByteSizes = { "B", "KB", "MB", "GB", "TB" };

        /// <summary>
        ///     Creates a more human readable <see cref="string" /> of a byte size.
        /// </summary>
        /// <example>
        ///     A byte size of 1024, will return a string of 1 KB.
        /// </example>
        /// <param name="base2Size">The number of bytes (binary) to measure.</param>
        /// <returns>A human readable version of the provided <paramref name="base2Size" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetHumanReadableFileSize(long base2Size)
        {
            long length = base2Size;
            int order = 0;
            const int k_IncrementLengthAdjusted = 4;
            while (length >= 1024 && order < k_IncrementLengthAdjusted)
            {
                order++;
                length /= 1024;
            }

            return $"{length:0.##} {k_ReadableByteSizes[order]}";
        }

        /// <summary>
        ///     <para>Get the ISO 639-1 language code for <paramref name="targetLanguage" />.</para>
        /// </summary>
        /// <remarks>Two additional non-compliant values may be returned DEV or DEFAULT.</remarks>
        /// <param name="targetLanguage">The target <see cref="Language" />.</param>
        /// <returns>The language code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetISO639_1(this Language targetLanguage)
        {
            switch (targetLanguage)
            {
                case Language.English:
                    return "en";
                case Language.German:
                    return "de";
                case Language.Spanish:
                    return "es";
                case Language.French:
                    return "fr";
                case Language.Polish:
                    return "pl";
                case Language.Russian:
                    return "ru";
                case Language.Development:
                    return "DEV";
                case Language.Default:
                    return "DEFAULT";
                default:
                    return "en";
            }
        }

        /// <summary>
        ///     <para>Get the IETF BCP 47 language code for <paramref name="targetLanguage" />.</para>
        /// </summary>
        /// <remarks>Two additional non-compliant values may be returned DEV or DEFAULT.</remarks>
        /// <param name="targetLanguage">The target <see cref="Language" />.</param>
        /// <returns>The language code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetIETF_BCP47(this Language targetLanguage)
        {
            switch (targetLanguage)
            {
                case Language.English:
                    return "en-US";
                case Language.German:
                    return "de-DE";
                case Language.Spanish:
                    return "es-ES";
                case Language.French:
                    return "fr-FR";
                case Language.Polish:
                    return "pl-PL";
                case Language.Russian:
                    return "ru-RU";
                case Language.Development:
                    return "DEV";
                case Language.Default:
                    return "DEFAULT";
                default:
                    return "en-US";
            }
        }

        /// <summary>
        ///     Get the <see cref="GDX.Localization.Language" /> equivalent of the <see cref="SystemLanguage" />.
        /// </summary>
        /// <returns>The appropriate <see cref="GDX.Localization.Language" />, or default.</returns>
        public static Language GetSystemLanguage()
        {
#if UNITY_DOTSRUNTIME
            string cultureName = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            if (cultureName.StartsWith("en"))
            {
                return Language.English;
            }
            if (cultureName.StartsWith("de"))
            {
                return Language.German;
            }
            if (cultureName.StartsWith("ru"))
            {
                return Language.Russian;
            }
            if (cultureName.StartsWith("pl"))
            {
                return Language.Polish;
            }
            if (cultureName.StartsWith("fr"))
            {
                return Language.French;
            }
            if (cultureName.StartsWith("es"))
            {
                return Language.Spanish;
            }

            return Language.Default;
#else
            SystemLanguage language = Application.systemLanguage;
            switch (language)
            {
                case SystemLanguage.German:
                    return Language.German;
                case SystemLanguage.Russian:
                    return Language.Russian;
                case SystemLanguage.Polish:
                    return Language.Polish;
                case SystemLanguage.French:
                    return Language.French;
                case SystemLanguage.Spanish:
                    return Language.Spanish;
                case SystemLanguage.English:
                    return Language.English;
                default:
                    return Language.Default;
            }
#endif
        }

        /// <summary>
        ///     Get the localized <see cref="System.DateTime" />.<see cref="System.DateTime.ToString(string)" /> for
        ///     <paramref name="targetLanguage" />.
        /// </summary>
        /// <param name="targetLanguage">The target <see cref="Language" />.</param>
        /// <returns>The format <see cref="System.String" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetTimestampFormat(this Language targetLanguage)
        {
            switch (targetLanguage)
            {
                case Language.English:
                    return "M/d/yyyy h:mm a";
                case Language.Russian:
                case Language.Spanish:
                case Language.Polish:
                case Language.German:
                case Language.French:
                    return "d/M/yyyy h:mm a";
                case Language.Development:
                    return UtcTimestampFormat;
                case Language.Default:
                    return LocalTimestampFormat;
                default:
                    return LocalTimestampFormat;
            }
        }

        /// <summary>
        ///     Sets the current threads culture to a defined setting in <see cref="Config" />.
        /// </summary>
        /// <remarks>
        ///     Can be used to avoid issues with culture settings without a Gregorian Calendar. Configurable to automatically
        ///     execute after assemblies are loaded.
        /// </remarks>
        public static void SetDefaultCulture()
        {
            if (Config.LocalizationSetDefaultCulture && GetSystemLanguage() == Language.Default)
            {
                CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(Config.LocalizationDefaultCulture.GetIETF_BCP47());
            }
        }
    }
}