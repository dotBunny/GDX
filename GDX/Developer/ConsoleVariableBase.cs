// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;

namespace GDX.Developer
{
    public abstract class ConsoleVariableBase
    {
        public enum ConsoleVariableType
        {
            String,
            Integer,
            Float,
            Boolean
        }

        [Flags]
        public enum ConsoleVariableFlags
        {
            None = 0x0,
            Setting = 0x1,
            Cheat = 0x2
        }

        readonly ConsoleVariableFlags m_Flags;
        readonly string m_Name;
        readonly string m_Description;

        /// <summary>
        ///     Returns the minimum access level required to set a variable.
        /// </summary>
        /// <remarks>Overrideable, but defaults to having user level access.</remarks>
        /// <returns>The required user access level to alter a variable.</returns>
        public virtual Console.ConsoleAccessLevel GetAccessLevel()
        {
            return Console.ConsoleAccessLevel.Anonymous;
        }

        public abstract ConsoleVariableType GetConsoleVariableType();
        public abstract object GetBoxedValue();
        public abstract void SetValueFromString(string newValue);
        public abstract string GetCurrentValueAsString();
        public abstract string GetDefaultValueAsString();

        protected ConsoleVariableBase(string name, string description, ConsoleVariableFlags flags)
        {
            m_Name = name;
            m_Description = description;
            m_Flags = flags;

            Console.RegisterVariable(this);
        }

        public string GetName()
        {
            return m_Name;
        }

        public string GetDescription()
        {
            return m_Description;
        }

        public ConsoleVariableFlags GetFlags()
        {
            return m_Flags;
        }
    }
}