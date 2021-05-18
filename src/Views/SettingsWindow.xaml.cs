using CHAI.Data;
using CHAI.Extensions;
using CHAI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Trigger = CHAI.Models.Trigger;

namespace CHAI.Views
{
    /// <summary>
    /// Interaction logic for <see cref="SettingsWindow"/>.xaml.
    /// </summary>
    public partial class SettingsWindow : Window
    {
        /// <summary>
        /// Constant for Numbers only <see cref="Regex"/>.
        /// </summary>
        private const string NUMBERONLYREGEX = @"^\d+$";

        /// <summary>
        /// The injected <see cref="CHAIDbContext"/>.
        /// </summary>
        private readonly CHAIDbContext _context;

        /// <summary>
        /// The injected <see cref="ILogger{LoginWindow}"/>.
        /// </summary>
        private readonly ILogger _loginWindowLogger;

        /// <summary>
        /// The injected <see cref="ILogger{SettingsWindow}"/>.
        /// </summary>
        private readonly ILogger _settingsWindowLogger;

        private User _currentUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsWindow"/> class.
        /// </summary>
        /// <param name="context">The injected <see cref="CHAIDbContext"/>.</param>
        /// <param name="loginLogger">The injected <see cref="ILogger"/> for <see cref="LoginWindow"/>.</param>
        /// <param name="settingsLogger">The injected <see cref="ILogger"/>.</param>
        public SettingsWindow(CHAIDbContext context, ILogger loginLogger, ILogger settingsLogger)
        {
            _context = context;
            _loginWindowLogger = loginLogger;
            _settingsWindowLogger = settingsLogger;
            CurrentSettings = _context.Settings.FirstOrDefault();
            this.DataContext = CurrentSettings;
            InitializeComponent();
            ActiveProcessMenu.ItemsSource = GetActiveProcesses();
            _settingsWindowLogger.LogInformation("Settings window initialised successfully");
        }

        /// <summary>
        /// Gets or sets the <see cref="CurrentUser"/>.
        /// </summary>
        public User CurrentUser
        {
            get
            {
                return _currentUser;
            }

            set
            {
                _currentUser = value;
                Username.Text = _currentUser.Username;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="CurrentProcess"/>.
        /// </summary>
        private Process CurrentProcess { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CurrentSettings"/>.
        /// </summary>
        private Setting CurrentSettings { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="LoginWindow"/>.
        /// </summary>
        private LoginWindow LoginWindow { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ProcessDictionary"/>.
        /// </summary>
        private Dictionary<string, Process> ProcessDictionary { get; set; } = new Dictionary<string, Process>();

        /// <summary>
        /// Method for setting selected <see cref="Process"/>.
        /// </summary>
        /// <param name="sender">The sender of <see cref="ActiveProcessSelected"/> event.</param>
        /// <param name="e">Arguments from <see cref="ActiveProcessSelected"/> event.</param>
        private void ActiveProcessSelected(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine(ActiveProcessMenu.SelectedItem.ToString());
            CurrentProcess = ProcessDictionary[ActiveProcessMenu.SelectedItem.ToString()];
        }

        /// <summary>
        /// Method to Export <see cref="Trigger"/>s to a file.
        /// </summary>
        /// <param name="sender">The sender of <see cref="ExportTriggers"/> event.</param>
        /// <param name="e">Arguments from <see cref="ExportTriggers"/> event.</param>
        private void ExportTriggers(object sender, RoutedEventArgs e)
        {
            SaveFileDialog exportDialog = new SaveFileDialog()
            {
                Filter = "JSON file (*.json)|*.json",
            };

            var triggers = _context.Triggers
                .Select(t => new TriggerDTO(t))
                .ToArray();

            string triggersAsJson = JsonConvert.SerializeObject(triggers, Formatting.Indented);

            bool? result = exportDialog.ShowDialog();

            if (exportDialog.ShowDialog() == true)
            {
                File.WriteAllText(exportDialog.FileName, triggersAsJson);
            }
        }

        /// <summary>
        /// Method for getting the active <see cref="Process"/>es.
        /// </summary>
        /// <returns>List of active <see cref="Process"/>es.</returns>
        private IEnumerable<string> GetActiveProcesses()
        {
            ProcessDictionary = Process.GetProcesses()
                    .Where(proc => proc.MainWindowTitle != string.Empty && proc.ProcessName != "CHAI")
                    .GroupBy(proc => proc.ProcessName)
                    .Select(g => g.Select((p, i) =>
                        g.ToList().Count > 1 ?
                        new { Key = $"{g.Key} ({i + 1})", Process = p } :
                        new { g.Key, Process = p }))
                    .SelectMany(p => p)
                    .ToDictionary(p => p.Key, p => p.Process);

            return ProcessDictionary.Select(p => p.Key);
        }

        /// <summary>
        /// Method for validating that input value is a number.
        /// </summary>
        /// <param name="sender">The sender of <see cref="GlobalCooldownValueLostFocus"/> event.</param>
        /// <param name="e">Arguments from <see cref="GlobalCooldownValueLostFocus"/> event.</param>
        private void GlobalCooldownValueLostFocus(object sender, RoutedEventArgs e)
        {
            GlobalCooldownValue.Text = Regex.Match(GlobalCooldownValue.Text, NUMBERONLYREGEX, RegexOptions.IgnoreCase).Success ?
                GlobalCooldownValue.Text : "0";
        }

        /// <summary>
        /// Method for decreasing <see cref="GlobalCooldownValue"/>.
        /// </summary>
        /// <param name="sender">The sender of <see cref="GlobalCooldownValueMinusClick"/> event.</param>
        /// <param name="e">Arguments from <see cref="GlobalCooldownValueMinusClick"/> event.</param>
        private void GlobalCooldownValueMinusClick(object sender, RoutedEventArgs e)
        {
            var currentValue = CurrentSettings.GlobalCooldown;
            CurrentSettings.GlobalCooldown = currentValue > 0 ? currentValue -= 1 : 0;
            GlobalCooldownValue.Text = Convert.ToString(CurrentSettings.GlobalCooldown);
        }

        /// <summary>
        /// Method for increasing <see cref="GlobalCooldownValue"/>.
        /// </summary>
        /// <param name="sender">The sender of <see cref="GlobalCooldownValuePlusClick"/> event.</param>
        /// <param name="e">Arguments from <see cref="GlobalCooldownValuePlusClick"/> event.</param>
        private void GlobalCooldownValuePlusClick(object sender, RoutedEventArgs e)
        {
            CurrentSettings.GlobalCooldown += 1;
            GlobalCooldownValue.Text = Convert.ToString(CurrentSettings.GlobalCooldown);
        }

        /// <summary>
        /// Method to Import <see cref="Trigger"/>s from a file.
        /// </summary>
        /// <param name="sender">The sender of <see cref="ImportTriggers"/> event.</param>
        /// <param name="e">Arguments from <see cref="ImportTriggers"/> event.</param>
        private void ImportTriggers(object sender, RoutedEventArgs e)
        {
            OpenFileDialog browseDialog = new OpenFileDialog()
            {
                Filter = "JSON file (*.json)|*.json",
            };

            var data = string.Empty;
            if (browseDialog.ShowDialog() == true)
            {
                data = File.ReadAllText(browseDialog.FileName);
            }

            if (!string.IsNullOrWhiteSpace(data))
            {
                List<TriggerDTO> triggerDTOs = JsonConvert.DeserializeObject<List<TriggerDTO>>(data);
                var triggers = triggerDTOs.Select(t => new Trigger(t));
                string replaceTriggers = ShowImportConfirmation();

                if (replaceTriggers == "Yes" || replaceTriggers == "No")
                {
                    if (replaceTriggers == "Yes")
                    {
                        _context.Triggers.Clear();
                        _context.SaveChanges();
                    }

                    _context.Triggers.AddRange(triggers);
                    _context.SaveChanges();

                    ((MainWindow)Owner).UpdateTriggersList();
                }
            }
        }

        /// <summary>
        /// Method for logging into Twitch API.
        /// </summary>
        /// <param name="sender">The sender of <see cref="Login"/> event.</param>
        /// <param name="e">Arguments from <see cref="Login"/> event.</param>
        private void Login(object sender, RoutedEventArgs e)
        {
            if (Application.Current.Windows.OfType<LoginWindow>().Any())
            {
                Application.Current.Windows.OfType<LoginWindow>()
                    .FirstOrDefault()
                    .Activate();
            }
            else
            {
                LoginWindow = new LoginWindow(_loginWindowLogger)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                };
                LoginWindow.Show();
            }
        }

        /// <summary>
        /// Method for refreshing Active Process Menu.
        /// </summary>
        /// <param name="sender">The sender of <see cref="RefreshActiveProcessMenu"/> event.</param>
        /// <param name="e">Arguments from <see cref="RefreshActiveProcessMenu"/> event.</param>
        private void RefreshActiveProcessMenu(object sender, RoutedEventArgs e)
        {
            GetActiveProcesses();
        }

        /// <summary>
        /// Method for resetting the <see cref="Setting.GlobalCooldown"/> value.
        /// </summary>
        /// <param name="sender">The sender of <see cref="ResetGlobalCooldown"/> event.</param>
        /// <param name="e">Arguments from <see cref="ResetGlobalCooldown"/> event.</param>
        private void ResetGlobalCooldown(object sender, RoutedEventArgs e)
        {
            CurrentSettings.GlobalCooldown = 30;
        }

        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            var currentSettings = _context.Settings.FirstOrDefault();
            currentSettings.Application = CurrentProcess != null ? CurrentProcess.ProcessName : string.Empty;

            if (CurrentUser != null)
            {
                currentSettings.OAuthToken = CurrentUser.Token;
                currentSettings.UserID = CurrentUser.UserId;
                currentSettings.Username = CurrentUser.Username;
            }

            _context.Update(currentSettings);
            _context.SaveChanges();
            ((MainWindow)Owner).RefreshConnectedApplication();
            ((MainWindow)Owner).RefreshIRC();
            Close();
        }

        /// <summary>
        /// Method for showing help info for Global Cooldown.
        /// </summary>
        /// <param name="sender">The sender of <see cref="ShowGlobalCooldownHelp"/> event.</param>
        /// <param name="e">Arguments from <see cref="ShowGlobalCooldownHelp"/> event.</param>
        private void ShowGlobalCooldownHelp(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "The global cooldown applies to ALL triggers. During this period, none of the triggers can be used.\n\n" +
                "This is useful for preventing people from spamming multiple different triggers in a short period.",
                "Global cooldown");
        }

        private string ShowImportConfirmation()
        {
            var dialogResult = MessageBox.Show(
                "Would you like to replace all existing triggers?\n\nSelecting no will merge in new triggers.",
                "Importing triggers...",
                MessageBoxButton.YesNoCancel);
            return dialogResult.ToString();
        }
    }
}
