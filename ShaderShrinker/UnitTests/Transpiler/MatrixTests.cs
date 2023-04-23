// -----------------------------------------------------------------------
//  <copyright file="MatrixTests.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using NUnit.Framework;
using Transpiler;

namespace UnitTests.Transpiler;

[TestFixture]
public class MatrixTests
{
    [Test]
    public void CheckConstruction()
    {
        var m = new mat2(1.0f);
        Assert.That(m[0].Components, Is.EqualTo(new[] { 1.0f, 0.0f }));
        Assert.That(m[1].Components, Is.EqualTo(new[] { 0.0f, 1.0f }));
        
        m = new mat2();
        Assert.That(m[0].Components, Is.EqualTo(new[] { 0.0f, 0.0f }));
        Assert.That(m[1].Components, Is.EqualTo(new[] { 0.0f, 0.0f }));
        
        m = new mat2(1.0f, 2.0f, 3.0f, 4.0f);
        Assert.That(m[0].Components, Is.EqualTo(new[] { 1.0f, 2.0f }));
        Assert.That(m[1].Components, Is.EqualTo(new[] { 3.0f, 4.0f }));
        
        m = new mat2(new vec2(1.0f, 2.0f), new vec2(3.0f, 4.0f));
        Assert.That(m[0].Components, Is.EqualTo(new[] { 1.0f, 2.0f }));
        Assert.That(m[1].Components, Is.EqualTo(new[] { 3.0f, 4.0f }));
    }

    [Test]
    public void CheckVectorMatrixMultiplication()
    {
        var v = new vec2(3, 4);
        var m = new mat2(2, 1, 0, 3);
        Assert.That(v * m, Is.EqualTo(new vec2(10, 12)));
    }
    
    [Test]
    public void CheckEquality()
    {
        var m = new mat2(1, 3, 2, 4);
        Assert.That(m, Is.EqualTo(m));
        Assert.That(m, Is.EqualTo(new mat2(1, 3, 2, 4)));
        
        Assert.That(new mat2(1, 2, 3, 4), Is.Not.EqualTo(m));
    }
}