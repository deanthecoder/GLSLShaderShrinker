// -----------------------------------------------------------------------
//  <copyright file="NameAndFileInfo.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.IO;

namespace Shrinker.Avalonia.Models;

public class NameAndFileInfo
{
    public string Name { get; }
    public FileInfo File { get; }

    public NameAndFileInfo(FileInfo file)
        : this(file.Name, file)
    {
    }

    public NameAndFileInfo(string name, FileInfo file)
    {
        Name = name;
        File = file;
    }
}