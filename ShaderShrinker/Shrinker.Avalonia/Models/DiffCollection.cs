// -----------------------------------------------------------------------
//  <copyright file="DiffCollection.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DynamicData;

namespace Shrinker.Avalonia.Models;

/// <summary>
/// Represents the collection of left/right diff lines.
/// </summary>
public class DiffCollection : ObservableCollection<CombinedDiff>
{
    public string GetAllLeftText() => string.Join("\n", this.Select(o => o.LeftDiff?.Text).Where(o => o != null));
    public string GetAllRightText() => string.Join("\n", this.Select(o => o.RightDiff?.Text).Where(o => o != null));

    public void ReplaceAll(IEnumerable<CombinedDiff> newItems)
    {
        Clear();
        this.AddRange(newItems);
    }

    public bool HasRightContent() =>
        this.Any(o => !string.IsNullOrWhiteSpace(o.RightDiff?.Text));
}