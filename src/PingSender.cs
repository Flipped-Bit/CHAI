using CHAI.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Timers;
using Thread = System.Threading.Thread;
using ThreadStart = System.Threading.ThreadStart;

namespace CHAI
{
    /// <summary>
    /// Class for maintaining an IRC connection.
    /// </summary>
    public class PingSender : IrcService
    {
        /// <summary>
        /// The <see cref="Timer"/>.
        /// </summary>
        protected Timer timer;

        /// <summary>
        /// The injected <see cref="ILogger{PingSender}"/>.
        /// </summary>
        private new readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PingSender"/> class.
        /// </summary>
        /// <param name="logger">The injected <see cref="ILogger"/>.</param>
        /// <param name="ircClient">The configured IRC Client.</param>
        public PingSender(ILogger logger, IrcClient ircClient)
            : base(logger)
        {
            _ircClient = ircClient;
            _logger = logger;
            thread = new Thread(new ThreadStart(Run));
            timer = new Timer
            {
                Interval = TimeSpan.FromMinutes(4).TotalMilliseconds,
                AutoReset = true,
            };
        }

        /// <summary>
        /// Method that is <see cref="Run"/> when the <see cref="Thread"/> is started.
        /// </summary>
        public override void Run()
        {
            timer.Elapsed += SendPing;
            timer.Enabled = true;
        }

        /// <summary>
        /// Method to Start the <see cref="Thread"/>.
        /// </summary>
        public new void Start()
        {
            IsActive = true;
            thread.IsBackground = true;
            thread.Start();
            _logger.LogInformation("Ping Sender started successfully");
        }

        /// <summary>
        /// Method to send ping to server every 5 Minutes/>.
        /// </summary>
        private void SendPing(object sender, ElapsedEventArgs e)
        {
            _ircClient.SendMessage("PING irc.twitch.tv");
        }
    }
}
