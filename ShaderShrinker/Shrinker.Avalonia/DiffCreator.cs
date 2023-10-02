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
        var diffModel = diffBuilder.BuildDiffModel(oldText, newText, true);

        var diffs = new List<CombinedDiff>();
        foreach (var line in diffModel.Lines)
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

        if (!string.IsNullOrEmpty(newText))
        {
            MergeSimilarLines(diffs);
        }
        else
        {
            // The RHS is blank, so make the left non-colorized.
            diffs.ForEach(o => o.LeftDiff.Type = ChangeType.Unchanged);
        }

        return AssignLineNumbers(diffs);
    }

    private static IEnumerable<CombinedDiff> AssignLineNumbers(IReadOnlyCollection<CombinedDiff> diffs)
    {
        var leftLineNumber = 0;
        var rightLineNumber = 0;

        foreach (var diff in diffs)
        {
            if (diff.LeftDiff != null)
                diff.SetLeftPageNumber(++leftLineNumber);
            if (diff.RightDiff != null)
                diff.SetRightPageNumber(++rightLineNumber);
        }

        return diffs;
    }

    private static void MergeSimilarLines(List<CombinedDiff> diffs)
    {
        for (var i = 0; i < diffs.Count - 1; i++)
        {
            // Find deleted line on the LHS.
            if (diffs[i].LeftDiff?.Type != ChangeType.Deleted)
                continue;
            
            // Find next non-blank line on the RHS.
            var nextNonBlankRhsIndex = i;
            while (nextNonBlankRhsIndex < diffs.Count && diffs[nextNonBlankRhsIndex].RightDiff == null)
                nextNonBlankRhsIndex++;
            if (nextNonBlankRhsIndex >= diffs.Count)
                continue;

            // If they're similar, place them next to each other.
            if (diffs[nextNonBlankRhsIndex].RightDiff is { Type: ChangeType.Inserted })
            {
                var leftText = diffs[i].LeftDiff.Text;
                var rightText = diffs[nextNonBlankRhsIndex].RightDiff.Text;
                if (!string.IsNullOrWhiteSpace(RemoveWhiteSpace(leftText)) && CompareIgnoringWhitespace(rightText, leftText))
                {
                    diffs[i].LeftDiff.Type = ChangeType.Modified;
                    diffs[i].RightDiff = new DiffPiece(rightText, ChangeType.Modified);
                    diffs.RemoveAt(nextNonBlankRhsIndex);
                }
            }
        }
    }

    private static bool CompareIgnoringWhitespace(string rightText, string leftText) =>
        RemoveWhiteSpace(rightText).StartsWith(RemoveWhiteSpace(leftText));

    private static string RemoveWhiteSpace(string s) =>
        s.Replace(" ", string.Empty).Replace("\t", string.Empty);
}