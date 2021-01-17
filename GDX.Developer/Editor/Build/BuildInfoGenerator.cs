// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;

namespace GDX.Developer.Editor.Build
{
    /// <summary>
    /// The BuildInfo file generator.
    /// </summary>
    public static class BuildInfoGenerator
    {
        /// <summary>
        /// Create the content for the BuildInfo file based on provided information.
        /// </summary>
        /// <param name="config">A <see cref="GDXConfig"/> used to determine many of the keys for information.</param>
        /// <param name="forceDefaults">Should all default values be used instead?</param>
        /// <param name="internalDescription">An internally used description.</param>
        /// <returns>The files content.</returns>
        public static string GetContent(GDXConfig config, bool forceDefaults = false,
            string internalDescription = Strings.Null)
        {
            StringBuilder fileContent = new StringBuilder();

            fileContent.Append("namespace ");
            fileContent.Append(config.developerBuildInfoNamespace);
            fileContent.AppendLine();

            fileContent.AppendLine("{");

            fileContent.AppendLine("    /// <summary>");
            fileContent.AppendLine(
                "            A collection of information providing further information as to the conditions present when the build was made.");
            fileContent.AppendLine("    /// </summary>");
            fileContent.AppendLine("    public static class BuildInfo");
            fileContent.AppendLine("    {");

            // BuildNumber
            fileContent.AppendLine("        /// <summary>");
            fileContent.AppendLine("                The builds numerically incremented version.");
            fileContent.AppendLine("        /// </summary>");
            fileContent.AppendLine("        /// <remarks>");
            fileContent.AppendLine("                This may not be a shared build number across all build tasks.");
            fileContent.AppendLine("        /// </remarks>");
            fileContent.Append("        public const int BuildNumber = ");
            fileContent.Append(!forceDefaults &&
                               CommandLineParser.Arguments.ContainsKey(config.developerBuildInfoBuildNumberArgument)
                ? CommandLineParser.Arguments[config.developerBuildInfoBuildNumberArgument]
                : "0");
            fileContent.AppendLine(";");

            // Changelist
            fileContent.AppendLine("        /// <summary>");
            fileContent.AppendLine("                The revision the workspace was at when the build was made.");
            fileContent.AppendLine("        /// </summary>");
            fileContent.Append("        public const int Changelist = ");
            fileContent.Append(!forceDefaults &&
                               CommandLineParser.Arguments.ContainsKey(config.developerBuildInfoBuildChangelistArgument)
                ? CommandLineParser.Arguments[config.developerBuildInfoBuildChangelistArgument]
                : "0");
            fileContent.AppendLine(";");


            // BuildTask
            fileContent.AppendLine("        /// <summary>");
            fileContent.AppendLine("                The specific build task used to create the build.");
            fileContent.AppendLine("        /// </summary>");
            fileContent.Append("        public const string BuildTask = \"");
            fileContent.Append(!forceDefaults &&
                               CommandLineParser.Arguments.ContainsKey(config.developerBuildInfoBuildTaskArgument)
                ? CommandLineParser.Arguments[config.developerBuildInfoBuildTaskArgument]
                : "N/A");
            fileContent.AppendLine("\";");

            // Stream
            fileContent.AppendLine("        /// <summary>");
            fileContent.AppendLine("                The version control stream which the build was built from.");
            fileContent.AppendLine("        /// </summary>");
            fileContent.Append("        public const string Stream = \"");
            fileContent.Append(!forceDefaults &&
                               CommandLineParser.Arguments.ContainsKey(config.developerBuildInfoBuildStreamArgument)
                ? CommandLineParser.Arguments[config.developerBuildInfoBuildStreamArgument]
                : "N/A");
            fileContent.AppendLine("\";");

            // Build Description
            fileContent.AppendLine("        /// <summary>");
            fileContent.AppendLine("                The passed in build description.");
            fileContent.AppendLine("        /// </summary>");
            fileContent.Append("        public const string Description = \"");
            fileContent.Append(!forceDefaults &&
                               CommandLineParser.Arguments.ContainsKey(
                                   config.developerBuildInfoBuildDescriptionArgument)
                ? CommandLineParser.Arguments[config.developerBuildInfoBuildDescriptionArgument]
                : "N/A");
            fileContent.AppendLine("\";");

            // Internal Description
            fileContent.AppendLine("        /// <summary>");
            fileContent.AppendLine("                The internal description set through method invoke.");
            fileContent.AppendLine("        /// </summary>");
            fileContent.Append("        public const string InternalDescription = \"");
            fileContent.Append(!forceDefaults && !string.IsNullOrEmpty(internalDescription)
                ? internalDescription : "N/A");
            fileContent.AppendLine("\";");

            // Timestamp
            fileContent.AppendLine("        /// <summary>");
            fileContent.AppendLine("                The date and time when the build was started.");
            fileContent.AppendLine("        /// </summary>");
            fileContent.Append("        public const string Timestamp = \"");
            fileContent.Append(!forceDefaults
                ? DateTime.Now.ToString(Localization.Language.Default.GetTimestampFormat())
                : "N/A");
            fileContent.AppendLine("\";");

            fileContent.AppendLine("\t}");

            fileContent.AppendLine("}");

            return fileContent.ToString();
        }
    }
}