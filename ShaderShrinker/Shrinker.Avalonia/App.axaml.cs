using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Shrinker.Avalonia.ViewModels;
using Shrinker.Avalonia.Views;

namespace Shrinker.Avalonia;

public class App : Application
{
    public App()
    {
        DataContext = new AppViewModel();
    }
    
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel()
            };

            desktop.MainWindow.Closed += (_, _) =>
            {
                ((IDisposable)desktop.MainWindow.DataContext).Dispose();
                UserSettings.Instance.Dispose();
            };
        }
        
        base.OnFrameworkInitializationCompleted();
    }
}