// -----------------------------------------------------------------------
//  <copyright file="IntToken.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

namespace Shrinker.Lexer
{
    public class IntToken : Token, INumberToken
    {
        public IntToken()
        {
        }

        public IntToken(int i)
        {
            Content = i.ToString("D");
        }

        public override IToken TryMatch(string code, ref int offset)
        {
            if (!char.IsDigit(Peek(code, offset)))
                return null;

            var count = 0;
            while (char.IsDigit(Peek(code, offset + count)))
                count++;
            if (count == 0)
                return null;

            // Detect 1e7.
            if (Peek(code, offset + count) == 'e' || Peek(code, offset + count) == 'E')
            {
                count++;

                if (Peek(code, offset + count) == '-')
                    count++;

                count++;
                while (char.IsDigit(Peek(code, offset + count)))
                    count++;

                var floatAsString = Peek(code, offset, count);
                if (double.TryParse(floatAsString, out _))
                {
                    offset += count;
                    return new FloatToken(floatAsString);
                }
            }

            return new IntToken { Content = Read(code, ref offset, count) };
        }

        public void MakeNegative() => Content = $"-{Content.TrimStart('-')}";
        public void MakePositive() => Content = $"{Content.TrimStart('-')}";

        public bool IsOne() => long.Parse(Content) == 1;
        public bool IsZero() => long.Parse(Content) == 0;

        public IToken Simplify()
        {
            var isNegative = Content.StartsWith("-");
            Content = Content.TrimStart('-');

            // Trim unnecessary leading zeros.
            // Note: A single leading zero represents an octal number!
            while (Content.Length > 2 && Content.StartsWith("00") && char.IsNumber(Content[2]))
                Content = Content.Substring(1);

            if (Content == "00")
                Content = "0";

            if (isNegative)
                Content = $"-{Content}";

            return this;
        }
    }
}