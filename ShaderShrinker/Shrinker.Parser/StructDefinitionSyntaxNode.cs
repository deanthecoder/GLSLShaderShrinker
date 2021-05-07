// -----------------------------------------------------------------------
//  <copyright file="StructDefinitionSyntaxNode.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System;
using Shrinker.Lexer;

namespace Shrinker.Parser
{
    public class StructDefinitionSyntaxNode : SyntaxNode
    {
        public string Name { get; }
        public BraceSyntaxNode Braces { get; }

        public StructDefinitionSyntaxNode(GenericSyntaxNode nameNode, BraceSyntaxNode braceNode)
        {
            Name = ((AlphaNumToken)nameNode?.Token)?.Content ?? throw new ArgumentNullException(nameof(nameNode));
            Braces = braceNode ?? throw new ArgumentNullException(nameof(braceNode));

            Adopt(nameNode, Braces);
        }

        public override string UiName => $"struct {Name} {{...}}";
    }
}