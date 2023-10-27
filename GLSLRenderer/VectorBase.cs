// -----------------------------------------------------------------------
//  <copyright file="VectorBase.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace GLSLRenderer;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public partial class VectorBase
{
    private readonly int m_count;

    public static implicit operator VectorBase(in float f) => new(1, f);

    /// <summary>
    /// Constructor ONLY FOR USE when 'components' can be owned by this new object.
    /// </summary>
    private VectorBase(in float[] components)
    {
        m_count = components.Length;
        Components = components;
    }
    
    protected VectorBase(in int count, params float[] components)
    {
        m_count = count;
        Components = new float[count];
        
        if (components.Length == 1 && count > 0)
        {
            // Repeat single value to fill vector.
            for (var i = 0; i < count; i++)
                Components[i] = components[0];
        }
        else
        {
            var min = Math.Min(count, components.Length);
            for (var i = 0; i < min; i++)
                Components[i] = components[i];
        }
    }

    public float[] Components { get; init; }

    // Accessors.
    public float this[in int index]
    {
        get => Components[index];
        set => Components[index] = value;
    }

    public float x
    {
        get => this[0];
        set => this[0] = value;
    }

    public float y
    {
        get => this[1];
        set => this[1] = value;
    }

    public float z
    {
        get => this[2];
        set => this[2] = value;
    }

    public float w
    {
        get => this[3];
        set => this[3] = value;
    }

    public float r
    {
        get => x;
        set => x = value;
    }

    public float g
    {
        get => y;
        set => y = value;
    }

    public float b
    {
        get => z;
        set => z = value;
    }

    public float a
    {
        get => w;
        set => w = value;
    }

    public float s
    {
        get => x;
        set => x = value;
    }

    public float t
    {
        get => y;
        set => y = value;
    }

    public float p
    {
        get => z;
        set => z = value;
    }

    public float q
    {
        get => w;
        set => w = value;
    }

    // Operators.
    public VectorBase Sub(in float v)
    {
        var components = new float[m_count];
        for (var i = 0; i < components.Length; i++)
            components[i] = this[i] - v;
        return new(components);
    }
    
    public VectorBase Sub(in VectorBase v)
    {
        var components = new float[m_count];
        for (var i = 0; i < components.Length; i++)
            components[i] = this[i] - v.Components[i];
        return new(components);
    }

    public VectorBase Div(in float v)
    {
        var components = new float[m_count];
        for (var i = 0; i < components.Length; i++)
            components[i] = this[i] / v;
        return new(components);
    }
    
    public VectorBase Div(in VectorBase v)
    {
        var components = new float[m_count];
        for (var i = 0; i < components.Length; i++)
            components[i] = this[i] / v.Components[i];
        return new(components);
    }

    public VectorBase Mul(in float v)
    {
        var components = new float[m_count];
        for (var i = 0; i < components.Length; i++)
            components[i] = this[i] * v;
        return new(components);
    }
    
    public VectorBase Mul(in VectorBase v)
    {
        var components = new float[m_count];
        for (var i = 0; i < components.Length; i++)
            components[i] = this[i] * v.Components[i];
        return new(components);
    }
    
    // Overloads.
    public override string ToString() =>
        $"({Components.Select(o => o.ToString(CultureInfo.InvariantCulture)).Aggregate((s1, s2) => $"{s1},{s2}")})";

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;
        return Components.SequenceEqual(((VectorBase)obj).Components);
    }

    public static bool operator ==(VectorBase a, VectorBase b)
    {
        const float precision = 1e-6f;
        for (var i = 0; i < a.Components.Length; i++)
        {
            if (Math.Abs(a.x - b.x) > precision)
                return false;
        }
        
        return true;
    }

    public static bool operator !=(VectorBase a, VectorBase b) => !(a == b);

    public override int GetHashCode() => Components.GetHashCode();
}