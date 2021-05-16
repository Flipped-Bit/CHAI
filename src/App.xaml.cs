using CHAI.Data;
using CHAI.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Windows;

namespace CHAI
{
    /// <summary>
    /// Interaction logic for <see cref="App"/>.xaml.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// The path to <see cref="Environment.SpecialFolder.ApplicationData"/> folder.
        /// </summary>
        private static readonly string APPDATAFOLDER = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        /// <summary>
        /// The injected <see cref="IServiceProvider"/>.
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            ServiceCollection services = new ServiceCollection();

            services.AddDbContext<CHAIDbContext>(options =>
            {
                options.UseSqlite($"Data Source = {Path.Join(APPDATAFOLDER, "CHAI", "CHAI.db")}");
            });

            // Added Serilog
            var serilogLogger = new LoggerConfiguration()
            .WriteTo.File(
                Path.Join(APPDATAFOLDER, "CHAI", "Logs", $"{DateTime.Now:dd-MM-yyyy}.log"),
                LogEventLevel.Warning,
                outputTemplate: "[{SourceContext}] {Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.Debug()
            .CreateLogger();
            services.AddLogging(options =>
            {
                options.SetMinimumLevel(LogLevel.Information);
                options.AddSerilog(logger: serilogLogger, dispose: true);
            });

            services.AddSingleton<MainWindow>();
            _serviceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// Method for setting view to <see cref="MainWindow"/> on startup.
        /// </summary>
        /// <param name="sender">The sender of <see cref="OnStartup"/> event.</param>
        /// <param name="e">Arguments from <see cref="OnStartup"/> event.</param>
        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}
