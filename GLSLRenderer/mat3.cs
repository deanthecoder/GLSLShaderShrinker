// -----------------------------------------------------------------------
//  <copyright file="mat3.cs" company="Dean Edis">
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
public class mat3 : MatN<vec3>
{
    public mat3()
        : this(0, 0, 0, 0, 0, 0, 0, 0, 0)
    {
    }

    public mat3(float a)
        : this(a, 0, 0, 0, a, 0, 0, 0, a)
    {
    }

    public mat3(params float[] a)
        : base(new vec3(a[0], a[1], a[2]), new vec3(a[3], a[4], a[5]), new vec3(a[6], a[7], a[8]))
    {
    }

    public mat3(vec3 c1, vec3 c2, vec3 c3)
        : this(c1.x, c1.y, c1.z, c2.x, c2.y, c2.z, c3.x, c3.y, c3.z)
    {
    }
    
    public mat3 Clone() => new(m_columns.SelectMany(o => o.Components).ToArray());
}