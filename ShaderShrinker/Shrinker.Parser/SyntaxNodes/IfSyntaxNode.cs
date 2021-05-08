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
        public RoundBracketSyntaxNode Conditions { get; }
        public BraceSyntaxNode TrueBranch { get; }
        public BraceSyntaxNode FalseBranch { get; }

        public IfSyntaxNode(RoundBracketSyntaxNode conditions, BraceSyntaxNode trueBranch, BraceSyntaxNode falseBranch)
        {
            Conditions = conditions ?? throw new ArgumentNullException(nameof(conditions));
            TrueBranch = trueBranch ?? throw new ArgumentNullException(nameof(trueBranch));
            FalseBranch = falseBranch;

            Adopt(conditions, trueBranch);

            if (falseBranch != null)
                Adopt(falseBranch);
        }

        public override string UiName => FalseBranch == null ? "if (...) { ... }" : "if (...) { ... } else { ... }";
    }
}