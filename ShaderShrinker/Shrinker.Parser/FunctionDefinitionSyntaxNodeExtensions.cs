// -----------------------------------------------------------------------
//  <copyright file="FunctionDefinitionSyntaxNodeExtensions.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace Shrinker.Parser
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