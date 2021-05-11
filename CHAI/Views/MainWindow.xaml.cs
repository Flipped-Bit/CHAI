using CHAI.Data;
using CHAI.Extensions;
using CHAI.Models.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            var window = GetWindow(this);
            window.KeyDown += KeyDown;
            Closing += MainWindowClosing;
            _logger.LogInformation("Main window initialised successfully");
        }

        /// <summary>
        /// Gets or sets a value indicating whether Key presses are being recorded.
        /// </summary>
        private bool IsRecordingKey { get; set; }

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
                ((Trigger)TriggersList.SelectedItem).Keywords = string.Join(",", ((Trigger)TriggersList.SelectedItem).Keywords, newKeyword);
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
                Keywords = string.Empty,
                CharAnimTriggerKeyChar = string.Empty,
                CharAnimTriggerKeyValue = 0,
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

        /// <summary>
        /// Method for Updating <see cref="KeyValue"/>.
        /// </summary>
        /// <param name="sender">The sender of <see cref="TriggersListSelectionChanged"/> event.</param>
        /// <param name="e">Arguments from <see cref="TriggersListSelectionChanged"/> event.</param>
        private void TriggersListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            KeyValue.Text = (Trigger)TriggersList.SelectedItem != null ? ((Trigger)TriggersList.SelectedItem).CharAnimTriggerKeyChar : string.Empty;
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
