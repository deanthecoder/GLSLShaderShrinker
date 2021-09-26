using System.Linq;

namespace Shrinker.Lexer
{
    public class LineEndToken : Token
    {
        public override IToken TryMatch(string code, ref int offset)
        {
            var s = Peek(code, offset, 2);

            var doubleCharacters = new[] { "\r\n", "\n\r" };
            if (doubleCharacters.Contains(s))
                return new LineEndToken { Content = Read(code, ref offset, 2) };

            return "\r\n".Contains(s[0]) ? new LineEndToken { Content = Read(code, ref offset) } : null;
        }

        public override IToken Clone() => new LineEndToken { Content = Content };
    }
}