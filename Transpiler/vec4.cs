// -----------------------------------------------------------------------
//  <copyright file="vec4.cs" company="Dean Edis">
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

public class vec4 : VectorBase
{
    public vec4(VectorBase v)
        : this(v.Components)
    {
    }

    public vec4(params float[] f)
        : base(4, f)
    {
    }

    public vec4(float f)
        : this(f, f, f, f)
    {
    }

    public vec4()
        : this(0.0f)
    {
    }
    
    public vec4(vec3 v, float w)
        : this(v[0], v[1], v[2], w)
    {
    }
    
    public static vec4 operator -(vec4 v1, vec4 v2) => new(v1.Sub(v2));
    public static vec4 operator -(float v1, vec4 v2) => new(new vec4(v1).Sub(v2));
    public static vec4 operator -(vec4 v1, float v2) => new(v1.Sub(v2));
    public static vec4 operator +(vec4 v1, vec4 v2) => new(v1.Add(v2));
    public static vec4 operator +(float v1, vec4 v2) => new(new vec4(v1).Add(v2));
    public static vec4 operator +(vec4 v1, float v2) => new(v1.Add(v2));
    public static vec4 operator /(vec4 v1, vec4 v2) => new(v1.Div(v2));
    public static vec4 operator /(float v1, vec4 v2) => new(new vec4(v1).Div(v2));
    public static vec4 operator /(vec4 v1, float v2) => new(v1.Div(v2));
    public static vec4 operator *(vec4 v1, vec4 v2) => new(v1.Mul(v2));
    public static vec4 operator *(float v1, vec4 v2) => new(new vec4(v1).Mul(v2));
    public static vec4 operator *(vec4 v1, float v2) => new(v1.Mul(v2));
    
    public override object Clone() => new vec4(Components);
}