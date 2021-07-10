using CHAI.Data;
using CHAI.Extensions;
using CHAI.Models;
using CHAI.Models.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Trigger = CHAI.Models.Trigger;

namespace CHAI.Views
{
    /// <summary>
    /// Interaction logic for <see cref="MainWindow"/>.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Constant for Numbers only <see cref="Regex"/>.
        /// </summary>
        private const string NUMBERONLYREGEX = @"^\d+$";

        private static readonly NameValueCollection ClientData = (NameValueCollection)ConfigurationManager.GetSection("AppSettings/clientData");

        private static readonly NameValueCollection Endpoints = (NameValueCollection)ConfigurationManager.GetSection("AppSettings/endpoints");

        /// <summary>
        /// The injected <see cref="CHAIDbContext"/>.
        /// </summary>
        private readonly CHAIDbContext _context;

        /// <summary>
        /// The injected <see cref="ILogger{ChatListener}"/>.
        /// </summary>
        private readonly ILogger _chatlistenerLogger;

        /// <summary>
        /// The injected <see cref="ILogger{IrcService}"/>.
        /// </summary>
        private readonly ILogger _ircLogger;

        /// <summary>
        /// The injected <see cref="ILogger{PingSender}"/>.
        /// </summary>
        private readonly ILogger _pingLogger;

        /// <summary>
        /// The injected <see cref="ILogger{LoginWindow}"/>.
        /// </summary>
        private readonly ILogger _loginWindowLogger;

        /// <summary>
        /// The injected <see cref="ILogger{MainWindow}"/>.
        /// </summary>
        private readonly ILogger _mainWindowlogger;

        /// <summary>
        /// The Injected <see cref="ILogger{ProcessManager}"/>.
        /// </summary>
        private readonly ILogger _processManagerLogger;

        /// <summary>
        /// The injected <see cref="ILogger{SettingsWindow}"/>.
        /// </summary>
        private readonly ILogger _settingsWindowLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        /// <param name="context">The injected <see cref="CHAIDbContext"/>.</param>
        /// <param name="chatListenerLogger">The injected <see cref="ILogger"/> for <see cref="ChatListener"/>.</param>
        /// <param name="loginLogger">The injected <see cref="ILogger"/> for <see cref="LoginWindow"/>.</param>
        /// <param name="ircLogger">The injected <see cref="ILogger{IrcService}"/>.</param>
        /// <param name="mainLogger">The injected <see cref="ILogger"/>.</param>
        /// <param name="pingLogger">The injected <see cref="ILogger"/> for <see cref="PingSender"/>.</param>
        /// <param name="processManagerLogger">The injected <see cref="ILogger"/> for <see cref="ProcessManager"/>.</param>
        /// <param name="settingsLogger">The injected <see cref="ILogger"/> for <see cref="SettingsWindow"/>.</param>
        public MainWindow(
            CHAIDbContext context,
            ILogger<ChatListener> chatListenerLogger,
            ILogger<IrcService> ircLogger,
            ILogger<LoginWindow> loginLogger,
            ILogger<MainWindow> mainLogger,
            ILogger<PingSender> pingLogger,
            ILogger<ProcessManager> processManagerLogger,
            ILogger<SettingsWindow> settingsLogger)
        {
            _context = context;
            _chatlistenerLogger = chatListenerLogger;
            _ircLogger = ircLogger;
            _loginWindowLogger = loginLogger;
            _mainWindowlogger = mainLogger;
            _pingLogger = pingLogger;
            _processManagerLogger = processManagerLogger;
            _settingsWindowLogger = settingsLogger;
            InitializeComponent();
            RefreshConnectedApplication();
            RefreshIRC();
            UpdateTriggersList();
            var window = GetWindow(this);
            window.KeyDown += KeyDown;
            Closing += MainWindowClosing;
            _mainWindowlogger.LogInformation("Main window initialised successfully");
        }

        /// <summary>
        /// Gets or sets a <see cref="IrcClient"/> for the <see cref="MainWindow"/>.
        /// </summary>
        public IrcClient IrcClient { get; set; }

        /// <summary>
        /// Gets or sets the selected <see cref="Setting"/>.
        /// </summary>
        public Setting Settings { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="SettingsWindow"/> child for the <see cref="MainWindow"/>.
        /// </summary>
        public SettingsWindow SettingsWindow { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the selected <see cref="Process"/> is active.
        /// </summary>
        private bool HasActiveProcess { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Key presses are being recorded.
        /// </summary>
        private bool IsRecordingKey { get; set; }

        /// <summary>
        /// Method for refreshing Connected <see cref="Process"/>.
        /// </summary>
        public void RefreshConnectedApplication()
        {
            Settings = _context.Settings.FirstOrDefault();
            if (Settings != null)
            {
                HasActiveProcess = ProcessManager.ProcessRunning(Settings.Application);
            }

            if (HasActiveProcess)
            {
                ApplicationConnectionState.Text = $"{Settings.Application} Connected!";
                ApplicationConnectionState.Foreground = Brushes.Green;
            }
            else
            {
                ApplicationConnectionState.Text = string.IsNullOrWhiteSpace(Settings.Application) ? "Set Application in Settings" : $"{Settings.Application} not found!";
                ApplicationConnectionState.Foreground = Brushes.Red;
            }
        }

        /// <summary>
        /// Method for refreshing conection to IRC.
        /// </summary>
        public void RefreshIRC()
        {
            if (!string.IsNullOrWhiteSpace(Settings.Username))
            {
                CreateIRCClient(Settings.Username.ToLower());
                if (IrcClient != null)
                {
                    StartIRCConnection();
                    ChatConnectedState.Text = "Chat connected";
                    ChatConnectedState.Foreground = Brushes.Green;
                }
                else
                {
                    ChatConnectedState.Text = "Chat disconnected";
                    ChatConnectedState.Foreground = Brushes.Red;
                }
            }
            else
            {
                ChatConnectedState.Text = "Chat disconnected";
                ChatConnectedState.Foreground = Brushes.Red;
            }
        }

        /// <summary>
        /// Method for updating the <see cref="TriggersList"/> with all <see cref="Trigger"/>s.
        /// </summary>
        public void UpdateTriggersList()
        {
            TriggersList.ItemsSource = _context.Triggers.ToList();
        }

        /// <summary>
        /// Method for adding a <see cref="Keyword"/> to <see cref="Keywords"/>.
        /// </summary>
        /// <param name="sender">The sender of <see cref="AddKeywordBtnClick"/> event.</param>
        /// <param name="e">Arguments from <see cref="AddKeywordBtnClick"/> event.</param>
        private void AddKeywordBtnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Keyword.Text))
            {
                var newKeyword = Keyword.Text;
                ((Trigger)TriggersList.SelectedItem).Keywords = string.IsNullOrEmpty(((Trigger)TriggersList.SelectedItem).Keywords) ? newKeyword :
                    string.Join(",", ((Trigger)TriggersList.SelectedItem).Keywords, newKeyword);
                Keyword.Text = string.Empty;
                Keywords.ItemsSource = ((Trigger)TriggersList.SelectedItem).Keywords.Split(",")
                    .ToList();
            }
        }

        /// <summary>
        /// Method for validating that input value is a number.
        /// </summary>
        /// <param name="sender">The sender of <see cref="CooldownValueLostFocus"/> event.</param>
        /// <param name="e">Arguments from <see cref="CooldownValueLostFocus"/> event.</param>
        private void CooldownValueLostFocus(object sender, RoutedEventArgs e)
        {
            CooldownValue.Text = Regex.Match(CooldownValue.Text, NUMBERONLYREGEX, RegexOptions.IgnoreCase).Success ?
                CooldownValue.Text : "0";
        }

        /// <summary>
        /// Method for decreasing <see cref="CooldownValue"/>.
        /// </summary>
        /// <param name="sender">The sender of <see cref="CooldownValueMinusClick"/> event.</param>
        /// <param name="e">Arguments from <see cref="CooldownValueMinusClick"/> event.</param>
        private void CooldownValueMinusClick(object sender, RoutedEventArgs e)
        {
            var currentValue = ((Trigger)TriggersList.SelectedItem).Cooldown;
            ((Trigger)TriggersList.SelectedItem).Cooldown = currentValue > 0 ? currentValue -= 1 : 0;
            CooldownValue.Text = Convert.ToString(((Trigger)TriggersList.SelectedItem).Cooldown);
        }

        /// <summary>
        /// Method for increasing <see cref="CooldownValue"/>.
        /// </summary>
        /// <param name="sender">The sender of <see cref="CooldownValuePlusClick"/> event.</param>
        /// <param name="e">Arguments from <see cref="CooldownValuePlusClick"/> event.</param>
        private void CooldownValuePlusClick(object sender, RoutedEventArgs e)
        {
            ((Trigger)TriggersList.SelectedItem).Cooldown += 1;
            CooldownValue.Text = Convert.ToString(((Trigger)TriggersList.SelectedItem).Cooldown);
        }

        private void CreateIRCClient(string channelName)
        {
            var ip = Endpoints.Get("IP").Split(':')[0];
            var port = Convert.ToInt32(Endpoints.Get("IP").Split(':')[1]);
            var username = Settings != null ? Settings.Username.ToLower() : string.Empty;
            var oauth = ClientData.Get("OAuth");

            if (!string.IsNullOrWhiteSpace(username))
            {
                IrcClient = new IrcClient(
                    _ircLogger,
                    ip,
                    port,
                    username,
                    oauth,
                    channelName);
            }
        }

        /// <summary>
        /// Method for creating a new <see cref="Trigger"/>.
        /// </summary>
        /// <param name="sender">The sender of <see cref="CreateTrigger"/> event.</param>
        /// <param name="e">Arguments from <see cref="CreateTrigger"/> event.</param>
        private void CreateTrigger(object sender, RoutedEventArgs e)
        {
            var newTrigger = new Trigger()
            {
                Name = $"Trigger {TriggersList.Items.Count + 1}",
                CreatedAt = DateTime.Now,
                Keywords = string.Empty,
                CharAnimTriggerKeyChar = string.Empty,
                CharAnimTriggerKeyValue = 0,
                RewardName = string.Empty,
            };
            _context.Triggers.Add(newTrigger);
            _context.SaveChanges();
            UpdateTriggersList();
        }

        /// <summary>
        /// Method for deleting a selected <see cref="Trigger"/>.
        /// </summary>
        /// <param name="sender">The sender of <see cref="DeleteTrigger"/> event.</param>
        /// <param name="e">Arguments from <see cref="DeleteTrigger"/> event.</param>
        private void DeleteTrigger(object sender, RoutedEventArgs e)
        {
            var selectedTrigger = (Trigger)TriggersList.SelectedItem;
            if (selectedTrigger != null)
            {
                _context.Triggers.Remove(selectedTrigger);
                _context.SaveChanges();
                UpdateTriggersList();
            }
        }

        /// <summary>
        /// Method for validating that input value is a number.
        /// </summary>
        /// <param name="sender">The sender of <see cref="DurationValueLostFocus"/> event.</param>
        /// <param name="e">Arguments from <see cref="DurationValueLostFocus"/> event.</param>
        private void DurationValueLostFocus(object sender, RoutedEventArgs e)
        {
            DurationValue.Text = Regex.Match(DurationValue.Text, NUMBERONLYREGEX, RegexOptions.IgnoreCase).Success ?
                DurationValue.Text : "0";
        }

        /// <summary>
        /// Method for decreasing <see cref="DurationValue"/>.
        /// </summary>
        /// <param name="sender">The sender of <see cref="DurationValueMinusClick"/> event.</param>
        /// <param name="e">Arguments from <see cref="DurationValueMinusClick"/> event.</param>
        private void DurationValueMinusClick(object sender, RoutedEventArgs e)
        {
            var currentValue = ((Trigger)TriggersList.SelectedItem).Duration;
            ((Trigger)TriggersList.SelectedItem).Duration = currentValue > 0 ? currentValue -= 1 : 0;
            DurationValue.Text = Convert.ToString(((Trigger)TriggersList.SelectedItem).Duration);
        }

        /// <summary>
        /// Method for increasing <see cref="DurationValue"/>.
        /// </summary>
        /// <param name="sender">The sender of <see cref="DurationValuePlusClick"/> event.</param>
        /// <param name="e">Arguments from <see cref="DurationValuePlusClick"/> event.</param>
        private void DurationValuePlusClick(object sender, RoutedEventArgs e)
        {
            ((Trigger)TriggersList.SelectedItem).Duration += 1;
            DurationValue.Text = Convert.ToString(((Trigger)TriggersList.SelectedItem).Duration);
        }

        /// <summary>
        /// Method for saving Key Press events.
        /// </summary>
        /// <param name="sender">The sender of <see cref="KeyDown"/> event.</param>
        /// <param name="e">Arguments from <see cref="KeyDown"/> event.</param>
        private new void KeyDown(object sender, KeyEventArgs e)
        {
            if (IsRecordingKey)
            {
                var key = EnumExtensions.ToEnum<KeyCode>(e.Key.ToString());
                ((Trigger)TriggersList.SelectedItem).CharAnimTriggerKeyChar = key.ToString();
                ((Trigger)TriggersList.SelectedItem).CharAnimTriggerKeyValue = (int)key;
                KeyValue.Text = ((Trigger)TriggersList.SelectedItem).CharAnimTriggerKeyChar;
                IsRecordingKey = false;
                RecordKeyBtn.Content = "Record";
            }
        }

        /// <summary>
        /// Method for saving all triggers on close.
        /// </summary>
        /// <param name="sender">The sender of <see cref="MainWindowClosing"/> event.</param>
        /// <param name="e">Arguments from <see cref="MainWindowClosing"/> event.</param>
        private void MainWindowClosing(object sender, CancelEventArgs e)
        {
            foreach (Trigger trigger in TriggersList.ItemsSource)
            {
                _context.Triggers.Update(trigger);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Method for validating that input value is a number.
        /// </summary>
        /// <param name="sender">The sender of <see cref="MaximumBitsValueLostFocus"/> event.</param>
        /// <param name="e">Arguments from <see cref="MaximumBitsValueLostFocus"/> event.</param>
        private void MaximumBitsValueLostFocus(object sender, RoutedEventArgs e)
        {
            MaximumBitsValue.Text = Regex.Match(MaximumBitsValue.Text, NUMBERONLYREGEX, RegexOptions.IgnoreCase).Success ?
                MaximumBitsValue.Text : "0";
        }

        /// <summary>
        /// Method for decreasing <see cref="MaximumBitsValue"/>.
        /// </summary>
        /// <param name="sender">The sender of <see cref="MaximumBitsValueMinusClick"/> event.</param>
        /// <param name="e">Arguments from <see cref="MaximumBitsValueMinusClick"/> event.</param>
        private void MaximumBitsValueMinusClick(object sender, RoutedEventArgs e)
        {
            var currentValue = ((Trigger)TriggersList.SelectedItem).MaximumBits;
            ((Trigger)TriggersList.SelectedItem).MaximumBits = currentValue > 0 ?
                currentValue -= 1 : 0;
            MaximumBitsValue.Text = Convert.ToString(((Trigger)TriggersList.SelectedItem).MaximumBits);
        }

        /// <summary>
        /// Method for increasing <see cref="MaximumBitsValue"/>.
        /// </summary>
        /// <param name="sender">The sender of <see cref="MaximumBitsValuePlusClick"/> event.</param>
        /// <param name="e">Arguments from <see cref="MaximumBitsValuePlusClick"/> event.</param>
        private void MaximumBitsValuePlusClick(object sender, RoutedEventArgs e)
        {
            ((Trigger)TriggersList.SelectedItem).MaximumBits += 1;
            MaximumBitsValue.Text = Convert.ToString(((Trigger)TriggersList.SelectedItem).MaximumBits);
        }

        /// <summary>
        /// Method for validating that input value is a number.
        /// </summary>
        /// <param name="sender">The sender of <see cref="MinimumBitsValueLostFocus"/> event.</param>
        /// <param name="e">Arguments from <see cref="MinimumBitsValueLostFocus"/> event.</param>
        private void MinimumBitsValueLostFocus(object sender, RoutedEventArgs e)
        {
            MinimumBitsValue.Text = Regex.Match(MinimumBitsValue.Text, @"^\d+$", RegexOptions.IgnoreCase).Success ?
                MinimumBitsValue.Text : "0";
        }

        /// <summary>
        /// Method for decreasing <see cref="MinimumBitsValue"/>.
        /// </summary>
        /// <param name="sender">The sender of <see cref="MinimumBitsValueMinusClick"/> event.</param>
        /// <param name="e">Arguments from <see cref="MinimumBitsValueMinusClick"/> event.</param>
        private void MinimumBitsValueMinusClick(object sender, RoutedEventArgs e)
        {
            var currentValue = ((Trigger)TriggersList.SelectedItem).MinimumBits;
            ((Trigger)TriggersList.SelectedItem).MinimumBits = currentValue > 0 ?
                currentValue -= 1 : 0;
            MinimumBitsValue.Text = Convert.ToString(((Trigger)TriggersList.SelectedItem).MinimumBits);
        }

        /// <summary>
        /// Method for increasing <see cref="MinimumBitsValue"/>.
        /// </summary>
        /// <param name="sender">The sender of <see cref="MinimumBitsValuePlusClick"/> event.</param>
        /// <param name="e">Arguments from <see cref="MinimumBitsValuePlusClick"/> event.</param>
        private void MinimumBitsValuePlusClick(object sender, RoutedEventArgs e)
        {
            ((Trigger)TriggersList.SelectedItem).MinimumBits += 1;
            MinimumBitsValue.Text = Convert.ToString(((Trigger)TriggersList.SelectedItem).MinimumBits);
        }

        /// <summary>
        /// Method for opening the Settings Menu.
        /// </summary>
        /// <param name="sender">The sender of <see cref="OpenSettingsMenu"/> event.</param>
        /// <param name="e">Arguments from <see cref="OpenSettingsMenu"/> event.</param>
        private void OpenSettingsMenu(object sender, RoutedEventArgs e)
        {
            if (Application.Current.Windows.OfType<SettingsWindow>().Any())
            {
                Application.Current.Windows.OfType<SettingsWindow>()
                    .First()
                    .Activate();
            }
            else
            {
                SettingsWindow = new SettingsWindow(_context, _loginWindowLogger, _settingsWindowLogger)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                };
                SettingsWindow.Show();
            }
        }

        /// <summary>
        /// Method for Starting recording of Key Press events.
        /// </summary>
        /// <param name="sender">The sender of <see cref="RecordKeyBtnClick"/> event.</param>
        /// <param name="e">Arguments from <see cref="RecordKeyBtnClick"/> event.</param>
        private void RecordKeyBtnClick(object sender, RoutedEventArgs e)
        {
            IsRecordingKey = true;
            RecordKeyBtn.Content = "Recording...";
        }

        /// <summary>
        /// Method for triggering refresh of connection to <see cref="Process"/>.
        /// </summary>
        /// <param name="sender">The sender of <see cref="RefreshConnection"/> event.</param>
        /// <param name="e">Arguments from <see cref="RefreshConnection"/> event.</param>
        private void RefreshConnection(object sender, RoutedEventArgs e)
        {
            RefreshConnectedApplication();
        }

        /// <summary>
        /// Method for removing a <see cref="Keyword"/> from <see cref="Keywords"/>.
        /// </summary>
        /// <param name="sender">The sender of <see cref="RemoveKeywordBtnClick"/> event.</param>
        /// <param name="e">Arguments from <see cref="RemoveKeywordBtnClick"/> event.</param>
        private void RemoveKeywordBtnClick(object sender, RoutedEventArgs e)
        {
            var selectedKeyword = Keywords.SelectedItem;
            if (selectedKeyword != null)
            {
                Keywords.ItemsSource = ((Trigger)TriggersList.SelectedItem).Keywords.Split(",")
                    .Where(i => i != selectedKeyword.ToString())
                    .ToList();
            }
        }

        /// <summary>
        /// Method for resetting <see cref="LastTriggered"/>.
        /// </summary>
        /// <param name="sender">The sender of <see cref="ResetLastTriggeredBtnClick"/> event.</param>
        /// <param name="e">Arguments from <see cref="ResetLastTriggeredBtnClick"/> event.</param>
        private void ResetLastTriggeredBtnClick(object sender, RoutedEventArgs e)
        {
            ((Trigger)TriggersList.SelectedItem).LastTriggered = DateTime.MinValue;
        }

        /// <summary>
        /// Method for saving current <see cref="Trigger"/>.
        /// </summary>
        /// <param name="sender">The sender of <see cref="SaveChangesBtnClick"/> event.</param>
        /// <param name="e">Arguments from <see cref="SaveChangesBtnClick"/> event.</param>
        private void SaveChangesBtnClick(object sender, RoutedEventArgs e)
        {
            var newTrigger = (Trigger)TriggersList.SelectedItem;
            if (newTrigger != null)
            {
                if (_context.Triggers.Any(t => t.Id == newTrigger.Id))
                {
                    _context.Update(newTrigger);
                }
                else
                {
                    _context.Add(newTrigger);
                }

                _context.SaveChanges();
                HasUnsavedChanges.Text = "Changes Saved";
            }
        }

        private void ShowRewardNameHelp(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Enter the case-sensitive Twitch reward name here. When this reward is redeemed, the trigger will be activated.\n\n" +
                "As rewards can be redeemed by anyone who has enough points, permission and cooldown options are disabled. Twitch offers inbuilt cooldown functionality under the reward settings.\n\n" +
                "Keywords can still be entered, in case the reward accepts a text input.",
                "Reward name",
                MessageBoxButton.OK,
                MessageBoxImage.Question);
        }

        private void StartIRCConnection()
        {
            // Ping to the server to make sure the bot stays connected
            PingSender ping = new PingSender(_pingLogger, IrcClient);
            ping.Start();

            var triggers = _context.Triggers.ToList();

            // Listen to the chat
            ChatListener chatListener = new ChatListener(_chatlistenerLogger, _processManagerLogger, Settings, triggers, IrcClient, Settings.Username.ToLower());
            chatListener.Start();
            chatListener.StartLogging();
        }

        /// <summary>
        /// Method for testing <see cref="Trigger"/>.
        /// </summary>
        /// <param name="sender">The sender of <see cref="TestTrigger"/> event.</param>
        /// <param name="e">Arguments from <see cref="TestTrigger"/> event.</param>
        private void TestTrigger(object sender, RoutedEventArgs e)
        {
            var trigger = ((FrameworkElement)sender).DataContext as Trigger;
            if (!string.IsNullOrWhiteSpace(trigger.CharAnimTriggerKeyChar))
            {
                trigger.LastTriggered = DateTime.Now;

                // add event for activation to queue
                _context.EventQueue.Add(new QueuedEvent()
                {
                    TriggeredAt = trigger.LastTriggered,
                    TriggerId = trigger.Id,
                });
                _mainWindowlogger.LogInformation($"Event {(_context.SaveChanges() > 0 ? "added successfully" : "adding failed")}");
            }
            else
            {
                _mainWindowlogger.LogInformation("Trigger Char is unset");
            }
        }

        /// <summary>
        /// Method for Updating <see cref="KeyValue"/>.
        /// </summary>
        /// <param name="sender">The sender of <see cref="TriggersListSelectionChanged"/> event.</param>
        /// <param name="e">Arguments from <see cref="TriggersListSelectionChanged"/> event.</param>
        private void TriggersListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            KeyValue.Text = (Trigger)TriggersList.SelectedItem != null ? ((Trigger)TriggersList.SelectedItem).CharAnimTriggerKeyChar : string.Empty;
        }
    }
}
