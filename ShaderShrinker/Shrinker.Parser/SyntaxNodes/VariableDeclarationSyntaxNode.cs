// -----------------------------------------------------------------------
//  <copyright file="VariableDeclarationSyntaxNode.cs">
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
    public class VariableDeclarationSyntaxNode : SyntaxNode
    {
        public TypeToken VariableType { get; private set; }

        public IEnumerable<VariableAssignmentSyntaxNode> Definitions => Children.OfType<VariableAssignmentSyntaxNode>().ToList();

        public VariableDeclarationSyntaxNode(GenericSyntaxNode typeNode, GenericSyntaxNode nameNode = null) : this((TypeToken)typeNode?.Token, nameNode)
        {
        }

        private VariableDeclarationSyntaxNode(TypeToken variableType, GenericSyntaxNode nameNode = null)
        {
            VariableType = variableType ?? throw new ArgumentNullException(nameof(variableType));

            if (nameNode != null)
                Adopt(new VariableAssignmentSyntaxNode(nameNode));
        }

        public override string UiName
        {
            get
            {
                var s = new StringBuilder(VariableType.Content);
                s.Append(' ');

                foreach (var definition in Definitions)
                    s.Append($"{definition.UiName}, ");

                return $"{s.ToString().TrimEnd(' ', ',')};";
            }
        }

        protected override SyntaxNode CreateSelf() => new VariableDeclarationSyntaxNode(VariableType);

        public bool IsDeclared(string variableName) => Definitions.Any(o => o.Name == variableName);

        public bool IsSameType(VariableDeclarationSyntaxNode other) =>
            VariableType.Content == other.VariableType.Content &&
            VariableType.IsConst == other.VariableType.IsConst;

        public void RenameType(string newTypeName) =>
            VariableType = new TypeToken(VariableType.IsConst ? $"const {newTypeName}" : newTypeName);
    }
}