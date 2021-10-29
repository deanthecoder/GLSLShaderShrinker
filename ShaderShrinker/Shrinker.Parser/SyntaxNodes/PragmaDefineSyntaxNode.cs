// -----------------------------------------------------------------------
//  <copyright file="PragmaDefineSyntaxNode.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Shrinker.Lexer;

namespace Shrinker.Parser.SyntaxNodes
{
    public class PragmaDefineSyntaxNode : SyntaxNode
    {
        private bool m_hasParams;
        private bool m_hasValues;

        public string Name => Children[0].Token.Content;
        public RoundBracketSyntaxNode Params => m_hasParams ? (RoundBracketSyntaxNode)Children[1] : null;
        public IList<SyntaxNode> ValueNodes => m_hasValues ? Children.Skip(m_hasParams ? 2 : 1).ToList() : null;

        public PragmaDefineSyntaxNode(GenericSyntaxNode nameNode, RoundBracketSyntaxNode paramsNode = null, IList<SyntaxNode> valueNodes = null) : base((AlphaNumToken)nameNode?.Token)
        {
            Adopt(nameNode);

            if (paramsNode != null)
            {
                m_hasParams = true;
                Adopt(paramsNode);
            }

            if (valueNodes != null)
            {
                m_hasValues = true;
                Adopt(valueNodes.ToArray());
            }
        }

        private PragmaDefineSyntaxNode(IToken nameToken) : base(nameToken)
        {
        }

        public override string UiName =>
            $"#define {Name}{(Params?.Children.Any() == true ? "(...)" : string.Empty)} {(ValueNodes?.Any() == true ? "..." : string.Empty)}";

        protected override SyntaxNode CreateSelf() =>
            new PragmaDefineSyntaxNode(Token)
            {
                m_hasParams = m_hasParams,
                m_hasValues = m_hasValues
            };
    }
}