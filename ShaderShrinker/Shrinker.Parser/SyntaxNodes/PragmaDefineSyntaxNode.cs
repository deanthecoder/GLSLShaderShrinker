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
        private readonly int m_paramsIndex = -1;
        private readonly int m_valuesIndex = -1;

        public string Name { get; }
        public RoundBracketSyntaxNode Params => m_paramsIndex >= 0 ? (RoundBracketSyntaxNode)Children.Skip(m_paramsIndex).FirstOrDefault() : null;
        public IList<SyntaxNode> ValueNodes => m_valuesIndex >= 0 ? Children.Skip(m_valuesIndex).ToList() : null;

        public PragmaDefineSyntaxNode(GenericSyntaxNode nameNode, RoundBracketSyntaxNode paramsNode = null, IList<SyntaxNode> valueNodes = null)
        {
            Name = ((AlphaNumToken)nameNode?.Token)?.Content.Trim() ?? throw new ArgumentNullException(nameof(nameNode));

            Adopt(nameNode);

            if (paramsNode != null)
            {
                m_paramsIndex = 1;
                Adopt(paramsNode);
            }

            if (valueNodes != null)
            {
                m_valuesIndex = paramsNode == null ? 1 : 2;
                Adopt(valueNodes.ToArray());
            }
        }

        public override string UiName =>
            $"#define {Name}{(Params?.Children.Any() == true ? "(...)" : string.Empty)} {(ValueNodes?.Any() == true ? "..." : string.Empty)}";
    }
}