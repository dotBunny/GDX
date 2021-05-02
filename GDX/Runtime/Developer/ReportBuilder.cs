// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;

namespace GDX.Developer
{
    /// <summary>
    ///     A set of functionality used to generate reports.
    /// </summary>
    public static class ReportBuilder
    {
        /// <summary>
        ///     A <see cref="string" /> array used to represent the end of a line for splitting purposes.
        /// </summary>
        public static readonly string[] NewLineSplit = {Environment.NewLine};

        public static void AddInstanceInformation(Report report, ReportContext context, StringBuilder builder)
        {
            builder.AppendLine(context.CreateKVP("Active Scene", report.ActiveScene));
            builder.AppendLine(context.CreateKVP("Platform", report.Platform.ToString()));
            builder.AppendLine(context.CreateKVP("Created",
                report.Created.ToString(Localization.LocalTimestampFormat)));
        }

        public static void AddMemoryInformation(ResourcesReport resources, ReportContext context, StringBuilder builder,
            bool detailed = true)
        {
            builder.AppendLine(context.CreateKVP("Total Mono Heap",
                Localization.GetHumanReadableFileSize(resources.MonoHeapSize)));
            builder.AppendLine(context.CreateKVP("Used Mono Heap",
                Localization.GetHumanReadableFileSize(resources.MonoUsedSize)));

            if (detailed)
            {
                builder.AppendLine(context.CreateKVP("GFX Driver Allocated Memory",
                    Localization.GetHumanReadableFileSize(resources.UnityGraphicsDriverAllocatedMemory)));
                builder.AppendLine(context.CreateKVP("Total Allocated Memory",
                    Localization.GetHumanReadableFileSize(resources.UnityTotalAllocatedMemory)));
                builder.AppendLine(context.CreateKVP("Total Reserved Memory",
                    Localization.GetHumanReadableFileSize(resources.UnityTotalReservedMemory)));
                builder.AppendLine(context.CreateKVP("Total Unused Reserved Memory",
                    Localization.GetHumanReadableFileSize(resources.UnityTotalUnusedReservedMemory)));
                builder.AppendLine(context.CreateKVP("Used Heap",
                    Localization.GetHumanReadableFileSize(resources.UnityUsedHeapSize)));
            }
        }

        public static void AddObjectInfoLine(ObjectInfo info, ReportContext context, StringBuilder builder)
        {
            string workingNameString = info.Type.Name.PadRight(context.ObjectTypeWidth);
            if (workingNameString.Length > context.ObjectTypeWidth)
            {
                workingNameString = workingNameString.Substring(0, context.ObjectTypeWidth);
            }

            workingNameString = $"{workingNameString} {info.Name}".PadRight(context.ObjectNameTotalWidth);
            if (workingNameString.Length > context.ObjectNameTotalWidth)
            {
                workingNameString = workingNameString.Substring(0, context.ObjectNameTotalWidth);
            }


            builder.Append(workingNameString);
            builder.Append($" {Localization.GetHumanReadableFileSize(info.MemoryUsage)} x {info.CopyCount.ToString()}"
                .PadRight(context.ObjectSizeWidth));

            // Additional information
            string additional = info.GetDetailedInformation();
            if (additional != null)
            {
                if (additional.Length > context.ObjectInfoWidth)
                {
                    additional = additional.Substring(0, context.ObjectInfoWidth);
                }

                builder.AppendLine(additional);
            }
            else
            {
                builder.AppendLine();
            }
        }

        /// <summary>
        ///     Create a sized divider string for use in generating reports.
        /// </summary>
        /// <param name="context">Contextual information regarding the generation of the report.</param>
        /// <param name="divider">The optional character to use as the divider.</param>
        /// <returns>A sized string to be used as a divider.</returns>
        public static string CreateDivider(this ReportContext context, char divider = '-')
        {
            return "".PadRight(context.CharacterWidth, divider);
        }

        /// <summary>
        ///     Create a header with <paramref name="title" /> with repeated <paramref name="decorator" />s on the sides, filling
        ///     out to <see cref="ReportContext.CharacterWidth" />.
        /// </summary>
        /// <param name="context">Contextual information regarding the generation of the report.</param>
        /// <param name="title">The text to be treated as the title for the header.</param>
        /// <param name="decorator">The optional character to be used as the decorator.</param>
        /// <returns>A sized string to be used as a header.</returns>
        public static string CreateHeader(this ReportContext context, string title, char decorator = '=')
        {
            string workingTitle = $" {title.Trim()} ";

            int titleWidth = workingTitle.Length;
            int decoratorSideWidth = (context.CharacterWidth - titleWidth) / 2;

            // Pad left side first to ensure it is always the most accurate in length
            workingTitle = workingTitle.PadLeft(titleWidth + decoratorSideWidth, decorator);
            workingTitle = workingTitle.PadRight(context.CharacterWidth, decorator);

            return workingTitle;
        }

        public static string CreateKVP(this ReportContext context, string itemKey, string itemValue)
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
    }
}