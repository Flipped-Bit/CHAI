using CHAI.Models;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace CHAI
{
    /// <summary>
    /// Class for handling <see cref="IrcClient"/>s.
    /// </summary>
    public class IrcService
    {
        /// <summary>
        /// The <see cref="IrcClient"/>.
        /// </summary>
        protected IrcClient _ircClient;

        /// <summary>
        /// The <see cref="Thread"/>.
        /// </summary>
        protected Thread thread;

        /// <summary>
        /// The Injected <see cref="ILogger{IrcService}"/>.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="IrcService"/> class.
        /// </summary>
        /// <param name="logger">The Injected <see cref="ILogger"/>.</param>
        public IrcService(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="IrcService"/> <see cref="IsActive"/>.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Method to Start the <see cref="Thread"/>.
        /// </summary>
        public void Start()
        {
            IsActive = true;
            thread.IsBackground = true;
            thread.Start();
            _logger.LogInformation("IRC service started successfully");
        }

        /// <summary>
        /// Method to Stop the <see cref="Thread"/>.
        /// </summary>
        public void Stop()
        {
            IsActive = false;
        }

        /// <summary>
        /// Method that is <see cref="Run"/> when the <see cref="Thread"/> is <see cref="Start"/>ed.
        /// </summary>
        public virtual void Run()
        {
            while (IsActive)
            {
                _logger.LogInformation("Thread running");
            }
        }
    }
}
