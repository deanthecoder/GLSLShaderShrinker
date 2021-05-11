// -----------------------------------------------------------------------
//  <copyright file="IfSyntaxNode.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System;

namespace Shrinker.Parser.SyntaxNodes
{
    public class IfSyntaxNode : SyntaxNode, IStatementSyntaxNode
    {
        public RoundBracketSyntaxNode Conditions => (RoundBracketSyntaxNode)Children[0];
        public BraceSyntaxNode TrueBranch => (BraceSyntaxNode)Children[1];
        public BraceSyntaxNode FalseBranch => Children.Count > 2 ? (BraceSyntaxNode)Children[2] : null;

        public IfSyntaxNode(RoundBracketSyntaxNode conditions, BraceSyntaxNode trueBranch, BraceSyntaxNode falseBranch)
        {
            if (conditions == null)
                throw new ArgumentNullException(nameof(conditions));
            if (trueBranch == null)
                throw new ArgumentNullException(nameof(trueBranch));

            Adopt(conditions, trueBranch);

            if (falseBranch != null)
                Adopt(falseBranch);
        }

        private IfSyntaxNode()
        {
        }

        public override string UiName => FalseBranch == null ? "if (...) { ... }" : "if (...) { ... } else { ... }";

        protected override SyntaxNode CreateSelf() => new IfSyntaxNode();
    }
}