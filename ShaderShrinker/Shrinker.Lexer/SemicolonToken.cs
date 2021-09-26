namespace Shrinker.Lexer
{
    public class SemicolonToken : Token
    {
        public SemicolonToken()
        {
            Content = ";";
        }

        public override IToken TryMatch(string code, ref int offset) =>
            Peek(code, offset) == ';' ? new SemicolonToken { Content = Read(code, ref offset) } : null;

        public override IToken Clone() => new SemicolonToken();
    }
}