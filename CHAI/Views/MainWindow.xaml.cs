using CHAI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Trigger = CHAI.Models.Trigger;

namespace CHAI.Views
{
    /// <summary>
    /// Interaction logic for <see cref="MainWindow"/>.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The injected <see cref="CHAIDbContext"/>.
        /// </summary>
        private readonly CHAIDbContext _context;

        /// <summary>
        /// The injected <see cref="ILogger"/>.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow(CHAIDbContext context, ILogger<MainWindow> logger)
        {
            _context = context;
            _logger = logger;
            InitializeComponent();
            UpdateTriggersList();
            _logger.LogInformation("Main window initialised successfully");
        }

        /// <summary>
        /// Method for updating the <see cref="TriggersList"/> with all <see cref="Trigger"/>s.
        /// </summary>
        private void UpdateTriggersList()
        {
            TriggersList.ItemsSource = _context.Triggers.ToList();
        }
    }
}
