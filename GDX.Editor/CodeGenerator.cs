// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

// ReSharper disable MemberCanBePrivate.Global

namespace GDX.Editor
{
    public class CodeGenerator
    {
        private readonly System.Text.StringBuilder m_Builder = new System.Text.StringBuilder();
        private int m_IndentLevel;
        private const string k_IndentContent = "    ";

        public CodeGenerator(string namespaceIdentifier, string generatedNotice = "Generated file. DO NOT EDIT.")
        {
            m_Builder.AppendLine($"// {generatedNotice}");
            m_Builder.AppendLine($"namespace {namespaceIdentifier}");
            OpenBrace();
        }

        // ReSharper disable once UnusedMember.Global
        public void Append(string text)
        {
            m_Builder.Append(text);
        }

        public void AppendLine(string line = "")
        {
            ApplyIndent();
            m_Builder.AppendLine(line);
        }

        public void ApplyIndent()
        {
            for (int i = 0; i < m_IndentLevel; i++)
            {
                m_Builder.Append(k_IndentContent);
            }
        }

        public void CloseBrace(string suffix = "")
        {
            PopIndent();
            AppendLine($"}}{suffix}");
        }

        public void OpenBrace()
        {
            AppendLine("{");
            PushIndent();
        }

        public void PushIndent()
        {
            m_IndentLevel++;
        }

        public void PopIndent()
        {
            if (m_IndentLevel > 0)
            {
                m_IndentLevel--;
            }
        }

        public override string ToString()
        {
            while (m_IndentLevel > 0)
            {
                CloseBrace();
            }
            return m_Builder.ToString();
        }
    }
}