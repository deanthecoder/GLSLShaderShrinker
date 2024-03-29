﻿// -----------------------------------------------------------------------
//  <copyright file="ReturnSyntaxNode.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System.Linq;

namespace Shrinker.Parser.SyntaxNodes
{
    public class ReturnSyntaxNode : GroupSyntaxNode, IRenamable
    {
        public override string UiName => Children.Any() ? "return ..." : "return";

        protected override SyntaxNode CreateSelf() => new ReturnSyntaxNode();

        public string Name { get; private set; } = "return";

        public void Rename(string oldName, string newName) => Name = newName;
    }
}