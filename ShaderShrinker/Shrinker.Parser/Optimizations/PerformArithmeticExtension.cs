// -----------------------------------------------------------------------
//  <copyright file="PerformArithmeticExtension.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Shrinker.Lexer;
using Shrinker.Parser.SyntaxNodes;

namespace Shrinker.Parser.Optimizations
{
    public static class PerformArithmeticExtension
    {
        private const int MaxDp = 5;

        /// <summary>
        /// Returns true if all optimizations should be re-run.
        /// </summary>
        public static bool PerformArithmetic(this SyntaxNode rootNode)
        {
            var repeatSimplifications = false;

            while (true)
            {
                var didChange = false;

                // 'a = b + -c' => 'a = b - c'
                foreach (var symbolNode in rootNode.TheTree
                    .OfType<GenericSyntaxNode>()
                    .Where(
                           o => (o.Token as SymbolOperatorToken)?.Content == "+" &&
                                (o.Next?.Token as SymbolOperatorToken)?.Content == "-")
                    .ToList())
                {
                    symbolNode.Remove();
                    didChange = true;
                }

                // 'f + -2.3' => 'f - 2.3'
                foreach (var numNode in rootNode.TheTree
                    .OfType<GenericSyntaxNode>()
                    .Where(
                           o => o.Token is INumberToken &&
                                o.Token.Content.StartsWith("-") &&
                                o.Previous?.Token?.GetMathSymbolType() == TokenExtensions.MathSymbolType.AddSubtract)
                    .ToList())
                {
                    var symbol = numNode.Previous.Token.Content[0] == '-' ? "+" : "-";
                    numNode.Previous.ReplaceWith(new GenericSyntaxNode(new SymbolOperatorToken(symbol)));
                    ((INumberToken)numNode.Token).MakePositive();
                    didChange = true;
                }

                // 'f * 1.0' or 'f / 1.0' => 'f'
                foreach (var oneNode in rootNode.TheTree
                    .OfType<GenericSyntaxNode>()
                    .Where(
                           o => (o.Token as INumberToken)?.IsOne() == true &&
                                o.Previous?.Token?.GetMathSymbolType() == TokenExtensions.MathSymbolType.MultiplyDivide)
                    .ToList())
                {
                    oneNode.Previous.Remove();
                    oneNode.Remove();
                    didChange = true;
                }
                
                // 'f / 0.0' => 'f * 0.0'
                foreach (var zeroNode in rootNode.TheTree
                    .OfType<GenericSyntaxNode>()
                    .Where(
                           o => (o.Token as INumberToken)?.IsZero() == true &&
                                o.Previous?.Token?.Content == "/")
                    .ToList())
                {
                    zeroNode.Previous.ReplaceWith(new GenericSyntaxNode(new SymbolOperatorToken("*")));
                    didChange = true;
                }
                
                // 'f * 0.0' => <Nothing>
                if (!didChange)
                {
                    foreach (var zeroNode in rootNode.TheTree
                        .OfType<GenericSyntaxNode>()
                        .Where(
                               o => (o.Token as INumberToken)?.IsZero() == true &&
                                    o.Previous?.Token?.Content == "*" &&
                                    (o.Next == null || o.Next?.Token is CommaToken))
                        .ToList())
                    {
                        var f = zeroNode.Previous.Previous;
                        if ((f as GenericSyntaxNode)?.Token is AlphaNumToken ||
                            f is FunctionCallSyntaxNode call && !call.HasOutParam)
                        {
                            f.Remove();
                            zeroNode.Previous.Remove();
                            didChange = true;
                        }
                    }
                }
                
                // 'f + 0.0' => f
                if (!didChange)
                {
                    foreach (var zeroNode in rootNode.TheTree
                        .OfType<GenericSyntaxNode>()
                        .Where(
                               o => (o.Token as INumberToken)?.IsZero() == true &&
                                    o.Previous?.Token?.GetMathSymbolType() == TokenExtensions.MathSymbolType.AddSubtract &&
                                    (o.Next == null || o.Next?.Token is CommaToken))
                        .ToList())
                    {
                        zeroNode.Previous.Remove();
                        zeroNode.Remove();
                        didChange = true;
                    }
                }

                // pow(1.1, 2.2) => <the result>
                var functionCalls = rootNode.TheTree.OfType<GlslFunctionCallSyntaxNode>().ToList();
                foreach (var powNode in functionCalls
                    .Where(o => o.Name == "pow" && o.Params.IsNumericCsv())
                    .ToList())
                {
                    var xy = powNode.Params.Children.Where(o => o.Token is FloatToken).Select(o => ((FloatToken)o.Token).Number).ToList();
                    if (xy.Count == 2 && xy.All(o => o > 0.0))
                    {
                        powNode.Params.Remove();
                        powNode.ReplaceWith(new GenericSyntaxNode(FloatToken.From(Math.Pow(xy[0], xy[1]), MaxDp)));
                        didChange = true;
                    }
                }

                // Constant math/trig functions => <the result>
                var mathOp = new List<Tuple<string, Func<double, double>>>
                {
                    new Tuple<string, Func<double, double>>("abs", Math.Abs),
                    new Tuple<string, Func<double, double>>("sqrt", d => Math.Sqrt(Math.Abs(d))),
                    new Tuple<string, Func<double, double>>("sin", Math.Sin),
                    new Tuple<string, Func<double, double>>("cos", Math.Cos),
                    new Tuple<string, Func<double, double>>("tan", Math.Tan),
                    new Tuple<string, Func<double, double>>("asin", Math.Asin),
                    new Tuple<string, Func<double, double>>("acos", Math.Acos),
                    new Tuple<string, Func<double, double>>("atan", Math.Atan),
                    new Tuple<string, Func<double, double>>("radians", x => x / 180.0 * Math.PI),
                    new Tuple<string, Func<double, double>>("degrees", x => x / Math.PI * 180.0),
                    new Tuple<string, Func<double, double>>("sign", x => Math.Sign(x))
                };

                foreach (var mathNode in functionCalls
                    .Where(o => mathOp.Select(op => op.Item1).Contains(o.Name) && o.Params.IsNumericCsv())
                    .ToList())
                {
                    var x = mathNode.Params.Children.Where(o => o.Token is FloatToken).Select(o => ((FloatToken)o.Token).Number).ToList();
                    if (x.Count == 1)
                    {
                        mathNode.Params.Remove();
                        var mathFunc = mathOp.Single(o => o.Item1 == mathNode.Name).Item2;
                        mathNode.ReplaceWith(new GenericSyntaxNode(FloatToken.From(mathFunc(x[0]), MaxDp)));
                        didChange = true;
                    }
                }

                // 'vecN(...) + ...' or '... + vecN(...)'
                foreach (var vectorNode in rootNode.TheTree
                    .OfType<GenericSyntaxNode>()
                    .Where(o => o.Token is TypeToken t && t.IsVector()))
                {
                    // Find the brackets.
                    if (vectorNode.Next is not RoundBracketSyntaxNode brackets)
                        continue;

                    // Brackets must be filled with floats.
                    if (!brackets.IsNumericCsv())
                        continue;

                    // vector then float.
                    {
                        // Find the math symbol.
                        var symbolNode = brackets.Next;
                        if (symbolNode?.Token is SymbolOperatorToken symbol && symbol.GetMathSymbolType() != TokenExtensions.MathSymbolType.Unknown)
                        {
                            // Find the float number.
                            var numberNode = symbolNode.Next;
                            if (numberNode?.Token is FloatToken)
                            {
                                // Number must not be used in a following math operation of a different category.
                                if (numberNode.Next?.Token is not SymbolOperatorToken nextMath ||
                                    nextMath.GetMathSymbolType() == symbol.GetMathSymbolType() ||
                                    nextMath.GetMathSymbolType() == TokenExtensions.MathSymbolType.AddSubtract)
                                {
                                    // Perform math on each bracketed value.
                                    var newCsv =
                                        brackets
                                            .GetCsv()
                                            .Select(o => DoNodeMath(o.Single(), symbolNode, numberNode))
                                            .Select(o => new GenericSyntaxNode(FloatToken.From(o, MaxDp).AsIntIfPossible()));

                                    // Replace bracket content and sum (if it shrinks the code).
                                    var newBrackets = new RoundBracketSyntaxNode(newCsv.ToCsv());
                                    var oldSize = brackets.ToCode().Length + 1 + numberNode.ToCode().Length;
                                    var newSize = newBrackets.ToCode().Length;
                                    if (newSize <= oldSize)
                                    {
                                        brackets.ReplaceWith(newBrackets);
                                        symbolNode.Remove();
                                        numberNode.Remove();

                                        didChange = true;
                                        continue;
                                    }
                                }
                            }
                        }
                    }

                    // float then vector.
                    {
                        // Find the math symbol.
                        var symbolNode = vectorNode.Previous;
                        if (symbolNode?.Token is SymbolOperatorToken symbol && symbol.GetMathSymbolType() != TokenExtensions.MathSymbolType.Unknown)
                        {
                            // Find the float number.
                            var numberNode = symbolNode.Previous;
                            if (numberNode?.Token is FloatToken)
                            {
                                // Number must not be used in a preceding math operation.
                                if (numberNode.Previous?.Token is not SymbolOperatorToken nextMath || nextMath.GetMathSymbolType() != TokenExtensions.MathSymbolType.MultiplyDivide)
                                {
                                    // Perform math on each bracketed value.
                                    var newCsv =
                                        brackets
                                            .GetCsv()
                                            .Select(o => DoNodeMath(numberNode, symbolNode, o.Single()))
                                            .Select(o => new GenericSyntaxNode(FloatToken.From(o, MaxDp).AsIntIfPossible()));

                                    // Replace bracket content and sum (if it shrinks the code).
                                    var newBrackets = new RoundBracketSyntaxNode(newCsv.ToCsv());
                                    var oldSize = brackets.ToCode().Length + 1 + numberNode.ToCode().Length;
                                    var newSize = newBrackets.ToCode().Length;
                                    if (newSize <= oldSize)
                                    {
                                        brackets.ReplaceWith(newBrackets);
                                        symbolNode.Remove();
                                        numberNode.Remove();

                                        didChange = true;
                                        continue;
                                    }
                                }
                            }
                        }
                    }

                    // vector then vector.
                    {
                        // Find the math symbol.
                        var symbolNode = brackets.Next;
                        if (symbolNode?.Token is SymbolOperatorToken symbol && symbol.GetMathSymbolType() != TokenExtensions.MathSymbolType.Unknown)
                        {
                            // Find the second vector.
                            var rhsVectorNode = symbolNode.Next;
                            if (rhsVectorNode?.Token is TypeToken t && t.IsVector())
                            {
                                var rhsVectorBrackets = rhsVectorNode.Next as RoundBracketSyntaxNode;

                                // RHS vector must not be used in a following math operation of a different category.
                                if (rhsVectorBrackets?.Next?.Token is not SymbolOperatorToken nextMath ||
                                    nextMath.GetMathSymbolType() == symbol.GetMathSymbolType() ||
                                    nextMath.GetMathSymbolType() == TokenExtensions.MathSymbolType.AddSubtract)
                                {
                                    // Ensure both brackets have the same number of elements.
                                    var lhsCsv = brackets.GetCsv().ToList();
                                    var rhsCsv = rhsVectorBrackets.GetCsv().ToList();
                                    while (lhsCsv.Count < rhsCsv.Count)
                                        lhsCsv.Add(new[] { lhsCsv.First().Single().Clone() });
                                    while (rhsCsv.Count < lhsCsv.Count)
                                        rhsCsv.Add(new[] { rhsCsv.First().Single().Clone() });

                                    // Perform math on each bracketed value.
                                    var newCsv =
                                        lhsCsv
                                            .Select((o, i) => DoNodeMath(o.Single(), symbolNode, rhsCsv[i].Single()))
                                            .Select(o => new GenericSyntaxNode(FloatToken.From(o, MaxDp).AsIntIfPossible()));

                                    // Replace bracket content and sum.
                                    brackets.ReplaceWith(new RoundBracketSyntaxNode(newCsv.ToCsv()));
                                    symbolNode.Remove();
                                    rhsVectorNode.Remove();
                                    rhsVectorBrackets.Remove();

                                    didChange = true;
                                }
                            }
                        }
                    }
                }

                // Perform simple arithmetic calculations.
                foreach (var numNodeA in rootNode.TheTree
                    .OfType<GenericSyntaxNode>()
                    .Where(
                           o => o.Token is INumberToken &&
                                o.Next?.Token?.GetMathSymbolType() != TokenExtensions.MathSymbolType.Unknown &&
                                o.Next?.Next?.Token is INumberToken)
                    .Reverse()
                    .ToList())
                {
                    var symbolNode = numNodeA.Next;
                    var symbolType = symbolNode.Token.GetMathSymbolType();

                    if (symbolType != TokenExtensions.MathSymbolType.MultiplyDivide &&
                        numNodeA.Previous != null &&
                        numNodeA.Previous.Token is not CommaToken &&
                        numNodeA.Previous.Token?.GetMathSymbolType() != symbolType)
                    {
                        continue;
                    }

                    var numNodeB = symbolNode.Next;
                    if (numNodeB?.Next?.Token is SymbolOperatorToken &&
                        numNodeB.Next.Token.GetMathSymbolType() != TokenExtensions.MathSymbolType.AddSubtract)
                    {
                        continue;
                    }

                    var c = DoNodeMath(numNodeA, symbolNode, numNodeB);

                    var isFloatResult = numNodeA.Token is FloatToken || numNodeB.Token is FloatToken;
                    numNodeB.Remove();
                    symbolNode.Remove();

                    var newNode = numNodeA.ReplaceWith(new GenericSyntaxNode(isFloatResult ? FloatToken.From(c, MaxDp) : new IntToken((int)c)));

                    // If new node is the sole child of a group, promote it.
                    if (newNode.IsOnlyChild() && newNode.Parent.GetType() == typeof(GroupSyntaxNode))
                        newNode.Parent.ReplaceWith(newNode);

                    didChange = true;
                }

                if (!didChange)
                    break;

                repeatSimplifications = true;
            }

            return repeatSimplifications;
        }

        private static double DoNodeMath(SyntaxNode numNodeA, SyntaxNode symbolNode, SyntaxNode numNodeB)
        {
            var a = double.Parse(numNodeA.Token.Content);
            var b = double.Parse(numNodeB.Token.Content);

            var symbol = symbolNode.Token.Content[0];

            // Invert * or / if preceded by a /.
            // E.g. 1.2 / 2.3 * 4.5 = 1.2 / (2.3 / 4.5)
            //                      = 1.2 / 0.51111
            //                      = 2.3478
            if (numNodeA.Previous?.Token?.GetMathSymbolType() == TokenExtensions.MathSymbolType.MultiplyDivide &&
                numNodeA.Previous.HasNodeContent("/"))
            {
                symbol = symbol == '*' ? '/' : '*';
            }

            // Invert + or - if preceded by a -.
            // E.g. -3.0 + 0.1 = - (3.0 - 0.1)
            //                 = - (2.9)
            //                 = -2.9
            if (numNodeA.Previous.HasNodeContent("-") &&
                symbolNode.Token.GetMathSymbolType() == TokenExtensions.MathSymbolType.AddSubtract)
            {
                symbol = symbol == '+' ? '-' : '+';
            }

            switch (symbol)
            {
                case '+':
                    return a + b;
                case '-':
                    return a - b;
                case '*':
                    return a * b;
                case '/':
                    var c = a / b;
                    if (double.IsInfinity(c))
                        c = 0.0;
                    return c;
                default:
                    throw new InvalidOperationException($"Unrecognized math operation '{symbol}'.");
            }
        }
    }
}