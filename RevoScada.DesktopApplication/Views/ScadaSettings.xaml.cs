using DevExpress.Xpf.Editors;
using DevExpress.Xpf.WindowsUI;
using Revo.Core;
using RevoScada.ProcessController;
using RevoScada.Configurator;
using RevoScada.DesktopApplication.Models.SettingModels;
using RevoScada.DesktopApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using RevoScada.DesktopApplication.Views.Popups;
using System.Linq;

namespace RevoScada.DesktopApplication.Views
{
    /// <summary>
    /// Interaction logic for ScadaSettings.xaml
    /// </summary>
    public partial class ScadaSettings : Window
    {
        private ScadaSettingsVM _viewModel;
        private DispatcherTimer _timer;
        private Dictionary<string, string> _serviceNames;
        private bool IsCommandWorking { get; set; } = false;

        public ScadaSettings()
        {
            _viewModel = new ScadaSettingsVM();
            DataContext = _viewModel;

            InitializeComponent();

            try
            {
                if (File.Exists(DesktopApplication.Properties.Settings.Default.InitialConfigurationFilePath))
                {
                    _serviceNames = ApplicationConfigurations.Instance.Configuration.ServiceNames;
                }
            }
            catch (Exception ex)
            {
                //todo:l logla açıkla durumu
            }
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ReportExportSettings applicationInitialSettings = _viewModel.GetApplicationInitialSettings();

                txtInitialConfigurationFileFullPath.Text = Properties.Settings.Default.InitialConfigurationFilePath;
                txtExcelExportFilePath.Text = _viewModel.ReportExportSettings.ExcelExportFilePath;
                txtExcelExportFileNameBase.Text = _viewModel.ReportExportSettings.ExcelExportFileNameBase;
                txtExcelExportPassword.Text = _viewModel.ReportExportSettings.ExcelExportPassword;

                if (File.Exists(Properties.Settings.Default.InitialConfigurationFilePath))
                {
                    if (!string.IsNullOrEmpty(Properties.Settings.Default.InitialConfigurationFilePath))
                    {
                        try
                        {
                            ComboEditAirTcSelection.SelectedIndex = _viewModel.AirTcSelections.IndexOf(_viewModel.AirTcSelections.First(x => x.IsSelected == true));
                        }
                        catch
                        {
                        }
                        _timer = new DispatcherTimer();
                        _timer.Interval = TimeSpan.FromMilliseconds(1000);
                        _timer.Tick += _timer_Tick;
                        _timer.Start();
                        await UpdateServiceState();
                        RightPanel.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        RightPanel.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    RightPanel.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Configuration file is not valid. Please select valid file!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            chkReadService.Checked += ChkService_Checked;
            chkReadService.Unchecked += ChkService_UnChecked;
            chkWriteService.Checked += ChkService_Checked;
            chkWriteService.Unchecked += ChkService_UnChecked;
            chkDataLoggerService.Checked += ChkService_Checked;
            chkDataLoggerService.Unchecked += ChkService_UnChecked;
            chkAlarmService.Checked += ChkService_Checked;
            chkAlarmService.Unchecked += ChkService_UnChecked;
            chkProcessManagerService.Checked += ChkService_Checked;
            chkProcessManagerService.Unchecked += ChkService_UnChecked;
            chkSyncService.Checked += ChkService_Checked;
            chkSyncService.Unchecked += ChkService_UnChecked;
            ComboEditAirTcSelection.SelectedIndexChanged += ComboEditAirTcSelection_SelectedIndexChanged;

            OscillationEnabledToggleControl.Checked += OscillationEnabledToggleControl_Checked;
            CascadeControlEnabledToggleControl.Checked += CascadeControlEnabledToggleControl_Checked; ;
            OscillationEnabledToggleControl.Unchecked += OscillationEnabledToggleControl_Unchecked;
            CascadeControlEnabledToggleControl.Unchecked += CascadeControlEnabledToggleControl_Unchecked;

        }

        private void CascadeControlEnabledToggleControl_Unchecked(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveFurnaceProcessSettingstoPLC("cascadeControlEnabled", 0);
        }

        private void OscillationEnabledToggleControl_Unchecked(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveFurnaceProcessSettingstoPLC("oscillationEnabled", 0);
        }

        private void CascadeControlEnabledToggleControl_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveFurnaceProcessSettingstoPLC("cascadeControlEnabled", 1);
        }

        private void OscillationEnabledToggleControl_Checked(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveFurnaceProcessSettingstoPLC("oscillationEnabled", 1);
        }

        private void ComboEditAirTcSelection_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                AirTcSelection airTcSelection = ((AirTcSelection)ComboEditAirTcSelection.SelectedItem);
                _viewModel.SaveAirTcSelections(airTcSelection);
            }
            catch
            {
            }
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            _viewModel.UpdateStatusGridAsync();
        }

        #region CheckServiceEvents
        private async void ChkService_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsCommandWorking)
            {

                ToggleSwitch changedToggleSwitch = sender as ToggleSwitch;
                changedToggleSwitch.IsEnabled = false;
                btnRefreshServices.IsEnabled = false;
                ServiceManager serviceManager = new ServiceManager();

                await serviceManager.StartService(_serviceNames[changedToggleSwitch.CommandParameter.ToString()], 500);



                changedToggleSwitch.IsEnabled = true;
                btnRefreshServices.IsEnabled = true;

            }
        }
        private async void ChkService_UnChecked(object sender, RoutedEventArgs e)
        {
            if (!IsCommandWorking)
            {
                ToggleSwitch changedToggleSwitch = sender as ToggleSwitch;
                changedToggleSwitch.IsEnabled = false;
                btnRefreshServices.IsEnabled = false;
                ServiceManager serviceManager = new ServiceManager();

                await serviceManager.StopService(_serviceNames[changedToggleSwitch.CommandParameter.ToString()], 500);


                changedToggleSwitch.IsEnabled = true;
                btnRefreshServices.IsEnabled = true;
            }
        }
        private async Task UpdateServiceState()
        {
            try
            {

                btnRefreshServices.IsEnabled = false;
                IsCommandWorking = true;

                chkReadService.IsEnabled = false;
                chkWriteService.IsEnabled = false;
                chkDataLoggerService.IsEnabled = false;
                chkAlarmService.IsEnabled = false;
                chkProcessManagerService.IsEnabled = false;
                chkSyncService.IsEnabled = false;

                var serviceState = ProcessManager.Instance.ServiceStates();

                await Dispatcher.InvokeAsync(() =>
                {
                    chkReadService.IsChecked = serviceState["ReadService"];
                    chkWriteService.IsChecked = serviceState["WriteService"];
                    chkDataLoggerService.IsChecked = serviceState["DataLoggerService"];
                    chkAlarmService.IsChecked = serviceState["AlarmService"];
                    chkProcessManagerService.IsChecked = serviceState["ProcessManagerService"];
                    chkSyncService.IsChecked = serviceState["SynchronizationService"];
                });

                await Task.Delay(2000);

                IsCommandWorking = false;
                btnRefreshServices.IsEnabled = true;

                chkReadService.IsEnabled = true;

                chkWriteService.IsEnabled = true;
                chkDataLoggerService.IsEnabled = true;
                chkAlarmService.IsEnabled = true;
                chkProcessManagerService.IsEnabled = true;
                chkSyncService.IsEnabled = true;

            }
            catch (Exception)
            {
            }
        }
        private async void btnRefreshServices_Click(object sender, RoutedEventArgs e)
        {
            await UpdateServiceState();
        }

        #endregion

        #region GeneralSettingEvents
        private void txtExcelExportFilePath_DefaultButtonClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                txtExcelExportFilePath.Text = folderBrowserDialog.SelectedPath;

                _viewModel.ReportExportSettings.ExcelExportFilePath = folderBrowserDialog.SelectedPath;
                _viewModel.SaveApplicationInitialSettings();
            }
            else
            {
                return;
            }
        }
        private void txtInitialConfigurationFileFullPath_DefaultButtonClick(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            System.Windows.Forms.DialogResult result = openFileDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                txtInitialConfigurationFileFullPath.Text = openFileDialog.FileName;
            }
            else
            {
                return;
            }

            Properties.Settings.Default.InitialConfigurationFilePath = txtInitialConfigurationFileFullPath.Text;

            try
            {
                ApplicationConfigurations.Instance.InitializeConfiguration(Properties.Settings.Default.InitialConfigurationFilePath, true);
                Properties.Settings.Default.Save();
                WinUIMessageBox.Show("Application need restart!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.Shutdown();
            }
            catch
            {
                WinUIMessageBox.Show("Configuration file is not valid. Please select valid file!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void txtExcelExportFileNameBase_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            _viewModel.ReportExportSettings.ExcelExportFileNameBase = txtExcelExportFileNameBase.Text;
            _viewModel.SaveApplicationInitialSettings();
        }
        private void txtExcelExportPassword_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            _viewModel.ReportExportSettings.ExcelExportPassword = txtExcelExportPassword.Text;
            _viewModel.SaveApplicationInitialSettings();
        }

        #endregion

        private void LayoutItem_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.ReadServiceStateInfo = "Loading...";
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_timer != null)
                _timer.Stop();
        }

        private void ButonUpdateLoadSerialNumber_Click(object sender, RoutedEventArgs e)
        {
            var winUIDialogWindow = new WinUIDialogWindow("", MessageBoxButton.OKCancel);
            winUIDialogWindow.Content = new LoadNumberEditorPopup(ApplicationConfigurations.Instance.Configuration, ApplicationConfigurations.Instance.Configuration.Furnace.Id);
            winUIDialogWindow.ShowDialogWindow();
        }
    }
}

