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

namespace Shrinker.Parser
{
    public class FunctionDefinitionSyntaxNode : FunctionSyntaxNodeBase
    {
        public BraceSyntaxNode Braces { get; }

        public FunctionDefinitionSyntaxNode(GenericSyntaxNode typeNode, GenericSyntaxNode nameNode, RoundBracketSyntaxNode paramsNode, BraceSyntaxNode braceNode)
        {
            ReturnType = (TypeToken)typeNode?.Token ?? throw new ArgumentNullException(nameof(typeNode));
            Name = ((AlphaNumToken)nameNode?.Token)?.Content ?? throw new ArgumentNullException(nameof(nameNode));
            Params = paramsNode ?? throw new ArgumentNullException(nameof(paramsNode));
            Braces = braceNode ?? throw new ArgumentNullException(nameof(braceNode));

            Adopt(nameNode, Params, Braces);
        }

        public override string UiName => $"{ReturnType.Content} {Name}{(Params.Children.Any() ? "(...)" : "()")} {{...}}";

        public bool IsMain() => Name.StartsWith("main");
    }
}