// -----------------------------------------------------------------------
//  <copyright file="GroupVariableDeclarationsExtension.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Linq;
using Shrinker.Parser.SyntaxNodes;

namespace Shrinker.Parser.Optimizations
{
    public static class GroupVariableDeclarationsExtension
    {
        public static void GroupVariableDeclarations(this SyntaxNode rootNode)
        {
            rootNode.WalkTree(
                              node =>
                              {
                                  if (!node.Children.OfType<VariableDeclarationSyntaxNode>().Any())
                                      return true;

                                  if (node.HasAncestor<PragmaIfSyntaxNode>())
                                      return true;

                                  if (!node.HasAncestor<StructDefinitionSyntaxNode>())
                                  {
                                      // Float all const declarations to the top.
                                      var constDecls =
                                          node.Children.OfType<VariableDeclarationSyntaxNode>()
                                              .Where(o => o.VariableType.IsConst && !o.IsWithinIfPragma())
                                              .Reverse()
                                              .ToList();
                                      var firstNonCommentLine = 0;
                                      if (node == rootNode && constDecls.Any())
                                      {
                                          // Const is defined at the global scope - Move after any file header comments.
                                          while (firstNonCommentLine < node.Children.Count && node.Children[firstNonCommentLine].IsComment())
                                              firstNonCommentLine++;
                                      }

                                      constDecls.ForEach(o => node.InsertChild(firstNonCommentLine, o));

                                      // Groups of non-const definitions can be placed together.
                                      var i = 0;
                                      while (i < node.Children.Count)
                                      {
                                          // Find a declaration.
                                          var decl = node.Children
                                              .Skip(i)
                                              .OfType<VariableDeclarationSyntaxNode>()
                                              .FirstOrDefault(o => !o.IsWithinIfPragma());
                                          if (decl == null)
                                              break;

                                          // Find following declarations which have the same type.
                                          var similarDecls =
                                              decl.NextSiblings
                                                  .OfType<VariableDeclarationSyntaxNode>()
                                                  .Where(o => !o.IsWithinIfPragma())
                                                  .Where(o => o.VariableType.Content == decl.VariableType.Content)
                                                  .ToList();

                                          // Append the variable names to the first declaration variable list.
                                          var declNames = similarDecls.SelectMany(o => o.Definitions).Select(o => o.Name).ToList();
                                          declNames.ForEach(o => decl.Adopt(new VariableAssignmentSyntaxNode(o)));

                                          // ...and make the assignment stand on its own (outside of its declaration).
                                          similarDecls.SelectMany(o => o.Definitions).Where(o => !o.HasValue).ToList().ForEach(o => o.Remove());
                                          similarDecls.ForEach(o => o.ReplaceWith(o.Definitions));

                                          i = decl.NodeIndex + 1;
                                      }
                                  }
                                  else
                                  {
                                      // Combine consecutive struct fields of the same type.
                                      var n = node.Children.FirstOrDefault();
                                      while (n != null)
                                      {
                                          if (n is VariableDeclarationSyntaxNode decl1 &&
                                              n.Next is VariableDeclarationSyntaxNode decl2 &&
                                              decl1.VariableType.Content == decl2.VariableType.Content)
                                          {
                                              decl1.Adopt(decl2.Definitions.Cast<SyntaxNode>().ToArray());
                                              decl2.Remove();
                                              continue;
                                          }

                                          n = n.Next;
                                      }
                                  }

                                  return true;
                              });
        }
    }
}