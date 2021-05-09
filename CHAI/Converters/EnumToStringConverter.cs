﻿using CHAI.Extensions;
using CHAI.Models.Enums;
using System;
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
            if (value.ToString() == "True" || value.ToString() == "False")
            {
                return value.ToString();
            }

            var isBitCondition = Enum.IsDefined(typeof(BitsCondition), value.ToString());

            var isCooldownUnit = Enum.IsDefined(typeof(CooldownUnit), value.ToString());

            if (isBitCondition)
            {
                return Enum.Parse<BitsCondition>(value.ToString())
                    .GetDescription();
            }

            if (isCooldownUnit)
            {
                return Enum.Parse<CooldownUnit>(value.ToString())
                    .GetDescription();
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
            var enumValue = value.ToString();

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
