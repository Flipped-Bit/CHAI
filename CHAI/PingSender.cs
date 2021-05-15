using CHAI.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace CHAI
{
    /// <summary>
    /// Class for maintaining an IRC connection.
    /// </summary>
    public class PingSender : IrcService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PingSender"/> class.
        /// </summary>
        /// <param name="logger">The injected <see cref="ILogger"/>.</param>
        /// <param name="ircClient">The configured IRC Client.</param>
        public PingSender(ILogger logger, IrcClient ircClient)
            : base(logger)
        {
            _ircClient = ircClient;
            IsActive = false;
            thread = new Thread(new ThreadStart(Run));
        }

        /// <summary>
        /// Method to send ping to server every 5 Minutes/>.
        /// </summary>
        public override void Run()
        {
            while (IsActive)
            {
                _ircClient.SendMessage("PING irc.twitch.tv");
                Thread.Sleep(TimeSpan.FromMinutes(5));
            }
        }
    }
}
