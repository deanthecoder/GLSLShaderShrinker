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

using System.Linq;
using Shrinker.Lexer;

namespace Shrinker.Parser.SyntaxNodes
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

        public bool IsFileHeaderComment()
        {
            if (Parent is not FileSyntaxNode root)
                return false;

            var previousSiblings = root.Children.Take(NodeIndex);
            return previousSiblings.All(o => o is CommentSyntaxNodeBase);
        }
    }
}