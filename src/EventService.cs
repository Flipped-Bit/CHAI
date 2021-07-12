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
        /// The Injected <see cref="ILogger{ProcessManager}"/>.
        /// </summary>
        private readonly ILogger _processManagerLogger;

        /// <summary>
        /// The <see cref="Setting"/>s.
        /// </summary>
        private readonly Setting _settings;

        private bool queueEmpty;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventService"/> class.
        /// </summary>
        /// <param name="logger">The Injected <see cref="ILogger"/>.</param>
        /// <param name="processManagerLogger">The Injected <see cref="ILogger{ProcessManager}"/>.</param>
        /// <param name="settings">The <see cref="Setting"/>s.</param>
        public EventService(ILogger logger, ILogger processManagerLogger, Setting settings)
        {
            _logger = logger;
            _processManagerLogger = processManagerLogger;
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
            LogEventQueue();

            var pendingEvent = GetNextEvent();

            if (pendingEvent != null)
            {
                var trigger = GetTrigger(pendingEvent.TriggerId);

                ProcessManager.SendKeyPress(_processManagerLogger, _settings.Application, trigger.CharAnimTriggerKeyChar, trigger.CharAnimTriggerKeyValue);

                DequeueEvent(pendingEvent);
            }
        }

        private void LogEventQueue()
        {
            var context = new CHAIDbContextFactory()
                .CreateDbContext(null);

            var events = context.EventQueue
                .OrderBy(e => e.TriggeredAt)
                .ToList();

            if (events.Count > 0)
            {
                _logger.LogInformation("          ");
                _logger.LogInformation("----------");
                events.ForEach(e => _logger.LogInformation($"{e.TriggerId} | {e.TriggeredAt.ToLongTimeString()}"));
                queueEmpty = false;
                _logger.LogInformation("----------");
            }
            else
            {
                if (!queueEmpty)
                {
                    _logger.LogInformation("Event Queue is empty");
                    queueEmpty = true;
                }
            }
        }

        private QueuedEvent GetNextEvent()
        {
            var context = new CHAIDbContextFactory()
                    .CreateDbContext(null);

            return context.EventQueue
                .OrderBy(e => e.TriggeredAt)
                .FirstOrDefault(e => e.TriggeredAt < DateTime.Now);
        }

        private void DequeueEvent(QueuedEvent triggeredEvent)
        {
            var saved = false;
            while (!saved)
            {
                try
                {
                    var context = new CHAIDbContextFactory()
                        .CreateDbContext(null);
                    context.Remove(triggeredEvent);

                    // Attempt to save changes to the database
                    _logger.LogInformation($"Event {(context.SaveChanges() > 0 ? "removed successfully" : "removal failed")}");
                    saved = true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entity = ex.Entries.FirstOrDefault().Entity is QueuedEvent ?
                        ex.Entries.FirstOrDefault().Entity as QueuedEvent : default;
                    _logger.LogError($"Entity {entity} not removed: {ex.Message}");
                }
                catch (Exception exc)
                {
                    _logger.LogError($"Whoops!!, {exc.Message}");
                }
            }
        }

        private Trigger GetTrigger(int triggerId)
        {
            var context = new CHAIDbContextFactory()
                    .CreateDbContext(null);

            return context.Triggers.FirstOrDefault(t => t.Id == triggerId);
        }
    }
}
