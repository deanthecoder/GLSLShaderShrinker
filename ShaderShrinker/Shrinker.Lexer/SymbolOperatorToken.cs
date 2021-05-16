// -----------------------------------------------------------------------
//  <copyright file="SymbolOperatorToken.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Linq;

namespace Shrinker.Lexer
{
    public class SymbolOperatorToken : Token
    {
        public SymbolOperatorToken()
        {
        }

        public SymbolOperatorToken(string symbol)
        {
            if (!DoubleCharacterOperators.Contains(symbol) &&
                !ModifyingOperator.Contains(symbol) &&
                !SingleCharacterOperators.Contains(symbol))
                throw new SyntaxErrorException($"Unrecognized operator: '{symbol}'");

            Content = symbol;
        }

        public override IToken TryMatch(string code, ref int offset)
        {
            var s = Peek(code, offset, 2);

            if (DoubleCharacterOperators.Contains(s) || ModifyingOperator.Contains(s))
                return new SymbolOperatorToken(Read(code, ref offset, 2));

            return SingleCharacterOperators.Contains(s[0]) ? new SymbolOperatorToken(Read(code, ref offset)) : null;
        }

        private static string SingleCharacterOperators => ":!+-*/|<>@?&^%";

        private static string[] DoubleCharacterOperators { get; } = { "||", "&&", "<=", ">=", "<<=", ">>=", "<<", ">>" };
        public static string[] ModifyingOperator { get; } = { "--", "++", "+=", "-=", "/=", "!=", "%=", "^=", "&=", "*=" };
    }
}