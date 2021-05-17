// -----------------------------------------------------------------------
//  <copyright file="SyntaxNodeExtensions.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Shrinker.Lexer;

namespace Shrinker.Parser.SyntaxNodes
{
    public static class SyntaxNodeExtensions
    {
        public static bool HasAncestor<T>(this SyntaxNode node) =>
            node is T || node.Ancestors().OfType<T>().Any();

        public static bool HasAncestor(this SyntaxNode node, SyntaxNode candidate) =>
            node.Ancestors().Any(o => o == candidate);

        public static T FindAncestor<T>(this SyntaxNode node) =>
            node.Ancestors().OfType<T>().FirstOrDefault();

        private static IEnumerable<SyntaxNode> Ancestors(this SyntaxNode node)
        {
            while (node.Parent != null)
            {
                node = node.Parent;
                yield return node;
            }
        }

        public static bool IsOnlyChild(this SyntaxNode node) => node.Parent?.Children.Count == 1;

        public static bool IsSiblingOf(this SyntaxNode node, SyntaxNode candidate) => node.Parent == candidate.Parent;

        public static bool IsWithinIfPragma(this SyntaxNode node) => PragmaIfSyntaxNode.ContainsNode(node);

        public static string ToCode(this SyntaxNode rootNode) => OutputFormatter.ToCode(rootNode);

        public static T FindLastChild<T>(this SyntaxNode node, Func<T, bool> isMatchFunc = null) => isMatchFunc != null ? node.Children.OfType<T>().LastOrDefault(isMatchFunc) : node.Children.OfType<T>().LastOrDefault();

        public static SyntaxNode FindLastChild(this SyntaxNode node, Func<SyntaxNode, bool> isMatchFunc) => node.FindLastChild<SyntaxNode>(isMatchFunc);

        public static IEnumerable<SyntaxNode> TakeSiblings(this SyntaxNode node, int count) => node.TakeSiblingsWhile(o => true).Take(count);

        public static IEnumerable<SyntaxNode> TakeSiblingsWhile(this SyntaxNode node, Func<SyntaxNode, bool> isMatchFunc)
        {
            while (node.Next != null && isMatchFunc(node.Next))
            {
                yield return node.Next;
                node = node.Next;
            }
        }

        public static IEnumerable<FunctionDefinitionSyntaxNode> FindFunctionDefinitions(this SyntaxNode node) =>
            node.Children.OfType<FunctionDefinitionSyntaxNode>();

        public static SyntaxNode Root(this SyntaxNode node)
        {
            while (node.Parent != null)
                node = node.Parent;
            return node;
        }

        public static IEnumerable<VariableAssignmentSyntaxNode> FindGlobalVariables(this SyntaxNode node) =>
            node.Root().Children.OfType<VariableDeclarationSyntaxNode>().SelectMany(o => o.Definitions);

        public static bool IsComment(this SyntaxNode node) => node is CommentSyntaxNodeBase || node.Token is CommentTokenBase;

        public static bool HasNodeContent(this SyntaxNode node, string s) => node?.Token?.Content == s;
    }
}