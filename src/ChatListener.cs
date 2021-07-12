using CHAI.Data;
using CHAI.Models;
using CHAI.Models.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace CHAI
{
    /// <summary>
    /// Class for listing to <see cref="IrcClient"/>s.
    /// </summary>
    public class ChatListener : IrcService
    {
        /// <summary>
        /// The injected <see cref="ILogger{ChatListener}"/>.
        /// </summary>
        private new readonly ILogger _logger;

        /// <summary>
        /// The Injected <see cref="ILogger{ProcessManager}"/>.
        /// </summary>
        private readonly ILogger _processManagerLogger;

        private readonly List<Trigger> _triggers;

        private readonly Setting _settings;

        /// <summary>
        /// The channel the <see cref="IrcClient"/> is connected to/>.
        /// </summary>
        private readonly string _channel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatListener"/> class.
        /// </summary>
        /// <param name="logger">The injected <see cref="ILogger"/>.</param>
        /// <param name="processManagerLogger">The Injected <see cref="ILogger{ProcessManager}"/>.</param>
        /// <param name="settings">The current <see cref="Setting"/>.</param>
        /// <param name="triggers">List of available <see cref="Trigger"/>s.</param>
        /// <param name="ircClient">The configured IRC Client.</param>
        /// <param name="channel">The channel the <see cref="IrcClient"/> is connected to/>.</param>
        public ChatListener(ILogger logger, ILogger processManagerLogger, Setting settings, List<Trigger> triggers, IrcClient ircClient, string channel)
            : base(logger)
        {
            _channel = channel;
            _processManagerLogger = processManagerLogger;
            _settings = settings;
            _triggers = triggers;
            _ircClient = ircClient;
            _logger = logger;
            IsActive = false;
            IsLogging = false;
            thread = new Thread(new ThreadStart(Run));
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ChatListener"/> <see cref="IsLogging"/>.
        /// </summary>
        public bool IsLogging { get; set; }

        /// <summary>
        /// Method to start logging for the <see cref="ChatListener"/>.
        /// </summary>
        public void StartLogging()
        {
            _logger.LogInformation("+------------------+");
            _logger.LogInformation("| Starting chatlog |");
            _logger.LogInformation("+------------------+");
            IsLogging = true;
        }

        /// <summary>
        /// Method to Start the <see cref="Thread"/>.
        /// </summary>
        public override void Run()
        {
            Dictionary<int, bool> stateDictionary =
               _triggers.ToDictionary(t => t.Id, t => false);

            var eventService = new EventService(_logger, _processManagerLogger, _settings);
            eventService.Start();

            while (IsActive)
            {
                string message = _ircClient.ReadMessage();

                if (message != null)
                {
                    switch (message)
                    {
                        case var m when m.Contains("PRIVMSG"):
                            if (IsLogging)
                            {
                                var messageInfo = new Message(message, _channel);
                                RenderMessage(messageInfo);

                                if (string.IsNullOrEmpty(_settings.Application))
                                {
                                    _logger.LogInformation("Application not connected");
                                    return;
                                }

                                if (IsOnCooldown())
                                {
                                    _logger.LogInformation("Triggers are on global cooldown");
                                    return;
                                }

                                List<Trigger> triggers = _triggers
                                    .Where(trigger => string.IsNullOrEmpty(trigger.RewardName) && ContainsRequiredWord(trigger, messageInfo))
                                    .ToList();

                                foreach (var trigger in triggers)
                                {
                                    if (IsOnCooldown(trigger))
                                    {
                                        _logger.LogInformation($"{trigger.Name} is on cooldown");
                                        continue;
                                    }

                                    var bitsRequired = AreBitsRequired(trigger);

                                    if (bitsRequired && messageInfo.Bits == 0)
                                    {
                                        _logger.LogInformation("Bits were expected but none were given");
                                        continue;
                                    }

                                    var validBits = HasEnoughBits(messageInfo.Bits, trigger);

                                    if (!bitsRequired && trigger.Keywords != string.Empty)
                                    {
                                        validBits = true;
                                    }

                                    if (!validBits)
                                    {
                                        _logger.LogInformation("Not enough Bits given");
                                        continue;
                                    }

                                    if (!HasCorrectUserLevel(messageInfo, trigger))
                                    {
                                        _logger.LogInformation("User is wrong userlevel");
                                        continue;
                                    }

                                    if (trigger.Keywords != string.Empty && !PassesKeywordCheck(trigger.Keywords, messageInfo.Content))
                                    {
                                        continue;
                                    }

                                    _logger.LogInformation($"{trigger.Name} matched!!");

                                    trigger.LastTriggered = DateTime.Now;

                                    var context = new CHAIDbContextFactory()
                                        .CreateDbContext(null);

                                    // add event for activation to queue
                                    context.EventQueue.Add(new QueuedEvent()
                                    {
                                        TriggeredAt = trigger.LastTriggered,
                                        TriggerId = trigger.Id,
                                    });
                                    _logger.LogInformation($"Event {(context.SaveChanges() > 0 ? "added successfully" : "adding failed")}");

                                    if (trigger.HasDeactivationTime && trigger.Duration >= 1 && trigger.DurationUnit != TimeSpanUnit.None)
                                    {
                                        var isActive = stateDictionary[trigger.Id];

                                        if (isActive)
                                        {
                                            context = new CHAIDbContextFactory()
                                                .CreateDbContext(null);

                                            // remove follow up event for deactivation from queue
                                            var eventsToRemove = context.EventQueue.Where(e => e.TriggerId == trigger.Id);
                                            context.RemoveRange(eventsToRemove);

                                            _logger.LogInformation($"Event {(context.SaveChanges() > 0 ? "removed successfully" : "removal failed")}");
                                        }
                                        else
                                        {
                                            var delay = trigger.DurationUnit switch
                                            {
                                                TimeSpanUnit.Seconds => TimeSpan.FromSeconds(trigger.Duration),
                                                TimeSpanUnit.Minutes => TimeSpan.FromMinutes(trigger.Duration),
                                                TimeSpanUnit.Hours => TimeSpan.FromHours(trigger.Duration),
                                                _ => TimeSpan.FromSeconds(0),
                                            };

                                            var triggerDelay = trigger.LastTriggered.Add(delay);

                                            context = new CHAIDbContextFactory()
                                                .CreateDbContext(null);

                                            // add follow up event for deactivation to queue
                                            context.EventQueue.Add(new QueuedEvent()
                                            {
                                                TriggeredAt = triggerDelay,
                                                TriggerId = trigger.Id,
                                            });

                                            _logger.LogInformation($"Event {(context.SaveChanges() > 0 ? "added successfully" : "adding failed")}");
                                        }

                                        stateDictionary[trigger.Id] = !stateDictionary[trigger.Id];
                                    }

                                    if (_settings.LoggingEnabled)
                                    {
                                        _logger.LogInformation($"Trigger:{trigger.Name} triggered by {messageInfo.UserName} with {messageInfo.Bits} bits");
                                    }
                                }
                            }

                            break;
                        case var p when p.Contains("PONG"):
                            _logger.LogInformation("PONG");
                            break;
                        case var r when r.Contains("RECONNECT"):
                            _logger.LogWarning(r);
                            break;
                        default:
                            _logger.LogInformation(message);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Method to Start the <see cref="Thread"/>.
        /// </summary>
        public new void Start()
        {
            IsActive = true;
            thread.IsBackground = true;
            thread.Start();
            _logger.LogInformation("Chat Listener started successfully");
        }

        /// <summary>
        /// Method for rendering message to Output window.
        /// </summary>
        /// <param name="message">Message to render.</param>
        private void RenderMessage(Message message)
        {
            var output = $"{message.SentTime.ToLocalTime():G} ";

            foreach (var badge in message.Badges)
            {
                output += $"{badge} ";
            }

            output += $"{message.UserName}: {message.Content}";

            _logger.LogInformation(output);
        }

        /// <summary>
        /// Method to check if Bits are required for the <see cref="Trigger"/>.
        /// </summary>
        /// <param name="trigger">The <see cref="Trigger"/> to check.</param>
        /// <returns>Bool value indicating whether Bits are required for the <see cref="Trigger"/>.</returns>
        private bool AreBitsRequired(Trigger trigger)
        {
            return trigger.BitsEnabled && trigger.MinimumBits > 0;
        }

        /// <summary>
        /// Method to check if message contains any of the required words for the <see cref="Trigger"/>.
        /// </summary>
        /// <param name="trigger">The <see cref="Trigger"/> to check.</param>
        /// <param name="message">The <see cref="Message"/> to check.</param>
        /// <returns>
        /// Bool value indicating whether the message contains any of the required words for the <see cref="Trigger"/>.
        /// </returns>
        private bool ContainsRequiredWord(Trigger trigger, Message message)
        {
            return trigger.Keywords
                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Any(k => message.Content.Contains(k, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Method to check if <see cref="Trigger"/> is on Cooldown.
        /// </summary>
        /// <param name="lastTriggered"><see cref="DateTime"/> that <see cref="Trigger"/> was last triggered.</param>
        /// <param name="cooldownUnit"><see cref="TimeSpanUnit"/>.</param>
        /// <param name="cooldown">Duration of Coooldown.</param>
        /// <returns>Bool value indicating whether the <see cref="Trigger"/> Cooldown has ended.</returns>
        private DateTime GetEndOfCooldown(DateTime lastTriggered, TimeSpanUnit cooldownUnit, int cooldown)
        {
            switch (cooldownUnit)
            {
                case TimeSpanUnit.None:
                    _logger.LogInformation("Trigger has no Cooldown Unit set");
                    break;
                case TimeSpanUnit.Seconds:
                    return lastTriggered.AddSeconds(cooldown);
                case TimeSpanUnit.Minutes:
                    return lastTriggered.AddMinutes(cooldown);
                case TimeSpanUnit.Hours:
                    return lastTriggered.AddHours(cooldown);
                default:
                    break;
            }

            return DateTime.MinValue;
        }

        /// <summary>
        /// Method to check whether <see cref="Trigger"/> is on cooldown.
        /// </summary>
        /// <param name="trigger">The <see cref="Trigger"/> to check.</param>
        /// <returns>Bool value indicating whether the <see cref="Trigger"/> is on Cooldown.</returns>
        private bool IsOnCooldown(Trigger trigger = default)
        {
            DateTime cooldownEnd;
            if (trigger != null)
            {
                cooldownEnd = GetEndOfCooldown(trigger.LastTriggered, trigger.CooldownUnit, trigger.Cooldown);
            }
            else
            {
                cooldownEnd = GetEndOfCooldown(_settings.GlobalLastTriggered, _settings.GlobalCooldownUnit, _settings.GlobalCooldown);
            }

            return cooldownEnd > DateTime.Now;
        }

        /// <summary>
        /// Method to check <see cref="Message"/> contains the right Keyword(s).
        /// </summary>
        /// <param name="stringifiedkeywords">CSV list of keywords.</param>
        /// <param name="message">Message to check keyword(s).</param>
        /// <returns>Bool value indicating whether the <see cref="Message"/> contains the right Keyword(s).</returns>
        private bool PassesKeywordCheck(string stringifiedkeywords, string message)
        {
            List<string> rawkeywords = stringifiedkeywords
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .ToList();
            MatchCollection matches = null;
            MatchCollection hmatches = null;

            if (message.StartsWith("!"))
            {
                List<string> commands = rawkeywords.Where(k => k.StartsWith("!")).Select(k => Regex.Escape(k[1..])).ToList();
                if (commands.Count == 0)
                {
                    return false;
                }

                string commanded = string.Join("|", commands);
                Regex regexCommands = new Regex(@"^!(" + commanded + @")$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                matches = regexCommands.Matches(message);
                _logger.LogInformation("commands:" + commanded);
            }
            else
            {
                List<string> keywords = rawkeywords.Where(k => !k.StartsWith("!") && !k.StartsWith("#")).Select(k => Regex.Escape(k)).ToList();
                List<string> hashtags = rawkeywords.Where(k => k.StartsWith("#")).Select(k => Regex.Escape(k[1..])).ToList();

                if (keywords.Count == 0 && hashtags.Count == 0)
                {
                    return false;
                }

                if (keywords.Count > 0)
                {
                    var keyworded = string.Join("|", keywords);
                    Regex regexKeywords = new Regex(@"\b(" + keyworded + @")\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    matches = regexKeywords.Matches(message);
                    _logger.LogInformation("keywords:" + keyworded);
                }

                if (hashtags.Count > 0)
                {
                    var hashtagged = string.Join("|", hashtags);
                    Regex regexHashtags = new Regex(@"\B#(" + hashtagged + @")\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    hmatches = regexHashtags.Matches(message);
                    _logger.LogInformation("hashtags:" + hashtagged);
                }
            }

            if ((matches == null || matches.Count == 0) && (hmatches == null || hmatches.Count == 0))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Method to check if User <see cref="HasCorrectUserLevel"/> to trigger the <see cref="Trigger"/>.
        /// </summary>
        /// <param name="message"><see cref="Message"/> to check.</param>
        /// <param name="trigger">The <see cref="Trigger"/> to trigger.</param>
        /// <returns>Bool value indicating whether the <see cref="User"/> has the correct user level.</returns>
        private bool HasCorrectUserLevel(Message message, Trigger trigger)
        {
            if (!trigger.UserLevelEveryone)
            {
                return message.IsMod == trigger.UserLevelMods &
                    message.IsSub == trigger.UserLevelSubs &
                message.IsVIP == trigger.UserLevelVips;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Method to Check if message <see cref="HasEnoughBits"/> to trigger the <see cref="Trigger"/>.
        /// </summary>
        /// <param name="bits">The Bits thrown with the message.</param>
        /// <param name="trigger">The <see cref="Trigger"/> to trigger.</param>
        /// <returns>Bool value indicating whether enough Bits were thrown.</returns>
        private bool HasEnoughBits(int bits, Trigger trigger)
        {
            switch (trigger.BitsCondition)
            {
                case BitsCondition.None:
                    _logger.LogInformation("Trigger has no Bit Condition set");
                    break;
                case BitsCondition.Atleast:
                    return bits >= trigger.MinimumBits;
                case BitsCondition.AtMost:
                    return bits <= trigger.MinimumBits;
                case BitsCondition.Exactly:
                    return bits == trigger.MinimumBits;
                case BitsCondition.Between:
                    return bits >= trigger.MinimumBits &&
                        bits <= trigger.MaximumBits;
                default:
                    _logger.LogError("Trigger has unhandled Bit Condition");
                    break;
            }

            return false;
        }
    }
}
