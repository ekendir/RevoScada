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
    public partial class ValveControlOnOff : Window, INotifyPropertyChanged
    {

        public PipingAndInstrumentationVM _viewModel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            property = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _isControlsEditingMode;

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

        private float _getOnValue;

        public float GetOnValue
        {
            get => _getOnValue;
            set => OnPropertyChanged(ref _getOnValue, value);
        }

        private float _getOffValue;

        public float GetOffValue
        {
            get => _getOffValue;
            set => OnPropertyChanged(ref _getOffValue, value);
        }


        public ValveControlOnOff(PipingAndInstrumentationVM pipingAndInstrumentationVM, int getRadioButtonAutoManValue, int getRadioButtonOnOffValue)
        {
            InitializeComponent();
            DataContext = this;

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
                GetOnValue = 1;
                GetOffValue = 0;
            }
            else
            {
                GetOnValue = 0;
                GetOffValue = 1;
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
        private async void RbOn_Click(object sender, RoutedEventArgs e)
        {
            _isControlsEditingMode = true;
            RadioButton radioButton = (RadioButton)sender;

            bool plcResult = await _viewModel.SetToPlc(1, radioButton.Name,"On");

            if (plcResult == false)
                WinUIMessageBox.Show("Check services! (Lütfen servislerin çalıştığından emin olun!)", "Error!", MessageBoxButton.OK, MessageBoxImage.Warning);

            _isControlsEditingMode = false;
        }

        private async void RbOff_Click(object sender, RoutedEventArgs e)
        {
            _isControlsEditingMode = true;
            RadioButton radioButton = (RadioButton)sender;

            bool plcResult = await _viewModel.SetToPlc(0, radioButton.Name,"Off");

            if (plcResult == false)
                WinUIMessageBox.Show("Check services! (Lütfen servislerin çalıştığından emin olun!)", "Error!", MessageBoxButton.OK, MessageBoxImage.Warning);

            _isControlsEditingMode = false;
        }

    }
}
