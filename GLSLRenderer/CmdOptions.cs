// -----------------------------------------------------------------------
//  <copyright file="CmdOptions.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using CommandLine;
// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable CS8618

namespace GLSLRenderer;

// ReSharper disable once ClassNeverInstantiated.Global
public class CmdOptions
{
    [Option('s', "start", Default = 10.0f, HelpText = "Start time.")]
    public float StartTime { get; set; }

    [Option('e', "end", Default = 12.0f, HelpText = "End time.")]
    public float EndTime { get; set; }

    [Option('f', "fps", Default = 1.0f, HelpText = "Frames per second.")]
    public float Fps { get; set; }

    [Option('w', "width", Default = 640, HelpText = "Output width.")]
    public int Width { get; set; }

    [Option('h', "height", Default = 320, HelpText = "Output height.")]
    public int Height { get; set; }

    [Option('o', "output", Required = true, HelpText = "Output file path.")]
    public string OutputPath { get; set; }
}