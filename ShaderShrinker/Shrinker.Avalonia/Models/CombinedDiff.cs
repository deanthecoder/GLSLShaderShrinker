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

public class CombinedDiff
{
    public DiffPiece LeftDiff { get; }
    public DiffPiece RightDiff { get; }
 
    public CombinedDiff(DiffPiece leftDiff, DiffPiece rightDiff)
    {
        LeftDiff = leftDiff;
        RightDiff = rightDiff;
    }
}