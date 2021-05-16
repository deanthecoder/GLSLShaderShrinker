using System.Linq;
using System.Text;

namespace Shrinker.Lexer
{
    /// <summary>
    /// Represents a Preprocessor directives (#define, #ifdef, #version, #...)
    /// </summary>
    public class PreprocessorToken : Token
    {
        public override IToken TryMatch(string code, ref int offset)
        {
            if (Peek(code, offset) != '#')
                return null;

            var count = 1;
            while (char.IsWhiteSpace(Peek(code, offset + count)))
                count++;
            while (char.IsLetter(Peek(code, offset + count)))
                count++;

            var s = Peek(code, offset, count);
            if (s.RemoveAllWhitespace() == "#define")
            {
                offset += s.Length;

                var toEndOfLine = new StringBuilder();
                var i = offset + 1;
                while (!new[] { '\r', '\n', 0 }.Contains(Peek(code, i)))
                {
                    toEndOfLine.Append(Peek(code, i));
                    i++;
                }

                return new PreprocessorDefineToken(toEndOfLine.ToString());
            }

            if (s.RemoveAllWhitespace().IsAnyOf("#pragma", "#version"))
            {
                offset += s.Length;

                var toEndOfLine = new StringBuilder();
                while (!new[] { '\r', '\n', 0 }.Contains(Peek(code, offset)))
                    toEndOfLine.Append(Read(code, ref offset));
                var newLine = new StringBuilder();
                while (Peek(code, offset).IsNewline())
                    newLine.Append(Read(code, ref offset));

                return new VerbatimToken($"{s}{toEndOfLine}{newLine}");
            }

            return s.RemoveAllWhitespace().IsAnyOf("#ifdef", "#ifndef", "#if", "#else", "#endif") ? new PreprocessorToken { Content = Read(code, ref offset, s.Length) } : null;
        }
    }
}