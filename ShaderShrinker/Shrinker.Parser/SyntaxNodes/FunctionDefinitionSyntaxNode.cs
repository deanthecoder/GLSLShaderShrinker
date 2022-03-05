// -----------------------------------------------------------------------
//  <copyright file="FunctionDefinitionSyntaxNode.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using Shrinker.Lexer;

namespace Shrinker.Parser.SyntaxNodes
{
    public class FunctionDefinitionSyntaxNode : FunctionSyntaxNodeBase
    {
        public BraceSyntaxNode Braces => (BraceSyntaxNode)Children[2];

        public FunctionDefinitionSyntaxNode(GenericSyntaxNode typeNode, GenericSyntaxNode nameNode, RoundBracketSyntaxNode paramsNode, BraceSyntaxNode braceNode)
        {
            if (nameNode == null)
                throw new ArgumentNullException(nameof(nameNode));
            if (paramsNode == null)
                throw new ArgumentNullException(nameof(paramsNode));
            if (braceNode == null)
                throw new ArgumentNullException(nameof(braceNode));

            ReturnType = typeNode?.Token?.Content ?? throw new ArgumentNullException(nameof(typeNode));
            if (typeNode.Children.FirstOrDefault() is SquareBracketSyntaxNode arrayNode)
                ReturnType += arrayNode.ToCode();

            Adopt(nameNode, paramsNode, braceNode);
        }

        private FunctionDefinitionSyntaxNode()
        {
        }

        public override string UiName => $"{ReturnType} {Name}{(Params.Children.Any() ? "(...)" : "()")} {{...}}";

        protected override SyntaxNode CreateSelf() => new FunctionDefinitionSyntaxNode { ReturnType = ReturnType };

        public bool ModifiesGlobalVariables()
        {
            var globals = this.GlobalVariables().Select(o => o.Name).ToList();
            var theTree = Braces.TheTree.ToList();

            // Check self.
            if (theTree.Any(o => globals.Any(g => o.Token?.Content?.StartsWithVarName(g) == true)))
                return true;

            // Check any calls made by self...
            return this.FunctionCalls().Any(o => o.ModifiesGlobalVariables());
        }

        /// <summary>
        /// Whether the function uses iTime, iResolution, etc.
        /// </summary>
        public bool UsesGlslInputs() => TheTree.Select(o => o?.Token?.Content).Intersect(TypeToken.GlslInputs).Any();

        public bool DoesCall(FunctionDefinitionSyntaxNode otherFunction) =>
            this.FunctionCalls().Any(o => o.Name == otherFunction.Name);

        public bool CallsLocalFunctions() =>
            this.FunctionCalls().Any(o => o.GetCallee() != null);

        /// <summary>
        /// How many times does this function call another?
        /// </summary>
        public int CallCount(FunctionDefinitionSyntaxNode otherFunction) =>
            this.FunctionCalls().Count(o => o.Name == otherFunction.Name);

        /// <summary>
        /// Return the corresponding function definition (if it has been explicitly added to the code).
        /// </summary>
        public FunctionDeclarationSyntaxNode GetDeclaration() => this.Root().FunctionDeclarations().FirstOrDefault(o => o.Name == Name && o.Params.GetCsv().Count() == Params.GetCsv().Count());
    }
}