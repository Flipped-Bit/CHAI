using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CHAI.Models
{
    /// <summary>
    /// Model representing an IRC <see cref="Message"/>.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        /// <param name="message">The message to parse.</param>
        /// <param name="channel">The channel that the message originated from.</param>
        public Message(string message, string channel)
        {
            var headers = message.Split($"#{channel} :", 2)[0];
            UserName = GetUsernameFrom(headers);
            Badges = GetBadgesFrom(headers);
            Bits = GetBitsFrom(headers);
            Content = message.Split($"#{channel} :", 2)[1];
            IsMod = Badges.Any(b => b == "moderator");
            IsSub = Badges.Any(b => b == "subscriber");
            IsVIP = Badges.Any(b => b == "vip");
            SentTime = GetSentTimeFrom(headers);
        }

        /// <summary>
        /// Gets or sets a List of <see cref="Badges"/> of the <see cref="User"/> that created the <see cref="Message"/>.
        /// </summary>
        public List<string> Badges { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Bits"/> cheered in the <see cref="Message"/>.
        /// </summary>
        public int Bits { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Content"/> of the <see cref="Message"/>.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the creator of the <see cref="Message"/> <see cref="IsMod"/>.
        /// </summary>
        public bool IsMod { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the creator of the <see cref="Message"/> <see cref="IsSub"/>.
        /// </summary>
        public bool IsSub { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the creator of the <see cref="Message"/> <see cref="IsVIP"/>.
        /// </summary>
        public bool IsVIP { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="SentTime"/> of the <see cref="Message"/> as a Unix Timestamp.
        /// </summary>
        public DateTime SentTime { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="UserName"/> of the creator of the <see cref="Message"/>.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Method for extracting <see cref="Bits"/> from an IRC message.
        /// </summary>
        /// <param name="message">Message to extract from.</param>
        /// <returns><see cref="Bits"/>.</returns>
        private int GetBitsFrom(string message)
        {
            if (message.Contains("bits="))
            {
                Match m = Regex.Match(message, @"(bits=[\d]+;)", RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    var bits = m.Value[5..^1];
                    return string.IsNullOrWhiteSpace(bits) ? 0 : Convert.ToInt32(bits);
                }
            }

            return 0;
        }

        /// <summary>
        /// Method for extracting <see cref="Badges"/> from an IRC message.
        /// </summary>
        /// <param name="message">Message to extract from.</param>
        /// <returns>List of <see cref="Badges"/>.</returns>
        private List<string> GetBadgesFrom(string message)
        {
            if (message.Contains("badges="))
            {
                Match m = Regex.Match(message, @"(badges=[\w\d\/,]+;)", RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    var badges = m.Value[7..^1].Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Select(b => b.Split('/', 2)[0])
                        .ToList();
                    return badges;
                }
            }

            return new List<string>();
        }

        /// <summary>
        /// Method for extracting <see cref="SentTime"/> from an IRC message.
        /// </summary>
        /// <param name="message">Message to extract from.</param>
        /// <returns><see cref="SentTime"/>.</returns>
        private DateTime GetSentTimeFrom(string message)
        {
            if (message.Contains("tmi-sent-ts="))
            {
                Match m = Regex.Match(message, @"(tmi-sent-ts=[\d]+;)", RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    var sentTime = m.Value[12..^1];
                    var unixDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    var secondsToAdd = Convert.ToDouble(sentTime);

                    return unixDateTime.AddMilliseconds(secondsToAdd);
                }
            }

            return DateTime.MinValue;
        }

        /// <summary>
        /// Method for extracting <see cref="UserName"/> from an IRC message.
        /// </summary>
        /// <param name="message">Message to extract from.</param>
        /// <returns><see cref="UserName"/>.</returns>
        private string GetUsernameFrom(string message)
        {
            var username = string.Empty;
            if (message.Contains("display-name="))
            {
                Match m = Regex.Match(message, @"(display-name=[^;]+;)", RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    username = m.Value[13..^1];
                }
            }

            if (!Regex.IsMatch(username, "[a-zA-Z0-9_]{4,25}"))
            {
                Match m = Regex.Match(message, @"(@[a-zA-Z0-9_]+\.tmi\.twitch\.tv)", RegexOptions.IgnoreCase);

                var anglisedUsername = m.Value[1..^14];
                return $"{username}({anglisedUsername})";
            }
            else
            {
                return username;
            }
        }
    }
}
