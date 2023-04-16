using System.Diagnostics;
using System.Text;

// ReSharper disable InconsistentNaming

namespace Transpiler;

public class VectorBase
{
    private float[] m_components;
    
    protected virtual int ComponentCount { get; set; }

    public static implicit operator VectorBase(float f) =>
        new VectorBase(f) { ComponentCount = 1 };

    protected VectorBase(params float[] components)
    {
        Components = components;
    }

    public float[] Components
    {
        get => m_components;
        init => m_components = value.Concat(Enumerable.Repeat(0.0f, ComponentCount)).Take(ComponentCount).ToArray();
    }

    // Methods.

    // Accessors.
    public float this[int index]
    {
        get => Components[index];
        set => Components[index] = value;
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

    public vec2 xy => new(x, y);
    public vec3 xyx => new(x, y, x);
    public vec4 xyxy => new(x, y, x, y);

    public vec3 rgb => new(r, g, b);

    // Operators.
    protected VectorBase Add(VectorBase v) =>
        new(Components.Select((o, i) => o + v[i]).ToArray());

    protected VectorBase Sub(VectorBase v) =>
        new(Components.Select((o, i) => o - v[i]).ToArray());

    protected VectorBase Div(VectorBase v) =>
        new(Components.Select((o, i) => o / v[i]).ToArray());

    protected VectorBase Mul(VectorBase v) =>
        new(Components.Select((o, i) => o * v[i]).ToArray());
}

[DebuggerDisplay("({x}, {y})")]
public class vec2 : VectorBase
{
    protected override int ComponentCount { get; set; } = 2;

    public vec2(VectorBase v)
        : this(v.Components)
    {
    }

    public vec2(params float[] f)
        : base(f)
    {
    }

    public vec2(float f)
        : this(f, f)
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
}

[DebuggerDisplay("({x}, {y}, {z})")]
public class vec3 : VectorBase
{
    protected override int ComponentCount { get; set; } = 3;

    public vec3(VectorBase v)
        : this(v.Components)
    {
    }

    public vec3(params float[] f)
        : base(f)
    {
    }
    
    public vec3(float f)
        : this(f, f, f)
    {
    }

    public vec3()
        : this(0, 0, 0)
    {
    }

    public static vec3 operator -(vec3 v1, vec3 v2) => new(v1.Sub(v2));
    public static vec3 operator -(float v1, vec3 v2) => new(new vec3(v1).Sub(v2));
    public static vec3 operator -(vec3 v1, float v2) => new(v1.Sub(v2));
    public static vec3 operator +(vec3 v1, vec3 v2) => new(v1.Add(v2));
    public static vec3 operator +(float v1, vec3 v2) => new(new vec3(v1).Add(v2));
    public static vec3 operator +(vec3 v1, float v2) => new(v1.Add(v2));
    public static vec3 operator /(vec3 v1, vec3 v2) => new(v1.Div(v2));
    public static vec3 operator /(float v1, vec3 v2) => new(new vec3(v1).Div(v2));
    public static vec3 operator /(vec3 v1, float v2) => new(v1.Div(v2));
    public static vec3 operator *(vec3 v1, vec3 v2) => new(v1.Mul(v2));
    public static vec3 operator *(float v1, vec3 v2) => new(new vec3(v1).Mul(v2));
    public static vec3 operator *(vec3 v1, float v2) => new(v1.Mul(v2));
}

[DebuggerDisplay("({x}, {y}, {z}, {w})")]
public class vec4 : VectorBase
{
    protected override int ComponentCount { get; set; } = 4;

    public vec4(VectorBase v)
        : this(v.Components)
    {
    }

    public vec4(params float[] f)
        : base(f)
    {
    }

    public vec4(float f)
        : this(f, f, f, f)
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
}

internal static class Program
{
    private static vec4 iResolution { get; } = new vec2(640, 360).xyxy;
    private static float iTime { get; } = 2.4f;

    private static vec3 vec2(params float[] f) => new(f);
    private static vec4 vec2(VectorBase v, params float[] f) => new(v.Components.Concat(f).ToArray());
    private static vec4 vec2(VectorBase v1, VectorBase v2) => new(v1.Components.Concat(v2.Components).ToArray());
    private static vec3 vec3(params float[] f) => new(f);
    private static vec4 vec3(VectorBase v, params float[] f) => new(v.Components.Concat(f).ToArray());
    private static vec4 vec3(VectorBase v1, VectorBase v2) => new(v1.Components.Concat(v2.Components).ToArray());
    private static vec3 vec4(params float[] f) => new(f);
    private static vec4 vec4(VectorBase v, params float[] f) => new(v.Components.Concat(f).ToArray());
    private static vec4 vec4(VectorBase v1, VectorBase v2) => new(v1.Components.Concat(v2.Components).ToArray());

    private static T abs<T>(T v)
        where T : VectorBase, new() =>
        new T { Components = v.Components.Select(o => (float)Math.Abs(o)).ToArray() };

    private static T clamp<T>(T v)
        where T : VectorBase, new() =>
        new T { Components = v.Components.Select(o => Math.Clamp(o, 0.0f, 1.0f)).ToArray() };

    private static T floor<T>(T v)
        where T : VectorBase, new() =>
        new T { Components = v.Components.Select(o => (float)Math.Floor(o)).ToArray() };

    private static T fract<T>(T v)
        where T : VectorBase, new() =>
        new T { Components = v.Components.Select(o => o - (float)Math.Floor(o)).ToArray() };

    private static T pow<T>(T v, float f)
        where T : VectorBase, new() =>
        new T { Components = v.Components.Select(o => (float)Math.Pow(o, f)).ToArray() };

    private static T pow<T>(T v1, T v2)
        where T : VectorBase, new() =>
        new T { Components = v1.Components.Select((o, i) => (float)Math.Pow(o, v2.Components[i])).ToArray() };

    private static T cos<T>(T v)
        where T : VectorBase, new() =>
        new() { Components = v.Components.Select(o => (float)Math.Cos(o)).ToArray() };

    private static T sin<T>(T v)
        where T : VectorBase, new() =>
        new() { Components = v.Components.Select(o => (float)Math.Sin(o)).ToArray() };

    private static T tan<T>(T v)
        where T : VectorBase, new() =>
        new() { Components = v.Components.Select(o => (float)Math.Tan(o)).ToArray() };

    private static void WritePPM(string filePath, vec4[] pixels, int width, int height)
    {
        using var stream = new FileStream(filePath, FileMode.Create);

        // Write the PPM header
        var header = Encoding.ASCII.GetBytes($"P6\n{width} {height}\n255\n");
        stream.Write(header, 0, header.Length);

        // Write the pixel data
        var pixelData = new byte[3];
        foreach (var pixel in pixels)
        {
            var rgb = clamp(pixel.rgb) * 255.0f;
            pixelData[0] = (byte)rgb.x;
            pixelData[1] = (byte)rgb.y;
            pixelData[2] = (byte)rgb.z;

            stream.Write(pixelData, 0, pixelData.Length);
        }
    }
    
    private static void Main(string[] args)
    {
        var pixels = Enumerable
            .Range(0, (int)iResolution.y)
            .Reverse()
            .SelectMany(
                        y => Enumerable
                            .Range(0, (int)iResolution.x)
                            .Select(
                                    x =>
                                    {
                                        mainImage(out var fragColor, new vec2(x, y));
                                        return fragColor;
                                    }))
            .ToArray();
        WritePPM("/Users/Dean/Desktop/output.ppm", pixels, (int)iResolution.x, (int)iResolution.y);

        // //var glsl = "void mainImage( out vec4 fragColor, in vec2 fragCoord )\n{\n    // Normalized pixel coordinates (from 0 to 1)\n    vec2 uv = fragCoord/iResolution.xy;\n\n    // Time varying pixel color\n    vec3 col = 0.5 + 0.5*cos(iTime+uv.xyx+vec3(0,2,4));\n\n    // Output to screen\n    fragColor = vec4(col,1.0);\n}"; 
        // // if (string.IsNullOrEmpty(glsl))
        // // {
        // //     Console.WriteLine("Error - Clipboard is empty.");
        // //     return;
        // // }
        //
        // var lexer = new Lexer.Lexer();
        // if (!lexer.Load(glsl))
        // {
        //     Console.WriteLine("Error - Unable to process the GLSL.");
        //     return;
        // }
        //
        // var parser = new Parser.Parser(lexer);
        // try
        // {
        //     Console.WriteLine("Parsing...");
        //     var rootNode = parser.Parse();
        //
        //     Console.WriteLine("Simplifying...");
        //     var options = CustomOptions.All();
        //     options.CombineConsecutiveAssignments = false;
        //     options.RemoveComments = false;
        //     rootNode.Simplify(options);
        //     
        //     Console.WriteLine("Creating GLSL...");
        //     var newGlsl = rootNode.ToCode();
        // }
        // catch (Exception ex)
        // {
        //     Console.WriteLine($"  Error - {ex.Message}");
        // }
    }
    
    private static void mainImage(out vec4 fragColor, in vec2 fragCoord)
    {
        // Normalized pixel coordinates (from 0 to 1)
        var uv = fragCoord / iResolution.xy;

        // Time varying pixel color
        var col = 0.5f + 0.5f * cos(iTime + uv.xyx + vec3(0, 2, 4));

        // Output to screen
        fragColor = vec4(col, 1.0f);
    }
}