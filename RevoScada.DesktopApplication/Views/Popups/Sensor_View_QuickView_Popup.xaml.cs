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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RevoScada.DesktopApplication.Views.Popups
{
    /// <summary>
    /// Interaction logic for Sensor_View_QuickView_Popup.xaml
    /// </summary>
    public partial class Sensor_View_QuickView_Popup : Window
    {
        #region Fields
        private SensorViewItemsTableRow _sensorViewItemsTable;
        private SensorViewVM _sensorViewVM;
        private SiemensTagConfiguration _enableDisableCommandTagConfig;
        private int _buttonValue;
        private int _portNumIndex;
        private string _portType;
        #endregion

        #region Properties
        public string PortName { get; set; }
        public string PortValue { get; set; }
        public string RateValue { get; set; }
        #endregion

        public Sensor_View_QuickView_Popup(SensorViewItemsTableRow sensorViewItemsTable, SensorViewVM sensorViewVM,
                                           SiemensTagConfiguration enableDisableCommandTagConfig, int buttonValue, int portNumIndex,
                                           string portType, string rateValue)
        {
            InitializeComponent();
            DataContext = this;

            _sensorViewItemsTable = sensorViewItemsTable;
            _sensorViewVM = sensorViewVM;
            _enableDisableCommandTagConfig = enableDisableCommandTagConfig;
            _buttonValue = buttonValue;
            _portNumIndex = portNumIndex;
            _portType = portType;
            if (_portType == "PTC")
                Title = "Part Temperature Details";
            else
                Title = "Part Vacuum Details";

            RateValue = rateValue;
            GetDetails();
        }

        private void GetDetails()
        {
            PortName = _sensorViewItemsTable.PortName;

            if (string.IsNullOrEmpty(_sensorViewItemsTable.BagName))
                PortValue = String.Format("{0:F2}", _sensorViewItemsTable.PortValue);
            else
                PortValue = String.Format("{0:F2} / {1}", _sensorViewItemsTable.PortValue, _sensorViewItemsTable.BagName);

            // Set enabled, disabled buttons
            switch (_buttonValue)
            {
                case 1: // enter parts enable
                    enabledRadioBtn.IsChecked = true;
                    break;
                case 2: // operator disable
                    disabledRadioBtn.IsChecked = true;
                    break;
                case 3: // oscillation disable
                    disabledRadioBtn.IsChecked = true;
                    break;
                case 4: // operator enable
                    enabledRadioBtn.IsChecked = true;
                    break;
                default:
                    break;
            }
        }

        private async void enabledRadioBtn_Click(object sender, RoutedEventArgs e)
        {
            bool plcResult = await _sensorViewVM.SetEnableDisableToPLC(_enableDisableCommandTagConfig, true, _portNumIndex, _portType);
            this.Close();
            if (!plcResult)
            {
                WinUIMessageBox.Show(string.Format("{0} {1} portu PLC'ye setlenemedi. Lütfen tekrar deneyiniz.", _portType, _portNumIndex + 1),
                "Başarısız", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void disabledRadioBtn_Click(object sender, RoutedEventArgs e)
        {
            bool plcResult = await _sensorViewVM.SetEnableDisableToPLC(_enableDisableCommandTagConfig, false, _portNumIndex, _portType);
            this.Close();
            if (!plcResult)
            {
                WinUIMessageBox.Show(string.Format("{0} {1} portu PLC'ye setlenemedi. Lütfen tekrar deneyiniz.", _portType, _portNumIndex + 1),
                "Başarısız", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
