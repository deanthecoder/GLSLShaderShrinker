// -----------------------------------------------------------------------
//  <copyright file="mat4.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

namespace Transpiler;

public class mat4 : MatN<vec4>
{
    public mat4()
        : this(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
    {
    }

    public mat4(float a)
        : this(a, 0, 0, 0, 0, a, 0, 0, 0, 0, a, 0, 0, 0, 0, a)
    {
    }

    public mat4(params float[] a)
        : base(new vec4(a[0], a[1], a[2], a[3]), new vec4(a[4], a[5], a[6], a[7]), new vec4(a[8], a[9], a[10], a[11]), new vec4(a[12], a[13], a[14], a[15]))
    {
    }

    public mat4(vec4 c1, vec4 c2, vec4 c3, vec4 c4)
        : this(c1.x, c1.y, c1.z, c1.w, c2.x, c2.y, c2.z, c2.w, c3.x, c3.y, c3.z, c3.w, c4.x, c4.y, c4.z, c4.w)
    {
    }
}