// -----------------------------------------------------------------------
//  <copyright file="SimplifyVectorConstructorsExtension.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using Shrinker.Lexer;
using Shrinker.Parser.SyntaxNodes;

namespace Shrinker.Parser.Optimizations
{
    public static class SimplifyVectorConstructorsExtension
    {
        public static void SimplifyVectorConstructors(this SyntaxNode rootNode)
        {
            // Simplify numbers to integers.
            foreach (var brackets in rootNode.TheTree
                .Where(
                       node => node.Token is TypeToken token &&
                               TypeToken.MultiValueTypes.Any(o => o == token.Content) &&
                               node.Next is RoundBracketSyntaxNode)
                .Select(syntaxNode => (RoundBracketSyntaxNode)syntaxNode.Next)
                .ToList())
            {
                foreach (var child in brackets.Children.Where(o => o.Token is FloatToken).ToList())
                {
                    var hasCommaPrefix = child.Previous?.Token is CommaToken;
                    var hasCommaSuffix = child.Next?.Token is CommaToken;
                    if (child.Previous == null && hasCommaSuffix ||
                        hasCommaPrefix && hasCommaSuffix ||
                        hasCommaPrefix && child.Next == null ||
                        child.Previous == null && child.Next == null)
                    {
                        if (!double.TryParse(child.Token.Content, out var asDouble))
                            continue;

                        // vec3 (etc) can be constructed with integer params.
                        if (Math.Abs((int)asDouble - asDouble) < 0.0000001)
                        {
                            var asInt = new GenericSyntaxNode(new IntToken((int)asDouble));
                            if (asInt.ToCode().Length < child.ToCode().Length)
                                child.ReplaceWith(asInt);
                        }
                    }
                }

                // If all components are the same number, just list it once.
                if (brackets.Children.Count <= 1)
                    continue;
                var components = brackets.Children.Where(o => o.Token is not CommaToken).Select(o => o.Token?.Content).Distinct().ToList();
                if (components.Count == 1 && brackets.Children[0]?.Token is INumberToken)
                {
                    while (brackets.Children.Count > 1)
                        brackets.Children.Last().Remove();
                }
            }
        }

        public static void RemoveUnnecessaryVectorConstructors(this SyntaxNode rootNode)
        {
            // Simplify vectors with redundant copy constructor. (I.e. vec3(v) => v)
            foreach (var vectorLength in new[] { 2, 3, 4 })
            {
                var vecType = $"vec{vectorLength}";

                foreach (var vectorNode in rootNode.TheTree
                    .OfType<GenericSyntaxNode>()
                    .Where(
                           o => (o.Token as TypeToken)?.Content == vecType &&
                                (o.Next as RoundBracketSyntaxNode)?.Children.Count == 1))
                {
                    // Find type of vec parameter.
                    var brackets = (RoundBracketSyntaxNode)vectorNode.Next;
                    var vecParam = brackets.Children.Single() as GenericSyntaxNode;

                    var lhsRhs = vecParam?.Token?.Content.Split('.');
                    if (lhsRhs?.Length != 2 || lhsRhs[1].Any(ch => !"rgbaxyzw".Contains(ch)))
                    {
                        var vecParamType = vecParam?.FindVarDeclaration();
                        if (vecParamType?.VariableType.Content != vecType)
                            continue; // Can't find what type of 'vec' we have, or different types.
                    }

                    if (lhsRhs.Length >= 2 && lhsRhs[1].Length != vectorLength)
                        continue; // RHS does not define the entire vector.

                    // They match - Remove the surrounding vecN keyword.
                    brackets.Remove();
                    vectorNode.ReplaceWith(vecParam);
                }
            }
        }
    }
}