// -----------------------------------------------------------------------
//  <copyright file="RemoveCommentsExtension.cs">
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
    public static class RemoveCommentsExtension
    {
        public static void RemoveComments(this SyntaxNode rootNode, CustomOptions options)
        {
            rootNode.TheTree
                .OfType<CommentSyntaxNodeBase>()
                .Where(o => !options.KeepHeaderComments || !o.IsFileHeaderComment())
                .ToList()
                .ForEach(o => o.Remove());
        }
    }
}