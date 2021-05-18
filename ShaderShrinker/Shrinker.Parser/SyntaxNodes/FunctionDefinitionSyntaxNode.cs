// -----------------------------------------------------------------------
//  <copyright file="FunctionDefinitionSyntaxNode.cs">
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

namespace Shrinker.Parser.SyntaxNodes
{
    public class FunctionDefinitionSyntaxNode : FunctionSyntaxNodeBase
    {
        public BraceSyntaxNode Braces => (BraceSyntaxNode)Children[2];

        public FunctionDefinitionSyntaxNode(GenericSyntaxNode typeNode, GenericSyntaxNode nameNode, RoundBracketSyntaxNode paramsNode, BraceSyntaxNode braceNode)
        {
            if (nameNode == null)
                throw new ArgumentNullException(nameof(nameNode));
            if (paramsNode == null)
                throw new ArgumentNullException(nameof(paramsNode));
            if (braceNode == null)
                throw new ArgumentNullException(nameof(braceNode));

            ReturnType = typeNode?.Token?.Content ?? throw new ArgumentNullException(nameof(typeNode));
            if (typeNode.Children.FirstOrDefault() is SquareBracketSyntaxNode arrayNode)
                ReturnType += arrayNode.ToCode();

            Adopt(nameNode, paramsNode, braceNode);
        }

        private FunctionDefinitionSyntaxNode()
        {
        }

        public override string UiName => $"{ReturnType} {Name}{(Params.Children.Any() ? "(...)" : "()")} {{...}}";

        protected override SyntaxNode CreateSelf() => new FunctionDefinitionSyntaxNode { ReturnType = ReturnType };

        public bool IsMain() => Name.StartsWith("main");

        public bool HasOutParam => Params.TheTree.Select(o => o.Token as TypeToken).Any(o => o?.InOut == TypeToken.InOutType.InOut || o?.InOut == TypeToken.InOutType.Out);
    }
}