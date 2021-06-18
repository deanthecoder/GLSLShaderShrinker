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
                            !PragmaIfSyntaxNode.ContainsNode(o))
                .Where(o => o.FindAncestor<FunctionDefinitionSyntaxNode>()?.Params.Children.Contains(o) != true) // Don't remove function params.
                .ToList())
            {
                foreach (var varName in decl.Definitions.Select(o => o.Name).ToList())
                {
                    var parentTree = decl.Parent.TheTree;

                    // Any references?
                    if (parentTree
                        .OfType<GenericSyntaxNode>()
                        .Where(o => o.Token?.Content != null)
                        .Any(o => o.StartsWithVarName(varName)))
                        continue; // Variable was used.

                    // Variable not used - Remove any definitions using it.
                    var assignments = parentTree
                        .OfType<VariableAssignmentSyntaxNode>()
                        .Where(o => o.Name == varName).ToList();
                    foreach (var assignment in assignments)
                    {
                        // If RHS of the assignment calls a function with an 'out/inout' param, it is not safe to remove.
                        // Instead, replace the assignment with the RHS component.
                        if (assignment.TheTree.OfType<FunctionCallSyntaxNode>().Any(o => o.HasOutParam || o.ModifiesGlobalVariables()))
                        {
                            var rhs = assignment.ValueNodes.ToList();
                            rhs.Add(new GenericSyntaxNode(new SemicolonToken()));

                            var isInDecl = assignment.Parent is VariableDeclarationSyntaxNode;
                            if (isInDecl)
                            {
                                var isFirstDeclVariable = decl.Definitions.FirstOrDefault()?.Name == varName;
                                if (!isFirstDeclVariable)
                                {
                                    // In the middle of a list of variable declarations - Too tricky to remove.
                                    continue;
                                }

                                // Is the first assignment in a variable declaration - Move RHS to immediately before it.
                                var tempNode = new GenericSyntaxNode("temp");
                                decl.Parent.InsertChild(decl.NodeIndex, tempNode);
                                assignment.Remove();
                                tempNode.ReplaceWith(rhs);
                                continue;
                            }

                            assignment.ReplaceWith(rhs);
                        }
                        else
                        {
                            // ...otherwise we can remove the entire assignment.
                            assignment.Remove();
                        }
                    }

                    if (!decl.Definitions.Any())
                        decl.Remove();
                }
            }
        }
    }
}