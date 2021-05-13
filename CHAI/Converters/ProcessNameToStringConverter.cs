using CHAI.Extensions;
using CHAI.Models.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace CHAI.Converters
{

    /// <summary>
    /// Converter for converting <see cref="Process"/> values to <see cref="string"/>s.
    /// </summary>
    [ValueConversion(typeof(string), typeof(string))]
    public class ProcessNameToStringConverter : IValueConverter
    {
        /// <summary>
        /// Convert a <see cref="Process.ProcessName"/> value to a <see cref="string"/>.
        /// </summary>
        /// <param name="value">value to convert.</param>
        /// <param name="targetType"><see cref="Type"/> to convert to.</param>
        /// <param name="parameter">param.</param>
        /// <param name="culture"><see cref="CultureInfo"/>.</param>
        /// <returns>A <see cref="string"/> representation of the <see cref="Process.ProcessName"/> value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var processName = value.ToString();
            var process = Process.GetProcessesByName(processName).FirstOrDefault();

            return process != null ? processName : string.Empty;
        }

        /// <summary>
        /// Convert a <see cref="string"/> value to a <see cref="Process.ProcessName"/>.
        /// </summary>
        /// <param name="value">value to convert.</param>
        /// <param name="targetType"><see cref="Type"/> to convert to.</param>
        /// <param name="parameter">param.</param>
        /// <param name="culture"><see cref="CultureInfo"/>.</param>
        /// <returns>A <see cref="Process.ProcessName"/> representation of the <see cref="string"/> value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }
}
