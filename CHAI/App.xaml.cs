using CHAI.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Windows;

namespace CHAI
{
    /// <summary>
    /// Interaction logic for <see cref="App"/>.xaml.
    /// </summary>
    public partial class App : Application
    {
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

            // Added Serilog
            var serilogLogger = new LoggerConfiguration()
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
