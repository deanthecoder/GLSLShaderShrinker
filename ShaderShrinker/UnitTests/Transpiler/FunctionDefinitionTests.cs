// -----------------------------------------------------------------------
//  <copyright file="FunctionDefinitionTests.cs" company="Dean Edis">
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
public class FunctionDefinitionTests
{
    [Test]
    public void CheckInOutParamsConvertToRef()
    {
        var lexer = new Lexer();
        lexer.Load("void f(inout float a, in float b, out float c) { a++; c = b * 2.0f; }");

        var options = CustomOptions.TranspileOptions();
        options.RemoveUnusedVariables = false;
        var rootNode = new Parser(lexer)
            .Parse()
            .Simplify(options);

        Assert.That(rootNode.ToCode(options).ToSimple(), Is.EqualTo("void f(ref float a, float b, out float c) { a++; c = b * 2.0f; }"));
    }
    
    [Test]
    public void CheckNonInParamsAreNotCloned()
    {
        var lexer = new Lexer();
        lexer.Load("struct S { }; void main(inout vec2 v1, out vec2 v3, in int n) { }");

        var options = CustomOptions.TranspileOptions();
        var rootNode = new Parser(lexer)
            .Parse()
            .Simplify(options);

        Assert.That(rootNode.ToCode(options).ToSimple(), Does.Not.Contain("Clone"));
    }
    
    [Test, Sequential]
    public void CheckNonModifiedInParamsAreNotCloned([Values("struct S { }; void main(in vec2 v2, in S s) { }", "struct S { }; void main(vec2 v2, S s) { }")] string code)
    {
        var lexer = new Lexer();
        lexer.Load(code);

        var options = CustomOptions.TranspileOptions();
        var rootNode = new Parser(lexer)
            .Parse()
            .Simplify(options);

        Assert.That(rootNode.ToCode(options).ToSimple(), Does.Not.Contain("Clone"));
    }
    
    [Test]
    public void CheckModifiedInParamsAreCloned()
    {
        var lexer = new Lexer();
        lexer.Load("struct S { int i; }; void main(vec2 v2, in S s) { v2.x++; s.i *= -1; }");

        var options = CustomOptions.TranspileOptions();
        var rootNode = new Parser(lexer)
            .Parse()
            .Simplify(options);

        Assert.That(rootNode.ToCode(options).ToSimple(), Does.Contain("Clone(ref v2)"));
        Assert.That(rootNode.ToCode(options).ToSimple(), Does.Contain("Clone(ref s)"));
    }
}