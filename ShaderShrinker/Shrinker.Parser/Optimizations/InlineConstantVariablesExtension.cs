﻿// -----------------------------------------------------------------------
//  <copyright file="InlineConstantVariablesExtension.cs">
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
using Shrinker.Parser.SyntaxNodes;

namespace Shrinker.Parser.Optimizations
{
    public static class InlineConstantVariablesExtension
    {
        public static void InlineConstantVariables(this SyntaxNode rootNode)
        {
            // Const variables.
            var consts = rootNode.TheTree
                .OfType<VariableDeclarationSyntaxNode>()
                .Where(o => o.VariableType.IsConst && o.Definitions.Any())
                .ToList();
            foreach (var constDeclNode in consts)
            {
                var potentialUsage = constDeclNode.Parent.TheTree.ToList()
                    .OfType<GenericSyntaxNode>()
                    .Where(o => o.Token is AlphaNumToken)
                    .ToList();
                foreach (var definition in constDeclNode.Definitions.Where(o => o.IsSimpleAssignment()).ToList())
                {
                    var usages = potentialUsage.Where(o => o.Token.Content.StartsWithVarName(definition.Name)).ToList();

                    var anyUnsupportedReferences = usages.Any(o => o.Token.Content.Contains('.') || o.NextNonComment is SquareBracketSyntaxNode);
                    if (anyUnsupportedReferences)
                        continue;

                    if (usages.Count == 1)
                    {
                        // No copying needed - Just move definition to its single use.
                        usages.ForEach(o => o.ReplaceWith(definition.Children.ToList()));
                        definition.Remove();
                        continue;
                    }

                    // Is the const a single expression?
                    var couldInline = definition.TheTree.Count == 2;

                    // Perhaps a simple vector/matrix?
                    couldInline = couldInline ||
                                  definition.ValueNodes.Count() == 2 &&
                                  definition.Children[0]?.Token is TypeToken &&
                                  definition.Children[1] is RoundBracketSyntaxNode brackets &&
                                  brackets.TheTree.Skip(1).All(o => o.Token is INumberToken || o.Token is CommaToken);

                    if (!couldInline)
                        continue;

                    // Is it worth it?
                    var declCodeLength = constDeclNode.VariableType.Content.Length;
                    var costBeforeInlining = declCodeLength + definition.ToCode().Length + usages.Count * definition.Name.Length;
                    var rhsLength = definition.ToCode().Length - definition.Name.Length - 3;
                    var costAfterInlining = usages.Count * rhsLength;
                    if (constDeclNode.Definitions.Count() == 1)
                        costAfterInlining -= declCodeLength + definition.Name.Length;

                    if (costAfterInlining >= costBeforeInlining)
                        continue; // Inlining would increase code size.

                    // Inline.
                    usages.ForEach(o => o.ReplaceWith(definition.Clone().Children.ToList()));
                    definition.Remove();
                }

                if (!constDeclNode.Definitions.Any())
                    constDeclNode.Remove();
            }
        }
    }
}