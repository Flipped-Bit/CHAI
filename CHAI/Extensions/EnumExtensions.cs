using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace CHAI.Extensions
{
    /// <summary>
    /// Extensions for <see cref="Enum"/>s.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Extension method to get the description of a given <see cref="Enum"/>.
        /// </summary>
        /// <param name="genericEnum"><see cref="string"/> to find the <see cref="Enum"/> by.</param>
        /// <returns>A description of a given <see cref="Enum"/>.</returns>
        public static string GetDescription(this Enum genericEnum)
        {
            Type genericEnumType = genericEnum.GetType();
            MemberInfo[] memberInfo = genericEnumType.GetMember(genericEnum.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                var attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes != null && attributes.Count() > 0)
                {
                    return ((DescriptionAttribute)attributes.ElementAt(0)).Description;
                }
            }

            return genericEnum.ToString();
        }

        /// <summary>
        /// Extension method to return an <see cref="Enum"/> value of type for the given <see cref="string"/> value.
        /// </summary>
        /// <typeparam name="T">Type of enum returned.</typeparam>
        /// <param name="value"><see cref="string"/> to find the <see cref="Enum"/> by.</param>
        /// <returns>An <see cref="Enum"/> representation of the value.</returns>
        public static T ToEnum<T>(this string value)
            where T : struct, IConvertible
        {
            if (string.IsNullOrEmpty(value))
            {
                return default;
            }

            return Enum.TryParse(value, true, out T result) ? result : default;
        }
    }
}
