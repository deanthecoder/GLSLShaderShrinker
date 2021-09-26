// -----------------------------------------------------------------------
//  <copyright file="MultiLineCommentToken.cs">
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
    public class MultiLineCommentToken : CommentTokenBase
    {
        public override IToken TryMatch(string code, ref int offset)
        {
            if (Peek(code, offset, 2) != "/*")
                return null;

            var count = 2;
            while (Peek(code, offset + count) != (char)0 && Peek(code, offset + count, 2) != "*/")
                count++;

            if (Peek(code, offset + count) == (char)0)
                return null; // EOF

            return new MultiLineCommentToken { Content = Read(code, ref offset, count + 2) };
        }

        public override IToken Clone() => new MultiLineCommentToken { Content = Content };
    }
}