﻿// -----------------------------------------------------------------------
//  <copyright file="FunctionCallSyntaxNode.cs">
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
    /// <summary>
    /// Represents a user-defined function.
    /// </summary>
    public class FunctionCallSyntaxNode : SyntaxNode, IRenamable
    {
        public string Name { get; private set; }

        public RoundBracketSyntaxNode Params => Children.Any() ? (RoundBracketSyntaxNode)Children[0] : new RoundBracketSyntaxNode();

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

        public bool ModifiesGlobalVariables() => GetCallee()?.ModifiesGlobalVariables() == true;

        /// <summary>
        /// Find the definition of the function being called (which may be null for functions defined in other buffers, or GLSL function calls).
        /// </summary>
        public FunctionDefinitionSyntaxNode GetCallee() =>
            this.Root().FunctionDefinitions().FirstOrDefault(o => o.Name == Name && Params.GetCsv().Count() == o.Params.GetCsv().Count());

        public void Rename(string oldName, string newName)
        {
            Name = newName;
        }

        /// <summary>
        /// See if a node looks like it might be a function call (E.g. foo(...))
        /// </summary>
        public static bool IsNodeFunctionLike(SyntaxNode node)
        {
            if (node is not GenericSyntaxNode)
                return false;
            if (node.Next is not RoundBracketSyntaxNode brackets)
                return false;
            if (brackets.TheTree.Any(o => o.Token is SemicolonToken))
                return false;
            return true;
        }
    }
}