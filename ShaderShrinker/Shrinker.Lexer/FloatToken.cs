// -----------------------------------------------------------------------
//  <copyright file="FloatToken.cs">
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
    public class FloatToken : Token, INumberToken
    {
        public FloatToken(string s)
        {
            Content = s;
        }

        public double Number => double.Parse(Content.TrimEnd('f', 'F'));

        public override IToken TryMatch(string code, ref int offset) =>
            throw new InvalidOperationException("Should be created by DotToken.");

        public FloatToken Simplify()
        {
            if (Content.ToLower().Contains('e'))
                return this;

            // Strip starting '-'.
            var isNeg = Content.StartsWith("-");
            Content = Content.TrimStart('-');

            // Strip trailing 'f'.
            Content = Content.TrimEnd('f', 'F');

            try
            {
                if (Content.StartsWith("."))
                    Content = $"0{Content}";

                Content = Content.TrimEnd('0');

                if (!Content.EndsWith("."))
                {
                    // Remove leading zeroes.
                    Content = Content.TrimStart('0');
                }
                else
                {
                    // Detect 3e6.
                    var intPart = Content.Substring(0, Content.IndexOf('.'));
                    var trailingZeros = 0;
                    while (intPart.EndsWith("0"))
                    {
                        trailingZeros++;
                        intPart = intPart.Substring(0, intPart.Length - 1);
                    }

                    var expFormat = $"{intPart}e{trailingZeros}";
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

        public bool IsOne() => Math.Abs(Number - 1.0) < 0.000001;
    }
}