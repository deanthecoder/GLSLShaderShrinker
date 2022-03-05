// -----------------------------------------------------------------------
//  <copyright file="RemoveDisabledCodeExtension.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System.Linq;
using Shrinker.Parser.SyntaxNodes;

namespace Shrinker.Parser.Optimizations
{
    /// <summary>
    /// E.g. #if 0 and commented-out code.
    /// </summary>
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
                                          pragmaNode.ReplaceWithTrueBranch();
                                          continue;
                                      }

                                      if (pragmaNode.HasFalseBranch)
                                          pragmaNode.ReplaceWithFalseBranch();
                                      else
                                          pragmaNode.RemoveAll();
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