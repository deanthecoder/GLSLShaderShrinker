// -----------------------------------------------------------------------
//  <copyright file="CombinedDiff.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using DiffPlex.DiffBuilder.Model;

namespace Shrinker.Avalonia.Models;

/// <summary>
/// Represents a single left/right diff line.
/// </summary>
public class CombinedDiff
{
    public DiffPiece LeftDiff { get; }
    public DiffPiece RightDiff { get; set; }

    public string LeftLineNumber { get; private set; } = string.Empty;
    public string RightLineNumber { get; private set; } = string.Empty;

    public CombinedDiff(DiffPiece leftDiff, DiffPiece rightDiff)
    {
        LeftDiff = leftDiff;
        RightDiff = rightDiff;
    }

    public void SetLeftPageNumber(int i) => LeftLineNumber = $"{i:D}";
    public void SetRightPageNumber(int i) => RightLineNumber = $"{i:D}";

    public override string ToString() =>
        $"{LeftDiff?.Text ?? "<empty>"} | {RightDiff?.Text ?? "<empty>"}";
}