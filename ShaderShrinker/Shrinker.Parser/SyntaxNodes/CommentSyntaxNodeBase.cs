// -----------------------------------------------------------------------
//  <copyright file="CommentSyntaxNodeBase.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System.Linq;
using Shrinker.Lexer;

namespace Shrinker.Parser.SyntaxNodes
{
    public abstract class CommentSyntaxNodeBase : SyntaxNode
    {
        public string Comment => Token.Content;
        public bool IsAppendedToLine { get; }

        protected CommentSyntaxNodeBase(SyntaxNode commentNode) : this(commentNode.Token)
        {
        }

        protected CommentSyntaxNodeBase(IToken commentToken) : base(commentToken)
        {
            IsAppendedToLine = ((CommentTokenBase)commentToken).IsAppendedToLine;
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