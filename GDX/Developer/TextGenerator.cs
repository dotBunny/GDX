using System.Collections;
using System.Text;

namespace GDX.Developer
{
    /// <summary>
    ///     A formatted text generator useful for creating text based files with some semblance of organization.
    /// </summary>
    public class TextGenerator
    {
        /// <summary>
        ///     The actual buffer holder used to create the dynamic string.
        /// </summary>
        readonly StringBuilder m_Builder;

        /// <summary>
        ///     The content used to indicate the closing of a section that was indented.
        /// </summary>
        readonly string m_IndentClose;

        /// <summary>
        ///     The assigned indent content used to indent lines where applicable.
        /// </summary>
        readonly string m_IndentContent;

        /// <summary>
        ///     The content used to indicate the opening of a section which should be indented.
        /// </summary>
        readonly string m_IndentOpen;

        /// <summary>
        ///     The current level of indentation.
        /// </summary>
        int m_IndentLevel;

        /// <summary>
        ///     Create a new <see cref="TextGenerator" /> with the
        /// </summary>
        /// <param name="indentContent">
        ///     The characters used to indent the content when applicable. By default it will use a tab representation,
        ///     however for code files you may want to use four spaces.
        /// </param>
        /// <param name="indentOpen"></param>
        /// <param name="indentClose"></param>
        public TextGenerator(string indentContent = "\t", string indentOpen = null, string indentClose = null)
        {
            m_IndentContent = indentContent;
            m_IndentOpen = indentOpen;
            m_IndentClose = indentClose;
            m_Builder = new StringBuilder();
        }

        /// <summary>
        ///     Apply the current level of indent to the current line being operated on.
        /// </summary>
        public void ApplyIndent()
        {
            for (int i = 0; i < m_IndentLevel; i++)
            {
                m_Builder.Append(m_IndentContent);
            }
        }

        /// <summary>
        ///     Append content to the current line being operated on.
        /// </summary>
        /// <param name="content">The content to append to the current line.</param>
        public void Append(string content)
        {
            m_Builder.Append(content);
        }

        /// <summary>
        ///     Apply the appropriate amount of indentation to the current line, appending content afterwards and then
        ///     advancing to the next line.
        /// </summary>
        /// <param name="content">The content to append to the current line.</param>
        public void AppendLine(string content = "")
        {
            ApplyIndent();
            m_Builder.AppendLine(content);
        }

        /// <summary>
        ///     Append an <see cref="IEnumerable" /> set of content as individual lines with proper indentation.
        /// </summary>
        /// <param name="content">The content to be added.</param>
        public void AppendLineRange(IEnumerable content)
        {
            foreach (string s in content)
            {
                ApplyIndent();
                m_Builder.AppendLine(s);
            }
        }

        /// <summary>
        ///     Gets the current indent level of the <see cref="TextGenerator" />.
        /// </summary>
        /// <returns>The indent level.</returns>
        public int GetIndentLevel()
        {
            return m_IndentLevel;
        }

        /// <summary>
        ///     Move the builder to the start of the next line.
        /// </summary>
        public void NextLine()
        {
            m_Builder.AppendLine();
        }

        /// <summary>
        ///     Remove a level of indentation from the builder.
        /// </summary>
        public void PopIndent()
        {
            if (m_IndentLevel <= 0)
            {
                return;
            }

            m_IndentLevel--;
            if (m_IndentOpen == null)
            {
                return;
            }

            ApplyIndent();
            m_Builder.AppendLine(m_IndentClose);
        }

        /// <summary>
        ///     Add a level of indentation to the builder.
        /// </summary>
        /// <param name="applyOpener">Should the opener be applied?</param>
        public void PushIndent(bool applyOpener = true)
        {
            if (m_IndentOpen != null && applyOpener)
            {
                ApplyIndent();
                m_Builder.AppendLine(m_IndentOpen);
            }

            m_IndentLevel++;
        }

        /// <summary>
        ///     Returns the built string content for the builder.
        /// </summary>
        /// <remarks>Will automatically reduce the indentation level to 0.</remarks>
        public override string ToString()
        {
            while (m_IndentLevel > 0)
            {
                PopIndent();
            }

            return m_Builder.ToString().Trim();
        }
    }
}