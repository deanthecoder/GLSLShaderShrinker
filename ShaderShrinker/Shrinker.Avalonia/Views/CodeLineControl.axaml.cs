// -----------------------------------------------------------------------
//  <copyright file="CodeLineControl.axaml.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System.Reflection;
using System.Xml;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;
using DiffPlex.DiffBuilder.Model;
using Shrinker.Avalonia.Extensions;

namespace Shrinker.Avalonia.Views;

public partial class CodeLineControl : UserControl
{
    private static readonly Brush TextAddedColor;
    private static readonly Brush TextRemovedColor;
    private static readonly Brush TextModifiedColor;

    private DiffPiece m_diff;
    public static readonly DirectProperty<CodeLineControl, DiffPiece> DiffProperty =
        AvaloniaProperty
            .RegisterDirect<CodeLineControl, DiffPiece>(
                                                        nameof(Diff),
                                                        o => o.Diff,
                                                        (o, v) =>
                                                        {
                                                            o.Diff = v;
                                                            
                                                            o.m_editor.Text = v?.Text;

                                                            var brush = (Brush)null;
                                                            switch (v?.Type)
                                                            {
                                                                case ChangeType.Deleted:
                                                                    brush = TextRemovedColor;
                                                                    break;
                                                                case ChangeType.Inserted:
                                                                    brush = TextAddedColor;
                                                                    break;
                                                                case ChangeType.Modified:
                                                                    brush = TextModifiedColor;
                                                                    break;
                                                            }
                                                            o.m_editor.Background = brush;
                                                        });

    public static IHighlightingDefinition SyntaxHighlighter { get; }

    static CodeLineControl()
    {
        TextRemovedColor = new SolidColorBrush(Colors.Red) { Opacity = 0.1 };
        TextAddedColor = new SolidColorBrush(Colors.Green) { Opacity = 0.1 };
        TextModifiedColor = new SolidColorBrush(Colors.DarkOrange) { Opacity = 0.1 };
        
        var assembly = Assembly.GetExecutingAssembly();
        using var s = assembly.GetManifestResourceStream("Shrinker.Avalonia.GLSL.xshd");
        using var reader = new XmlTextReader(s);
        SyntaxHighlighter = HighlightingLoader.Load(reader, HighlightingManager.Instance);
    }
    
    public CodeLineControl()
    {
        InitializeComponent();
    }

    public DiffPiece Diff
    {
        get => m_diff;
        set => SetAndRaise(DiffProperty, ref m_diff, value);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var textEditor = ((TextEditor)sender).MakeReadOnly();
        textEditor.SyntaxHighlighting = SyntaxHighlighter;
    }
}