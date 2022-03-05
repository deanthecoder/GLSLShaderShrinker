// -----------------------------------------------------------------------
//  <copyright file="SyntaxNodeExtensions.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Shrinker.Lexer;
using Shrinker.Parser.SyntaxNodes;

namespace UnitTests.Extensions
{
    public static class SyntaxNodeExtensions
    {
        public static IEnumerable<IToken> NonNullChildTokens(this SyntaxNode root) => root.Children.Where(o => o.Token != null).Select(o => o.Token);
    }
}