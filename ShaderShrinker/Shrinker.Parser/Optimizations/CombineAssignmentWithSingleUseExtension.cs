// -----------------------------------------------------------------------
//  <copyright file="CombineAssignmentWithSingleUseExtension.cs">
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
    public static class CombineAssignmentWithSingleUseExtension
    {
        /// <summary>
        /// Merge variable assignment with single use in an assignment on the next line.
        /// </summary>
        public static void CombineAssignmentWithSingleUse(this SyntaxNode rootNode)
        {
            var functionNodes = rootNode.FindFunctionDefinitions().ToList();
            var functionNames = functionNodes.Select(o => o.Name).ToList();
            foreach (var functionNode in functionNodes)
            {
                while (true)
                {
                    var didChange = false;

                    var localVariables = functionNode.LocalVariables().ToList();
                    var localVariablesNames = localVariables.Select(o => o.Name).ToList(); // todo - inline
                    var assignments = functionNode.TheTree.OfType<VariableAssignmentSyntaxNode>()
                        .Where(o => o.HasValue && // Exclude declarations of the variable name.
                                    localVariablesNames.Contains(o.Name)) // Variable must be declared locally.
                        .ToList();
                    foreach (var assignment in assignments)
                    {
                        // Find the declaration matching the variable being assigned to.
                        var variableDecl = localVariables.LastOrDefault(o => o.FindDeclarationScope().Contains(assignment));

                        // Variable must only ever be used once after the assignment.
                        var scopedNodesAfterAssignment = 
                            variableDecl
                                .FindDeclarationScope() // Get the entire scope of the variable.
                                .SkipWhile(o => o != assignment) // ...starting from the RHS of the assignment.
                                .Skip(1)
                                .SkipWhile(o => o.HasAncestor(assignment)) // ...and skipping to the next line.
                                .ToList();
                        if (scopedNodesAfterAssignment.OfType<GenericSyntaxNode>().Count(o => o.StartsWithVarName(assignment.Name)) != 1)
                            continue;
                        if (scopedNodesAfterAssignment.OfType<GenericSyntaxNode>().Count(o => o.IsVarName(assignment.Name)) != 1)
                            continue;

                        // Find 'next' node.
                        var declParent = assignment.Parent as VariableDeclarationSyntaxNode;
                        var n = assignment.NextNonComment;
                        if (n == null && declParent != null)
                            n = declParent.NextNonComment;

                        // If next operation isn't an assignment, ignore...
                        var nextAssignment = n as VariableAssignmentSyntaxNode;
                        if (nextAssignment == null)
                            continue;

                        // The next assignment must be for a different variable.
                        if (nextAssignment.Name == assignment.Name)
                            continue;

                        // ...and must use the variable exactly once...
                        var nextAssignmentUsesOfVar =
                            nextAssignment
                                .TheTree
                                .OfType<GenericSyntaxNode>()
                                .Where(o => o.IsVarName(assignment.Name))
                                .ToList();
                        if (nextAssignmentUsesOfVar.Count != 1)
                            continue;

                        var usage = nextAssignmentUsesOfVar.Single();

                        // Don't join if the next assignment uses a function call.
                        // (Just in case it modifies the variable somehow.)
                        var intermediateNodes = nextAssignment.TheTree.TakeWhile(o => o != usage);
                        var hasFunctionCall = intermediateNodes.OfType<GenericSyntaxNode>().Any(o => functionNames.Contains(o.Token?.Content));
                        if (hasFunctionCall)
                            continue;

                        // Inline the variable!
                        var addBrackets = assignment.Children.Any(o => o.Token is SymbolOperatorToken);
                        if (addBrackets)
                        {
                            usage.ReplaceWith(new RoundBracketSyntaxNode(assignment.Children));

                            // Try to remove the brackets if we can.
                            var customOptions = CustomOptions.None();
                            customOptions.SimplifyArithmetic = true;
                            nextAssignment.Simplify(customOptions);
                        }
                        else
                        {
                            usage.ReplaceWith(assignment.Children.ToArray());
                        }

                        assignment.Remove();

                        // If the declaration isn't declaring any variables any more, remove it.
                        if (declParent != null && !declParent.Children.Any())
                            declParent.Remove();

                        didChange = true;
                    }

                    if (!didChange)
                        break;
                }
            }
        }
    }
}