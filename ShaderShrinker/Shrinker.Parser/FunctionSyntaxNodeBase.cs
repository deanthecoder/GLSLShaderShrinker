// -----------------------------------------------------------------------
//  <copyright file="FunctionSyntaxNodeBase.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using Shrinker.Lexer;

namespace Shrinker.Parser
{
    public abstract class FunctionSyntaxNodeBase : SyntaxNode
    {
        public TypeToken ReturnType { get; protected set; }
        public string Name { get; protected set; }
        public RoundBracketSyntaxNode Params { get; protected set; }

        public bool IsVoidParam() => Params.Children.Count == 1 && Params.Children[0].Token.Content == "void";
    }
}