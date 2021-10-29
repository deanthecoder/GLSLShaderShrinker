// -----------------------------------------------------------------------
//  <copyright file="FunctionDeclarationSyntaxNode.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.Linq;

namespace Shrinker.Parser.SyntaxNodes
{
    public class FunctionDeclarationSyntaxNode : FunctionSyntaxNodeBase
    {
        public FunctionDeclarationSyntaxNode(GenericSyntaxNode typeNode, GenericSyntaxNode nameNode, RoundBracketSyntaxNode paramsNode)
        {
            if (nameNode == null)
                throw new ArgumentNullException(nameof(nameNode));
            if (paramsNode == null)
                throw new ArgumentNullException(nameof(paramsNode));

            ReturnType = typeNode?.Token?.Content ?? throw new ArgumentNullException(nameof(typeNode));
            if (typeNode.Children.FirstOrDefault() is SquareBracketSyntaxNode arrayNode)
                ReturnType += arrayNode.ToCode();

            Adopt(nameNode, paramsNode);
        }

        private FunctionDeclarationSyntaxNode()
        {
        }

        public override string UiName => $"{ReturnType} {Name}{(Params.Children.Any() ? "(...)" : "()")};";

        protected override SyntaxNode CreateSelf() => new FunctionDeclarationSyntaxNode { ReturnType = ReturnType };
    }
}