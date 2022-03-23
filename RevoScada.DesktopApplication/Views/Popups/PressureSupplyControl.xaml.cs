using DevExpress.Xpf.Editors;
using DevExpress.Xpf.WindowsUI;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.ViewModels;
using RevoScada.Entities.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RevoScada.DesktopApplication.Views.Popups
{
    /// <summary>
    /// Interaction logic for PNI_Full_Screen_Set_Alarm.xaml
    /// </summary>
    public partial class PressureSupplyControl : Window, INotifyPropertyChanged
    {

        public PipingAndInstrumentationVM _viewModel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            property = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _isControlsEditingMode;
        private float _getPandIValue;

        public float GetPandIValue
        {
            get => _getPandIValue;
            set => OnPropertyChanged(ref _getPandIValue, value);
        }

        private float _getAutoValue;

        public float GetAutoValue
        {
            get => _getAutoValue;
            set => OnPropertyChanged(ref _getAutoValue, value);
        }

        private float _getManValue;

        public float GetManValue
        {
            get => _getManValue;
            set => OnPropertyChanged(ref _getManValue, value);
        }

        private float _getCompressorValue;

        public float GetCompressorValue
        {
            get => _getCompressorValue;
            set => OnPropertyChanged(ref _getCompressorValue, value);
        }

        private float _getBoosterValue;

        public float GetBoosterValue
        {
            get => _getBoosterValue;
            set => OnPropertyChanged(ref _getBoosterValue, value);
        }


        public PressureSupplyControl(PipingAndInstrumentationVM pipingAndInstrumentationVM, int getRadioButtonAutoManValue, int getRadioButtonOnOffValue,float getTextEditValue)
        {
            InitializeComponent();
            DataContext = this;
            GetPandIValue = getTextEditValue;

            if (getRadioButtonAutoManValue == 1)
            {
                GetAutoValue = 1;
                GetManValue = 0;
            }
            else
            {
                GetAutoValue = 0;
                GetManValue = 1;
            }

            if (getRadioButtonOnOffValue == 1)
            {
                GetCompressorValue = 0;
                GetBoosterValue = 1;
            }
            else
            {
                GetCompressorValue = 1;
                GetBoosterValue = 0;
            }

            _viewModel = pipingAndInstrumentationVM;

        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void RbMan_Click(object sender, RoutedEventArgs e)
        {
            _isControlsEditingMode = true;
            RadioButton radioButton = (RadioButton)sender;

           bool plcResult = await _viewModel.SetToPlc(0, radioButton.Name,"Manual");

            if (plcResult == false)
                WinUIMessageBox.Show("Check services! (Lütfen servislerin çalıştığından emin olun!)", "Error!", MessageBoxButton.OK, MessageBoxImage.Warning);

            _isControlsEditingMode = false;
        }

        private async void RbAuto_Click(object sender, RoutedEventArgs e)
        {
            _isControlsEditingMode = true;
            RadioButton radioButton = (RadioButton)sender;

            bool plcResult = await _viewModel.SetToPlc(1,radioButton.Name,"Auto");

            if (plcResult == false)
                WinUIMessageBox.Show("Check services! (Lütfen servislerin çalıştığından emin olun!)", "Error!", MessageBoxButton.OK, MessageBoxImage.Warning);

            _isControlsEditingMode = false;

        }

        private async void RbCompressor_Click(object sender, RoutedEventArgs e)
        {
            _isControlsEditingMode = true;
            RadioButton radioButton = (RadioButton)sender;

            bool plcResult = await _viewModel.SetToPlc(0, radioButton.Name,"Compressor On");

            if (plcResult == false)
                WinUIMessageBox.Show("Check services! (Lütfen servislerin çalıştığından emin olun!)", "Error!", MessageBoxButton.OK, MessageBoxImage.Warning);

            _isControlsEditingMode = false;
        }

        private async void RbBooster_Click(object sender, RoutedEventArgs e)
        {
            _isControlsEditingMode = true;
            RadioButton radioButton = (RadioButton)sender;

            bool plcResult = await _viewModel.SetToPlc(1, radioButton.Name,"Booster On");

            if (plcResult == false)
                WinUIMessageBox.Show("Check services! (Lütfen servislerin çalıştığından emin olun!)", "Error!", MessageBoxButton.OK, MessageBoxImage.Warning);

            _isControlsEditingMode = false;
        }

        private void valueBox_GotFocus(object sender, RoutedEventArgs e)
        {
            _isControlsEditingMode = true;
        }

        private async void valueBox_KeyDown(object sender, KeyEventArgs e)
        {
            _isControlsEditingMode = true;


            if (e.Key == Key.Decimal)
            {
                e.Handled = true;
            }

            if (e.Key == Key.Tab)
            {
                if (_viewModel.WaitIndicatorControl.IsWaitIndicatorVisible)
                    e.Handled = true;
                else
                    _isControlsEditingMode = false;

                return;
            }

            if (e.Key == Key.Enter)
            {
                TextEdit txtBox = (TextEdit)sender;
                var txtBoxBgColor = txtBox.Background;
                SolidColorBrush loadingYellowColor = (SolidColorBrush)Application.Current.Resources["LoadingYellowColor"];
                txtBox.Background = loadingYellowColor;

                //BindingExpression bindingExpression = txtBox.GetBindingExpression(TextEdit.TextProperty);
                //Binding parentBinding = bindingExpression.ParentBinding;

                bool plcResult = await _viewModel.SetToPlc(txtBox.Text, "", txtBox.Text);
                txtBox.Background = txtBoxBgColor;

                if (plcResult == false)
                    WinUIMessageBox.Show("Check services! (Lütfen servislerin çalıştığından emin olun!)", "Error!", MessageBoxButton.OK, MessageBoxImage.Warning);

                _isControlsEditingMode = false;
                // FocusableGrid.Focus();
            }

            if (e.Key == Key.Escape)
            {
                _isControlsEditingMode = false;
            }
        }

        private void valueBox_LostFocus(object sender, RoutedEventArgs e)
        {
            _isControlsEditingMode = false;
        }
    }
}
