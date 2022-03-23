using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using RevoScada.DesktopApplication.Models.ModelEnums;
using RevoScada.DesktopApplication.Reports;
using RevoScada.DesktopApplication.ViewModels;
using RevoScada.DesktopApplication.ViewModels.CalibrationViewModels;
using RevoScada.DesktopApplication.Views.ReportTemplates;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for Calibration.xaml
    /// </summary>
    public partial class CalibrationType4 : UserControl
    {
        #region Fields
        private CalibrationType4VM _viewModel;
        private TextEdit _senderTextBox;
        private SimpleButton _senderButton;
        private bool _setCommandRunninng = false;
        #endregion

        public CalibrationType4()
        {
            InitializeComponent();
        }
        void BeforSetStyle()
        {

            _senderTextBox.Background = (SolidColorBrush)Application.Current.Resources["LoadingYellowColor"];
            _senderTextBox.IsEnabled = false;
            _senderButton.IsEnabled = false;
        }
        void AfterSetStyle()
        {
            _senderTextBox.IsEnabled = true;
            _senderButton.IsEnabled = true;
            _senderTextBox.Background = Brushes.White;
        }

        /// <summary>
        /// It'll allow dataGridGroupScrollViewer to use scrollbar functionality by using mouse wheel.
        /// Main purpose is, pass the datagrid values to the scrollviewer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GeneralDatagrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            dataGridGroupScrollViewer.ScrollToVerticalOffset(dataGridGroupScrollViewer.VerticalOffset - e.Delta / 3);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation(0, 1,
                            (Duration)TimeSpan.FromSeconds(1));
            this.BeginAnimation(UIElement.OpacityProperty, animation);


            textBlockRangeSlider.Visibility = Visibility.Collapsed;
            borderRangeSlider.Visibility = Visibility.Collapsed;

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as CalibrationType4VM;
        }
 
        private void sensorTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.CalibrationFormInput.SelectedCalibrationSensorType = (CalibrationSensorType)sensorTypeComboBox.SelectedValue;
            txtSetSourceToLowSensorRange.Text = "0";
            txtSetSourceToHighSensorRange.Text = "0";

            // Save selected sensor type value to App.Settings
            _viewModel.CalibrationSettings.SensorType = _viewModel.CalibrationFormInput.SelectedCalibrationSensorType;
            _viewModel.CalibrationSettingsSetter = _viewModel.CalibrationSettings;

            //_viewModel.CalibrationFormInput.SequenceOfSensorRangeEndSelection = 3;
            //_viewModel.CalibrationFormInput.SequenceOfSensorRangeStartSelection = 1;

            switch (_viewModel.CalibrationFormInput.SelectedCalibrationSensorType)
            {
                case CalibrationSensorType.PTC:

                    _viewModel.CalibrationFormInput.DefaultSequenceOfSensorMin = _viewModel.DefaultSequenceOfSensorMinPTC;
                    _viewModel.CalibrationFormInput.DefaultSequenceOfSensorMax = _viewModel.DefaultSequenceOfSensorMaxPTC;

                    if (_viewModel.CalibrationSettings.LastSelectedSequenceOfSensorMinPTC != 0)
                        _viewModel.CalibrationFormInput.SequenceOfSensorRangeStartSelection = _viewModel.CalibrationSettings.LastSelectedSequenceOfSensorMinPTC;
                    else
                        _viewModel.CalibrationFormInput.SequenceOfSensorRangeStartSelection = _viewModel.DefaultSequenceOfSensorMinPTC;

                    if (_viewModel.CalibrationSettings.LastSelectedSequenceOfSensorMaxPTC != 0)
                        _viewModel.CalibrationFormInput.SequenceOfSensorRangeEndSelection = _viewModel.CalibrationSettings.LastSelectedSequenceOfSensorMaxPTC;
                    else
                        _viewModel.CalibrationFormInput.SequenceOfSensorRangeEndSelection = _viewModel.DefaultSequenceOfSensorMaxPTC;

                    twoThumbSlider.IsEnabled = true;
                    textBlockRangeSlider.Visibility = Visibility.Visible;
                    borderRangeSlider.Visibility = Visibility.Visible;

                    break;
                case CalibrationSensorType.MON:

                    _viewModel.CalibrationFormInput.DefaultSequenceOfSensorMin = _viewModel.DefaultSequenceOfSensorMinMON;
                    _viewModel.CalibrationFormInput.DefaultSequenceOfSensorMax = _viewModel.DefaultSequenceOfSensorMaxMON;

                    if (_viewModel.CalibrationSettings.LastSelectedSequenceOfSensorMinMON != 0)
                        _viewModel.CalibrationFormInput.SequenceOfSensorRangeStartSelection = _viewModel.CalibrationSettings.LastSelectedSequenceOfSensorMinMON;
                    else
                        _viewModel.CalibrationFormInput.SequenceOfSensorRangeStartSelection = _viewModel.DefaultSequenceOfSensorMinMON;

                    if (_viewModel.CalibrationSettings.LastSelectedSequenceOfSensorMaxMON != 0)
                        _viewModel.CalibrationFormInput.SequenceOfSensorRangeEndSelection = _viewModel.CalibrationSettings.LastSelectedSequenceOfSensorMaxMON;
                    else
                        _viewModel.CalibrationFormInput.SequenceOfSensorRangeEndSelection = _viewModel.DefaultSequenceOfSensorMaxMON;

                    textBlockRangeSlider.Visibility = Visibility.Visible;
                    borderRangeSlider.Visibility = Visibility.Visible;
                    break;
                case CalibrationSensorType.VACHeaderMonitor:
                case CalibrationSensorType.AIRTCHigh:
                case CalibrationSensorType.AIRTCMediumHigh:
                case CalibrationSensorType.AIRTCLow:
                case CalibrationSensorType.Pressure:
                case CalibrationSensorType.VacuumHeaderRight:
                case CalibrationSensorType.VacuumHeaderLeft:
                case CalibrationSensorType.AIRTCMediumLow:
                    textBlockRangeSlider.Visibility = Visibility.Collapsed;
                    borderRangeSlider.Visibility = Visibility.Collapsed;

                    break;
            }

            _viewModel.SetSensorType();
        }

        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            CalibrationSensorType calibrationSensorType = (CalibrationSensorType)sensorTypeComboBox.SelectedValue;

            switch (calibrationSensorType)
            {
                case CalibrationSensorType.PTC:

                    _viewModel.CalibrationFormInput.SequenceOfSensorRangeStartSelection = Convert.ToInt16(twoThumbSlider.Start);
                    _viewModel.CalibrationFormInput.SequenceOfSensorRangeEndSelection = Convert.ToInt16(twoThumbSlider.End);
                    _viewModel.SequenceOfSensorRangeStartSelectionPTC = Convert.ToInt16(twoThumbSlider.Start);
                    _viewModel.SequenceOfSensorRangeEndSelectionPTC = Convert.ToInt16(twoThumbSlider.End);

                    _viewModel.InitSequenceOfSensorPTC = true;

                    break;
                case CalibrationSensorType.MON:

                    _viewModel.CalibrationFormInput.SequenceOfSensorRangeStartSelection = Convert.ToInt16(twoThumbSlider.Start);
                    _viewModel.CalibrationFormInput.SequenceOfSensorRangeEndSelection = Convert.ToInt16(twoThumbSlider.End);
                    _viewModel.SequenceOfSensorRangeStartSelectionMON = Convert.ToInt16(twoThumbSlider.Start);
                    _viewModel.SequenceOfSensorRangeEndSelectionMON = Convert.ToInt16(twoThumbSlider.End);

                    _viewModel.InitSequenceOfSensorMON = true;

                    break;
                case CalibrationSensorType.VACHeaderMonitor:
                    break;
                case CalibrationSensorType.AIRTCHigh:
                    break;
                case CalibrationSensorType.AIRTCMediumHigh:
                    break;
                case CalibrationSensorType.AIRTCLow:
                    break;
                case CalibrationSensorType.Pressure:
                    break;
                case CalibrationSensorType.VacuumHeaderRight:
                    break;
                case CalibrationSensorType.VacuumHeaderLeft:
                    break;
                case CalibrationSensorType.AIRTCMediumLow:
                    break;
                default:
                    break;
            }
           
        }

        private async void btnSetSourceToHighSensorRange_Click(object sender, RoutedEventArgs e)
        {
            if (_setCommandRunninng == true)
                return;

            _senderTextBox = txtSetSourceToHighSensorRange;
            _senderButton = (SimpleButton)sender;
            BeforSetStyle();
            _setCommandRunninng = true;
            await _viewModel.SetSourcetoHighSensorRange();
            _setCommandRunninng = false;
            AfterSetStyle();
        }

        private async void btnSetSourceToLowSensorRange_Click(object sender, RoutedEventArgs e)
        {
            if (_setCommandRunninng == true)
                return;

            _senderTextBox = txtSetSourceToLowSensorRange;
            _senderButton = (SimpleButton)sender;
            BeforSetStyle();
            _setCommandRunninng = true;
            await _viewModel.SetSourcetoLowSensorRange();
            _setCommandRunninng = false;
            AfterSetStyle();
        }

        private async void btnSetSequenceOfSensorRangeSelection_Click(object sender, RoutedEventArgs e)
        {
            txtSetSourceToHighSensorRange.IsEnabled = false;
            txtSetSourceToLowSensorRange.IsEnabled = false;
            btnSetSourceToHighSensorRange.IsEnabled = false;
            btnSetSourceToLowSensorRange.IsEnabled = false;
            btnSetSequenceOfSensorRangeSelection.IsEnabled = false;
            btnCalculate.IsEnabled = false;

            _viewModel.CalibrationFormInput.SequenceOfSensorRangeStartSelection = Convert.ToInt16(twoThumbSlider.Start);
            _viewModel.CalibrationFormInput.SequenceOfSensorRangeEndSelection = Convert.ToInt16(twoThumbSlider.End);

            await _viewModel.SetSequenceOfSensorRangeSelection();

            txtSetSourceToHighSensorRange.IsEnabled = true;
            txtSetSourceToLowSensorRange.IsEnabled = true;
            btnSetSourceToHighSensorRange.IsEnabled = true;
            btnSetSourceToLowSensorRange.IsEnabled = true;
            btnSetSequenceOfSensorRangeSelection.IsEnabled = true;
            btnCalculate.IsEnabled = true;
        }

        private void sensorTypeComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel.CalibrationSettings.SensorType == null)
                return;

            sensorTypeComboBox.SelectedValue = _viewModel.CalibrationSettings.SensorType;
        }

        private void twoThumbSlider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            switch (_viewModel.CalibrationFormInput.SelectedCalibrationSensorType)
            {
                case CalibrationSensorType.PTC:
                    _viewModel.CalibrationSettings.LastSelectedSequenceOfSensorMinPTC = Convert.ToInt16(twoThumbSlider.Start);
                    _viewModel.CalibrationSettings.LastSelectedSequenceOfSensorMaxPTC = Convert.ToInt16(twoThumbSlider.End);
                    _viewModel.CalibrationSettingsSetter = _viewModel.CalibrationSettings;
                    break;
                case CalibrationSensorType.MON:
                    _viewModel.CalibrationSettings.LastSelectedSequenceOfSensorMinMON = Convert.ToInt16(twoThumbSlider.Start);
                    _viewModel.CalibrationSettings.LastSelectedSequenceOfSensorMaxMON = Convert.ToInt16(twoThumbSlider.End);
                    _viewModel.CalibrationSettingsSetter = _viewModel.CalibrationSettings;
                    break;
            }
        }

        private void twoThumbSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            twoThumbSlider_DragCompleted(sender, null);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _viewModel.SetCalibrationDatablock(false);
        }
    }
}
