// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if !UNITY_DOTSRUNTIME

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace GDX.Developer.Reports.Resource
{
    /// <exception cref="UnsupportedRuntimeException">Not supported on DOTS Runtime.</exception>
    public abstract class ResourceReport
    {
        /// <summary>
        ///     A <see cref="string" /> array used to represent the end of a line for splitting purposes.
        /// </summary>
        static readonly string[] k_NewLineSplit = {Environment.NewLine};

        /// <summary>
        ///     Output the report format as an array of <see cref="string" />.
        /// </summary>
        /// <remarks>It is usually best to provide a <see cref="StringBuilder" /> or <see cref="StreamWriter" /> instead.</remarks>
        /// <param name="context"></param>
        /// <returns>A generated report as an array of <see cref="string" />.</returns>
        public string[] Output(ResourceReportContext context = null)
        {
            StringBuilder builder = new StringBuilder();
            return Output(builder, context)
                ? builder.ToString().Split(k_NewLineSplit, StringSplitOptions.None)
                : null;
        }

        /// <summary>
        ///     Output the report format utilizing the provided <paramref name="writer" />, optionally limiting the
        ///     write buffers by <paramref name="bufferSize" />.
        /// </summary>
        /// <param name="context">Contextual information regarding the generation of the report.</param>
        /// <param name="writer">A <see cref="StreamWriter"/> instance to use for output.</param>
        /// <param name="bufferSize">The write buffer size.</param>
        /// <returns>true/false if the report was successfully written to the provided <paramref name="writer" />.</returns>
        public bool Output(StreamWriter writer, int bufferSize = 1024, ResourceReportContext context = null)
        {
            StringBuilder builder = new StringBuilder();
            if (!Output(builder, context))
            {
                return false;
            }

            int contentLength = builder.Length;
            char[] content = new char[bufferSize];
            int leftOvers = contentLength % bufferSize;
            int writeCount = (contentLength - leftOvers) / bufferSize;

            // Fixed sized writes
            for (int i = 0; i < writeCount; i++)
            {
                builder.CopyTo(i * bufferSize, content, 0, bufferSize);
                writer.Write(content, 0, bufferSize);
            }

            if (leftOvers > 0)
            {
                builder.CopyTo(writeCount * bufferSize, content, 0, leftOvers);
                writer.Write(content, 0, leftOvers);
            }

            writer.Flush();
            return true;
        }

        /// <summary>
        ///     Output the report format utilizing the provided <paramref name="builder" />.
        /// </summary>
        /// <param name="builder">A <see cref="StringBuilder"/> to use when generating the report.</param>
        /// <param name="context">Contextual information regarding the generation of the report.</param>
        /// <returns>true/false if report was added to <paramref name="builder"/>.</returns>
        public abstract bool Output(StringBuilder builder, ResourceReportContext context = null);


        /// <summary>
        ///     Create a sized divider string for use in generating reports.
        /// </summary>
        /// <param name="context">Contextual information regarding the generation of the report.</param>
        /// <param name="divider">The optional character to use as the divider.</param>
        /// <returns>A sized string to be used as a divider.</returns>
        public static string CreateDivider(ResourceReportContext context, char divider = '-')
        {
            return "".PadRight(context.CharacterWidth, divider);
        }

        /// <summary>
        ///     Create a header with <paramref name="title" /> with repeated <paramref name="decorator" />s on the sides, filling
        ///     out to <see cref="ResourceReportContext.CharacterWidth" />.
        /// </summary>
        /// <param name="context">Contextual information regarding the generation of the report.</param>
        /// <param name="title">The text to be treated as the title for the header.</param>
        /// <param name="decorator">The optional character to be used as the decorator.</param>
        /// <returns>A sized string to be used as a header.</returns>
        public static string CreateHeader(ResourceReportContext context, string title, char decorator = '=')
        {
            string workingTitle = $" {title.Trim()} ";

            int titleWidth = workingTitle.Length;
            int decoratorSideWidth = (context.CharacterWidth - titleWidth) / 2;

            // Pad left side first to ensure it is always the most accurate in length
            workingTitle = workingTitle.PadLeft(titleWidth + decoratorSideWidth, decorator);
            workingTitle = workingTitle.PadRight(context.CharacterWidth, decorator);

            return workingTitle;
        }

        public static string CreateKeyValuePair(ResourceReportContext context, string itemKey, string itemValue)
        {
            string workingLine = $"{itemKey}: ".PadRight(context.KeyValuePairWidth);
            if (workingLine.Length > context.KeyValuePairWidth)
            {
                workingLine = workingLine.Substring(0, context.KeyValuePairWidth);
            }

            workingLine = $"{workingLine}{itemValue}";
            if (workingLine.Length > context.CharacterWidth)
            {
                workingLine = workingLine.Substring(0, context.CharacterWidth);
            }

            return workingLine;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string CreateYesNo(bool flag)
        {
            return flag ? "Y" : "N";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string PositiveSign(long targetValue)
        {
            return targetValue > 0 ? "+" : null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string PositiveSign(int targetValue)
        {
            return targetValue > 0 ? "+" : null;
        }
    }
}
#endif