// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Text;
using GDX.Collections.Generic;
using GDX.Developer.ObjectInfos;
using UnityEngine;

namespace GDX.Developer
{
    public static class ResourcesReport
    {
        /// <summary>
        ///     Generate a report for the provided <see cref="ResourcesQuery"/> array.
        /// </summary>
        /// <remarks>
        ///     This method uses reflection to generate the necessary typed parameters, its often more efficient to
        ///     create your own custom reports like in <see cref="GeneralReport"/>.
        /// </remarks>
        /// <param name="queries">A list of <see cref="ResourcesQuery"/> to generate a report from.</param>
        /// <returns></returns>
        public static ResourcesState Generate(ResourcesQuery[] queries)
        {
            // Create our collection object, this is going to effect memory based on its size
            ResourcesState resourcesState = new ResourcesState();

            // Types to actual?
            int count = queries.Length;
            for (int i = 0; i < count; i++)
            {
                resourcesState.Query(queries[i]);
            }

            return resourcesState;
        }

        /// <summary>
        ///     Generate a report of a common subset of data which usually eats a large portion of memory, and often can reveal
        ///     memory leaks.
        /// </summary>
        /// <returns></returns>
        public static ResourcesState GeneralReport()
        {
            // Create our collection object, this is going to effect memory based on its size
            ResourcesState resourcesState = new ResourcesState();

            // Sections
            resourcesState.QueryForType<RenderTexture, TextureObjectInfo>();
            resourcesState.QueryForType<Texture3D, TextureObjectInfo>();
            resourcesState.QueryForType<Texture2D, TextureObjectInfo>();
            resourcesState.QueryForType<Texture2DArray, TextureObjectInfo>();
            resourcesState.QueryForType<Cubemap, TextureObjectInfo>();
            resourcesState.QueryForType<CubemapArray, TextureObjectInfo>();
            resourcesState.QueryForType<Material, ObjectInfo>();
            resourcesState.QueryForType<Shader, ObjectInfo>();
            resourcesState.QueryForType<AnimationClip, ObjectInfo>();

            return resourcesState;
        }


        public static bool Output(this ResourcesState resources, StreamWriter writer)
        {
            return true;
        }

        public static bool Output(this ResourcesState resources, StringBuilder builder)
        {
            return true;
        }

        public static string[] GenerateReport(this ResourcesState resources)
        {
            SimpleList<string> returnLines = new SimpleList<string>(resources.ObjectCount + 10);



            return returnLines.Array;
        }


        private static void CreateHeader(ResourcesState resources, ref StringBuilder builder)
        {


        }




    }
}