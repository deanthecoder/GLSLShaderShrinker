// -----------------------------------------------------------------------
//  <copyright file="PragmaIfSyntaxNode.cs">
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

namespace Shrinker.Parser.SyntaxNodes
{
    public class PragmaIfSyntaxNode : SyntaxNode
    {
        public string Name { get; }
        public GroupSyntaxNode TrueBranch { get; } = new GroupSyntaxNode();

        public GroupSyntaxNode FalseBranch { get; }

        public PragmaIfSyntaxNode(string name, IList<SyntaxNode> ifSection, IList<SyntaxNode> elseSection)
        {
            Name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));
            if (!Name.StartsWith("#"))
                throw new ArgumentException("Name must start with a # (I.e. #if FLAG or #ifdef NAME).");

            if (ifSection == null)
                throw new ArgumentNullException(nameof(ifSection));

            Adopt(TrueBranch.Adopt(ifSection.ToArray()));

            if (elseSection != null)
            {
                FalseBranch = new GroupSyntaxNode(elseSection);
                Adopt(FalseBranch);
            }
        }

        public override string UiName => FalseBranch == null ? $"{Name}...#endif" : $"{Name}...#else...#endif";

        protected override SyntaxNode CreateSelf() =>
            new PragmaIfSyntaxNode(Name, TrueBranch.Clone().m_children, FalseBranch?.Clone().m_children);

        public bool Is0() => Name.StartsWith("#if 0");
        public bool Is1() => Name.StartsWith("#if 1");
    }
}