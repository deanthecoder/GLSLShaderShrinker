// -----------------------------------------------------------------------
//  <copyright file="GlslFunctionTests.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using GLSLRenderer;
using NUnit.Framework;

namespace UnitTests.Transpiler;

[TestFixture]
public class GlslFunctionTests
{
    [Test]
    public void CheckAbs()
    {
        Assert.That(GLSLProgBase.abs(-3.0f), Is.EqualTo(3.0f));
        Assert.That(GLSLProgBase.abs(3.0f), Is.EqualTo(3.0f));
        Assert.That(GLSLProgBase.abs(new vec2(-1, -2)).Components, Is.EqualTo(new[] { 1.0f, 2.0f }));
    }

    [Test]
    public void CheckClamp()
    {
        Assert.That(GLSLProgBase.clamp(0.5f, 0.0f, 1.0f), Is.EqualTo(0.5f));
        Assert.That(GLSLProgBase.clamp(-0.5f, 0.0f, 1.0f), Is.EqualTo(0.0f));
        Assert.That(GLSLProgBase.clamp(1.5f, 0.0f, 1.0f), Is.EqualTo(1.0f));
        
        Assert.That(GLSLProgBase.clamp(new vec2(0.5f, 1.5f), 0.0f, 1.0f).Components, Is.EqualTo(new[] { 0.5f, 1.0f }));
        Assert.That(GLSLProgBase.clamp(new vec2(-0.5f, 0.5f), 0.0f, 1.0f).Components, Is.EqualTo(new[] { 0.0f, 0.5f }));
        
        Assert.That(GLSLProgBase.clamp(new vec2(-0.5f, 1.5f), new vec2(0), new vec2(1)).Components, Is.EqualTo(new[] { 0.0f, 1.0f }));
    }

    [Test]
    public void CheckFloor()
    {
        Assert.That(GLSLProgBase.floor(1.9f), Is.EqualTo(1.0f));
        Assert.That(GLSLProgBase.floor(new vec2(5.5f, 1.9f)).Components, Is.EqualTo(new[] { 5.0f, 1.0f }));
    }

    [Test]
    public void CheckFract()
    {
        Assert.That(GLSLProgBase.fract(1.9f), Is.EqualTo(0.9f));
        Assert.That(GLSLProgBase.fract(new vec2(5.5f, 1.9f)).Components, Is.EqualTo(new[] { 0.5f, 0.9f }));
    }

    [Test]
    public void CheckPow()
    {
        Assert.That(GLSLProgBase.pow(2.0f, 3.0f), Is.EqualTo(8.0f));
        Assert.That(GLSLProgBase.pow(new vec2(2.0f, 3.0f), new vec2(2.0f, 3.0f)).Components, Is.EqualTo(new[] { 4.0f, 27.0f }));
    }
    
    [Test]
    public void CheckDot()
    {
        Assert.That(GLSLProgBase.dot(2.0f, 3.0f), Is.EqualTo(6.0f));
        Assert.That(GLSLProgBase.dot(new vec2(2.0f, 3.0f), new vec2(2.0f, 3.0f)), Is.EqualTo(13.0f));
    }
    
    [Test]
    public void CheckSqrt()
    {
        Assert.That(GLSLProgBase.sqrt(4.0f), Is.EqualTo(2.0f));
        Assert.That(GLSLProgBase.sqrt(new vec2(9.0f, 16.0f)).Components, Is.EqualTo(new[] { 3.0f, 4.0f }));
    }
    
    [Test]
    public void CheckCross()
    {
        Assert.That(GLSLProgBase.cross(new vec3(2, 3, 4), new vec3(5, 6, 7)).Components, Is.EqualTo(new[] { -3.0f, 6.0f, -3.0f }));
    }
    
    [Test]
    public void CheckNormalize()
    {
        Assert.That(GLSLProgBase.normalize(new vec2(0, 1)).Components, Is.EqualTo(new[] { 0.0f, 1.0f }));
        Assert.That(GLSLProgBase.normalize(new vec2(1, 0)).Components, Is.EqualTo(new[] { 1.0f, 0.0f }));
        Assert.That(GLSLProgBase.normalize(new vec2(1, 1)).x, Is.EqualTo(0.707f).Within(0.001f));
        Assert.That(GLSLProgBase.normalize(new vec2(1, 1)).y, Is.EqualTo(0.707f).Within(0.001f));
    }
    
    [Test]
    public void CheckLength()
    {
        Assert.That(GLSLProgBase.length(2.0f), Is.EqualTo(2.0f));
        Assert.That(GLSLProgBase.length(-2.0f), Is.EqualTo(2.0f));
        Assert.That(GLSLProgBase.length(new vec2(2.0f, 3.0f)), Is.EqualTo(3.6f).Within(0.1f));
    }

    [Test]
    public void CheckDistance()
    {
        Assert.That(GLSLProgBase.distance(new vec2(0), new vec2(2.0f, 3.0f)), Is.EqualTo(3.6f).Within(0.1f));
        Assert.That(GLSLProgBase.distance(new vec2(2.0f, 3.0f), new vec2(0)), Is.EqualTo(3.6f).Within(0.1f));
        Assert.That(GLSLProgBase.distance(new vec2(-1), new vec2(1)), Is.EqualTo(2.82f).Within(0.1f));
    }

    [Test]
    public void CheckSign()
    {
        Assert.That(GLSLProgBase.sign(0.0f), Is.Zero);
        Assert.That(GLSLProgBase.sign(-2.2f), Is.EqualTo(-1.0f));
        Assert.That(GLSLProgBase.sign(3.3f), Is.EqualTo(1.0f));
        
        Assert.That(GLSLProgBase.sign(new vec2(0)).Components, Is.EqualTo(new[] { 0.0f, 0.0f }));
        Assert.That(GLSLProgBase.sign(new vec2(-1, 2)).Components, Is.EqualTo(new[] { -1.0f, 1.0f }));
    }
    
    [Test]
    public void CheckMod()
    {
        Assert.That(GLSLProgBase.mod(0.0f, 0.0f), Is.Zero);
        Assert.That(GLSLProgBase.mod(0.0f, 2.0f), Is.Zero);
        Assert.That(GLSLProgBase.mod(2.0f, 2.0f), Is.Zero);
        Assert.That(GLSLProgBase.mod(1.0f, 2.0f), Is.EqualTo(1.0f));
        Assert.That(GLSLProgBase.mod(2.5f, 2.0f), Is.EqualTo(0.5f));
        
        Assert.That(GLSLProgBase.mod(new vec3(0.0f, 1.0f, 2.5f), 2.0f).Components, Is.EqualTo(new[] { 0.0f, 1.0f, 0.5f }));
        Assert.That(GLSLProgBase.mod(new vec3(0.0f, 1.0f, 2.5f), new vec3(2.0f)).Components, Is.EqualTo(new[] { 0.0f, 1.0f, 0.5f }));
    }
    
    [Test]
    public void CheckMin()
    {
        Assert.That(GLSLProgBase.min(2.5f, 2.0f), Is.EqualTo(2.0f));
        Assert.That(GLSLProgBase.min(-2.5f, 2.0f), Is.EqualTo(-2.5f));
        
        Assert.That(GLSLProgBase.min(new vec2(-2.5f, 3.0f), 2.0f).Components, Is.EqualTo(new[] { -2.5f, 2.0f }));
        Assert.That(GLSLProgBase.min(new vec2(-2.5f, 3.0f), new vec2(-3.0f, 2.0f)).Components, Is.EqualTo(new[] { -3.0f, 2.0f }));
    }

    [Test]
    public void CheckMax()
    {
        Assert.That(GLSLProgBase.max(2.5f, 2.0f), Is.EqualTo(2.5f));
        Assert.That(GLSLProgBase.max(-2.5f, 2.0f), Is.EqualTo(2.0f));
        
        Assert.That(GLSLProgBase.max(new vec2(-2.5f, 3.0f), 2.0f).Components, Is.EqualTo(new[] { 2.0f, 3.0f }));
        Assert.That(GLSLProgBase.max(new vec2(-2.5f, 3.0f), new vec2(-3.0f, 2.0f)).Components, Is.EqualTo(new[] { -2.5f, 3.0f }));
    }
    
    [Test]
    public void CheckMix()
    {
        Assert.That(GLSLProgBase.mix(0.0f, 10.0f, 0.3f), Is.EqualTo(3.0f));
        Assert.That(GLSLProgBase.mix(new vec2(-10, 0), new vec2(0, 10), 0.3f).Components, Is.EqualTo(new[] { -7.0f, 3.0f }));
        Assert.That(GLSLProgBase.mix(new vec2(-10, 0), new vec2(0, 10), new vec2(0.3f)).Components, Is.EqualTo(new[] { -7.0f, 3.0f }));
    }

    [Test]
    public void CheckFWidth()
    {
        Assert.That(GLSLProgBase.fwidth(1.0f), Is.EqualTo(0.5f));
        Assert.That(GLSLProgBase.fwidth(new vec2(2.2f)).x, Is.EqualTo(0.5f));
        Assert.That(GLSLProgBase.fwidth(new vec2(2.2f)).y, Is.EqualTo(0.5f));
    }
}