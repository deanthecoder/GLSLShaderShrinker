// -----------------------------------------------------------------------
//  <copyright file="ExternalFunctionCallSyntaxNode.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

namespace Shrinker.Parser.SyntaxNodes
{
    /// <summary>
    /// Represents an external function call - Hopefully defined in another Shadertoy buffer.
    /// </summary>
    public class ExternalFunctionCallSyntaxNode : FunctionCallSyntaxNode
    {
        public ExternalFunctionCallSyntaxNode(GenericSyntaxNode nameNode, RoundBracketSyntaxNode brackets)
            : base(nameNode, brackets)
        {
        }

        /// <summary>
        /// Be pessimistic and assume the function has an 'out' param.
        /// </summary>
        public override bool HasOutParam => true;
    }
}