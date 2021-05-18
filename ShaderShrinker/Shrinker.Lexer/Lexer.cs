// -----------------------------------------------------------------------
//  <copyright file="LexerTests.cs">
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
using System.IO;

namespace Shrinker.Lexer
{
    /// <summary>
    /// Processes a raw stream of code characters to build a linear sequence of tokens.
    /// </summary>
    /// <remarks>These tokens can then be passed to the Parser for further processing.</remarks>
    public class Lexer
    {
        public List<IToken> Tokens { get; } = new List<IToken>();

        /// <summary>
        /// The list of all known Tokens classes.
        /// </summary>
        /// <remarks>Order is important - Tokens earlier in the list are processed first.</remarks>
        private readonly Token[] m_tokenObjects =
        {
            new MultiLineCommentToken(),
            new SingleLineCommentToken(),
            new LineEndToken(),
            new BackslashToken(),
            new WhitespaceToken(),
            new UniformToken(),
            new ConstToken(),
            new PrecisionToken(),
            new AlphaNumToken(),
            new IntToken(),
            new QuoteToken(),
            new BracketToken(),
            new CommaToken(),
            new DotToken(),
            new SemicolonToken(),
            new EqualityOperatorToken(),
            new AssignmentOperatorToken(),
            new SymbolOperatorToken(),
            new PreprocessorToken()
        };

        public bool Load(FileInfo codeFile)
        {
            if (!codeFile.Exists)
                throw new FileNotFoundException("Can't find GLSL file.", codeFile.FullName);

            return Load(File.ReadAllText(codeFile.FullName));
        }

        public bool Load(string code)
        {
            Tokens.Clear();

            var offset = 0;
            while (true)
            {
                var lexeme = Detect(code, ref offset);
                if (lexeme == null)
                    break;

                Tokens.Add(lexeme);
            }

            JoinTokens();

            var remaining = code.Length - offset;
            if (remaining > 0)
            {
                Console.WriteLine($"{remaining} characters remaining.");
                Console.WriteLine($"Next: {code.Substring(offset, 16)}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Process the sequence of Tokens, optionally combining some into sections more closely representing the code.
        /// </summary>
        private void JoinTokens()
        {
            while (true)
            {
                var anyChanges = false;

                for (var tokenIndex = 0; tokenIndex < Tokens.Count; tokenIndex++)
                {
                    var newToken = Tokens[tokenIndex].TryJoin(Tokens, tokenIndex, out var deletePrevious, out var deleteTotal);
                    if (newToken == null)
                        continue;

                    tokenIndex -= deletePrevious;
                    if (deleteTotal > 1)
                        Tokens.RemoveRange(tokenIndex, deleteTotal - 1);
                    Tokens[tokenIndex] = newToken;

                    anyChanges = true;
                }

                if (!anyChanges)
                    break;
            }
        }

        private IToken Detect(string code, ref int offset)
        {
            foreach (var tokenObject in m_tokenObjects)
            {
                var match = tokenObject.TryMatch(code, ref offset);
                if (match != null)
                    return match;
            }

            return null;
        }
    }
}