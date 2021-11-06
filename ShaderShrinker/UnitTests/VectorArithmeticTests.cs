// -----------------------------------------------------------------------
//  <copyright file="VectorArithmeticTests.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This code is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this code.
//  </summary>
// -----------------------------------------------------------------------

using NUnit.Framework;
using Shrinker.Lexer;
using Shrinker.Parser;

namespace UnitTests
{
    [TestFixture]
    public class VectorArithmeticTests : UnitTestBase
    {
        [Test, Sequential]
        public void CheckArithmeticWithZero(
                    [Values("int i = 1 * 0;",
                    "float o = 1.0 * 0.0;",
                    "float o = (1.0 + 2.0) * 0.0;",
                    "float f() { return 1.0; } void main() { float v = f() * 0.0; }",
                    "float f(out int p) { p = 2; return 1.0; } void main() { int p; float v = f(p) * 0.0; }",
                    "float a = 1.0, v = a * 0.0;",
                    "int a = 1, v = a * 0;",
                    "vec2 v = vec2(1, 2); float a = v.x * 0.0;",
                    "vec2 v = vec2(1, 2); float a = v * 0.0;",
                    "int a[3] = (1, 2, 3); a[0] = 0;")] string code,
                    [Values("int i = 0;",
                    "float o = 0.;",
                    "float o = 0.;",
                    "float f() { return 1.0; } void main() { float v = 0.0; }",
                    "float f(out int p) { p = 2; return 1.0; } void main() { int p; float v = f(p) * 0.0; }",
                    "float a = 1.0, v = 0.0;",
                    "int a = 1, v = 0;",
                    "vec2 v = vec2(1, 2); float a = 0.0;",
                    "vec2 v = vec2(1, 2); float a = 0.0;",
                    "int a[3] = (1, 2, 3); a[0] = 0;")] string expected)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.PerformArithmetic = true;
            options.SimplifyArithmetic = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(expected));
        }

        [Test, Sequential]
        public void CheckArithmeticWithPowFunction(
            [Values("float f = pow(2.0, 4.0);",
                    "float f = pow(2.0, pow(2., 2.));",
                    "float f; f = pow(2.0, pow(2., 2.));",
                    "float f = pow(-1.5, 2.0);",
                    "float f = pow(1.5, -2.0);")] string code,
            [Values("float f = 16.;",
                    "float f = 16.;",
                    "float f; f = 16.;",
                    "float f = pow(-1.5, 2.0);",
                    "float f = pow(1.5, -2.0);")] string expected)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.PerformArithmetic = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(expected));
        }

        [Test, Sequential]
        public void CheckArithmeticWithSignFunction(
            [Values("float f = sign(2.0);",
                    "float f = sign(.0);",
                    "float f = sign(-12.34);")] string code,
            [Values("float f = 1.;",
                    "float f = 0.;",
                    "float f = -1.;")] string expected)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.PerformArithmetic = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(expected));
        }

        [Test, Sequential]
        public void CheckArithmeticWithAbsFunction(
            [Values("float f = abs(2.0);",
                    "float f = abs(-2.2);",
                    "float f = abs(1e3);")] string code,
            [Values("float f = 2.;",
                    "float f = 2.2;",
                    "float f = 1e3;")] string expected)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.PerformArithmetic = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(expected));
        }

        [Test, Sequential]
        public void CheckArithmeticWitSqrtFunction(
            [Values("float f = sqrt(4.0);",
                    "float f = sqrt(-8.0);",
                    "float f; f = sqrt(sqrt(4.));")] string code,
            [Values("float f = 2.;",
                    "float f = 2.82843;",
                    "float f; f = 1.41421;")] string expected)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.PerformArithmetic = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(expected));
        }

        [Test, Sequential]
        public void CheckArithmeticWithTrigFunctions(
            [Values("float f = sin(3.141);",
                    "float f = cos(0.5);",
                    "float f = tan(1.0);",
                    "float f = asin(1.0);",
                    "float f = acos(0.6);",
                    "float f = atan(1.0);",
                    "float f = radians(0.5);",
                    "float f = degrees(90.0);")] string code,
            [Values("float f = 59e-5;",
                    "float f = .87758;",
                    "float f = 1.55741;",
                    "float f = 1.5708;",
                    "float f = .9273;",
                    "float f = .7854;",
                    "float f = .00873;",
                    "float f = 5156.6201;")] string expected)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.PerformArithmetic = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(expected));
        }

        [Test, Sequential]
        public void CheckSingleElementVectorArithmeticSimplifiesToFloat(
            [Values(
                       "vec2 main() { return vec2(1, 2) + vec2(3); }",
                       "vec2 main() { return vec2(1) + vec2(2, 3); }",
                       "vec2 main() { return vec2(1, 2) - vec2(3); }",
                       "vec2 main() { return vec2(1) - vec2(2, 3); }",
                       "vec2 main() { return vec2(1, 2) - vec2(-3); }",
                       "vec2 main() { return vec2(-1) - vec2(2, 3); }",
                       "vec2 main() { return -vec2(-1) - vec2(2, 3); }",
                       "vec2 main() { return vec2(1, 2) * vec2(3); }",
                       "vec2 main() { return vec2(1) * vec2(2, 3); }",
                       "vec2 main() { return vec2(1, 2) / vec2(3); }",
                       "vec2 main() { return vec2(1) / vec2(2, 3); }",
                       "vec2 main() { return vec2(1) + vec2(2); }",
                       "vec4 main() { return vec4(1) + vec4(2) * vec4(3.0); }"
                   )] string code,
            [Values(
                       "vec2 main() { return vec2(1, 2) + 3.; }",
                       "vec2 main() { return 1. + vec2(2, 3); }",
                       "vec2 main() { return vec2(1, 2) - 3.; }",
                       "vec2 main() { return 1. - vec2(2, 3); }",
                       "vec2 main() { return vec2(1, 2) + 3.; }",
                       "vec2 main() { return -1. - vec2(2, 3); }",
                       "vec2 main() { return 1. - vec2(2, 3); }",
                       "vec2 main() { return vec2(1, 2) * 3.; }",
                       "vec2 main() { return 1. * vec2(2, 3); }",
                       "vec2 main() { return vec2(1, 2) / 3.; }",
                       "vec2 main() { return 1. / vec2(2, 3); }",
                       "vec2 main() { return 1. + vec2(2); }",
                       "vec4 main() { return 1. + 2. * vec4(3.0); }"
                   )] string expected)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.SimplifyArithmetic = true;

            var rootNode = new Parser(lexer).Parse().Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(expected));
        }

        [Test, Sequential]
        public void CheckArithmeticWithVectorAndScalar(
            [Values("vec2 f = vec2(1.1, 2.2) + 3.3;",
                    "vec3 f = vec3(1.1, 2.2, 3.3) - 4.4;",
                    "vec4 f = vec4(1.1) * 2.2;",
                    "vec4 f = vec4(10) / 2.0;",
                    "vec4 f = vec4(22, 23, 24) / 7.0;",
                    "void f(vec2 v) { } void main() { f(vec2(1.1, 2.2) + 3.3); }",
                    "vec2 f = vec2(1.1, 2.2) + 3.3 * 4.4;",
                    "float i = 2.0; vec2 f = vec2(1.1, 2.2) + 3.3 * i;",
                    "vec4 f = vec4(1.1) * 2.2 + 3.3;",
                    "vec2 main() { return vec2(1, 2) * 3.; }",
                    "vec2 main() { return vec2(1, 2) / 0.5; }",
                    "vec2 main() { return vec2(1, 2) * 3. + 1.; }",
                    "vec2 main() { return vec2(1, 2) / 0.5 - 2.; }",
                    "vec2 main() { return 3. + vec2(1, 2); }",
                    "vec2 main() { return 3. * vec2(1, 2) * 3.; }",
                    "vec2 main() { return 2. / vec2(1, 2); }",
                    "vec2 main() { return 1. + 3. * vec2(1, 2); }",
                    "vec2 main() { return 1. + vec2(1, 2) * 3.; }",
                    "vec2 main() { return vec2(21, 22) / 7.; }",
                    "vec3 main() { vec3 v = vec3(.4); float k = 1.; return (v + vec3(.6, .8, 1) * .5 * k) / (1. + k); }")] string code,
            [Values("vec2 f = vec2(4.4, 5.5);",
                    "vec3 f = vec3(-3.3, -2.2, -1.1);",
                    "vec4 f = vec4(2.42);",
                    "vec4 f = vec4(5);",
                    "vec4 f = vec4(22, 23, 24) / 7.0;",
                    "void f(vec2 v) { } void main() { f(vec2(4.4, 5.5)); }",
                    "vec2 f = vec2(15.62, 16.72);",
                    "float i = 2.0; vec2 f = vec2(1.1, 2.2) + 3.3 * i;",
                    "vec4 f = vec4(5.72);",
                    "vec2 main() { return vec2(3, 6); }",
                    "vec2 main() { return vec2(2, 4); }",
                    "vec2 main() { return vec2(4, 7); }",
                    "vec2 main() { return vec2(0, 2); }",
                    "vec2 main() { return vec2(4, 5); }",
                    "vec2 main() { return vec2(9, 18); }",
                    "vec2 main() { return vec2(2, 1); }",
                    "vec2 main() { return vec2(4, 7); }",
                    "vec2 main() { return vec2(4, 7); }",
                    "vec2 main() { return vec2(21, 22) / 7.; }",
                    "vec3 main() { vec3 v = vec3(.4); float k = 1.; return (v + vec3(.3, .4, .5) * k) / (1. + k); }")] string expected)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.PerformArithmetic = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(expected));
        }

        // todo - vec and vec
    }
}