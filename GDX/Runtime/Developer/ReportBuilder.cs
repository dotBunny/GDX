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
                builder.AppendLine(context.CreateKVP("Total Reserved Memory",
                    Localization.GetHumanReadableFileSize(resources.UnityTotalReservedMemory)));
                builder.AppendLine(context.CreateKVP("Total Allocated Memory",
                    Localization.GetHumanReadableFileSize(resources.UnityTotalAllocatedMemory)));
                builder.AppendLine(context.CreateKVP("Total Unused Reserved Memory",
                    Localization.GetHumanReadableFileSize(resources.UnityTotalUnusedReservedMemory)));
            }
        }

        public static void AddObjectInfoLine(ObjectInfo info, ReportContext context, StringBuilder builder)
        {
            string typeName = info.Type.Name.PadRight(context.ObjectTypeWidth);
            if (typeName.Length > context.ObjectTypeWidth)
            {
                typeName = typeName.Substring(0, context.ObjectTypeWidth);
            }

            string objectName = info.Name.PadRight(context.ObjectNameWidth);
            if (objectName.Length > context.ObjectNameWidth)
            {
                objectName = objectName.Substring(0, context.ObjectNameWidth);
            }

            string sizeInfo =
                $"{Localization.GetHumanReadableFileSize(info.MemoryUsage)} x {info.CopyCount.ToString()}".PadRight(
                    context.ObjectSizeWidth);
            if (sizeInfo.Length > context.ObjectSizeWidth)
            {
                sizeInfo = sizeInfo.Substring(0, context.ObjectSizeWidth);
            }


            // Additional information
            string additionalInfo = info.GetDetailedInformation(context.ObjectInfoWidth);

            // Add to builder
            builder.AppendLine( additionalInfo != null
                ? $"{typeName} {objectName} {sizeInfo} {additionalInfo}"
                : $"{typeName} {objectName} {sizeInfo}");
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

        public static string CreateYesNo(this bool flag)
        {
            return flag ? "Y" : "N";
        }
    }
}