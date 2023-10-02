// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;

namespace GDX.Developer
{

    //https://docs.unity3d.com/Packages/com.unity.burst@1.8/manual/csharp-shared-static.html
    public class ConsoleVariable
    {
        //TODO: Generic class? Might make parsing a problem
        // TODO: NO side static class with actuals

        enum ConsoleVariableType
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
        readonly ConsoleVariableType m_Type;
        readonly string m_Name;
        readonly string m_Description;

        // Wasteful allocation
        int m_IntegerValue;
        float m_FloatValue;
        bool m_BooleanValue;
        string m_StringValue;

        public ConsoleVariable(string name, string description, string defaultValue, ConsoleVariableFlags flags = ConsoleVariableFlags.None)
        {
            m_Type = ConsoleVariableType.String;
            m_Name = name;
            m_Description = description;
            m_StringValue = CommandLineParser.Arguments.ContainsKey(name) ? CommandLineParser.Arguments[name] : defaultValue;
            m_Flags = flags;
        }

        public ConsoleVariable(string name, string description, int defaultValue, ConsoleVariableFlags flags = ConsoleVariableFlags.None)
        {
            m_Type = ConsoleVariableType.Integer;
            m_Name = name;
            m_Description = description;
            if (CommandLineParser.Arguments.ContainsKey(name))
            {
                if(!int.TryParse(CommandLineParser.Arguments[name], out m_IntegerValue))
                {
                    m_IntegerValue = defaultValue;
                }
            }
            else
            {
                m_IntegerValue = defaultValue;
            }
            m_Flags = flags;
        }

        public ConsoleVariable(string name, string description, float defaultValue, ConsoleVariableFlags flags = ConsoleVariableFlags.None)
        {
            m_Type = ConsoleVariableType.Float;
            m_Name = name;
            m_Description = description;
            if (CommandLineParser.Arguments.ContainsKey(name))
            {
                if(!float.TryParse(CommandLineParser.Arguments[name], out m_FloatValue))
                {
                    m_FloatValue = defaultValue;
                }
            }
            else
            {
                m_FloatValue = defaultValue;
            }
            m_Flags = flags;
        }

        public ConsoleVariable(string name, string description, bool defaultValue, ConsoleVariableFlags flags = ConsoleVariableFlags.None)
        {
            m_Type = ConsoleVariableType.Boolean;
            m_Name = name;
            m_Description = description;
            if (CommandLineParser.Arguments.ContainsKey(name))
            {
                if(!bool.TryParse(CommandLineParser.Arguments[name], out m_BooleanValue))
                {
                    m_BooleanValue = defaultValue;
                }
            }
            else
            {
                m_BooleanValue = defaultValue;
            }
            m_Flags = flags;
        }
    }
}