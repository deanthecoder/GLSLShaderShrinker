// -----------------------------------------------------------------------
//  <copyright file="BraceSyntaxNode.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Linq;
using Shrinker.Lexer;

namespace Shrinker.Parser.SyntaxNodes
{
    public class BraceSyntaxNode : GroupSyntaxNode
    {
        public override string UiName => "{...}";

        public override SyntaxNode Adopt(params SyntaxNode[] nodes)
        {
            // Replace comma-separated statements with semi-colons.
            var toAdopt = nodes.ToArray();
            for (var i = 0; i < toAdopt.Length; i++)
            {
                if (toAdopt[i].Token is not CommaToken)
                    continue;

                toAdopt[i].Remove();
                toAdopt[i] = new GenericSyntaxNode(new SemicolonToken());
            }

            return base.Adopt(toAdopt);
        }
    }
}