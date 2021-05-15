// -----------------------------------------------------------------------
//  <copyright file="VariableAssignmentSyntaxNode.cs">
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
    public class VariableAssignmentSyntaxNode : SyntaxNode
    {
        public string Name { get; }

        /// <summary>
        /// The variable name, including any array [].
        /// </summary>
        public string FullName => IsArray ? $"{Name}{Children[0].ToCode()}" : Name;

        public VariableAssignmentSyntaxNode(string nameNode, List<SyntaxNode> valueNodes = null) : this(new GenericSyntaxNode(nameNode), valueNodes)
        {
        }

        public VariableAssignmentSyntaxNode(GenericSyntaxNode nameNode, List<SyntaxNode> valueNodes = null)
        {
            Name = ((AlphaNumToken)nameNode?.Token)?.Content ?? throw new ArgumentNullException(nameof(nameNode));

            if (valueNodes != null)
                Adopt(valueNodes.ToArray());

            // Include the array brackets []?
            if (nameNode.NextNonComment is SquareBracketSyntaxNode)
                InsertChild(0, nameNode.NextNonComment);
        }

        public override string UiName => $"{FullName} = {(ValueNodes.Count() == 1 ? ValueNodes.Single().UiName : "<Children>")}";

        protected override SyntaxNode CreateSelf() => new VariableAssignmentSyntaxNode(Name);

        public bool IsArray => Children.FirstOrDefault() is SquareBracketSyntaxNode;
        public IEnumerable<SyntaxNode> ValueNodes => IsArray ? Children.Skip(1) : Children;
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
    }
}