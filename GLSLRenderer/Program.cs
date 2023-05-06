using System.Diagnostics;
using System.Reflection;
using System.Text;

// ReSharper disable InconsistentNaming

namespace GLSLRenderer;

public static class Program
{
	public static void Main(string[] args)
	{
		var iResolution = GLSLProgBase.vec2(160, 90) * 1.0f;
		var mainImageMethod = typeof(GLSLProg).GetMethod("mainImage", BindingFlags.NonPublic | BindingFlags.Instance);

		Console.WriteLine($"Generating image {iResolution}...");
		var totalPixels = (int)iResolution.x * (int)iResolution.y;
		vec4[] pixels = new vec4[totalPixels];

		// Measure the start time.
		var stopwatch = new Stopwatch();
		stopwatch.Start();

		var glslProg = new GLSLProg(iResolution);
		Parallel.For(0, totalPixels, index =>
		{
			var x = index % (int)iResolution.x;
			var y = (int)iResolution.y - index / (int)iResolution.x - 1;
			object[] parameters = { null!, new vec2(x, y) };
			mainImageMethod!.Invoke(glslProg, parameters);
			pixels[index] = (vec4)parameters[0];
		});

		// Calculate the elapsed time.
		stopwatch.Stop();
		Console.WriteLine($"Image generation took {stopwatch.Elapsed.TotalSeconds:F1}s");

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