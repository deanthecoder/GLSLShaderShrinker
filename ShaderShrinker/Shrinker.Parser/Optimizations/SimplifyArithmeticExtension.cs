// -----------------------------------------------------------------------
//  <copyright file="SimplifyArithmeticExtension.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System.Linq;
using Shrinker.Lexer;
using Shrinker.Parser.SyntaxNodes;

namespace Shrinker.Parser.Optimizations
{
    public static class SimplifyArithmeticExtension
    {
        /// <summary>
        /// Returns true if all optimizations should be re-run.
        /// </summary>
        public static bool SimplifyArithmetic(this SyntaxNode rootNode)
        {
            var repeatSimplifications = false;

            while (true)
            {
                var didChange = false;

                // Brackets within brackets.
                foreach (var toRemove in
                    rootNode.TheTree
                        .OfType<RoundBracketSyntaxNode>()
                        .Where(o => o.IsOnlyChild() && o.Parent is RoundBracketSyntaxNode)
                        .ToList())
                {
                    toRemove.ReplaceWith(toRemove.Children.ToArray());
                    didChange = true;
                }

                // Brackets containing just a number.
                foreach (var toRemove in
                    rootNode.TheTree
                        .OfType<RoundBracketSyntaxNode>()
                        .Where(
                               o => o.Children.Count == 1
                                    && o.Children.Single()?.Token is INumberToken
                                    && o.Parent is not FunctionCallSyntaxNode
                                    && (o.IsOnlyChild() || o.Previous?.Token is SymbolOperatorToken))
                        .ToList())
                {
                    toRemove.ReplaceWith(toRemove.Children.Single());
                    didChange = true;
                }

                // Assignments/return statements of the form 'return (...);'
                foreach (var toRemove in
                    rootNode.TheTree
                        .OfType<RoundBracketSyntaxNode>()
                        .Where(
                               o => o.IsOnlyChild()
                                    && (o.Parent is ReturnSyntaxNode || o.Parent is VariableAssignmentSyntaxNode))
                        .ToList())
                {
                    toRemove.ReplaceWith(toRemove.Children.ToArray());
                    didChange = true;
                }

                // Brackets within list of arguments. (E.g. 'f(a, (b + 1))' => 'f(a, b + 1)')
                foreach (var toRemove in
                    rootNode.TheTree
                        .OfType<RoundBracketSyntaxNode>()
                        .Where(
                               o => o.Previous?.Token is CommaToken && (o.NextNonComment?.Token is CommaToken || o.Next == null) ||
                                    o.Previous == null && o.NextNonComment?.Token is CommaToken)
                        .ToList())
                {
                    toRemove.ReplaceWith(toRemove.Children.ToArray());
                    didChange = true;
                }

                // Multiplication/division sequence.
                // E.g (a * 2.0 / b) => a * 2.0 / b
                foreach (var toRemove in
                    rootNode.TheTree
                        .OfType<RoundBracketSyntaxNode>()
                        .Where(
                               o => o.Parent is not FunctionCallSyntaxNode &&
                                    o.Parent is not IfSyntaxNode &&
                                    o.Parent is not ForSyntaxNode &&
                                    o.Parent is not SwitchSyntaxNode &&
                                    o.Parent is not PragmaDefineSyntaxNode &&
                                    !o.HasAncestor<PragmaDefineSyntaxNode>() && // Extra brackets in #define values are often needed.
                                    o.Previous is not SquareBracketSyntaxNode && // E.g. int[](1)
                                    o.Previous?.Token is not AlphaNumToken &&
                                    o.Previous?.Token is not TypeToken &&
                                    o.Previous?.Token is not KeywordToken &&
                                    o.Previous?.Token?.Content != "<<" &&
                                    o.Previous?.Token?.Content != ">>" &&
                                    o.Previous?.Token?.Content != "/" &&
                                    o.NextNonComment?.Token?.Content != "?" &&
                                    o.NextNonComment?.Token?.Content != "." && // Don't remove from 'p = (v * f).xyz'.
                                    o.GetCsv().Count() == 1)
                        .ToList())
                {
                    var symbols = toRemove.Children
                        .Where(o => o.Token is SymbolOperatorToken || o.Token is AssignmentOperatorToken || o.Token is EqualityOperatorToken)
                        .Select(o => o.Token.Content)
                        .Distinct()
                        .Except(new[] { "*", "/", "<", "<=", ">", ">=", "!" }); // Remove the 'safe' operators.

                    // If no symbols remain, it's safe to remove the brackets.
                    if (!symbols.Any())
                    {
                        toRemove.ReplaceWith(toRemove.Children.ToArray());
                        didChange = true;
                    }
                }

                // Simplify single-element vector to float if used in further vector arithmetic.
                foreach (var functionNodes in rootNode.FunctionDefinitions().Select(o => o.Braces))
                {
                    // Find vectors.
                    var vectorNodes = functionNodes.TheTree.OfType<GenericSyntaxNode>().Where(o => (o.Token as TypeToken)?.IsVector() == true).ToList();
                    foreach (var vectorNode in vectorNodes)
                    {
                        // Limit to vectors containing a single float. (E.g. vec3(1))
                        var brackets = vectorNode.Next as RoundBracketSyntaxNode;
                        if (brackets?.IsNumericCsv() != true || brackets.GetCsv().Count() != 1)
                            continue;

                        // Next node must be a math operator.
                        var mathOpNode = brackets.Next;
                        var isPartOfMathOp = mathOpNode != null && mathOpNode.Token?.GetMathSymbolType() != TokenExtensions.MathSymbolType.Unknown;
                        if (isPartOfMathOp)
                        {
                            // ...and then another vector.
                            var isNextNodeVector = (mathOpNode.Next?.Token as TypeToken)?.IsVector() == true;
                            if (isNextNodeVector)
                            {
                                // Replace lhs vector with a float.
                                var numToken = brackets.Children.Single().Token as INumberToken;
                                if (numToken is IntToken intToken)
                                    numToken = intToken.AsFloatToken();

                                HandleDoubleNegative(vectorNode, numToken);

                                brackets.Remove();
                                vectorNode.ReplaceWith(new GenericSyntaxNode((FloatToken)numToken));

                                didChange = true;
                                continue;
                            }
                        }

                        // ...or previous node must be a math operator.
                        mathOpNode = vectorNode.Previous;
                        isPartOfMathOp = mathOpNode != null && mathOpNode.Token?.GetMathSymbolType() != TokenExtensions.MathSymbolType.Unknown;
                        if (isPartOfMathOp)
                        {
                            // ...preceded by another vector.
                            var isPreviousNodeVector = mathOpNode.Previous is RoundBracketSyntaxNode &&
                                                       (mathOpNode.Previous.Previous?.Token as TypeToken)?.IsVector() == true;
                            if (isPreviousNodeVector)
                            {
                                // Replace rhs vector with a float.
                                var numToken = brackets.Children.Single().Token as INumberToken;
                                if (numToken is IntToken intToken)
                                    numToken = intToken.AsFloatToken();

                                HandleDoubleNegative(vectorNode, numToken);

                                brackets.Remove();
                                vectorNode.ReplaceWith(new GenericSyntaxNode((FloatToken)numToken));

                                didChange = true;
                            }
                        }
                    }
                }

                if (!didChange)
                    break;

                repeatSimplifications = true;
            }

            // Remove unnecessary '+' prefixes.
            foreach (var plusNode in rootNode.TheTree
                .OfType<GenericSyntaxNode>()
                .Where(
                       o => o.Token is SymbolOperatorToken token &&
                            token.Content == "+" &&
                            (o.Previous == null || o.Previous.Token?.Content?.IsAnyOf("=", "(", "return", ",", "*", "/", "%", "-") == true))
                .ToList())
            {
                plusNode.Remove();
                repeatSimplifications = true;
            }

            return repeatSimplifications;
        }

        private static void HandleDoubleNegative(GenericSyntaxNode vectorNode, INumberToken numToken)
        {
            if (vectorNode.Previous?.Token is SymbolOperatorToken { Content: "-" }
                && numToken.IsNegative())
            {
                // Invert a double-negative.
                numToken.Negate();

                if (vectorNode.Previous.Previous == null)
                    vectorNode.Previous.Remove();
                else
                    vectorNode.Previous.ReplaceWith(new GenericSyntaxNode(new SymbolOperatorToken("+")));
            }
        }
    }
}