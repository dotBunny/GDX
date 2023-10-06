// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;

namespace GDX.Developer.ConsoleVariables
{
    public class BooleanConsoleVariable : ConsoleVariableBase
    {

        public Action<bool> OnValueChanged;
        readonly bool m_DefaultValue;
        bool m_CurrentValue;

        public BooleanConsoleVariable(string name, string description, bool defaultValue,
            ConsoleVariableFlags flags = ConsoleVariableFlags.None) : base(name, description, flags)
        {
            if (ConsoleVariableSettings.TryGetValue(name, out string settingsValue) && bool.TryParse(settingsValue, out bool newDefaultValue))
            {
                m_DefaultValue = newDefaultValue;
            }
            else
            {
                m_DefaultValue = defaultValue;
            }

            if (CommandLineParser.Arguments.ContainsKey(name) && bool.TryParse(CommandLineParser.Arguments[name], out bool newCurrentValue))
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
            return ConsoleVariableType.Boolean;
        }

        /// <inheritdoc />
        public override object GetBoxedValue()
        {
            return m_CurrentValue;
        }

        /// <inheritdoc />
        public sealed override void SetValueFromString(string newValue)
        {
            if (bool.TryParse(newValue, out m_CurrentValue))
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

        public bool GetValue()
        {
            return m_CurrentValue;
        }

        public void SetValue(bool newValue)
        {
            m_CurrentValue = newValue;
            OnValueChanged?.Invoke(m_CurrentValue);
        }
    }
}