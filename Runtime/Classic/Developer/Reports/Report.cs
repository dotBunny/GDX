// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;

namespace GDX.Classic.Developer.Reports
{
    public abstract class Report
    {
        /// <summary>
        ///     Output the report format as an array of <see cref="string" />.
        /// </summary>
        /// <remarks>It is usually best to provide a <see cref="StringBuilder" /> or <see cref="StreamWriter" /> instead.</remarks>
        /// <param name="context"></param>
        /// <returns>A generated report as an array of <see cref="string" />.</returns>
        public string[] Output(ReportContext context = null)
        {
            StringBuilder builder = new StringBuilder();
            return Output(builder, context)
                ? builder.ToString().Split(ReportExtensions.NewLineSplit, StringSplitOptions.None)
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
        public bool Output(StreamWriter writer, int bufferSize = 1024, ReportContext context = null)
        {
            StringBuilder builder = new StringBuilder();
            if (!Output(builder, context))
            {
                return false;
            }

            int contentLength = builder.Length;
            char[] content = new char[bufferSize];
            int leftOvers = (contentLength % bufferSize);
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
        public abstract bool Output(StringBuilder builder, ReportContext context = null);
    }
}