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

namespace GLSLRenderer;

// ReSharper disable once InconsistentNaming
public class vec4 : VectorBase
{
    public vec4(VectorBase v)
        : this(v.Components)
    {
    }

    public vec4(VectorBase v, params float[] f) : this(v.Components.Concat(f).ToArray())
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
    
    public static vec4 operator -(vec4 v) => new(-v.x, -v.y, -v.z, -v.w);

    public static vec4 operator -(vec4 v1, vec4 v2) => new(v1.Sub(v2));
    public static vec4 operator -(float v1, vec4 v2) => new(new vec4(v1).Sub(v2));
    public static vec4 operator -(vec4 v1, float v2) => new(v1.Sub(v2));
    public static vec4 operator +(vec4 v1, vec4 v2) => new(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z, v1.w + v2.w);
    public static vec4 operator +(float v1, vec4 v2) => v2 + v1;
    public static vec4 operator +(vec4 v1, float v2) => new(v1.x + v2, v1.y + v2, v1.z + v2, v1.w + v2);
    public static vec4 operator /(vec4 v1, vec4 v2) => new(v1.Div(v2));
    public static vec4 operator /(float v1, vec4 v2) => new(new vec4(v1).Div(v2));
    public static vec4 operator /(vec4 v1, float v2) => new(v1.Div(v2));
    public static vec4 operator *(vec4 v1, vec4 v2) => new(v1.Mul(v2));
    public static vec4 operator *(float v1, vec4 v2) => new(new vec4(v1).Mul(v2));
    public static vec4 operator *(vec4 v1, float v2) => new(v1.Mul(v2));
    
    public vec4 Clone() => new(Components);
}