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
        /// <summary>
        /// Merge variable assignment with single use in an return statement on the next line.
        /// </summary>
        public static void CombineAssignmentWithReturn(this SyntaxNode rootNode)
        {
            foreach (var functionNode in rootNode.FindFunctionDefinitions())
            {
                foreach (var braces in functionNode.TheTree.OfType<BraceSyntaxNode>().ToList())
                {
                    while (true)
                    {
                        var didChange = false;

                        foreach (var localVariable in functionNode.LocalVariables())
                        {
                            // Variable used in return statement?
                            var returnNode = braces.FindLastChild<ReturnSyntaxNode>();
                            if (returnNode == null)
                                continue; // ...no. Ignore...

                            var usagesWithSuffix = returnNode
                                .TheTree
                                .OfType<GenericSyntaxNode>()
                                .Where(o => o.StartsWithVarName(localVariable.Name) && !o.IsVarName(localVariable.Name));
                            if (usagesWithSuffix.Any())
                                continue; // There is a use, but it has an array accessor. Ignore...

                            var usages = returnNode
                                .TheTree
                                .OfType<GenericSyntaxNode>()
                                .Where(o => o.IsVarName(localVariable.Name))
                                .ToList();
                            if (usages.Count != 1)
                                continue; // Used more than once.  Ignore...

                            // Find the last assignment of the variable.
                            var assignment = braces
                                .FindLastChild<VariableAssignmentSyntaxNode>(o => o.Name.StartsWithVarName(localVariable.Name));
                            if (assignment != null && assignment.FullName != localVariable.Name)
                                continue; // Assignment has '.' suffix part, or array accessor.

                            assignment ??= localVariable;

                            // If defined in a variable declaration, bail if it's not the last one.
                            var assignmentDecl = assignment.Parent as VariableDeclarationSyntaxNode;
                            if (assignmentDecl != null && assignmentDecl.Definitions.Last() != assignment)
                                continue;

                            // Are we assigning from a non-const global variable?
                            // Bad idea - It might be modified by a function call in the 'return'.
                            var functionsInReturn = returnNode.TheTree.OfType<FunctionCallSyntaxNode>().ToList();
                            if (functionsInReturn.Any() ||
                                assignment.TheTree.OfType<FunctionCallSyntaxNode>().Any())
                            {
                                var globals = rootNode.FindGlobalVariables()
                                    .Where(o => (o.Parent as VariableDeclarationSyntaxNode)?.VariableType.IsConst != true)
                                    .Select(o => o.Name)
                                    .ToList();
                                var assignmentUsesGlobals = globals.Any(g => assignment.TheTree.Any(o => o.Token?.Content?.StartsWithVarName(g) == true));
                                if (assignmentUsesGlobals)
                                    continue;
                                var returnUsesGlobals = globals.Any(g => returnNode.TheTree.Any(o => o.Token?.Content?.StartsWithVarName(g) == true));
                                if (returnUsesGlobals)
                                    continue;
                            }

                            // Any usages between the assignment and return?
                            var assignmentLine = assignment.Parent is VariableDeclarationSyntaxNode ? assignment.Parent : assignment;
                            var middleNodes = assignmentLine.TakeSiblingsWhile(o => o != returnNode)
                                .SelectMany(o => o.TheTree)
                                .Distinct();
                            if (middleNodes.Any())
                                continue;

                            // If variable is passed into a function with an 'out' param, skip it...
                            if (functionsInReturn.Any())
                            {
                                var allFunctions = rootNode.Root().Children.OfType<FunctionDefinitionSyntaxNode>();
                                var calledFunctionNames = functionsInReturn.Select(o => o.Name).ToList();
                                var calledFunctions = allFunctions.Where(o => calledFunctionNames.Contains(o.Name));
                                if (calledFunctions.Any(o => o.HasOutParam))
                                    continue;
                            }

                            // Inline the definition (adding (...) if necessary).
                            var addBrackets = assignment.Children.Any(o => o.Token is SymbolOperatorToken);
                            if (addBrackets)
                                usages.Single().ReplaceWith(new RoundBracketSyntaxNode(assignment.ValueNodes));
                            else
                                usages.Single().ReplaceWith(assignment.ValueNodes.ToArray());

                            assignment.Remove();
                            if (assignmentDecl?.Children.Any() == false)
                                assignmentDecl.Remove(); // Declaration is now empty - Remove it.

                            didChange = true;

                            if (addBrackets)
                            {
                                var customOptions = CustomOptions.None();
                                customOptions.SimplifyArithmetic = true;
                                braces.Simplify(customOptions);
                            }

                            {
                                var customOptions = CustomOptions.None();
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