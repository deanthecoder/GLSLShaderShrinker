// -----------------------------------------------------------------------
//  <copyright file="KeywordToken.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Shrinker.Lexer
{
    [DebuggerDisplay("{" + nameof(Content) + "}")]
    public class KeywordToken : IToken
    {
        public string Content { get; }
        public static IEnumerable<string> Names { get; } = new[] { "if", "else", "return", "for", "while", "struct", "inout", "in", "out", "inout", "do", "break", "continue", "switch", "case" };

        public IToken TryJoin(List<IToken> tokens, int tokenIndex, out int deletePrevious, out int deleteTotal)
        {
            deletePrevious = 0;
            deleteTotal = 0;
            return null;
        }

        public IToken Clone() => new KeywordToken(Content);

        public KeywordToken(string content)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }
    }
}