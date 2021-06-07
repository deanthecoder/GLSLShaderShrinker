// -----------------------------------------------------------------------
//  <copyright file="ShrinkerTests.cs">
//      Copyright (c) 2021 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  We do not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using NUnit.Framework;
using Shrinker.Lexer;
using Shrinker.Parser;

namespace UnitTests
{
    [TestFixture]
    public class ShrinkerTests : UnitTestBase
    {
        [Test]
        public void CheckJoiningSingleConstDeclarationAndDefinition()
        {
            var lexer = new Lexer();
            lexer.Load("const int i; i = 10;");

            var options = CustomOptions.None();
            options.JoinVariableDeclarationsWithAssignments = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("const int i = 10;"));
        }
        
        [Test]
        public void CheckGlobalConstDeclarationsArePositionedAfterFileHeaderCommentsButBeforeDefines()
        {
            var lexer = new Lexer();
            lexer.Load("// Header\n#define FOO\nconst vec2 uv = vec2(1);");

            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(CustomOptions.None());

            Assert.That(rootNode.ToCode().Trim(), Is.EqualTo("// Header\n#define FOO\n\nconst vec2 uv = vec2(1);"));
        }

        [Test]
        public void CheckJoiningSingleDeclarationAndDefinition()
        {
            var lexer = new Lexer();
            lexer.Load("vec2 uv; uv = vec2(1);");

            var options = CustomOptions.None();
            options.JoinVariableDeclarationsWithAssignments = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("vec2 uv = vec2(1);"));
        }
        
        [Test]
        public void GivenKnownFunctionCallSettingVariableFollowedByExplicitAssignmentCheckJoiningAssignmentToDeclarationDoesNotOccur()
        {
            const string Code = "void setA(out int a) { a = 1; } void main() { int a; setA(a); a = 2; }";

            var lexer = new Lexer();
            lexer.Load(Code);

            var options = CustomOptions.None();
            options.JoinVariableDeclarationsWithAssignments = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(Code));
        }

        [Test]
        public void GivenUnknownFunctionCallSettingVariableFollowedByExplicitAssignmentCheckJoiningAssignmentToDeclarationDoesNotOccur()
        {
            const string Code = "void main() { int a; setA(a); a = 2; }";

            var lexer = new Lexer();
            lexer.Load(Code);

            var options = CustomOptions.None();
            options.JoinVariableDeclarationsWithAssignments = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(Code));
        }

        [Test]
        public void CheckConstDeclarationsAreGrouped()
        {
            var lexer = new Lexer();
            lexer.Load("int foo; const int z; vec3 bar; const int a;");

            var options = CustomOptions.None();
            options.GroupVariableDeclarations = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("const int z, a; int foo; vec3 bar;"));
        }

        [Test]
        public void CheckConstDeclarationsWithAssignmentsAreGrouped()
        {
            var lexer = new Lexer();
            lexer.Load("const int a = 1; const int b = 2;");

            var options = CustomOptions.None();
            options.GroupVariableDeclarations = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("const int a = 1, b = 2;"));
        }

        [Test]
        public void GivenVariableNameMatchingFunctionNameCheckDeclarationIsNotSplitFromDefinition()
        {
            const string Code = "int f() { return 1; } void main() { float a; int b; float f = f(); }";

            var lexer = new Lexer();
            lexer.Load(Code);

            var options = CustomOptions.None();
            options.GroupVariableDeclarations = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(Code));
        }

        [Test]
        public void CheckJoiningConsecutiveConstDeclarationAndDefinitions()
        {
            var lexer = new Lexer();
            lexer.Load("const int i; const int j; i = 10; j = 11;");

            var options = CustomOptions.None();
            options.JoinVariableDeclarationsWithAssignments = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("const int i = 10; const int j = 11;"));
        }

        [Test, Sequential]
        public void CheckGroupingAndJoiningConsecutiveConstDeclarationAndDefinitions([Values("const int i; const int j; i = 10; j = 11;", "const int i; const int j; j = 11; i = 10;")] string code)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.GroupVariableDeclarations = true;
            options.JoinVariableDeclarationsWithAssignments = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("const int i = 10, j = 11;"));
        }

        [Test]
        public void CheckJoiningConsecutiveNonConstDeclarationAndDefinitions()
        {
            var lexer = new Lexer();
            lexer.Load("int c, a, b; c = 3; b = 2; a = 1;");

            var options = CustomOptions.None();
            options.GroupVariableDeclarations = true;
            options.JoinVariableDeclarationsWithAssignments = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("int a, c = 3, b = 2; a = 1;"));
        }
        
        [Test]
        public void CheckJoiningMixedDeclarationsAndDefinitions()
        {
            var lexer = new Lexer();
            lexer.Load("{ float a; int b; b = 1; a = 1.0; }");

            var options = CustomOptions.None();
            options.GroupVariableDeclarations = true;
            options.JoinVariableDeclarationsWithAssignments = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("{ int b = 1; float a = 1.0; }"));
        }

        [Test]
        public void CheckJoiningDeclarationsAndDefinitionsWhenSeparatedByOtherCode()
        {
            var lexer = new Lexer();
            lexer.Load("{ float a; int b; b = 1; sin(1.0); a = 1.0; }");

            var options = CustomOptions.None();
            options.GroupVariableDeclarations = true;
            options.JoinVariableDeclarationsWithAssignments = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("{ int b = 1; sin(1.0); float a = 1.0; }"));
        }

        [Test]
        public void CheckJoiningDeclarationsAndDefinitionsWhenReferencingParam()
        {
            var lexer = new Lexer();
            lexer.Load("void func(float f) { int i = 1; f += 10.0; float g = f; }");

            var options = CustomOptions.None();
            options.GroupVariableDeclarations = true;
            options.JoinVariableDeclarationsWithAssignments = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("void func(float f) { int i = 1; f += 10.0; float g = f; }"));
        }

        [Test]
        public void CheckNonConstDeclarationAndDefinitionAreJoinedIfAtTheRootLevel()
        {
            var lexer = new Lexer();
            lexer.Load("float bpm; void f() {} bpm = 130.;");

            var options = CustomOptions.None();
            options.JoinVariableDeclarationsWithAssignments = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("float bpm = 130.; void f() { }"));
        }

        [Test]
        public void CheckJoiningDeclarationsAndDefinitionsWhenUsingArrays()
        {
            const string Code = "void f(int a, int b) { } void main() { int a[2], b; a[0] = 2; b = 2; f(a, b); }";

            var lexer = new Lexer();
            lexer.Load(Code);

            var options = CustomOptions.None();
            options.JoinVariableDeclarationsWithAssignments = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(Code));
        }

        [Test]
        public void CheckJoiningDeclarationsAndDefinitionsWhenUsingVectorAccess()
        {
            const string Code = "void f(int a, int b) { } void main() { vec2 a, b; a.x = 2; b = 2; f(a, b); }";

            var lexer = new Lexer();
            lexer.Load(Code);

            var options = CustomOptions.None();
            options.JoinVariableDeclarationsWithAssignments = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(Code));
        }

        [Test]
        public void CheckDeclarationIsNotMovedIfAssignedInBracedCodeBranch()
        {
            var lexer = new Lexer();
            lexer.Load("{ int i; if (true) { i = 1; } else { i = 0; } i = 4; }");

            var options = CustomOptions.None();
            options.GroupVariableDeclarations = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("{ int i; if (true) i = 1; else i = 0; i = 4; }"));
        }

        [Test]
        public void CheckDeclarationIsNotMovedIfAssignedInUnbracedCodeBranch()
        {
            var lexer = new Lexer();
            lexer.Load("{ int i; if (true) i = 1; else i = 0; i = 4; }");

            var options = CustomOptions.None();
            options.GroupVariableDeclarations = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("{ int i; if (true) i = 1; else i = 0; i = 4; }"));
        }

        [Test]
        public void CheckDeclarationIsNotMovedIfWithinIfPragma()
        {
            var lexer = new Lexer();
            lexer.Load("#if A\nint i = 12; #else\nint i = 21; #endif");

            var options = CustomOptions.None();
            options.GroupVariableDeclarations = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("#if A int i = 12; #else int i = 21; #endif"));
        }

        [Test]
        public void CheckDeclarationIsNotJoinedWithDefinitionIfWithinIfPragma()
        {
            var lexer = new Lexer();
            lexer.Load("void f() { int i; #if A\ni = 12; #else\ni = 21; #endif\n}");

            var options = CustomOptions.None();
            options.GroupVariableDeclarations = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("void f() { int i; #if A i = 12; #else i = 21; #endif }"));
        }
        
        [Test]
        public void CheckDeclarationIsNotJoinedWithDefinitionIfDeepWithinIfPragma()
        {
            var lexer = new Lexer();
            lexer.Load("void f() {\n#ifdef AA\n{ vec2 v = vec2(1);\n#else\nvec2 v = vec2(2);\n#endif\n#ifdef AA\n}\n#endif\n}");

            var options = CustomOptions.None();
            options.GroupVariableDeclarations = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("void f() { #ifdef AA { vec2 v = vec2(1); #else vec2 v = vec2(2); #endif #ifdef AA } #endif }"));
        }

        [Test, Sequential]
        public void CheckSimplifyingFloats(
            [Values("10.0", "1.1", "0.10", "0.0000", "-0.09", "100.0", "100.1", "1100000.", "1.23f", "-0.1f", ".0f", "0.f", "10.00F", "102.", "001.1", "3.141592653589793238462643383279502884197", "-3.141592653589793238462643383279502884197", "1.541182543454656e-4", "1e10")] string code,
            [Values("10.", "1.1", ".1", "0.", "-.09", "1e2", "100.1", "11e5", "1.23", "-.1", "0.", "0.", "10.", "102.", "1.1", "3.1415926", "-3.1415926", "1.5411e-4", "1e10")] string expectedOutput)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.SimplifyFloatFormat = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(expectedOutput));
        }

        [Test, Sequential]
        public void CheckSimplifyingFloatCasts(
            [Values("float(1.1)", "float(1e2)", "float(2)", "float(1.800f)", "float(-0.1)", "float(-0.2f)")] string code,
            [Values("1.1", "1e2", "2.", "1.8", "-.1", "-.2")] string expectedOutput)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.SimplifyFloatFormat = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(expectedOutput));
        }

        [Test, Sequential]
        public void CheckSimplifyingIntCasts(
            [Values("int(1.9)", "int(1e2)", "int(2)", "int(1.82f)", "int(-2)", "int(-2.8)")] string code,
            [Values("1", "100", "2", "1", "-2", "-2")] string expectedOutput)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.SimplifyFloatFormat = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(expectedOutput));
        }

        [Test, Sequential]
        public void CheckSimplifyingVecConstruction(
            [Values("vec3(1.0, -2.0, 3.0)", "vec2(-1.0, 2.2)", "vec4(8.0)", "mat3(1.0)", "mat4(1.0)", "mat4x4(2.0)", "mat3x2(3.0)", "vec3(1.0, 1, 1.0)")] string code,
            [Values("vec3(1, -2, 3)", "vec2(-1, 2.2)", "vec4(8)", "mat3(1)", "mat4(1)", "mat4x4(2)", "mat3x2(3)", "vec3(1)")] string expectedCode)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.SimplifyVectorConstructors = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(expectedCode));
        }

        [Test, Sequential]
        public void CheckInliningPragmaIf01SectionsInRoot([Values("#if 0\nint remove;\n#else\nint keep;\n#endif", "#if 1\nint keep;\n#else\nint remove;\n#endif", "#if 1\nint keep;\n#endif")] string code)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.RemoveDisabledCode = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("int keep;"));
        }

        [Test, Sequential]
        public void CheckInliningPragmaIf01Sections([Values("void foo() {\n#if 0\nint remove;\n#else\nint keep;\n#endif\n}", "void foo() {\n#if 1\nint keep;\n#else\nint remove;\n#endif\n}", "void foo() {\n#if 1\nint keep;\n#endif\n}")] string code)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.RemoveDisabledCode = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("void foo() { int keep; }"));
        }

        [Test]
        public void CheckRemovingSingleBranchPragma0InRoot()
        {
            var lexer = new Lexer();
            lexer.Load("#if 0\nint remove;\n#endif\n");

            var options = CustomOptions.None();
            options.RemoveDisabledCode = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.Empty);
        }

        [Test]
        public void CheckRemovingSingleBranchPragma0()
        {
            var lexer = new Lexer();
            lexer.Load("void foo() {\n#if 0\nint remove;\n#endif\n}");

            var options = CustomOptions.None();
            options.RemoveDisabledCode = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("void foo() { }"));
        }

        [Test]
        public void CheckRemovingUnreferencedFunctionsDoesNotRemoveUsedFunction()
        {
            var lexer = new Lexer();
            lexer.Load("void foo() { } void bar() { foo(); } void main() { bar(); }");

            var options = CustomOptions.None();
            options.RemoveDisabledCode = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("void foo() { } void bar() { foo(); } void main() { bar(); }"));
        }

        [Test, Sequential]
        public void CheckRemovingUnreferencedFunctions([Values("void bar(); void foo() {} void bar() { foo(); } void main() { return; }", "void foo() {} void bar() { foo(); } void main() { return; }")] string code)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.RemoveUnusedFunctions = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("void main() { return; }"));
        }

        [Test, Sequential]
        public void CheckUnreferencedFunctionsNotRemovedIfEntryPointFunctionDetected()
        {
            const string Code = "void unused1() { } void unused2() { }";

            var lexer = new Lexer();
            lexer.Load(Code);

            var options = CustomOptions.None();
            options.RemoveUnusedFunctions = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(Code));
        }

        [Test]
        public void CheckFunctionDeclarationsWithoutDefinitionsAreRemoved()
        {
            var lexer = new Lexer();
            lexer.Load("void foo(); void main() { }");

            var options = CustomOptions.None();
            options.SimplifyFunctionDeclarations = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("void main() { }"));
        }

        [Test]
        public void CheckFunctionDeclarationParamNamesAreRemoved()
        {
            var lexer = new Lexer();
            lexer.Load("void foo(int a, int b); void main() { foo(1, 2); } void foo(int a, int b) { }");

            var options = CustomOptions.None();
            options.SimplifyFunctionDeclarations = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("void foo(int, int); void main() { foo(1, 2); } void foo(int a, int b) { }"));
        }
        
        [Test]
        public void CheckVoidParamInFunctionDeclarationsAreRemoved()
        {
            var lexer = new Lexer();
            lexer.Load("void foo(void); void main() { foo(); } void foo() { }");

            var options = CustomOptions.None();
            options.SimplifyFunctionParams = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("void foo(); void main() { foo(); } void foo() { }"));
        }

        [Test]
        public void CheckVoidParamInFunctionDefinitionsAreRemoved()
        {
            var lexer = new Lexer();
            lexer.Load("void foo(void) { } void main() { foo(); }");

            var options = CustomOptions.None();
            options.SimplifyFunctionParams = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("void foo() { } void main() { foo(); }"));
        }

        [Test, Sequential]
        public void CheckInliningConstantDefines([Values("#define AA\n#define NUM 2.3\nfloat main() { return NUM; }", "#define AA\nfloat main() {\n#define NUM 2.3\nreturn NUM; }")] string code)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.InlineDefines = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("#define AA float main() { return 2.3; }"));
        }
        
        [Test, Sequential]
        public void CheckInliningConstantDefinesWhenUsageIsWithVectorComponent()
        {
            var lexer = new Lexer();
            lexer.Load("#define R iResolution\nfloat main() { return R.x; }");

            var options = CustomOptions.None();
            options.InlineDefines = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("float main() { return iResolution.x; }"));
        }

        [Test, Sequential]
        public void CheckInliningConstantDefinesWhenUsageIsFunctionName()
        {
            var lexer = new Lexer();
            lexer.Load("#define S smoothstep\nfloat main() { return S(0.0, 1.0, 0.5); }");

            var options = CustomOptions.None();
            options.InlineDefines = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("float main() { return smoothstep(0.0, 1.0, 0.5); }"));
        }
        
        [Test]
        public void CheckInliningConstantDefinesNotInlinedIfResultIncreasesCodeSize()
        {
            const string Code = "#define T\t(iTime + 12.3 / 1.234)\n\nvoid main() { float t = T + T * 2.0 + T * 3.0 + T * 4.0; }";

            var lexer = new Lexer();
            lexer.Load(Code);

            var options = CustomOptions.None();
            options.InlineDefines = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode(), Is.EqualTo(Code));
        }

        [Test]
        public void CheckInliningConstantDefinesWithMultipleUses()
        {
            var lexer = new Lexer();
            lexer.Load("#define FOO 1\nint main() { return FOO + FOO; }");

            var options = CustomOptions.None();
            options.InlineDefines = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("int main() { return 1 + 1; }"));
        }

        [Test]
        public void CheckConstantDefineIsNotInlinedIfDefinedWithinPragmaIf()
        {
            var lexer = new Lexer();
            lexer.Load("#if HW_PERFORMANCE==0\n#define AA 1\n#else\n#define AA 2\n#endif");

            var options = CustomOptions.None();
            options.InlineDefines = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("#if HW_PERFORMANCE == 0 #define AA 1 #else #define AA 2 #endif"));
        }
        
        [Test]
        public void CheckConstantDefineIsNotInlinedIfUsedInPragmaIf()
        {
            var lexer = new Lexer();
            lexer.Load("#define FOO 1\nint main() {\n#if FOO>1\nreturn 1;\n#endif\n}");

            var options = CustomOptions.None();
            options.InlineDefines = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("#define FOO 1 int main() { #if FOO > 1 return 1; #endif }"));
        } 
        
        [Test, Sequential]
        public void CheckConstantDefinitionOfSimpleVectorIsInlined([Values("vec2(1.1, 1.2)", "vec3(1.0)", "vec4(0)")] string define)
        {
            var lexer = new Lexer();
            lexer.Load($"#define V {define}\nvoid main() {{ return V; }}");

            var options = CustomOptions.None();
            options.InlineDefines = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo($"void main() {{ return {define}; }}"));
        }

        [Test, Sequential]
        public void CheckInliningConstantVariables([Values("const float NUM = 2.3, FOO = 1.0; float main() { return NUM + FOO; }", "float main() { const float NUM = 2.3, FOO = 1.0; return NUM + FOO; }")] string code)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.InlineConstantVariables = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("float main() { return 2.3 + 1.0; }"));
        }

        [Test]
        public void CheckGlobalConstantVariableNotInlinedWithinFunctionWithMatchingParameterName()
        {
            const string Code = "const int g = 2; void f(int g) { return g; }";

            var lexer = new Lexer();
            lexer.Load(Code);

            var options = CustomOptions.None();
            options.InlineConstantVariables = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(Code));
        }

        [Test, Sequential]
        public void CheckInliningConstantVectors(
            [Values("vec3 f() { const vec3 theVector = vec3(1.0, 1.0, 1.0); return theVector * 2.0; }",
                    "vec3 f() { const vec3 theVector = vec3(1, 1, 1); return theVector * 2.0; }",
                    "vec3 f() { const vec3 theVector = vec3(1); return theVector * 2.0; }")] string code)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.InlineConstantVariables = true;
            options.SimplifyVectorConstructors = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("vec3 f() { return vec3(1) * 2.0; }"));
        }

        [Test]
        public void CheckInliningConstantVariableWithMultipleUses()
        {
            var lexer = new Lexer();
            lexer.Load("int main() { const int FOO = 1; return FOO + FOO; }");

            var options = CustomOptions.None();
            options.InlineConstantVariables = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("int main() { return 1 + 1; }"));
        }

        [Test, Sequential]
        public void CheckMultipleUsesOfConstantComplexVariableIsNotInlined([Values("int main() { const vec2 FOO = vec2(1, 2); return FOO.x; }",
                                                                                   "int main() { const int FOO = 1 + 2; return FOO + FOO; }")] string code)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.InlineConstantVariables = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(code));
        }

        [Test]
        public void CheckRemovingUnnecessaryInFunctionParams()
        {
            var lexer = new Lexer();
            lexer.Load("void f(in vec2 uv); void main(in vec3 uv) { f(uv); } void f(in vec2 uv) { }");

            var options = CustomOptions.None();
            options.SimplifyFunctionParams = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("void f(vec2 uv); void main(vec3 uv) { f(uv); } void f(vec2 uv) { }"));
        }

        [Test]
        public void CheckStructFieldsOfConsecutiveTypeAreJoined()
        {
            var lexer = new Lexer();
            lexer.Load("struct Foo { int a; int b; float c; };");

            var options = CustomOptions.None();
            options.GroupVariableDeclarations = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("struct Foo { int a, b; float c; };"));
        }

        [Test]
        public void CheckStructFieldsAreNotReordered()
        {
            const string Code = "struct Foo { int a; float b; int c; };";

            var lexer = new Lexer();
            lexer.Load(Code);

            var options = CustomOptions.None();
            options.GroupVariableDeclarations = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(Code));
        }

        [Test, Sequential]
        public void CheckCommentedOutCodeIsRemoved([Values("// foo();\n// do not remove.\nvoid main() { }", "// a = b;\n// do not remove.\nvoid main() { }", "// break;\n// do not remove.\nvoid main() { }")] string code)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.RemoveDisabledCode = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode(), Is.EqualTo("// do not remove.\nvoid main() { }"));
        }

        [Test]
        public void CheckRemovalOfAllComments()
        {
            var lexer = new Lexer();
            lexer.Load("// a comment\n// line 2\nvoid foo() { // another\n }");

            var options = CustomOptions.None();
            options.RemoveComments = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode(), Is.EqualTo("void foo() { }"));
        }

        [Test]
        public void CheckRemovalOfNonHeaderComments()
        {
            var lexer = new Lexer();
            lexer.Load("// a comment\n// line 2\nvoid foo() { // another\n }");

            var options = CustomOptions.None();
            options.RemoveComments = true;
            options.KeepHeaderComments = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode(), Is.EqualTo("// a comment\n// line 2\nvoid foo() { }"));
        }

        [Test]
        public void CheckCombiningAssignmentFromFuncAndReturnAtEndOfFunction()
        {
            var lexer = new Lexer();
            lexer.Load("int num() { return 3; } void main() { int n = num(); return n; }");

            var options = CustomOptions.None();
            options.CombineAssignmentWithSingleUse = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("int num() { return 3; } void main() { return num(); }"));
        }

        [Test]
        public void CheckCombiningAssignmentFromFuncAndReturnAtEndOfBraces()
        {
            var lexer = new Lexer();
            lexer.Load("int num() { return 3; } void main() { if (1) { int n, a; a = 10; n = num(); return n; } }");

            var options = CustomOptions.None();
            options.CombineAssignmentWithSingleUse = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("int num() { return 3; } void main() { if (1) return num(); }"));
        }

        [Test]
        public void CheckCombiningAssignmentFromCalcAndReturnAtEndOfFunction()
        {
            var lexer = new Lexer();
            lexer.Load("float smin(float a, float b, float k) { float h = clamp(.5 + .5 * (b - a) / k, 0., 1.); return mix(b, a, h) - k * h * (1. - h); }");

            var options = CustomOptions.None();
            options.CombineAssignmentWithSingleUse = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("float smin(float a, float b, float k) { float h = clamp(.5 + .5 * (b - a) / k, 0., 1.); return mix(b, a, h) - k * h * (1. - h); }"));
        }

        [Test]
        public void CheckAssignmentNotJoinedWithReturnStatementIfUsedMultipleTimes()
        {
            var lexer = new Lexer();
            lexer.Load("void main() { float n = 1.0; return n + n; }");

            var options = CustomOptions.None();
            options.CombineAssignmentWithSingleUse = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("void main() { float n = 1.0; return n + n; }"));
        }

        [Test]
        public void CheckCombiningAssignmentFromCalcAndReturnAtEndOfFunctionWithDeclDefJoin()
        {
            var lexer = new Lexer();
            lexer.Load("int num() { return 3; } void main() { int n = 1; n = min(d, num); return n; }");

            var options = CustomOptions.None();
            options.CombineAssignmentWithSingleUse = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("int num() { return 3; } void main() { return min(d, num); }"));
        }

        [Test]
        public void CheckIndividualMatrixAssignmentsAreNotCombined()
        {
            const string Code = "mat3 f() { mat3 m; m[0] = vec3(0); return m; }";

            var lexer = new Lexer();
            lexer.Load(Code);

            var options = CustomOptions.None();
            options.CombineAssignmentWithSingleUse = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(Code));
        } 

        [Test, Sequential]
        public void CheckCombiningAssignmentWithUseWhenUsedOnlyOnce(
            [Values("int main() { int a = 1, b = 2, c = b + 1; return c; }",
                    "vec3 cam(vec3 ro, vec3 la, vec2 uv) { vec3 f = normalize(la - ro), r = normalize(cross(vec3(0, 1, 0), f)), up = cross(f, r); return normalize(f + r * uv.x + up * uv.y); }",
                    "int f(int i) { return i; } void main() { int a; for (int i = 0; i < 2; i++) { a = 2 + i; f(a); } return a; }",
                    "int main(out int i) { i = 2; return i; }",
                    "int main(inout int i) { i = 2; return i; }",
                    "int main(int i) { i = 2; return i; }")] string code,
            [Values("int main() { return 2 + 1; }",
                    "vec3 cam(vec3 ro, vec3 la, vec2 uv) { vec3 f = normalize(la - ro), r = normalize(cross(vec3(0, 1, 0), f)); return normalize(f + r * uv.x + cross(f, r) * uv.y); }",
                    "int f(int i) { return i; } void main() { int a; for (int i = 0; i < 2; i++) { a = 2 + i; f(a); } return a; }",
                    "int main(out int i) { i = 2; return i; }",
                    "int main(inout int i) { i = 2; return i; }",
                    "int main(int i) { i = 2; return i; }")] string expected)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.CombineAssignmentWithSingleUse = true;
            options.GroupVariableDeclarations = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(expected));
        }

        [Test, Sequential]
        public void CheckCombiningConsecutiveReassignments(
            [Values("int main() { int a = 1; a = a + 2; a = a - 4; return a; }",
                    "int main() { int a = 1; a = a * 2; a = a / 4; return a; }",
                    "void main() { vec2 v; v = vec2(1.); v = v + v.x; }")] string code,
            [Values("int main() { int a = 1; a = (a + 2) - 4; return a; }",
                    "int main() { int a = 1; a = a * 2 / 4; return a; }",
                    "void main() { vec2 v; v = vec2(1.); v = v + v.x; }")] string expected)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.CombineConsecutiveAssignments = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(expected));
        }

        [Test]
        public void CheckCombiningAssignmentsRecognizesArrayAccessors()
        {
            const string Code = "void main() { int a[2]; a[0] = a[0] + 1; a[1] = a[1] + 2; }";

            var lexer = new Lexer();
            lexer.Load(Code);

            var options = CustomOptions.None();
            options.CombineConsecutiveAssignments = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(Code));
        }

        [Test, Sequential]
        public void GivenVectorReferencingAllComponentsCheckSimplifyingToJustVariableName(
            [Values("vec4 a = vec4(1, 2, 3, 4), b = a.rgba;",
                    "vec3 a = vec3(1, 2, 3), b = a.xyz;",
                    "vec2 a = vec2(1, 2), b = a.xy;")] string code,
            [Values("vec4 a = vec4(1, 2, 3, 4), b = a;",
                    "vec3 a = vec3(1, 2, 3), b = a;",
                    "vec2 a = vec2(1, 2), b = a;")] string expected)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.GroupVariableDeclarations = true;
            options.SimplifyVectorReferences = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(expected));
        }

        [Test, Sequential]
        public void CheckSimplifyingVec4ConstructionFromVecComponents(
            [Values("vec3 ab; vec4 v = vec4(ab.x, ab.y, ab.z, 1.0);",
                    "vec3 ab; vec4 v = vec4(1.0, ab.x, ab.y, ab.z);",
                    "vec3 ab; vec4 v = vec4(ab.y, ab.x, ab.x, ab.y);",
                    "vec3 ab; vec4 v = vec4(1.0, ab.x, ab.x, 2.0);",
                    "vec3 ab; vec4 v = vec4(1.0, ab.xy, ab.z);",
                    "vec4 ab; vec4 v = vec4(ab.r, ab.g, ab.b, ab.a);",
                    "vec4 ab; vec4 v = vec4(1.0, ab.b, ab.g, ab.r);",
                    "vec2 ab; vec4 v = vec4(ab, ab.x, ab.y);",
                    "vec2 ab; vec4 v = vec4(ab, .1, .2);",
                    "vec3 ab; vec2 v = vec2(ab.x, ab.z);",
                    "vec3 ab; vec2 v = vec2(ab.x);",
                    "struct S{ float A, B, C; }; S s; vec3 v = vec3(s.A, s.B, s.C);")] string code,
            [Values("vec3 ab; vec4 v = vec4(ab, 1);",
                    "vec3 ab; vec4 v = vec4(1, ab);",
                    "vec3 ab; vec4 v = ab.yxxy;",
                    "vec3 ab; vec4 v = vec4(1, ab.xx, 2);",
                    "vec3 ab; vec4 v = vec4(1, ab);",
                    "vec4 ab, v = ab;",
                    "vec4 ab, v = vec4(1, ab.bgr);",
                    "vec2 ab; vec4 v = vec4(ab, ab);",
                    "vec2 ab; vec4 v = vec4(ab, .1, .2);",
                    "vec3 ab; vec2 v = ab.xz;",
                    "vec3 ab; vec2 v = vec2(ab.x);",
                    "struct S { float A, B, C; }; S s; vec3 v = vec3(s.A, s.B, s.C);")] string expected)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.GroupVariableDeclarations = true;
            options.JoinVariableDeclarationsWithAssignments = true;
            options.SimplifyVectorConstructors = true;
            options.SimplifyVectorReferences = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(expected));
        }

        [Test, Sequential]
        public void CheckSimplifyingArithmetic(
            [Values("float main() { return ((1.23)); }",
                    "int main() { return ((1 + 2)); }",
                    "int main() { return ((1 + 2 * 3)); }",
                    "int main() { return (1 + ((2))); }",
                    "int main() { int i = (1 + ((2))); }",
                    "int main() { int i = ((2)); }",
                    "int main() { int i = -(2 + 3); }",
                    "void main() { int i = +4; }",
                    "void main() { vec2 v; v.x = (v.y) }",
                    "void main() { vec2 v = vec2(+1, +2); }",
                    "void main() { float i = +fract(1.1); }",
                    "float main() { return 1.0 * (2.0 / 3.0); }",
                    "void main() { float d = 1.0, t = 2.0; if ((d < (.003 * t)) || (t >= 25.)) return; }",
                    "void main() { float f = (1. - exp((-.12 * t))); }",
                    "void main() { float n = 2.0, f = (1.0 + ((1.2 * n * 3.0) * (4.0 + n))); }")] string code,
            [Values("float main() { return 1.23; }",
                    "int main() { return 1 + 2; }",
                    "int main() { return 1 + 2 * 3; }",
                    "int main() { return 1 + 2; }",
                    "int main() { int i = 1 + 2; }",
                    "int main() { int i = 2; }",
                    "int main() { int i = -(2 + 3); }",
                    "void main() { int i = 4; }",
                    "void main() { vec2 v; v.x = v.y }",
                    "void main() { vec2 v = vec2(1, 2); }",
                    "void main() { float i = fract(1.1); }",
                    "float main() { return 1.0 * 2.0 / 3.0; }",
                    "void main() { float d = 1.0, t = 2.0; if (d < .003 * t || t >= 25.) return; }",
                    "void main() { float f = 1. - exp(-.12 * t); }",
                    "void main() { float n = 2.0, f = 1.0 + 1.2 * n * 3.0 * (4.0 + n); }")] string expected)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.SimplifyArithmetic = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(expected));
        }

        [Test, Sequential]
        public void CheckSimplifyingArithmeticDoesNotRemoveRequiredBrackets(
            [Values("int main() { if (1) return 1; return 0; }",
                    "float main() { return 1.0 / (2.0 * 3.0); }",
                    "void main() { float d, n = 1.2; if ((d = 1.0) < 2.0 || (d = 2.0) >= n) return; }",
                    "vec2 v = vec2(1);",
                    "while (1) { continue; }",
                    "vec4 v1, v2; vec3 v = (v1 + v2).xyz;")] string code)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.SimplifyArithmetic = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(code));
        }

        [Test]
        public void CheckArithmeticInPragmaDefinesIsNotSimplified()
        {
            var lexer = new Lexer();
            lexer.Load("#define F(x) ((x) * 2.0)");

            var options = CustomOptions.None();
            options.SimplifyArithmetic = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("#define F(x) ((x) * 2.0)"));
        }

        [Test, Sequential]
        public void CheckSimplifyingElseStatements(
            [Values("float main() { int = 1; if (i == 1) return; else i++; }",
                    "float main() { int = 1; if (i == 1) { i--; return; } else { i++; } }",
                    "float main() { for (int i = 0; i < 2; i++) { if (i == 1) break; else i *= 1; } }",
                    "float main() { for (int i = 0; i < 2; i++) { if (i == 0) { continue; } else i *= 1; } }")] string code,
            [Values("float main() { int = 1; if (i == 1) return; i++; }",
                    "float main() { int = 1; if (i == 1) { i--; return; } i++; }",
                    "float main() { for (int i = 0; i < 2; i++) { if (i == 1) break; i *= 1; } }",
                    "float main() { for (int i = 0; i < 2; i++) { if (i == 0) continue; i *= 1; } }")] string expected)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.SimplifyBranching = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(expected));
        }

        [Test, Sequential]
        public void CheckUnreachableContentIsRemoved(
            [Values("void main() { return; int i = 1; i += 2; }",
                    "void main() { return 10; return; }",
                    "void main() { if (1 == 2) { return 10; int i = 2; } }")] string code,
            [Values("void main() { return; }",
                    "void main() { return 10; }",
                    "void main() { if (1 == 2) return 10; }")] string expected)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.RemoveUnreachableCode = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(expected));
        }

        [Test, Sequential]
        public void CheckSimplifyingSimpleSums(
            [Values("int i = 1 + 2;",
                    "int i; i = 1 + 2;",
                    "int i = 1 + 2 + 3;",
                    "int i = 3 / 2;",
                    "float i = 1.1 + 2.2;",
                    "float i = 1.1 - 2.2;",
                    "float i; i = 1.2 + 2.3;",
                    "float i = 1.1 + 2.3 + 3.2;",
                    "float i = 1.1 * 2.2;",
                    "float i = 3.141 / 4.;",
                    "float i = 5.0 + 1.1 * 2.2 - 0.1;",
                    "float a = 1.2, b = 2.3, c = a + - b;",
                    "float a = 1.2, i = a - -0.5;",
                    "float a = 1.2, i = a + -0.5;",
                    "float a = 1.2, i = a / 10.0 + 1.1;",
                    "float a = 1.2, i = a / 10.0 * 1.1;",
                    "float a = 1.2, i = a / 10.0 / 1.1;",
                    "float i = (5.0 + 1.1) * 2.2 * 0.1;",
                    "float i = 1.2, b = i * 1.0;",
                    "float i = -3.0 + 0.2;",
                    "#define smin(a,b,k) min(a,b)-pow(max(k-abs(a-b),0.)/k,k)*k*(1.0/6.0)",
                    "#define D (2.1 / 12.)\nfloat f = D * 2.0;")] string code,
            [Values("int i = 3;",
                    "int i; i = 3;",
                    "int i = 6;",
                    "int i = 1;",
                    "float i = 3.3;",
                    "float i = -1.1;",
                    "float i; i = 3.5;",
                    "float i = 6.6;",
                    "float i = 2.42;",
                    "float i = .78525;",
                    "float i = 7.32;",
                    "float a = 1.2, b = 2.3, c = a - b;",
                    "float a = 1.2, i = a + 0.5;",
                    "float a = 1.2, i = a - 0.5;",
                    "float a = 1.2, i = a / 10.0 + 1.1;",
                    "float a = 1.2, i = a / 9.09091;",
                    "float a = 1.2, i = a / 11.;",
                    "float i = 1.342;",
                    "float i = 1.2, b = i;",
                    "float i = -2.8;",
                    "#define smin(a, b, k) min(a, b) - pow(max(k - abs(a - b), 0.) / k, k) * k * .16667",
                    "float f = .35;")] string expected)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.SimplifyArithmetic = true;
            options.PerformArithmetic = true;
            options.InlineDefines = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(expected));
        }

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
        public void CheckRemovingUnusedVariables(
            [Values("void main() { int a; }",
                    "void main() { int a = 1; }",
                    "int main() { int a, b = 2, c = 3; return c; }",
                    "int a; int main() { return 1; }",
                    "int a = 1; int main() { return 1; }",
                    "int a = 1; int f(int n) { return n; } int main() { return f(a); }",
                    "int a = 1; int f() { a++; } void main() { f(); }",
                    "int a = 1; int f() { a++; } void main() { }",
                    "out int a; void main() { }",
                    "int main() { vec2 a = vec2(1, 2); return 1; }",
                    "int main() { vec2 a = vec2(1, 2); return a.x; }",
                    "struct S { int a; }; void main() { S s = S(1); return s.a; }",
                    "int f(int n) { return 1; } int main() { return f(2); }",
                    "int main() { int a = 1, b = a + 1; return b; }",
                    "struct Sphere { }; const Sphere lights[1] = Sphere[]();",
                    "vec2 main() { vec2 h; h.x = 1.1; h.y = 2.2; return h; }")] string code,
            [Values("void main() { }",
                    "void main() { }",
                    "int main() { int c = 3; return c; }",
                    "int main() { return 1; }",
                    "int main() { return 1; }",
                    "int a = 1; int f(int n) { return n; } int main() { return f(a); }",
                    "int a = 1; int f() { a++; } void main() { f(); }",
                    "void main() { }",
                    "out int a; void main() { }",
                    "int main() { return 1; }",
                    "int main() { vec2 a = vec2(1, 2); return a.x; }",
                    "struct S { int a; }; void main() { S s = S(1); return s.a; }",
                    "int f(int n) { return 1; } int main() { return f(2); }",
                    "int main() { int a = 1, b = a + 1; return b; }",
                    "struct Sphere { };",
                    "vec2 main() { vec2 h; h.x = 1.1; h.y = 2.2; return h; }")] string expected)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.GroupVariableDeclarations = true;
            options.RemoveUnusedFunctions = true;
            options.RemoveUnusedVariables = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(expected));
        }

        [Test, Sequential]
        public void GivenUnusedVariableAssignedValueFromFunctionWithOutParamCheckFunctionCallIsNotRemoved(
            [Values("int f(inout int n) { n++; return n; } void main() { int a = 1, b = f(a); }",
                    "int f(inout float n) { n++; return 1; } void main() { float a = 1.0; int b = f(a); }",
                    "int f(inout float n) { n++; return 1; } void main() { float a = 1.0; int b; b = 1.0 + f(a); }")] string code,
            [Values("int f(inout int n) { n++; return n; } void main() { int a = 1, b = f(a); }",
                    "int f(inout float n) { n++; return 1; } void main() { float a = 1.0; f(a); }",
                    "int f(inout float n) { n++; return 1; } void main() { float a = 1.0; 1.0 + f(a); }")] string expected)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.RemoveUnusedVariables = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(expected));
        }

        [Test]
        public void CheckVariableAssignedByFunctionCallOutParamCalledByReturnStatementIsNotRemoved()
        {
            const string Code = "void f(out int i) { } void main() { int n; return f(n); }";

            var lexer = new Lexer();
            lexer.Load(Code);

            var options = CustomOptions.None();
            options.CombineAssignmentWithSingleUse = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(Code));
        }

        [Test, Sequential]
        public void CheckFindingVariablesToMakeConst(
            [Values("int g = 2; int main() { return g; }",
                    "int g = 2; int main() { g++; return g; }",
                    "int g = 2; int main() { g *= 2; return g; }",
                    "int main() { vec2 v; if (true) break; v = vec2(1, 2); return v; }",
                    "int main() { int i, j = 2; for (i = 0; i < j; i++) break; return j; }",
                    "int main() { int i; if (1) continue; i = 3; return i; }",
                    "vec3 ro = vec3(0); ro.xz = rot(a) * ro.xz;",
                    "int i = 1; i++;",
                    "int i = 1; ++i;")] string code,
            [Values("int main() { return 2; }",
                    "int g = 2; int main() { g++; return g; }",
                    "int g = 2; int main() { g *= 2; return g; }",
                    "int main() { if (true) break; return vec2(1, 2); }",
                    "int main() { int i; for (i = 0; i < 2; i++) break; return 2; }",
                    "int main() { if (1) continue; return 3; }",
                    "vec3 ro = vec3(0); ro.xz = rot(a) * ro.xz;",
                    "int i = 1; i++;",
                    "int i = 1; ++i;")] string expected)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.InlineConstantVariables = true;
            options.DetectConstants = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(expected));
        }

        [Test, Sequential]
        public void CheckNonConstVariablesAreNotMadeConstIfPassedIntoOutParam(
            [Values("void f(inout int n) { n++; } void main() { int i = 1; f(i); }",
                    "void f(out int n) { n++; } void main() { int i; f(i); }",
                    "void f(out int n) { n++; } void main() { int i = 2; f(i); }")] string code)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.DetectConstants = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(code));
        }

        [Test, Sequential]
        public void CheckAssignmentAndMathOpCanBeJoined(
            [Values("void f() { float a = 1.; a = a * 2.; }",
                    "void f() { float a = 1.; a = (a * 2.); }",
                    "void f() { float a = 1.; a = a / 2.; }",
                    "void f() { float a = 1.; a = (a / 2.); }",
                    "void f() { float a = 1.; a = a + 2.; }",
                    "void f() { float a = 1.; a = (a + 2.); }",
                    "void f() { float a = 1.; a = a - 2.; }",
                    "void f() { float a = 1.; a = (a - 2.); }",
                    "void f() { float a = 1.; a = a * 2. + 3.; }",
                    "void f() { float a = 1.; a = (a * 2.) * 3.; }",
                    "void f() { float a = 1.; a = (a / 2.) * 3.; }",
                    "void f() { float a = 1.; a = (a + 2.) / 3.; }",
                    "void f() { float a = 1.; a = (a - 2.) / 3; }",
                    "void f() { vec2 v = vec2(1); v.x = v.x + 2.; }",
                    "void f() { vec2 v = vec2(1); v.x = v.y + 2.; }")] string code,
            [Values("void f() { float a = 1.; a *= 2.; }",
                    "void f() { float a = 1.; a *= 2.; }",
                    "void f() { float a = 1.; a /= 2.; }",
                    "void f() { float a = 1.; a /= 2.; }",
                    "void f() { float a = 1.; a += 2.; }",
                    "void f() { float a = 1.; a += 2.; }",
                    "void f() { float a = 1.; a -= 2.; }",
                    "void f() { float a = 1.; a -= 2.; }",
                    "void f() { float a = 1.; a = a * 2. + 3.; }",
                    "void f() { float a = 1.; a = (a * 2.) * 3.; }",
                    "void f() { float a = 1.; a = (a / 2.) * 3.; }",
                    "void f() { float a = 1.; a = (a + 2.) / 3.; }",
                    "void f() { float a = 1.; a = (a - 2.) / 3; }",
                    "void f() { vec2 v = vec2(1); v.x += 2.; }",
                    "void f() { vec2 v = vec2(1); v.x = v.y + 2.; }")] string expected)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.IntroduceMathOperators = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(expected));
        }

        [Test, Sequential]
        public void CheckDetectingIncrementDecrementOperatorOpportunity(
            [Values("i += 1;",
                    "i += 1.0;",
                    "i += 1.;",
                    "float main() { vec2 v; v.x += 1.0; return v; }",
                    "int main() { int v[1]; v[0] = 1; v[0] += 1; return v[0]; }",
                    "void main() { for (int i = 0; i < 10; i += 1) { } }",
                    "i -= 1;",
                    "i -= 1.0;",
                    "i -= 1.;",
                    "float main() { vec2 v; v.x -= 1.0; return v; }",
                    "int main() { int v[1]; v[0] = 1; v[0] -= 1; return v[0]; }",
                    "void main() { for (int i = 10; i > 0; i -= 1) { } }")] string code,
            [Values("i++;",
                    "i++;",
                    "i++;",
                    "float main() { vec2 v; v.x++; return v; }",
                    "int main() { int v[1]; v[0] = 1; v[0]++; return v[0]; }",
                    "void main() { for (int i = 0; i < 10; i++) { } }",
                    "i--;",
                    "i--;",
                    "i--;",
                    "float main() { vec2 v; v.x--; return v; }",
                    "int main() { int v[1]; v[0] = 1; v[0]--; return v[0]; }",
                    "void main() { for (int i = 10; i > 0; i--) { } }")] string expected)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.IntroduceMathOperators = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(expected));
        }

        [Test]
        public void CheckReturnStatementDoesNotTruncateFurtherStatementsIfWithinSwitchCase()
        {
            var lexer = new Lexer();
            lexer.Load("switch (1) { case 1: break; case 2: { break; } case 3: return; case 4: { return; int a = 1; } default: break; }");

            var options = CustomOptions.None();
            options.RemoveUnreachableCode = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("switch (1) { case 1: break; case 2: { break; } case 3: return; case 4: { return; int a = 1; } default: break; }"));
        }

        [Test, Sequential]
        public void CheckStatementBracketsNotRemovedWhenSimplifyingArithmetic(
            [Values("switch (1) { case 1: break; }", "if (1) break;")] string code)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.SimplifyArithmetic = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(code));
        }

        [Test]
        public void CheckSupportForTernaryOperator()
        {
            var lexer = new Lexer();
            lexer.Load("int main() { int a = true ? 1 : 0; return a; }");

            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify();

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("int main() { return true ? 1 : 0; }"));
        }

        [Test]
        public void CheckParsingUniformsLeaveCodeUnchanged()
        {
            var lexer = new Lexer();
            lexer.Load("uniform float fGlobalTime;\nuniform vec2 v2Resolution;");

            var options = CustomOptions.All();
            options.RemoveUnusedVariables = false;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("uniform float fGlobalTime; uniform vec2 v2Resolution;"));
        }

        [Test, Sequential]
        public void CheckArrayAssignmentsAreMaintained(
            [Values("int arr[2] = int[2](23, 32);",
                    "int arr[] = int[2](23, 32);",
                    "int arr[2] = int[](23, 32);",
                    "int arr[] = int[](23, 32);",
                    "int arr[2];",
                    "const int arr[2] = int[2](23, 32);",
                    "const int arr[] = int[2](23, 32);",
                    "const int arr[2] = int[](23, 32);",
                    "const int arr[] = int[](23, 32);")] string code)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(CustomOptions.None());

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(code));
        }

        [Test]
        public void CheckConstArrayAssignmentsAreNotInlined()
        {
            const string Code = "int foo() { const int arr[2] = int[2](23, 32); return arr[0]; }";

            var lexer = new Lexer();
            lexer.Load(Code);

            var options = CustomOptions.None();
            options.InlineConstantVariables = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(Code));
        }

        [Test]
        public void CheckNonConstArrayAssignmentsCanBeMadeConst()
        {
            var lexer = new Lexer();
            lexer.Load("int foo() { int arr[2] = int[2](23, 32); return arr[0]; }");

            var options = CustomOptions.None();
            options.DetectConstants = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("int foo() { const int arr[2] = int[2](23, 32); return arr[0]; }"));
        }

        [Test]
        public void CheckModifiedNonConstArrayIsNotMadeConst()
        {
            const string Code = "int foo() { int arr[2] = int[2](23, 32); arr[0] = 1; return arr[0]; }";

            var lexer = new Lexer();
            lexer.Load(Code);

            var options = CustomOptions.None();
            options.DetectConstants = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(Code));
        }

        [Test, Sequential]
        public void CheckArrayAccessWithIndexSucceeds(
            [Values("int main() { int arr[2] = int[2](23, 32); return arr[0]; }",
                    "int main() { int arr[2]; a[0] = 1; }",
                    "int main() { int arr[2] = int[2](23, 32), i = 1; return arr[i]; }")] string code)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(CustomOptions.None());

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(code));
        }

        [Test, Sequential]
        public void CheckGroupingArrayDeclarations(
            [Values("int arr1[2]; int arr2[2];",
                    "int arr1[2] = int[2](1, 2); int arr2[2];",
                    "int arr1[2]; int arr2[2] = int[](1, 2);")] string code,
            [Values("int arr1[2], arr2[2];",
                    "int arr1[2] = int[2](1, 2), arr2[2];",
                    "int arr1[2], arr2[2]; arr2 = int[](1, 2);")] string expected)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.None();
            options.GroupVariableDeclarations = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(expected));
        }

        [Test]
        public void CheckJoiningArrayDeclarationsWithAssignments()
        {
            var lexer = new Lexer();
            lexer.Load("int arr[2]; arr[2] = int[2](23, 32);");

            var options = CustomOptions.None();
            options.JoinVariableDeclarationsWithAssignments = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("int arr[2] = int[2](23, 32);"));
        }

        [Test]
        public void CheckOptimizationsApplyWithinArrayAccessors()
        {
            var lexer = new Lexer();
            lexer.Load("int foo() { return 1; } int main() { int arr[2] = int[2](23, 32); return arr[foo() + 1 * 2 - 1]; }");

            var options = CustomOptions.None();
            options.PerformArithmetic = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("int foo() { return 1; } int main() { int arr[2] = int[2](23, 32); return arr[foo() + 1]; }"));
        }

        [Test]
        public void CheckCombiningArrayAssignmentWithReturn()
        {
            var lexer = new Lexer();
            lexer.Load("int[2] foo() { int arr[2] = int[2](23, 32); return arr; }");

            var options = CustomOptions.None();
            options.CombineAssignmentWithSingleUse = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo("int[2] foo() { return int[2](23, 32); }"));
        }

        [Test, Sequential]
        public void CheckFullOptimizationDoesNotCorruptArrays(
            [Values(
                       "int main() { int arr[2] = int[2](23, 32); return arr[0]; }",
                       "int main() { int arr[2]; arr[0] = 1; }",
                       "int main() { int arr[] = int[](1); arr[0 + 0] = arr[0] * 2; }",
                       "int main() { int arr[] = int[](1); arr[0] = arr[1] * 2; }",
                       "int main() { int arr[2] = int[2](23, 32), i = 1; return arr[i]; }")] string code,
            [Values(
                       "int main() { return int[2](23, 32)[0]; }",
                       "int main() { int arr[2]; arr[0] = 1; }",
                       "int main() { const int arr[] = int[](1); arr[0] *= 2; }",
                       "int main() { int arr[] = int[](1); arr[0] = arr[1] * 2; }",
                       "int main() { return int[2](23, 32)[1]; }")] string expected)
        {
            var lexer = new Lexer();
            lexer.Load(code);

            var options = CustomOptions.All();
            options.RemoveUnusedVariables = false;

            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(expected));
        }

        [Test]
        public void CheckFloatWithEIsNotExpandedWithinVectorConstructor()
        {
            const string Code = "vec2 d = vec2(1e5, 0);";

            var lexer = new Lexer();
            lexer.Load(Code);

            var options = CustomOptions.None();
            options.SimplifyVectorConstructors = true;
            var rootNode = new Parser(lexer)
                .Parse()
                .Simplify(options);

            Assert.That(rootNode.ToCode().ToSimple(), Is.EqualTo(Code));
        }
    }
}