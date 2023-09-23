// -----------------------------------------------------------------------
//  <copyright file="ClipboardCommand.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System;
using Avalonia.Threading;
using TextCopy;

namespace Shrinker.Avalonia.Commands;

/// <summary>
/// Continuously monitors the clipboard, enabling the command if it contains non-empty content.
/// </summary>
public class ClipboardCommand : RelayCommand
{
    public ClipboardCommand(Action<object> execute, Func<bool> canExecute = null)
        : base(execute, canExecute)
    {
        DispatcherTimer.Run(
                            () =>
                            {
                                RaiseCanExecuteChanged();
                                return true;
                            },
                            TimeSpan.FromSeconds(0.2));
    }

    public override bool CanExecute(object parameter) =>
        base.CanExecute(parameter) && !string.IsNullOrWhiteSpace(ClipboardService.GetText());
}