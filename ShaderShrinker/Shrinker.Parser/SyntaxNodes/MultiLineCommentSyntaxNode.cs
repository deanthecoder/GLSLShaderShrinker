// -----------------------------------------------------------------------
//  <copyright file="MultiLineCommentSyntaxNode.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using Shrinker.Lexer;

namespace Shrinker.Parser.SyntaxNodes
{
    public class MultiLineCommentSyntaxNode : CommentSyntaxNodeBase
    {
        public MultiLineCommentSyntaxNode(SyntaxNode commentNode) : base(commentNode)
        {
        }

        private MultiLineCommentSyntaxNode(IToken commentToken) : base(commentToken)
        {
        }

        protected override SyntaxNode CreateSelf() => new MultiLineCommentSyntaxNode(Token);
    }
}