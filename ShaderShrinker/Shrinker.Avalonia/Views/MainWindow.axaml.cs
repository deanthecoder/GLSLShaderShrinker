using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Shrinker.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void GiveFocus(object sender, RoutedEventArgs e)
    {
        var textBox = (TextBox)sender;
        textBox.Focus();
        textBox.CaretIndex = textBox.Text?.Length ?? 0;
    }
}