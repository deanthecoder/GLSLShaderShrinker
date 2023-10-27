// -----------------------------------------------------------------------
//  <copyright file="StructDefinitionSyntaxNode.cs">
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
    public class StructDefinitionSyntaxNode : SyntaxNode
    {
        public string Name => Children[0].Token.Content;
        public BraceSyntaxNode Braces => (BraceSyntaxNode)Children[1];

        public StructDefinitionSyntaxNode(GenericSyntaxNode nameNode, BraceSyntaxNode braceNode)
        {
            if (nameNode == null)
                throw new ArgumentNullException(nameof(nameNode));
            if (braceNode == null)
                throw new ArgumentNullException(nameof(braceNode));

            Adopt(nameNode, braceNode);
        }

        private StructDefinitionSyntaxNode()
        {
        }

        public override string UiName => $"struct {Name} {{...}}";

        protected override SyntaxNode CreateSelf() => new StructDefinitionSyntaxNode();

        /// <summary>
        /// Append a coded string representing a suitable C# constructor.
        /// </summary>
        public static void WriteConstructor(StringBuilder sb, StructDefinitionSyntaxNode o)
        {
            var declarations = o.Braces.Children.OfType<VariableDeclarationSyntaxNode>().ToList();
            var fields = new List<(TypeToken VariableType, string Name)>();
            foreach (var decl in declarations)
                fields.AddRange(decl.Definitions.Select(def => (decl.VariableType, def.Name)));

            sb.Append($"public {o.Name}(");
            sb.Append(string.Join(", ", fields.Select(field => $"{field.VariableType.Content} {field.Name}_")));
            sb.AppendLine(") {");
            foreach (var field in fields)
            {
                if (field.VariableType.IsVector() || field.VariableType.IsMatrix() || field.VariableType.IsStruct())
                    sb.AppendLine($"{field.Name} = Clone(ref {field.Name}_);");
                else
                    sb.AppendLine($"{field.Name} = {field.Name}_;");
            }
            
            sb.AppendLine("}");

            declarations.ForEach(decl => sb.AppendLine($"public {decl.UiName}"));
        }
    }
}