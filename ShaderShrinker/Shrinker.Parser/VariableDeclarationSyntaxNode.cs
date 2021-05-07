// -----------------------------------------------------------------------
//  <copyright file="VariableDeclarationSyntaxNode.cs">
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
using System.Text;
using Shrinker.Lexer;

namespace Shrinker.Parser
{
    public class VariableDeclarationSyntaxNode : SyntaxNode
    {
        public TypeToken VariableType { get; }

        public IEnumerable<VariableAssignmentSyntaxNode> Definitions => Children.OfType<VariableAssignmentSyntaxNode>().ToList();

        public VariableDeclarationSyntaxNode(GenericSyntaxNode typeNode, GenericSyntaxNode nameNode = null)
        {
            VariableType = (TypeToken)typeNode?.Token ?? throw new ArgumentNullException(nameof(typeNode));

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
    }
}