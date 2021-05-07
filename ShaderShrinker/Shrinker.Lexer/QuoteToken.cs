using System.Linq;

namespace Shrinker.Lexer
{
    public class QuoteToken : Token
    {
        public override IToken TryMatch(string code, ref int offset) =>
            "'\"".Contains(Peek(code, offset)) ? new QuoteToken { Content = Read(code, ref offset) } : null;
    }
}