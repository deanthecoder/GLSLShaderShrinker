﻿// -----------------------------------------------------------------------
//  <copyright file="Hinter.UnusedFunctionHint.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

namespace Shrinker.Parser.Hints
{
    public class UnusedFunctionHint : CodeHint
    {
        public UnusedFunctionHint(string function) : base(function, "Function is never called.", HintPriority.High)
        {
        }
    }
}