// -----------------------------------------------------------------------
//  <copyright file="RoundBracketSyntaxNode.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Shrinker.Lexer;

namespace Shrinker.Parser.SyntaxNodes
{
    public class RoundBracketSyntaxNode : GroupSyntaxNode
    {
        public override string UiName => "(...)";

        public RoundBracketSyntaxNode()
        {
        }

        public RoundBracketSyntaxNode(IEnumerable<SyntaxNode> nodes)
            : base(nodes)
        {
        }

        public IEnumerable<IList<SyntaxNode>> GetCsv()
        {
            var group = new List<SyntaxNode>();
            foreach (var node in Children)
            {
                if (node.Token is CommaToken)
                {
                    yield return new List<SyntaxNode>(group);
                    group.Clear();
                }
                else
                {
                    group.Add(node);
                }
            }

            if (group.Any())
                yield return group;
        }

        protected override SyntaxNode CreateSelf() => new RoundBracketSyntaxNode();

        /// <summary>
        /// Check if the brackets contain a simple sequence of numbers. (E.g. (1, 3, 2))
        /// </summary>
        public bool IsSimpleCsv() =>
            TheTree.Skip(1).All(o => o.Token is INumberToken || o.Token is CommaToken);
    }
}