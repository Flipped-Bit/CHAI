using System.ComponentModel;

namespace CHAI.Models.Enums
{
    /// <summary>
    /// An enumeration of all available <see cref="TimeSpanUnit"/>s.
    /// </summary>
    public enum TimeSpanUnit
    {
        /// <summary>
        /// No unit set
        /// </summary>
        [Description("")]
        None,

        /// <summary>
        /// Using Second(s) as unit
        /// </summary>
        [Description("Second(s)")]
        Seconds,

        /// <summary>
        /// Using Minute(s) as unit
        /// </summary>
        [Description("Minute(s)")]
        Minutes,

        /// <summary>
        /// Using Hour(s) as unit
        /// </summary>
        [Description("Hour(s)")]
        Hours,
    }
}
