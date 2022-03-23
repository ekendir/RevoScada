using DevExpress.Xpf.WindowsUI;
using RevoScada.ProcessController;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Models;
using RevoScada.DesktopApplication.Models.ModelEnums;
using RevoScada.DesktopApplication.ViewModels;
using RevoScada.Entities.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RevoScada.DesktopApplication.Views.ItemViews
{
    /// <summary>
    /// Interaction logic for VacuumControlSection.xaml
    /// </summary>
    public partial class VacuumControlSectionType2 : UserControl
    {
        #region Fields
        private VacuumLinesVM _viewModel;
        private Vacuum_Lines _vacuumLinesView;
        private PlcCommandManager _plcCommandManager;
        #endregion

        public VacuumControlSectionType2()
        {
            InitializeComponent();

            _plcCommandManager = new PlcCommandManager(ApplicationConfigurations.Instance.Configuration.RedisServer);
        }

        private void flowLayout_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as VacuumLinesVM;
            _vacuumLinesView = _viewModel.Vacuum_Lines;
        }

        #region Vacuum Pump Control State Controls
        private async void vacPumpAuto_Clicked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            int tagVal = (int)radioButton.Tag;

            // Set value to PLC
            if (_viewModel == null)
                return;

            int setValue = 1;
            bool plcResult = await _viewModel.SetToPlc(setValue, null, "vacuumPumpControlStateAuto");

            if (plcResult == false)
                WinUIMessageBox.Show("Vacuum Pump Auto komutu PLC'ye setlenemedi. Lütfen tekrar deneyiniz.", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void vacPumpMan_Clicked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            int tagVal = (int)radioButton.Tag;

            // Set value to PLC
            if (_viewModel == null)
                return;

            int setValue = 0;
            bool plcResult = await _viewModel.SetToPlc(setValue, null, "vacuumPumpControlStateAuto");

            if (plcResult == false)
                WinUIMessageBox.Show("Vacuum Pump Manuel komutu PLC'ye setlenemedi. Lütfen tekrar deneyiniz.", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void vacPumpAuto_Loaded(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            int tagVal = (int)radioButton.Tag;
            if (tagVal == 1)
                radioButton.IsChecked = true;
        }

        private void vacPumpAuto_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Tag == null)
                return;

            int tagVal = (int)radioButton.Tag;
            if (tagVal == 1)
                radioButton.IsChecked = true;
        }

        private void vacPumpMan_Loaded(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Tag == null)
                return;
            
            int tagVal = (int)radioButton.Tag;
            if (tagVal == 0)
                radioButton.IsChecked = true;
        }

        private void vacPumpMan_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Tag == null)
                return;

            int tagVal = (int)radioButton.Tag;
            if (tagVal == 0)
                radioButton.IsChecked = true;
        }
        #endregion

        #region Vacuum Pump State On/Off Controls

        private async void vacPumpOn_Clicked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            int tagVal = (int)radioButton.Tag;

            // Set value to PLC
            if (_viewModel == null)
                return;

            int setValue = 1;
            bool plcResult = await _viewModel.SetToPlc(setValue, null, "vacuumPumpControlStateOnOff");

            if (plcResult == false)
                WinUIMessageBox.Show("Vakum Pump State On konumuna alınıp PLC'ye setlenemedi. Lütfen tekrar deneyiniz.", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void vacPumpOff_Clicked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            int tagVal = (int)radioButton.Tag;

            // Set value to PLC
            if (_viewModel == null)
                return;

            int setValue = 0;
            bool plcResult = await _viewModel.SetToPlc(setValue, null, "vacuumPumpControlStateOnOff");

            if (plcResult == false)
                WinUIMessageBox.Show("Vakum Pump State Off konumuna alınıp PLC'ye setlenemedi. Lütfen tekrar deneyiniz.", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void vacPumpOn_Loaded(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            int tagVal = (int)radioButton.Tag;
            if (tagVal == 1)
                radioButton.IsChecked = true;
        }

        private void vacPumpOn_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Tag == null)
                return;

            int tagVal = (int)radioButton.Tag;
            if (tagVal == 1)
                radioButton.IsChecked = true;
        }

        private void vacPumpOff_Loaded(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Tag == null)
                return;

            int tagVal = (int)radioButton.Tag;
            if (tagVal == 0)
                radioButton.IsChecked = true;
        }

        private void vacPumpOff_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Tag == null)
                return;

            int tagVal = (int)radioButton.Tag;
            if (tagVal == 0)
                radioButton.IsChecked = true;
        }
        #endregion

        #region Vacuum Set Control State Auto-Man
        private void vacSetAuto_Loaded(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Tag == null)
                return;

            int tagVal = (int)radioButton.Tag;
            if (tagVal == 1)
                radioButton.IsChecked = true;
        }

        private void vacSetAuto_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Tag == null)
                return;

            int tagVal = (int)radioButton.Tag;
            if (tagVal == 1)
                radioButton.IsChecked = true;
        }

        private async void vacSetAuto_Clicked(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null)
                return;

            // Set value to PLC
            int setValue = 1;
            bool plcResult = await _viewModel.SetToPlc(setValue, null, "vacuumSetControlStateAuto");

            if (plcResult == false)
                WinUIMessageBox.Show("Vacuum Pump Manuel komutu PLC'ye setlenemedi. Lütfen tekrar deneyiniz.", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void vacSetMan_Loaded(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            int tagVal = (int)radioButton.Tag;
            if (tagVal == 0)
                radioButton.IsChecked = true;
        }

        private void vacSetMan_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Tag == null)
                return;

            int tagVal = (int)radioButton.Tag;
            if (tagVal == 0)
                radioButton.IsChecked = true;
        }

        private async void vacSetMan_Clicked(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null)
                return;

            // Set value to PLC
            int setValue = 0;
            bool plcResult = await _viewModel.SetToPlc(setValue, null, "vacuumSetControlStateAuto");

            if (plcResult == false)
                WinUIMessageBox.Show("Vacuum Pump Manuel komutu PLC'ye setlenemedi. Lütfen tekrar deneyiniz.", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

            var spValueBgColor = spValue.Background;
            SolidColorBrush loadingYellowColor = (SolidColorBrush)Application.Current.Resources["LoadingYellowColor"];
            spValue.Background = loadingYellowColor;

            // Set to SP value to PLC
            //bool spTbPlcResult = await _viewModel.SetToPlc(0, null, "vacuumSetControlStateSp");
            //if (spTbPlcResult == false)
            //    WinUIMessageBox.Show("Vacuum Set Control SP değeri PLC'ye setlenemedi. Lütfen servislerinizi kontrol edip tekrar deneyiniz.", "Başarısız",
            //        MessageBoxButton.OK, MessageBoxImage.Warning);

            // Set back color to textbox
            spValue.Background = spValueBgColor;
        }
        #endregion

        #region Vacuum Set Control State SP Value
        private async void AddOrDecrease(int increaseAmount, string vacuumStateName)
        {
            var spValueBgColor = spValue.Background;
            SolidColorBrush loadingYellowColor = (SolidColorBrush)Application.Current.Resources["LoadingYellowColor"];
            spValue.Background = loadingYellowColor;
            _vacuumLinesView.StopTimer();

            string textboxVal = spValue.Text;

            if (string.IsNullOrEmpty(textboxVal))
                textboxVal = "0";

            float floatVal = float.Parse(textboxVal, CultureInfo.InvariantCulture.NumberFormat);

            // Increase value
            floatVal += increaseAmount;

            //if (floatVal < -760 || floatVal > 0)
            if (floatVal < _viewModel.VacuumMinValue || floatVal > _viewModel.VacuumMaxValue)
            {
                // Set back color to textbox
                spValue.Background = spValueBgColor;
                flowLayout.Focus();
                _vacuumLinesView.StartTimer();
                WinUIMessageBox.Show($"Girilen değer {_viewModel.VacuumMinValue}'dan büyük {_viewModel.VacuumMaxValue}'dan küçük olmalıdır.", "Başarısız",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            spValue.Text = floatVal.ToString();

            // Set to PLC
            bool plcResult = await _viewModel.SetToPlc(floatVal, null, vacuumStateName);

            // Set back color to textbox
            spValue.Background = spValueBgColor;

            if (plcResult == false)
                WinUIMessageBox.Show("Vacuum Set Control SP değeri PLC'ye setlenemedi. Lütfen servislerinizi kontrol edip tekrar deneyiniz.", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

            flowLayout.Focus();
            _vacuumLinesView.StartTimer();
        }

        private async void AddOrDecreaseRight(int increaseAmount, string vacuumStateName)
        {
            var spValueBgColor = spValueRight.Background;
            SolidColorBrush loadingYellowColor = (SolidColorBrush)Application.Current.Resources["LoadingYellowColor"];
            spValueRight.Background = loadingYellowColor;
            _vacuumLinesView.StopTimer();

            string textboxVal = spValueRight.Text;

            if (string.IsNullOrEmpty(textboxVal))
                textboxVal = "0";

            float floatVal = float.Parse(textboxVal, CultureInfo.InvariantCulture.NumberFormat);

            // Increase value
            floatVal += increaseAmount;

            //if (floatVal < -760 || floatVal > 0)
            if (floatVal < _viewModel.VacuumMinValue || floatVal > _viewModel.VacuumMaxValue)
            {
                // Set back color to textbox
                spValueRight.Background = spValueBgColor;
                flowLayout.Focus();
                _vacuumLinesView.StartTimer();
                WinUIMessageBox.Show($"Girilen değer {_viewModel.VacuumMinValue}'dan büyük {_viewModel.VacuumMaxValue}'dan küçük olmalıdır.", "Başarısız",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            spValueRight.Text = floatVal.ToString();

            // Set to PLC
            bool plcResult = await _viewModel.SetToPlc(floatVal, null, vacuumStateName);

            // Set back color to textbox
            spValueRight.Background = spValueBgColor;

            if (plcResult == false)
                WinUIMessageBox.Show("Vacuum Set Control SP değeri PLC'ye setlenemedi. Lütfen servislerinizi kontrol edip tekrar deneyiniz.", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

            flowLayout.Focus();
            _vacuumLinesView.StartTimer();
        }

        private void decreaseOne_Click(object sender, RoutedEventArgs e)
        {
            AddOrDecrease(-1, "vacuumSetControlStateSp");
        }

        private void decreaseTen_Click(object sender, RoutedEventArgs e)
        {
            AddOrDecrease(-10, "vacuumSetControlStateSp");
        }

        private void increaseOne_Click(object sender, RoutedEventArgs e)
        {
            AddOrDecrease(1, "vacuumSetControlStateSp");
        }

        private void increaseTen_Click(object sender, RoutedEventArgs e)
        {
            AddOrDecrease(10, "vacuumSetControlStateSp");
        }
        private async void spValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                _vacuumLinesView.StartTimer();
                return;
            }

            if (e.Key == Key.Enter)
            {
                var spValueBgColor = spValue.Background;
                SolidColorBrush loadingYellowColor = (SolidColorBrush)Application.Current.Resources["LoadingYellowColor"];
                spValue.Background = loadingYellowColor;

                _vacuumLinesView.StopTimer();

                string textboxVal = spValue.Text;

                if (string.IsNullOrEmpty(textboxVal))
                    textboxVal = "0";

                float floatVal = float.Parse(textboxVal, CultureInfo.InvariantCulture.NumberFormat);

                //if (floatVal < -760 || floatVal > 0)
                if (floatVal < _viewModel.VacuumMinValue || floatVal > _viewModel.VacuumMaxValue)
                {
                    // Set back color to textbox
                    spValue.Background = spValueBgColor;
                    flowLayout.Focus();
                    _vacuumLinesView.StartTimer();
                    WinUIMessageBox.Show($"Girilen değer {_viewModel.VacuumMinValue}'dan büyük {_viewModel.VacuumMaxValue}'dan küçük olmalıdır.", "Başarısız",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Set to PLC
                bool plcResult = await _viewModel.SetToPlc(floatVal, null, "vacuumSetControlStateSp");

                // Set back color to textbox
                spValue.Background = spValueBgColor;

                if (plcResult == false)
                    WinUIMessageBox.Show("Vacuum Set Control SP değeri PLC'ye setlenemedi. Lütfen servislerinizi kontrol edip tekrar deneyiniz.", "Başarısız",
                        MessageBoxButton.OK, MessageBoxImage.Warning);

                flowLayout.Focus();
                _vacuumLinesView.StartTimer();
            }

            if (e.Key == Key.Escape)
            {
                flowLayout.Focus();
                _vacuumLinesView.StartTimer();
            }
        }

        private void spValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            var change = e.Changes.FirstOrDefault();

            TextBox txtBox = (TextBox)sender;

            // dot and hyphen (-) added.
            Regex regex = new Regex(@"^[.0-9-]{0,7}$");

            Match match = regex.Match(txtBox.Text);

            if (!match.Success)
            {
                txtBox.Text = txtBox.Text.Remove(change.Offset, change.AddedLength);
            }
        }

        private void spValue_GotFocus(object sender, RoutedEventArgs e)
        {
            _vacuumLinesView.AllowUpdatingSpValue = false;
        }

        private void spValue_LostFocus(object sender, RoutedEventArgs e)
        {
            _vacuumLinesView.AllowUpdatingSpValue = true;
        }
        #endregion

        #region All Vacuum Lines Controls
        private async void vacRadioBtn_Clicked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            var vacResult = WinUIMessageBox.Show("Bütün vakum valflerini açmak istediğinize emin misiniz?", "Vakum",
                                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (vacResult == MessageBoxResult.No)
            {
                radioButton.IsChecked = false;
                return;
            }

            // Set value to the PLC
            radioButton.IsChecked = true;

            SiemensTagConfiguration port;
            if (radioButton.Tag != null)
                port = (SiemensTagConfiguration)radioButton.Tag;
            else
                throw new Exception("Tag value not found!");

            int setValue = 1;
            string customPortName = "All_VAC";

            bool plcResult = await _viewModel.SetToPlc(setValue, port, customPortName);

            if (plcResult == false)
                WinUIMessageBox.Show("Bütün vakum valflerini açma komutu PLC'ye setlenemedi. Lütfen tekrar deneyiniz.", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void ventRadioBtn_Clicked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            var ventResult = WinUIMessageBox.Show("Bütün vakum tahliye valflerini açmak istediğinize emin misiniz?", "Tahliye",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (ventResult == MessageBoxResult.No)
            {
                radioButton.IsChecked = false;
                return;
            }

            // Set value to the PLC
            radioButton.IsChecked = true;

            SiemensTagConfiguration port;
            if (radioButton.Tag != null)
                port = (SiemensTagConfiguration)radioButton.Tag;
            else
                throw new Exception("Tag value not found!");

            int setValue = 1;
            string customPortName = "All_VENT";

            bool plcResult = await _viewModel.SetToPlc(setValue, port, customPortName);

            if (plcResult == false)
                WinUIMessageBox.Show("Bütün vakum tahliye valflerini açma komutu PLC'ye setlenemedi. Lütfen tekrar deneyiniz.", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void offRadioBtn_Clicked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            var offResult = WinUIMessageBox.Show("Bütün valfleri kapatmak istediğinize emin misiniz?", "Kapatma",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (offResult == MessageBoxResult.No)
            {
                radioButton.IsChecked = false;
                return;
            }

            // Set value to the PLC
            radioButton.IsChecked = true;

            SiemensTagConfiguration port;
            if (radioButton.Tag != null)
                port = (SiemensTagConfiguration)radioButton.Tag;
            else
                throw new Exception("Tag value not found!");

            int setValue = 1;
            string customPortName = "All_OFF";

            bool plcResult = await _viewModel.SetToPlc(setValue, port, customPortName);

            if (plcResult == false)
                WinUIMessageBox.Show("Bütün vakum valfleri kapatma komutu PLC'ye setlenemedi. Lütfen tekrar deneyiniz.", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void manRadioBtn_Clicked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            var manResult = WinUIMessageBox.Show("Bütün portları manuel moda almak istediğinize emin misiniz?", "Manuel Mod",
            MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (manResult == MessageBoxResult.No)
            {
                radioButton.IsChecked = false;
                return;
            }

            // Set value to the PLC
            radioButton.IsChecked = true;

            SiemensTagConfiguration port;
            if (radioButton.Tag != null)
                port = (SiemensTagConfiguration)radioButton.Tag;
            else
                throw new Exception("Tag value not found!");

            int setValue = 1;
            string customPortName = "All_MANUAL";

            bool plcResult = await _viewModel.SetToPlc(setValue, port, customPortName);

            if (plcResult == false)
                WinUIMessageBox.Show("Bütün portları manuel moda alma komutu PLC'ye setlenemedi. Lütfen tekrar deneyiniz.", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private async void autoRadioBtn_Clicked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            var autoResult = WinUIMessageBox.Show("Bütün portları otomatik moda almak istediğinize emin misiniz?", "Otomatik Mod",
                            MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (autoResult == MessageBoxResult.No)
            {
                radioButton.IsChecked = false;
                return;
            }

            // Set value to the PLC
            radioButton.IsChecked = true;

            SiemensTagConfiguration port;
            if (radioButton.Tag != null)
                port = (SiemensTagConfiguration)radioButton.Tag;
            else
                throw new Exception("Tag value not found!");

            int setValue = 1;
            string customPortName = "All_AUTO";

            bool plcResult = await _viewModel.SetToPlc(setValue, port, customPortName);

            if (plcResult == false)
                WinUIMessageBox.Show("Bütün portları otomatik moda alma komutu PLC'ye setlenemedi. Lütfen tekrar deneyiniz.", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        #endregion

        #region Port Items Filtering
        private void activeItemsRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
                _viewModel.ApplyFiltering(true);
        }

        private void allItemsRadioBtn_Checked(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
                _viewModel.ApplyFiltering(false);
        }
        #endregion

        private void activeItemsRadioBtn_Loaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null)
                return;

            if(_viewModel.VacuumLinesSettings.GeneralFilter == GeneralFilterState.CurrentItems)
                activeItemsRadioBtn.IsChecked = true;
            else
                activeItemsRadioBtn.IsChecked = false;
        }

        private void allItemsRadioBtn_Loaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null)
                return;

            if (_viewModel.VacuumLinesSettings.GeneralFilter == GeneralFilterState.AllItems)
                allItemsRadioBtn.IsChecked = true;
            else
                allItemsRadioBtn.IsChecked = false;
        }

        private void vacPumpAutoRight_Loaded(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            int tagVal = (int)radioButton.Tag;
            if (tagVal == 1)
                radioButton.IsChecked = true;
        }

        private void vacPumpAutoRight_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Tag == null)
                return;

            int tagVal = (int)radioButton.Tag;
            if (tagVal == 1)
                radioButton.IsChecked = true;
        }

        private async void vacPumpManRight_Click(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            int tagVal = (int)radioButton.Tag;

            // Set value to PLC
            if (_viewModel == null)
                return;

            int setValue = 0;
            bool plcResult = await _viewModel.SetToPlc(setValue, null, "vacuumPumpControlStateAutoRight");

            if (plcResult == false)
                WinUIMessageBox.Show("Vacuum Pump Manuel komutu PLC'ye setlenemedi. Lütfen tekrar deneyiniz.", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void vacPumpManRight_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Tag == null)
                return;

            int tagVal = (int)radioButton.Tag;
            if (tagVal == 0)
                radioButton.IsChecked = true;
        }

        private async void vacPumpAutoRight_Click(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            int tagVal = (int)radioButton.Tag;

            // Set value to PLC
            if (_viewModel == null)
                return;

            int setValue = 1;
            bool plcResult = await _viewModel.SetToPlc(setValue, null, "vacuumPumpControlStateAutoRight");

            if (plcResult == false)
                WinUIMessageBox.Show("Vacuum Pump Auto komutu PLC'ye setlenemedi. Lütfen tekrar deneyiniz.", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void vacPumpManRight_Loaded(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Tag == null)
                return;

            int tagVal = (int)radioButton.Tag;
            if (tagVal == 0)
                radioButton.IsChecked = true;
        }

        private void vacPumpOnRight_Loaded(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            int tagVal = (int)radioButton.Tag;
            if (tagVal == 1)
                radioButton.IsChecked = true;
        }

        private async void vacPumpOnRight_Click(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            int tagVal = (int)radioButton.Tag;

            // Set value to PLC
            if (_viewModel == null)
                return;

            int setValue = 1;
            bool plcResult = await _viewModel.SetToPlc(setValue, null, "vacuumPumpControlStateOnOffRight");

            if (plcResult == false)
                WinUIMessageBox.Show("Vakum Pump State On konumuna alınıp PLC'ye setlenemedi. Lütfen tekrar deneyiniz.", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void vacPumpOnRight_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Tag == null)
                return;

            int tagVal = (int)radioButton.Tag;
            if (tagVal == 1)
                radioButton.IsChecked = true;
        }

        private async void vacPumpOffRight_Click(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            int tagVal = (int)radioButton.Tag;

            // Set value to PLC
            if (_viewModel == null)
                return;

            int setValue = 0;
            bool plcResult = await _viewModel.SetToPlc(setValue, null, "vacuumPumpControlStateOnOffRight");

            if (plcResult == false)
                WinUIMessageBox.Show("Vakum Pump State Off konumuna alınıp PLC'ye setlenemedi. Lütfen tekrar deneyiniz.", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void vacPumpOffRight_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Tag == null)
                return;

            int tagVal = (int)radioButton.Tag;
            if (tagVal == 0)
                radioButton.IsChecked = true;
        }

        private void vacPumpOffRight_Loaded(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Tag == null)
                return;

            int tagVal = (int)radioButton.Tag;
            if (tagVal == 0)
                radioButton.IsChecked = true;
        }

        private void vacSetAutoRight_Loaded(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Tag == null)
                return;

            int tagVal = (int)radioButton.Tag;
            if (tagVal == 1)
                radioButton.IsChecked = true;
        }

        private void vacSetAutoRight_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Tag == null)
                return;

            int tagVal = (int)radioButton.Tag;
            if (tagVal == 1)
                radioButton.IsChecked = true;
        }

        private async void vacSetAutoRight_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null)
                return;

            // Set value to PLC
            int setValue = 1;
            bool plcResult = await _viewModel.SetToPlc(setValue, null, "vacuumSetControlStateAutoRight");

            if (plcResult == false)
                WinUIMessageBox.Show("Vacuum Pump Manuel komutu PLC'ye setlenemedi. Lütfen tekrar deneyiniz.", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void vacSetManRight_Loaded(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            int tagVal = (int)radioButton.Tag;
            if (tagVal == 0)
                radioButton.IsChecked = true;
        }

        private void vacSetManRight_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Tag == null)
                return;

            int tagVal = (int)radioButton.Tag;
            if (tagVal == 0)
                radioButton.IsChecked = true;
        }

        private async void vacSetManRight_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null)
                return;

            // Set value to PLC
            int setValue = 0;
            bool plcResult = await _viewModel.SetToPlc(setValue, null, "vacuumSetControlStateAutoRight");

            if (plcResult == false)
                WinUIMessageBox.Show("Vacuum Pump Manuel komutu PLC'ye setlenemedi. Lütfen tekrar deneyiniz.", "Başarısız",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

            var spValueBgColor = spValueRight.Background;
            SolidColorBrush loadingYellowColor = (SolidColorBrush)Application.Current.Resources["LoadingYellowColor"];
            spValueRight.Background = loadingYellowColor;

            //// Set to SP value to PLC
            //bool spTbPlcResult = await _viewModel.SetToPlc(0, null, "vacuumSetControlStateSpRight");
            //if (spTbPlcResult == false)
            //    WinUIMessageBox.Show("Vacuum Set Control SP değeri PLC'ye setlenemedi. Lütfen servislerinizi kontrol edip tekrar deneyiniz.", "Başarısız",
            //        MessageBoxButton.OK, MessageBoxImage.Warning);

            // Set back color to textbox
            spValueRight.Background = spValueBgColor;
        }

        private void decreaseOneRight_Click(object sender, RoutedEventArgs e)
        {
            AddOrDecreaseRight(-1, "vacuumSetControlStateSpRight");
        }

        private void decreaseTenRight_Click(object sender, RoutedEventArgs e)
        {
            AddOrDecreaseRight(-10, "vacuumSetControlStateSpRight");
        }

        private async void spValueRight_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                _vacuumLinesView.StartTimer();
                return;
            }

            if (e.Key == Key.Enter)
            {
                var spValueBgColor = spValue.Background;
                SolidColorBrush loadingYellowColor = (SolidColorBrush)Application.Current.Resources["LoadingYellowColor"];
                spValueRight.Background = loadingYellowColor;

                _vacuumLinesView.StopTimer();

                string textboxVal = spValueRight.Text;

                if (string.IsNullOrEmpty(textboxVal))
                    textboxVal = "0";

                float floatVal = float.Parse(textboxVal, CultureInfo.InvariantCulture.NumberFormat);

                //if (floatVal < -760 || floatVal > 0)
                if (floatVal < _viewModel.VacuumMinValue || floatVal > _viewModel.VacuumMaxValue)
                {
                    // Set back color to textbox
                    spValueRight.Background = spValueBgColor;
                    flowLayout.Focus();
                    _vacuumLinesView.StartTimer();
                    WinUIMessageBox.Show($"Girilen değer {_viewModel.VacuumMinValue}'dan büyük {_viewModel.VacuumMaxValue}'dan küçük olmalıdır.", "Başarısız",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Set to PLC
                bool plcResult = await _viewModel.SetToPlc(floatVal, null, "vacuumSetControlStateSpRight");

                // Set back color to textbox
                spValueRight.Background = spValueBgColor;

                if (plcResult == false)
                    WinUIMessageBox.Show("Vacuum Set Control SP değeri PLC'ye setlenemedi. Lütfen servislerinizi kontrol edip tekrar deneyiniz.", "Başarısız",
                        MessageBoxButton.OK, MessageBoxImage.Warning);

                flowLayout.Focus();
                _vacuumLinesView.StartTimer();
            }

            if (e.Key == Key.Escape)
            {
                flowLayout.Focus();
                _vacuumLinesView.StartTimer();
            }
        }

        private void spValueRight_TextChanged(object sender, TextChangedEventArgs e)
        {
            var change = e.Changes.FirstOrDefault();

            TextBox txtBox = (TextBox)sender;

            // dot and hyphen (-) added.
            Regex regex = new Regex(@"^[.0-9-]{0,7}$");

            Match match = regex.Match(txtBox.Text);

            if (!match.Success)
            {
                txtBox.Text = txtBox.Text.Remove(change.Offset, change.AddedLength);
            }
        }

        private void spValueRight_GotFocus(object sender, RoutedEventArgs e)
        {
            _vacuumLinesView.AllowUpdatingSpValue = false;
        }

        private void spValueRight_LostFocus(object sender, RoutedEventArgs e)
        {
            _vacuumLinesView.AllowUpdatingSpValue = true;
        }

        private void increaseOneRight_Click(object sender, RoutedEventArgs e)
        {
            AddOrDecreaseRight(1, "vacuumSetControlStateSpRight");
        }

        private void increaseTenRight_Click(object sender, RoutedEventArgs e)
        {
            AddOrDecreaseRight(10, "vacuumSetControlStateSpRight");
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _viewModel.SetVacuumLinesDatablock(false);
        }
    }
}
