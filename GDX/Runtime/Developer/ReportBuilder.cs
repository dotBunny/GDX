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
        ///     The number of characters to be considered a line in a report.
        /// </summary>
        public const int CharacterWidth = 120;

        public const int DataTableItemWidth = 40;

        public const int ObjectTypeWidth = 19;
        public const int ObjectNameTotalWidth = 64;
        public const int ObjectSizeWidth = 14;
        public const int ObjectInfoWidth = 20;

        /// <summary>
        ///     A <see cref="string" /> array used to represent the end of a line for splitting purposes.
        /// </summary>
        public static readonly string[] NewLineSplit = {Environment.NewLine};

        public static void AddInstanceInformation(Report report, ref StringBuilder builder)
        {
            builder.AppendLine($"{"Active Scene:".PadRight(DataTableItemWidth)}{report.ActiveScene}");
            builder.AppendLine($"{"Platform:".PadRight(DataTableItemWidth)}{report.Platform.ToString()}");
            builder.AppendLine($"{"Created Scene:".PadRight(DataTableItemWidth)}{report.Created.ToString(Localization.LocalTimestampFormat)}");
        }

        public static void AddMemoryInformation(ResourcesReport resources, ref StringBuilder builder,
            bool detailed = true)
        {
            builder.AppendLine($"{"Total Mono Heap:".PadRight(DataTableItemWidth)}{Localization.GetHumanReadableFileSize(resources.MonoHeapSize)}");
            builder.AppendLine($"{"Used Mono Heap:".PadRight(DataTableItemWidth)}{Localization.GetHumanReadableFileSize(resources.MonoUsedSize)}");

            if (detailed)
            {
                builder.AppendLine($"{"GFX Driver Allocated Memory:".PadRight(DataTableItemWidth)}{Localization.GetHumanReadableFileSize(resources.UnityGraphicsDriverAllocatedMemory)}");
                builder.AppendLine($"{"Total Allocated Memory:".PadRight(DataTableItemWidth)}{Localization.GetHumanReadableFileSize(resources.UnityTotalAllocatedMemory)}");
                builder.AppendLine($"{"Total Reserved Memory:".PadRight(DataTableItemWidth)}{Localization.GetHumanReadableFileSize(resources.UnityTotalReservedMemory)}");
                builder.AppendLine($"{"Total Unused Reserved Memory:".PadRight(DataTableItemWidth)}{Localization.GetHumanReadableFileSize(resources.UnityTotalUnusedReservedMemory)}");
                builder.AppendLine($"{"Used Heap:".PadRight(DataTableItemWidth)}{Localization.GetHumanReadableFileSize(resources.UnityUsedHeapSize)}");
            }
        }

        public static void AddObjectInfoLine(ObjectInfo info, ref StringBuilder builder)
        {
            string workingNameString = info.Type.Name.PadRight(ObjectTypeWidth);
            if (workingNameString.Length > ObjectTypeWidth)
            {
                workingNameString = workingNameString.Substring(0, ObjectTypeWidth);
            }
            workingNameString = $"{workingNameString} {info.Name}".PadRight(ObjectNameTotalWidth);
            if (workingNameString.Length > ObjectNameTotalWidth)
            {
                workingNameString = workingNameString.Substring(0, ObjectNameTotalWidth);
            }



            builder.Append(workingNameString);
            builder.Append( $" {Localization.GetHumanReadableFileSize(info.MemoryUsage)} x {info.CopyCount.ToString()}"
                .PadRight(ObjectSizeWidth));

            // Additional information
            string additional = info.GetDetailedInformation();
            if (additional != null)
            {
                if (additional.Length > ObjectInfoWidth)
                {
                    additional = additional.Substring(0, ObjectInfoWidth);
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
        /// <param name="divider">The optional character to use as the divider.</param>
        /// <returns>A sized string to be used as a divider.</returns>
        public static string CreateDivider(char divider = '-')
        {
            return "".PadRight(CharacterWidth, divider);
        }

        /// <summary>
        ///     Create a header with <paramref name="title" /> with repeated <paramref name="decorator" />s on the sides, filling
        ///     out to <see cref="CharacterWidth" />.
        /// </summary>
        /// <param name="title">The text to be treated as the title for the header.</param>
        /// <param name="decorator">The optional character to be used as the decorator.</param>
        /// <returns>A sized string to be used as a header.</returns>
        public static string CreateHeader(string title, char decorator = '=')
        {
            string workingTitle = $" {title.Trim()} ";

            int titleWidth = workingTitle.Length;
            int decoratorSideWidth = (CharacterWidth - titleWidth) / 2;

            // Pad left side first to ensure it is always the most accurate in length
            workingTitle = workingTitle.PadLeft(titleWidth + decoratorSideWidth, decorator);
            workingTitle = workingTitle.PadRight(CharacterWidth, decorator);

            return workingTitle;
        }
    }
}