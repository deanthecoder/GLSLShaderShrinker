// -----------------------------------------------------------------------
//  <copyright file="VariableAssignmentSyntaxNode.cs">
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
using System.Text;
using Shrinker.Lexer;

namespace Shrinker.Parser.SyntaxNodes
{
    public class VariableAssignmentSyntaxNode : SyntaxNode
    {
        public string Name { get; }

        /// <summary>
        /// The variable name, including any array [].
        /// </summary>
        public string FullName
        {
            get
            {
                var sb = new StringBuilder(Name);
                foreach (var squareBrackets in Children.TakeWhile(o => o is SquareBracketSyntaxNode))
                    sb.Append(squareBrackets.ToCode());
                return sb.ToString();
            }
        }

        public VariableAssignmentSyntaxNode(string nameNode, IList<SyntaxNode> valueNodes = null) : this(new GenericSyntaxNode(nameNode), valueNodes)
        {
        }

        public VariableAssignmentSyntaxNode(GenericSyntaxNode nameNode, IList<SyntaxNode> valueNodes = null)
        {
            Name = ((AlphaNumToken)nameNode?.Token)?.Content ?? throw new ArgumentNullException(nameof(nameNode));

            if (valueNodes != null)
                Adopt(valueNodes.ToArray());

            // Include the array brackets []?
            var i = 0;
            while (nameNode.NextNonComment is SquareBracketSyntaxNode)
                InsertChild(i++, nameNode.NextNonComment);
        }

        public override string UiName =>
            !ValueNodes.Any() ? FullName : $"{FullName} = {(ValueNodes.Count() == 1 ? ValueNodes.Single().UiName : "...")}";

        protected override SyntaxNode CreateSelf() => new VariableAssignmentSyntaxNode(Name);

        public bool IsArray => Children.FirstOrDefault() is SquareBracketSyntaxNode;
        public IEnumerable<SyntaxNode> ValueNodes => Children.SkipWhile(o => o is SquareBracketSyntaxNode);
        public bool HasValue => ValueNodes.Any();

        /// <summary>
        /// Determine whether the assignment is 'simple'.
        /// I.e. A constant expression, not relying on any other function call or variable.
        /// E.g. v = vec2(1.2, 3.4 + 5.6);
        /// </summary>
        public bool IsSimpleAssignment()
        {
            if (!HasValue)
                return false;

            if (!IsArray)
                return TheTree
                    .Skip(1) // Skip 'this' node itself.
                    .All(
                         o => o is RoundBracketSyntaxNode ||
                              o.Token is SymbolOperatorToken ||
                              o.Token is CommaToken ||
                              o.Token is INumberToken ||
                              o.Token is TypeToken);

            var arrayValueParent = TheTree.OfType<RoundBracketSyntaxNode>().ToList();
            if (arrayValueParent.Count != 1)
                return false;

            return arrayValueParent
                .Single()
                .TheTree
                .Skip(1)
                .All(
                     o => o.Token is SymbolOperatorToken ||
                          o.Token is CommaToken ||
                          o.Token is INumberToken ||
                          o.Token is TypeToken);
        }

        /// <summary>
        /// Return all the nodes which are within the scope of the declaration of 'this' variable.
        /// </summary>
        public IEnumerable<SyntaxNode> FindDeclarationScope()
        {
            var decl = GetDeclaration();
            if (decl == null)
                yield break;

            // Return all nodes after the assignment _within_ the declaration variable list.
            foreach (var node in decl.Definitions.First(o => o.Name == Name).SelfAndNextSiblings.SelectMany(o => o.TheTree))
                yield return node;

            // Return all the nodes after the declaration node too.
            foreach (var node in decl.NextSiblings.SelectMany(o => o.TheTree))
                yield return node;
        }

        public VariableDeclarationSyntaxNode GetDeclaration() =>
            Parent as VariableDeclarationSyntaxNode ?? this.Root().TheTree.TakeWhile(o => o != this).OfType<VariableDeclarationSyntaxNode>().LastOrDefault(o => o.IsDeclared(Name));
    }
}