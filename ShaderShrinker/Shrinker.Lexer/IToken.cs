// -----------------------------------------------------------------------
//  <copyright file="IToken.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace Shrinker.Lexer
{
    /// <summary>
    /// Represents a single word/symbol in the source GLSL.
    /// </summary>
    /// <remarks>New tokens should be added to the Lexer class.</remarks>
    /// <see cref="Lexer"/>
    public interface IToken
    {
        string Content { get; }

        /// <summary>
        /// Post-process the complete token list, allowing a token to merge with its neighbors. (E.g. '//' merging with following text until newline.
        /// </summary>
        IToken TryJoin(List<IToken> tokens, int tokenIndex, out int deletePrevious, out int deleteTotal);
    }
}