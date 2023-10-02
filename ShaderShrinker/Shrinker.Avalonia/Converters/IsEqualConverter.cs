// -----------------------------------------------------------------------
//  <copyright file="IsEqualConverter.cs" company="Dean Edis">
//      Copyright (c) 2023 Dean Edis. All rights reserved.
//  </copyright>
//  <summary>
//  This example is provided on an "as is" basis and without warranty of any kind.
//  Dean Edis. does not warrant or make any representations regarding the use or
//  results of use of this example.
//  </summary>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Shrinker.Avalonia.Converters;

public class IsEqualConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value == parameter || value?.Equals(parameter) == true;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}