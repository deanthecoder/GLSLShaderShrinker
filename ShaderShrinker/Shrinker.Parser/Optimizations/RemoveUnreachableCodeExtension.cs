// -----------------------------------------------------------------------
//  <copyright file="RemoveUnreachableCodeExtension.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
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
    public static class RemoveUnreachableCodeExtension
    {
        public static void RemoveUnreachableCode(this SyntaxNode rootNode)
        {
            // Remove unreachable code after a 'return' statement.
            foreach (var returnNode in rootNode.TheTree
                .OfType<ReturnSyntaxNode>()
                .Where(o => !o.HasAncestor<SwitchSyntaxNode>() && !o.IsWithinIfPragma())
                .Where(o => o.Next?.Next != null)
                .ToList())
            {
                // Note: returnNode.Next should be a ';', which we need to keep.
                while (returnNode.Next.Next != null)
                    returnNode.Next.Next.Remove();
            }
        }
    }
}