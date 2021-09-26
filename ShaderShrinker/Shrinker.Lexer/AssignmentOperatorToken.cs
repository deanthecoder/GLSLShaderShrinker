namespace Shrinker.Lexer
{
    public class AssignmentOperatorToken : Token
    {
        public override IToken TryMatch(string code, ref int offset) =>
            Peek(code, offset) == '=' ? new AssignmentOperatorToken { Content = Read(code, ref offset) } : null;

        public override IToken Clone() => new AssignmentOperatorToken { Content = Content };
    }
}