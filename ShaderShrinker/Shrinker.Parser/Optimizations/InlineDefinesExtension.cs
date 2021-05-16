// -----------------------------------------------------------------------
//  <copyright file="InlineDefinesExtension.cs">
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
    public static class InlineDefinesExtension
    {
        public static void InlineDefines(this SyntaxNode rootNode)
        {
            // #defines.
            var pragmaIfs = rootNode.TheTree.OfType<PragmaIfSyntaxNode>().Select(o => o.Name).ToList();
            foreach (var define in rootNode.TheTree
                .OfType<PragmaDefineSyntaxNode>()
                .Where(
                       o => o.Params == null &&
                            o.ValueNodes?.Count == 1 &&
                            !pragmaIfs.Any(p => p.Contains(o.Name)) &&
                            !PragmaIfSyntaxNode.ContainsNode(o))
                .ToList())
            {
                var usages = rootNode.TheTree
                    .OfType<GenericSyntaxNode>()
                    .Where(o => (o.Token as AlphaNumToken)?.Content.StartsWithVarName(define.Name) == true)
                    .Where(o => o.Parent != define)
                    .ToList();

                var countIfKept = define.ToCode().GetCodeCharCount() + usages.Count * define.Name.Length;
                var countIfRemoved = usages.Count * define.ValueNodes.Sum(o => o.ToCode().Length);

                if (countIfRemoved >= countIfKept)
                    continue; // Code length increases - Don't inline.
                
                define.Remove();
                foreach (var usage in usages)
                {
                    var newContent = define.ValueNodes.Select(o => o.Clone()).ToList();
                    if (usage.Token.Content != define.Name)
                    {
                        // We have a reference of the form DEFINE.xy, so keep the last '.xy'.
                        var newLastContent = newContent.Last().Token.Content + usage.Token.Content.Substring(define.Name.Length);
                        newContent[newContent.Count - 1] = new GenericSyntaxNode(newLastContent);
                    }

                    usage.ReplaceWith(newContent);
                }
            }
        }
    }
}