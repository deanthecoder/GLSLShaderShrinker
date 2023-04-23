// -----------------------------------------------------------------------
//  <copyright file="FunctionTests.cs" company="Dean Edis">
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
public class GlslFunctionTests
{
    [Test]
    public void CheckAbs()
    {
        Assert.That(Program.abs(-3.0f), Is.EqualTo(3.0f));
        Assert.That(Program.abs(3.0f), Is.EqualTo(3.0f));
        Assert.That(Program.abs(new vec2(-1, -2)).Components, Is.EqualTo(new[] { 1.0f, 2.0f }));
    }

    [Test]
    public void CheckClamp()
    {
        Assert.That(Program.clamp(0.5f, 0.0f, 1.0f), Is.EqualTo(0.5f));
        Assert.That(Program.clamp(-0.5f, 0.0f, 1.0f), Is.EqualTo(0.0f));
        Assert.That(Program.clamp(1.5f, 0.0f, 1.0f), Is.EqualTo(1.0f));
        
        Assert.That(Program.clamp(new vec2(0.5f, 1.5f), 0.0f, 1.0f).Components, Is.EqualTo(new[] { 0.5f, 1.0f }));
        Assert.That(Program.clamp(new vec2(-0.5f, 0.5f), 0.0f, 1.0f).Components, Is.EqualTo(new[] { 0.0f, 0.5f }));
        
        Assert.That(Program.clamp(new vec2(-0.5f, 1.5f), new vec2(0), new vec2(1)).Components, Is.EqualTo(new[] { 0.0f, 1.0f }));
    }

    [Test]
    public void CheckFloor()
    {
        Assert.That(Program.floor(1.9f), Is.EqualTo(1.0f));
        Assert.That(Program.floor(new vec2(5.5f, 1.9f)).Components, Is.EqualTo(new[] { 5.0f, 1.0f }));
    }

    [Test]
    public void CheckFract()
    {
        Assert.That(Program.fract(1.9f), Is.EqualTo(0.9f));
        Assert.That(Program.fract(new vec2(5.5f, 1.9f)).Components, Is.EqualTo(new[] { 0.5f, 0.9f }));
    }

    [Test]
    public void CheckPow()
    {
        Assert.That(Program.pow(2.0f, 3.0f), Is.EqualTo(8.0f));
        Assert.That(Program.pow(new vec2(2.0f, 3.0f), new vec2(2.0f, 3.0f)).Components, Is.EqualTo(new[] { 4.0f, 27.0f }));
    }
    
    [Test]
    public void CheckDot()
    {
        Assert.That(Program.dot(2.0f, 3.0f), Is.EqualTo(6.0f));
        Assert.That(Program.dot(new vec2(2.0f, 3.0f), new vec2(2.0f, 3.0f)), Is.EqualTo(13.0f));
    }
    
    [Test]
    public void CheckCross()
    {
        Assert.That(Program.cross(new vec3(2, 3, 4), new vec3(5, 6, 7)).Components, Is.EqualTo(new[] { -3.0f, 6.0f, -3.0f }));
    }
    
    [Test]
    public void CheckNormalize()
    {
        Assert.That(Program.normalize(new vec2(0, 1)).Components, Is.EqualTo(new[] { 0.0f, 1.0f }));
        Assert.That(Program.normalize(new vec2(1, 0)).Components, Is.EqualTo(new[] { 1.0f, 0.0f }));
        Assert.That(Program.normalize(new vec2(1, 1)).x, Is.EqualTo(0.707f).Within(0.001f));
        Assert.That(Program.normalize(new vec2(1, 1)).y, Is.EqualTo(0.707f).Within(0.001f));
    }
    
    [Test]
    public void CheckLength()
    {
        Assert.That(Program.length(2.0f), Is.EqualTo(2.0f));
        Assert.That(Program.length(-2.0f), Is.EqualTo(2.0f));
        Assert.That(Program.length(new vec2(2.0f, 3.0f)), Is.EqualTo(3.6f).Within(0.1f));
    }

    [Test]
    public void CheckDistance()
    {
        Assert.That(Program.distance(new vec2(0), new vec2(2.0f, 3.0f)), Is.EqualTo(3.6f).Within(0.1f));
        Assert.That(Program.distance(new vec2(2.0f, 3.0f), new vec2(0)), Is.EqualTo(3.6f).Within(0.1f));
        Assert.That(Program.distance(new vec2(-1), new vec2(1)), Is.EqualTo(2.82f).Within(0.1f));
    }

    [Test]
    public void CheckSign()
    {
        Assert.That(Program.sign(0.0f), Is.Zero);
        Assert.That(Program.sign(-2.2f), Is.EqualTo(-1.0f));
        Assert.That(Program.sign(3.3f), Is.EqualTo(1.0f));
        
        Assert.That(Program.sign(new vec2(0)).Components, Is.EqualTo(new[] { 0.0f, 0.0f }));
        Assert.That(Program.sign(new vec2(-1, 2)).Components, Is.EqualTo(new[] { -1.0f, 1.0f }));
    }
    
    [Test]
    public void CheckMod()
    {
        Assert.That(Program.mod(0.0f, 0.0f), Is.Zero);
        Assert.That(Program.mod(0.0f, 2.0f), Is.Zero);
        Assert.That(Program.mod(2.0f, 2.0f), Is.Zero);
        Assert.That(Program.mod(1.0f, 2.0f), Is.EqualTo(1.0f));
        Assert.That(Program.mod(2.5f, 2.0f), Is.EqualTo(0.5f));
        
        Assert.That(Program.mod(new vec3(0.0f, 1.0f, 2.5f), 2.0f).Components, Is.EqualTo(new[] { 0.0f, 1.0f, 0.5f }));
        Assert.That(Program.mod(new vec3(0.0f, 1.0f, 2.5f), new vec3(2.0f)).Components, Is.EqualTo(new[] { 0.0f, 1.0f, 0.5f }));
    }
    
    [Test]
    public void CheckMin()
    {
        Assert.That(Program.min(2.5f, 2.0f), Is.EqualTo(2.0f));
        Assert.That(Program.min(-2.5f, 2.0f), Is.EqualTo(-2.5f));
        
        Assert.That(Program.min(new vec2(-2.5f, 3.0f), 2.0f).Components, Is.EqualTo(new[] { -2.5f, 2.0f }));
        Assert.That(Program.min(new vec2(-2.5f, 3.0f), new vec2(-3.0f, 2.0f)).Components, Is.EqualTo(new[] { -3.0f, 2.0f }));
    }

    [Test]
    public void CheckMax()
    {
        Assert.That(Program.max(2.5f, 2.0f), Is.EqualTo(2.5f));
        Assert.That(Program.max(-2.5f, 2.0f), Is.EqualTo(2.0f));
        
        Assert.That(Program.max(new vec2(-2.5f, 3.0f), 2.0f).Components, Is.EqualTo(new[] { 2.0f, 3.0f }));
        Assert.That(Program.max(new vec2(-2.5f, 3.0f), new vec2(-3.0f, 2.0f)).Components, Is.EqualTo(new[] { -2.5f, 3.0f }));
    }
    
    [Test]
    public void CheckMix()
    {
        Assert.That(Program.mix(0.0f, 10.0f, 0.3f), Is.EqualTo(3.0f));
        Assert.That(Program.mix(new vec2(-10, 0), new vec2(0, 10), 0.3f).Components, Is.EqualTo(new[] { -7.0f, 3.0f }));
        Assert.That(Program.mix(new vec2(-10, 0), new vec2(0, 10), new vec2(0.3f)).Components, Is.EqualTo(new[] { -7.0f, 3.0f }));
    }
}