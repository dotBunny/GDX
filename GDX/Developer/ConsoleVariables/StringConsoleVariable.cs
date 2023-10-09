// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;

namespace GDX.Developer.ConsoleVariables
{
#if UNITY_2022_2_OR_NEWER
    public class StringConsoleVariable : ConsoleVariableBase
    {
        public Action<string> OnValueChanged;
        readonly string m_DefaultValue;
        string m_CurrentValue;

        public StringConsoleVariable(string name, string description, string defaultValue,
            ConsoleVariableFlags flags = ConsoleVariableFlags.None) : base(name, description, flags)
        {
            m_DefaultValue = ConsoleVariableSettings.TryGetValue(name, out string settingsValue) ? settingsValue : defaultValue;
            m_CurrentValue = CommandLineParser.Arguments.ContainsKey(name) ? CommandLineParser.Arguments[name] : defaultValue;
        }

        /// <inheritdoc />
        public override ConsoleVariableType GetConsoleVariableType()
        {
            return ConsoleVariableType.String;
        }

        /// <inheritdoc />
        public override object GetBoxedValue()
        {
            return m_CurrentValue;
        }

        /// <inheritdoc />
        public sealed override void SetValueFromString(string newValue)
        {
            m_CurrentValue = newValue;
            OnValueChanged?.Invoke(m_CurrentValue);
        }

        /// <inheritdoc />
        public override string GetCurrentValueAsString()
        {
            return m_CurrentValue;
        }

        /// <inheritdoc />
        public override string GetDefaultValueAsString()
        {
            return m_DefaultValue;
        }

        public string GetValue()
        {
            return m_CurrentValue;
        }

        public void SetValue(string newValue)
        {
            m_CurrentValue = newValue;
            OnValueChanged?.Invoke(m_CurrentValue);
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}