﻿using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using CommandLine;
using ImageMagick;

// ReSharper disable InconsistentNaming

namespace GLSLRenderer;

public static class Program
{
    private static bool UserCancelled;
    
    public static void Main(string[] args)
    {
        // Register event handler for Ctrl+C
        Console.CancelKeyPress += (_, eventArgs) =>
        {
            UserCancelled = true;
            eventArgs.Cancel = true;
            Console.WriteLine();
            Console.WriteLine("User cancel requested.  Please wait...");
        };

        Parser.Default.ParseArguments<CmdOptions>(args).WithParsed(RunShader);
    }

    private static void RunShader(CmdOptions options)
    {
        var iResolution = new vec2(options.Width, options.Height);
        var startTime = options.StartTime;
        var endTime = options.EndTime;
        var fps = options.Fps;

        var mainImageMethod = typeof(GLSLProg).GetMethod("mainImage", BindingFlags.NonPublic | BindingFlags.Instance);
        if (mainImageMethod == null)
        {
            Console.WriteLine("Error - Unable to locate mainImage(...)");
            return;
        }

        Console.WriteLine($"Generating {iResolution.x:F0}x{iResolution.y:F0} @ {fps:F0}fps.");
        var pixels = new vec4[(int)iResolution.x * (int)iResolution.y];
        var images = new List<MagickImage>();

        for (var iTime = startTime; iTime <= endTime && !UserCancelled; iTime += 1.0f / fps)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Console.Write($"Rendering (iTime = {iTime:F2})...");
            Parallel.For(
                         0,
                         pixels.Length,
                         index =>
                         {
                             var x = index % (int)iResolution.x;
                             var y = (int)iResolution.y - index / (int)iResolution.x - 1;
                             var glslProg = new GLSLProg(iResolution, iTime);
                             object[] parameters = { null!, new vec2(x, y) };
                             mainImageMethod.Invoke(glslProg, parameters);
                             pixels[index] = (vec4)parameters[0];
                         });

            var hash = new StringBuilder();
            var allComponents = pixels.SelectMany(pixel => pixel.Components).ToArray();
            foreach (var t in SHA256.HashData(FloatsToBytes(allComponents)).Take(4))
                hash.Append(t.ToString("X2"));

            Console.WriteLine($" {stopwatch.Elapsed.TotalSeconds:F1}s ({hash})");

            images.Add(PixelsToMagickImage(pixels, iResolution, fps));
        }

        Console.Write($"Writing {options.OutputPath}...");
        CreateAnimatedGif(options.OutputPath, images);
        Console.WriteLine(" Done.");
    }

    private static byte[] FloatsToBytes(float[] values)
    {
        var result = new byte[values.Length * sizeof(float)];
        Buffer.BlockCopy(values, 0, result, 0, result.Length);
        return result;
    }

    private static MagickImage PixelsToMagickImage(vec4[] pixels, vec2 iResolution, float fps)
    {
        var width = (int)iResolution.x;
        var height = (int)iResolution.y;

        var image = new MagickImage(MagickColors.Transparent, width, height)
        {
            Format = MagickFormat.Rgb,
            AnimationDelay = (int)(100.0f / fps)
        };

        var pixelsData = new byte[width * height * 3];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var rgb = GLSLProgBase.clamp(pixels[x + y * width].rgb, new vec3(0), new vec3(1)) * 255.0f;
                var dataIndex = (x + y * width) * 3;
                pixelsData[dataIndex] = (byte)rgb.x;
                pixelsData[dataIndex + 1] = (byte)rgb.y;
                pixelsData[dataIndex + 2] = (byte)rgb.z;
            }
        }

        image.Read(pixelsData);
        return image;
    }

    private static void CreateAnimatedGif(string filePath, IEnumerable<MagickImage> images)
    {
        using var collection = new MagickImageCollection(images);
        collection.Write(filePath);
    }
}
