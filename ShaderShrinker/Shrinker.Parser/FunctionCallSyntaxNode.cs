// -----------------------------------------------------------------------
//  <copyright file="FunctionCallSyntaxNode.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using Shrinker.Lexer;

namespace Shrinker.Parser
{
    public class FunctionCallSyntaxNode : SyntaxNode
    {
        public string Name { get; }
        public RoundBracketSyntaxNode Params { get; }

        public FunctionCallSyntaxNode(GenericSyntaxNode nameNode, RoundBracketSyntaxNode brackets)
        {
            Name = ((AlphaNumToken)nameNode?.Token)?.Content ?? throw new ArgumentNullException(nameof(nameNode));
            Params = brackets ?? throw new ArgumentNullException(nameof(brackets));

            Adopt(brackets);
        }

        public override string UiName => $"{Name}({(Params.Children.Any() ? "..." : string.Empty)})";
    }
}