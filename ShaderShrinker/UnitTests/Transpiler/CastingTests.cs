// -----------------------------------------------------------------------
//  <copyright file="CastingTests.cs" company="Dean Edis">
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
public class CastingTests
{
    [Test]
    public void CheckCastingFloatToInt()
    {
        var lexer = new Lexer();
        lexer.Load("float g() { return 1.23; }\nint f() { return int(1.2 * (1.0 + 2.0) / g()); }");

        var options = CustomOptions.TranspileOptions();
        options.SimplifyArithmetic = false;
        options.PerformArithmetic = false;
        var rootNode = new Parser(lexer)
            .Parse()
            .Simplify(options);

        Assert.That(rootNode.ToCode(options).ToSimple(), Is.EqualTo("float g() { return 1.230f; } int f() { return toInt(1.20f * (1.0f + 2.0f) / g()); }"));
    }
    
    [Test]
    public void CheckCastingIntToFloat()
    {
        var lexer = new Lexer();
        lexer.Load("int g() { return 1; }\nfloat f() { return float(2 * (1 + 2) / g()); }");

        var options = CustomOptions.TranspileOptions();
        options.SimplifyArithmetic = false;
        options.PerformArithmetic = false;
        var rootNode = new Parser(lexer)
            .Parse()
            .Simplify(options);

        Assert.That(rootNode.ToCode(options).ToSimple(), Is.EqualTo("int g() { return 1; } float f() { return toFloat(2 * (1 + 2) / g()); }"));
    }
}