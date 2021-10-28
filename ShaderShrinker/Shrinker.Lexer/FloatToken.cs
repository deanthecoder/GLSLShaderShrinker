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
        public static FloatToken From(double c, int maxDp)
        {
            var floatToken = Math.Abs(c) < 0.0001 && Math.Abs(c) > 0.0 ? new FloatToken(c.ToString($".#{new string('#', maxDp - 1)}e0")) : new FloatToken(c.ToString($"F{maxDp}"));
            return (FloatToken)floatToken.Simplify();
        }

        public FloatToken(string s)
        {
            Content = s;
        }

        public double Number => double.Parse(Content.TrimEnd('f', 'F'));

        public override IToken TryMatch(string code, ref int offset) =>
            throw new InvalidOperationException("Should be created by DotToken.");

        public override IToken Clone() => new FloatToken(Content);

        public IToken Simplify()
        {
            const int MaxSigFigs = 9;

            if (Content.ToLower().Contains('e'))
            {
                var eIndex = Content.IndexOfAny(new[] { 'e', 'E' });
                while (eIndex > 0 && Content.Length > MaxSigFigs)
                {
                    Content = Content.Remove(eIndex - 1, 1);
                    eIndex--;
                }

                return this;
            }

            // Strip starting '-'.
            var isNeg = Content.StartsWith("-");
            Content = Content.TrimStart('-');

            // Trim unnecessary leading zeros.
            while (Content.Length > 1 && Content.StartsWith("0") && char.IsNumber(Content[1]))
                Content = Content.Substring(1);

            // Strip trailing 'f'.
            Content = Content.TrimEnd('f', 'F');

            try
            {
                if (Content.StartsWith("."))
                    Content = $"0{Content}";

                Content = Content.TrimEnd('0');

                // Any decimal places?
                if (!Content.EndsWith("."))
                {
                    // Yes - Remove leading zeroes.
                    Content = Content.TrimStart('0');

                    // Can we simplify using the 'e' format?
                    if (Content.StartsWith("."))
                    {
                        var eFormat = Content.Substring(1);
                        var zeros = 0;
                        while (eFormat.Length > 0 && eFormat[0] == '0')
                        {
                            zeros++;
                            eFormat = eFormat.Substring(1);
                        }

                        if (eFormat.Length > 0 && zeros > 0)
                        {
                            eFormat = $"{eFormat}e-{zeros + eFormat.Length}";
                            if (eFormat.Length < Content.Length)
                                Content = eFormat;
                        }
                    }
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

                // Trim to a sensible number of significant figures.
                if (Content.Length > MaxSigFigs)
                    Content = Content.Substring(0, MaxSigFigs);
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

        public void Negate()
        {
            if (Math.Sign(Number) > 0.0)
                MakeNegative();
            else if (Math.Sign(Number) < 0.0)
                MakePositive();
        }

        public bool IsOne() => Math.Abs(Number - 1.0) < 0.000001;
        public bool IsZero() => Math.Abs(Number) < 0.000001;
    }
}