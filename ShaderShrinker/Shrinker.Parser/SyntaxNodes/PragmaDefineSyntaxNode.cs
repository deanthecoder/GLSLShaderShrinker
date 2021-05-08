// -----------------------------------------------------------------------
//  <copyright file="PragmaDefineSyntaxNode.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Shrinker.Lexer;

namespace Shrinker.Parser.SyntaxNodes
{
    public class PragmaDefineSyntaxNode : SyntaxNode
    {
        public string Name { get; }
        public RoundBracketSyntaxNode Params { get; }
        public IList<SyntaxNode> ValueNodes { get; }

        public PragmaDefineSyntaxNode(GenericSyntaxNode nameNode, RoundBracketSyntaxNode paramsNode = null, IList<SyntaxNode> valueNodes = null)
        {
            Name = ((AlphaNumToken)nameNode?.Token)?.Content.Trim() ?? throw new ArgumentNullException(nameof(nameNode));
            Params = paramsNode;
            ValueNodes = valueNodes;

            Adopt(nameNode);

            if (Params != null)
                Adopt(Params);

            if (ValueNodes != null)
                Adopt(valueNodes.ToArray());
        }

        public override string UiName =>
            $"#define {Name}{(Params?.Children.Any() == true ? "(...)" : string.Empty)} {(ValueNodes?.Any() == true ? "..." : string.Empty)}";
    }
}