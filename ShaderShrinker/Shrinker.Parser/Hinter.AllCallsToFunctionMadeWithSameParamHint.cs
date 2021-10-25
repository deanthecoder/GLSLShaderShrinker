// -----------------------------------------------------------------------
//  <copyright file="Hinter.AllCallsToFunctionMadeWithSameParamHint.cs" company="Global Graphics Software Ltd">
//      Copyright (c) 2021 Global Graphics Software Ltd. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Global Graphics Software Ltd. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Linq;
using Shrinker.Parser.SyntaxNodes;

namespace Shrinker.Parser;

public static partial class Hinter
{
    public class AllCallsToFunctionMadeWithSameParamHint : CodeHint
    {
        public AllCallsToFunctionMadeWithSameParamHint(FunctionDefinitionSyntaxNode function, string paramName) : base($"{function.ReturnType} {function.Name}({string.Join(", ", function.ParamNames.Select(o => o.UiName))})", $"All callers pass the same value for parameter '{paramName}'.\nConsider hard-coding the caller's value into the target function.")
        {
        }
    }
}