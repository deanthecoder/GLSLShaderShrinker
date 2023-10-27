// -----------------------------------------------------------------------
//  <copyright file="DeclarationTests.cs" company="Dean Edis">
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
public class DeclarationTests
{
    [Test]
    public void CheckBaseTypesAreAlwaysInitialized()
    {
        var lexer = new Lexer();
        lexer.Load("void f() { vec2 v; mat2 m; }");

        var options = CustomOptions.TranspileOptions();
        options.RemoveUnusedVariables = false;
        var rootNode = new Parser(lexer)
            .Parse()
            .Simplify(options);

        Assert.That(rootNode.ToCode(options).ToSimple(), Is.EqualTo("void f() { vec2 v = new(); mat2 m = new(); }"));
    }
    
    [Test]
    public void CheckStructsAreAlwaysInitialized()
    {
        var lexer = new Lexer();
        lexer.Load("struct S { int d; }; void f() { S s; }");

        var options = CustomOptions.TranspileOptions();
        options.RemoveUnusedVariables = false;
        var rootNode = new Parser(lexer)
            .Parse()
            .Simplify(options);

        Assert.That(rootNode.ToCode(options).ToSimple(), Does.Contain("void f() { S s = new(); }"));
    }
}