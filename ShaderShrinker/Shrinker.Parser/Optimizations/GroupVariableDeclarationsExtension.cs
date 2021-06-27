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
        /// <summary>
        /// Turns "int a; int b;" into "int a, b;".
        /// </summary>
        public static void GroupVariableDeclarations(this SyntaxNode rootNode)
        {
            var functionNames = rootNode.Root().FunctionDefinitions().Select(o => o.Name).ToList();

            rootNode.WalkTree(
                              node =>
                              {
                                  if (!node.Children.OfType<VariableDeclarationSyntaxNode>().Any())
                                      return true;

                                  if (PragmaIfSyntaxNode.ContainsNode(node))
                                      return true;

                                  if (!node.HasAncestor<StructDefinitionSyntaxNode>())
                                  {
                                      // Float all const declarations to the top.
                                      var constDecls =
                                          node.Children.OfType<VariableDeclarationSyntaxNode>()
                                              .Where(o => o.VariableType.IsConst && !o.IsWithinIfPragma() && o.VariableType.IsGlslType)
                                              .Reverse()
                                              .ToList();
                                      var insertionLineIndex = 0;
                                      if (node == rootNode && constDecls.Any())
                                      {
                                          // Const is defined at the global scope - Move after any file header comments and/or #defines.
                                          while (insertionLineIndex < node.Children.Count && (node.Children[insertionLineIndex].IsComment() || node.Children[insertionLineIndex] is PragmaDefineSyntaxNode))
                                              insertionLineIndex++;
                                      }

                                      constDecls.ForEach(o => node.InsertChild(insertionLineIndex, o));

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
                                                  .Where(o => o.IsSameType(decl))
                                                  .ToList();

                                          // If the declaration is assigning a value which matches a function name, ignore it.
                                          // (Splitting the declaration and assignment will cause a compiler error.)
                                          similarDecls = similarDecls
                                              .Where(d => !functionNames.Any(d.IsDeclared))
                                              .ToList();

                                          // Append the variables to the first declaration variable list.
                                          var assignments =
                                              similarDecls
                                                  .SelectMany(o => o.Definitions)
                                                  .ToList();
                                          if (assignments.Any())
                                          {
                                              if (decl.VariableType.IsConst)
                                              {
                                                  // Const assignments move to the target location.
                                                  decl.Adopt(assignments.OfType<SyntaxNode>().ToArray());
                                                  similarDecls.ForEach(o => o.Remove());
                                              }
                                              else
                                              {
                                                  // The declarations are moved to the target location, but assignments left where they are.
                                                  assignments.ForEach(
                                                                      o =>
                                                                      {
                                                                          var newNode = new VariableAssignmentSyntaxNode(o.Name);
                                                                          if (o.IsArray)
                                                                              newNode.Adopt(o.Children.First().Clone());
                                                                          decl.Adopt(newNode);
                                                                      });

                                                  // ...and make the assignment stand on its own (outside of its declaration).
                                                  similarDecls.SelectMany(o => o.Definitions).Where(o => !o.HasValue).ToList().ForEach(o => o.Remove());
                                                  similarDecls.ForEach(o => o.ReplaceWith(o.Definitions));
                                              }
                                          }

                                          i = decl.NodeIndex + 1;
                                      }

                                      return true;
                                  }

                                  // Combine consecutive struct fields of the same type.
                                  var n = node.Children.FirstOrDefault();
                                  while (n != null)
                                  {
                                      if (n is VariableDeclarationSyntaxNode decl1 &&
                                          n.Next is VariableDeclarationSyntaxNode decl2 &&
                                          decl1.IsSameType(decl2))
                                      {
                                          decl1.Adopt(decl2.Definitions.Cast<SyntaxNode>().ToArray());
                                          decl2.Remove();
                                          continue;
                                      }

                                      n = n.Next;
                                  }

                                  return true;
                              });
        }
    }
}