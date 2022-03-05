// -----------------------------------------------------------------------
//  <copyright file="SingleLineCommentSyntaxNode.cs">
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
    public class SingleLineCommentSyntaxNode : CommentSyntaxNodeBase
    {
        public SingleLineCommentSyntaxNode(SyntaxNode commentNode) : base(commentNode)
        {
        }

        private SingleLineCommentSyntaxNode(IToken commentToken) : base(commentToken)
        {
        }

        protected override SyntaxNode CreateSelf() => new SingleLineCommentSyntaxNode(Token);
    }
}