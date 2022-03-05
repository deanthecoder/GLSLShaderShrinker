// -----------------------------------------------------------------------
//  <copyright file="CommaToken.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

namespace Shrinker.Lexer
{
    public class CommaToken : Token
    {
        public CommaToken()
        {
            Content = ",";
        }

        public override IToken TryMatch(string code, ref int offset) =>
            Peek(code, offset) == ',' ? new CommaToken { Content = Read(code, ref offset) } : null;

        public override IToken Clone() => new CommaToken();
    }
}