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


// ReSharper disable UnusedAutoPropertyAccessor.Global

using CommandLine;

#pragma warning disable CS8618

namespace Shrinker.Cmd;

// ReSharper disable once ClassNeverInstantiated.Global
public class CmdOptions
{
    [Option('o', "output", Default = "", HelpText = "Transpile GLSL->CSharp file path.")]
    public string CSharpOutputPath { get; set; }
    
    [Option('d', "diff", Default = false, HelpText = "Launch external diffing tool.")]
    public bool Diff { get; set; }
}