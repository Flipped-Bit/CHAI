using System.ComponentModel;

namespace CHAI.Models.Enums
{
    /// <summary>
    /// An enumeration of all available <see cref="BitsCondition"/>s.
    /// </summary>
    public enum BitsCondition
    {
        /// <summary>
        /// No Condition set
        /// </summary>
        [Description("")]
        None,

        /// <summary>
        /// Condition based on value being greater than or equal to
        ///  <see cref="Trigger.MinimumBits"/>.
        /// </summary>
        [Description("At least")]
        Atleast,

        /// <summary>
        /// Condition based on value being less than or equal to
        ///  <see cref="Trigger.MinimumBits"/>.
        /// </summary>
        [Description("At most")]
        AtMost,

        /// <summary>
        /// Condition based on value being equal to
        ///  <see cref="Trigger.MinimumBits"/>.
        /// </summary>
        [Description("Exactly")]
        Exactly,

        /// <summary>
        /// Condition based on value being between
        ///  <see cref="Trigger.MinimumBits"/> and <see cref="Trigger.MaximumBits"/>.
        /// </summary>
        [Description("Between")]
        Between,
    }
}
