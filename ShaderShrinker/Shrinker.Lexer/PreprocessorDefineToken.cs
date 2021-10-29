// -----------------------------------------------------------------------
//  <copyright file="PreprocessorDefineToken.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System;

namespace Shrinker.Lexer
{
    public class PreprocessorDefineToken : PreprocessorToken
    {
        public bool HasParams { get; }

        public PreprocessorDefineToken(string toEndOfLine)
        {
            Content = "#define";

            var bracketIndex = toEndOfLine.IndexOf('(');
            HasParams = bracketIndex != -1 && !char.IsWhiteSpace(toEndOfLine[bracketIndex - 1]);
        }

        public override IToken TryMatch(string code, ref int offset) => throw new NotImplementedException($"Created by {nameof(PreprocessorToken)}.");
    }
}