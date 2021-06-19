using CHAI.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace CHAI.Models
{
    /// <summary>
    /// Model representing a <see cref="Setting"/>.
    /// </summary>
    public class Setting
    {
        /// <summary>
        /// Gets or sets the <see cref="Id"/> that uniquely identifies this <see cref="Setting" /> instance.
        /// </summary>
        [Key]
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the <see cref="Application"/> of this <see cref="Setting" /> instance.
        /// </summary>
        [MaxLength]
        public string Application { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Username"/> of the User of this <see cref="Setting" /> instance.
        /// </summary>
        [MaxLength(25)]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="UserID"/> of the User of this <see cref="Setting" /> instance.
        /// </summary>
        [MaxLength]
        public string UserID { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="OAuthToken"/> of the User of this <see cref="Setting" /> instance.
        /// </summary>
        [MaxLength(24)]
        public string OAuthToken { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="GlobalCooldown"/> interval for all <see cref="Trigger" /> instances.
        /// </summary>
        [Required]
        public int GlobalCooldown { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="GlobalCooldownUnit"/> for all <see cref="Trigger" /> instances.
        /// </summary>
        [Required]
        public TimeSpanUnit GlobalCooldownUnit { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> that any <see cref="Trigger" /> instance was <see cref="Trigger.LastTriggered"/>.
        /// </summary>
        public DateTime GlobalLastTriggered { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether logging is enabled.
        /// </summary>
        [Required]
        public bool LoggingEnabled { get; set; }
    }
}
