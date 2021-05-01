﻿// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Developer
{
    /// <summary>
    ///     A structure that defines the string inputs necessary to query for loaded resources of a specific type.
    /// </summary>
    /// <remarks>
    ///     This forces the path through reflection when querying; there are faster methods available. These queries are
    ///     built ideally to support dynamic developer console calls.
    /// </remarks>
    public readonly struct ResourcesQuery
    {
        /// <summary>
        ///     The fully qualified type that is going to be evaluated.
        /// </summary>
        /// <example>
        ///     UnityEngine.Texture2D,UnityEngine
        /// </example>
        public readonly string TypeDefinition;

        /// <summary>
        ///     The fully qualified type that is going to be used to generate a report on the type.
        /// </summary>
        /// <example>
        ///     GDX.Developer.Reports.ObjectInfo,GDX
        /// </example>
        public readonly string ObjectInfoTypeDefinition;

        /// <summary>
        ///     Create a <see cref="ResourcesQuery" /> for the given <paramref name="typeDefinition" />.
        /// </summary>
        /// <remarks>Uses the default <see cref="ObjectInfo" /> for report generation.</remarks>
        /// <example>
        ///     var queryTexture2D = new ResourcesQuery("UnityEngine.AnimationClip,UnityEngine");
        /// </example>
        /// <param name="typeDefinition">The fully qualified type that is going to be evaluated.</param>
        public ResourcesQuery(string typeDefinition)
        {
            TypeDefinition = typeDefinition;
            ObjectInfoTypeDefinition = ObjectInfo.TypeDefinition;
        }

        /// <summary>
        ///     Create a <see cref="ResourcesQuery" /> for the given <paramref name="typeDefinition" />, while
        ///     attempting to use the provided <paramref name="objectInfoTypeDefinition" /> for report generation.
        /// </summary>
        /// <remarks>
        ///     Uses the default <see cref="ObjectInfo" /> for report generation if
        ///     <paramref name="objectInfoTypeDefinition" /> fails to qualify.
        /// </remarks>
        /// <param name="typeDefinition">The fully qualified type that is going to be evaluated.</param>
        /// <param name="objectInfoTypeDefinition">
        ///     The fully qualified type that is going to be used to generate a report on the
        ///     type.
        /// </param>
        /// <example>
        ///     var queryTexture2D = new ResourcesQuery("UnityEngine.Texture2D,UnityEngine",
        ///     "GDX.Developer.Reports.ObjectInfos.TextureObjectInfo,GDX");
        /// </example>
        public ResourcesQuery(string typeDefinition, string objectInfoTypeDefinition)
        {
            TypeDefinition = typeDefinition;
            ObjectInfoTypeDefinition = objectInfoTypeDefinition;
        }
    }
}