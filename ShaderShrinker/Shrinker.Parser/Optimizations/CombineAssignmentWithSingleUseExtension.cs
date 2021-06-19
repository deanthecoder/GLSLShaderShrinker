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
        public static bool CombineAssignmentWithSingleUse(this SyntaxNode rootNode)
        {
            var anyChanges = false;
            var functionNodes = rootNode.FunctionDefinitions().ToList();
            foreach (var functionNode in functionNodes)
            {
                while (true)
                {
                    var didChange = false;

                    var localVariables = functionNode.LocalVariables().ToList();
                    var localVariablesNames = localVariables.Select(o => o.Name).ToList();
                    var assignments = functionNode.TheTree.OfType<VariableAssignmentSyntaxNode>()
                        .Where(o => o.HasValue && // Exclude declarations of the variable name.
                                    localVariablesNames.Contains(o.Name)) // Variable must be declared locally.
                        .ToList();
                    foreach (var assignment in assignments)
                    {
                        if (assignment.IsArray)
                            continue;

                        // Find the declaration matching the variable being assigned to.
                        var variableDecl = assignment.GetDeclaration();
                        if (variableDecl == null)
                            continue;

                        // Variable must only ever be used once after the assignment.
                        var scopedNodesAfterAssignment = 
                            assignment
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
                        if (n is VariableAssignmentSyntaxNode nextAssignment)
                        {
                            if (!TryCombineWithNextAssignment(variableDecl, assignment, nextAssignment))
                                continue;
                        }
                        else if (n is IfSyntaxNode ifNode)
                        {
                            if (!TryCombineWithNextIf(variableDecl, assignment, ifNode))
                                continue;
                        }
                        else
                        {
                            // No change made.
                            continue;
                        }

                        // If the declaration isn't declaring any variables any more, remove it.
                        if (declParent != null && !declParent.Children.Any())
                            declParent.Remove();

                        didChange = true;
                        anyChanges = true;
                    }

                    if (!didChange)
                        break;
                }
            }

            return anyChanges;
        }

        private static bool TryCombineWithNextAssignment(VariableDeclarationSyntaxNode variableDecl, VariableAssignmentSyntaxNode assignment, VariableAssignmentSyntaxNode nextAssignment)
        {
            // The next assignment must be for a different variable.
            if (nextAssignment.Name == assignment.Name)
                return false;

            // The assignment must happen in the same scope as the variable declaration.
            if (variableDecl.FindAncestor<BraceSyntaxNode>() != nextAssignment.FindAncestor<BraceSyntaxNode>())
                return false;

            // ...and must use the variable exactly once...
            var nextAssignmentUsesOfVar =
                nextAssignment
                    .TheTree
                    .OfType<GenericSyntaxNode>()
                    .Where(o => o.IsVarName(assignment.Name))
                    .ToList();
            if (nextAssignmentUsesOfVar.Count != 1)
                return false;

            var usage = nextAssignmentUsesOfVar.Single();

            // Don't join if the next assignment uses a function call.
            // (Just in case it modifies the variable.)
            var intermediateNodes = nextAssignment.TheTree.TakeWhile(o => o != usage);
            var hasFunctionCall = intermediateNodes.OfType<FunctionCallSyntaxNode>().Any(o => o.HasOutParam);
            if (hasFunctionCall)
                return false;

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
            return true;
        }

        private static bool TryCombineWithNextIf(VariableDeclarationSyntaxNode variableDecl, VariableAssignmentSyntaxNode assignment, IfSyntaxNode ifNode)
        {
            var conditionTree = ifNode.Conditions.TheTree;

            // The 'if' condition must use the variable exactly once.
            var usages = conditionTree.OfType<GenericSyntaxNode>().Where(o => o.IsVarName(assignment.Name)).ToList();
            if (usages.Count != 1)
                return false;

            // ...and cannot use it with an array index or vector field.
            if (conditionTree.OfType<GenericSyntaxNode>().Count(o => o.StartsWithVarName(assignment.Name)) != 1)
                return false;

            // Don't join if the 'if' condition uses a function call.
            // (Just in case it modifies the variable.)
            var hasFunctionCall = conditionTree.OfType<FunctionCallSyntaxNode>().Any(o => o.HasOutParam);
            if (hasFunctionCall)
                return false;

            // Inline the variable!
            usages.Single().ReplaceWith(assignment.Children.ToArray());
            assignment.Remove();

            // Remove the declaration too?
            var assDef = variableDecl.Definitions.FirstOrDefault(o => o.Name == assignment.Name);
            if (assDef?.FindDeclarationScope().OfType<GenericSyntaxNode>().Any(o => o.StartsWithVarName(assignment.Name)) == false)
                assDef.Remove();

            if (!variableDecl.Children.Any())
                variableDecl.Remove();

            return true;
        }
    }
}