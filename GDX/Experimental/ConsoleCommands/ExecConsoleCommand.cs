// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Collections.Generic;
using GDX.Experimental.Logging;

namespace GDX.Experimental.ConsoleCommands
{
    public class ExecConsoleCommand : ConsoleCommandBase
    {
        string[] m_FilePaths;
        int m_FilePathsCount;

        /// <inheritdoc />
        public override bool Evaluate(float deltaTime)
        {
            for (int i = 0; i < m_FilePathsCount; i++)
            {
                string[] lines = File.ReadAllLines(m_FilePaths[i]);
                ManagedLog.Info(LogCategory.Default, $"Queueing commands found in {m_FilePaths[i]} ...");
                int lineCount = lines.Length;
                for (int j = 0; j < lineCount; j++)
                {
                    string line = lines[j].Trim();
#if UNITY_2021_3_OR_NEWER
                    if (line.StartsWith('#'))
#else
                    if (line.StartsWith("#"))
#endif
                    {
                        continue;
                    }

                    if (!DeveloperConsole.QueueCommand(line))
                    {
                        ManagedLog.Warning(LogCategory.Default,
                            $"An error occured adding command '{line}' at line #{j} in {m_FilePaths[i]}.");
                    }
                }
            }
            return true;
        }

        /// <inheritdoc />
        public override string GetKeyword()
        {
            return "exec";
        }

        /// <inheritdoc />
        public override string GetHelpUsage()
        {
            return "exec <absolute path>";
        }

        /// <inheritdoc />
        public override string GetHelpMessage()
        {
            return "Execute an arbitrary script file based on the known commands of the console.";
        }

        /// <inheritdoc />
        public override ConsoleCommandBase GetInstance(string context)
        {
#if UNITY_2021_3_OR_NEWER
            string[] files = context.Split(',', StringSplitOptions.RemoveEmptyEntries);
#else
            string[] files = context.Split(",", StringSplitOptions.RemoveEmptyEntries);
#endif
            int fileCount = files.Length;
            List<string> foundFiles = new List<string>(fileCount);

            for (int i = 0; i < fileCount; i++)
            {
                string workingFile = files[i];
                if (!File.Exists(workingFile))
                {
                    ManagedLog.Warning(LogCategory.Default, $"Unable to find script to execute @ {workingFile}.");
                    continue;
                }
                foundFiles.Add(workingFile);
            }

            if (foundFiles.Count <= 0)
            {
                return null;
            }

            ExecConsoleCommand command = new ExecConsoleCommand() {
                m_FilePaths = foundFiles.ToArray(),
                m_FilePathsCount = foundFiles.Count
            };
            return command;
        }
    }
}