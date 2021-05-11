// -----------------------------------------------------------------------
//  <copyright file="GenericSyntaxNode.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Linq;
using Shrinker.Lexer;

namespace Shrinker.Parser.SyntaxNodes
{
    public class GenericSyntaxNode : SyntaxNode
    {
        public GenericSyntaxNode(IToken token) : base(token)
        {
        }

        public GenericSyntaxNode(string s) : base(new AlphaNumToken(s))
        {
        }

        /// <summary>
        /// Assuming this is the name of a variable, try to find where it was declared.
        /// </summary>
        public VariableDeclarationSyntaxNode FindVarDeclaration()
        {
            var varName = Token.Content.Split('.').First();
            var node = (SyntaxNode)this;

            while (node != null)
            {
                var decl = node.Parent?.Children
                    .OfType<VariableDeclarationSyntaxNode>()
                    .FirstOrDefault(o => o.Definitions.Any(def => def.Name == varName));
                if (decl != null)
                    return decl;

                node = node.Parent;
            }

            return null;
        }

        public bool StartsWithVarName(string varName) => Token?.Content?.StartsWithVarName(varName) == true;
        public bool IsVarName(string varName) => this.HasNodeContent(varName);

        protected override SyntaxNode CreateSelf() => new GenericSyntaxNode(Token);
    }
}