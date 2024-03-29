// -----------------------------------------------------------------------
//  <copyright file="vec3.cs" company="Dean Edis">
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
public class vec3 : VectorBase
{
    public vec3(VectorBase v)
        : this(v.Components)
    {
    }

    public vec3(VectorBase v, params float[] f) : this(v.Components.Concat(f).ToArray())
    {
    }

    public vec3(params float[] f)
        : base(3, f)
    {
    }
    
    public vec3(float f)
        : this(f, f, f)
    {
    }

    public vec3()
        : this(0)
    {
    }

    public static vec3 operator -(vec3 v) => new(-v.x, -v.y, -v.z);

    public static vec3 operator -(vec3 v1, vec3 v2) => new(v1.Sub(v2));
    public static vec3 operator -(float v1, vec3 v2) => new(new vec3(v1).Sub(v2));
    public static vec3 operator -(vec3 v1, float v2) => new(v1.Sub(v2));
    public static vec3 operator +(vec3 v1, vec3 v2) => new(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
    public static vec3 operator +(float v1, vec3 v2) => v2 + v1;
    public static vec3 operator +(vec3 v1, float v2) => new(v1.x + v2, v1.y + v2, v1.z + v2);
    public static vec3 operator /(vec3 v1, vec3 v2) => new(v1.Div(v2));
    public static vec3 operator /(float v1, vec3 v2) => new(new vec3(v1).Div(v2));
    public static vec3 operator /(vec3 v1, float v2) => new(v1.Div(v2));
    public static vec3 operator *(vec3 v1, vec3 v2) => new(v1.Mul(v2));
    public static vec3 operator *(float v1, vec3 v2) => new(new vec3(v1).Mul(v2));
    public static vec3 operator *(vec3 v1, float v2) => new(v1.Mul(v2));
    
    public vec3 Clone() => new(Components);
}