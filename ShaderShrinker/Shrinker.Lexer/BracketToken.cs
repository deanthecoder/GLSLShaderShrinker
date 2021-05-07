namespace Shrinker.Lexer
{
    public class BracketToken : Token
    {
        public override IToken TryMatch(string code, ref int offset) =>
            "(){}[]".Contains(Peek(code, offset, 1)) ? new BracketToken { Content = Read(code, ref offset) } : null;
    }
}