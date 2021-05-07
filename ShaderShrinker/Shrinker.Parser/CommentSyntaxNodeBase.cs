// -----------------------------------------------------------------------
//  <copyright file="CommentSyntaxNodeBase.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using Shrinker.Lexer;

namespace Shrinker.Parser
{
    public abstract class CommentSyntaxNodeBase : SyntaxNode
    {
        public string Comment { get; }
        public bool IsAppendedToLine { get; }

        protected CommentSyntaxNodeBase(SyntaxNode commentNode)
        {
            Comment = commentNode.Token.Content;
            IsAppendedToLine = ((CommentTokenBase)commentNode.Token).IsAppendedToLine;
        }

        public override string UiName => Comment;
    }
}