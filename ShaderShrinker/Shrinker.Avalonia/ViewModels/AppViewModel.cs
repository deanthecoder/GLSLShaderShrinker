// -----------------------------------------------------------------------
//  <copyright file="AppViewModel.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using Shrinker.Avalonia.Commands;
using Shrinker.Avalonia.Views;

namespace Shrinker.Avalonia.ViewModels;

public class AppViewModel : ReactiveObject
{
    public ICommand AboutCommand { get; }

    public AppViewModel()
    {
        var isOpen = false;
        AboutCommand = new RelayCommand(
                                        _ =>
                                        {
                                            if (isOpen)
                                                return;
                                            var dialog = new AboutDialog();
                                            dialog.Opened += (sender, args) => isOpen = true;
                                            dialog.Closed += (sender, args) => isOpen = false;
                                            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
                                            dialog.ShowDialog(mainWindow);
                                        });
    }
}