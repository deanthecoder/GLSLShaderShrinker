// -----------------------------------------------------------------------
//  <copyright file="ConstToken.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace Shrinker.Lexer
{
    public class ConstToken : Token
    {
        public override IToken TryMatch(string code, ref int offset) =>
            IsMatch(code, offset) ? new ConstToken { Content = Read(code, ref offset, 5) } : null;

        private static bool IsMatch(string code, int offset) =>
            Peek(code, offset, 5) == "const" && !char.IsLetterOrDigit(Peek(code, offset + 5));

        public override IToken TryJoin(List<IToken> tokens, int tokenIndex, out int deletePrevious, out int deleteTotal)
        {
            base.TryJoin(tokens, tokenIndex, out deletePrevious, out deleteTotal);

            var count = 1;
            for (; tokenIndex + count < tokens.Count; count++)
            {
                if (tokens[tokenIndex + count] is WhitespaceToken)
                    continue;
                if (tokens[tokenIndex + count] is TypeToken)
                    break;

                return null;
            }

            if (tokenIndex + count == tokens.Count)
                return null;

            ((TypeToken)tokens[tokenIndex + count]).IsConst = true;

            deleteTotal = count + 1;
            return tokens[tokenIndex + count];
        }

        public override IToken Clone() => new ConstToken { Content = Content };
    }
}