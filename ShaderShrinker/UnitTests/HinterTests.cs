// -----------------------------------------------------------------------
//  <copyright file="HinterTests.cs" company="Global Graphics Software Ltd">
//      Copyright (c) 2021 Global Graphics Software Ltd. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Global Graphics Software Ltd. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Linq;
using NUnit.Framework;
using Shrinker.Lexer;
using Shrinker.Parser;

namespace UnitTests
{
    [TestFixture]
    public class HinterTests
    {
        [Test]
        public void CheckDetectingRemoveToInline()
        {
            var lexer = new Lexer();
            lexer.Load("void f() { } void main() { return; }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(() => rootNode.GetHints().OfType<Hinter.UnusedFunctionHint>().ToList(), Has.Count.EqualTo(1));
        }

        [Test]
        public void CheckDetectingFunctionToInline()
        {
            var lexer = new Lexer();
            lexer.Load("void f() { } void main() { f(); }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(() => rootNode.GetHints().OfType<Hinter.FunctionToInlineHint>().ToList(), Has.Count.EqualTo(1));
        }

        [Test, Sequential]
        public void CheckDetectingFunctionWithUnusedParam(
            [Values(
                       "int f(int a, int b) { return a * 2; } void main() { f(1, 2); }",
                       "float f(vec2 a, vec2 b) { return a.x * 2.0; } void main() { f(vec2(1), vec2(2)); }",
                       "void f(inout T a, int b) { a.a = 2.0; a.b = 1.0; } void main() { T m; f(m, 1); }")
            ]
            string code)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var rootNode = new Parser(lexer).Parse();

            var hints = rootNode.GetHints().OfType<Hinter.FunctionHasUnusedParam>().ToList();
            Assert.That(hints, Has.Count.EqualTo(1));
            Assert.That(() => hints.Single().Suggestion, Does.Contain("'b'"));
        }
    }
}