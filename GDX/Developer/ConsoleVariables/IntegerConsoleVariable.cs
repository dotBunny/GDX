// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;

namespace GDX.Developer.ConsoleVariables
{
    public class IntegerConsoleVariable : ConsoleVariableBase
    {
        public Action<int> OnValueChanged;
        readonly int m_DefaultValue;
        int m_CurrentValue;

        public IntegerConsoleVariable(string name, string description, int defaultValue,
            ConsoleVariableFlags flags = ConsoleVariableFlags.None) : base(name, description, flags)
        {
            if (ConsoleVariableSettings.TryGetValue(name, out string settingsValue) && int.TryParse(settingsValue, out int newDefaultValue))
            {
                m_DefaultValue = newDefaultValue;
            }
            else
            {
                m_DefaultValue = defaultValue;
            }

            if (CommandLineParser.Arguments.ContainsKey(name) && int.TryParse(CommandLineParser.Arguments[name], out int newCurrentValue))
            {
                m_CurrentValue = newCurrentValue;
            }
            else
            {
                m_CurrentValue = defaultValue;
            }
        }

        /// <inheritdoc />
        public override ConsoleVariableType GetConsoleVariableType()
        {
            return ConsoleVariableType.Integer;
        }

        /// <inheritdoc />
        public override object GetBoxedValue()
        {
            return m_CurrentValue;
        }

        /// <inheritdoc />
        public sealed override void SetValueFromString(string newValue)
        {
            if (int.TryParse(newValue, out m_CurrentValue))
            {
                OnValueChanged?.Invoke(m_CurrentValue);
            }
        }

        /// <inheritdoc />
        public override string GetCurrentValueAsString()
        {
            return m_CurrentValue.ToString();
        }

        /// <inheritdoc />
        public override string GetDefaultValueAsString()
        {
            return m_DefaultValue.ToString();
        }

        public int GetValue()
        {
            return m_CurrentValue;
        }

        public void SetValue(int newValue)
        {
            m_CurrentValue = newValue;
            OnValueChanged?.Invoke(m_CurrentValue);
        }
    }
}