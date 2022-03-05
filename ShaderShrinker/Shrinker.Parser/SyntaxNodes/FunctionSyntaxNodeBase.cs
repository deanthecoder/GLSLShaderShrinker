// -----------------------------------------------------------------------
//  <copyright file="FunctionSyntaxNodeBase.cs">
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

namespace Shrinker.Parser.SyntaxNodes
{
    public abstract class FunctionSyntaxNodeBase : SyntaxNode
    {
        public string ReturnType { get; protected set; }
        public string Name => Children[0].Token.Content;
        public RoundBracketSyntaxNode Params => (RoundBracketSyntaxNode)Children[1];
        public bool HasOutParam => Params.TheTree.Select(o => o.Token as TypeToken).Any(o => o?.InOut == TypeToken.InOutType.InOut || o?.InOut == TypeToken.InOutType.Out);

        public bool IsVoidParam() => Params.Children.Count == 1 && Params.Children[0].HasNodeContent("void");

        public List<GenericSyntaxNode> ParamNames
        {
            get
            {
                if (!Params.Children.Any())
                    return new List<GenericSyntaxNode>();
                var children = Params.Children.OfType<GenericSyntaxNode>().Where(o => o.Token is AlphaNumToken or CommaToken).Append(new GenericSyntaxNode(new CommaToken())).ToList();
                var commaIndexes = children.Where(o => o.Token is CommaToken).Select(o => children.IndexOf(o));
                return commaIndexes.Where(i => i > 0 && children[i - 1].Token is not CommaToken).Select(i => children[i - 1]).ToList();
            }
        }

        public bool IsMain() => Name.StartsWith("main");
    }
}