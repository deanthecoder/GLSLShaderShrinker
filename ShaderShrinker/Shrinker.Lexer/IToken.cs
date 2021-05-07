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