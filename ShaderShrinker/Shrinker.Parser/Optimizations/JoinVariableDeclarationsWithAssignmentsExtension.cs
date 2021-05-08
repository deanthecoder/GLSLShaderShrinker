// -----------------------------------------------------------------------
//  <copyright file="JoinVariableDeclarationsWithAssignmentsExtension.cs">
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

namespace Shrinker.Parser.Optimizations
{
    public static class JoinVariableDeclarationsWithAssignmentsExtension
    {
        public static void JoinVariableDeclarationsWithAssignments(this SyntaxNode rootNode)
        {
            rootNode.WalkTree(
                              node =>
                              {
                                  while (true)
                                  {
                                      var i = 0;
                                      while (i < node.Children.Count)
                                      {
                                          var n = node.Children[i++];

                                          // Find declaration.
                                          if (n is not VariableDeclarationSyntaxNode decl)
                                              continue;

                                          while (n != null)
                                          {
                                              var isSimpleAssignment = false;
                                              VariableAssignmentSyntaxNode defn;
                                              if (decl.VariableType.IsConst || decl.Parent?.Parent == null)
                                              {
                                                  // Pull in the const definition wherever it is.
                                                  var declVarNames = decl.Definitions.Where(o => !o.HasValue).Select(o => o.Name).ToList();
                                                  defn = node.Children.Skip(n.NodeIndex).OfType<VariableAssignmentSyntaxNode>().FirstOrDefault(o => declVarNames.Contains(o.Name));
                                              }
                                              else
                                              {
                                                  // The definition must closely follow the declaration.
                                                  defn = GetGroupOfType<VariableAssignmentSyntaxNode>(decl.Next).FirstOrDefault();

                                                  // UNLESS the assignment does not depend on anything else!
                                                  if (defn == null)
                                                  {
                                                      var candidateDefn = decl.NextSiblings.OfType<VariableAssignmentSyntaxNode>().FirstOrDefault();
                                                      if (candidateDefn?.HasValue == true &&
                                                          decl.Definitions.Any(o => o.Name == candidateDefn.Name) &&
                                                          candidateDefn.IsSimpleAssignment())
                                                      {
                                                          var nodesInBetween = decl.TakeSiblingsWhile(o => o != candidateDefn);
                                                          if (!nodesInBetween.All(o => o is BraceSyntaxNode || o is IfSyntaxNode || o is SwitchSyntaxNode))
                                                          {
                                                              isSimpleAssignment = true;
                                                              defn = candidateDefn;
                                                          }
                                                      }
                                                  }
                                              }

                                              if (defn == null)
                                                  break;

                                              // Definition found - Does it match declaration?
                                              var declDef = decl.Definitions.FirstOrDefault(o => o.Name == defn.Name && !o.HasValue);
                                              if (declDef != null)
                                              {
                                                  // If the (non-const) decl variable list has a defined value _after_
                                                  // the name we just found, we can't join them.
                                                  var indexOfDeclName = decl.Definitions.ToList().IndexOf(declDef);
                                                  if (isSimpleAssignment || decl.VariableType.IsConst || !decl.Definitions.Skip(indexOfDeclName + 1).Any(o => o.HasValue))
                                                  {
                                                      // Join them.
                                                      declDef.Adopt(defn.Children.ToArray());
                                                      defn.Remove();
                                                      continue;
                                                  }
                                              }

                                              // Advance to next node.
                                              n = n.Next;
                                          }
                                      }

                                      // Move any decls which completely unassigned nearer the definition.
                                      var declWithNoDefs = node.Children
                                          .OfType<VariableDeclarationSyntaxNode>()
                                          .Where(o => !o.IsWithinIfPragma())
                                          .FirstOrDefault(o => o.Definitions.All(d => !d.HasValue));
                                      if (declWithNoDefs == null)
                                          break; // Nope - Give up.

                                      // Yes - Move it to nearer the first suitable definition.
                                      var unassignedVariableNames = declWithNoDefs.Definitions.Select(o => o.Name).ToList();
                                      var nextDefNodes = node.TheTree
                                          .OfType<VariableAssignmentSyntaxNode>()
                                          .Where(o => unassignedVariableNames.Contains(o.Name) && o.HasValue)
                                          .ToList();
                                      if (nextDefNodes.Any(o => !node.Children.Contains(o)))
                                      {
                                          // One of the variables is defined in a sub-tree of code.
                                          // We won't move the declaration...
                                          break;
                                      }

                                      var nearestDefNode = nextDefNodes.OrderBy(o => o.NodeIndex).FirstOrDefault();
                                      if (nearestDefNode == null)
                                          break; // No definition node found - Give up.

                                      // Move declaration to before definition.
                                      declWithNoDefs.Remove();
                                      node.InsertChild(nearestDefNode.NodeIndex, declWithNoDefs);
                                  }

                                  return true;
                              });

            foreach (var declNode in rootNode.TheTree
                .OfType<VariableDeclarationSyntaxNode>()
                .Where(o => o.Definitions.Any(d => d.HasValue) && o.Definitions.Any(d => !d.HasValue))
                .ToList())
            {
                var reorderedDefs = declNode.Definitions.OrderBy(o => o.HasValue ? 1 : 0).Cast<SyntaxNode>().ToArray();
                declNode.Adopt(reorderedDefs);
            }
        }

        /// <summary>
        /// Build a list from consecutive nodes of the specified type (skipping comments).
        /// </summary>
        private static List<T> GetGroupOfType<T>(SyntaxNode node)
        {
            var group = new List<T>();
            while (node is T match)
            {
                group.Add(match);
                node = node.Next;

                while (node is CommentSyntaxNodeBase)
                    node = node.Next;
            }

            return group;
        }
    }
}