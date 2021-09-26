// -----------------------------------------------------------------------
//  <copyright file="VerbatimToken.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System;

namespace Shrinker.Lexer
{
    public class VerbatimToken : Token
    {
        public VerbatimToken(string entireLine)
        {
            Content = entireLine;
        }

        public override IToken TryMatch(string code, ref int offset) => throw new NotImplementedException("Created by PreprocessorToken.");

        public override IToken Clone() => new VerbatimToken(Content);
    }
}