using System;
using System.Globalization;
using System.Windows.Data;

namespace CHAI.Converters
{
    /// <summary>
    /// Converter for converting <see cref="DateTime"/> values to <see cref="string"/>s.
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateTimeToStringConverter : IValueConverter
    {
        /// <summary>
        /// Convert a <see cref="DateTime"/> value to a <see cref="string"/>.
        /// </summary>
        /// <param name="value">value to convert.</param>
        /// <param name="targetType"><see cref="Type"/> to convert to.</param>
        /// <param name="parameter">param.</param>
        /// <param name="culture"><see cref="CultureInfo"/>.</param>
        /// <returns>A <see cref="string"/> representation of the <see cref="DateTime"/> value if it is greater than <see cref="DateTime.MinValue"/>.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (DateTime)value > DateTime.MinValue ? ((DateTime)value).ToString() : "Never";
        }

        /// <summary>
        /// Convert a <see cref="string"/> value to a <see cref="DateTime"/>.
        /// </summary>
        /// <param name="value">value to convert.</param>
        /// <param name="targetType"><see cref="Type"/> to convert to.</param>
        /// <param name="parameter">param.</param>
        /// <param name="culture"><see cref="CultureInfo"/>.</param>
        /// <returns>A <see cref="DateTime"/> representation of the <see cref="string"/> value if not set to Never.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value != "Never" ? DateTime.Parse(value.ToString()) : DateTime.MinValue;
        }
    }
}
