// -----------------------------------------------------------------------
//  <copyright file="Hinter.NegativeSqrtHint.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

namespace Shrinker.Parser.Hints;

public class NegativeSqrtHint : CodeHint
{
    public NegativeSqrtHint(string function)
        : base(function, "Invalid 'sqrt' argument - Argument must be positive.", HintPriority.High)
    {
    }
}