// -----------------------------------------------------------------------
//  <copyright file="TranspiledOutputTests.cs" company="Dean Edis">
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
public class TranspiledOutputTests
{
    [Test]
    public void CheckForStatementsOutputCorrectly()
    {
        var lexer = new Lexer();
        lexer.Load("vec2 f(vec3 p) { for (int i = 0; i < 2; i++) { } }");

        var options = CustomOptions.TranspileOptions();
        var rootNode = new Parser(lexer)
            .Parse()
            .Simplify(options);

        Assert.That(rootNode.ToCode(options).ToSimple(), Is.Not.Empty);
    }
    
    [Test]
    public void CheckDirectVectorAssignmentClonesSource()
    {
        var lexer = new Lexer();
        lexer.Load("void f() { vec2 v1 = vec2(1); vec2 v2 = v1; }");

        var options = CustomOptions.None();
        options.TranspileToCSharp = true;
        var rootNode = new Parser(lexer)
            .Parse()
            .Simplify(options);

        Assert.That(rootNode.ToCode(options).ToSimple(), Does.Contain("vec2 v2 = v1.Clone();"));
    }
}