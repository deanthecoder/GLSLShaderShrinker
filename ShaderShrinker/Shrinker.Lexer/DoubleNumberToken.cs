// -----------------------------------------------------------------------
//  <copyright file="DoubleNumberToken.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.Linq;

namespace Shrinker.Lexer
{
    public class DoubleNumberToken : Token, INumberToken
    {
        public DoubleNumberToken(string s)
        {
            Content = s;
        }

        public override IToken TryMatch(string code, ref int offset) =>
            throw new InvalidOperationException("Should be created by DotToken.");

        public DoubleNumberToken Simplify()
        {
            if (Content.ToLower().Contains('e'))
                return this;

            // Strip starting '-'.
            var isNeg = Content.StartsWith("-");
            Content = Content.TrimStart('-');

            try
            {
                if (Content.StartsWith("."))
                    Content = $"0{Content}";

                Content = Content.TrimEnd('0');

                if (!Content.EndsWith("."))
                {
                    // Remove leading zeroes.
                    while (Content.StartsWith("0"))
                        Content = Content.Substring(1);
                }
                else
                {
                    // Detect 3e6.
                    var leadingNonZeros = Content.TakeWhile(ch => ch != '0' && ch != '.').Count();
                    var followingZeros = Content.Skip(leadingNonZeros).TakeWhile(ch => ch == '0').Count();
                    var expFormat = $"{Content.Substring(0, leadingNonZeros)}e{followingZeros}";
                    if (expFormat.Length < Content.Length)
                        Content = expFormat;
                }
            }
            finally
            {
                // Put back starting '-'.
                if (isNeg)
                    Content = $"-{Content}";
            }

            return this;
        }

        public void MakeNegative() => Content = $"-{Content.TrimStart('-')}";
        public void MakePositive() => Content = $"{Content.TrimStart('-')}";

        public bool IsOne() => Math.Abs(double.Parse(Content) - 1.0) < 0.000001;
    }
}