// -----------------------------------------------------------------------
//  <copyright file="SyntaxNode.cs">
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
using System.Diagnostics;
using System.Linq;
using Shrinker.Lexer;

namespace Shrinker.Parser.SyntaxNodes
{
    [DebuggerDisplay("{" + nameof(UiName) + "}")]
    public abstract class SyntaxNode
    {
        protected internal readonly List<SyntaxNode> m_children = new List<SyntaxNode>();
        public virtual string UiName => Token?.Content;

        protected abstract SyntaxNode CreateSelf();

        public SyntaxNode Clone()
        {
            var cloned = CreateSelf();
            cloned.Adopt(m_children.Select(o => o.Clone()).ToArray());
            return cloned;
        }

        protected SyntaxNode(IToken token = null)
        {
            Token = token;
        }

        public SyntaxNode Parent { get; private set; }

        public int NodeIndex => Parent?.m_children.IndexOf(this) ?? -1;

        public SyntaxNode Previous
        {
            get
            {
                var nodeIndex = NodeIndex;
                return nodeIndex >= 1 ? Parent.Children[nodeIndex - 1] : null;
            }
        }

        public SyntaxNode Next => NextSiblings.FirstOrDefault();
        public SyntaxNode NextNonComment => NextSiblings.FirstOrDefault(o => !o.IsComment());

        public IEnumerable<SyntaxNode> SelfAndNextSiblings
        {
            get
            {
                yield return this;

                if (Parent != null)
                {
                    for (var i = NodeIndex + 1; i < Parent.Children.Count; i++)
                        yield return Parent.Children[i];
                }
            }
        }

        public IEnumerable<SyntaxNode> NextSiblings => SelfAndNextSiblings.Skip(1);

        public IToken Token { get; protected set; }

        public IReadOnlyList<SyntaxNode> Children => m_children;

        public void Remove()
        {
            if (Parent == null)
                return;

            if (!Parent.m_children.Remove(this))
                throw new ArgumentException("Unrecognized child.");

            Parent = null;
        }

        protected void AddChild(SyntaxNode node)
        {
            if (node == null)
                return;

            node.Remove();
            node.Parent = this;
            m_children.Add(node);
        }

        private void SetChild(int index, SyntaxNode node)
        {
            node.Remove();
            node.Parent = this;
            if (m_children[index] != null)
                m_children[index].Parent = null;
            m_children[index] = node;
        }

        public void InsertChild(int index, SyntaxNode node)
        {
            m_children.Insert(index, null);
            SetChild(index, node);
        }

        public void InsertNextSibling(SyntaxNode node) => Parent.InsertChild(NodeIndex + 1, node);

        public SyntaxNode ReplaceWith(SyntaxNode node)
        {
            Parent.SetChild(NodeIndex, node);
            return node;
        }

        public void ReplaceWith(IEnumerable<SyntaxNode> nodes)
        {
            var i = 0;
            foreach (var node in nodes)
            {
                Parent.InsertChild(NodeIndex + i + 1, node);
                i++;
            }

            Remove();
        }

        private SyntaxNode Adopt(IEnumerable<SyntaxNode> allNodes, int startIndex, int endIndex)
        {
            allNodes
                .Skip(startIndex).Take(endIndex - startIndex + 1)
                .ToList()
                .ForEach(AddChild);
            return this;
        }

        public virtual SyntaxNode Adopt(params SyntaxNode[] nodes) => Adopt(nodes, 0, nodes.Length - 1);

        protected static BraceSyntaxNode ReadNodesIntoBraceSyntaxNode(ref SyntaxNode node)
        {
            if (node is BraceSyntaxNode braces)
            {
                node = braces.Next;
                return braces;
            }

            var branch = new BraceSyntaxNode();
            if (node is IStatementSyntaxNode)
            {
                branch.Adopt(node);
                node = node.Next;
                return branch;
            }

            // Find terminating semi-colon.
            var peek = node;
            while (peek != null)
            {
                var next = peek.Next;

                if (peek.Token is SemicolonToken
                    || peek is VariableAssignmentSyntaxNode) // Has built-in semicolon.
                {
                    node = next;
                    break;
                }

                branch.Adopt(peek);
                peek = next;
            }

            if (peek != null)
                branch.Adopt(peek);

            return branch;
        }

        public void WalkTree(Func<SyntaxNode, bool> callbackFunc) => WalkTreeImpl(this, callbackFunc);

        private static bool WalkTreeImpl(SyntaxNode rootNode, Func<SyntaxNode, bool> callbackFunc) =>
            rootNode.Children.All(child => WalkTreeImpl(child, callbackFunc)) && callbackFunc(rootNode);

        public IList<SyntaxNode> TheTree
        {
            get
            {
                var all = new List<SyntaxNode> { this };
                foreach (var child in Children)
                {
                    var nodes = child.TheTree.ToList();
                    all.AddRange(nodes);
                }

                return all;
            }
        }

        protected static bool FoldBrackets(string open, string closed, List<SyntaxNode> syntaxNodes, Func<SyntaxNode> createBracketNodeFunc)
        {
            if (!syntaxNodes.Any(o => o.Token is BracketToken))
                return false;

            var didFold = false;

            for (var startIndex = syntaxNodes.Count - 1; startIndex >= 0; startIndex--)
            {
                if ((syntaxNodes[startIndex].Token as BracketToken)?.Content != open)
                    continue;

                // Find matching end token.
                var endIndex = startIndex;
                if (!FindNode(syntaxNodes, ref endIndex, token => (token as BracketToken)?.Content == closed))
                    throw new SyntaxErrorException($"Cannot find matching '{closed}'.");

                // Move all nodes in between to under first node.
                var newParent = createBracketNodeFunc();
                newParent.Parent = syntaxNodes[startIndex].Parent;
                syntaxNodes[startIndex] = newParent;
                syntaxNodes.RemoveAt(endIndex);
                newParent.Adopt(syntaxNodes, startIndex + 1, endIndex - 1);

                didFold = true;
            }

            return didFold;
        }

        private static bool FindNode(IReadOnlyList<SyntaxNode> nodes, ref int i, Func<IToken, bool> isMatchFunc)
        {
            for (; i < nodes.Count; i++)
            {
                if (isMatchFunc(nodes[i].Token))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Detect a series of tokens/syntax nodes (ignoring comments)
        /// </summary>
        protected List<SyntaxNode> TryGetMatchingChildren(int startIndex, params Type[] types)
        {
            var samples = Children
                .Skip(startIndex)
                .Where((o, i) => i == 0 || !o.IsComment())
                .Take(types.Length)
                .ToList();
            if (samples.Count == types.Length &&
                samples.Where((_, i) => samples[i].GetType() == types[i] || samples[i].Token?.GetType() == types[i])
                    .Count() == types.Length)
            {
                return samples;
            }

            return null;
        }

        public List<SyntaxNode> TryGetMatchingSiblings(params Type[] types) => Parent.TryGetMatchingChildren(NodeIndex + 1, types);
   }
}