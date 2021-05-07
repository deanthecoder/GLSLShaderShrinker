namespace Shrinker.Lexer
{
    public class EqualityOperatorToken : Token
    {
        public override IToken TryMatch(string code, ref int offset) =>
            Peek(code, offset, 2) == "==" ? new EqualityOperatorToken { Content = Read(code, ref offset, 2) } : null;
    }
}