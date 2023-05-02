using System.Reflection;
using System.Text;

// ReSharper disable InconsistentNaming

namespace GLSLRenderer;

// todo - allow non static methods (by adding Shader class)
// todo - uses of struct (Hit(...)) need 'new' prefix.
// todo - structs need constructor to init all fields.
// todo - unassigned declarations of objects (vecN, struct, matN) should have '= new()'

public static class Program
{
    public static void Main(string[] args)
    {
	    var iResolution = GLSLProgBase.vec2(160, 90) * 1.0f;
        var glslProg = new GLSLProg(iResolution);
        
        var mainImageMethod = typeof(GLSLProg).GetMethod("mainImage", BindingFlags.NonPublic | BindingFlags.Instance);
        
        Console.WriteLine($"Generating image {iResolution}...");
        var pixels = Enumerable
            .Range(0, (int)glslProg.iResolution.y)
            .Reverse()
            .SelectMany(
                        y =>
                        {
                            return Enumerable
                                .Range(0, (int)glslProg.iResolution.x)
                                .Select(
                                        x =>
                                        {
	                                        object[] parameters = { null!, new vec2(x, y) };
	                                        mainImageMethod!.Invoke(glslProg, parameters);
	                                        return (vec4)parameters[0];
                                        });
                        })
            .ToArray();
        Console.WriteLine("Writing image...");
        WritePPM("/Users/Dean/Desktop/output.ppm", pixels, (int)glslProg.iResolution.x, (int)glslProg.iResolution.y);
        Console.WriteLine("Done.");
    }
    
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
		    var rgb = GLSLProgBase.clamp(pixel.rgb, GLSLProgBase.vec3(0.0f), GLSLProgBase.vec3(1.0f)) * 255.0f;
		    pixelData[0] = (byte)rgb.x;
		    pixelData[1] = (byte)rgb.y;
		    pixelData[2] = (byte)rgb.z;

		    stream.Write(pixelData, 0, pixelData.Length);
	    }
    }
}