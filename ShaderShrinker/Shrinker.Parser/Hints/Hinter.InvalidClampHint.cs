// -----------------------------------------------------------------------
//  <copyright file="Hinter.InvalidClampHint.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

namespace Shrinker.Parser.Hints;

public class InvalidClampHint : CodeHint
{
    public InvalidClampHint(string function) : base(function, "Invalid 'clamp' argument order - Use clamp(f, min, max).", HintPriority.High)
    {
    }
}