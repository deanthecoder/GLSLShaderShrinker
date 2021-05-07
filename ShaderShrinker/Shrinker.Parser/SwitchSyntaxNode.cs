// -----------------------------------------------------------------------
//  <copyright file="SwitchSyntaxNode.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System;

namespace Shrinker.Parser
{
    public class SwitchSyntaxNode : SyntaxNode, IStatementSyntaxNode
    {
        public RoundBracketSyntaxNode Condition => (RoundBracketSyntaxNode)Children[0];
        public BraceSyntaxNode Cases => (BraceSyntaxNode)Children[1];

        public SwitchSyntaxNode(RoundBracketSyntaxNode condition, BraceSyntaxNode cases)
        {
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));
            if (cases == null)
                throw new ArgumentNullException(nameof(cases));

            Adopt(condition, cases);
        }

        public override string UiName => "switch (...) { case: ... }";
    }
}