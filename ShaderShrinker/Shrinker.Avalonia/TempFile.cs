// -----------------------------------------------------------------------
//  <copyright file="TempFile.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.IO;

namespace Shrinker.Avalonia;

public class TempFile : IDisposable
{
    public string FilePath { get; private set; }

    public TempFile(string content)
    {
        // Create a unique temp file
        FilePath = Path.GetTempFileName();

        // Write the provided content to the temp file.
        File.WriteAllText(FilePath, content ?? string.Empty);
    }

    // Implicit conversion operator
    public static implicit operator FileInfo(TempFile tempFile) =>
        new(tempFile.FilePath);

    public void Dispose()
    {
        DeleteTempFile();
        GC.SuppressFinalize(this);
    }

    ~TempFile() => DeleteTempFile();

    private void DeleteTempFile()
    {
        if (!File.Exists(FilePath))
            return;
        try
        {
            File.Delete(FilePath);
        }
        catch
        {
            // No op.
        }
    }
}