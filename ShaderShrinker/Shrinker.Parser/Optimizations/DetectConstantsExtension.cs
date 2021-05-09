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
                foreach (var defCandidate in decl.Definitions.Where(o => o.IsSimpleAssignment()))
                {
                    // Is this variable assigned anywhere else?
                    var isReassigned = decl.Parent.TheTree
                        .OfType<VariableAssignmentSyntaxNode>()
                        .Where(o => o != defCandidate)
                        .Any(o => o.Name == defCandidate.Name);

                    if (!isReassigned)
                    {
                        // Perhaps modified using an operator?
                        var isModified = decl.Parent.TheTree
                            .OfType<GenericSyntaxNode>()
                            .Any(
                                 o => o.Token?.Content.StartsWithVarName(defCandidate.Name) == true &&
                                      SymbolOperatorToken.ModifyingOperator.Contains(o.Next?.Token?.Content));
                        isReassigned = isModified;
                    }

                    if (isReassigned)
                        continue;

                    // Make a const version.
                    var newType = new TypeToken(decl.VariableType.Content)
                    {
                        IsConst = true
                    };
                    var newDecl = new VariableDeclarationSyntaxNode(new GenericSyntaxNode(newType), new GenericSyntaxNode(defCandidate.Name));
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