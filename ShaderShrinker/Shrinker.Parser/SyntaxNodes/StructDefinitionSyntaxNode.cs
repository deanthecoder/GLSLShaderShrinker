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

namespace Shrinker.Parser.SyntaxNodes
{
    public class StructDefinitionSyntaxNode : SyntaxNode
    {
        public string Name => Children[0].Token.Content;
        public BraceSyntaxNode Braces => (BraceSyntaxNode)Children[1];

        public StructDefinitionSyntaxNode(GenericSyntaxNode nameNode, BraceSyntaxNode braceNode)
        {
            if (nameNode == null)
                throw new ArgumentNullException(nameof(nameNode));
            if (braceNode == null)
                throw new ArgumentNullException(nameof(braceNode));

            Adopt(nameNode, braceNode);
        }

        private StructDefinitionSyntaxNode()
        {
        }

        public override string UiName => $"struct {Name} {{...}}";

        protected override SyntaxNode CreateSelf() => new StructDefinitionSyntaxNode();
    }
}