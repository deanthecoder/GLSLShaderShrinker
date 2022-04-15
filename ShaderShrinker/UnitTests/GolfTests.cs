// -----------------------------------------------------------------------
//  <copyright file="GolfTests.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
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
using Shrinker.Parser.SyntaxNodes;

namespace UnitTests
{
    [TestFixture]
    public class GolfTests
    {
        [Test, Sequential]
        public void CheckFindingUserDefinedNames(
            [Values(
                       "int a;|a",
                       "void func() {int i = 2;}|func,i",
                       "#define FOO|FOO",
                       "#define VALUE 1.0|VALUE",
                       "#define F(x) x*2.0|F",
                       "void f(){\\n#define F\\n}|f,F"
                   )]
            string codeAndNames)
        {
            var code = codeAndNames.Split('|')[0];
            var names = codeAndNames.Split('|')[1].Split(',');

            var lexer = new Lexer();
            lexer.Load(code);

            var foundNames = new Parser(lexer).Parse().FindUserDefinedNames().ToList();
            Assert.That(foundNames, Is.EqualTo(names));
        }

        [Test, Sequential]
        public void GivenFunctionMainCheckFindingUserDefinedNamesReturnsEmptyList([Values("main", "mainImage", "mainSound", "mainVR")] string main)
        {
            var lexer = new Lexer();
            lexer.Load($"void {main}() {{ }}");

            var foundNames = new Parser(lexer).Parse().FindUserDefinedNames().ToList();
            Assert.That(foundNames, Is.Empty);
        }

        [Test]
        public void CheckGolfingCodeNames(
            [Values(
                       "void main() { }|void main() { }",
                       "void foo() { }|void f() { }",
                       "void foo() { } void bar() { }|void f() { } void b() { }",
                       "void foo1() { } void foo2() { }|void f() { } void a() { }",
                       "void foo() { } void main() { foo(); }|void f() { } void main() { f(); }",
                       "void foo() { int a = 1; }|void f() { int a = 1; }",
                       "void foo() { int _a = 1; }|void f() { int a = 1; }",
                       "void main() { int number = 1; }|void main() { int n = 1; }",
                       "void mainImage() { int number = 1; }|void mainImage() { int n = 1; }",
                       "int foo() { int number = 1; number++; return number; }|int f() { int n = 1; n++; return n; }",
                       "int foo() { int number = 1; return ++number; }|int f() { int n = 1; return ++n; }",
                       "int foo() { int number = 1; return number++; }|int f() { int n = 1; return n++; }",
                       "int foo(int var) { int n = 1, sum = n + var; return sum; }|int f(int v) { int n = 1, s = n + v; return s; }",
                       "void foo(); void main() { foo(); } void foo() { }|void f(); void main() { f(); } void f() { }",
                       "void f(int a, int aa, int apple) { return a + aa + apple; }|void f(int a, int b, int c) { return a + b + c; }",
                       "float f() { vec2 vector = vec2(1, 2); return vector.x; }|float f() { vec2 v = vec2(1, 2); return v.x; }",
                       "void f() { vec2 vector; vector.x = 1.; }|void f() { vec2 v; v.x = 1.; }",
                       "void f(float n) { } void g() { vec2 vector = vec2(1); f(vector.x); }|void f(float n) { } void g() { vec2 v = vec2(1); f(v.x); }",
                       "void f(vec2 a) { } void g() { vec2 vector = vec2(1); f(vector); }|void f(vec2 a) { } void g() { vec2 v = vec2(1); f(v); }",
                       "struct S { int num; }; void f() { int num; S s; s.num = 1; }|struct S { int num; }; void f() { int n; S s; s.num = 1; }",
                       "#define NUM 1.0\nfloat f() { return NUM; }|#define N 1.0 float f() { return N; }"
                   )]
            string codeAndGolfed)
        {
            var code = codeAndGolfed.Split('|')[0];
            var expected = codeAndGolfed.Split('|')[1];

            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.GolfNames = true;

            var rootNode = new Parser(lexer).Parse().Simplify(options);
            var golfed = rootNode.ToCode().ToSimple();
            Assert.That(golfed, Is.EqualTo(expected));
        }

        [Test]
        public void CheckGolfingCommonTerms(
            [Values(
                       "void f() { vec3 a; vec3 b; }|void f() { vec3 a; vec3 b; }",
                       "void f(vec3 q) { const vec3 a = vec3(1); vec3 b; vec3 c; vec3 d; vec3 e; vec3 f; vec3 g; vec3 h; vec3 i; }|#define v3 vec3 void f(v3 q) { const v3 a = v3(1); v3 b; v3 c; v3 d; v3 e; v3 f; v3 g; v3 h; v3 i; }",
                       "float f(float a, float b) { return smoothstep(0.0, 1.0, a) + smoothstep(0.5, 1.5, b) + smoothstep(0.0, 1.0, a + b); }|#define S smoothstep float f(float a, float b) { return S(0.0, 1.0, a) + S(0.5, 1.5, b) + S(0.0, 1.0, a + b); }"
                   )]
            string codeAndGolfed)
        {
            var code = codeAndGolfed.Split('|')[0];
            var expected = codeAndGolfed.Split('|')[1];

            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.GolfDefineCommonTerms = true;

            var rootNode = new Parser(lexer).Parse().Simplify(options);
            var golfed = rootNode.ToCode().ToSimple();
            Assert.That(golfed, Is.EqualTo(expected));
        }
    }
}