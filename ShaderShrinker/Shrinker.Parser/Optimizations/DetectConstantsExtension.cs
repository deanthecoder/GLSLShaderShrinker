// -----------------------------------------------------------------------
//  <copyright file="DetectConstantsExtension.cs">
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
    public static class DetectConstantsExtension
    {
        public static bool DetectConstants(this SyntaxNode rootNode)
        {
            var repeatSimplifications = false;

            foreach (var decl in rootNode.TheTree
                .OfType<VariableDeclarationSyntaxNode>()
                .Where(o => !o.VariableType.IsConst && !o.VariableType.IsUniform && o.VariableType.IsGlslType))
            {
                // Find all assignments of each declared variable.
                var nodesInScope = decl.Definitions.First().FindDeclarationScope().ToList();
                var assignmentsInScope = nodesInScope.OfType<VariableAssignmentSyntaxNode>().ToList();

                foreach (var defCandidate in assignmentsInScope.Where(o => decl.IsDeclared(o.Name)).ToList())
                {
                    var parentTree = decl.Parent.TheTree;

                    // Is this variable assigned multiple times?
                    var assignmentsOfVar = assignmentsInScope.Where(o => o.HasValue && o.Name.StartsWithVarName(defCandidate.Name)).ToList();
                    if (assignmentsOfVar.Count != 1)
                        continue;

                    // Assignment a non-trivial(/simple/'const-able') value?
                    if (assignmentsOfVar.Any(o => !o.IsSimpleAssignment()))
                        continue;

                    // Perhaps modified using an operator? (E.g. +=, *=, ...)
                    var uses = nodesInScope
                        .Where(o => o.Token?.Content.StartsWithVarName(defCandidate.Name) == true)
                        .ToList();
                    var isModified = uses.Any(o => o.Next?.Token?.IsAnyOf(SymbolOperatorToken.ModifyingOperator) == true);

                    // ...or '++i' ?
                    if (!isModified)
                    {
                        isModified = uses.Any(o => o.Previous?.Token is SymbolOperatorToken opToken &&
                                                  (opToken.Content == "--" || opToken.Content == "++"));
                    }

                    if (isModified)
                        continue;

                    // Used in a 'inout' or 'out' function call?
                    var passedIntoOutParam = parentTree
                        .OfType<FunctionCallSyntaxNode>()
                        .Where(o => o.Params.Children.OfType<GenericSyntaxNode>().Any(p => p.StartsWithVarName(defCandidate.Name)))
                        .Any(o => o.HasOutParam);
                    if (passedIntoOutParam)
                        continue;

                    // Make a const version.
                    var newType = new TypeToken(decl.VariableType.Content)
                    {
                        IsConst = true
                    };
                    var newDecl = new VariableDeclarationSyntaxNode(new GenericSyntaxNode(newType), defCandidate.Name);
                    newDecl.Children.Single().Adopt(defCandidate.Children.ToArray());
                    defCandidate.Remove();

                    decl.Parent.InsertChild(decl.NodeIndex, newDecl);

                    // Remove declaration if has no other content.
                    if (!decl.Definitions.Any())
                        decl.Remove();

                    repeatSimplifications = true;
                }
            }

            return repeatSimplifications;
        }
    }
}