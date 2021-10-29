// -----------------------------------------------------------------------
//  <copyright file="MiscCharacterToken.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

namespace Shrinker.Lexer
{
    public class MiscCharacterToken : Token
    {
        public override IToken TryMatch(string code, ref int offset) =>
            offset < code.Length ? new MiscCharacterToken { Content = Read(code, ref offset) } : null;

        public override IToken Clone() => new MiscCharacterToken { Content = Content };
    }
}