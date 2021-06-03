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
            if (!MultiCharacterOperators.Contains(symbol) &&
                !ModifyingOperator.Contains(symbol) &&
                !SingleCharacterOperators.Contains(symbol))
                throw new SyntaxErrorException($"Unrecognized operator: '{symbol}'");

            Content = symbol;
        }

        public override IToken TryMatch(string code, ref int offset)
        {
            foreach (var peekLength in new[] { 3, 2, 1 })
            {
                var s = Peek(code, offset, peekLength);

                var match = MultiCharacterOperators.FirstOrDefault(o => o == s) ??
                            ModifyingOperator.FirstOrDefault(o => o == s);
                if (match != null)
                    return new SymbolOperatorToken(Read(code, ref offset, match.Length));

                if (peekLength == 1 && SingleCharacterOperators.Contains(s[0]))
                    return new SymbolOperatorToken(Read(code, ref offset));
            }

            return null;
        }

        private static string SingleCharacterOperators => ":!+-*/|<>@?&^%";

        private static string[] MultiCharacterOperators { get; } = { "||", "&&", "<=", ">=", "<<=", ">>=", "<<", ">>" };
        public static string[] ModifyingOperator { get; } = { "--", "++", "+=", "-=", "/=", "!=", "%=", "^=", "&=", "*=" };
    }
}