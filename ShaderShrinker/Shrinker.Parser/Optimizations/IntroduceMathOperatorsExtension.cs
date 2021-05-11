// -----------------------------------------------------------------------
//  <copyright file="IntroduceMathOperatorsExtension.cs">
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
using Shrinker.Parser.SyntaxNodes;

namespace Shrinker.Parser.Optimizations
{
    public static class IntroduceMathOperatorsExtension
    {
        /// <summary>
        /// Returns true if all optimizations should be re-run.
        /// </summary>
        public static bool IntroduceMathOperators(this SyntaxNode rootNode)
        {
            var repeatSimplifications = false;

            // Join simple arithmetic into +=, -=, /=, *=.
            foreach (var functionContent in rootNode.FindFunctionDefinitions().Select(o => o.Braces))
            {
                while (true)
                {
                    var didChange = false;

                    foreach (var assignment in functionContent.TheTree
                        .OfType<VariableAssignmentSyntaxNode>()
                        .Where(o => o.TheTree.OfType<GenericSyntaxNode>().Any(n => n.HasNodeContent(o.Name))))
                    {
                        var rhs = assignment.Children[0].IsOnlyChild && assignment.Children.Single() is RoundBracketSyntaxNode bracketNode ? bracketNode.Children : assignment.Children;

                        // Must be at least three nodes (Variable name, math op, and more).
                        if (rhs.Count < 3)
                            continue;

                        // First expression on rhs must be the variable name.
                        if ((rhs[0] as GenericSyntaxNode)?.Token.Content.StartsWithVarName(assignment.Name) != true)
                            continue;

                        // Second expression must be a math operator.
                        var symbolType = rhs[1].Token.GetMathSymbolType();
                        if (symbolType == TokenExtensions.MathSymbolType.Unknown)
                            continue;

                        // All maths ops should be the same type.
                        if (symbolType != TokenExtensions.MathSymbolType.AddSubtract)
                        {
                            var symbolTypes = assignment.TheTree
                                .Where(o => o.Token is SymbolOperatorToken)
                                .Select(o => o.Token.GetMathSymbolType()).Distinct();
                            if (symbolTypes.Count() != 1)
                                continue;
                        }

                        // Replace with +=, -=, etc.
                        var newNodes = new List<SyntaxNode>
                        {
                            new GenericSyntaxNode(assignment.Name),
                            new GenericSyntaxNode(new SymbolOperatorToken($"{rhs[1].Token.Content}="))
                        };

                        var newRhs = rhs.Skip(2).ToList();
                        while (newRhs.Count == 1 && newRhs[0] is RoundBracketSyntaxNode && newRhs[0].Children.Any())
                            newRhs = newRhs[0].Children.ToList();
                        newNodes.AddRange(newRhs);
                        newNodes.Add(new GenericSyntaxNode(new SemicolonToken()));
                        assignment.ReplaceWith(newNodes);

                        repeatSimplifications = didChange = true;
                    }

                    if (!didChange)
                        break;
                }
            }

            return repeatSimplifications;
        }
    }
}