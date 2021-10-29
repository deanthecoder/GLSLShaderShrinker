// -----------------------------------------------------------------------
//  <copyright file="Parser.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using Shrinker.Lexer;
using Shrinker.Parser.SyntaxNodes;

namespace Shrinker.Parser
{
    /// <summary>
    /// Takes a sequence of tokens from the Lexer and parses them into a syntax tree (mirroring the format of the code hierarchy).
    /// </summary>
    /// <remarks>This syntax tree can then be passed to the Shrinker for optimization.</remarks>
    public class Parser
    {
        private readonly ReadOnlyCollection<IToken> m_tokens;

        public SyntaxNode RootNode { get; private set; }

        public Parser(Lexer.Lexer lexer)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));
            m_tokens = lexer.Tokens.AsReadOnly();
        }

        public SyntaxNode Parse()
        {
            try
            {
                RootNode = FileSyntaxNode.Create(m_tokens);
                return RootNode;
            }
            catch (SyntaxErrorException e)
            {
                Console.WriteLine($"Syntax error: {e.Message}");
                throw;
            }
        }
    }
}