// -----------------------------------------------------------------------
//  <copyright file="HinterTests.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using System.Linq;
using NUnit.Framework;
using Shrinker.Lexer;
using Shrinker.Parser;
using Shrinker.Parser.Hints;

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

            Assert.That(rootNode.GetHints().OfType<UnusedFunctionHint>().ToList(), Has.Count.EqualTo(1));
        }

        [Test]
        public void CheckDetectingFunctionToInline()
        {
            var lexer = new Lexer();
            lexer.Load("void f() { } void main() { f(); }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(rootNode.GetHints().OfType<FunctionToInlineHint>().ToList(), Has.Count.EqualTo(1));
        }

        [Test]
        public void CheckInlineHintNotGivenForMainFunctions()
        {
            var lexer = new Lexer();
            lexer.Load("vec3 mainImage() { return vec3(0); } vec3 main() { return mainImage(); }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(rootNode.GetHints().OfType<FunctionToInlineHint>().ToList(), Is.Empty);
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

            var hints = rootNode.GetHints().OfType<FunctionHasUnusedParamHint>().ToList();
            Assert.That(hints, Has.Count.EqualTo(1));
            Assert.That(hints.Single().Suggestion, Does.Contain("'b'"));
        }

        [Test]
        public void CheckSingleUseResolutionDoesNotTriggerHint()
        {
            var lexer = new Lexer();
            lexer.Load("float main() { return iResolution.x; }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(rootNode.GetHints(), Is.Empty);
        }

        [Test]
        public void CheckMultiUseResolutionTriggersHint()
        {
            var lexer = new Lexer();
            lexer.Load("float main() { return iResolution.x + iResolution.y + iResolution.y; }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(rootNode.GetHints().OfType<IntroduceDefineHint>().ToList(), Has.Count.EqualTo(1));
        }

        [Test]
        public void CheckSingleUseSmoothstepDoesNotTriggerHint()
        {
            var lexer = new Lexer();
            lexer.Load("float main() { return smoothstep(0.0, 1.0, 0.5); }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(rootNode.GetHints(), Is.Empty);
        }

        [Test]
        public void CheckMultiUseSmoothstepTriggersHint()
        {
            var lexer = new Lexer();
            lexer.Load("float main() { return smoothstep(0.0, 1.0, smoothstep(0.0, 1.0, smoothstep(0.0, 1.0, 0.5))); }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(rootNode.GetHints().OfType<IntroduceDefineHint>().ToList(), Has.Count.EqualTo(1));
        }

        [Test]
        public void CheckSingleUseMouseDoesNotTriggerHint()
        {
            var lexer = new Lexer();
            lexer.Load("float main() { return iMouse.x; }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(rootNode.GetHints(), Is.Empty);
        }

        [Test]
        public void CheckMultiUseMouseTriggersHint()
        {
            var lexer = new Lexer();
            lexer.Load("float main() { return iMouse.x + iMouse.y * iMouse.z; }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(rootNode.GetHints().OfType<IntroduceDefineHint>().ToList(), Has.Count.EqualTo(1));
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

            Assert.That(rootNode.GetHints().OfType<FunctionCalledWithAllConstParamsHint>(), Is.Empty);
        }

        [Test]
        public void CheckCallingFunctionWithConstParamUsingGlobalDoesNotTriggerHint()
        {
            var lexer = new Lexer();
            lexer.Load("int g = 0; int f(int a) { return g + a; } int main() { return f(2); }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(rootNode.GetHints().OfType<FunctionCalledWithAllConstParamsHint>(), Is.Empty);
        }

        [Test]
        public void CheckCallingFunctionWithConstParamUsingTimeDoesNotTriggerHint()
        {
            var lexer = new Lexer();
            lexer.Load("float f(float a) { return a + iTime; } float main() { return f(2.0); }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(rootNode.GetHints().OfType<FunctionCalledWithAllConstParamsHint>(), Is.Empty);
        }

        [Test]
        public void CheckCallingFunctionWithNonNumericVectorDoesNotTriggerHint()
        {
            var lexer = new Lexer();
            lexer.Load("vec3 f(vec3 a) { return a * 2.; } vec3 main() { vec3 v = vec3(1); return f(v); }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(rootNode.GetHints().OfType<FunctionCalledWithAllConstParamsHint>(), Is.Empty);
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

            Assert.That(rootNode.GetHints().OfType<FunctionCalledWithAllConstParamsHint>().ToList(), Has.Count.EqualTo(1));
        }

        [Test]
        public void CheckCallingFunctionMultipleTimesWithCommonConstFloatsTriggersHint()
        {
            var lexer = new Lexer();
            lexer.Load("float f(float a, float b) { return a + b; } float main() { return f(1.0, 2.0) + f(3.0, 2.0); }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(rootNode.GetHints().OfType<AllCallsToFunctionMadeWithSameParamHint>().ToList(), Has.Count.EqualTo(1));
        }

        [Test]
        public void CheckCallingFunctionMultipleTimesWithCommonConstIntegerTriggersHint()
        {
            var lexer = new Lexer();
            lexer.Load("int f(int a, int b) { return a + b; } int main() { return f(1, 2) + f(3, 2); }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(rootNode.GetHints().OfType<AllCallsToFunctionMadeWithSameParamHint>().ToList(), Has.Count.EqualTo(1));
        }

        [Test]
        public void CheckCallingFunctionMultipleTimesWithCommonConstVectorTriggersHint()
        {
            var lexer = new Lexer();
            lexer.Load("void f(int a, int b) { return a + b; } int main() { return f(vec2(1), 2) + f(vec2(1), 3); }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(rootNode.GetHints().OfType<AllCallsToFunctionMadeWithSameParamHint>().ToList(), Has.Count.EqualTo(1));
        }

        [Test]
        public void CheckCallingFunctionMultipleTimesWithNoCommonConstIntegersDoesNotTriggerHint()
        {
            var lexer = new Lexer();
            lexer.Load("int f(int a, int b) { return a + b; } int main() { return f(1, 2) + f(3, 4); }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(rootNode.GetHints().OfType<AllCallsToFunctionMadeWithSameParamHint>(), Is.Empty);
        }

        [Test]
        public void CheckCallingFunctionMultipleTimesWithNoCommonConstVectorDoesNotTriggerHint()
        {
            var lexer = new Lexer();
            lexer.Load("void f(int a, int b) { return a + b; } int main() { return f(vec2(1), 2) + f(vec2(2), 3); }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(rootNode.GetHints().OfType<AllCallsToFunctionMadeWithSameParamHint>(), Is.Empty);
        }

        [Test]
        public void CheckCallingFunctionOnceWithConstIntegersTriggersHint()
        {
            var lexer = new Lexer();
            lexer.Load("int f(int a, int b) { return a + b; } int main() { return f(1, 1); }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(rootNode.GetHints().OfType<AllCallsToFunctionMadeWithSameParamHint>().ToList(), Has.Count.EqualTo(2));
        }

        [Test]
        public void CheckValidClampDoesNotTriggerHint()
        {
            var lexer = new Lexer();
            lexer.Load("void main(float f) { clamp(f, 0.0, 1.0); }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(rootNode.GetHints().OfType<InvalidClampHint>(), Is.Empty);
        }
        
        [Test]
        public void CheckInvalidClampTriggersHint()
        {
            var lexer = new Lexer();
            lexer.Load("void main(float f) { clamp(0.0, 1.0, f); }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(rootNode.GetHints().OfType<InvalidClampHint>().ToList(), Has.Count.EqualTo(1));
        }
        
        [Test, Sequential]
        public void CheckValidPowerArgumentDoesNotTriggerHint([Values(0.0, 1.2)] double powerArg)
        {
            var lexer = new Lexer();
            lexer.Load($"void main(float f) {{ pow(f, {powerArg}); }}");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(rootNode.GetHints().OfType<NegativePowHint>(), Is.Empty);
        }
        
        [Test]
        public void CheckInvalidPowerArgumentTriggersHint()
        {
            var lexer = new Lexer();
            lexer.Load("void main(float f) { pow(-1.2, f); }");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(rootNode.GetHints().OfType<NegativePowHint>().ToList(), Has.Count.EqualTo(1));
        }
        
        [Test, Sequential]
        public void CheckValidSqrtArgumentDoesNotTriggerHint([Values(0.5, 1.2)] double powerArg)
        {
            var lexer = new Lexer();
            lexer.Load($"void main(float f) {{ sqrt({powerArg}); }}");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(rootNode.GetHints().OfType<NegativeSqrtHint>(), Is.Empty);
        }
        
        [Test, Sequential]
        public void CheckInvalidSqrtArgumentTriggersHint([Values(-0.5, 0.0)] double powerArg)
        {
            var lexer = new Lexer();
            lexer.Load($"void main(float f) {{ sqrt({powerArg}); }}");

            var rootNode = new Parser(lexer).Parse();

            Assert.That(rootNode.GetHints().OfType<NegativeSqrtHint>().ToList(), Has.Count.EqualTo(1));
        }
    }
}