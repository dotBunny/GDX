using System;
using System.Collections.Generic;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CollectionNeverQueried.Global
// ReSharper disable HeapView.ObjectAllocation.Evident

namespace GDX.Developer
{
    /// <summary>
    ///     A simplified commandline parser that handles arguments which follow the <c>--FLAG</c> or <c>--KEY=VALUE</c> format.
    ///     Automatically initialized during normal runtime operations, however can be manually triggered for author time
    ///     use cases as well.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The prefix and divider are configurable via the <see cref="Config" />, however the always, the <c>FLAG</c>
    ///         and <c>KEY</c> will be Uppercase.
    ///     </para>
    /// </remarks>
    public static class CommandLineParser
    {
        /// <summary>
        ///     The processed arguments found after parsing the arguments
        /// </summary>
        public static readonly Dictionary<string, string> Arguments = new Dictionary<string, string>();

        /// <summary>
        ///     The processed flags found in the arguments.
        /// </summary>
        public static readonly List<string> Flags = new List<string>();

        static CommandLineParser()
        {
            ProcessArguments(Environment.GetCommandLineArgs());
        }

        /// <summary>
        ///     Process an array of arguments into <see cref="Arguments" /> and <see cref="Flags" />.
        /// </summary>
        /// <param name="argumentArray">An array of arguments to process.</param>
        /// <param name="shouldClear">Should the storage containers be cleared.</param>
        public static void ProcessArguments(string[] argumentArray, bool shouldClear = true)
        {
            if (shouldClear)
            {
                Arguments.Clear();
                Flags.Clear();
            }

            string argumentPrefix = Core.Config.developerCommandLineParserArgumentPrefix;
            int prefixLength = argumentPrefix.Length;
            string argumentSplit = Core.Config.developerCommandLineParserArgumentSplit;
            int argumentLength = argumentArray.Length;
            for (int i = 0; i < argumentLength; i++)
            {
                // Cache current argument
                string argument = argumentArray[i];

                // Has the starter and has an assignment
                if (!argument.StartsWith(argumentPrefix))
                {
                    continue;
                }

                if (argument.Contains(argumentSplit))
                {
                    int keyEnd = argument.IndexOf(argumentSplit, StringComparison.Ordinal);
                    string key = argument.Substring(prefixLength, keyEnd - prefixLength);

                    int valueStart = argument.IndexOf(argumentSplit, StringComparison.Ordinal) + 1;
                    int valueEnd = argument.Length;
                    string value = argument.Substring(valueStart, valueEnd - valueStart);

                    // Value parameter
                    Arguments.Add(key.ToUpper(), value);
                }
                else
                {
                    string flag = argument.Substring(prefixLength).ToUpper();

                    // Flag parameter, ensure they are unique
                    if (!Flags.Contains(flag))
                    {
                        Flags.Add(flag);
                    }
                }
            }
        }
    }
}