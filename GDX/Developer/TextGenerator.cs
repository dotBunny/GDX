namespace GDX.Developer
{
    public class TextGenerator
    {
        private readonly System.Text.StringBuilder m_Builder = new System.Text.StringBuilder();
        private int m_IndentLevel;
        private const string k_IndentContent = "\t";

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

        public void NextLine()
        {
            m_Builder.AppendLine();
        }

        public void ApplyIndent()
        {
            for (int i = 0; i < m_IndentLevel; i++)
            {
                m_Builder.Append(k_IndentContent);
            }
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
                PopIndent();
            }
            return m_Builder.ToString();
        }
    }
}