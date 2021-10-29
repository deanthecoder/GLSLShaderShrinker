// -----------------------------------------------------------------------
//  <copyright file="PrecisionToken.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Text;

namespace Shrinker.Lexer
{
    public class PrecisionToken : Token
    {
        public override IToken TryMatch(string code, ref int offset) =>
            IsMatch(code, offset) ? new PrecisionToken { Content = Read(code, ref offset, 9) } : null;

        private static bool IsMatch(string code, int offset) =>
            Peek(code, offset, 9) == "precision" && !char.IsLetterOrDigit(Peek(code, offset + 9));

        public override IToken TryJoin(List<IToken> tokens, int tokenIndex, out int deletePrevious, out int deleteTotal)
        {
            base.TryJoin(tokens, tokenIndex, out deletePrevious, out deleteTotal);

            var newContent = new StringBuilder();
            while (tokenIndex + deleteTotal < tokens.Count && !(tokens[tokenIndex + deleteTotal] is SemicolonToken))
            {
                newContent.Append(tokens[tokenIndex + deleteTotal].Content);
                deleteTotal++;
            }

            return deleteTotal > 1 ? new VerbatimToken(newContent.ToString()) : null;
        }

        public override IToken Clone() => new PrecisionToken { Content = Content };
    }
}