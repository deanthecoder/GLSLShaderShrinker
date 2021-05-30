// -----------------------------------------------------------------------
//  <copyright file="SimplifyFloatFormatExtension.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Linq;
using Shrinker.Lexer;
using Shrinker.Parser.SyntaxNodes;

namespace Shrinker.Parser.Optimizations
{
    public static class SimplifyFloatFormatExtension
    {
        public static void SimplifyFloatFormat(this SyntaxNode rootNode)
        {
            // Simplify float numbers to minimize space. (E.g. 1.1000 => 1.1, 1000.0 => 1e3)
            rootNode.TheTree.ToList().ForEach(o => (o.Token as FloatToken)?.Simplify());

            // Simplify 'float(...)'
            var nodes = rootNode.TheTree
                .OfType<GenericSyntaxNode>()
                .Where(
                       o => o.NextNonComment is RoundBracketSyntaxNode brackets &&
                            brackets.Children.Count == 1)
                .ToList();
            foreach (var floatKeyword in nodes.Where(o => o.Token.Content == "float"))
            {
                var bracketContent = floatKeyword.NextNonComment.Children[0];
                switch (bracketContent.Token)
                {
                    case FloatToken:
                        // E.g. float(1.2) => Remove the 'float' cast.
                        floatKeyword.NextNonComment.Remove();
                        floatKeyword.ReplaceWith(bracketContent);
                        break;

                    case IntToken i:
                        // E.g. float(123) => Remove the 'float' cast and turn content into float value.
                        floatKeyword.NextNonComment.Remove();
                        floatKeyword.ReplaceWith(new GenericSyntaxNode(new FloatToken($"{i.Content}.")));
                        break;
                }
            }

            // Simplify 'int(...)'
            foreach (var intKeyword in nodes.Where(o => o.Token.Content == "int"))
            {
                var bracketContent = intKeyword.NextNonComment.Children[0];
                switch (bracketContent.Token)
                {
                    case FloatToken f:
                        // E.g. int(1.2) => Remove the cast and convert float to int.
                        intKeyword.NextNonComment.Remove();
                        intKeyword.ReplaceWith(new GenericSyntaxNode(new IntToken((int)f.Number)));
                        break;

                    case IntToken:
                        // E.g. int(123) => Remove the 'int' cast.
                        intKeyword.NextNonComment.Remove();
                        intKeyword.ReplaceWith(bracketContent);
                        break;
                }
            }
        }
    }
}