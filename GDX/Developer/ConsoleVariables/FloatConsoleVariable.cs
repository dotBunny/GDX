// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;

namespace GDX.Developer.ConsoleVariables
{
    public class FloatConsoleVariable : ConsoleVariableBase
    {
#if UNITY_2022_2_OR_NEWER
        public Action<float> OnValueChanged;
        readonly float m_DefaultValue;
        float m_CurrentValue;

        public FloatConsoleVariable(string name, string description, float defaultValue,
            ConsoleVariableFlags flags = ConsoleVariableFlags.None) : base(name, description, flags)
        {
            if (ConsoleVariableSettings.TryGetValue(name, out string settingsValue) && float.TryParse(settingsValue, out float newDefaultValue))
            {
                m_DefaultValue = newDefaultValue;
            }
            else
            {
                m_DefaultValue = defaultValue;
            }

            if (CommandLineParser.Arguments.ContainsKey(name) && float.TryParse(CommandLineParser.Arguments[name], out float newCurrentValue))
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
            return ConsoleVariableType.Float;
        }

        /// <inheritdoc />
        public override object GetBoxedValue()
        {
            return m_CurrentValue;
        }

        /// <inheritdoc />
        public sealed override void SetValueFromString(string newValue)
        {
            if(float.TryParse(newValue, out m_CurrentValue))
            {
                OnValueChanged?.Invoke(m_CurrentValue);
            }
        }

        /// <inheritdoc />
        public override string GetCurrentValueAsString()
        {
            return m_CurrentValue.ToString(CultureInfo.InvariantCulture);
        }

        /// <inheritdoc />
        public override string GetDefaultValueAsString()
        {
            return m_DefaultValue.ToString(CultureInfo.InvariantCulture);
        }

        public float GetValue()
        {
            return m_CurrentValue;
        }

        public void SetValue(float newValue)
        {
            m_CurrentValue = newValue;
            OnValueChanged?.Invoke(m_CurrentValue);
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}