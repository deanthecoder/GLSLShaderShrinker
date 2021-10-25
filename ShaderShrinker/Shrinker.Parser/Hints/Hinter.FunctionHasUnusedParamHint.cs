// -----------------------------------------------------------------------
//  <copyright file="Hinter.FunctionHasUnusedParamHint.cs" company="Global Graphics Software Ltd">
//      Copyright (c) 2021 Global Graphics Software Ltd. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Global Graphics Software Ltd. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

namespace Shrinker.Parser.Hints;

public class FunctionHasUnusedParamHint : CodeHint
{
    public FunctionHasUnusedParamHint(string function, string param) : base(function, $"Function parameter '{param}' is unused.")
    {
    }
}