namespace Shrinker.Lexer
{
    public class WhitespaceToken : Token
    {
        public override IToken TryMatch(string code, ref int offset)
        {
            var count = 0;

            char ch;
            while (char.IsWhiteSpace(ch = Peek(code, offset + count)))
            {
                if (ch.IsNewline())
                    break;

                count++;
            }

            return count == 0 ? null : new WhitespaceToken { Content = Read(code, ref offset, count) };
        }
    }
}