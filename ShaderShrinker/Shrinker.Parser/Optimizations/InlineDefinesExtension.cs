// -----------------------------------------------------------------------
//  <copyright file="InlineDefinesExtension.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System.Linq;
using Shrinker.Lexer;
using Shrinker.Parser.SyntaxNodes;

namespace Shrinker.Parser.Optimizations
{
    public static class InlineDefinesExtension
    {
        public static void InlineDefines(this SyntaxNode rootNode, CustomOptions customOptions)
        {
            var defineCount = rootNode.TheTree.OfType<PragmaDefineSyntaxNode>().Count();

            while (true)
            {
                InlineDefinesWithSingleNodeRhsValue(rootNode, customOptions);

                if (customOptions?.TranspileToCSharp == true)
                {
                    ApplyDefinesWithNoRhs(rootNode);
                    InlineFunctionLikeDefines(rootNode);
                }
                
                // Keep going until nothing changes.
                var newDefineCount = rootNode.TheTree.OfType<PragmaDefineSyntaxNode>().Count();
                if (newDefineCount == defineCount)
                    break;
                defineCount = newDefineCount;
            }
        }

        private static void InlineFunctionLikeDefines(SyntaxNode rootNode)
        {
            var allDefines = rootNode.TheTree.OfType<PragmaDefineSyntaxNode>();
            var defines =
                allDefines
                    .Where(
                           o => o.Params != null &&
                                o.ValueNodes?.Count == 1 &&
                                o.ValueNodes.Single() is FunctionCallSyntaxNode)
                    .ToList();
            foreach (var define in defines)
            {
                define.Remove();

                var usages = rootNode.TheTree.OfType<GenericSyntaxNode>()
                    .Where(o => o.Name == define.Name && o.Next is RoundBracketSyntaxNode)
                    .ToList();

                foreach (var usage in usages)
                {
                    // Replace the instance args with the defined value args.
                    var originalArgsNode = (RoundBracketSyntaxNode)usage.Next;
                    var originalArgsValues = originalArgsNode.GetCsv().ToList();
                    var newArgsNode = (RoundBracketSyntaxNode)originalArgsNode.ReplaceWith(((FunctionCallSyntaxNode)define.ValueNodes.Single()).Params.Clone());
                    var defineLhsArgNames = define.Params.GetCsv().Select(o => ((GenericSyntaxNode)o.Single()).Name).ToList();
                    foreach (var argToReplace in newArgsNode.GetCsv().Select(o => (GenericSyntaxNode)o.Single()).ToList())
                    {
                        // Get arg name.
                        var argName = argToReplace.Name;

                        // Find where name is in the LHS of the #define expression.
                        var argIndex = defineLhsArgNames.IndexOf(argName);
                        if (argIndex >= 0)
                        {
                            // Replace the placeholder with the value from original code.
                            argToReplace.ReplaceWith(originalArgsValues[argIndex].Select(o => o.Clone()));
                        }
                    }

                    // Replace the name with the defined value.
                    var replacementName = ((FunctionCallSyntaxNode)define.ValueNodes.First()).Name;
                    var newNode = usage.ReplaceWith(new GenericSyntaxNode(replacementName));
                    
                    // If the replacement now looks like a function, treat it as such.
                    TryReplaceNodeWithFunctionCallNode(newNode);
                }
            }
        }

        private static void ApplyDefinesWithNoRhs(SyntaxNode rootNode)
        {
            var definesWithNoRhs =
                rootNode.TheTree.OfType<PragmaDefineSyntaxNode>()
                    .Where(o => o.Params == null && o.ValueNodes?.Any() != true)
                    .ToList();
            foreach (var define in definesWithNoRhs)
            {
                // Convert any #ifdef usages to #if.
                var s = $"#ifdef {define.Name}";
                rootNode.TheTree.OfType<PragmaIfSyntaxNode>()
                    .Where(o => o.Name == s)
                    .ToList()
                    .ForEach(
                             o =>
                             {
                                 o.MakeTrue();
                                 o.Parent.RemoveDisabledCode();
                             });

                define.Remove();
            }
        }

        private static void InlineDefinesWithSingleNodeRhsValue(SyntaxNode rootNode, CustomOptions customOptions)
        {
            var pragmaIfs = rootNode.TheTree.OfType<PragmaIfSyntaxNode>().Select(o => o.Name).ToList();
            var allDefines = rootNode.TheTree.OfType<PragmaDefineSyntaxNode>();
            foreach (var define in allDefines
                         .Where(
                                o => CouldInline(o) &&
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

                if (!customOptions.TranspileToCSharp && countIfRemoved >= countIfKept)
                    continue; // Code length increases - Don't inline.

                define.Remove();
                foreach (var usage in usages)
                {
                    var prevNode = usage.Previous;
                    try
                    {
                        var newContent = define.ValueNodes.Select(o => o.Clone()).ToList();
                        if (usage.Token.Content == define.Name)
                        {
                            usage.ReplaceWith(newContent);
                            continue;
                        }

                        // We have a reference of the form DEFINE.xy, so keep the last '.xy'.
                        var suffix = usage.Token.Content.Substring(define.Name.Length);
                        if (newContent.Last().Token != null)
                        {
                            var newLastContent = newContent.Last().ToCode() + suffix;
                            newContent[newContent.Count - 1] = new GenericSyntaxNode(newLastContent);
                            usage.ReplaceWith(newContent);
                        }
                        else
                        {
                            // Same situation, but last node in the #define is not 'simple'.
                            var prefix = define.ToCode().Split('\t')[1];
                            usage.ReplaceWith(new GenericSyntaxNode(prefix + suffix));
                        }
                    }
                    finally
                    {
                        // If the replacement now looks like a function, treat it as such.
                        TryReplaceNodeWithFunctionCallNode(prevNode?.Next);
                    }
                }
            }
        }

        private static void TryReplaceNodeWithFunctionCallNode(SyntaxNode node)
        {
            if (FunctionCallSyntaxNode.IsNodeFunctionLike(node))
                node.ReplaceWith(new ExternalFunctionCallSyntaxNode((GenericSyntaxNode)node, (RoundBracketSyntaxNode)node.Next));
        }

        private static bool CouldInline(PragmaDefineSyntaxNode o)
        {
            if (o.Params != null)
                return false;

            switch (o.ValueNodes?.Count)
            {
                case 1:
                    return true; // The value is a single node - Definite candidate to inline.

                case 2:
                    // Is this a vector or similar, containing just a comma-separated list of numbers?
                    // E.g. #define V vec3(1, 4, 2)
                    return o.ValueNodes[0].Token is TypeToken t &&
                        t.IsGlslType &&
                        o.ValueNodes[1] is RoundBracketSyntaxNode brackets &&
                        brackets.IsNumericCsv();
            }

            // Nope - Can't inline.
            return false;
        }
    }
}