// -----------------------------------------------------------------------
//  <copyright file="CombineAssignmentWithReturnExtension.cs">
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
    public static class CombineAssignmentWithReturnExtension
    {
        public static void CombineAssignmentWithReturn(this SyntaxNode rootNode)
        {
            // Merge variable assignment with single use in return statement.
            foreach (var functionNode in rootNode.FindFunctionDefinitions())
            {
                foreach (var braces in functionNode.TheTree.OfType<BraceSyntaxNode>().ToList())
                {
                    while (true)
                    {
                        var didChange = false;

                        foreach (var localVariable in functionNode.LocalVariables())
                        {
                            // Single use in return statement?
                            var returnNode = braces.FindLastChild<ReturnSyntaxNode>();
                            if (returnNode == null)
                                continue;

                            var usagesWithSuffix = returnNode
                                .TheTree
                                .OfType<GenericSyntaxNode>()
                                .Where(o => o.StartsWithVarName(localVariable.Name) && !o.IsVarName(localVariable.Name));
                            if (usagesWithSuffix.Any())
                                continue;

                            var usages = returnNode
                                .TheTree
                                .OfType<GenericSyntaxNode>()
                                .Where(o => o.IsVarName(localVariable.Name))
                                .ToList();
                            if (usages.Count != 1)
                                continue;

                            // Find the last assignment of the variable.
                            var assignment = braces
                                .FindLastChild<VariableAssignmentSyntaxNode>(o => o.Name.StartsWithVarName(localVariable.Name));
                            if (assignment != null && assignment.Name != localVariable.Name)
                                continue; // Assignment has '.' suffix part.

                            assignment ??= localVariable;

                            // If defined in a variable declaration, bail if it's not the last one.
                            var assignmentDecl = assignment.Parent as VariableDeclarationSyntaxNode;
                            if (assignmentDecl != null && assignmentDecl.Definitions.Last() != assignment)
                                continue;

                            // Are we assigning from a non-const global variable?
                            // Bad idea - It might be modified by a function call in the 'return'.
                            var globals = rootNode.FindGlobalVariables()
                                .Where(o => (o.Parent as VariableDeclarationSyntaxNode)?.VariableType.IsConst != true)
                                .Select(o => o.Name);
                            if (globals.Any(g => assignment.TheTree.Any(o => o.Token?.Content?.StartsWithVarName(g) == true)))
                                continue;

                            // Any usages between the two locations?
                            var assignmentLine = assignment.Parent is VariableDeclarationSyntaxNode ? assignment.Parent : assignment;
                            var middleNodes = assignmentLine.TakeSiblingsWhile(o => o != returnNode)
                                .SelectMany(o => o.TheTree)
                                .Distinct();
                            if (middleNodes.Any())
                                continue;

                            // Inline the definition (adding (...) if necessary).
                            var addBrackets = assignment.Children.Any(o => o.Token is SymbolOperatorToken);
                            if (addBrackets)
                                usages.Single().ReplaceWith(new RoundBracketSyntaxNode(assignment.Children));
                            else
                                usages.Single().ReplaceWith(assignment.Children.ToArray());

                            assignment.Remove();
                            if (assignmentDecl?.Children.Any() == false)
                                assignmentDecl.Remove(); // Declaration is now empty - Remove it.

                            didChange = true;

                            if (addBrackets)
                            {
                                var customOptions = CustomOptions.Disabled();
                                customOptions.SimplifyArithmetic = true;
                                braces.Simplify(customOptions);
                            }

                            {
                                var customOptions = CustomOptions.Disabled();
                                customOptions.RemoveUnusedVariables = true;
                                braces.Simplify(customOptions);
                            }
                        }

                        if (!didChange)
                            break;
                    }
                }
            }
        }
    }
}