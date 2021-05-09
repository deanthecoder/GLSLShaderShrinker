// -----------------------------------------------------------------------
//  <copyright file="RemoveUnusedVariablesExtension.cs">
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
    public static class RemoveUnusedVariablesExtension
    {
        public static void RemoveUnusedVariables(this SyntaxNode rootNode)
        {
            foreach (var decl in rootNode.TheTree
                .OfType<VariableDeclarationSyntaxNode>()
                .Where(
                       o => !o.HasAncestor<StructDefinitionSyntaxNode>() &&
                            (o.VariableType.InOut == TypeToken.InOutType.In ||
                             o.VariableType.InOut == TypeToken.InOutType.None) && // inout/out variables should not be removed.
                            !o.HasAncestor<PragmaIfSyntaxNode>())
                .Where(o => o.FindAncestor<FunctionDefinitionSyntaxNode>()?.Params.Children.Contains(o) != true) // Don't remove function params.
                .ToList())
            {
                foreach (var varName in decl.Definitions.Select(o => o.Name).ToList())
                {
                    // Any references?
                    if (decl.Parent.TheTree
                        .OfType<GenericSyntaxNode>()
                        .Where(o => o.Token?.Content != null)
                        .Any(o => o.StartsWithVarName(varName)))
                        continue; // Variable was used.

                    // Variable not used - Remove any definitions using it.
                    var defs = decl.Parent.TheTree
                        .OfType<VariableAssignmentSyntaxNode>()
                        .Where(o => o.Name == varName).ToList();
                    defs.ForEach(o => o.Remove());

                    if (!decl.Definitions.Any())
                        decl.Remove();
                }
            }
        }
    }
}