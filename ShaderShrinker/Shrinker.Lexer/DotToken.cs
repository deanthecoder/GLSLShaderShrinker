// -----------------------------------------------------------------------
//  <copyright file="DotToken.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Shrinker.Lexer
{
    public class DotToken : Token
    {
        public override IToken TryMatch(string code, ref int offset) =>
            Peek(code, offset) == '.' ? new DotToken { Content = Read(code, ref offset) } : null;

        public override IToken TryJoin(List<IToken> tokens, int tokenIndex, out int deletePrevious, out int deleteTotal)
        {
            base.TryJoin(tokens, tokenIndex, out deletePrevious, out deleteTotal);

            if (tokenIndex > 0 && tokens[tokenIndex - 1] is IntToken)
            {
                deletePrevious = 1;
                tokenIndex--;
            }

            var newContent = new StringBuilder();
            while (tokenIndex + deleteTotal < tokens.Count && (tokens[tokenIndex + deleteTotal] is IntToken || tokens[tokenIndex + deleteTotal] is DotToken))
            {
                newContent.Append(tokens[tokenIndex + deleteTotal].Content);
                deleteTotal++;
            }

            // Support floats appended with 'f'.
            if (tokenIndex + deleteTotal < tokens.Count && tokens[tokenIndex + deleteTotal].Content.Equals("f", StringComparison.OrdinalIgnoreCase))
            {
                newContent.Append(tokens[tokenIndex + deleteTotal].Content);
                deleteTotal++;
            }

            return deleteTotal <= 1 ? null : new FloatToken(newContent.ToString());
        }
    }
}