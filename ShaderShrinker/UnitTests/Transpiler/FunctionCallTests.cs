// -----------------------------------------------------------------------
//  <copyright file="FunctionCallTests.cs" company="Dean Edis">
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
public class FunctionCallTests
{
    [Test]
    public void GivenFunctionWithInOutParamCheckCallerUsesRefKeyword()
    {
        var lexer = new Lexer();
        lexer.Load("void f(inout vec2 v) { } void g(vec2 v) { } vec2 main() { vec2 a; f(a); g(a); return dot(a, a); }");

        var options = CustomOptions.TranspileOptions();
        var rootNode = new Parser(lexer)
            .Parse()
            .Simplify(options);

        var code = rootNode.ToCode(options).ToSimple();
        Assert.That(code, Does.Contain("f(ref a);"));
        Assert.That(code, Does.Contain("g(a);"));
        Assert.That(code, Does.Contain("dot(a, a);"));
    }
}