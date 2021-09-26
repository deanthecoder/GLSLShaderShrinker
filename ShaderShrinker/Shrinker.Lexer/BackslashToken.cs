// -----------------------------------------------------------------------
//  <copyright file="BackslashToken.cs">
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
    public class BackslashToken : Token
    {
        public BackslashToken()
        {
            Content = "\\";
        }

        public override IToken TryMatch(string code, ref int offset) =>
            Peek(code, offset) == '\\' ? new BackslashToken { Content = Read(code, ref offset) } : null;

        public override IToken Clone() => new BackslashToken();
    }
}