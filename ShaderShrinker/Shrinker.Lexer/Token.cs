// -----------------------------------------------------------------------
//  <copyright file="Token.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;

namespace Shrinker.Lexer
{
    [DebuggerDisplay("{" + nameof(Content) + "}")]
    public abstract class Token : IToken
    {
        public string Content { get; protected set; }

        public abstract IToken TryMatch(string code, ref int offset);

        protected static char Peek(string code, int offset) =>
            offset < code.Length ? code[offset] : (char)0;

        protected static string Peek(string code, int offset, int count)
        {
            var s = string.Empty;
            for (var i = 0; i < count; i++)
                s += Peek(code, offset + i);
            return s;
        }

        protected static string Read(string code, ref int offset, int count = 1)
        {
            var s = code.Substring(offset, count);
            offset += count;
            return s;
        }

        /// <summary>
        /// Called to 'join' this token with the 'next' tokens in the list. (E.g. Join a '//' with all subsequent tokens until reaching a newline.)
        /// </summary>
        /// <param name="tokens">The entire list of tokens.</param>
        /// <param name="tokenIndex">The current token (which will be of 'this' type, when called).</param>
        /// <param name="deletePrevious">How many tokens before the 'current' should be deleted.</param>
        /// <param name="deleteTotal">How many tokens in the sequence to delete.</param>
        /// <returns>The replacement token (or 'null' if no change necessary).</returns>
        public virtual IToken TryJoin(List<IToken> tokens, int tokenIndex, out int deletePrevious, out int deleteTotal)
        {
            deletePrevious = 0;
            deleteTotal = 0;
            return null;
        }

        public abstract IToken Clone();
    }
}