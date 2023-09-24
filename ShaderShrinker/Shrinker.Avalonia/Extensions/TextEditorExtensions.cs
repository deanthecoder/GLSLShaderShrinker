using Avalonia;
using Avalonia.Media;
using AvaloniaEdit;

namespace Shrinker.Avalonia.Extensions;

public static class TextEditorExtensions
{
    public static TextEditor MakeReadOnly(this TextEditor textEditor)
    {
        textEditor.TextArea.TextView.LinkTextForegroundBrush = Brushes.Cyan;
        textEditor.TextArea.TextView.Margin = new Thickness(0);
        textEditor.TextArea.SelectionChanged += (_, _) => textEditor.TextArea.ClearSelection();
        textEditor.TextArea.Caret.PositionChanged += (_, _) => textEditor.TextArea.Caret.Hide();
        return textEditor;
    }
}