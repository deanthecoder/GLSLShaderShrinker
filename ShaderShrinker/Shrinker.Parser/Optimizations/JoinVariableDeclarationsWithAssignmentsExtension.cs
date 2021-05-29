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
using Shrinker.Lexer;
using Shrinker.Parser.SyntaxNodes;

namespace Shrinker.Parser.Optimizations
{
    /// <summary>
    /// Take something like "int a; a = 3;" and join to make "int a = 3;".
    /// </summary>
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
                                                  defn = n.NextSiblings.OfType<VariableAssignmentSyntaxNode>().FirstOrDefault(o => declVarNames.Contains(o.Name));
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
                                                          decl.IsDeclared(candidateDefn.Name) &&
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
                                              var declDef = decl.Definitions.FirstOrDefault(o => o.FullName == defn.FullName && !o.HasValue);
                                              if (declDef != null)
                                              {
                                                  // If the (non-const) decl variable list has a defined value _after_
                                                  // the name we just found, we can't join them.
                                                  var indexOfDeclName = decl.Definitions.ToList().IndexOf(declDef);
                                                  if (isSimpleAssignment || decl.VariableType.IsConst || !decl.Definitions.Skip(indexOfDeclName + 1).Any(o => o.HasValue))
                                                  {
                                                      // Join them.
                                                      declDef.Adopt(defn.ValueNodes.ToArray());
                                                      defn.Remove();
                                                      continue;
                                                  }
                                              }

                                              // Advance to next node.
                                              n = n.Next;
                                          }
                                      }

                                      // Find any variable declarations which are used later (potentially to move nearer actual the use).
                                      var declWithNoDefs = node.Children
                                          .OfType<VariableDeclarationSyntaxNode>()
                                          .Where(o => !o.IsWithinIfPragma())
                                          .FirstOrDefault(o => o.Definitions.All(d => !d.HasValue));
                                      if (declWithNoDefs == null)
                                          break; // Nope - Give up.

                                      // Yes - Find all uses.
                                      var nextNodesInScope = declWithNoDefs.NextSiblings.SelectMany(o => o.TheTree).ToList();
                                      var unassignedVariableNames = declWithNoDefs.Definitions.Select(o => o.Name).ToList();
                                      var nextAssignments = nextNodesInScope.OfType<VariableAssignmentSyntaxNode>()
                                          .Where(o => unassignedVariableNames.Any(n => o.Name.StartsWithVarName(n)) && o.HasValue);
                                      var nextUses = nextNodesInScope.OfType<FunctionCallSyntaxNode>()
                                          .Where(o => o.Params.TheTree.OfType<GenericSyntaxNode>().Any(n => unassignedVariableNames.Any(n.StartsWithVarName)))
                                          .Cast<SyntaxNode>()
                                          .ToList();
                                      nextUses.AddRange(nextAssignments);

                                      if (!nextUses.Any())
                                          break; // No uses found.

                                      var nearestUseIndex = nextUses.Min(o => nextNodesInScope.IndexOf(o));
                                      var nearestUseNode = nextNodesInScope[nearestUseIndex];
                                      if (!nearestUseNode.IsSiblingOf(declWithNoDefs))
                                      {
                                          // One of the variables is used in a sub-tree of code.
                                          // Moving the declaration would change it's scope...
                                          break;
                                      }

                                      if (declWithNoDefs.NextNonComment == nearestUseNode)
                                          break; // Already next to each other.

                                      // Move declaration to before definition.
                                      declWithNoDefs.Remove();
                                      node.InsertChild(nearestUseNode.NodeIndex, declWithNoDefs);
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
                node = node.NextNonComment;
            }

            return group;
        }
    }
}