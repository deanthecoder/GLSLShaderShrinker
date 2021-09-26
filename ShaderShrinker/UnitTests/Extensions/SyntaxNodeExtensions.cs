// -----------------------------------------------------------------------
//  <copyright file="SyntaxNodeExtensions.cs" company="Global Graphics Software Ltd">
//      Copyright (c) 2021 Global Graphics Software Ltd. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Global Graphics Software Ltd. does not warrant or make any representations regarding the use or
//  results of use of this example.
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