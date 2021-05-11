using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;

namespace CHAI.Converters
{
    /// <summary>
    /// Converter for converting a csv <see cref="string"/> values to <see cref="List{T}"/> whose generic type argument is <see cref="string"/>.
    /// </summary>
    [ValueConversion(typeof(string), typeof(List<string>))]
    public class CSVStringToListOfStringsConverter : IValueConverter
    {
        /// <summary>
        /// Convert a CSV <see cref="string"/> value to <see cref="ObservableCollection{T}"/> whose generic type argument is <see cref="string"/>.
        /// </summary>
        /// <param name="value">value to convert.</param>
        /// <param name="targetType"><see cref="Type"/> to convert to.</param>
        /// <param name="parameter">param.</param>
        /// <param name="culture"><see cref="CultureInfo"/>.</param>
        /// <returns>A <see cref="ObservableCollection{T}"/> whose generic type argument is <see cref="string"/> representation of the CSV <see cref="string"/> value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = value.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
            return new ObservableCollection<string>(input);
        }

        /// <summary>
        /// Convert a <see cref="ObservableCollection{T}"/> whose generic type argument is <see cref="string"/> value to a CSV <see cref="string"/>.
        /// </summary>
        /// <param name="value">value to convert.</param>
        /// <param name="targetType"><see cref="Type"/> to convert to.</param>
        /// <param name="parameter">param.</param>
        /// <param name="culture"><see cref="CultureInfo"/>.</param>
        /// <returns>A CSV <see cref="string"/> representation of the <see cref="ObservableCollection{T}"/> whose generic type argument is <see cref="string"/> value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = value as List<string>;
            return string.Join(',', input);
        }
    }
}
