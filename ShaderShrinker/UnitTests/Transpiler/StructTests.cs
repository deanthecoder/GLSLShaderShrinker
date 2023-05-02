// -----------------------------------------------------------------------
//  <copyright file="StructTests.cs" company="Dean Edis">
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
public class StructTests
{
    [Test]
    public void CheckStructCreationUsesNewOperator()
    {
        var lexer = new Lexer();
        lexer.Load("struct S { int a; }; void f() { S f = S(1); }");

        var options = CustomOptions.TranspileOptions();
        options.RemoveUnusedVariables = false;
        var rootNode = new Parser(lexer)
            .Parse()
            .Simplify(options);

        Assert.That(rootNode.ToCode(options).ToSimple(), Does.Contain("void f() { S f = new S(1); }"));
    }
    
    [Test]
    public void CheckStructContainsConstructor()
    {
        var lexer = new Lexer();
        lexer.Load("struct S { int a, b; vec3 v; };");

        var options = CustomOptions.TranspileOptions();
        options.RemoveUnusedVariables = false;
        var rootNode = new Parser(lexer)
            .Parse()
            .Simplify(options);

        Assert.That(rootNode.ToCode(options).ToSimple(), Does.Contain("public S(int a_, int b_, vec3 v_) { a = a_; b = b_; v = Clone(ref v_); }"));
    }
}