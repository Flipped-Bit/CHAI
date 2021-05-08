using Microsoft.Extensions.Logging;
using System.Windows;

namespace CHAI.Views
{
    /// <summary>
    /// Interaction logic for <see cref="MainWindow"/>.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The injected <see cref="ILogger"/>.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow(ILogger<MainWindow> logger)
        {
            _logger = logger;
            InitializeComponent();
            _logger.LogInformation("Main window initialised successfully");
        }
    }
}
