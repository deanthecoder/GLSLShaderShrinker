// -----------------------------------------------------------------------
//  <copyright file="vec2.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Diagnostics;

namespace Transpiler;

public class vec2 : VectorBase
{
    public vec2(VectorBase v)
        : this(v.Components)
    {
    }

    public vec2(params float[] f)
        : base(2, f)
    {
    }

    public vec2(float f)
        : this(f, f)
    {
    }

    public vec2()
        : this(0.0f)
    {
    }

    public static vec2 operator -(vec2 v1, vec2 v2) => new(v1.Sub(v2));
    public static vec2 operator -(float v1, vec2 v2) => new(new vec2(v1).Sub(v2));
    public static vec2 operator -(vec2 v1, float v2) => new(v1.Sub(v2));
    public static vec2 operator +(vec2 v1, vec2 v2) => new(v1.Add(v2));
    public static vec2 operator +(float v1, vec2 v2) => new(new vec2(v1).Add(v2));
    public static vec2 operator +(vec2 v1, float v2) => new(v1.Add(v2));
    public static vec2 operator /(vec2 v1, vec2 v2) => new(v1.Div(v2));
    public static vec2 operator /(float v1, vec2 v2) => new(new vec2(v1).Div(v2));
    public static vec2 operator /(vec2 v1, float v2) => new(v1.Div(v2));
    public static vec2 operator *(vec2 v1, vec2 v2) => new(v1.Mul(v2));
    public static vec2 operator *(float v1, vec2 v2) => new(new vec2(v1).Mul(v2));
    public static vec2 operator *(vec2 v1, float v2) => new(v1.Mul(v2));
    public static vec2 operator *(vec2 v, mat2 m) =>
        new(
            v.x * m[0].x + v.y * m[0].y,
            v.x * m[1].x + v.y * m[1].y
           );

    public override object Clone() => new vec2(Components);
}