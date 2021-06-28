using CHAI.Data;
using CHAI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Timers;
using Thread = System.Threading.Thread;
using ThreadStart = System.Threading.ThreadStart;

namespace CHAI
{
    /// <summary>
    /// Class for handling <see cref="QueuedEvent"/>s.
    /// </summary>
    public class EventService
    {
        /// <summary>
        /// The <see cref="Thread"/>.
        /// </summary>
        protected Thread thread;

        /// <summary>
        /// The <see cref="Timer"/>.
        /// </summary>
        protected Timer timer;

        /// <summary>
        /// The Injected <see cref="ILogger{EventService}"/>.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The <see cref="Setting"/>s.
        /// </summary>
        private readonly Setting _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventService"/> class.
        /// </summary>
        /// <param name="logger">The Injected <see cref="ILogger"/>.</param>
        /// <param name="settings">The <see cref="Setting"/>s.</param>
        public EventService(ILogger logger, Setting settings)
        {
            _logger = logger;
            _settings = settings;
            thread = new Thread(new ThreadStart(Run));
            timer = new Timer
            {
                Interval = 1000,
                AutoReset = true,
            };
        }

        /// <summary>
        /// Method to <see cref="Start"/> the <see cref="Thread"/>.
        /// </summary>
        public void Start()
        {
            thread.IsBackground = true;
            thread.Start();
            _logger.LogInformation("Event service started successfully");
        }

        /// <summary>
        /// Method that is <see cref="Run"/> when the <see cref="Thread"/> is <see cref="Start"/>ed.
        /// </summary>
        public void Run()
        {
            timer.Elapsed += TriggerEvent;
            timer.Enabled = true;
        }

        /// <summary>
        /// Method to handle <see cref="QueuedEvent"/>s.
        /// </summary>
        private void TriggerEvent(object sender, ElapsedEventArgs e)
        {
            var context = new CHAIDbContextFactory()
                .CreateDbContext(null);

            var pendingEvent = context.EventQueue
                .OrderBy(e => e.TriggeredAt)
                .FirstOrDefault(e => e.TriggeredAt < DateTime.Now);
            if (pendingEvent != null)
            {
                var trigger = context.Triggers.FirstOrDefault(t => t.Id == pendingEvent.TriggerId);
                ProcessManager.SendKeyPress(_logger, _settings.Application, trigger.CharAnimTriggerKeyChar, trigger.CharAnimTriggerKeyValue);
                context.Remove(pendingEvent);
                var saved = false;
                while (!saved)
                {
                    try
                    {
                        // Attempt to save changes to the database
                        context.SaveChanges();
                        saved = true;
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        foreach (var entry in ex.Entries)
                        {
                            if (entry.Entity is QueuedEvent)
                            {
                                var proposedValues = entry.CurrentValues;
                                var databaseValues = entry.GetDatabaseValues();

                                foreach (var property in proposedValues.Properties)
                                {
                                    var proposedValue = proposedValues[property];
                                    var databaseValue = databaseValues[property];
                                }

                                // Refresh original values to bypass next concurrency check
                                entry.OriginalValues.SetValues(databaseValues);
                            }
                            else
                            {
                                throw new NotSupportedException(
                                    "Don't know how to handle concurrency conflicts for "
                                    + entry.Metadata.Name);
                            }
                        }
                    }
                }
            }
        }
    }
}
