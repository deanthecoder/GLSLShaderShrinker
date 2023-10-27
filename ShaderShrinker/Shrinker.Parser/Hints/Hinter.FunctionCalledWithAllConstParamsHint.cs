// -----------------------------------------------------------------------
//  <copyright file="Hinter.FunctionCalledWithAllConstParamsHint.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using Shrinker.Parser.SyntaxNodes;

namespace Shrinker.Parser.Hints
{
    public class FunctionCalledWithAllConstParamsHint : CodeHint
    {
        public FunctionCalledWithAllConstParamsHint(SyntaxNode function) : base(function.ToCode(), "Function called with all-constant arguments. Consider replacing with the result.", HintPriority.Medium)
        {
        }
    }
}