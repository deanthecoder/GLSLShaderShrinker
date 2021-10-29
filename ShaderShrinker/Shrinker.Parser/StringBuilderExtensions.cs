// -----------------------------------------------------------------------
//  <copyright file="StringBuilderExtensions.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System.Text;
using Shrinker.Lexer;

namespace Shrinker.Parser
{
    public static class StringBuilderExtensions
    {
        public static int GetColumn(this StringBuilder sb)
        {
            var column = 0;
            while (column < sb.Length && !sb[sb.Length - column - 1].IsNewline())
                column++;
            return column;
        }
    }
}