// -----------------------------------------------------------------------
//  <copyright file="DefineTests.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using NUnit.Framework;
using Shrinker.Lexer;
using Shrinker.Parser;

namespace UnitTests.Transpiler;

[TestFixture]
public class DefineTests
{
    [Test]
    public void GivenDefineWithNoRhsCheckHashIfIsReplacedWithContent()
    {
        var lexer = new Lexer();
        lexer.Load("#define D\n#ifdef D\nvoid main() { }\n#endif");

        var options = CustomOptions.TranspileOptions();
        var rootNode = new Parser(lexer)
            .Parse()
            .Simplify(options);

        Assert.That(rootNode.ToCode(options).ToSimple(), Is.EqualTo("void main() { }"));
    }
    
    [Test]
    public void GivenNeverTrueIfDefWithFalseBranchCheckTrueBranchIsRemoved()
    {
        var lexer = new Lexer();
        lexer.Load("#ifdef D\nvoid main() { }\n#else\nvoid f() { }\n#endif");

        var options = CustomOptions.TranspileOptions();
        options.RemoveUnusedFunctions = false;
        var rootNode = new Parser(lexer)
            .Parse()
            .Simplify(options);

        Assert.That(rootNode.ToCode(options).ToSimple(), Is.EqualTo("void f() { }"));
    }
    
    [Test]
    public void GivenNeverTrueIfDefWithoutFalseBranchCheckExpressionIsRemoved()
    {
        var lexer = new Lexer();
        lexer.Load("#ifdef D\nvoid main() { }\n#endif");

        var options = CustomOptions.TranspileOptions();
        options.RemoveUnusedFunctions = false;
        var rootNode = new Parser(lexer)
            .Parse()
            .Simplify(options);

        Assert.That(rootNode.ToCode(options).ToSimple(), Is.Empty);
    }
    
    [Test]
    public void GivenFunctionLikeDefineCheckInliningV1()
    {
        var lexer = new Lexer();
        lexer.Load("#define S(a,b,c) smoothstep(a,b,c)\nfloat f(float f) { return S(1.0,2.0,f); }");

        var options = CustomOptions.TranspileOptions();
        var rootNode = new Parser(lexer)
            .Parse()
            .Simplify(options);

        Assert.That(rootNode.ToCode(options).ToSimple(), Is.EqualTo("float f(float f) { return smoothstep(1.0f, 2.0f, f); }"));
    }
    
    [Test]
    public void GivenFunctionLikeDefineCheckInliningV2()
    {
        var lexer = new Lexer();
        lexer.Load("#define S(a,b,c) custom(a,b,c)\nfloat f(float f) { return S(1.0,2.0,f); }");

        var options = CustomOptions.TranspileOptions();
        var rootNode = new Parser(lexer)
            .Parse()
            .Simplify(options);

        Assert.That(rootNode.ToCode(options).ToSimple(), Is.EqualTo("float f(float f) { return custom(1.0f, 2.0f, f); }"));
    }
    
    [Test]
    public void GivenFunctionLikeDefineCheckInliningV3()
    {
        var lexer = new Lexer();
        lexer.Load("#define S01(a) smoothstep(0.0,1.0,a)\nfloat f(float f) { return S01(f); }");

        var options = CustomOptions.TranspileOptions();
        var rootNode = new Parser(lexer)
            .Parse()
            .Simplify(options);

        Assert.That(rootNode.ToCode(options).ToSimple(), Is.EqualTo("float f(float f) { return smoothstep(0.0f, 1.0f, f); }"));
    }
    
    [Test]
    public void GivenFunctionLikeDefineWithArgReorderingCheckInlining()
    {
        var lexer = new Lexer();
        lexer.Load("#define S(a,b) swap(b,a)\nfloat f() { return S(1.0,2.0); }");

        var options = CustomOptions.TranspileOptions();
        var rootNode = new Parser(lexer)
            .Parse()
            .Simplify(options);

        Assert.That(rootNode.ToCode(options).ToSimple(), Is.EqualTo("float f() { return swap(2.0f, 1.0f); }"));
    }
    
    [Test]
    public void GivenFunctionLikeDefineReferencingAnotherDefineCheckInlining()
    {
        var lexer = new Lexer();
        lexer.Load("#define S smoothstep\n#define S01(a) S(0.0,1.0,a)\nfloat f(float f) { return S01(f); }");

        var options = CustomOptions.TranspileOptions();
        var rootNode = new Parser(lexer)
            .Parse()
            .Simplify(options);

        Assert.That(rootNode.ToCode(options).ToSimple(), Is.EqualTo("float f(float f) { return smoothstep(0.0f, 1.0f, f); }"));
    }

    [Test]
    public void GivenWordReplacementDefineCheckInlining()
    {
        var lexer = new Lexer();
        lexer.Load("#define S smoothstep\nfloat f(float f) { return S(1.0,2.0,f); }");

        var options = CustomOptions.TranspileOptions();
        var rootNode = new Parser(lexer)
            .Parse()
            .Simplify(options);

        Assert.That(rootNode.ToCode(options).ToSimple(), Is.EqualTo("float f(float f) { return smoothstep(1.0f, 2.0f, f); }"));
    }
    
    [Test]
    public void GiveFunctionLikeDefineCheckInlining()
    {
        var lexer = new Lexer();
        lexer.Load("#define rgba(col) vec4(col * fade, 0)\nvec4 f() { vec3 c; float fade; return rgba(c); }");

        var options = CustomOptions.TranspileOptions();
        var rootNode = new Parser(lexer)
            .Parse()
            .Simplify(options);

        var code = rootNode.ToCode(options).ToSimple();
        Assert.That(code, Does.Not.Contain("#define"));
        Assert.That(code, Does.Contain("return vec4(c * fade, 0);"));
    }
    
    [Test]
    public void GivenFunctionLikeDefineCheckInliningWhenSourceHasDuplicateParamReferences()
    {
        var lexer = new Lexer();
        lexer.Load("#define dub(v, v) float(v + v)\nfloat f() { return dub(1.0f); }");

        var options = CustomOptions.TranspileOptions();
        var rootNode = new Parser(lexer)
            .Parse()
            .Simplify(options);

        var code = rootNode.ToCode(options).ToSimple();
        Assert.That(code, Does.Not.Contain("#define"));
        Assert.That(code, Does.Contain("float f() { return toFloat(1.0f + 1.0f); }"));
    }
    
    [Test]
    public void GivenMacroNameToFunctionDefineCheckInlining()
    {
        var lexer = new Lexer();
        lexer.Load("#define I0 min(iFrame, 0)\nfloat f() { return I0; }");

        var options = CustomOptions.TranspileOptions();
        var rootNode = new Parser(lexer)
            .Parse()
            .Simplify(options);

        var code = rootNode.ToCode(options).ToSimple();
        Assert.That(code, Does.Not.Contain("#define"));
        Assert.That(code, Does.Contain("float f() { return min(iFrame, 0); }"));
    }
    
    [Test]
    public void CheckInliningWithinForStatement()
    {
        var lexer = new Lexer();
        lexer.Load("#define I0 min(iFrame, 0)\nvoid f() { for (int i = I0; i < 2; i++) { } }");

        var options = CustomOptions.TranspileOptions();
        var rootNode = new Parser(lexer)
            .Parse()
            .Simplify(options);

        var code = rootNode.ToCode(options).ToSimple();
        Assert.That(code, Is.EqualTo("void f() { for (int i = min(iFrame, 0); i < 2; i++) { } }"));
    }
}