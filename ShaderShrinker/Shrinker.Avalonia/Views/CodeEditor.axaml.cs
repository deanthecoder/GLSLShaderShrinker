// -----------------------------------------------------------------------
//  <copyright file="CodeEditor.axaml.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.Indentation.CSharp;

namespace Shrinker.Avalonia.Views;

public partial class CodeEditor : Window
{
    private string m_glsl;
    public static readonly DirectProperty<CodeEditor, string> GlslProperty =
        AvaloniaProperty.RegisterDirect<CodeEditor, string>(
                                                            nameof(Glsl),
                                                            o => o.Glsl,
                                                            (o, v) => o.Glsl = v,
                                                            defaultBindingMode: BindingMode.TwoWay);
    private Action<string> m_accept;
    public static readonly DirectProperty<CodeEditor, Action<string>> AcceptProperty =
        AvaloniaProperty.RegisterDirect<CodeEditor, Action<string>>(
                                                                    nameof(Accept),
                                                                    o => o.Accept,
                                                                    (o, v) => o.Accept = v);

    public string Glsl
    {
        get => m_glsl;
        set
        {
            if (SetAndRaise(GlslProperty, ref m_glsl, value))
                m_editor.Text = value;
        }
    }

    public Action<string> Accept
    {
        get => m_accept;
        set => SetAndRaise(AcceptProperty, ref m_accept, value);
    }

    public CodeEditor()
    {
        InitializeComponent();
    }

    private void OnAccept(object sender, RoutedEventArgs e)
    {
        Accept?.Invoke(m_editor.Text);
        Close();
    }

    private void OnDiscard(object sender, RoutedEventArgs e) =>
        Close();

    private void OnTextEditorLoaded(object sender, RoutedEventArgs e)
    {
        var textEditor = (TextEditor)sender;
        textEditor.SyntaxHighlighting = CodeLineControl.SyntaxHighlighter;
        textEditor.TextArea.TextView.LinkTextForegroundBrush = Brushes.Cyan;
        textEditor.TextArea.IndentationStrategy = new CSharpIndentationStrategy();
    }
}