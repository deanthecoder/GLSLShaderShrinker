// -----------------------------------------------------------------------
//  <copyright file="RemoveDisabledCodeExtension.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Linq;

namespace Shrinker.Parser.Optimizations
{
    public static class RemoveDisabledCodeExtension
    {
        public static void RemoveDisabledCode(this SyntaxNode rootNode)
        {
            // Remove #if 0...#endif code.
            rootNode.WalkTree(
                              node =>
                              {
                                  var pragmaNodes = node.Children
                                      .OfType<PragmaIfSyntaxNode>()
                                      .Where(o => o.Is0() || o.Is1())
                                      .ToList();
                                  foreach (var pragmaNode in pragmaNodes)
                                  {
                                      if (pragmaNode.Is1())
                                      {
                                          pragmaNode.ReplaceWith(pragmaNode.TrueBranch);
                                          continue;
                                      }

                                      if (pragmaNode.FalseBranch != null)
                                          pragmaNode.ReplaceWith(pragmaNode.FalseBranch);
                                      else
                                          pragmaNode.Remove();
                                  }

                                  return true;
                              });

            // Remove commented-out code.
            rootNode.TheTree
                .OfType<CommentSyntaxNodeBase>()
                .Where(o => o.Comment.Contains(';'))
                .ToList()
                .ForEach(o => o.Remove());
        }
    }
}