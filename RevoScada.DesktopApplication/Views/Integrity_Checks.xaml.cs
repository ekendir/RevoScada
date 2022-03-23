using DevExpress.Xpf.WindowsUI;
using RevoScada.DesktopApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RevoScada.DesktopApplication.Views
{
    /// <summary>
    /// Interaction logic for Integrity_Checks.xaml
    /// </summary>
    public partial class Integrity_Checks : UserControl
    {
        #region Fields
        private IntegrityChecksVM _viewModel;
        private DispatcherTimer _timer;
        public volatile bool IsControlsEditingMode = false;
        #endregion

        public Integrity_Checks()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as IntegrityChecksVM;
            _viewModel.Integrity_Checks_View = this;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(0, 1, (Duration)TimeSpan.FromSeconds(1));
            this.BeginAnimation(UIElement.OpacityProperty, animation);

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!IsControlsEditingMode)
            {
                _viewModel.ContinuousUpdate();
            }
        }

        private void activeItemsRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
                _viewModel.FilterIntegrityCheckItems(true);
        }

        private void allItemsRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
                _viewModel.FilterIntegrityCheckItems(false);
        }

        private async void testValTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
                return;

            IsControlsEditingMode = true;

            if (e.Key == Key.Enter)
            {
                TextBox txtBox = (TextBox)sender;

                var txtBoxBgColor = txtBox.Background;
                SolidColorBrush loadingYellowColor = (SolidColorBrush)Application.Current.Resources["LoadingYellowColor"];
                txtBox.Background = loadingYellowColor;

                // Set to PLC
                bool plcResult = await _viewModel.SetFailureCriteriaTestToPlc();

                txtBox.Background = txtBoxBgColor;

                if (plcResult == false)
                    WinUIMessageBox.Show("PLC'ye setleme işlemi başarısız oldu. Lütfen servislerinizi kontrol edip tekrar deneyiniz.", "Başarısız",
                                         MessageBoxButton.OK, MessageBoxImage.Warning);

                integrityCheckTable.Focus();
                IsControlsEditingMode = false;
            }

            if (e.Key == Key.Escape)
            {
                integrityCheckTable.Focus();
                IsControlsEditingMode = false;
            }
        }

        private async void manualTimeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
                return;

            IsControlsEditingMode = true;

            if (e.Key == Key.Enter)
            {
                TextBox txtBox = (TextBox)sender;

                if (string.IsNullOrEmpty(txtBox.Text))
                    txtBox.Text = "0";

                int intValue = int.Parse(txtBox.Text);

                // Check value
                if (intValue > _viewModel.IntegrityCheckMaxTimeValue)
                {
                    WinUIMessageBox.Show($"Manual Time {_viewModel.IntegrityCheckMaxTimeValue}'dan fazla girilemez!", "Manual Time Limiti", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    txtBox.Text = "0";
                    return;
                }

                var txtBoxBgColor = txtBox.Background;
                SolidColorBrush loadingYellowColor = (SolidColorBrush)Application.Current.Resources["LoadingYellowColor"];
                txtBox.Background = loadingYellowColor;

                SetPresetTimeRadioButtonsToFalse();

                // Set to PLC
                bool plcResult = await _viewModel.SetFailureCriteriaSetTimeValueToPlc();

                txtBox.Background = txtBoxBgColor;

                if (plcResult == false)
                    WinUIMessageBox.Show("PLC'ye setleme işlemi başarısız oldu. Lütfen servislerinizi kontrol edip tekrar deneyiniz.", "Başarısız",
                        MessageBoxButton.OK, MessageBoxImage.Warning);

                integrityCheckTable.Focus();
                IsControlsEditingMode = false;
            }

            if (e.Key == Key.Escape)
            {
                integrityCheckTable.Focus();
                IsControlsEditingMode = false;
            }
        }

        /// <summary>
        /// Set preset time radio buttons to either checked or unchecked by value.
        /// </summary>
        /// <param name="thisTextBox"></param>
        private void UpdatePresetTimeRadioButtons(TextBox thisTextBox)
        {
            if (thisTextBox.Text != "1")
                preset1MRadioBtn.IsChecked = false;
            else
                preset1MRadioBtn.IsChecked = true;

            if (thisTextBox.Text != "5")
                preset5MRadioBtn.IsChecked = false;
            else
                preset5MRadioBtn.IsChecked = true;

            if (thisTextBox.Text != "10")
                preset10MRadioBtn.IsChecked = false;
            else
                preset10MRadioBtn.IsChecked = true;

            if (thisTextBox.Text != "15")
                preset15MRadioBtn.IsChecked = false;
            else
                preset15MRadioBtn.IsChecked = true;
        }

        public void SetPresetTimeRadioButtonsToFalse()
        {
            preset1MRadioBtn.IsChecked = false;
            preset5MRadioBtn.IsChecked = false;
            preset10MRadioBtn.IsChecked = false;
            preset15MRadioBtn.IsChecked = false;
        }

        private async void SetPresetTimeValueToPlc(int value)
        {
            var txtBoxBgColor = manualTimeTextBox.Background;
            SolidColorBrush loadingYellowColor = (SolidColorBrush)Application.Current.Resources["LoadingYellowColor"];
            manualTimeTextBox.Background = loadingYellowColor;

            _viewModel.LeakageTestFailureCriteriaSetTimeValue = value;
            manualTimeTextBox.Text = value.ToString();

            // Set to PLC
            bool plcResult = await _viewModel.SetFailureCriteriaSetTimeValueToPlc();

            manualTimeTextBox.Background = txtBoxBgColor;

            if (plcResult == false)
                WinUIMessageBox.Show("PLC'ye setleme işlemi başarısız oldu. Lütfen servislerinizi kontrol edip tekrar deneyiniz.", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void preset1MRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            SetPresetTimeValueToPlc(1);
        }

        private void preset5MRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            SetPresetTimeValueToPlc(5);
        }

        private void preset10MRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            SetPresetTimeValueToPlc(10);
        }

        private void preset15MRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            SetPresetTimeValueToPlc(15);
        }

        private void testValTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var change = e.Changes.FirstOrDefault();

            TextBox txtBox = (TextBox)sender;

            //Regex regex = new Regex(@"^[0-9]{0,4}$");
            Regex regex = new Regex(@"^[,.0-9-]{0,15}$");

            // Step 2: call Match on Regex instance.
            Match match = regex.Match(txtBox.Text);

            // Step 3: test for Success.
            if (!match.Success)
            {
                txtBox.Text = txtBox.Text.Remove(change.Offset, change.AddedLength);
            }

            if (txtBox.Text.Contains(','))
            {
                txtBox.Text = txtBox.Text.Replace(',', '.');
            }
        }

        private void manualTimeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var change = e.Changes.FirstOrDefault();

            TextBox txtBox = (TextBox)sender;

            Regex regex = new Regex(@"^[0-9]{1,3}$");

            // Step 2: call Match on Regex instance.
            Match match = regex.Match(txtBox.Text);

            // Step 3: test for Success.
            if (!match.Success)
            {
                txtBox.Text = txtBox.Text.Remove(change.Offset, change.AddedLength);
            }
        }

        private void showPtcBtn_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.IsPTCPortsGridVisible = true;
            _viewModel.IsShowPTCPortsBtnVisible = false;
            _viewModel.IsHidePTCPortsBtnVisible = true;
        }

        private void hidePtcBtn_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.IsPTCPortsGridVisible = false;
            _viewModel.IsHidePTCPortsBtnVisible = false;
            _viewModel.IsShowPTCPortsBtnVisible = true;
        }

    
    }
}
