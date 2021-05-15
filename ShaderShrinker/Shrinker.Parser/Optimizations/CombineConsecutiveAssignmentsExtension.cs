// -----------------------------------------------------------------------
//  <copyright file="CombineConsecutiveAssignmentsExtension.cs">
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
    public static class CombineConsecutiveAssignmentsExtension
    {
        public static void CombineConsecutiveAssignments(this SyntaxNode rootNode)
        {
            // Merge consecutive assignments of the same variable.
            foreach (var functionNode in rootNode.FindFunctionDefinitions())
            {
                foreach (var braces in functionNode.TheTree.OfType<BraceSyntaxNode>().ToList())
                {
                    while (true)
                    {
                        var didChange = false;

                        foreach (var localVariable in functionNode.LocalVariables())
                        {
                            // Find consecutive assignment of same variable.
                            var assignment = braces.Children.OfType<VariableAssignmentSyntaxNode>()
                                .Where(o => o.HasValue && o.Parent is not VariableDeclarationSyntaxNode)
                                .Where(o => o.Next is VariableAssignmentSyntaxNode)
                                .Where(o => o.Name == localVariable.Name && ((VariableAssignmentSyntaxNode)o.Next).Name == localVariable.Name)
                                .FirstOrDefault(o => o.Next.TheTree.OfType<GenericSyntaxNode>().Count(n => n.IsVarName(localVariable.Name)) == 1);

                            if (assignment == null)
                                continue;

                            // Inline the definition (adding (...) if necessary).
                            var usage = assignment.Next.TheTree.OfType<GenericSyntaxNode>().Single(o => o.IsVarName(localVariable.Name));
                            var addBrackets = assignment.Children.Any(o => o.Token is SymbolOperatorToken);
                            if (addBrackets)
                                usage.ReplaceWith(new RoundBracketSyntaxNode(assignment.Children));
                            else
                                usage.ReplaceWith(assignment.Children.ToArray());

                            var nodeAfterAssignment = assignment.Next;
                            assignment.Remove();

                            didChange = true;

                            if (addBrackets && nodeAfterAssignment != null)
                            {
                                var customOptions = CustomOptions.None();
                                customOptions.SimplifyArithmetic = true;
                                nodeAfterAssignment.Simplify(customOptions);
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