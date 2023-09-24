// -----------------------------------------------------------------------
//  <copyright file="HintDialog.axaml.cs" company="Dean Edis">
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
using Avalonia.Interactivity;
using AvaloniaEdit;
using Shrinker.Avalonia.Extensions;
using Shrinker.Parser;

namespace Shrinker.Avalonia.Views;

public partial class HintDialog : UserControl
{
    public HintDialog()
    {
        InitializeComponent();
    }

    private void OnTextEditorLoaded(object sender, RoutedEventArgs e)
    {
        var textEditor = ((TextEditor)sender).MakeReadOnly();
        textEditor.SyntaxHighlighting = CodeLineControl.SyntaxHighlighter;
        textEditor.Text = ((CodeHint)textEditor.DataContext)?.Item;
    }

    private void OnListLoaded(object sender, RoutedEventArgs e) =>
        ((ListBox)sender).ScrollIntoView(0);
}