// -----------------------------------------------------------------------
//  <copyright file="GroupSyntaxNode.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace Shrinker.Parser.SyntaxNodes
{
    /// <summary>
    /// A generic group of syntax nodes.
    /// </summary>
    public class GroupSyntaxNode : SyntaxNode
    {
        public GroupSyntaxNode()
        {
        }

        public GroupSyntaxNode(IEnumerable<SyntaxNode> nodes)
        {
            Adopt(nodes.ToArray());
        }

        public override string UiName => "<Group>";

        protected override SyntaxNode CreateSelf() => new GroupSyntaxNode();
    }
}