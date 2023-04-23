// -----------------------------------------------------------------------
//  <copyright file="mat2.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

namespace Transpiler;

public class mat2 : MatN<vec2>
{
    public mat2()
        : this(0, 0, 0, 0)
    {
    }
    
    public mat2(float a)
        : this(a, 0, 0, a)
    {
    }
    
    public mat2(params float[] a)
        : base(new vec2(a[0], a[1]), new vec2(a[2], a[3]))
    {
    }

    public mat2(vec2 c1, vec2 c2)
        : this(c1.x, c1.y, c2.x, c2.y)
    {
    }
}