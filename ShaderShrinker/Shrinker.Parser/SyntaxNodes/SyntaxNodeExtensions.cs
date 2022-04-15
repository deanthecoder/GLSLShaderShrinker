// -----------------------------------------------------------------------
//  <copyright file="SyntaxNodeExtensions.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
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

        public static IEnumerable<SyntaxNode> Ancestors(this SyntaxNode node)
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

        public static T FindLastChild<T>(this SyntaxNode node, Func<T, bool> isMatchFunc = null) => isMatchFunc != null ? node.Children.OfType<T>().LastOrDefault(isMatchFunc) : node.Children.OfType<T>().LastOrDefault();

        public static SyntaxNode FindLastChild(this SyntaxNode node, Func<SyntaxNode, bool> isMatchFunc) => node.FindLastChild<SyntaxNode>(isMatchFunc);

        public static IEnumerable<SyntaxNode> TakeSiblings(this SyntaxNode node, int count) => node.TakeSiblingsWhile(_ => true).Take(count);

        public static IEnumerable<SyntaxNode> TakeSiblingsWhile(this SyntaxNode node, Func<SyntaxNode, bool> isMatchFunc)
        {
            while (node.Next != null && isMatchFunc(node.Next))
            {
                yield return node.Next;
                node = node.Next;
            }
        }

        public static IEnumerable<FunctionDefinitionSyntaxNode> FunctionDefinitions(this SyntaxNode node) =>
            node.Children.OfType<FunctionDefinitionSyntaxNode>();

        public static IEnumerable<FunctionDeclarationSyntaxNode> FunctionDeclarations(this SyntaxNode node) =>
            node.Children.OfType<FunctionDeclarationSyntaxNode>();

        public static IEnumerable<FunctionCallSyntaxNode> FunctionCalls(this SyntaxNode node) =>
            node.TheTree.OfType<FunctionCallSyntaxNode>();

        public static SyntaxNode Root(this SyntaxNode node)
        {
            while (node.Parent != null)
                node = node.Parent;
            return node;
        }

        public static IEnumerable<VariableAssignmentSyntaxNode> GlobalVariables(this SyntaxNode node) =>
            node.Root().Children.OfType<VariableDeclarationSyntaxNode>().SelectMany(o => o.Definitions);

        public static bool IsComment(this SyntaxNode node) => node is CommentSyntaxNodeBase || node.Token is CommentTokenBase;

        public static bool HasNodeContent(this SyntaxNode node, string s) => node?.Token?.Content == s;

        public static IEnumerable<SyntaxNode> ToCsv(this IEnumerable<SyntaxNode> values)
        {
            var nodes = values.ToList();
            foreach (var node in nodes)
            {
                yield return node;
                if (node != nodes.Last())
                    yield return new GenericSyntaxNode(new CommaToken());
            }
        }

        public static bool HasEntryPointFunction(this SyntaxNode rootNode) =>
            rootNode
                .Root()
                .FunctionDefinitions()
                .Select(o => o.Name)
                .Any(o => o.StartsWith("main"));

        /// <summary>
        /// Find all function/variable/#define names declared in the code.
        /// </summary>
        public static IEnumerable<string> FindUserDefinedNames(this SyntaxNode root)
        {
            var nonUniformGlobals = root.GlobalVariables().Where(o => (o.Parent as VariableDeclarationSyntaxNode)?.VariableType.IsUniform != true);
            var names = nonUniformGlobals.Select(o => o.Name).ToList();

            var functionDefinitions = root.FunctionDefinitions().ToList();
            names.AddRange(functionDefinitions.Select(o => o.Name));

            var functionParams = functionDefinitions.SelectMany(o => o.ParamNames).Select(o => o.Name);
            names.AddRange(functionParams);

            var functionVariables = functionDefinitions.SelectMany(o => o.LocalVariables().Select(v => v.Name));
            names.AddRange(functionVariables);
            names.AddRange(root.TheTree.OfType<PragmaDefineSyntaxNode>().Select(o => o.Name));

            // Skip non-user-defined names.
            return names.Where(o => !o.IsAnyOf("main", "mainImage", "mainSound", "mainVR")).Distinct().OrderBy(o => o);
        }
    }
}