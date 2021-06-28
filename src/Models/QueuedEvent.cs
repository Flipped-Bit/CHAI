using System;

namespace CHAI.Models
{
    /// <summary>
    /// Model representing a <see cref="QueuedEvent"/>.
    /// </summary>
    public class QueuedEvent
    {
        /// <summary>
        /// Gets or sets the <see cref="Id"/> of the <see cref="QueuedEvent"/>.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> that the <see cref="QueuedEvent"/> should be <see cref="TriggeredAt"/>.
        /// </summary>
        public DateTime TriggeredAt { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Trigger.Id"/> of the <see cref="Trigger"/> for the <see cref="QueuedEvent"/>.
        /// </summary>
        public int TriggerId { get; set; }
    }
}
