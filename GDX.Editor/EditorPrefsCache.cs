// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using GDX.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GDX.Editor
{
    /// <summary>
    ///     A cached version of <see cref="EditorPrefs"/>, allowing for faster access.
    /// </summary>
    /// <remarks>
    ///     It is important to use the equivalent set methods to update the cache.
    /// </remarks>
    public static class EditorPrefsCache
    {
        /// <summary>
        ///     A cache of boolean values backed by <see cref="EditorPrefs" />.
        /// </summary>
        static StringKeyDictionary<bool> s_CachedBooleans = new StringKeyDictionary<bool>(30);

        /// <summary>
        ///     A cache of color values backed by <see cref="EditorPrefs" />.
        /// </summary>
        static StringKeyDictionary<Color> s_CachedColors = new StringKeyDictionary<Color>(15);

        /// <summary>
        ///     A cache of float values backed by <see cref="EditorPrefs" />.
        /// </summary>
        static StringKeyDictionary<float> s_CachedFloats = new StringKeyDictionary<float>(15);

        /// <summary>
        ///     A cache of integer values backed by <see cref="EditorPrefs" />.
        /// </summary>
        static StringKeyDictionary<int> s_CachedIntegers = new StringKeyDictionary<int>(15);

        /// <summary>
        ///     A cache of string values backed by <see cref="EditorPrefs" />.
        /// </summary>
        static StringKeyDictionary<string> s_CachedStrings = new StringKeyDictionary<string>(15);

        /// <summary>
        ///     Get a cached value or fill it from <see cref="EditorPrefs" />.
        /// </summary>
        /// <param name="id">Identifier for the <see cref="bool" /> value.</param>
        /// <param name="defaultValue">If no value is found, what should be used.</param>
        /// <returns>A boolean value.</returns>
        public static bool GetBoolean(string id, bool defaultValue = true)
        {
            if (s_CachedBooleans.IndexOf(id) == -1)
            {
                s_CachedBooleans[id] = EditorPrefs.GetBool(id, defaultValue);
            }

            return s_CachedBooleans[id];
        }

        /// <summary>
        ///     Get a cached value or fill it from <see cref="EditorPrefs" />.
        /// </summary>
        /// <param name="id">Identifier for the <see cref="Color" /> value.</param>
        /// <param name="defaultValue">If no value is found, what should be used.</param>
        /// <returns>A color value.</returns>
        public static Color GetColor(string id, Color defaultValue)
        {
            if (s_CachedColors.IndexOf(id) != -1)
            {
                return s_CachedColors[id];
            }

            //  Parse and save?
            if (ColorUtility.TryParseHtmlString(
                    EditorPrefs.GetString(id, ColorUtility.ToHtmlStringRGBA(defaultValue)),
                    out Color returnValue))
            {
                s_CachedColors[id] = returnValue;
                return returnValue;
            }

            EditorPrefs.SetString(id, ColorUtility.ToHtmlStringRGBA(defaultValue));
            return defaultValue;
        }

        /// <summary>
        ///     Get a cached value or fill it from <see cref="EditorPrefs" />.
        /// </summary>
        /// <param name="id">Identifier for the <see cref="float" /> value.</param>
        /// <param name="defaultValue">If no value is found, what should be used.</param>
        /// <returns>A float value.</returns>
        public static float GetFloat(string id, float defaultValue = 0f)
        {
            if (s_CachedFloats.IndexOf(id) == -1)
            {
                s_CachedFloats[id] = EditorPrefs.GetFloat(id, defaultValue);
            }

            return s_CachedFloats[id];
        }

        /// <summary>
        ///     Get a cached value or fill it from <see cref="EditorPrefs" />.
        /// </summary>
        /// <param name="id">Identifier for the <see cref="int" /> value.</param>
        /// <param name="defaultValue">If no value is found, what should be used.</param>
        /// <returns>An integer value.</returns>
        public static float GetInteger(string id, int defaultValue = -1)
        {
            if (s_CachedIntegers.IndexOf(id) == -1)
            {
                s_CachedIntegers[id] = EditorPrefs.GetInt(id, defaultValue);
            }

            return s_CachedIntegers[id];
        }

        /// <summary>
        ///     Get a cached value or fill it from <see cref="EditorPrefs" />.
        /// </summary>
        /// <param name="id">Identifier for the <see cref="string" /> value.</param>
        /// <param name="defaultValue">If no value is found, what should be used.</param>
        /// <returns>A string, or null.</returns>
        public static string GetString(string id, string defaultValue = null)
        {
            if (s_CachedStrings.IndexOf(id) == -1)
            {
                s_CachedStrings[id] = EditorPrefs.GetString(id, defaultValue);
            }

            return s_CachedStrings[id];
        }

        /// <summary>
        ///     Sets the cached value (and <see cref="EditorPrefs" />) for the <paramref name="id" />.
        /// </summary>
        /// <param name="id">Identifier for the <see cref="bool" /> value.</param>
        /// <param name="setValue">The desired value to set.</param>
        public static void SetBoolean(string id, bool setValue)
        {
            if (!s_CachedBooleans.ContainsKey(id))
            {
                s_CachedBooleans[id] = setValue;
                EditorPrefs.SetBool(id, setValue);
            }
            else
            {
                if (s_CachedBooleans[id] == setValue)
                {
                    return;
                }

                s_CachedBooleans[id] = setValue;
                EditorPrefs.SetBool(id, setValue);
            }
        }

        /// <summary>
        ///     Sets the cached value (and <see cref="EditorPrefs" />) for the <paramref name="id" />.
        /// </summary>
        /// <param name="id">Identifier for the <see cref="Color" /> value.</param>
        /// <param name="setValue">The desired value to set.</param>
        public static void SetColor(string id, Color setValue)
        {
            if (!s_CachedColors.ContainsKey(id))
            {
                s_CachedColors[id] = setValue;
                EditorPrefs.SetString(id, ColorUtility.ToHtmlStringRGBA(setValue));
            }
            else
            {
                if (s_CachedColors[id] == setValue)
                {
                    return;
                }

                s_CachedColors[id] = setValue;
                EditorPrefs.SetString(id, ColorUtility.ToHtmlStringRGBA(setValue));
            }
        }

        /// <summary>
        ///     Sets the cached value (and <see cref="EditorPrefs" />) for the <paramref name="id" />.
        /// </summary>
        /// <param name="id">Identifier for the <see cref="float" /> value.</param>
        /// <param name="setValue">The desired value to set.</param>
        public static void SetFloat(string id, float setValue)
        {
            if (!s_CachedFloats.ContainsKey(id))
            {
                s_CachedFloats[id] = setValue;
                EditorPrefs.SetFloat(id, setValue);
            }
            else
            {
                if (Math.Abs(s_CachedFloats[id] - setValue) < Platform.FloatTolerance)
                {
                    return;
                }

                s_CachedFloats[id] = setValue;
                EditorPrefs.SetFloat(id, setValue);
            }
        }

        /// <summary>
        ///     Sets the cached value (and <see cref="EditorPrefs" />) for the <paramref name="id" />.
        /// </summary>
        /// <param name="id">Identifier for the <see cref="int" /> value.</param>
        /// <param name="setValue">The desired value to set.</param>
        public static void SetInteger(string id, int setValue)
        {
            if (!s_CachedIntegers.ContainsKey(id))
            {
                s_CachedIntegers[id] = setValue;
                EditorPrefs.SetInt(id, setValue);
            }
            else
            {
                if (s_CachedIntegers[id] == setValue)
                {
                    return;
                }

                s_CachedFloats[id] = setValue;
                EditorPrefs.SetInt(id, setValue);
            }
        }

        /// <summary>
        ///     Sets the cached value (and <see cref="EditorPrefs" />) for the <paramref name="id" />.
        /// </summary>
        /// <param name="id">Identifier for the <see cref="string" /> value.</param>
        /// <param name="setValue">The desired value to set.</param>
        public static void SetString(string id, string setValue)
        {
            if (!s_CachedStrings.ContainsKey(id))
            {
                s_CachedStrings[id] = setValue;
                EditorPrefs.SetString(id, setValue);
            }
            else
            {
                if (s_CachedStrings[id] == setValue)
                {
                    return;
                }

                s_CachedStrings[id] = setValue;
                EditorPrefs.SetString(id, setValue);
            }
        }

    }
}