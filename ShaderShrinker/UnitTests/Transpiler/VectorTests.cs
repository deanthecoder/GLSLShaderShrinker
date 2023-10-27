// -----------------------------------------------------------------------
//  <copyright file="VectorTests.cs" company="Dean Edis">
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
public class TranspilerTests
{
    [Test]
    public void CheckVectorConstruction()
    {
        Assert.That(new vec2().Components, Is.EqualTo(new[] { 0.0f, 0.0f }));
        Assert.That(new vec2(1).Components, Is.EqualTo(new[] { 1.0f, 1.0f }));
        Assert.That(new vec2(2, 3).Components, Is.EqualTo(new[] { 2.0f, 3.0f }));
        Assert.That(new vec2(2.2f, 3.3f).Components, Is.EqualTo(new[] { 2.2f, 3.3f }));
        Assert.That(new vec2(new vec2(2.2f, 3.3f)).Components, Is.EqualTo(new[] { 2.2f, 3.3f }));

        Assert.That(new vec3().Components, Is.EqualTo(new[] { 0.0f, 0.0f, 0.0f }));
        Assert.That(new vec3(1).Components, Is.EqualTo(new[] { 1.0f, 1.0f, 1.0f }));
        Assert.That(new vec3(2, 3, 4).Components, Is.EqualTo(new[] { 2.0f, 3.0f, 4.0f }));
        Assert.That(new vec3(2.2f, 3.3f, 4.4f).Components, Is.EqualTo(new[] { 2.2f, 3.3f, 4.4f }));
        Assert.That(new vec3(new vec3(2.2f, 3.3f, 4.4f)).Components, Is.EqualTo(new[] { 2.2f, 3.3f, 4.4f }));

        Assert.That(new vec4().Components, Is.EqualTo(new[] { 0.0f, 0.0f, 0.0f, 0.0f }));
        Assert.That(new vec4(1).Components, Is.EqualTo(new[] { 1.0f, 1.0f, 1.0f, 1.0f }));
        Assert.That(new vec4(2, 3, 4, 5).Components, Is.EqualTo(new[] { 2.0f, 3.0f, 4.0f, 5.0f }));
        Assert.That(new vec4(2.2f, 3.3f, 4.4f, 5.5f).Components, Is.EqualTo(new[] { 2.2f, 3.3f, 4.4f, 5.5f }));
        Assert.That(new vec4(new vec4(2.2f, 3.3f, 4.4f, 5.5f)).Components, Is.EqualTo(new[] { 2.2f, 3.3f, 4.4f, 5.5f }));
    }
    
    [Test]
    public void CheckVectorSwizzleGetters()
    {
        Assert.That(new vec2(1, 2).x, Is.EqualTo(1.0f));
        Assert.That(new vec2(1, 2).y, Is.EqualTo(2.0f));
        Assert.That(new vec2(1, 2).xy.Components, Is.EqualTo(new[] { 1.0f, 2.0f }));
        Assert.That(new vec2(1, 2).yx.Components, Is.EqualTo(new[] { 2.0f, 1.0f }));

        Assert.That(new vec3(1, 2, 3).x, Is.EqualTo(1.0f));
        Assert.That(new vec3(1, 2, 3).y, Is.EqualTo(2.0f));
        Assert.That(new vec3(1, 2, 3).z, Is.EqualTo(3.0f));
        Assert.That(new vec3(1, 2, 3).xy.Components, Is.EqualTo(new[] { 1.0f, 2.0f }));
        Assert.That(new vec3(1, 2, 3).yx.Components, Is.EqualTo(new[] { 2.0f, 1.0f }));
        Assert.That(new vec3(1, 2, 3).xyz.Components, Is.EqualTo(new[] { 1.0f, 2.0f, 3.0f }));
        Assert.That(new vec3(1, 2, 3).zyx.Components, Is.EqualTo(new[] { 3.0f, 2.0f, 1.0f }));

        Assert.That(new vec4(1, 2, 3, 4).x, Is.EqualTo(1.0f));
        Assert.That(new vec4(1, 2, 3, 4).y, Is.EqualTo(2.0f));
        Assert.That(new vec4(1, 2, 3, 4).z, Is.EqualTo(3.0f));
        Assert.That(new vec4(1, 2, 3, 4).w, Is.EqualTo(4.0f));
        Assert.That(new vec4(1, 2, 3, 4).xy.Components, Is.EqualTo(new[] { 1.0f, 2.0f }));
        Assert.That(new vec4(1, 2, 3, 4).yx.Components, Is.EqualTo(new[] { 2.0f, 1.0f }));
        Assert.That(new vec4(1, 2, 3, 4).xyz.Components, Is.EqualTo(new[] { 1.0f, 2.0f, 3.0f }));
        Assert.That(new vec4(1, 2, 3, 4).zyx.Components, Is.EqualTo(new[] { 3.0f, 2.0f, 1.0f }));
        Assert.That(new vec4(1, 2, 3, 4).xyzw.Components, Is.EqualTo(new[] { 1.0f, 2.0f, 3.0f, 4.0f }));
        Assert.That(new vec4(1, 2, 3, 4).wzyx.Components, Is.EqualTo(new[] { 4.0f, 3.0f, 2.0f, 1.0f }));
    }
    
    [Test]
    public void CheckVectorSwizzleSetters()
    {
        var v2 = new vec2(1, 2);
        v2.x = 3.0f;
        Assert.That(v2.x, Is.EqualTo(3.0f));
        Assert.That(v2.y, Is.EqualTo(2.0f));
        v2.y = 4.0f;
        Assert.That(v2.x, Is.EqualTo(3.0f));
        Assert.That(v2.y, Is.EqualTo(4.0f));
        v2.xy = new vec2(4, 5);
        Assert.That(v2.xy.Components, Is.EqualTo(new[] { 4.0f, 5.0f }));

        var v3 = new vec3(1, 2, 3);
        v3.x = 3.0f;
        Assert.That(v3.x, Is.EqualTo(3.0f));
        Assert.That(v3.y, Is.EqualTo(2.0f));
        Assert.That(v3.z, Is.EqualTo(3.0f));
        v3.y = 4.0f;
        Assert.That(v3.x, Is.EqualTo(3.0f));
        Assert.That(v3.y, Is.EqualTo(4.0f));
        Assert.That(v3.z, Is.EqualTo(3.0f));
        v3.z = 5.0f;
        Assert.That(v3.x, Is.EqualTo(3.0f));
        Assert.That(v3.y, Is.EqualTo(4.0f));
        Assert.That(v3.z, Is.EqualTo(5.0f));
        v3.xy = new vec2(4, 5);
        Assert.That(v3.Components, Is.EqualTo(new[] { 4.0f, 5.0f, 5.0f }));
        v3.yz = new vec2(1, 2);
        Assert.That(v3.Components, Is.EqualTo(new[] { 4.0f, 1.0f, 2.0f }));
    }
    
    [Test]
    public void CheckVectorAddition()
    {
        Assert.That((new vec2(1, 2) + 2.0f).Components, Is.EqualTo(new[] { 3.0f, 4.0f }));
        Assert.That((new vec2(1, 2) + new vec2(2.0f)).Components, Is.EqualTo(new[] { 3.0f, 4.0f }));
        Assert.That((2.0f + new vec2(1, 2)).Components, Is.EqualTo(new[] { 3.0f, 4.0f }));

        Assert.That((new vec3(1, 2, 3) + 2.0f).Components, Is.EqualTo(new[] { 3.0f, 4.0f, 5.0f }));
        Assert.That((new vec3(1, 2, 3) + new vec3(2.0f)).Components, Is.EqualTo(new[] { 3.0f, 4.0f, 5.0f }));
        Assert.That((2.0f + new vec3(1, 2, 3)).Components, Is.EqualTo(new[] { 3.0f, 4.0f, 5.0f }));

        Assert.That((new vec4(1, 2, 3, 4) + 2.0f).Components, Is.EqualTo(new[] { 3.0f, 4.0f, 5.0f, 6.0f }));
        Assert.That((new vec4(1, 2, 3, 4) + new vec4(2.0f)).Components, Is.EqualTo(new[] { 3.0f, 4.0f, 5.0f, 6.0f }));
        Assert.That((2.0f + new vec4(1, 2, 3, 4)).Components, Is.EqualTo(new[] { 3.0f, 4.0f, 5.0f, 6.0f }));
    }
    
    [Test]
    public void CheckVectorSubtraction()
    {
        Assert.That((new vec2(1, 2) - 2.0f).Components, Is.EqualTo(new[] { -1.0f, 0.0f }));
        Assert.That((new vec2(1, 2) - new vec2(2.0f)).Components, Is.EqualTo(new[] { -1.0f, 0.0f }));
        Assert.That((2.0f - new vec2(1, 2)).Components, Is.EqualTo(new[] { 1.0f, 0.0f }));

        Assert.That((new vec3(1, 2, 3) - 2.0f).Components, Is.EqualTo(new[] { -1.0f, 0.0f, 1.0f }));
        Assert.That((new vec3(1, 2, 3) - new vec3(2.0f)).Components, Is.EqualTo(new[] { -1.0f, 0.0f, 1.0f }));
        Assert.That((2.0f - new vec3(1, 2, 3)).Components, Is.EqualTo(new[] { 1.0f, 0.0f, -1.0f }));

        Assert.That((new vec4(1, 2, 3, 4) - 2.0f).Components, Is.EqualTo(new[] { -1.0f, 0.0f, 1.0f, 2.0f }));
        Assert.That((new vec4(1, 2, 3, 4) - new vec4(2.0f)).Components, Is.EqualTo(new[] { -1.0f, 0.0f, 1.0f, 2.0f }));
        Assert.That((2.0f - new vec4(1, 2, 3, 4)).Components, Is.EqualTo(new[] { 1.0f, 0.0f, -1.0f, -2.0f }));
    }

    [Test]
    public void CheckVectorMultiplication()
    {
        Assert.That((new vec2(1, 2) * 2.0f).Components, Is.EqualTo(new[] { 2.0f, 4.0f }));
        Assert.That((new vec2(1, 2) * new vec2(3, 4)).Components, Is.EqualTo(new[] { 3.0f, 8.0f }));
        Assert.That((2.0f * new vec2(1, 2)).Components, Is.EqualTo(new[] { 2.0f, 4.0f }));

        Assert.That((new vec3(1, 2, 3) * 2.0f).Components, Is.EqualTo(new[] { 2.0f, 4.0f, 6.0f }));
        Assert.That((new vec3(1, 2, 3) * new vec3(3, 4, 2)).Components, Is.EqualTo(new[] { 3.0f, 8.0f, 6.0f }));
        Assert.That((2.0f * new vec3(1, 2, 3)).Components, Is.EqualTo(new[] { 2.0f, 4.0f, 6.0f }));
    }

    [Test]
    public void CheckVectorDivision()
    {
        Assert.That((new vec2(1, 2) / 2.0f).Components, Is.EqualTo(new[] { 0.5f, 1.0f }));
        Assert.That((new vec2(1, 2) / new vec2(2, 8)).Components, Is.EqualTo(new[] { 0.5f, 0.25f }));
        Assert.That((2.0f / new vec2(1, 2)).Components, Is.EqualTo(new[] { 2.0f, 1.0f }));
    }
    
    [Test]
    public void CheckVectorNegation()
    {
        var negated = -new vec2(1, 2);
        Assert.That(negated.Components, Is.EqualTo(new[] { -1.0f, -2.0f }));
    }

    [Test]
    public void CheckConstructionMethods()
    {
        Assert.That(GLSLProgBase.vec2(1, 2).Components, Is.EqualTo(new[] { 1.0f, 2.0f }));
        Assert.That(GLSLProgBase.vec2(1, 2, 3).Components, Is.EqualTo(new[] { 1.0f, 2.0f }));
        Assert.That(GLSLProgBase.vec2(new vec2(1, 2), 3).Components, Is.EqualTo(new[] { 1.0f, 2.0f }));
        Assert.That(GLSLProgBase.vec2(1, new vec2(2, 3)).Components, Is.EqualTo(new[] { 1.0f, 2.0f }));

        Assert.That(GLSLProgBase.vec3(1, 2, 3, 4).Components, Is.EqualTo(new[] { 1.0f, 2.0f, 3.0f }));
        Assert.That(GLSLProgBase.vec3(1, 2, 3, 4, 5).Components, Is.EqualTo(new[] { 1.0f, 2.0f, 3.0f }));
        Assert.That(GLSLProgBase.vec3(new vec2(1, 2), new vec2(3, 4)).Components, Is.EqualTo(new[] { 1.0f, 2.0f, 3.0f }));
        Assert.That(GLSLProgBase.vec3(1, new vec2(2, 3)).Components, Is.EqualTo(new[] { 1.0f, 2.0f, 3.0f }));
        Assert.That(GLSLProgBase.vec3(new vec2(1, 2), 3).Components, Is.EqualTo(new[] { 1.0f, 2.0f, 3.0f }));

        Assert.That(GLSLProgBase.vec4(1, 2, 3, 4).Components, Is.EqualTo(new[] { 1.0f, 2.0f, 3.0f, 4.0f }));
        Assert.That(GLSLProgBase.vec4(1, 2, 3, 4, 5).Components, Is.EqualTo(new[] { 1.0f, 2.0f, 3.0f, 4.0f }));
        Assert.That(GLSLProgBase.vec4(new vec2(1, 2), new vec2(3, 4)).Components, Is.EqualTo(new[] { 1.0f, 2.0f, 3.0f, 4.0f }));
        Assert.That(GLSLProgBase.vec4(new vec2(1, 2), 3, 4).Components, Is.EqualTo(new[] { 1.0f, 2.0f, 3.0f, 4.0f }));
        Assert.That(GLSLProgBase.vec4(1, 2, new vec2(3, 4)).Components, Is.EqualTo(new[] { 1.0f, 2.0f, 3.0f, 4.0f }));
        Assert.That(GLSLProgBase.vec4(1, new vec2(2, 3), 4).Components, Is.EqualTo(new[] { 1.0f, 2.0f, 3.0f, 4.0f }));
    }

    [Test]
    public void CheckEquality()
    {
        var v = new vec2(1, 2);
        Assert.That(v, Is.EqualTo(v));
        Assert.That(v, Is.EqualTo(new vec2(1, 2)));
        
        Assert.That(new vec2(1, 2), Is.Not.EqualTo(new vec3(1, 2, 3)));
        Assert.That(new vec2(2, 1), Is.Not.EqualTo(new vec3(1, 2)));
    }

    [Test]
    public void CheckCloningVec2()
    {
        var v = new vec2(1, 2);
        Assert.That(v.Clone(), Is.Not.SameAs(v));
        Assert.That(v.Clone(), Is.EqualTo(v));

        new vec2(v).x++;
        Assert.That(v.x, Is.EqualTo(1.0f));
        v.Clone().x++;
        Assert.That(v.x, Is.EqualTo(1.0f));
    }

    [Test]
    public void CheckCloningVec3()
    {
        var v = new vec3(1, 2, 3);
        Assert.That(v.Clone(), Is.Not.SameAs(v));
        Assert.That(v.Clone(), Is.EqualTo(v));

        new vec3(v).x++;
        Assert.That(v.x, Is.EqualTo(1.0f));
        v.Clone().x++;
        Assert.That(v.x, Is.EqualTo(1.0f));
    }
    
    [Test]
    public void CheckCloningVec4()
    {
        var v = new vec3(1, 2, 3, 4);
        Assert.That(v.Clone(), Is.Not.SameAs(v));
        Assert.That(v.Clone(), Is.EqualTo(v));

        new vec4(v).x++;
        Assert.That(v.x, Is.EqualTo(1.0f));
        v.Clone().x++;
        Assert.That(v.x, Is.EqualTo(1.0f));
    }
}