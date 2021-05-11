using CHAI.Data;
using CHAI.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

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
        /// The injected <see cref="ILogger"/>.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsWindow"/> class.
        /// </summary>
        public SettingsWindow(CHAIDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
            CurrentSettings = _context.Settings.FirstOrDefault();
            this.DataContext = CurrentSettings;
            InitializeComponent();
            ActiveProcessMenu.ItemsSource = GetActiveProcesses();
            _logger.LogInformation("Settings window initialised successfully");
        }

        private Process CurrentProcess { get; set; }

        private Setting CurrentSettings { get; set; }

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
    }
}
