// -----------------------------------------------------------------------
//  <copyright file="Hinter.AllCallsToFunctionMadeWithSameParamHint.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System.Linq;
using Shrinker.Parser.SyntaxNodes;

namespace Shrinker.Parser.Hints
{
    public class AllCallsToFunctionMadeWithSameParamHint : CodeHint
    {
        public AllCallsToFunctionMadeWithSameParamHint(FunctionDefinitionSyntaxNode callee, int paramIndex)
            : base($"{callee.ReturnType} {callee.Name}({string.Join(", ", callee.ParamNames.Select(o => o.UiName))})", $"All callers pass the same value for parameter '{callee.ParamNames[paramIndex].UiName}'.\nConsider hard-coding the caller's value into the target function.")
        {
        }
    }
}
