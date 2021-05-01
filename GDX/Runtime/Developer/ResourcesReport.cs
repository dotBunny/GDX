// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;
using GDX.Developer.ObjectInfos;
using UnityEngine;

namespace GDX.Developer
{
    public static class ResourcesReport
    {
        /// <summary>
        ///     Output the provided <paramref name="resources" /> int report format as an array of <see cref="string" />.
        /// </summary>
        /// <remarks>It is usually best to provide a <see cref="StringBuilder" /> or <see cref="StreamWriter" /> instead.</remarks>
        /// <param name="resources"></param>
        /// <returns>A generated report as an array of <see cref="string" />.</returns>
        public static string[] Output(this ResourcesState resources)
        {
            StringBuilder builder = new StringBuilder();
            return Output(resources, ref builder)
                ? builder.ToString().Split(Report.NewLineSplit, StringSplitOptions.None)
                : null;
        }

        /// <summary>
        ///     Output the provided <paramref name="resources" /> in report format utilizing the provided
        ///     <paramref name="writer" />, optionally limiting the write buffers by <paramref name="bufferSize" />.
        /// </summary>
        /// <param name="resources"></param>
        /// <param name="writer"></param>
        /// <param name="bufferSize"></param>
        /// <returns>true/false if the report was successfully written to the provided <paramref name="writer" />.</returns>
        public static bool Output(this ResourcesState resources, ref StreamWriter writer, int bufferSize = -1)
        {
            StringBuilder builder = new StringBuilder();
            if (Output(resources, ref builder))
            {
                //writer.Write();
            }

            return false;
        }

        public static bool Output(this ResourcesState resources, ref StringBuilder builder)
        {
            return true;
        }


        private static void CreateHeader(ResourcesState resources, ref StringBuilder builder)
        {
        }
    }
}