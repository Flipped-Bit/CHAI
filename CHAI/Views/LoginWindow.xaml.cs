using CHAI.Extensions;
using CHAI.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Web;
using System.Windows;

namespace CHAI.Views
{
    /// <summary>
    /// Interaction logic for <see cref="LoginWindow"/>.xaml.
    /// </summary>
    public partial class LoginWindow : Window
    {
        /// <summary>
        /// The injected <see cref="ILogger{LoginWindow}"/>.
        /// </summary>
        private readonly ILogger _loginWindowLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginWindow"/> class.
        /// </summary>
        public LoginWindow(ILogger logger)
        {
            _loginWindowLogger = logger;
            InitializeComponent();
            DataContext = Owner;
        }
    }
}
