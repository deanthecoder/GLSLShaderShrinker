// -----------------------------------------------------------------------
//  <copyright file="ForSyntaxNode.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System;

namespace Shrinker.Parser.SyntaxNodes
{
    public class ForSyntaxNode : SyntaxNode, IStatementSyntaxNode
    {
        public RoundBracketSyntaxNode LoopSetup { get; }
        public BraceSyntaxNode LoopCode { get; }

        public ForSyntaxNode(RoundBracketSyntaxNode loopSetup, BraceSyntaxNode loopCode)
        {
            LoopSetup = loopSetup ?? throw new ArgumentNullException(nameof(loopSetup));
            LoopCode = loopCode ?? throw new ArgumentNullException(nameof(loopCode));

            Adopt(loopSetup, loopCode);
        }

        private ForSyntaxNode()
        {
        }

        public override string UiName => "for (...) { ... }";

        protected override SyntaxNode CreateSelf() => new ForSyntaxNode();
    }
}