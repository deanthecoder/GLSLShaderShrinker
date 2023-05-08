// -----------------------------------------------------------------------
//  <copyright file="GLSLProgBase.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace GLSLRenderer;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
public class GLSLProgBase
{
    protected GLSLProgBase(vec2 resolution, float time)
    {
        iResolution = new vec3(resolution, 1.0f);
        iTime = time;
    }

    public vec3 iResolution { get; }
    public float iTime { get; }
    public int iFrame => (int)Math.Floor(iTime * 60.0);
    public static vec4 iMouse => new(320.0f, 180.0f, 0.0f, 0.0f);

    public static T Clone<T>(ref T o)
    {
        o = o.DeepClone();
        return o;
    }

    public static vec2 vec2(params float[] f) => new(f);
    public static vec2 vec2(VectorBase v, params float[] f) => new(v.Components.Concat(f).ToArray());
    public static vec2 vec2(VectorBase v1, VectorBase v2) => new(v1.Components.Concat(v2.Components).ToArray());
    public static vec3 vec3(params float[] f) => new(f);
    public static vec3 vec3(VectorBase v, params float[] f) => new(v.Components.Concat(f).ToArray());
    public static vec3 vec3(VectorBase v1, VectorBase v2) => new(v1.Components.Concat(v2.Components).ToArray());
    public static vec4 vec4(params float[] f) => new(f);
    public static vec4 vec4(VectorBase v, params float[] f) => new(v.Components.Concat(f).ToArray());
    public static vec4 vec4(VectorBase v1, VectorBase v2) => new(v1.Components.Concat(v2.Components).ToArray());
    public static vec4 vec4(float a, float b, VectorBase v1) => vec4(vec2(a, b), v1);
    public static vec4 vec4(float a, VectorBase v1, float b) => new(new[] { a }.Concat(v1.Components).Append(b).ToArray());
    public static mat2 mat2(params float[] f) => new(f);

    public static int toInt(in float f) => (int)f;
    public static float toFloat(in int n) => n;
    
    public static float abs(in float v) => Math.Abs(v);

    public static T abs<T>(in T v)
        where T : VectorBase, new()
    {
        var components = new float[v.Components.Length];
        for (var i = 0; i < components.Length; i++)
            components[i] = abs(v[i]);
        return new() { Components = components };
    }

    public static float clamp(in float v, in float min, in float max) =>
        Math.Clamp(v, min, max);

    public static T clamp<T>(T v, VectorBase min, VectorBase max)
        where T : VectorBase, new() =>
        new()
            { Components = v.Components.Select((o, i) => clamp(o, min.Components[i], max.Components[i])).ToArray() };

    public static T clamp<T>(T v, float min, float max)
        where T : VectorBase, new() =>
        new()
            { Components = v.Components.Select(o => clamp(o, min, max)).ToArray() };

    public static float floor(float v) => (float)Math.Floor(v);
    public static T floor<T>(T v)
        where T : VectorBase, new() =>
        new()
            { Components = v.Components.Select(floor).ToArray() };

    public static float round(float v) => floor(v + 0.5f);
    public static T round<T>(T v)
        where T : VectorBase, new() =>
        new()
            { Components = v.Components.Select(round).ToArray() };

    public static float fract(float v) => v - (float)Math.Floor(v);
    public static T fract<T>(T v)
        where T : VectorBase, new() =>
        new()
            { Components = v.Components.Select(fract).ToArray() };

    public static float pow(float v1, float v2)
    {
        if (v1 < 0.0f)
            throw new ArgumentOutOfRangeException(nameof(v1), v1, "The base of the pow() function must be a non-negative number.");
        return (float)Math.Pow(v1, v2);
    }

    public static T pow<T>(T v1, VectorBase v2)
        where T : VectorBase, new() =>
        new()
            { Components = v1.Components.Select((o, i) => pow(o, v2.Components[i])).ToArray() };

    public static float sqrt(float v1)
    {
        if (v1 < 0.0f)
            throw new ArgumentOutOfRangeException(nameof(v1), v1, "The base of the sqrt() function must be a non-negative number.");
        return (float)Math.Sqrt(v1);
    }

    public static T sqrt<T>(T v1)
        where T : VectorBase, new() =>
        new()
            { Components = v1.Components.Select(sqrt).ToArray() };

    public static float dot(in VectorBase v1, in VectorBase v2)
    {
        var s = 0.0f;
        for (var i = 0; i < v1.Components.Length; i++)
            s += v1[i] * v2[i];
        return s;
    }

    public static vec3 cross(in vec3 x, in vec3 y) =>
        new(
            x.y * y.z - x.z * y.y,
            x.z * y.x - x.x * y.z,
            x.x * y.y - x.y * y.x
           );

    public static T normalize<T>(in T v)
        where T : VectorBase, new()
    {
        var l = length(v);
        return new()
        {
            Components = v.Components.Select(o => l == 0.0f ? 0.0f : o / l).ToArray()
        };
    }

    public static float length(in float v) => abs(v);

    public static float length(in VectorBase v)
    {
        var sum = 0.0f;
        for (var i = 0; i < v.Components.Length; i++)
            sum += v[i] * v[i];
        return (float)Math.Sqrt(sum);
    }

    public static float distance(in VectorBase v1, in VectorBase v2) => length(v2.Sub(v1));

    public static float exp(float f) => (float)Math.Exp(f);

    public static T exp<T>(T v1)
        where T : VectorBase, new() =>
        new() { Components = v1.Components.Select(exp).ToArray() };

    public static float sign(float f) => Math.Sign(f);

    public static T sign<T>(T v1)
        where T : VectorBase, new() =>
        new() { Components = v1.Components.Select(sign).ToArray() };

    public static float mod(float f, float a) => a != 0.0f ? f % a : 0.0f;

    public static T mod<T>(T v1, T v2)
        where T : VectorBase, new() =>
        new() { Components = v1.Components.Select((o, i) => mod(o, v2[i])).ToArray() };

    public static T mod<T>(T v1, float f)
        where T : VectorBase, new() =>
        new() { Components = v1.Components.Select(o => mod(o, f)).ToArray() };

    public static float min(float v1, float v2) =>
        Math.Min(v1, v2);

    public static int min(int v1, int v2) =>
        Math.Min(v1, v2);
    
    public static T min<T>(T v1, T v2)
        where T : VectorBase, new() =>
        new() { Components = v1.Components.Select((o, i) => min(o, v2[i])).ToArray() };

    public static T min<T>(T v1, float f)
        where T : VectorBase, new() =>
        new() { Components = v1.Components.Select(o => min(o, f)).ToArray() };

    public static float max(float v1, float v2) =>
        Math.Max(v1, v2);
    
    public static T max<T>(T v1, T v2)
        where T : VectorBase, new() =>
        new() { Components = v1.Components.Select((o, i) => max(o, v2[i])).ToArray() };

    public static T max<T>(T v1, float f)
        where T : VectorBase, new()
    {
        var components = new float[v1.Components.Length];
        for (var i = 0; i < components.Length; i++)
            components[i] = max(v1[i], f);
        return new() { Components = components };
    }

    public static float mix(float v1, float v2, float f) =>
        v1 + (v2 - v1) * f;

    public static T mix<T>(T v1, T v2, T f)
        where T : VectorBase, new() =>
        new() { Components = v1.Components.Select((o, i) => mix(o, v2[i], f[i])).ToArray() };
    
    public static T mix<T>(T v1, T v2, float f)
        where T : VectorBase, new() =>
        new() { Components = v1.Components.Select((o, i) => mix(o, v2[i], f)).ToArray() };
    
    public static float smoothstep(float edge0, float edge1, float f)
    {
        var t = clamp((f - edge0) / (edge1 - edge0), 0.0f, 1.0f);
        return t * t * (3.0f - 2.0f * t);
    }
    
    public static T smoothstep<T>(T v1, T v2, T f)
        where T : VectorBase, new() =>
        new() { Components = v1.Components.Select((o, i) => smoothstep(o, v2[i], f[i])).ToArray() };
    
    public static T smoothstep<T>(float v1, float v2, T f)
        where T : VectorBase, new() =>
        new() { Components = f.Components.Select(o => smoothstep(v1, v2, o)).ToArray() };

    public static float step(float edge, float x) => x >= edge ? 1.0f : 0.0f;

    public static T step<T>(T edge, T x)
        where T : VectorBase, new() =>
        new() { Components = edge.Components.Select((o, i) => step(o, x[i])).ToArray() };
    
    public static float cos(float v) => (float)Math.Cos(v);

    public static T cos<T>(T v)
        where T : VectorBase, new() =>
        new() { Components = v.Components.Select(cos).ToArray() };

    public static float sin(float v) => (float)Math.Sin(v);

    public static T sin<T>(T v)
        where T : VectorBase, new() =>
        new() { Components = v.Components.Select(sin).ToArray() };

    public static float tan(float v) => (float)Math.Tan(v);

    public static T tan<T>(T v)
        where T : VectorBase, new() =>
        new() { Components = v.Components.Select(tan).ToArray() };

    public static float atan(float y, float x) => (float)Math.Atan2(y, x);

    public static T atan<T>(T v1, T v2)
        where T : VectorBase, new() =>
        new() { Components = v1.Components.Select((o, i) => atan(o, v2[i])).ToArray() };

    public static float atan(float yx) => (float)Math.Atan(yx);

    public static T atan<T>(T v1)
        where T : VectorBase, new() =>
        new() { Components = v1.Components.Select(o => atan(o)).ToArray() };
    
    public static vec3 reflect(vec3 I, vec3 N) => I - 2.0f * N * dot(N, I);
    public static vec3 refract(vec3 I, vec3 N, float eta)
    {
        var k = 1.0f - eta * eta * (1.0f - dot(N, I) * dot(N, I));
        if (k < 0.0f)
            return vec3(0);
        return eta * I - (eta * dot(N, I) + sqrt(k)) * N;
    }

    public static float fwidth(float v) => 0.5f;

    public static T fwidth<T>(T v)
        where T : VectorBase, new() =>
        new()
            { Components = Enumerable.Repeat(0.5f, v.Components.Length).ToArray() };
}