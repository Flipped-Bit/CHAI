using CHAI.Extensions;
using CHAI.Models.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace CHAI.Converters
{
    /// <summary>
    /// Converter for converting <see cref="Enum"/> values to <see cref="string"/>s.
    /// </summary>
    [ValueConversion(typeof(Enum), typeof(string))]
    public class EnumToStringConverter : IValueConverter
    {
        /// <summary>
        /// Convert a <see cref="Enum"/> value to a <see cref="string"/>.
        /// </summary>
        /// <param name="value">value to convert.</param>
        /// <param name="targetType"><see cref="Type"/> to convert to.</param>
        /// <param name="parameter">param.</param>
        /// <param name="culture"><see cref="CultureInfo"/>.</param>
        /// <returns>A <see cref="string"/> representation of the <see cref="Enum"/> value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isBitCondition = Enum.IsDefined(typeof(BitsCondition), value.ToString());

            var isCooldownUnit = Enum.IsDefined(typeof(CooldownUnit), value.ToString());

            if (isBitCondition)
            {
                var newEnum = Enum.Parse<BitsCondition>(value.ToString());
                return new KeyValuePair<string, string>(newEnum.GetDescription(), newEnum.ToString());
            }

            if (isCooldownUnit)
            {
                var newEnum = Enum.Parse<CooldownUnit>(value.ToString());
                return new KeyValuePair<string, string>(newEnum.GetDescription(), newEnum.ToString());
            }

            return new NotImplementedException();
        }

        /// <summary>
        /// Convert a <see cref="string"/> value to a <see cref="Enum"/>.
        /// </summary>
        /// <param name="value">value to convert.</param>
        /// <param name="targetType"><see cref="Type"/> to convert to.</param>
        /// <param name="parameter">param.</param>
        /// <param name="culture"><see cref="CultureInfo"/>.</param>
        /// <returns>A <see cref="Enum"/> representation of the <see cref="string"/> value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var enumValue = ((KeyValuePair<string, string>)value).Value;

            switch (targetType.Name)
            {
                case nameof(BitsCondition):
                    return enumValue.ToEnum<BitsCondition>();
                case nameof(CooldownUnit):
                    return enumValue.ToEnum<CooldownUnit>();
                default:
                    return new NotImplementedException();
            }
        }
    }
}
