// -----------------------------------------------------------------------
//  <copyright file="SingleLineCommentToken.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
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
    public class SingleLineCommentToken : CommentTokenBase
    {
        public override IToken TryMatch(string code, ref int offset)
        {
            if (Peek(code, offset, 2) != "//")
                return null;

            var i = offset - 1;
            var isAppendedToLine = false;
            while (i >= 0)
            {
                if (Peek(code, i).IsNewline())
                    break;
                if (!char.IsWhiteSpace(Peek(code, i)))
                {
                    isAppendedToLine = true;
                    break;
                }

                i--;
            }

            return new SingleLineCommentToken { Content = Read(code, ref offset, 2), IsAppendedToLine = isAppendedToLine };
        }

        public override IToken TryJoin(List<IToken> tokens, int tokenIndex, out int deletePrevious, out int deleteTotal)
        {
            base.TryJoin(tokens, tokenIndex, out deletePrevious, out deleteTotal);

            var newContent = new StringBuilder();
            while (tokenIndex + deleteTotal < tokens.Count && !(tokens[tokenIndex + deleteTotal] is LineEndToken))
            {
                newContent.Append(tokens[tokenIndex + deleteTotal].Content);
                deleteTotal++;
            }

            return deleteTotal > 1 ? new SingleLineCommentToken { Content = newContent.ToString(), IsAppendedToLine = IsAppendedToLine } : null;
        }

        public override IToken Clone() => new SingleLineCommentToken { Content = Content };
    }
}