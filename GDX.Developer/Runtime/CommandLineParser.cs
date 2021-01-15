using System;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CollectionNeverQueried.Global
// ReSharper disable HeapView.ObjectAllocation.Evident

namespace GDX.Developer
{
    /// <summary>
    ///     A parser specifically for command line arguments which follow the --FLAG or --KEY=VALUE format.
    /// </summary>
    /// <remarks>
    ///     <para>This does NOT support the /FLAG or /KEY=VALUE format. All FLAGs and KEYs are stored in Uppercase.</para>
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

        /// <summary>
        ///     Process the environment's commandline arguments into <see cref="Arguments" /> and <see cref="Flags" />.
        /// </summary>
        /// <remarks>
        ///     <para>This is automatically done at runtime.</para>
        /// </remarks>
        [RuntimeInitializeOnLoadMethod]
        public static void ParseArguments()
        {
            Arguments.Clear();
            Flags.Clear();

            GDXConfig gdxConfig = GDXConfig.Get();
            string argumentPrefix = gdxConfig.developerCommandLineParserArgumentPrefix;
            int prefixLength = argumentPrefix.Length;
            string argumentSplit = gdxConfig.developerCommandLineParserArgumentSplit;
            
            foreach (string argument in Environment.GetCommandLineArgs())
            {
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