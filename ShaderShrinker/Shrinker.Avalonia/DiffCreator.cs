// -----------------------------------------------------------------------
//  <copyright file="DiffCreator.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Shrinker.Avalonia.Models;

namespace Shrinker.Avalonia;

public static class DiffCreator
{
    public static IEnumerable<CombinedDiff> CreateDiffs(string oldText, string newText)
    {
        var differ = new Differ();
        var diffBuilder = new InlineDiffBuilder(differ);
        var diff = diffBuilder.BuildDiffModel(oldText, newText, true);

        var diffs = new List<CombinedDiff>();
        foreach (var line in diff.Lines)
        {
            switch (line.Type)
            {
                case ChangeType.Inserted:
                    diffs.Add(new CombinedDiff(null, line));
                    break;
                case ChangeType.Deleted:
                    diffs.Add(new CombinedDiff(line, null));
                    break;
                case ChangeType.Unchanged:
                    diffs.Add(new CombinedDiff(line, line));
                    break;
            }
        }

        if (string.IsNullOrEmpty(newText))
        {
            // The RHS is blank, so make the left non-colorized.
            diffs.ForEach(o => o.LeftDiff.Type = ChangeType.Unchanged);
        }
        
        // todo - if both lines only differ in whitespace, put side by side.
        // todo - if left line prefixes right light, put side by side? E.g. Functions changed to a single line...

        return diffs;
    }
}