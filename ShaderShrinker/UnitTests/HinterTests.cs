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

            var hints = rootNode.GetHints().OfType<Hinter.FunctionHasUnusedParamHint>().ToList();
            Assert.That(hints, Has.Count.EqualTo(1));
            Assert.That(() => hints.Single().Suggestion, Does.Contain("'b'"));
        }

        [Test]
        public void CheckSingleUseResolutionDoesNotTriggerHint()
        {
            var lexer = new Lexer();
            lexer.Load("float main() { return iResolution.x; }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(() => rootNode.GetHints(), Is.Empty);
        }

        [Test]
        public void CheckMultiUseResolutionTriggersHint()
        {
            var lexer = new Lexer();
            lexer.Load("float main() { return iResolution.x + iResolution.y + iResolution.y; }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(() => rootNode.GetHints().OfType<Hinter.IntroduceDefineHint>().ToList(), Has.Count.EqualTo(1));
        }

        [Test]
        public void CheckSingleUseSmoothstepDoesNotTriggerHint()
        {
            var lexer = new Lexer();
            lexer.Load("float main() { return smoothstep(0.0, 1.0, 0.5); }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(() => rootNode.GetHints(), Is.Empty);
        }

        [Test]
        public void CheckMultiUseSmoothstepTriggersHint()
        {
            var lexer = new Lexer();
            lexer.Load("float main() { return smoothstep(0.0, 1.0, smoothstep(0.0, 1.0, smoothstep(0.0, 1.0, 0.5))); }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(() => rootNode.GetHints().OfType<Hinter.IntroduceDefineHint>().ToList(), Has.Count.EqualTo(1));
        }

        [Test]
        public void CheckSingleUseMouseDoesNotTriggerHint()
        {
            var lexer = new Lexer();
            lexer.Load("float main() { return iMouse.x; }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(() => rootNode.GetHints(), Is.Empty);
        }

        [Test]
        public void CheckMultiUseMouseTriggersHint()
        {
            var lexer = new Lexer();
            lexer.Load("float main() { return iMouse.x + iMouse.y * iMouse.z; }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(() => rootNode.GetHints().OfType<Hinter.IntroduceDefineHint>().ToList(), Has.Count.EqualTo(1));
        }

        [Test, Sequential]
        public void CheckCallingVoidFunctionWithConstParamDoesNotTriggerHint(
            [Values(
                       "void f(int a) { } void main() { f(1); }",
                       "void f(float a) { } void main() { f(1.0); }",
                       "void f(vec3 a) { } void main() { f(vec3(1)); }")
            ]
            string code)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var rootNode = new Parser(lexer).Parse();

            Assert.That(() => rootNode.GetHints().OfType<Hinter.FunctionCalledWithConstParamsHint>(), Is.Empty);
        }

        [Test]
        public void CheckCallingFunctionWithConstParamUsingGlobalDoesNotTriggerHint()
        {
            var lexer = new Lexer();
            lexer.Load("int g = 0; int f(int a) { return g + a; } int main() { return f(2); }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(() => rootNode.GetHints().OfType<Hinter.FunctionCalledWithConstParamsHint>(), Is.Empty);
        }

        [Test]
        public void CheckCallingFunctionWithConstParamUsingTimeDoesNotTriggerHint()
        {
            var lexer = new Lexer();
            lexer.Load("float f(float a) { return a + iTime; } float main() { return f(2.0); }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(() => rootNode.GetHints().OfType<Hinter.FunctionCalledWithConstParamsHint>(), Is.Empty);
        }
        
        [Test]
        public void CheckCallingFunctionWithNonNumericVectorDoesNotTriggerHint()
        {
            var lexer = new Lexer();
            lexer.Load("vec3 f(vec3 a) { return a * 2.; } vec3 main() { vec3 v = vec3(1); return f(v); }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(() => rootNode.GetHints().OfType<Hinter.FunctionCalledWithConstParamsHint>(), Is.Empty);
        }

        [Test]
        public void CheckCallingFunctionWithConstParamTriggersHint(
            [Values(
                       "int f(int a) { return a * 2; } int main() { return f(2); }",
                       "vec3 f(vec3 a) { return a * 2.; } vec3 main() { return f(vec3(1, 2, 3)); }",
                       "vec3 f(vec3 a) { return a * 2.; } vec3 main() { return f(vec3(1.1)); }")
            ]
            string code)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var rootNode = new Parser(lexer).Parse();

            Assert.That(() => rootNode.GetHints().OfType<Hinter.FunctionCalledWithConstParamsHint>().ToList(), Has.Count.EqualTo(1));
        }
    }
}