// -----------------------------------------------------------------------
//  <copyright file="FunctionDefinitionSyntaxNodeExtensions.cs">
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
    public static class FunctionDefinitionSyntaxNodeExtensions
    {
        public static IEnumerable<VariableAssignmentSyntaxNode> LocalVariables(this FunctionDefinitionSyntaxNode functionNode)
        {
            return functionNode.Braces.TheTree
                .OfType<VariableDeclarationSyntaxNode>()
                .SelectMany(o => o.Definitions);
        }
    }
}