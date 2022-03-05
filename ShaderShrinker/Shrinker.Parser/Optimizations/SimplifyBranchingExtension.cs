// -----------------------------------------------------------------------
//  <copyright file="SimplifyBranchingExtension.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
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
using Shrinker.Parser.SyntaxNodes;

namespace Shrinker.Parser.Optimizations
{
    public static class SimplifyBranchingExtension
    {
        public static void SimplifyBranching(this SyntaxNode rootNode)
        {
            // Remove 'else' keyword if the 'true' branch terminates with return/break/continue.
            foreach (var ifElseNode in rootNode.TheTree
                .OfType<IfSyntaxNode>()
                .Where(
                       o => o.FalseBranch != null &&
                            (o.TrueBranch.Children.OfType<ReturnSyntaxNode>().Any() ||
                             o.TrueBranch.FindLastChild(n => (n as GenericSyntaxNode)?.Token?.IsAnyOf("break", "continue") == true) != null))
                .ToList())
            {
                var falseBranch = ifElseNode.FalseBranch;

                // Don't inline if 'false' branch declares variables. (As their scope would change.)
                if (falseBranch.Children.OfType<VariableDeclarationSyntaxNode>().Any())
                    continue;

                var replacements = new List<SyntaxNode> { new IfSyntaxNode(ifElseNode.Conditions, ifElseNode.TrueBranch, null) };
                replacements.AddRange(falseBranch.Children);
                ifElseNode.ReplaceWith(replacements);
            }
        }
    }
}