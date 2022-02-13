// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

// ReSharper disable MemberCanBePrivate.Global

namespace GDX.Editor
{
    public class CodeGenerator
    {
        private readonly System.Text.StringBuilder _builder = new System.Text.StringBuilder();
        private int _indentLevel;
        private const string IndentContent = "    ";

        public CodeGenerator(string namespaceIdentifier, string generatedNotice = "Generated file. DO NOT EDIT.")
        {
            _builder.AppendLine($"// {generatedNotice}");
            _builder.AppendLine($"namespace {namespaceIdentifier}");
            OpenBrace();
        }

        // ReSharper disable once UnusedMember.Global
        public void Append(string text)
        {
            _builder.Append(text);
        }

        public void AppendLine(string line = "")
        {
            ApplyIndent();
            _builder.AppendLine(line);
        }

        public void ApplyIndent()
        {
            for (int i = 0; i < _indentLevel; i++)
            {
                _builder.Append(IndentContent);
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
            _indentLevel++;
        }

        public void PopIndent()
        {
            if (_indentLevel > 0)
            {
                _indentLevel--;
            }
        }

        public override string ToString()
        {
            while (_indentLevel > 0)
            {
                CloseBrace();
            }
            return _builder.ToString();
        }
    }
}