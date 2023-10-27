// -----------------------------------------------------------------------
//  <copyright file="MarkdownToHtmlConverter.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.Linq;
using Avalonia;
using Avalonia.Data.Converters;
using Markdig;
using Markdown.ColorCode;

namespace Shrinker.Avalonia.Converters;

public class MarkdownToHtmlConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is string markdownText)
        {
            var lines = markdownText.Split('\n', '\r').Select(o => o.Trim());

            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseColorCode()
                .Build();
            var html = Markdig.Markdown.ToHtml(string.Join('\n', lines), pipeline);
            return html;
        }

        return AvaloniaProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => null;
}