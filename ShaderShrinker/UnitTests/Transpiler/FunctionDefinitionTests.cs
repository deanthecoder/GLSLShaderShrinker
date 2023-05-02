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
    public void CheckInParamsAreClonedBeforeUse()
    {
        var lexer = new Lexer();
        lexer.Load("struct S { }; void main(inout vec2 v1, in vec2 v2, out vec2 v3, in S s, in int n) { }");

        var options = CustomOptions.TranspileOptions();
        var rootNode = new Parser(lexer)
            .Parse()
            .Simplify(options);

        Assert.That(rootNode.ToCode(options).ToSimple(), Does.Not.Contain("Clone(ref v1)"));
        Assert.That(rootNode.ToCode(options).ToSimple(), Does.Contain("Clone(ref v2)"));
        Assert.That(rootNode.ToCode(options).ToSimple(), Does.Not.Contain("Clone(ref v3)"));
        Assert.That(rootNode.ToCode(options).ToSimple(), Does.Contain("Clone(ref s)"));
        Assert.That(rootNode.ToCode(options).ToSimple(), Does.Not.Contain("Clone(ref n)"));
    }
}