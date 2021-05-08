// -----------------------------------------------------------------------
//  <copyright file="TokenExtensions.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

namespace Shrinker.Lexer
{
    public static class TokenExtensions
    {
        public enum MathSymbolType
        {
            AddSubtract,
            MultiplyDivide,
            Unknown
        }

        public static MathSymbolType GetMathSymbolType(this IToken token)
        {
            if (token is not SymbolOperatorToken)
                return MathSymbolType.Unknown;

            switch (token.Content)
            {
                case "+":
                case "-":
                    return MathSymbolType.AddSubtract;

                case "*":
                case "/":
                    return MathSymbolType.MultiplyDivide;

                default:
                    return MathSymbolType.Unknown;
            }
        }
    }
}