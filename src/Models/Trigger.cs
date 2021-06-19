using CHAI.Models.Enums;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace CHAI.Models
{
    /// <summary>
    /// Model representing a <see cref="Trigger"/>.
    /// </summary>
    public class Trigger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Trigger"/> class.
        /// </summary>
        public Trigger()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Trigger"/> class from a <see cref="TriggerDTO"/>.
        /// </summary>
        /// <param name="triggerDTO"><see cref="TriggerDTO"/> to construct from.</param>
        public Trigger(TriggerDTO triggerDTO)
        {
            Name = triggerDTO.Name;
            CreatedAt = triggerDTO.CreatedAt;
            BitsEnabled = triggerDTO.BitsEnabled;
            BitsCondition = (BitsCondition)triggerDTO.BitsCondition + 1;
            MinimumBits = triggerDTO.BitsAmount;
            MaximumBits = triggerDTO.BitsAmount2;
            UserLevelEveryone = triggerDTO.UserLevelEveryone;
            UserLevelSubs = triggerDTO.UserLevelSubs;
            UserLevelVips = triggerDTO.UserLevelVips;
            UserLevelMods = triggerDTO.UserLevelMods;
            Keywords = string.Join(',', JsonConvert.DeserializeObject<string[]>(triggerDTO.Keywords));
            CharAnimTriggerKeyChar = triggerDTO.CharAnimTriggerKeyChar;
            CharAnimTriggerKeyValue = triggerDTO.CharAnimTriggerKeyValue;
            Cooldown = triggerDTO.Cooldown;
            CooldownUnit = (TimeSpanUnit)triggerDTO.CooldownUnit + 1;
            LastTriggered = triggerDTO.LastTriggered;
            RewardName = triggerDTO.RewardName;
        }

        /// <summary>
        /// Gets or sets the <see cref="Id"/> that uniquely identifies this <see cref="Trigger" /> instance.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Name"/> of this <see cref="Trigger" /> instance.
        /// </summary>
        [MaxLength]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> that this <see cref="Trigger" /> instance was <see cref="CreatedAt"/>.
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Trigger" /> instance has <see cref="BitsEnabled"/>.
        /// </summary>
        [Required]
        public bool BitsEnabled { get; set; } = false;

        /// <summary>
        /// Gets or sets the <see cref="BitsCondition"/> for this <see cref="Trigger" /> instance.
        /// </summary>
        [Required]
        public BitsCondition BitsCondition { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="MinimumBits"/> required to trigger this <see cref="Trigger"/> instance.
        /// </summary>
        [Required]
        public int MinimumBits { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="MinimumBits"/> required to trigger this <see cref="Trigger"/> instance.
        /// </summary>
        [Required]
        public int MaximumBits { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Trigger" /> instance can be triggered by Evewryone.
        /// </summary>
        [Required]
        public bool UserLevelEveryone { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Trigger" /> instance can be triggered by Subscribers.
        /// </summary>
        [Required]
        public bool UserLevelSubs { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Trigger" /> instance can be triggered by VIPs.
        /// </summary>
        [Required]
        public bool UserLevelVips { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Trigger" /> instance can be triggered by Mods.
        /// </summary>
        [Required]
        public bool UserLevelMods { get; set; } = false;

        /// <summary>
        /// Gets or Sets the <see cref="Keywords"/> for this <see cref="Trigger"/> instance.
        /// </summary>
        [Required]
        public string Keywords { get; set; }

        /// <summary>
        /// Gets or Sets the character of the <see cref="KeyCode"/> for this <see cref="Trigger"/> instance.
        /// </summary>
        [Required]
        public string CharAnimTriggerKeyChar { get; set; }

        /// <summary>
        /// Gets or Sets the value of the <see cref="KeyCode"/> for this <see cref="Trigger"/> instance.
        /// </summary>
        [Required]
        public int CharAnimTriggerKeyValue { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Cooldown"/> interval for this <see cref="Trigger"/> instance.
        /// </summary>
        [Required]
        public int Cooldown { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CooldownUnit"/> for this <see cref="Trigger" /> instance.
        /// </summary>
        [Required]
        public TimeSpanUnit CooldownUnit { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> that this <see cref="Trigger" /> instance was <see cref="LastTriggered"/>.
        /// </summary>
        public DateTime LastTriggered { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="RewardName"/> used to trigger this <see cref="Trigger" /> instance.
        /// </summary>
        [MaxLength(45)]
        public string RewardName { get; set; }
    }
}
