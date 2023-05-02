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

    public static implicit operator VectorBase(float f) => new(1, f);

    protected VectorBase(int count, params float[] components)
    {
        m_count = count;
        if (components.Length == 1 && count > 0)
        {
            // Repeat single value to fill vector.
            Components = new float[count];
            for (var i = 0; i < Components.Length; i++)
                Components[i] = components[0];
        }
        else if (components.Length == count)
            Components = components.ToArray();
        else
            Components = components.Take(count).ToArray();
    }

    public float[] Components { get; init; }

    // Accessors.
    public float this[int index]
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
    protected VectorBase Add(float v) =>
        new(m_count, Components.Select(o => o + v).ToArray());
    public VectorBase Add(VectorBase v)
    {
        var components = Components.ToArray();
        for (var i = 0; i < components.Length; i++)
            components[i] += v.Components[i];
        return new(m_count, components);
    }
    
    public VectorBase Sub(float v) =>
        new(m_count, Components.Select(o => o - v).ToArray());
    public VectorBase Sub(VectorBase v)
    {
        var components = Components.ToArray();
        for (var i = 0; i < components.Length; i++)
            components[i] -= v.Components[i];
        return new(m_count, components);
    }

    public VectorBase Div(float v)
    {
        var components = Components.ToArray();
        for (var i = 0; i < components.Length; i++)
            components[i] /= v;
        return new(m_count, components);
    }
    public VectorBase Div(VectorBase v)
    {
        var components = Components.ToArray();
        for (var i = 0; i < components.Length; i++)
            components[i] /= v.Components[i];
        return new(m_count, components);
    }

    public VectorBase Mul(float v)
    {
        var components = Components.ToArray();
        for (var i = 0; i < components.Length; i++)
            components[i] *= v;
        return new(m_count, components);
    }
    public VectorBase Mul(VectorBase v)
    {
        var components = Components.ToArray();
        for (var i = 0; i < components.Length; i++)
            components[i] *= v.Components[i];
        return new(m_count, components);
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

    public override int GetHashCode() => Components.GetHashCode();
}