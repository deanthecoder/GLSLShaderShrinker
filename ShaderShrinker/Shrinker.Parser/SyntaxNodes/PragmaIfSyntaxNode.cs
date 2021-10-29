// -----------------------------------------------------------------------
//  <copyright file="PragmaIfSyntaxNode.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using System.Text;
using Shrinker.Lexer;

namespace Shrinker.Parser.SyntaxNodes
{
    public class PragmaIfSyntaxNode : SyntaxNode
    {
        private readonly SyntaxNode m_elseNode;
        private readonly SyntaxNode m_endNode;

        public string Name { get; }

        public bool HasFalseBranch => m_elseNode != null;

        public PragmaIfSyntaxNode(SyntaxNode startNode, SyntaxNode elseNode, SyntaxNode endNode)
        {
            if (startNode == null)
                throw new ArgumentNullException(nameof(startNode));
            m_elseNode = elseNode;
            m_endNode = endNode ?? throw new ArgumentNullException(nameof(endNode));

            // Read (and discard) '#if' arguments to get name.
            var ifName = new StringBuilder();
            OutputFormatter.AppendCode(ifName, startNode);

            var ifArgs = startNode.TakeSiblingsWhile(o => o.Token is not LineEndToken).ToList();
            ifArgs.ForEach(o => OutputFormatter.AppendCode(ifName, o));
            ifArgs.ForEach(o => o.Remove());

            Name = ifName.ToString().Trim();

            if (!Name.StartsWith("#"))
                throw new ArgumentException("Name must start with #if...");
        }

        public override string UiName => Name;

        protected override SyntaxNode CreateSelf() =>
            throw new InvalidOperationException();

        public bool Is0() => Name.StartsWith("#if 0");
        public bool Is1() => Name.StartsWith("#if 1");

        public static bool IsStart(SyntaxNode node) => (node?.Token as PreprocessorToken)?.IsAnyOf("#if", "#ifdef", "#ifndef") == true;
        public static bool IsElse(SyntaxNode node) => (node?.Token as PreprocessorToken)?.Content == "#else";
        public static bool IsEnd(SyntaxNode node) => (node?.Token as PreprocessorToken)?.Content == "#endif";

        public static bool ContainsNode(SyntaxNode node)
        {
            var theTree = node.Root().TheTree;
            var nodeIndex = theTree.IndexOf(node);
            if (nodeIndex == -1)
                throw new ArgumentException("Node not found in tree.");

            return
                theTree
                    .OfType<PragmaIfSyntaxNode>()
                    .Where(candidateIf => theTree.IndexOf(candidateIf) <= nodeIndex)
                    .Any(candidateIf => theTree.IndexOf(candidateIf.m_endNode) >= nodeIndex);
        }

        public void ReplaceWithTrueBranch()
        {
            // Remove the '#if' line.
            var toRemove = SelfAndNextSiblings.TakeWhile(o => o.Token is not LineEndToken).ToList();
            toRemove.Add(toRemove.Last().Next); // Include the newline.

            if (HasFalseBranch)
            {
                // Remove '#else' to just before the '#endif'.
                toRemove.AddRange(m_elseNode.SelfAndNextSiblings.SelectMany(o => o.TheTree).TakeWhile(o => o != m_endNode));
            }

            // Remove the '#endif'.
            toRemove.Add(m_endNode);

            // Do it.
            toRemove.ForEach(o => o.Remove());
        }

        public void ReplaceWithFalseBranch()
        {
            if (!HasFalseBranch)
                throw new InvalidOperationException("There is no 'false' branch to remove.");

            // Remove '#if' to just before the '#else'.
            var toRemove = SelfAndNextSiblings.SelectMany(o => o.TheTree).TakeWhile(o => o != m_elseNode).ToList();

            // Remove the '#else'.
            toRemove.Add(m_elseNode);

            // Remove the '#endif'.
            toRemove.Add(m_endNode);

            // Do it.
            toRemove.ForEach(o => o.Remove());
        }

        public void RemoveAll()
        {
            // Remove '#if' to just before the '#endif'.
            var toRemove = SelfAndNextSiblings.SelectMany(o => o.TheTree).TakeWhile(o => o != m_endNode).ToList();

            // Remove the '#endif'.
            toRemove.Add(m_endNode);

            // Do it.
            toRemove.ForEach(o => o.Remove());
        }
    }
}