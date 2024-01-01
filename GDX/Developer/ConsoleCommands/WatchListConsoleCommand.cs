// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Developer.ConsoleCommands
{
#if UNITY_2022_2_OR_NEWER
    public class WatchListConsoleCommand : ConsoleCommandBase
    {
        string m_Filter = null;

        public override bool Evaluate(float deltaTime)
        {
			TextGenerator enabledGenerator = new TextGenerator();
            enabledGenerator.AppendLine("Enabled Watches");

            TextGenerator disabledGenerator = new TextGenerator();
            disabledGenerator.AppendLine("Disabled Watches");

            bool isFiltering = !string.IsNullOrEmpty(m_Filter);

            WatchProvider.WatchList watches = WatchProvider.GetWatchList();

            for (int i = 0; i < watches.Count; i++)
            {
                if (isFiltering && !watches.Identfiers[i].Contains(m_Filter))
                {
                    continue;
                }

                if (watches.IsActive[i])
                {
                    enabledGenerator.AppendLine($"\t{watches.Identfiers[i]} : {watches.DisplayNames[i]}");
                }
                else
                {
                    disabledGenerator.AppendLine($"\t{watches.Identfiers[i]} : {watches.DisplayNames[i]}");
                }
            }
            enabledGenerator.AppendLine(disabledGenerator.ToString());
            UnityEngine.Debug.Log(enabledGenerator.ToString());
            return true;
        }

        /// <inheritdoc />
        public override string GetKeyword()
        {
            return "watch.list";
        }

        /// <inheritdoc />
        public override string GetHelpUsage()
        {
            return "watch.list <filter>";
        }

        /// <inheritdoc />
        public override string GetHelpMessage()
        {
            return "Return a list of all registered watches, with the ability to filter.";
        }

        /// <inheritdoc />
        public override ConsoleCommandBase GetInstance(string context)
        {
            return new WatchListConsoleCommand { m_Filter = context };
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}