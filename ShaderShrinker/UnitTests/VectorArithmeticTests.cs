// -----------------------------------------------------------------------
//  <copyright file="VectorArithmeticTests.cs">
//      Copyright (c) 2021 Dean Edis. (Twitter: @deanthecoder) All rights reserved.
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
        public void CheckArithmeticWithSqrtFunction(
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
        public void CheckArithmeticWithLengthFunction(
            [Values("float f = length(12.34);",
                    "float f = length(-12.34);",
                    "float f = length(vec2(3, 4));",
                    "float f = length(vec3(9));")] string code,
            [Values("float f = 12.34;",
                    "float f = 12.34;",
                    "float f = 5.;",
                    "float f = 15.58846;")] string expected)
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
        public void CheckArithmeticWithNormalizeFunction(
            [Values("float f = normalize(vec2(3, 4));",
                    "float f = normalize(vec3(9));")] string code,
            [Values("float f = vec2(.6, .8);",
                    "float f = vec3(.57735);")] string expected)
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
        public void CheckArithmeticWithClampFunction(
            [Values("float f = clamp(12.34, 0.0, 20.0);",
                    "float f = clamp(12.34, 15.0, 20.0);",
                    "float f = clamp(12.34, 0.0, 10.0);")] string code,
            [Values("float f = 12.34;",
                    "float f = 15.;",
                    "float f = 10.;")] string expected)
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

        [Test]
        public void CheckArithmeticWithFloorFunction()
        {
            var lexer = new Lexer();
            lexer.Load("float f = floor(12.3);");

            var options = CustomOptions.None();
            options.PerformArithmetic = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("float f = 12.;"));
        }

        [Test]
        public void CheckArithmeticWithCeilFunction()
        {
            var lexer = new Lexer();
            lexer.Load("float f = ceil(12.3);");

            var options = CustomOptions.None();
            options.PerformArithmetic = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("float f = 13.;"));
        }

        [Test]
        public void CheckArithmeticWithTruncFunction()
        {
            var lexer = new Lexer();
            lexer.Load("float f = trunc(12.9);");

            var options = CustomOptions.None();
            options.PerformArithmetic = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("float f = 12.;"));
        }

        [Test]
        public void CheckArithmeticWithFractFunction()
        {
            var lexer = new Lexer();
            lexer.Load("float f = fract(12.9);");

            var options = CustomOptions.None();
            options.PerformArithmetic = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("float f = .9;"));
        }

        [Test, Sequential]
        public void CheckArithmeticWithSingleIndexDotFunction(
            [Values("vec3 v; float f = dot(vec3(0, 0, 1), v);|vec3 v; float f = v.z;",
                    "vec3 v; float f = dot(vec3(0, 1, 0), v);|vec3 v; float f = v.y;",
                    "vec3 v; float f = dot(vec3(1, 0, 0), v);|vec3 v; float f = v.x;",
                    "vec3 v; float f = dot(v, vec3(0, 1, 0));|vec3 v; float f = v.y;",
                    "vec2 v; float f = dot(v, vec2(1, 0));|vec2 v; float f = v.x;",
                    "float f = dot(vec2(1, 2), vec2(3, 4));|float f = 11.;",
                    "float f = dot(vec2(2), vec2(3, 4));|float f = 14.;",
                    "vec2 v; float f = dot(v + v, vec2(1, 0));|vec2 v; float f = dot(v + v, vec2(1, 0));")] string code)
        {
            var input = code.Split('|')[0];
            var expected = code.Split('|')[1];

            var lexer = new Lexer();
            lexer.Load(input);

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
                    "vec3 main() { vec3 v = vec3(.4); float k = 1.; return (v + vec3(.6, .8, 1) * .5 * k) / (1. + k); }",
                    "p.xy -= 5. * vec2(0.64, 0.7) - 2.5;")] string code,
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
                    "vec3 main() { vec3 v = vec3(.4); float k = 1.; return (v + vec3(.3, .4, .5) * k) / (1. + k); }",
                    "p.xy -= vec2(.7, 1);")] string expected)
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
        public void CheckArithmeticWithVectorAndVector(
            [Values("vec2 f = vec2(1.1, 2.2) + vec2(1, 2);",
                    "vec2 f = vec2(1.1, 2.2) + vec2(1, 2) + vec2(6, 7);",
                    "vec2 f = vec2(1.1, 2.2) + vec2(1, 2) + vec2(5);",
                    "vec2 f = vec2(1.1, 2.2) + vec2(1, 2) + 2.0;",
                    "vec2 f = vec2(1.1, 2.2) + vec2(1, 2) * vec2(6, 7);",
                    "vec2 f = vec2(1.1, 2.2) + vec2(1, 2) * vec2(5);",
                    "vec2 f = vec2(1.1, 2.2) + vec2(1, 2) * 2.0;",
                    "vec2 f = vec2(1.1, 2.2) * vec2(1, 2);",
                    "vec2 f = vec2(1.1, 2.2) * vec2(1, 2) + vec2(6, 7);",
                    "vec2 f = vec2(1.1, 2.2) * vec2(1, 2) + vec2(5);",
                    "vec2 f = vec2(1.1, 2.2) * vec2(1, 2) + 2.0;",
                    "vec2 f = vec2(1.1, 2.2) * vec2(1, 2) * vec2(6, 7);",
                    "vec2 f = vec2(1.1, 2.2) * vec2(1, 2) * vec2(5);",
                    "vec2 f = vec2(1.1, 2.2) * vec2(1, 2) * 2.0;",
                    "vec2 f = vec2(3) + vec2(4);",
                    "vec2 f = vec2(3, 4) + vec2(5);",
                    "vec2 f = vec2(3) + vec2(4, 5);",
                    "vec2 v = vec2(1, 2), f = v * vec2(1.1, 2.2) + vec2(6, 7);",
                    "vec2 v = vec2(1, 2), f = vec2(1.1, 2.2) + vec2(6, 7) * v;",
                    "vec2 f = vec2(3, 4) + vec2(1. * 2., 3);")] string code,
            [Values("vec2 f = vec2(2.1, 4.2);",
                    "vec2 f = vec2(8.1, 11.2);",
                    "vec2 f = vec2(7.1, 9.2);",
                    "vec2 f = vec2(4.1, 6.2);",
                    "vec2 f = vec2(7.1, 16.2);",
                    "vec2 f = vec2(6.1, 12.2);",
                    "vec2 f = vec2(3.1, 6.2);",
                    "vec2 f = vec2(1.1, 4.4);",
                    "vec2 f = vec2(7.1, 11.4);",
                    "vec2 f = vec2(6.1, 9.4);",
                    "vec2 f = vec2(3.1, 6.4);",
                    "vec2 f = vec2(6.6, 30.8);",
                    "vec2 f = vec2(5.5, 22);",
                    "vec2 f = vec2(2.2, 8.8);",
                    "vec2 f = vec2(7);",
                    "vec2 f = vec2(8, 9);",
                    "vec2 f = vec2(7, 8);",
                    "vec2 v = vec2(1, 2), f = v * vec2(1.1, 2.2) + vec2(6, 7);",
                    "vec2 v = vec2(1, 2), f = vec2(1.1, 2.2) + vec2(6, 7) * v;",
                    "vec2 f = vec2(5, 7);")] string expected)
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

        // todo - normalize(vec3(-20, -10.5, 25))
    }
}