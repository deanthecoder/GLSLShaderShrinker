using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Shrinker.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void GiveFocus(object sender, RoutedEventArgs e) => ((InputElement)sender).Focus();
}