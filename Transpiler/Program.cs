using System.Text;

// ReSharper disable InconsistentNaming

namespace Transpiler;

// todo - allow non static methods (by adding Shader class)
// todo - #defines need inlining.
// todo - numbers to 1.0f format.
// todo - uses of struct (Hit(...)) need 'new' prefix.
// todo - structs need constructor to init all fields.
// todo - unassigned declarations of objects (vecN, struct, matN) should have '= new()'

public static class Program
{
    public static vec3 iResolution { get; } = new(320, 180, 1);
    public static float iTime { get; } = 1.0f;

    public static vec2 vec2(params float[] f) => new(f);
    public static vec2 vec2(VectorBase v, params float[] f) => new(v.Components.Concat(f).ToArray());
    public static vec2 vec2(VectorBase v1, VectorBase v2) => new(v1.Components.Concat(v2.Components).ToArray());
    public static vec3 vec3(params float[] f) => new(f);
    public static vec3 vec3(VectorBase v, params float[] f) => new(v.Components.Concat(f).ToArray());
    public static vec3 vec3(VectorBase v1, VectorBase v2) => new(v1.Components.Concat(v2.Components).ToArray());
    public static vec4 vec4(params float[] f) => new(f);
    public static vec4 vec4(VectorBase v, params float[] f) => new(v.Components.Concat(f).ToArray());
    public static vec4 vec4(VectorBase v1, VectorBase v2) => new(v1.Components.Concat(v2.Components).ToArray());
    public static mat2 mat2(params float[] f) => new(f);

    public static float abs(float v) => Math.Abs(v);

    public static T abs<T>(T v)
        where T : VectorBase, new()
    {
        var components = new float[v.Components.Length];
        for (var i = 0; i < components.Length; i++)
            components[i] = abs(v[i]);
        return new() { Components = components };
    }

    public static float clamp(float v, float min, float max) =>
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

    public static float fract(float v) => v - (float)Math.Floor(v);
    public static T fract<T>(T v)
        where T : VectorBase, new() =>
        new()
            { Components = v.Components.Select(fract).ToArray() };
    
    public static float pow(float v1, float v2) => (float)Math.Pow(v1, v2);
    public static T pow<T>(T v1, VectorBase v2)
        where T : VectorBase, new() =>
        new()
            { Components = v1.Components.Select((o, i) => pow(o, v2.Components[i])).ToArray() };

    public static float dot(VectorBase v1, VectorBase v2) => v1.Components.Select((o, i) => o * v2[i]).Sum();
    
    public static vec3 cross(vec3 x, vec3 y) =>
        new(
            x.y * y.z - x.z * y.y,
            x.z * y.x - x.x * y.z,
            x.x * y.y - x.y * y.x
           );

    public static T normalize<T>(T v)
        where T : VectorBase, new()
    {
        var l = length(v);
        return new()
        {
            Components = v.Components.Select(o => l == 0.0f ? 0.0f : o / l).ToArray()
        };
    }

    public static float length(float v) => abs(v);

    public static float length(VectorBase v) =>
        (float)Math.Sqrt(v.Components.Sum(o => o * o));

    public static float distance(VectorBase v1, VectorBase v2) => length(v2.Sub(v1));

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
    
    public static vec3 reflect(vec3 I, vec3 N) => I - 2.0f * N * dot(N, I);

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
            var rgb = clamp(pixel.rgb, vec3(0.0f), vec3(1.0f)) * 255.0f;
            pixelData[0] = (byte)rgb.x;
            pixelData[1] = (byte)rgb.y;
            pixelData[2] = (byte)rgb.z;

            stream.Write(pixelData, 0, pixelData.Length);
        }
    }
    
    public static void Main(string[] args)
    {
        Console.WriteLine("Generating image...");
        var pixels = Enumerable
            .Range(0, (int)iResolution.y)
            .Reverse()
            .SelectMany(
                        y =>
                        {
                            return Enumerable
                                .Range(0, (int)iResolution.x)
                                .Select(
                                        x =>
                                        {
                                            mainImage(out var fragColor, new vec2(x, y));
                                            return fragColor;
                                        });
                        })
            .ToArray();
        Console.WriteLine("Writing image...");
        WritePPM("/Users/Dean/Desktop/output.ppm", pixels, (int)iResolution.x, (int)iResolution.y);
        Console.WriteLine("Done.");

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
    
    // ReSharper disable ArrangeTypeMemberModifiers
    // ReSharper disable FieldCanBeMadeReadOnly.Local
    // ReSharper disable SuggestVarOrType_SimpleTypes
    /*********************************************************************************************/
    struct Hit {
        public Hit(float d1, vec3 mat1, float spe1)
        {
            d = d1;
            mat = mat1;
            spe = spe1;
        }
        public float d;
        public vec3 mat;
        public float spe;
    }

    static float smin(float a, float b, float k) {
        float h = clamp(0.5f + 0.5f * (b - a) / k, 0.0f, 1.0f);
        return mix(b, a, h) - k * h * (1.0f - h);
    }

    static float B(vec3 p, vec3 b) { return length(max(abs(p) - b, 0.0f)); }

    static float cap(vec3 p, float h, float r) {
        vec2 d = abs(vec2(length(p.xz), p.y)) - vec2(h, r);
        return min(max(d.x, d.y), 0.0f) + length(max(d, 0.0f));
    }

    static float P(vec3 p) {
        p.y -= 3.2f;
        float d = max(length(p.xz) - .3f, p.y);
        d = smin(d, length(p) - .8f, .1f);
        p.y += 1.05f;
        d = smin(d, cap(p, .75f, .07f), .5f);
        p.y += 1.5f;
        d = smin(d, cap(p, .75f, .12f), .5f);
        p.y += .4f;
        return smin(d, cap(p, .9f, .2f), .4f);
    }

    static float K(vec3 p) {
        p.y -= 1.85f;
        float d = cap(p, .4f - .14f * cos(p.y * 1.4f - .8f), 2.0f);
        p.y--;
        d = smin(d, cap(p, .7f, .1f), .2f);
        p.y += 2.0f;
        d = smin(d, cap(p, .7f, .1f), .2f);
        p.y += .5f;
        d = smin(d, cap(p, 1.0f, .3f), .1f);
        p.xz *= mat2(.76484f, -.64422f, .64422f, .76484f);
        p.y -= 4.0f;
        return min(min(d, B(p, vec3(.5f, .2f, .1f))), B(p, vec3(.2f, .5f, .1f)));
    }

    static Hit map(vec3 p) {
        Hit hit = new Hit(P(p * 1.2f), vec3(.8f), 20.0f);
        float gnd = length(p.y);
        if (gnd < hit.d) {
            hit.d = gnd;
            hit.mat = vec3(.2f);
        }

        return hit;
    }

    static vec3 no(vec3 p, float t) {
        vec2 e = vec2(.5773f, -.5773f) * t * .003f;
        return normalize(e.xyy * map(p + e.xyy).d + e.yyx * map(p + e.yyx).d + e.yxy * map(p + e.yxy).d + e.xxx * map(p + e.xxx).d);
    }

    static float sha(vec3 p) {
        vec3 rd = normalize(vec3(-8, 8, -8) - p);
        float res = 1.0f,
            t = .5f;
        for (float h, i = 0.0f; i < 32.0f; i++) {
            h = K(p + rd * t);
            res = min(res, 150.0f * h / t);
            t += h;
            if (res < .01f || t > 20.0f) break;
        }

        return clamp(res, 0.0f, 1.0f);
    }

    static float ao(vec3 p, vec3 n, float h) { return map(p + h * n).d / h; }

    static vec3 applyLighting(vec3 p, vec3 rd, float d, Hit data) {
        vec3 l = normalize(vec3(-8, 8, -8) - p),
            n = no(p, d);
        float _ao = dot(vec3(ao(p, n, .2f), ao(p, n, .5f), ao(p, n, 2.0f)), vec3(.2f, .3f, .5f)),
            primary = max(0.0f, dot(l, n)),
            bounce = max(0.0f, dot(l * vec3(-1, 0, -1), n)) * .1f,
            spe = smoothstep(0.0f, 1.0f, pow(max(0.0f, dot(rd, reflect(l, n))), data.spe));
        primary *= mix(.4f, 1.0f, sha(p));
        return data.mat * ((primary + bounce) * _ao + spe) * vec3(2f, 1.6f, 1.4f) * exp(-length(p) * .1f);
    }

    static vec3 rgb(vec3 ro, vec3 rd) {
        vec3 p = new();
        float d = .01f;
        Hit hit = new();
        for (float steps = 0.0f; steps < 45.0f; steps++) {
            p = ro + rd * d;
            hit = map(p);
            if (abs(hit.d) < .0015f) break;
            if (d > 64.0f) return vec3(0);
            d += hit.d;
        }

        return applyLighting(p, rd, d, hit);
    }

    static void mainImage(out vec4 fragColor, vec2 fc)
    {
        vec3 f,
            r,
            ro = vec3(2, 5, -10),
            col = vec3(0),
            R = iResolution;
        ro.yz *= mat2(.98007f, -.19867f, .19867f, .98007f);
        f = normalize(vec3(2, 4, 0) - ro);
        r = normalize(cross(vec3(0, 1, 0), f));
        vec2 uv = (fc - .5f * R.xy) / R.y;
        col += rgb(ro, normalize(f + r * uv.x + cross(f, r) * uv.y));

        col = pow(col, vec3(.45f));
        vec2 q = fc.xy / R.xy;
        col *= .5f + .5f * pow(16.0f * q.x * q.y * (1.0f - q.x) * (1.0f - q.y), .4f);
        fragColor = vec4(col, 1);
    }
}