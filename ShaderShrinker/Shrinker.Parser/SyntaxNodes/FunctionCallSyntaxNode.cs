// -----------------------------------------------------------------------
//  <copyright file="FunctionCallSyntaxNode.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.Linq;

namespace Shrinker.Parser.SyntaxNodes
{
    /// <summary>
    /// Represents a user-defined function.
    /// </summary>
    public class FunctionCallSyntaxNode : SyntaxNode
    {
        public string Name { get; }
        public RoundBracketSyntaxNode Params => (RoundBracketSyntaxNode)Children[0];

        public FunctionCallSyntaxNode(GenericSyntaxNode nameNode, RoundBracketSyntaxNode brackets) : this(nameNode?.Token?.Content)
        {
            Adopt(brackets ?? throw new ArgumentNullException(nameof(brackets)));
        }

        private FunctionCallSyntaxNode(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public override string UiName => $"{Name}({(Params.Children.Any() ? "..." : string.Empty)})";

        protected override SyntaxNode CreateSelf() => new FunctionCallSyntaxNode(Name);

        public virtual bool HasOutParam => this.Root().Children.OfType<FunctionSyntaxNodeBase>().Any(o => o.Name == Name && o.HasOutParam);

        public bool ModifiesGlobalVariables() =>
            this.Root().FunctionDefinitions().FirstOrDefault(o => o.Name == Name)?.ModifiesGlobalVariables() == true;
    }
}