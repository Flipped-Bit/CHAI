using System.ComponentModel;

namespace CHAI.Models.Enums
{
    /// <summary>
    /// An enumeration of all available <see cref="CooldownUnit"/>s.
    /// </summary>
    public enum CooldownUnit
    {
        /// <summary>
        /// No unit set
        /// </summary>
        [Description("")]
        None,

        /// <summary>
        /// Using Second(s) as unit
        /// </summary>
        [Description("Seconds")]
        Seconds,

        /// <summary>
        /// Using Minute(s) as unit
        /// </summary>
        [Description("Minutes")]
        Minutes,

        /// <summary>
        /// Using Hour(s) as unit
        /// </summary>
        [Description("Hours")]
        Hours,
    }
}
