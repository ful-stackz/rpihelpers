using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RpiHelpers.Mvvm.Converters
{
    internal class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is bool val
            ? (val ? Visibility.Visible : Visibility.Collapsed)
            : throw new InvalidOperationException($"Expected {nameof(value)} to be of type bool.");

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is Visibility val
            ? (val == Visibility.Visible ? true : false)
            : throw new InvalidOperationException($"Expected {nameof(value)} to be of type {nameof(Visibility)}");
    }
}
