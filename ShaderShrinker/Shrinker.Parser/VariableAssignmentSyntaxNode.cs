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

namespace Shrinker.Parser
{
    public class VariableAssignmentSyntaxNode : SyntaxNode
    {
        public string Name { get; }

        public VariableAssignmentSyntaxNode(string nameNode, List<SyntaxNode> valueNodes = null) : this(new GenericSyntaxNode(nameNode), valueNodes)
        {
        }

        public VariableAssignmentSyntaxNode(GenericSyntaxNode nameNode, List<SyntaxNode> valueNodes = null)
        {
            Name = ((AlphaNumToken)nameNode?.Token)?.Content ?? throw new ArgumentNullException(nameof(nameNode));

            if (valueNodes != null)
                Adopt(valueNodes.ToArray());
        }

        public override string UiName => Children.Any() ? $"{Name} = {(Children.Count == 1 ? Children.Single().UiName : "<Children>")}" : Name;

        public bool HasValue => Children.Any();

        /// <summary>
        /// Determine whether the assignment is 'simple'.
        /// I.e. A constant expression, not relying on any other function call or variable.
        /// E.g. v = vec2(1.2, 3.4 + 5.6);
        /// </summary>
        public bool IsSimpleAssignment()
        {
            return HasValue &&
                   TheTree
                       .Skip(1) // Skip 'this' node itself.
                       .All(
                            o => o is RoundBracketSyntaxNode ||
                                 o.Token is SymbolOperatorToken ||
                                 o.Token is CommaToken ||
                                 o.Token is INumberToken ||
                                 o.Token is TypeToken);
        }
    }
}