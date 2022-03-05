// -----------------------------------------------------------------------
//  <copyright file="UniformToken.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace Shrinker.Lexer
{
    public class UniformToken : Token
    {
        public override IToken TryMatch(string code, ref int offset) =>
            IsMatch(code, offset) ? new UniformToken { Content = Read(code, ref offset, 7) } : null;

        private static bool IsMatch(string code, int offset) =>
            Peek(code, offset, 7) == "uniform" && !char.IsLetterOrDigit(Peek(code, offset + 7));

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

            ((TypeToken)tokens[tokenIndex + count]).IsUniform = true;

            deleteTotal = count + 1;
            return tokens[tokenIndex + count];
        }

        public override IToken Clone() => new UniformToken { Content = Content };
    }
}