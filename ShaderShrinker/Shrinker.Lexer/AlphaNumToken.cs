// -----------------------------------------------------------------------
//  <copyright file="AlphaNumToken.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System.Linq;

namespace Shrinker.Lexer
{
    public class AlphaNumToken : Token
    {
        public AlphaNumToken(string content = null)
        {
            Content = content;
        }

        public override IToken TryMatch(string code, ref int offset)
        {
            if (char.IsDigit(Peek(code, offset)))
                return null; // Can't start with a digit.

            var count = 0;
            char ch;
            while ((ch = Peek(code, offset + count)) >= 128 ||
                   char.IsLetterOrDigit(ch) ||
                   new[] { '_', '©' }.Contains(ch))
                count++;

            if (count == 0)
                return null;

            var s = Peek(code, offset, count);

            if (TypeToken.Names.Contains(s))
                return new TypeToken(Read(code, ref offset, s.Length));

            if (KeywordToken.Names.Contains(s))
                return new KeywordToken(Read(code, ref offset, s.Length));

            return new AlphaNumToken(Read(code, ref offset, s.Length));
        }

        public override IToken Clone() => new AlphaNumToken(Content);
    }
}