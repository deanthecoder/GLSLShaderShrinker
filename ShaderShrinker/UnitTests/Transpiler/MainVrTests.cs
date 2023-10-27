// -----------------------------------------------------------------------
//  <copyright file="MainVrTests.cs" company="Dean Edis">
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
public class MainVrTests
{
    [Test]
    public void CheckMainVrFunctionIsRemoved()
    {
        var lexer = new Lexer();
        lexer.Load("void mainVR() { } void main() { }");

        var options = CustomOptions.TranspileOptions();
        options.RemoveUnusedFunctions = false;
        var rootNode = new Parser(lexer)
            .Parse()
            .Simplify(options);

        Assert.That(rootNode.ToCode(options).ToSimple(), Is.EqualTo("void main() { }"));
    }
}