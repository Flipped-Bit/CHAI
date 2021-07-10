using CHAI.Models.Enums;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace CHAI.Models
{
    /// <summary>
    /// Model representing a <see cref="Trigger"/> data transfer object.
    /// </summary>
    public class TriggerDTO
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerDTO"/> class.
        /// </summary>
        public TriggerDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerDTO"/> class from a <see cref="Trigger"/>.
        /// </summary>
        /// <param name="trigger"><see cref="Trigger"/> to construct from.</param>
        public TriggerDTO(Trigger trigger)
        {
            Name = trigger.Name;
            CreatedAt = trigger.CreatedAt;
            BitsEnabled = trigger.BitsEnabled;
            BitsCondition = (int)trigger.BitsCondition - 1;
            BitsAmount = trigger.MinimumBits;
            BitsAmount2 = trigger.MaximumBits;
            UserLevelEveryone = trigger.UserLevelEveryone;
            UserLevelSubs = trigger.UserLevelSubs;
            UserLevelVips = trigger.UserLevelVips;
            UserLevelMods = trigger.UserLevelMods;
            Keywords = JsonConvert.SerializeObject(trigger.Keywords.Split(',', StringSplitOptions.RemoveEmptyEntries));
            CharAnimTriggerKeyChar = trigger.CharAnimTriggerKeyChar;
            CharAnimTriggerKeyValue = trigger.CharAnimTriggerKeyValue;
            Cooldown = trigger.Cooldown;
            CooldownUnit = (int)trigger.CooldownUnit - 1;
            LastTriggered = trigger.LastTriggered;
            RewardName = trigger.RewardName ?? string.Empty;
            HasDeactivationTime = trigger.HasDeactivationTime;
            DeactivateAt = trigger.DeactivateAt;
            Duration = trigger.Duration;
            DurationUnit = trigger.DurationUnit;
        }

        /// <summary>
        /// Gets or sets the <see cref="Name"/> of this <see cref="Trigger" /> instance.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> that this <see cref="Trigger" /> instance was <see cref="CreatedAt"/>.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Trigger" /> instance has <see cref="BitsEnabled"/>.
        /// </summary>
        public bool BitsEnabled { get; set; } = false;

        /// <summary>
        /// Gets or sets the <see cref="BitsCondition"/> for this <see cref="Trigger" /> instance.
        /// </summary>
        public int BitsCondition { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="BitsAmount"/> required to trigger this <see cref="Trigger"/> instance.
        /// </summary>
        public int BitsAmount { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="BitsAmount2"/> required to trigger this <see cref="Trigger"/> instance.
        /// </summary>
        public int BitsAmount2 { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Trigger" /> instance can be triggered by Evewryone.
        /// </summary>
        public bool UserLevelEveryone { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Trigger" /> instance can be triggered by Subscribers.
        /// </summary>
        public bool UserLevelSubs { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Trigger" /> instance can be triggered by VIPs.
        /// </summary>
        public bool UserLevelVips { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Trigger" /> instance can be triggered by Mods.
        /// </summary>
        public bool UserLevelMods { get; set; } = false;

        /// <summary>
        /// Gets or Sets the <see cref="Keywords"/> for this <see cref="Trigger"/> instance.
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// Gets or Sets the character of the <see cref="KeyCode"/> for this <see cref="Trigger"/> instance.
        /// </summary>
        public string CharAnimTriggerKeyChar { get; set; }

        /// <summary>
        /// Gets or Sets the value of the <see cref="KeyCode"/> for this <see cref="Trigger"/> instance.
        /// </summary>
        public int CharAnimTriggerKeyValue { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Cooldown"/> interval for this <see cref="Trigger"/> instance.
        /// </summary>
        public int Cooldown { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CooldownUnit"/> for this <see cref="Trigger" /> instance.
        /// </summary>
        public int CooldownUnit { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> that this <see cref="Trigger" /> instance was <see cref="LastTriggered"/>.
        /// </summary>
        public DateTime LastTriggered { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="RewardName"/> used to trigger this <see cref="Trigger" /> instance.
        /// </summary>
        public string RewardName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Trigger"/> instance has <see cref="HasDeactivationTime"/>.
        /// </summary>
        public bool HasDeactivationTime { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> that this <see cref="Trigger"/> instance is going to be deactivated at.
        /// </summary>
        public DateTime? DeactivateAt { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="Duration"/> interval for this <see cref="Trigger"/> instance.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DurationUnit"/> for this <see cref="Trigger"/> instance.
        /// </summary>
        public TimeSpanUnit DurationUnit { get; set; }
    }
}
